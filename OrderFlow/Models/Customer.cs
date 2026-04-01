namespace OrderFlow.Models;

public class Customer
{
    public int Id { get; private set; }
    public string Nazwa { get; private set; }
    public string Email { get; private set; }
    public string Haslo { get; private set; }
    public bool IsVip { get; private set; }

    public Customer(int id, string nazwa, string email, string haslo, bool isVip = false)
    {
        Id = id;
        Nazwa = nazwa;
        Email = email;
        Haslo = haslo;
        IsVip = isVip;
    }
}