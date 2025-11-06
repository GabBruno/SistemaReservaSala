public class Reserva
{
    public int id;
    public Cliente cliente; 
    public Sala sala; 
    public DateTime DataHoraInicio; 
    public DateTime DataHoraFim;

    public List<ItemReserva> ItensConsumidos;
    public List<Pagamento> PagamentosRegistrados;
    public decimal ValorTotalCalculado;
    public string StatusReserva;

    public Reserva()
    {
        this.id = 0;
        this.cliente = new Cliente();
        this.sala = new Sala();
        this.DataHoraInicio = DateTime.MinValue;
        this.DataHoraFim = DateTime.MinValue;
        this.ItensConsumidos = new List<ItemReserva>();
        this.PagamentosRegistrados = new List<Pagamento>();
        this.ValorTotalCalculado = 0;
        this.StatusReserva = "Pendente";
    }

    // RN-014: Cálculo de Tarifa
    public void CalcularCustoTotal()
    {
        decimal valorBase = 0;
        TimeSpan duracao = DataHoraFim - DataHoraInicio;
        double totalHoras = Math.Max(1, duracao.TotalHours); 

        valorBase = (decimal)totalHoras * sala.valorHora;

        decimal valorItens = 0;
        foreach (var item in ItensConsumidos)
        {
            valorItens += item.Recurso.CustoPorUnidade * item.QuantidadeSolicitada;
        }

        this.ValorTotalCalculado = valorBase + valorItens;
    }

    // Método auxiliar para RN-015
    public decimal GetValorPagoTotal()
    {
        decimal totalPago = 0;
        foreach (var pag in PagamentosRegistrados)
        {
            totalPago += pag.Valor;
        }
        return totalPago;
    }

    // Método auxiliar para RN-015
    public void AtualizarStatusReserva()
    {
        if (this.ValorTotalCalculado == 0)
        {
            this.StatusReserva = "Pendente";
            return;
        }
        
        decimal totalPago = GetValorPagoTotal();
        decimal valorMinimo50 = this.ValorTotalCalculado * 0.5m;

        if (totalPago >= valorMinimo50)
        {
            this.StatusReserva = "Confirmada";
        }
        else
        {
            this.StatusReserva = "Pendente";
        }
    }
}