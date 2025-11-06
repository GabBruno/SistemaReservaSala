using System.Net.Mail;

public class Cliente
{
    // propriedades
    public int id;
    public string nome;
    public string cpf;
    public string email;
    public string telefone;

    public Cliente()
    {
        this.id = 0;
        this.nome = "";
        this.cpf = "";
        this.email = "";
        this.telefone = "";
    }

    public Cliente(int id, string nome, string cpf, string email, string telefone)
    {
        this.id = id;
        this.nome = nome;
        this.cpf = cpf;
        this.email = email;
        this.telefone = telefone;
    }
}