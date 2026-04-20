using System.Xml.Linq;
using OrderFlow.Models;

namespace OrderFlow.Persistence;

public class XmlReportBuilder
{
    public XDocument BuildReport(IEnumerable<Order> orders)
    {
        var report = new XDocument(
            new XElement("report",
                new XAttribute("generated", DateTime.Now.ToString("s")),

                new XElement("summary",
                    new XAttribute("totalOrders", orders.Count()),
                    new XAttribute("totalRevenue", orders.Sum(o => o.TotalAmount))
                ),

                new XElement("byStatus",
                    from order in orders
                    group order by order.Status into statusGroup
                    select new XElement("status",
                        new XAttribute("name", statusGroup.Key.ToString()),
                        new XAttribute("count", statusGroup.Count()),
                        new XAttribute("revenue", statusGroup.Sum(o => o.TotalAmount))
                    )
                ),

                new XElement("byCustomer",
                    from order in orders
                    group order by order.Klient into customerGroup
                    select new XElement("customer",
                        new XAttribute("id", customerGroup.Key.Id),
                        new XAttribute("name", customerGroup.Key.Nazwa),
                        new XAttribute("isVip", customerGroup.Key.IsVip),
                        new XElement("orderCount", customerGroup.Count()),
                        new XElement("totalSpent", customerGroup.Sum(o => o.TotalAmount)),

                        new XElement("orders",
                            from customerOrder in customerGroup
                            select new XElement("orderRef",
                                new XAttribute("id", customerOrder.Id),
                                new XAttribute("total", customerOrder.TotalAmount)
                            )
                        )
                    )
                )
            )
        );

        return report;
    }

    public async Task SaveReportAsync(XDocument report, string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(path);
        await report.SaveAsync(stream, SaveOptions.None, CancellationToken.None);
    }

    public async Task<IEnumerable<int>> FindHighValueOrderIdsAsync(string reportPath, decimal threshold)
    {
        if (!File.Exists(reportPath)) return Enumerable.Empty<int>();

        await using var stream = File.OpenRead(reportPath);
        var doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

        var highValueIds = doc.Descendants("orderRef")
                              .Where(element => (decimal)element.Attribute("total")! > threshold)
                              .Select(element => (int)element.Attribute("id")!)
                              .ToList();

        return highValueIds;
    }
}