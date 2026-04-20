using OrderFlow.Data;
using OrderFlow.Persistence;

Console.WriteLine("=== LAB 3 | ZADANIE 2: RAPORT LINQ TO XML ===\n");

string appDir = AppContext.BaseDirectory;
string reportPath = Path.Combine(appDir, "data", "report.xml");

var reportBuilder = new XmlReportBuilder();

var orginalneZamowienia = SampleData.Orders;

Console.WriteLine("Generowanie drzewka XML za pomocą LINQ to XML");
var xmlReport = reportBuilder.BuildReport(orginalneZamowienia);

Console.WriteLine($"Zapis do: {reportPath}");
await reportBuilder.SaveReportAsync(xmlReport, reportPath);

Console.WriteLine("\nPodgląd raportu w pamięci:");
Console.WriteLine(xmlReport.Root?.Element("summary"));

decimal progKwotowy = 1000m;
Console.WriteLine($"\nSzukanie zamówień > {progKwotowy:C}");

var highValueOrders = await reportBuilder.FindHighValueOrderIdsAsync(reportPath, progKwotowy);

Console.WriteLine($"Znaleziono zamówienia ({highValueOrders.Count()} szt.):");
foreach (var id in highValueOrders)
{
    Console.WriteLine($" -> Zamówienie #{id}");
}

Console.ReadLine();