using System.Collections.Concurrent;
using OrderFlow.Models;

namespace OrderFlow.Services;

public class OrderStatistics
{
    public int TotalProcessed = 0;
    public decimal TotalRevenue = 0m;

    public Dictionary<OrderStatus, int> UnsafeOrdersPerStatus = new();
    public List<string> ProcessingErrors = new();

    public ConcurrentDictionary<OrderStatus, int> SafeOrdersPerStatus = new();

    private readonly object _revenueLock = new object();
    private readonly object _errorsLock = new object();

    public void Reset()
    {
        TotalProcessed = 0;
        TotalRevenue = 0m;
        UnsafeOrdersPerStatus.Clear();
        SafeOrdersPerStatus.Clear();
        ProcessingErrors.Clear();
    }

    public void CalculateUnsafe(List<Order> orders)
    {
        Parallel.ForEach(orders, order =>
        {
            TotalProcessed++;

            TotalRevenue += order.TotalAmount;

            if (order.Pozycje.Count == 0)
            {
                ProcessingErrors.Add($"Błąd: Puste zamówienie #{order.Id}");
            }
        });
    }

    public void CalculateSafe(List<Order> orders)
    {
        Parallel.ForEach(orders, order =>
        {
            Interlocked.Increment(ref TotalProcessed);

            lock (_revenueLock)
            {
                TotalRevenue += order.TotalAmount;
            }

            if (order.Pozycje.Count == 0)
            {
                lock (_errorsLock)
                {
                    ProcessingErrors.Add($"Błąd: Puste zamówienie #{order.Id}");
                }
            }

            SafeOrdersPerStatus.AddOrUpdate(
                order.Status,
                1,
                (status, obecnaWartosc) => obecnaWartosc + 1
            );
        });
    }
}