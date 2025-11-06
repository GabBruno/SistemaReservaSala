public class Sala
{
    public int id;
    public string nome;
    public int capacidade;
    public decimal valorHora;
    public List<string> recursosFixos;

    public Sala()
    {
        this.id = 0;
        this.nome = "";
        this.capacidade = 0;
        this.valorHora = 0;
        this.recursosFixos = new List<string>();
    }
    
    public Sala(int id, string nome, int capacidade, decimal valorHora, List<string> recursos)
    {
        this.id = id;
        this.nome = nome;
        this.capacidade = capacidade;
        this.valorHora = valorHora;
        this.recursosFixos = recursos;
    }
}