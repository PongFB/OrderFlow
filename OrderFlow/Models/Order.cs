namespace OrderFlow.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime DataZamowienia { get; set; }
    public OrderStatus Status { get; set; }
    public string? Notes { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public List<OrderItem> Pozycje { get; set; } = new();

    public decimal TotalAmount => Pozycje.Sum(pozycja => pozycja.TotalPrice);

    public Order() { }

    public Order(int customerId)
    {
        CustomerId = customerId;
        DataZamowienia = DateTime.Now;
        Status = OrderStatus.New;
    }

    public void DodajPozycje(OrderItem nowaPozycja)
    {
        Pozycje.Add(nowaPozycja);
    }

    public void ZmienStatus(OrderStatus nowyStatus)
    {
        Status = nowyStatus;
    }
}