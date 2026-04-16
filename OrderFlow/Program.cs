using OrderFlow.Data;
using OrderFlow.Persistence;

Console.WriteLine("=== LAB 3 | ZADANIE 1: REPOZYTORIUM ZAMÓWIEŃ (JSON & XML) ===\n");

string appDir = AppContext.BaseDirectory;
string jsonPath = Path.Combine(appDir, "data", "orders.json");
string xmlPath = Path.Combine(appDir, "data", "orders.xml");

var repository = new OrderRepository();

var orginalneZamowienia = SampleData.Orders;
Console.WriteLine($"[START] Posiadamy {orginalneZamowienia.Count} zamówień testowych.");

Console.WriteLine("\n[ZAPIS] Zapisuję zamówienia do plików...");
await repository.SaveToJsonAsync(orginalneZamowienia, jsonPath);
await repository.SaveToXmlAsync(orginalneZamowienia, xmlPath);
Console.WriteLine($" -> Zapisano JSON: {jsonPath}");
Console.WriteLine($" -> Zapisano XML: {xmlPath}");

orginalneZamowienia = null;
Console.WriteLine("\n[RESET] Wyczyszczono pamięć RAM (Zamówienia = null).");

Console.WriteLine("\n[ODCZYT] Wczytuję dane z dysku...");
var wczytaneZJson = await repository.LoadFromJsonAsync(jsonPath);
var wczytaneZXml = await repository.LoadFromXmlAsync(xmlPath);

Console.WriteLine("\n=== WYNIKI ROUND-TRIP ===");
Console.WriteLine($"Z JSON wczytano: {wczytaneZJson.Count} zamówień");
Console.WriteLine($"Z XML wczytano:  {wczytaneZXml.Count} zamówień");

Console.ReadLine();