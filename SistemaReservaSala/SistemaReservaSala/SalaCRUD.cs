public class SalaCRUD
{
    private Tela tela; 
    private List<Sala> salas;
    private Sala sala;
    private int posicao;
    private int proximoID = 1;
    private const int LIMITE_SALAS = 30; 

    public SalaCRUD(Tela telaPrincipal)
    {
        this.tela = telaPrincipal;
        this.salas = new List<Sala>();
        this.sala = new Sala();
        this.posicao = -1;
    }
    
    public void ExecutarCRUD()
    {
        string opcao;
        List<string> opcoes = new List<string>(); 
        opcoes.Add("[1] Cadastrar Sala");
        opcoes.Add("[2] Editar Sala   ");
        opcoes.Add("[3] Consultar Sala");
        opcoes.Add("[4] Listar Salas  ");
        opcoes.Add("[0] Voltar        ");
        
        while(true)
        {
            tela.LimparJanelaAcao();
            
            opcao = tela.DesenharMenu("GESTÃO DE SALAS", opcoes);

            switch (opcao)
            {
                case "1": CadastrarSala(); break;
                case "2": EditarSala(); break;
                case "3": ConsultarSala(); break;
                case "4": ListarSalas(); break;
                case "0": tela.LimparJanelaMenu(); return; 
                default:
                    tela.MostrarMensagemRodape("Opção inválida. Pressione Enter.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void CadastrarSala()
    {
        if (this.salas.Count >= LIMITE_SALAS)
        {
            tela.MostrarMensagemRodape($"ERRO: O limite máximo de {LIMITE_SALAS} salas foi atingido. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        tela.DesenharJanelaAcao("CADASTRAR SALA");
        this.sala = new Sala();

        sala.nome = tela.PerguntarNaAcao(3, "Nome da Sala: ");
        
        int.TryParse(tela.PerguntarNaAcao(4, "Capacidade (Pessoas): "), out int cap);
        sala.capacidade = cap;

        decimal.TryParse(tela.PerguntarNaAcao(5, "Valor por Hora (R$): "), out decimal val);
        sala.valorHora = val;
        
        string recursos = tela.PerguntarNaAcao(6, "Recursos Fixos (Ex: Projetor, Wi-Fi): ");
        sala.recursosFixos.AddRange(recursos.Split(','));

        if (string.IsNullOrWhiteSpace(sala.nome) || sala.capacidade <= 0 || sala.valorHora <= 0)
        {
            tela.MostrarMensagemRodape("Erro: Nome, Capacidade (>0) e Valor (>0) são obrigatórios. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        if (ProcurarPorNome(sala.nome) != null)
        {
            tela.MostrarMensagemRodape("Erro: Já existe uma sala com este nome. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        sala.id = this.proximoID++;
        this.salas.Add(sala);
        tela.MostrarMensagemRodape("Sala cadastrada com sucesso! Pressione Enter.");
        Console.ReadKey();
    }

    private void EditarSala()
    {
        string nomeBusca = tela.PerguntarRodape("Digite o Nome da sala para editar: ");

        Sala salaParaEditar = ProcurarPorNome(nomeBusca);
        if (salaParaEditar == null)
        {
            tela.MostrarMensagemRodape("Sala não encontrada. Pressione Enter.");
            Console.ReadKey();
            return;
        }
        
        this.posicao = salas.IndexOf(salaParaEditar);

        tela.DesenharJanelaAcao("EDITAR SALA");
        int linDiv = 7; 
        tela.DesenharDivisoriaAcao(linDiv, " DADOS ATUAIS ");

        tela.EscreverNaAcao(linDiv + 2, $"Nome: {salaParaEditar.nome}");
        tela.EscreverNaAcao(linDiv + 3, $"Capacidade: {salaParaEditar.capacidade}");
        tela.EscreverNaAcao(linDiv + 4, $"Valor/Hora: R$ {salaParaEditar.valorHora:F2}");

        string nome = tela.PerguntarNaAcao(3, $"Novo Nome [{salaParaEditar.nome}]: ");
        string capStr = tela.PerguntarNaAcao(4, $"Nova Capacidade [{salaParaEditar.capacidade}]: ");
        string valStr = tela.PerguntarNaAcao(5, $"Novo Valor/Hora [{salaParaEditar.valorHora:F2}]: ");


        if (!string.IsNullOrWhiteSpace(nome))
        {
             Sala nomeDuplicado = salas.Find(s => s.nome.Equals(nome, StringComparison.OrdinalIgnoreCase) && s.id != salaParaEditar.id);
             if (nomeDuplicado != null)
             {
                 tela.MostrarMensagemRodape("Erro: Este Nome de Sala já está cadastrado. Tente outro nome. Pressione Enter.");
                 Console.ReadKey();
                 return;
             }
             salaParaEditar.nome = nome;
        }

        if (int.TryParse(capStr, out int cap) && cap > 0) salaParaEditar.capacidade = cap;
        if (decimal.TryParse(valStr, out decimal val) && val > 0) salaParaEditar.valorHora = val;

        this.salas[this.posicao] = salaParaEditar;
        tela.MostrarMensagemRodape("Sala atualizada com sucesso! Pressione Enter.");
        Console.ReadKey();
    }

    private void ConsultarSala()
    {
        string nomeBusca = tela.PerguntarRodape("Digite o Nome da sala para consultar: ");

        this.sala = ProcurarPorNome(nomeBusca);
        if (this.sala == null)
        {
            tela.MostrarMensagemRodape("Sala não encontrada. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        tela.DesenharJanelaAcao("CONSULTAR SALA");

        tela.EscreverNaAcao(3, $"ID: {sala.id}");
        tela.EscreverNaAcao(4, $"Nome: {sala.nome}");
        tela.EscreverNaAcao(5, $"Capacidade: {sala.capacidade} pessoas");
        tela.EscreverNaAcao(6, $"Valor/Hora: R$ {sala.valorHora:F2}");
        tela.EscreverNaAcao(7, $"Recursos: {string.Join(", ", sala.recursosFixos)}");
        
        tela.MostrarMensagemRodape("Pressione Enter para voltar ao menu de salas...");
        Console.ReadKey();
    }

    private void ListarSalas()
    {
        tela.DesenharJanelaAcao("LISTAGEM DE SALAS");

        if (salas.Count == 0)
        {
            tela.MostrarMensagemRodape("Nenhuma sala cadastrada. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        int linhaAtual = 3;
        
        int colId = 2;
        int colNome = 7;
        int colCap = 30;
        int colValor = 45;

        tela.EscreverNaAcao(linhaAtual, colId, "ID");
        tela.EscreverNaAcao(linhaAtual, colNome, "Nome da Sala");
        tela.EscreverNaAcao(linhaAtual, colCap, "Capacidade");
        tela.EscreverNaAcao(linhaAtual, colValor, "Valor/Hora");
        linhaAtual++;
        tela.EscreverNaAcao(linhaAtual++, new string('-', 66));
        
        foreach (var s in salas)
        {
            if (linhaAtual >= 24) 
            {
                tela.MostrarMensagemRodape("Muitas salas para exibir. Pressione Enter...");
                Console.ReadKey();
                tela.LimparJanelaAcao();
                tela.DesenharJanelaAcao("LISTAGEM DE SALAS");
                linhaAtual = 3;
            }

            tela.EscreverNaAcao(linhaAtual, colId, s.id.ToString());
            tela.EscreverNaAcao(linhaAtual, colNome, s.nome);
            tela.EscreverNaAcao(linhaAtual, colCap, s.capacidade.ToString());
            tela.EscreverNaAcao(linhaAtual, colValor, $"R$ {s.valorHora:F2}");
            linhaAtual++;
        }

        tela.MostrarMensagemRodape("Pressione Enter para voltar ao menu de salas...");
        Console.ReadKey();
    }

    public Sala ProcurarPorNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return null;
        
        Sala encontrado = salas.Find(s => s.nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
        if (encontrado != null)
        {
            this.posicao = salas.IndexOf(encontrado);
        }
        return encontrado;
    }
}