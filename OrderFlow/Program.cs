using Microsoft.EntityFrameworkCore;
using OrderFlow.Models;
using OrderFlow.Persistence;

Console.WriteLine("URUCHAMIAM ORDERFLOW (EF CORE CRUD)\n");

using var db = new OrderFlowContext();

await db.Database.MigrateAsync();

await DatabaseSeeder.SeedAsync(db);

Console.WriteLine("OPERACJE CRUD\n");

var klient = await db.Customers.FirstAsync();
var produkt1 = await db.Products.FirstAsync();

var noweZamowienie = new Order(klient.Id) { Notes = "Test operacji CREATE" };
noweZamowienie.DodajPozycje(new OrderItem(produkt1.Id, 3, produkt1.Cena));

db.Orders.Add(noweZamowienie);
await db.SaveChangesAsync();
Console.WriteLine($"[CREATE] Dodano zamówienie #{noweZamowienie.Id} dla klienta {klient.Nazwa}.\n");

var pobraneZamowienie = await db.Orders
    .Include(o => o.Customer)
    .Include(o => o.Pozycje)
        .ThenInclude(p => p.Product)
    .FirstAsync(o => o.Id == noweZamowienie.Id);

Console.WriteLine($"[READ] Szczegóły pobranego zamówienia #{pobraneZamowienie.Id}:");
Console.WriteLine($"  Klient: {pobraneZamowienie.Customer.Nazwa}");
Console.WriteLine($"  Status: {pobraneZamowienie.Status}");
foreach (var pozycja in pobraneZamowienie.Pozycje)
{
    Console.WriteLine($"  -> {pozycja.Product.Nazwa} (Ilość: {pozycja.ZamowionaIlosc}, Cena za szt.: {pozycja.UnitPrice} PLN)");
}
Console.WriteLine();

var zamowienieDoAktualizacji = await db.Orders.FindAsync(noweZamowienie.Id);
if (zamowienieDoAktualizacji != null)
{
    zamowienieDoAktualizacji.Status = OrderStatus.Processing;
    zamowienieDoAktualizacji.Notes = "Zaktualizowano status w operacji UPDATE!";

    await db.SaveChangesAsync();
    Console.WriteLine($"[UPDATE] Zmieniono status zamówienia #{zamowienieDoAktualizacji.Id} na {zamowienieDoAktualizacji.Status}.\n");
}

var klientDoUsuniecia = await db.Customers.FirstAsync();
Console.WriteLine($"[DELETE] Próba usunięcia klienta '{klientDoUsuniecia.Nazwa}' z bazy...");

try
{
    db.Customers.Remove(klientDoUsuniecia);
    await db.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    Console.WriteLine("[DELETE-ZABLOKOWANE] Baza danych zablokowała usunięcie.");
    Console.WriteLine($"Treść błędu: {ex.InnerException?.Message}\n");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine("[DELETE-ZABLOKOWANE] EF Core zablokowało usunięcie klienta.");
    Console.WriteLine($"Treść błędu EF Core: {ex.Message}\n");
}