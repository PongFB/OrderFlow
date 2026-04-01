using OrderFlow.Models;

namespace OrderFlow.Services;

public class OrderProcessor
{
    public List<Order> FilterOrders(List<Order> orders, Predicate<Order> condition)
    {
        var result = new List<Order>();
        foreach (var order in orders)
        {
            if (condition(order))
            {
                result.Add(order);
            }
        }
        return result;
    }

    public void ProcessOrders(List<Order> orders, Action<Order> action)
    {
        foreach (var order in orders)
        {
            action(order);
        }
    }

    public List<T> TransformOrders<T>(List<Order> orders, Func<Order, T> transform)
    {
        var result = new List<T>();
        foreach (var order in orders)
        {
            result.Add(transform(order));
        }
        return result;
    }

    public decimal AggregateOrders(List<Order> orders, Func<IEnumerable<Order>, decimal> aggregator)
    {
        return aggregator(orders);
    }
}