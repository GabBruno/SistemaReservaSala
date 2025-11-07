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
        
        this.salas.Add(new Sala(proximoID++, "Delta", 10, 50.00m, new List<string>{"Wi-Fi", "Quadro Branco"}));
        this.salas.Add(new Sala(proximoID++, "Alpha", 8, 150.00m, new List<string>{"Wi-Fi Premium", "Projetor", "Café"}));
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
            tela.PrepararTelaPrincipal("Gestão de Aluguéis de Salas de Reunião - Gestão de Salas");
            
            tela.LimparJanelaAcao();
            
            tela.LimparJanelaMenu();
            opcao = tela.DesenharMenu("GESTÃO DE SALAS", opcoes);

            switch (opcao)
            {
                case "1": CadastrarSala(); break;
                case "2": EditarSala(); break;
                case "3": ConsultarSala(); break;
                case "4": ListarSalas(); break; 
                case "0": return; 
                default: tela.Pausa("Opção inválida. Pressione Enter."); break;
            }
        }
    }

    private void CadastrarSala()
    {
        if (this.salas.Count >= LIMITE_SALAS)
        {
            tela.Pausa($"ERRO: O limite máximo de {LIMITE_SALAS} salas foi atingido. Pressione Enter.");
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
            tela.Pausa("Erro: Nome, Capacidade (>0) e Valor (>0) são obrigatórios. Pressione Enter.");
            return;
        }

        if (ProcurarPorNome(sala.nome) != null)
        {
            tela.Pausa("Erro: Já existe uma sala com este nome. Pressione Enter.");
            return;
        }

        sala.id = this.proximoID++;
        this.salas.Add(sala);
        tela.Pausa("Sala cadastrada com sucesso! Pressione Enter.");
    }

    private void EditarSala()
    {
        string nomeBusca = tela.PerguntarRodape("Digite o Nome da sala para editar: ");

        Sala salaParaEditar = ProcurarPorNome(nomeBusca);
        if (salaParaEditar == null)
        {
            tela.Pausa("Sala não encontrada. Pressione Enter.");
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
                 tela.Pausa("Erro: Este Nome de Sala já está cadastrado. Tente outro nome. Pressione Enter.");
                 return;
             }
             salaParaEditar.nome = nome;
        }

        if (int.TryParse(capStr, out int cap) && cap > 0) salaParaEditar.capacidade = cap;
        if (decimal.TryParse(valStr, out decimal val) && val > 0) salaParaEditar.valorHora = val;

        this.salas[this.posicao] = salaParaEditar;
        tela.Pausa("Sala atualizada com sucesso! Pressione Enter.");
    }

    private void ConsultarSala()
    {
        string nomeBusca = tela.PerguntarRodape("Digite o Nome da sala para consultar: ");

        this.sala = ProcurarPorNome(nomeBusca);
        if (this.sala == null)
        {
            tela.Pausa("Sala não encontrada. Pressione Enter.");
            return;
        }

        tela.DesenharJanelaAcao("CONSULTAR SALA");
        tela.EscreverNaAcao(3, $"ID: {sala.id}");
        tela.EscreverNaAcao(4, $"Nome: {sala.nome}");
        tela.EscreverNaAcao(5, $"Capacidade: {sala.capacidade} pessoas");
        tela.EscreverNaAcao(6, $"Valor/Hora: R$ {sala.valorHora:F2}");
        tela.EscreverNaAcao(7, $"Recursos: {string.Join(", ", sala.recursosFixos)}");
        
        tela.Pausa("Pressione Enter para voltar ao menu de salas...");
    }
    private void ListarSalas()
    {
        tela.PrepararTelaPrincipal("LISTAGEM DE SALAS");

        if (salas.Count == 0)
        {
            tela.Pausa("Nenhuma sala cadastrada. Pressione Enter.");
            return;
        }

        int linhaAtual = 4;
        
        int colId = 2;
        int colNome = 7;
        int colCap = 30;
        int colValor = 45;
        int colRec = 60;

        Console.SetCursorPosition(colId, linhaAtual); Console.Write("ID");
        Console.SetCursorPosition(colNome, linhaAtual); Console.Write("Nome da Sala");
        Console.SetCursorPosition(colCap, linhaAtual); Console.Write("Capacidade");
        Console.SetCursorPosition(colValor, linhaAtual); Console.Write("Valor/Hora");
        Console.SetCursorPosition(colRec, linhaAtual); Console.Write("Recursos Fixos");
        linhaAtual++;
        Console.SetCursorPosition(colId, linhaAtual); Console.Write(new string('─', 100));
        linhaAtual++;
        
        foreach (var s in salas)
        {
            if (linhaAtual >= 25) 
            {
                tela.Pausa("Muitas salas para exibir. Pressione Enter...");
                tela.PrepararTelaPrincipal("LISTAGEM DE SALAS");
                linhaAtual = 5;
            }

            Console.SetCursorPosition(colId, linhaAtual); Console.Write(s.id.ToString());
            Console.SetCursorPosition(colNome, linhaAtual); Console.Write(s.nome);
            Console.SetCursorPosition(colCap, linhaAtual); Console.Write(s.capacidade.ToString());
            Console.SetCursorPosition(colValor, linhaAtual); Console.Write($"R$ {s.valorHora:F2}");
            Console.SetCursorPosition(colRec, linhaAtual); Console.Write(string.Join(", ", s.recursosFixos));
            linhaAtual++;
        }

        tela.Pausa("Pressione Enter para voltar ao menu de salas...");
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