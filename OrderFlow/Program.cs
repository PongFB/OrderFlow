using OrderFlow.Data;
using OrderFlow.Persistence;
using OrderFlow.Services;
using OrderFlow.Watchers;

Console.WriteLine("LAB 3 | ZADANIE 3: AUTOMATYCZNY IMPORT\n");

string appDir = AppContext.BaseDirectory;
string inboxPath = Path.Combine(appDir, "inbox");

var repository = new OrderRepository();
var pipeline = new OrderPipeline();

pipeline.StatusChanged += (s, e) =>
    Console.WriteLine($"Zamówienie #{e.Order.Id} zmieniło status: {e.OldStatus} -> {e.NewStatus}");

using var watcher = new InboxWatcher(inboxPath, pipeline, repository);
Console.WriteLine("Watcher uruchomiony.");

var simulationTask = Task.Run(async () =>
{
    for (int i = 1; i <= 3; i++)
    {
        await Task.Delay(3000);
        string fileName = $"auto_import_{i}.json";
        string filePath = Path.Combine(inboxPath, fileName);

        Console.WriteLine($"\nGeneruję nowy plik do importu: {fileName}");
        await repository.SaveToJsonAsync(SampleData.Orders, filePath);
    }
});

Console.WriteLine("Naciśnij ENTER, aby zakończyć działanie programu");
Console.ReadLine();