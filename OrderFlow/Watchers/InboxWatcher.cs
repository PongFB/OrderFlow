using OrderFlow.Persistence;
using OrderFlow.Services;

namespace OrderFlow.Watchers;

public class InboxWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private readonly OrderPipeline _pipeline;
    private readonly OrderRepository _repository;
    private readonly string _inboxPath;
    private readonly string _processedPath;
    private readonly string _failedPath;

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(2, 2);

    public InboxWatcher(string inboxPath, OrderPipeline pipeline, OrderRepository repository)
    {
        _inboxPath = inboxPath;
        _pipeline = pipeline;
        _repository = repository;

        _processedPath = Path.Combine(_inboxPath, "processed");
        _failedPath = Path.Combine(_inboxPath, "failed");

        Directory.CreateDirectory(_inboxPath);
        Directory.CreateDirectory(_processedPath);
        Directory.CreateDirectory(_failedPath);

        _watcher = new FileSystemWatcher(_inboxPath, "*.json")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };

        _watcher.Created += OnCreated;
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        string fileName = e.Name ?? Path.GetFileName(e.FullPath);
        Task.Run(() => ProcessFileAsync(e.FullPath, fileName));
    }

    private async Task ProcessFileAsync(string fullPath, string fileName)
    {
        await _semaphore.WaitAsync();
        try
        {
            Console.WriteLine($"Wykryto plik: {fileName}. Oczekiwanie na dostęp...");

            string content = await ReadFileWithRetry(fullPath);

            var orders = await _repository.LoadFromJsonAsync(fullPath);
            Console.WriteLine($"Importowanie {orders.Count} zamówień z {fileName}...");

            foreach (var order in orders)
            {
                await _pipeline.ProcessOrderAsync(order);
            }

            MoveFile(fullPath, Path.Combine(_processedPath, fileName));
            Console.WriteLine($"Sukces: {fileName} przeniesiony do /processed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd pliku {fileName}: {ex.Message}");
            HandleFailure(fullPath, fileName, ex.Message);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<string> ReadFileWithRetry(string path, int maxRetries = 5)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                return "ready";
            }
            catch (IOException)
            {
                await Task.Delay(300);
            }
        }
        throw new IOException($"Nie można uzyskać dostępu do pliku {path} po {maxRetries} próbach.");
    }

    private void MoveFile(string source, string destination)
    {
        if (File.Exists(destination)) File.Delete(destination);
        File.Move(source, destination);
    }

    private void HandleFailure(string fullPath, string fileName, string errorMessage)
    {
        try
        {
            string dest = Path.Combine(_failedPath, fileName);
            MoveFile(fullPath, dest);
            File.WriteAllText(dest + ".error.txt", errorMessage);
        }
        catch {}
    }

    public void Dispose()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _semaphore.Dispose();
    }
}