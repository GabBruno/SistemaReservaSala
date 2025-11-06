public class Recurso
{
    public int id;
    public string nome;
    public decimal CustoPorUnidade;
    public int QuantidadeEmEstoque;

    public Recurso()
    {
        this.id = 0;
        this.nome = "";
        this.CustoPorUnidade = 0;
        this.QuantidadeEmEstoque = 0;
    }
}