using OrderFlow.Models;
using OrderFlow.Data;
using OrderFlow.Services;

Console.WriteLine("=== LAB 2 | ZADANIE 1: ZDARZENIA ===");

var pipeline = new OrderPipeline();

pipeline.StatusChanged += (sender, e) =>
{
    Console.WriteLine($"[LOG {e.Timestamp:HH:mm:ss}] Zlecenie #{e.Order.Id}: {e.OldStatus} -> {e.NewStatus}");
};

pipeline.StatusChanged += (sender, e) =>
{
    if (e.NewStatus == OrderStatus.Completed)
    {
        Console.WriteLine($"[EMAIL] Wysłano maila do {e.Order.Klient.Email}: 'Twoje zamówienie zostało zrealizowane!'");
    }
};

pipeline.ValidationCompleted += (sender, e) =>
{
    if (e.IsValid)
    {
        Console.WriteLine($"[STATYSTYKI] Zamówienie #{e.Order.Id} przeszło walidację poprawnie.");
    }
    else
    {
        Console.WriteLine($"[STATYSTYKI] Zamówienie #{e.Order.Id} odrzucone. Powód: {string.Join(", ", e.Errors)}");
    }
};

var poprawneZamowienie = SampleData.Orders[2];

var pusteZamowienie = new Order(99, SampleData.Customers[0]);

Console.WriteLine("\n--- Przetwarzam poprawne zamówienie ---");
pipeline.ProcessOrder(poprawneZamowienie);

Console.WriteLine("\n--- Przetwarzam puste zamówienie ---");
pipeline.ProcessOrder(pusteZamowienie);

Console.ReadLine();