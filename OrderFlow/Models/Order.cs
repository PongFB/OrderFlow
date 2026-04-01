using OrderFlow.Models;

public class Order
{
    public int Id { get; private set; }
    public Customer Klient { get; private set; }
    public DateTime DataZamowienia { get; private set; }
    public OrderStatus Status { get; private set; }
    public List<OrderItem> Pozycje { get; private set; } = new List<OrderItem>();
    public decimal TotalAmount => Pozycje.Sum(pozycja => pozycja.TotalPrice);

    public Order(int id, Customer klient)
    {
        Id = id;
        Klient = klient;
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