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
        
        this.clientes.Add(new Cliente(proximoID++, "Joao", "1", "joao@gmail.com", "(47) 93456-7890"));
        this.clientes.Add(new Cliente(proximoID++, "Maria", "2", "maria@gmail.com", "(11) 96654-3210"));
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
            tela.PrepararTelaPrincipal("Gestão de Aluguéis de Salas de Reunião - Gestão de Clientes");

            tela.LimparJanelaAcao();
            
            tela.LimparJanelaMenu();
            opcao = tela.DesenharMenu("GESTÃO DE CLIENTES", opcoes);

            switch (opcao)
            {
                case "1": CadastrarCliente(); break;
                case "2": EditarCliente(); break;
                case "3": ConsultarCliente(); break;
                case "4": ListarClientes(); break;
                case "0": return; 
                default:tela.Pausa("Opção inválida. Pressione Enter."); break;
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
            tela.Pausa("Erro: Todos os campos são obrigatórios. Pressione Enter.");
            return; 
        }

        if (ProcurarPorDocumento(cliente.cpf) != null)
        {
            tela.Pausa("Erro: Este CPF já está cadastrado. Pressione Enter.");
            return;
        }
        
        if (ProcurarPorEmail(cliente.email) != null)
        {
            tela.Pausa("Erro: Este Email já está cadastrado. Pressione Enter.");
            return;
        }

        string resp = tela.PerguntarRodape("Confirma o cadastro deste cliente? (S/N): ");
        if (resp.ToUpper() == "S")
        {
            cliente.id = this.proximoID++;
            this.clientes.Add(cliente);
            tela.Pausa("Cliente cadastrado com sucesso! Pressione Enter.");
        }
        else
        {
            tela.Pausa("Cadastro cancelado. Pressione Enter.");
        }
    }

    private void EditarCliente()
    {
        string doc = tela.PerguntarRodape("Digite o CPF do cliente para editar: ");
        
        Cliente clienteParaEditar = ProcurarPorDocumento(doc);
        
        if (clienteParaEditar == null)
        {
            tela.Pausa("Cliente não encontrado. Pressione Enter.");
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
                 tela.Pausa("Erro: Este Email já pertence a outro cliente. Pressione Enter.");
                 return;
            }
        }


        string resp = tela.PerguntarRodape("Confirma as alterações? (S/N): ");
        if (resp.ToUpper() == "S")
        {
            if (!string.IsNullOrWhiteSpace(nome)) clienteParaEditar.nome = nome;
            clienteParaEditar.email = email;
            if (!string.IsNullOrWhiteSpace(tel)) clienteParaEditar.telefone = tel;

            this.clientes[this.posicao] = clienteParaEditar;
            tela.Pausa("Cliente atualizado com sucesso! Pressione Enter.");
        }
        else
        {
            tela.Pausa("Alteração cancelada. Pressione Enter.");
        }
    }
    private void ConsultarCliente()
    {
        string doc = tela.PerguntarRodape("Digite o CPF do cliente para consultar: ");

        this.cliente = ProcurarPorDocumento(doc);
        if (this.cliente == null)
        {
            tela.Pausa("Cliente não encontrado. Pressione Enter.");
            return;
        }
        
        tela.DesenharJanelaAcao("CONSULTAR CLIENTE");
        tela.EscreverNaAcao(3, $"ID: {cliente.id}");
        tela.EscreverNaAcao(4, $"Nome: {cliente.nome}");
        tela.EscreverNaAcao(5, $"CPF: {cliente.cpf}");
        tela.EscreverNaAcao(6, $"Email: {cliente.email}");
        tela.EscreverNaAcao(7, $"Telefone: {cliente.telefone}");
        
        tela.Pausa("Pressione Enter para voltar ao menu de clientes...");
    }

    private void ListarClientes()
    {
        tela.PrepararTelaPrincipal("LISTAGEM DE CLIENTES");

        if (clientes.Count == 0)
        {
            tela.Pausa("Nenhum cliente cadastrado. Pressione Enter.");
            return;
        }

        int linhaAtual = 4; 
        
        int colId = 2;
        int colNome = 8;
        int colCpf = 35;
        int colEmail = 55;
        int colTel = 85; 

        Console.SetCursorPosition(colId, linhaAtual); Console.Write("ID");
        Console.SetCursorPosition(colNome, linhaAtual); Console.Write("Nome");
        Console.SetCursorPosition(colCpf, linhaAtual); Console.Write("CPF");
        Console.SetCursorPosition(colEmail, linhaAtual); Console.Write("E-mail");
        Console.SetCursorPosition(colTel, linhaAtual); Console.Write("Telefone");
        linhaAtual++;
        Console.SetCursorPosition(colId, linhaAtual); Console.Write(new string('─', 100));
        linhaAtual++;
        
        foreach (var c in clientes)
        {
            if (linhaAtual >= 25) 
            {
                tela.Pausa("Muitos clientes para exibir. Pressione Enter...");
                tela.PrepararTelaPrincipal("LISTAGEM DE CLIENTES");
                linhaAtual = 5; 
            }
            
            Console.SetCursorPosition(colId, linhaAtual); Console.Write(c.id.ToString());
            Console.SetCursorPosition(colNome, linhaAtual); Console.Write(c.nome);
            Console.SetCursorPosition(colCpf, linhaAtual); Console.Write(c.cpf);
            Console.SetCursorPosition(colEmail, linhaAtual); Console.Write(c.email);
            Console.SetCursorPosition(colTel, linhaAtual); Console.Write(c.telefone);
            linhaAtual++;
        }

        tela.Pausa("Pressione Enter para voltar ao menu de clientes...");
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