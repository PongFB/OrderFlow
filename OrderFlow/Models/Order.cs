using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OrderFlow.Models;

[XmlRoot("zamowienie")]
public class Order
{
    [XmlAttribute("id_zamowienia")]
    public int Id { get;  set; }

    [JsonPropertyName("klient")]
    [XmlElement("dane_klient")]
    public Customer Klient { get; set; }

    [JsonIgnore]
    [XmlIgnore]
    public DateTime DataZamowienia { get; set; }

    public OrderStatus Status { get; set; }

    [XmlArray("lista_pozycji")]
    [XmlArrayItem("pozycja")]
    public List<OrderItem> Pozycje { get; set; } = new List<OrderItem>();

    public decimal TotalAmount => Pozycje.Sum(pozycja => pozycja.TotalPrice);

    public Order() { }

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