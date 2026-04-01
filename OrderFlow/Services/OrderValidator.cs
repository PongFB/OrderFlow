using OrderFlow.Models;

namespace OrderFlow.Services;

public delegate bool ValidationRule(Order order, out string errorMessage);

public class OrderValidator
{
    private readonly List<ValidationRule> _customRules = new();
    private readonly List<Func<Order, bool>> _funcRules = new();

    public OrderValidator()
    {
        _customRules.Add(OrderMustHaveItems);
        _customRules.Add(OrderAmountCannotExceedLimit);
        _customRules.Add(OrderItemsMustHavePositiveQuantity);

        _funcRules.Add(order => order.DataZamowienia <= DateTime.Now);

        _funcRules.Add(order => order.Status != OrderStatus.Cancelled);
    }

    private bool OrderMustHaveItems(Order order, out string errorMessage)
    {
        if (order.Pozycje.Count == 0)
        {
            errorMessage = "Zamówienie nie zawiera żadnych pozycji.";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }

    private bool OrderAmountCannotExceedLimit(Order order, out string errorMessage)
    {
        if (order.TotalAmount > 15000m)
        {
            errorMessage = $"Kwota zamówienia ({order.TotalAmount:C}) przekracza limit 15 000 zł.";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }

    private bool OrderItemsMustHavePositiveQuantity(Order order, out string errorMessage)
    {
        if (order.Pozycje.Any(p => p.ZamowionaIlosc <= 0))
        {
            errorMessage = "Każda pozycja w zamówieniu musi mieć ilość większą niż 0.";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }

    public void ValidateAll(Order order)
    {
        Console.WriteLine($"Walidacja zamówienia ID: {order.Id} (Klient: {order.Klient.Nazwa})");
        bool isValid = true;

        foreach (var rule in _customRules)
        {
            if (!rule(order, out string error))
            {
                Console.WriteLine($"BŁĄD {error}");
                isValid = false;
            }
        }

        int ruleIndex = 1;
        foreach (var funcRule in _funcRules)
        {
            if (!funcRule(order))
            {
                Console.WriteLine($"BŁĄD Zamówienie oblało szybką regułę Func nr {ruleIndex}.");
                isValid = false;
            }
            ruleIndex++;
        }

        if (isValid)
        {
            Console.WriteLine("Zamówienie jest poprawne.");
        }
        Console.WriteLine();
    }
}