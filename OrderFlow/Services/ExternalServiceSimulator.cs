using System;
using System.Collections.Generic;
using System.Diagnostics;
using OrderFlow.Models;

namespace OrderFlow.Services;

public class ExternalServiceSimulator
{
    public async Task CheckInventoryAsync(Product product)
    {
        int delay = Random.Shared.Next(500, 1500);
        await Task.Delay(delay);
        Console.WriteLine($"      [Magazyn] Produkt '{product.Nazwa}' jest dostępny. (Czas: {delay}ms)");
    }

    public async Task ValidatePaymentAsync(Order order)
    {
        int delay = Random.Shared.Next(1000, 2000);
        await Task.Delay(delay);
        Console.WriteLine($"      [Płatność] Płatność dla zamówienia #{order.Id} zaakceptowana. (Czas: {delay}ms)");
    }

    public async Task CalculateShippingAsync(Order order)
    {
        int delay = Random.Shared.Next(300, 800);
        await Task.Delay(delay);
        Console.WriteLine($"      [Wysyłka] Koszt wysyłki dla #{order.Id} obliczony. (Czas: {delay}ms)");
    }

    public async Task ProcessOrderAsync(Order order)
    {
        var sw = Stopwatch.StartNew();
        var productToCheck = order.Pozycje.FirstOrDefault()?.ZamowionyProdukt;
        if (productToCheck == null) return;
        var inventoryTask = CheckInventoryAsync(productToCheck);
        var paymentTask = ValidatePaymentAsync(order);
        var shippingTask = CalculateShippingAsync(order);
        await Task.WhenAll(inventoryTask, paymentTask, shippingTask);
        sw.Stop();
        Console.WriteLine($"   => Zlecenie #{order.Id} załadowane w {sw.ElapsedMilliseconds}ms\n");
    }

    public async Task ProcessMultipleOrdersAsync(List<Order> orders)
    {
        using var semaphore = new SemaphoreSlim(3);
        int processedCount = 0;
        int totalCount = orders.Count;

        var tasks = new List<Task>();

        foreach (var order in orders)
        {
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await ProcessOrderAsync(order);
                    int current = Interlocked.Increment(ref processedCount);
                    Console.WriteLine($"[POSTĘP] Przetworzono {current}/{totalCount} zamówień.");
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }
        await Task.WhenAll(tasks);
    }
}