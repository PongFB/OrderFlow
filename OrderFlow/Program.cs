using OrderFlow.Models;
using OrderFlow.Data;
using OrderFlow.Services;

var orders = SampleData.Orders;
var processor = new OrderProcessor();

Predicate<Order> isCompleted = o => o.Status == OrderStatus.Completed;
Predicate<Order> isHighValue = o => o.TotalAmount > 1000m;
Predicate<Order> hasManyItems = o => o.Pozycje.Sum(p => p.ZamowionaIlosc) >= 3;

var completedOrders = processor.FilterOrders(orders, isCompleted);

Console.WriteLine($"\nZakończonych zamówień: {completedOrders.Count}");

Action<Order> printOrder = o => Console.WriteLine($" - Zamówienie #{o.Id} | Kwota: {o.TotalAmount:C}");
Action<Order> markAsProcessing = o => {
    if (o.Status == OrderStatus.New) o.ZmienStatus(OrderStatus.Processing);
};

Console.WriteLine("\nWypisywanie zakończonych zamówień (Action):");

processor.ProcessOrders(completedOrders, printOrder);

Func<Order, object> toAnonymousType = o => new {
    Numer = o.Id,
    Klient = o.Klient.Nazwa,
    Suma = o.TotalAmount
};

var projections = processor.TransformOrders(orders, toAnonymousType);
Console.WriteLine("\nProjekcja pierwszego zamówienia na typ anonimowy:");
Console.WriteLine(projections[0]);

var totalSum = processor.AggregateOrders(orders, list => list.Sum(o => o.TotalAmount));
var maxOrder = processor.AggregateOrders(orders, list => list.Max(o => o.TotalAmount));
var averageOrder = processor.AggregateOrders(orders, list => list.Average(o => o.TotalAmount));

Console.WriteLine($"\nAgregacje: Suma = {totalSum:C}, Max = {maxOrder:C}, Średnia = {averageOrder:C}");

Console.WriteLine("\nŁańcuch (Top 2 najdroższe zwalidowane/zakończone zamówienia):");

var topOrders = processor.FilterOrders(orders, o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Validated)
    .OrderByDescending(o => o.TotalAmount)
    .Take(2)
    .ToList();

processor.ProcessOrders(topOrders, printOrder);

Console.ReadLine();

var klienci = SampleData.Customers;
var produkty = SampleData.Products;

var zamówieniaZKlientami = orders
    .Join(klienci,
        o => o.Klient.Id,
        k => k.Id,
        (o, k) => new
        {
            o.Id,
            Klient = k.Nazwa,
            Kwota = o.TotalAmount
        });

Console.WriteLine("\n1. Join (Zamówienia i Klienci):");
foreach (var z in zamówieniaZKlientami) Console.WriteLine($"Zamówienie #{z.Id} - {z.Klient} ({z.Kwota:C})");

var wszystkieSprzedaneProdukty = orders
    .SelectMany(o => o.Pozycje, (zamowienie, pozycja) => new
    {
        ZamowienieId = zamowienie.Id,
        Produkt = pozycja.ZamowionyProdukt.Nazwa,
        Ilosc = pozycja.ZamowionaIlosc
    });

Console.WriteLine("\n2. SelectMany (Wszystkie sprzedane pozycje):");
foreach (var p in wszystkieSprzedaneProdukty) Console.WriteLine($"Zlec. #{p.ZamowienieId}: {p.Produkt} (x{p.Ilosc})");

var statystykiStatusow = from o in orders
                         group o by o.Status into grupa
                         select new
                         {
                             Status = grupa.Key,
                             LiczbaZamowien = grupa.Count(),
                             SumaPrzychodu = grupa.Sum(x => x.TotalAmount)
                         };

Console.WriteLine("\n3. GroupBy (Statystyki statusów):");
foreach (var stat in statystykiStatusow) Console.WriteLine($"Status: {stat.Status} | Ilość: {stat.LiczbaZamowien} | Suma: {stat.SumaPrzychodu:C}");

var klienciIZamowienia = from k in klienci
                         join o in orders on k.Id equals o.Klient.Id into historiaZamowien
                         select new
                         {
                             Klient = k.Nazwa,
                             IloscZamowien = historiaZamowien.Count(),
                             WydanoRazem = historiaZamowien.Sum(x => x.TotalAmount)
                         };

Console.WriteLine("\n4. GroupJoin (Klienci i ich wydatki - Left Join):");
foreach (var kz in klienciIZamowienia) Console.WriteLine($"{kz.Klient} złożył {kz.IloscZamowien} zamówień na kwotę {kz.WydanoRazem:C}");

var zamowieniaPoRabacie = from o in orders
                          let czyVip = o.Klient.IsVip
                          let cenaKoncowa = czyVip ? o.TotalAmount * 0.9m : o.TotalAmount
                          where cenaKoncowa > 1000m
                          orderby cenaKoncowa descending
                          select new { o.Id, Klient = o.Klient.Nazwa, CenaVip = cenaKoncowa };

Console.WriteLine("\n5. Query z 'let' (Duże zamówienia po rabacie VIP):");
foreach (var z in zamowieniaPoRabacie) Console.WriteLine($"#{z.Id} - {z.Klient}: {z.CenaVip:C}");

var popularneProdukty = (from o in orders
                         where o.Status == OrderStatus.Completed
                         from pozycja in o.Pozycje
                         select pozycja.ZamowionyProdukt.Nazwa)
                        .Distinct()
                        .OrderBy(nazwa => nazwa)
                        .ToList();

Console.WriteLine("\n6. Mixed Syntax (Unikalne produkty z zakończonych zamówień):");
foreach (var nazwa in popularneProdukty) Console.WriteLine($" - {nazwa}");