public class RecursoCRUD
{
    private Tela tela; 
    private List<Recurso> recursos;
    private Recurso recurso;
    private int posicao;
    private int proximoID = 1;

    public RecursoCRUD(Tela telaPrincipal)
    {
        this.tela = telaPrincipal;
        this.recursos = new List<Recurso>();
        this.recurso = new Recurso();
        this.posicao = -1;

        recursos.Add(new Recurso { id = proximoID++, nome = "Coffee Break", CustoPorUnidade = 15.00m, QuantidadeEmEstoque = 100 });
        recursos.Add(new Recurso { id = proximoID++, nome = "Wi-Fi", CustoPorUnidade = 35.00m, QuantidadeEmEstoque = 10 });
        recursos.Add(new Recurso { id = proximoID++, nome = "Projetor", CustoPorUnidade = 50.00m, QuantidadeEmEstoque = 5 });
    }
    
    public void ExecutarCRUD()
    {
        string opcao;
        List<string> opcoes = new List<string>(); 
        opcoes.Add("[1] Cadastrar Recurso   ");
        opcoes.Add("[2] Editar Estoque/Preço");
        opcoes.Add("[3] Listar Recursos     ");
        opcoes.Add("[0] Voltar              ");
        
        while (true)
        {
            tela.PrepararTelaPrincipal("Gestão de Aluguéis de Salas de Reunião - Gestão de Recursos");
            
            tela.LimparJanelaAcao();
            
            tela.LimparJanelaMenu();
            opcao = tela.DesenharMenu("GESTÃO DE RECURSOS", opcoes);

            switch (opcao)
            {
                case "1": CadastrarRecurso(); break;
                case "2": EditarRecurso(); break;
                case "3": ListarRecursos(); break; 
                case "0": return; 
                default: tela.Pausa("Opção inválida. Pressione Enter."); break;
            }
        }
    }

    private void CadastrarRecurso()
    {
        tela.DesenharJanelaAcao("CADASTRAR RECURSO");
        this.recurso = new Recurso();

        recurso.nome = tela.PerguntarNaAcao(3, "Nome do Recurso: ");
        decimal.TryParse(tela.PerguntarNaAcao(4, "Custo por Unidade (R$): "), out decimal custo);
        recurso.CustoPorUnidade = custo;
        int.TryParse(tela.PerguntarNaAcao(5, "Quantidade em Estoque: "), out int qtd);
        recurso.QuantidadeEmEstoque = qtd;

        if (string.IsNullOrWhiteSpace(recurso.nome) || recurso.CustoPorUnidade < 0 || recurso.QuantidadeEmEstoque < 0)
        {
            tela.Pausa("Erro: Nome, Custo (>=0) e Quantidade (>=0) são obrigatórios. Pressione Enter.");
            return;
        }

        if (ProcurarPorNome(recurso.nome) != null)
        {
            tela.Pausa("Erro: Já existe um recurso com este nome. Pressione Enter.");
            return;
        }

        string resp = tela.PerguntarRodape("Confirma o cadastro do recurso? (S/N): ");
        if (resp.ToUpper() == "S")
        {
            recurso.id = this.proximoID++;
            this.recursos.Add(recurso);
            tela.Pausa("Recurso cadastrado com sucesso! Pressione Enter.");
        }
        else
        {
             tela.Pausa("Cadastro cancelado. Pressione Enter.");
        }
    }

    private void EditarRecurso()
    {
        string nomeBusca = tela.PerguntarRodape("Digite o Nome do recurso para editar: ");

        Recurso recursoEditar = ProcurarPorNome(nomeBusca);
        if (recursoEditar == null)
        {
            tela.Pausa("Recurso não encontrado. Pressione Enter.");
            return;
        }
        
        this.posicao = recursos.IndexOf(recursoEditar);
        tela.DesenharJanelaAcao("EDITAR RECURSO");
        int linDiv = 7;
        tela.DesenharDivisoriaAcao(linDiv, " DADOS ATUAIS ");
        tela.EscreverNaAcao(linDiv + 2, $"Recurso: {recursoEditar.nome}");
        tela.EscreverNaAcao(linDiv + 3, $"Custo: R$ {recursoEditar.CustoPorUnidade:F2}");
        tela.EscreverNaAcao(linDiv + 4, $"Estoque: {recursoEditar.QuantidadeEmEstoque}");

        string novoNome = tela.PerguntarNaAcao(3, $"Novo Nome: ");
        string custoStr = tela.PerguntarNaAcao(4, $"Novo Custo (R$): ");
        string qtdStr = tela.PerguntarNaAcao(5, $"Nova Qtd. Estoque: ");

        if (!string.IsNullOrWhiteSpace(novoNome))
        {
             Recurso nomeDuplicado = ProcurarPorNome(novoNome);
             if (nomeDuplicado != null && nomeDuplicado.id != recursoEditar.id)
             {
                 tela.Pausa("Erro: Já existe um recurso com este nome. Pressione Enter.");
                 return;
             }
        }

        string resp = tela.PerguntarRodape("Confirma as alterações no recurso? (S/N): ");
        if (resp.ToUpper() == "S")
        {
            if (!string.IsNullOrWhiteSpace(novoNome)) recursoEditar.nome = novoNome;
            if (decimal.TryParse(custoStr, out decimal custo) && custo >= 0) recursoEditar.CustoPorUnidade = custo;
            if (int.TryParse(qtdStr, out int qtd) && qtd >= 0) recursoEditar.QuantidadeEmEstoque = qtd;
            
            this.recursos[this.posicao] = recursoEditar;
            tela.Pausa("Recurso atualizado com sucesso! Pressione Enter.");
        }
        else
        {
            tela.Pausa("Alteração cancelada. Pressione Enter.");
        }
    }

    private void ListarRecursos()
    {
        tela.PrepararTelaPrincipal("LISTAGEM DE RECURSOS");

        if (recursos.Count == 0)
        {
            tela.Pausa("Nenhum recurso cadastrado. Pressione Enter.");
            return;
        }

        int linhaAtual = 4;
        
        int colId = 2;
        int colNome = 7;
        int colCusto = 30;
        int colQtd = 48;
        
        Console.SetCursorPosition(colId, linhaAtual); Console.Write("ID");
        Console.SetCursorPosition(colNome, linhaAtual); Console.Write("Nome");
        Console.SetCursorPosition(colCusto, linhaAtual); Console.Write("Custo Unit.");
        Console.SetCursorPosition(colQtd, linhaAtual); Console.Write("Qtd. Estoque");
        linhaAtual++;
        Console.SetCursorPosition(colId, linhaAtual); Console.Write(new string('─', 100));
        linhaAtual++;
        
        foreach (var r in recursos)
        {
            if (linhaAtual >= 25)
            {
                tela.Pausa("Muitos recursos para exibir. Pressione Enter...");
                tela.PrepararTelaPrincipal("LISTAGEM DE RECURSOS");
                linhaAtual = 5;
            }
            
            Console.SetCursorPosition(colId, linhaAtual); Console.Write(r.id.ToString());
            Console.SetCursorPosition(colNome, linhaAtual); Console.Write(r.nome);
            Console.SetCursorPosition(colCusto, linhaAtual); Console.Write($"R$ {r.CustoPorUnidade:F2}");
            Console.SetCursorPosition(colQtd, linhaAtual); Console.Write(r.QuantidadeEmEstoque.ToString());
            linhaAtual++;
        }

        tela.Pausa("Pressione Enter para voltar ao menu de recursos...");
    }
    
    public Recurso ProcurarPorNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) return null;
        return recursos.Find(r => r.nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }

    public bool VerificarDisponibilidade(Recurso recurso, int qtdSolicitada)
    {
        var recEstoque = ProcurarPorNome(recurso.nome);
        if (recEstoque == null) return false;
        return recEstoque.QuantidadeEmEstoque >= qtdSolicitada;
    }
    
    public void BaixarEstoque(Recurso recurso, int qtdBaixa)
    {
        var recEstoque = ProcurarPorNome(recurso.nome);
        if (recEstoque != null)
        {
            int idx = recursos.IndexOf(recEstoque);
            if (idx != -1)
            {
                recEstoque.QuantidadeEmEstoque -= qtdBaixa;
                this.recursos[idx] = recEstoque;
            }
        }
    }
}