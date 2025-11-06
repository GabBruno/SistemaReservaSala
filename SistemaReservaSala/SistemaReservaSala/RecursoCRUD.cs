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
            tela.LimparJanelaAcao();
            
            opcao = tela.DesenharMenu("GESTÃO DE ESTOQUE", opcoes);

            switch (opcao)
            {
                case "1": CadastrarRecurso(); break;
                case "2": EditarRecurso(); break;
                case "3": ListarRecursos(); break;
                case "0": tela.LimparJanelaMenu(); return; 
                default:
                    tela.MostrarMensagemRodape("Opção inválida. Pressione Enter.");
                    Console.ReadKey();
                    break;
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
            tela.MostrarMensagemRodape("Erro: Nome, Custo (>=0) e Quantidade (>=0) são obrigatórios. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        if (ProcurarPorNome(recurso.nome) != null)
        {
            tela.MostrarMensagemRodape("Erro: Já existe um recurso com este nome. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        recurso.id = this.proximoID++;
        this.recursos.Add(recurso);
        tela.MostrarMensagemRodape("Recurso cadastrado com sucesso! Pressione Enter.");
        Console.ReadKey();
    }

    private void EditarRecurso()
    {
        string nomeBusca = tela.PerguntarRodape("Digite o Nome do recurso para editar: ");

        Recurso recursoEditar = ProcurarPorNome(nomeBusca);
        if (recursoEditar == null)
        {
            tela.MostrarMensagemRodape("Recurso não encontrado. Pressione Enter.");
            Console.ReadKey();
            return;
        }
        
        this.posicao = recursos.IndexOf(recursoEditar);

        tela.DesenharJanelaAcao("EDITAR RECURSO");
        
        int linDiv = 6; 
        tela.DesenharDivisoriaAcao(linDiv, " DADOS ATUAIS ");
        tela.EscreverNaAcao(linDiv + 2, $"Recurso: {recursoEditar.nome}");
        tela.EscreverNaAcao(linDiv + 3, $"Custo: R$ {recursoEditar.CustoPorUnidade:F2}");
        tela.EscreverNaAcao(linDiv + 4, $"Estoque: {recursoEditar.QuantidadeEmEstoque}");

        string custoStr = tela.PerguntarNaAcao(3, $"Novo Custo (R$) [{recursoEditar.CustoPorUnidade:F2}]: ");
        string qtdStr = tela.PerguntarNaAcao(4, $"Nova Qtd. Estoque [{recursoEditar.QuantidadeEmEstoque}]: ");

        if (decimal.TryParse(custoStr, out decimal custo) && custo >= 0) recursoEditar.CustoPorUnidade = custo;
        if (int.TryParse(qtdStr, out int qtd) && qtd >= 0) recursoEditar.QuantidadeEmEstoque = qtd;

        this.recursos[this.posicao] = recursoEditar;
        tela.MostrarMensagemRodape("Recurso atualizado com sucesso! Pressione Enter.");
        Console.ReadKey();
    }

    private void ListarRecursos()
    {
        tela.DesenharJanelaAcao("LISTAGEM DE RECURSOS (ESTOQUE)");

        if (recursos.Count == 0)
        {
            tela.MostrarMensagemRodape("Nenhum recurso cadastrado. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        int linhaAtual = 3;
        
        int colId = 2;
        int colNome = 7;
        int colCusto = 30;
        int colQtd = 48;
        
        tela.EscreverNaAcao(linhaAtual, colId, "ID");
        tela.EscreverNaAcao(linhaAtual, colNome, "Nome");
        tela.EscreverNaAcao(linhaAtual, colCusto, "Custo Unit.");
        tela.EscreverNaAcao(linhaAtual, colQtd, "Qtd. Estoque");
        linhaAtual++;
        tela.EscreverNaAcao(linhaAtual++, new string('-', 66));
        
        foreach (var r in recursos)
        {
            if (linhaAtual >= 24)
            {
                tela.MostrarMensagemRodape("Muitos recursos para exibir. Pressione Enter...");
                Console.ReadKey();
                tela.LimparJanelaAcao();
                tela.DesenharJanelaAcao("LISTAGEM DE RECURSOS (ESTOQUE)");
                linhaAtual = 3;
            }
            
            tela.EscreverNaAcao(linhaAtual, colId, r.id.ToString());
            tela.EscreverNaAcao(linhaAtual, colNome, r.nome);
            tela.EscreverNaAcao(linhaAtual, colCusto, $"R$ {r.CustoPorUnidade:F2}");
            tela.EscreverNaAcao(linhaAtual, colQtd, r.QuantidadeEmEstoque.ToString());
            linhaAtual++;
        }

        tela.MostrarMensagemRodape("Pressione Enter para voltar ao menu de recursos...");
        Console.ReadKey();
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