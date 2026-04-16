namespace OrderFlow.Models;

public class Product
{
    public int Id { get; set; }
    public string Nazwa { get; set; }
    public decimal Cena { get; set; }
    public int Ilosc { get; set; }
    public string Kategoria { get; set; }
    public bool Status { get; set; }

    public Product() { }

    public Product(int id, string nazwa, decimal cena, int ilosc, string kategoria, bool status)
    {
        Id = id;
        Nazwa = nazwa;
        Cena = cena;
        Ilosc = ilosc;
        Kategoria = kategoria;
        Status = status;
    }
}