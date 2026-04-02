using OrderFlow.Data;
using OrderFlow.Services;

Console.WriteLine("=== LAB 2 | ZADANIE 3: THREAD SAFETY ===\n");

var massiveOrderList = new List<Order>();
for (int i = 0; i < 100_000; i++)
{
    massiveOrderList.AddRange(SampleData.Orders);
}

int expectedCount = massiveOrderList.Count;
decimal expectedRevenue = 0;
foreach (var o in massiveOrderList) expectedRevenue += o.TotalAmount;

Console.WriteLine($"Oczekiwana liczba zamówień: {expectedCount}");
Console.WriteLine($"Oczekiwany łączny przychód:  {expectedRevenue:C}\n");

var stats = new OrderStatistics();

Console.WriteLine("--- 1. Przetwarzanie RÓWNOLEGŁE (Bez synchronizacji) ---");
stats.CalculateUnsafe(massiveOrderList);

Console.WriteLine($"Otrzymana liczba zamówień:   {stats.TotalProcessed} ❌ (Zgubiono: {expectedCount - stats.TotalProcessed})");
Console.WriteLine($"Otrzymany łączny przychód:   {stats.TotalRevenue:C} ❌");

stats.Reset();

Console.WriteLine("\n--- 2. Przetwarzanie RÓWNOLEGŁE (Bezpieczne - lock & Interlocked) ---");
stats.CalculateSafe(massiveOrderList);

Console.WriteLine($"Otrzymana liczba zamówień:   {stats.TotalProcessed} ✅ (Zgubiono: 0)");
Console.WriteLine($"Otrzymany łączny przychód:   {stats.TotalRevenue:C} ✅");
Console.WriteLine("\nPodział na statusy (ConcurrentDictionary):");
foreach (var status in stats.SafeOrdersPerStatus)
{
    Console.WriteLine($" - {status.Key}: {status.Value}");
}

Console.ReadLine();