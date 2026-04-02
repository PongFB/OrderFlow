using OrderFlow.Models;

namespace OrderFlow.Services;

public class OrderPipeline
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<OrderValidationEventArgs>? ValidationCompleted;

    public void ProcessOrder(Order order)
    {
        bool isValid = order.Pozycje.Count > 0;
        var errors = new List<string>();
        if (!isValid)
        {
            errors.Add("Zamówienie jest puste.");
        }

        ValidationCompleted?.Invoke(this, new OrderValidationEventArgs(order, isValid, errors));

        if (!isValid)
        {
            ChangeStatus(order, OrderStatus.Cancelled);
            return;
        }

        ChangeStatus(order, OrderStatus.Validated);
        ChangeStatus(order, OrderStatus.Processing);
        ChangeStatus(order, OrderStatus.Completed);
    }

    private void ChangeStatus(Order order, OrderStatus newStatus)
    {
        var oldStatus = order.Status;
        order.ZmienStatus(newStatus);

        StatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, oldStatus, newStatus));
    }
}