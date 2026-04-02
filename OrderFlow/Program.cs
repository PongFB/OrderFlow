using System.Diagnostics;
using OrderFlow.Data;
using OrderFlow.Services;

Console.WriteLine("=== LAB 2 | ZADANIE 2: ASYNCHRONICZNOŚĆ ===\n");

var orders = SampleData.Orders.Where(o => o.Pozycje.Count > 0).ToList();
var externalServices = new ExternalServiceSimulator();
var sw = new Stopwatch();

Console.WriteLine("--- 1. Przetwarzanie SEKWENCYJNE (po kolei) ---");
sw.Start();
foreach (var order in orders)
{
    await externalServices.ProcessOrderAsync(order);
}
sw.Stop();
var sekwencyjnieCzas = sw.ElapsedMilliseconds;


Console.WriteLine("\n--- 2. Przetwarzanie RÓWNOLEGŁE (max 3 na raz) ---");
sw.Restart();
await externalServices.ProcessMultipleOrdersAsync(orders);
sw.Stop();
var rownolegleCzas = sw.ElapsedMilliseconds;

Console.WriteLine("\n=== PODSUMOWANIE CZASOWE ===");
Console.WriteLine($"Czas sekwencyjnie: {sekwencyjnieCzas} ms");
Console.WriteLine($"Czas równolegle:   {rownolegleCzas} ms");

Console.ReadLine();