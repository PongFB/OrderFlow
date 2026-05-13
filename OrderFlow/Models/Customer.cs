namespace OrderFlow.Models;

public class Customer
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = "";
    public string? Email { get; set; }
    public string Haslo { get; set; } = "";
    public string City { get; set; } = "";
    public bool IsVip { get; set; }

    public List<Order> Orders { get; set; } = new();

    public Customer() { }

    public Customer(int id, string nazwa, string email, string haslo, string city, bool isVip = false)
    {
        Id = id;
        Nazwa = nazwa;
        Email = email;
        Haslo = haslo;
        City = city;
        IsVip = isVip;
    }
}