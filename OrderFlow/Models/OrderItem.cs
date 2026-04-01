namespace OrderFlow.Models;

public class OrderItem
{
    public Product ZamowionyProdukt { get; private set; }
    public int ZamowionaIlosc { get; private set; }

    public decimal TotalPrice => ZamowionyProdukt.Cena * ZamowionaIlosc;

    public OrderItem(Product produkt, int zamowionaIlosc)
    {
        ZamowionyProdukt = produkt;
        ZamowionaIlosc = zamowionaIlosc;
    }
}