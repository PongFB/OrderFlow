namespace OrderFlow.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int ZamowionaIlosc { get; set; }
    public decimal UnitPrice { get; set; } 

    public decimal TotalPrice => ZamowionaIlosc * UnitPrice;

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public OrderItem() { }

    public OrderItem(int productId, int zamowionaIlosc, decimal unitPrice)
    {
        ProductId = productId;
        ZamowionaIlosc = zamowionaIlosc;
        UnitPrice = unitPrice;
    }
}