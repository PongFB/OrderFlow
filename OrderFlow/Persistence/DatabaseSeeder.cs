using Microsoft.EntityFrameworkCore;
using OrderFlow.Models;

namespace OrderFlow.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(OrderFlowContext db)
    {
        if (await db.Customers.AnyAsync())
        {
            return;
        }

        Console.WriteLine("Rozpoczynam seedowanie bazy danych...");

        var customers = new List<Customer>
        {
            new Customer(0, "Jan Kowalski", "jan@Merito.pl", "haslo1", "Warszawa", true),
            new Customer(0, "Anna Nowak", "anna@Merito.pl", "haslo2", "Kraków", false),
            new Customer(0, "Piotr Wiśniewski", "piotr@Merito.pl", "haslo3", "Gdańsk", false),
            new Customer(0, "Kasia Zielińska", "kasia@Merito.pl", "haslo4", "Warszawa", true)
        };
        db.Customers.AddRange(customers);

        var products = new List<Product>
        {
            new Product(0, "Myszka Logitech", 150.00m, 50, "Elektronika", true),
            new Product(0, "Klawiatura Razer", 350.00m, 30, "Elektronika", true),
            new Product(0, "Monitor Dell", 1200.00m, 15, "Elektronika", true),
            new Product(0, "Podkładka", 50.00m, 100, "Akcesoria", true),
            new Product(0, "Słuchawki Sony", 800.00m, 40, "Audio", true)
        };
        db.Products.AddRange(products);

        await db.SaveChangesAsync();

        var orders = new List<Order>
        {
            new Order(customers[0].Id) { Status = OrderStatus.Completed, Notes = "Szybka wysyłka" },
            new Order(customers[1].Id) { Status = OrderStatus.New },
            new Order(customers[2].Id) { Status = OrderStatus.Processing },
            new Order(customers[3].Id) { Status = OrderStatus.Completed }
        };

        orders[0].DodajPozycje(new OrderItem(products[0].Id, 2, products[0].Cena));
        orders[1].DodajPozycje(new OrderItem(products[1].Id, 1, products[1].Cena));
        orders[2].DodajPozycje(new OrderItem(products[2].Id, 1, products[2].Cena));
        orders[2].DodajPozycje(new OrderItem(products[3].Id, 1, products[3].Cena));
        orders[3].DodajPozycje(new OrderItem(products[4].Id, 1, products[4].Cena));

        db.Orders.AddRange(orders);

        await db.SaveChangesAsync();

        Console.WriteLine("Seedowanie zakończone sukcesem!\n");
    }
}