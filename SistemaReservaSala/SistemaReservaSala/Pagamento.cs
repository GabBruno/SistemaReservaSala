public class Pagamento
{
    public int id;
    public decimal Valor;
    public DateTime DataPagamento;
    public string Metodo;

    public Pagamento()
    {
        this.id = 0;
        this.Valor = 0;
        this.DataPagamento = DateTime.Now;
        this.Metodo = "";
    }
}