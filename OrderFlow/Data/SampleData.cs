using System.Collections.Generic;
using OrderFlow.Models;

namespace OrderFlow.Data;

public static class SampleData
{
    public static List<Product> Products { get; private set; }
    public static List<Customer> Customers { get; private set; }
    public static List<Order> Orders { get; private set; }

    static SampleData()
    {
        Products = new List<Product>
        {
            new Product(1, "Laptop Dell XPS", 6500.00m, 10, "Elektronika", true),
            new Product(2, "Myszka bezprzewodowa Logitech", 150.00m, 50, "Akcesoria", true),
            new Product(3, "Kawa ziarnista Lavazza 1kg", 60.00m, 100, "Spożywcze", true),
            new Product(4, "Fotel ergonomiczny Herman Miller", 4500.00m, 5, "Meble", true),
            new Product(5, "Monitor LG Ultrawide", 1200.00m, 15, "Elektronika", true),
            new Product(6, "Książka: C# in Depth", 120.00m, 30, "Książki", false)
        };

        Customers = new List<Customer>
        {
            new Customer(1, "Jan Kowalski", "jan.kowalski@example.com", "pass123"),
            new Customer(2, "Anna Nowak", "anna.nowak@example.com", "pass456", isVip: true),
            new Customer(3, "Piotr Wiśniewski", "piotr.w@example.com", "pass789"),
            new Customer(4, "Katarzyna Wójcik", "kasia.w@example.com", "pass000")
        };

        Orders = new List<Order>();

        var order1 = new Order(1, Customers[0]);
        order1.DodajPozycje(new OrderItem(Products[1], 1));
        Orders.Add(order1);

        var order2 = new Order(2, Customers[1]);
        order2.DodajPozycje(new OrderItem(Products[0], 1));
        order2.DodajPozycje(new OrderItem(Products[4], 2));
        order2.ZmienStatus(OrderStatus.Completed);
        Orders.Add(order2);

        var order3 = new Order(3, Customers[2]);
        order3.DodajPozycje(new OrderItem(Products[3], 2));
        order3.ZmienStatus(OrderStatus.Processing);
        Orders.Add(order3);

        var order4 = new Order(4, Customers[3]);
        order4.DodajPozycje(new OrderItem(Products[5], 1));
        order4.ZmienStatus(OrderStatus.Cancelled);
        Orders.Add(order4);

        var order5 = new Order(5, Customers[1]);
        order5.DodajPozycje(new OrderItem(Products[2], 5));
        order5.ZmienStatus(OrderStatus.Validated);
        Orders.Add(order5);

        var order6 = new Order(6, Customers[0]);
        order6.DodajPozycje(new OrderItem(Products[4], 1));
        order6.ZmienStatus(OrderStatus.Completed);
        Orders.Add(order6);
    }
}