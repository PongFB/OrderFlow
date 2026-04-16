namespace OrderFlow.Models;

public class OrderItem
{
    public Product ZamowionyProdukt { get; set; }
    public int ZamowionaIlosc { get; set; }

    public decimal TotalPrice => ZamowionyProdukt.Cena * ZamowionaIlosc;

    public OrderItem() { }

    public OrderItem(Product produkt, int zamowionaIlosc)
    {
        ZamowionyProdukt = produkt;
        ZamowionaIlosc = zamowionaIlosc;
    }
}