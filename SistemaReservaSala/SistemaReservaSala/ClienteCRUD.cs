public class ClienteCRUD
{
    private Tela tela; 
    private List<Cliente> clientes;
    private Cliente cliente;
    private int posicao;
    private int proximoID = 1;

    public ClienteCRUD(Tela telaPrincipal)
    {
        this.tela = telaPrincipal; 
        this.clientes = new List<Cliente>();
        this.cliente = new Cliente();
        this.posicao = -1;
    }
    
    public void ExecutarCRUD()
    {
        string opcao;
        List<string> opcoes = new List<string>(); 
        opcoes.Add("[1] Cadastrar Cliente");
        opcoes.Add("[2] Editar Cliente   ");
        opcoes.Add("[3] Consultar Cliente");
        opcoes.Add("[4] Listar Clientes  ");
        opcoes.Add("[0] Voltar           ");
        
        while(true)
        {
            tela.LimparJanelaAcao();
            
            opcao = tela.DesenharMenu("GESTÃO DE CLIENTES", opcoes);

            switch (opcao)
            {
                case "1": CadastrarCliente(); break;
                case "2": EditarCliente(); break;
                case "3": ConsultarCliente(); break;
                case "4": ListarClientes(); break;
                case "0": tela.LimparJanelaMenu(); return; 
                default:
                    tela.MostrarMensagemRodape("Opção inválida. Pressione Enter.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void CadastrarCliente()
    {
        tela.DesenharJanelaAcao("CADASTRAR CLIENTE");
        
        this.cliente = new Cliente();
        
        cliente.nome = tela.PerguntarNaAcao(3, "Nome: ");
        cliente.cpf = tela.PerguntarNaAcao(4, "CPF: ");
        cliente.email = tela.PerguntarNaAcao(5, "E-mail: ");
        cliente.telefone = tela.PerguntarNaAcao(6, "Telefone: ");

        if (string.IsNullOrWhiteSpace(cliente.nome) || string.IsNullOrWhiteSpace(cliente.cpf) ||
            string.IsNullOrWhiteSpace(cliente.email) || string.IsNullOrWhiteSpace(cliente.telefone))
        {
            tela.MostrarMensagemRodape("Erro: Todos os campos são obrigatórios. Pressione Enter.");
            Console.ReadKey();
            return; 
        }

        if (ProcurarPorDocumento(cliente.cpf) != null)
        {
            tela.MostrarMensagemRodape("Erro: Este CPF já está cadastrado. Pressione Enter.");
            Console.ReadKey();
            return;
        }
        
        if (ProcurarPorEmail(cliente.email) != null)
        {
            tela.MostrarMensagemRodape("Erro: Este Email já está cadastrado. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        cliente.id = this.proximoID++;
        this.clientes.Add(cliente);
        tela.MostrarMensagemRodape("Cliente cadastrado com sucesso! Pressione Enter.");
        Console.ReadKey();
    }

    private void EditarCliente()
    {
        string doc = tela.PerguntarRodape("Digite o CPF do cliente para editar: ");

        Cliente clienteParaEditar = ProcurarPorDocumento(doc);

        if (clienteParaEditar == null)
        {
            tela.MostrarMensagemRodape("Cliente não encontrado. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        this.posicao = clientes.IndexOf(clienteParaEditar);

        tela.DesenharJanelaAcao("EDITAR CLIENTE");

        int linDiv = 7;
        tela.DesenharDivisoriaAcao(linDiv, " DADOS ATUAIS ");

        tela.EscreverNaAcao(linDiv + 2, $"Nome: {clienteParaEditar.nome}");
        tela.EscreverNaAcao(linDiv + 3, $"CPF: {clienteParaEditar.cpf}");
        tela.EscreverNaAcao(linDiv + 4, $"Email: {clienteParaEditar.email}");
        tela.EscreverNaAcao(linDiv + 5, $"Telefone: {clienteParaEditar.telefone}");

        string nome = tela.PerguntarNaAcao(3, "Novo Nome: ");
        string email = tela.PerguntarNaAcao(4, "Novo E-mail: ");
        string tel = tela.PerguntarNaAcao(5, "Novo Telefone: ");

        if (!string.IsNullOrWhiteSpace(email))
        {
            Cliente emailDuplicado = ProcurarPorEmail(email);
            if (emailDuplicado != null && emailDuplicado.id != clienteParaEditar.id)
            {
                tela.MostrarMensagemRodape("Erro: Este Email já pertence a outro cliente. Pressione Enter.");
                Console.ReadKey();
                return;
            }
            clienteParaEditar.email = email;
        }

        if (!string.IsNullOrWhiteSpace(nome)) clienteParaEditar.nome = nome;
        if (!string.IsNullOrWhiteSpace(tel)) clienteParaEditar.telefone = tel;

        this.clientes[this.posicao] = clienteParaEditar;

        tela.MostrarMensagemRodape("Cliente atualizado com sucesso! Pressione Enter.");
        Console.ReadKey();
    }

    private void ConsultarCliente()
    {
        string doc = tela.PerguntarRodape("Digite o CPF do cliente para consultar: ");

        this.cliente = ProcurarPorDocumento(doc);
        if (this.cliente == null)
        {
            tela.MostrarMensagemRodape("Cliente não encontrado. Pressione Enter.");
            Console.ReadKey();
            return;
        }
        
        tela.DesenharJanelaAcao("CONSULTAR CLIENTE");

        tela.EscreverNaAcao(3, $"ID: {cliente.id}");
        tela.EscreverNaAcao(4, $"Nome: {cliente.nome}");
        tela.EscreverNaAcao(5, $"CPF: {cliente.cpf}");
        tela.EscreverNaAcao(6, $"Email: {cliente.email}");
        tela.EscreverNaAcao(7, $"Telefone: {cliente.telefone}");
        
        tela.MostrarMensagemRodape("Pressione Enter para voltar ao menu de clientes...");
        Console.ReadKey();
    }

    private void ListarClientes()
    {
        tela.DesenharJanelaAcao("LISTAGEM DE CLIENTES");

        if (clientes.Count == 0)
        {
            tela.MostrarMensagemRodape("Nenhum cliente cadastrado. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        int linhaAtual = 3; 
        
        int colId = 2;
        int colNome = 7;
        int colCpf = 28;
        int colEmail = 44;
        int colTel = 65; 

        tela.EscreverNaAcao(linhaAtual, colId, "ID");
        tela.EscreverNaAcao(linhaAtual, colNome, "Nome");
        tela.EscreverNaAcao(linhaAtual, colCpf, "CPF");
        tela.EscreverNaAcao(linhaAtual, colEmail, "E-mail");
        tela.EscreverNaAcao(linhaAtual, colTel, "Telefone");
        linhaAtual++;
        tela.EscreverNaAcao(linhaAtual++, new string('-', 66));
        
        foreach (var c in clientes)
        {
            if (linhaAtual >= 24)
            {
                tela.MostrarMensagemRodape("Muitos clientes para exibir. Pressione Enter...");
                Console.ReadKey();
                tela.LimparJanelaAcao();
                tela.DesenharJanelaAcao("LISTAGEM DE CLIENTES");
                linhaAtual = 3;
            }
            
            tela.EscreverNaAcao(linhaAtual, colId, c.id.ToString());
            tela.EscreverNaAcao(linhaAtual, colNome, c.nome);
            tela.EscreverNaAcao(linhaAtual, colCpf, c.cpf);
            tela.EscreverNaAcao(linhaAtual, colEmail, c.email);
            tela.EscreverNaAcao(linhaAtual, colTel, c.telefone);
            linhaAtual++;
        }

        tela.MostrarMensagemRodape("Pressione Enter para voltar ao menu de clientes...");
        Console.ReadKey();
    }
    
    public Cliente ProcurarPorDocumento(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return null;
        Cliente encontrado = clientes.Find(c => c.cpf == cpf);
        
        if (encontrado != null)
        {
            this.posicao = clientes.IndexOf(encontrado);
        }
        return encontrado;
    }
    
    private Cliente ProcurarPorEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return clientes.Find(c => c.email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}