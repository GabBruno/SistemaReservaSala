public class ItemReserva
{
    public Recurso Recurso;
    public int QuantidadeSolicitada;

    public ItemReserva(Recurso recurso, int quantidade)
    {
        this.Recurso = recurso;
        this.QuantidadeSolicitada = quantidade;
    }
}