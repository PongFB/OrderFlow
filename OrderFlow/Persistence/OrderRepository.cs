using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Serialization;
using OrderFlow.Models;

namespace OrderFlow.Persistence;

public class OrderRepository
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public async Task SaveToJsonAsync(IEnumerable<Order> orders, string path)
    {
        EnsureDirectoryExists(path);
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, orders, _jsonOptions);
    }

    public async Task<List<Order>> LoadFromJsonAsync(string path)
    {
        if (!File.Exists(path))
        {
            return new List<Order>();
        }

        try
        {
            await using var stream = File.OpenRead(path);
            var orders = await JsonSerializer.DeserializeAsync<List<Order>>(stream, _jsonOptions);
            return orders ?? new List<Order>();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[JSON Error] Błąd I/O: {ex.Message}");
            return new List<Order>();
        }
    }

    public async Task SaveToXmlAsync(IEnumerable<Order> orders, string path)
    {
        EnsureDirectoryExists(path);
        var serializer = new XmlSerializer(typeof(List<Order>));

        await Task.Run(() =>
        {
            using var writer = new StreamWriter(path);
            serializer.Serialize(writer, orders);
        });
    }

    public async Task<List<Order>> LoadFromXmlAsync(string path)
    {
        if (!File.Exists(path))
        {
            return new List<Order>();
        }

        try
        {
            var serializer = new XmlSerializer(typeof(List<Order>));
            return await Task.Run(() =>
            {
                using var reader = new StreamReader(path);
                var orders = (List<Order>?)serializer.Deserialize(reader);
                return orders ?? new List<Order>();
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[XML Error] Błąd: {ex.Message}");
            return new List<Order>();
        }
    }

    private void EnsureDirectoryExists(string path)
    {
        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}