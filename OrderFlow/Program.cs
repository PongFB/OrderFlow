using Microsoft.EntityFrameworkCore;
using OrderFlow.Models;
using OrderFlow.Persistence;

using var db = new OrderFlowContext();
await db.Database.MigrateAsync();
await DatabaseSeeder.SeedAsync(db);

Console.WriteLine("==============================================");
Console.WriteLine("          CZĘŚĆ 1: ZAAWANSOWANE LINQ          ");
Console.WriteLine("==============================================\n");

decimal progVip = 200m;
var vipOrders = await db.Orders
    .Where(o => o.Customer.IsVip && o.Pozycje.Sum(p => p.ZamowionaIlosc * p.UnitPrice) > progVip)
    .Include(o => o.Customer)
    .ToListAsync();

Console.WriteLine($"1. Zamówienia VIP powyżej {progVip} PLN:");
foreach (var o in vipOrders)
{
    Console.WriteLine($"   Zamówienie #{o.Id} (Klient: {o.Customer.Nazwa})");
}

var ranking = await db.Orders
    .GroupBy(o => o.Customer)
    .Select(g => new
    {
        CustomerName = g.Key.Nazwa,
        TotalSpent = g.Sum(o => o.Pozycje.Sum(p => p.ZamowionaIlosc * p.UnitPrice))
    })

    .OrderByDescending(r => (double)r.TotalSpent)
    .ToListAsync();

Console.WriteLine("\n2. Ranking klientów (Top wydatki):");
foreach (var r in ranking)
{
    Console.WriteLine($"   {r.CustomerName}: {r.TotalSpent} PLN");
}

var avgPerCity = await db.Orders
    .Select(o => new
    {
        City = o.Customer.City,

        OrderTotal = o.Pozycje.Sum(p => p.ZamowionaIlosc * (double)p.UnitPrice)
    })
    .GroupBy(x => x.City)
    .Select(g => new
    {
        City = g.Key,
        AvgOrderValue = g.Average(x => x.OrderTotal)
    })
    .ToListAsync();

Console.WriteLine("\n3. Średnia wartość zamówienia per miasto:");
foreach (var c in avgPerCity)
{
    Console.WriteLine($"   {c.City}: {c.AvgOrderValue:F2} PLN");
}

var unlovedProducts = await db.Products
    .Where(p => !p.OrderItems.Any())
    .ToListAsync();

Console.WriteLine("\n4. Produkty nigdy niezamówione:");
foreach (var p in unlovedProducts)
{
    Console.WriteLine($"   {p.Nazwa}");
}

OrderStatus? filterStatus = OrderStatus.Completed;
decimal minAmount = 100m;

IQueryable<Order> query = db.Orders.Include(o => o.Customer);

if (filterStatus.HasValue)
{
    query = query.Where(o => o.Status == filterStatus.Value);
}
if (minAmount > 0)
{
    query = query.Where(o => o.Pozycje.Sum(p => p.ZamowionaIlosc * p.UnitPrice) >= minAmount);
}

var dynResults = await query.ToListAsync();
Console.WriteLine($"\n5. Dynamiczne zapytanie (Status: {filterStatus}, Min: {minAmount}): Znaleziono {dynResults.Count} zamówień.\n");


Console.WriteLine("==============================================");
Console.WriteLine("           CZĘŚĆ 2: TRANSAKCJE BAZY           ");
Console.WriteLine("==============================================\n");

var klientDoSukcesu = await db.Customers.FirstAsync();
var dostepnyProdukt = await db.Products.FirstAsync();
var zamowienieSukces = new Order(klientDoSukcesu.Id) { Status = OrderStatus.New };
zamowienieSukces.DodajPozycje(new OrderItem(dostepnyProdukt.Id, 1, dostepnyProdukt.Cena));
db.Orders.Add(zamowienieSukces);
await db.SaveChangesAsync();

Console.WriteLine("[TEST 1] Uruchamianie poprawnego zamówienia...");
await ProcessOrderAsync(db, zamowienieSukces.Id);

var zamowieniePorazka = new Order(klientDoSukcesu.Id) { Status = OrderStatus.New };
zamowieniePorazka.DodajPozycje(new OrderItem(dostepnyProdukt.Id, 999, dostepnyProdukt.Cena));
db.Orders.Add(zamowieniePorazka);
await db.SaveChangesAsync();

Console.WriteLine("\n[TEST 2] Uruchamianie zamówienia bez wystarczającej ilości towaru...");
await ProcessOrderAsync(db, zamowieniePorazka.Id);

static async Task ProcessOrderAsync(OrderFlowContext db, int orderId)
{
    using var transaction = await db.Database.BeginTransactionAsync();

    try
    {
        var order = await db.Orders
            .Include(o => o.Pozycje)
                .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) throw new Exception("Nie znaleziono zamówienia.");

        order.Status = OrderStatus.Processing;
        await db.SaveChangesAsync();

        foreach (var item in order.Pozycje)
        {
            if (item.Product.Ilosc < item.ZamowionaIlosc)
            {
                throw new Exception($"Brak w magazynie: {item.Product.Nazwa}. Chcesz: {item.ZamowionaIlosc}, Dostępne: {item.Product.Ilosc}");
            }
            item.Product.Ilosc -= item.ZamowionaIlosc;
        }

        order.Status = OrderStatus.Completed;
        await db.SaveChangesAsync();

        await transaction.CommitAsync();
        Console.WriteLine($" => [SUKCES] Zamówienie #{orderId} przetworzone pomyślnie. Stany magazynowe zostały zmniejszone.");
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        Console.WriteLine($" => [PORAŻKA - ROLLBACK] Wycofano transakcję zamówienia #{orderId}.");
        Console.WriteLine($" => Powód: {ex.Message}");
    }
}