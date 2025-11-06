public class ClienteCRUD
{
    private Tela tela;
    private List<Cliente> clientes;
    private Cliente cliente;
    private int posicao;
    private int proximoID = 1;

    public ClienteCRUD(Tela tela)
    {
        this.tela = tela;
        this.clientes = new List<Cliente>();
        this.cliente = new Cliente();
        this.posicao = -1;
    }

    public void ExecutarCRUD()
    {
        string opcao;
        List<string> opcoes = new List<string>();
        opcoes.Add("     MENU    ");
        opcoes.Add("[1] Cadastrar");
        opcoes.Add("[2] Editar   ");
        opcoes.Add("[3] Consultar");
        opcoes.Add("[4] Listar   ");
        opcoes.Add("[0] Sair     ");

        while (true)
        {
            Tela telaCRUD = new Tela(10, 5, 60, 10);
            tela.PrepararTela("Gestão de Clientes");
            opcao = tela.MostrarMenu(opcoes, 10, 5);

            switch (opcao)
            {
                case "0": break;
                case "1": CadastrarCliente(); break;
                case "2": EditarCliente(); break;
                case "3": ConsultarCliente(); break;
                case "4": ListarClientes(); break;
                default:
                    tela.MostrarMensagem("Opção inválida. Pressione Enter.");
                    Console.ReadKey();
                    break;
            }
        }
    }
    
    private void CadastrarCliente()
    {
        tela.PrepararTela("Cadastrar Novo Cliente");
        this.cliente = new Cliente();
        
        Console.SetCursorPosition(2, 5); Console.Write("Nome: ");
        cliente.nome = Console.ReadLine();
        
        Console.SetCursorPosition(2, 6); Console.Write("CPF: ");
        cliente.cpf = Console.ReadLine();
        
        Console.SetCursorPosition(2, 7); Console.Write("E-mail: ");
        cliente.email = Console.ReadLine();

        Console.SetCursorPosition(2, 8); Console.Write("Telefone: ");
        cliente.telefone = Console.ReadLine();

        cliente.id = this.proximoID++;
        this.clientes.Add(cliente);
        tela.MostrarMensagem("Cliente cadastrado com sucesso! Pressione Enter.");
        Console.ReadKey();
    }

    // RF-002: Editar Cliente
    private void EditarCliente()
    {
        tela.PrepararTela("Editar Cliente");
        string doc = tela.Perguntar("Digite o CPF do cliente para editar: ");
        
    
    }

    // RF-003: Consultar Cliente
    private void ConsultarCliente()
    {
        tela.PrepararTela("Consultar Cliente");
        string doc = tela.Perguntar("Digite o CPF ou CNPJ do cliente para consultar: ");
    }

    // RF-004: Listar Clientes
    private void ListarClientes()
    {
        tela.PrepararTela("Listagem de Clientes");
     
    }
    
}