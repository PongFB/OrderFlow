namespace OrderFlow.Models;

public class Product
{
    public int Id { get; private set; }
    public string Nazwa { get; private set; }
    public decimal Cena { get; private set; }
    public int Ilosc { get; private set; }
    public string Kategoria { get; private set; }
    public bool Status { get; private set; }

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