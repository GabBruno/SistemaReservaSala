Tela tela = new Tela();
ClienteCRUD clienteCRUD = new ClienteCRUD(tela);
SalaCRUD salaCRUD = new SalaCRUD(tela);
RecursoCRUD recursoCRUD = new RecursoCRUD(tela);
ReservaCRUD reservaCRUD = new ReservaCRUD(tela, clienteCRUD, salaCRUD, recursoCRUD);
RelatorioCRUD relatorioCRUD = new RelatorioCRUD(tela, reservaCRUD, salaCRUD);

string opcao;
List<string> opcoes = new List<string>();
opcoes.Add("[1] Gestão de Reservas");
opcoes.Add("[2] Gestão de Clientes");
opcoes.Add("[3] Gestão de Salas   ");
opcoes.Add("[4] Gestão de Recursos");
opcoes.Add("[5] Relatórios        ");
opcoes.Add("[0] Sair do Sistema   ");

while (true)
{
    tela.PrepararTelaPrincipal("Gestão de Aluguéis de Salas de Reunião");
    tela.LimparJanelaMenu(); 
    tela.LimparJanelaAcao(); 
    
    opcao = tela.DesenharMenu("MENU PRINCIPAL", opcoes);
    
    if (opcao == "0") break;
    
    switch(opcao)
    {
        case "1": reservaCRUD.ExecutarCRUD(); break;
        case "2": clienteCRUD.ExecutarCRUD(); break;
        case "3": salaCRUD.ExecutarCRUD(); break;
        case "4": recursoCRUD.ExecutarCRUD(); break;
        case "5": relatorioCRUD.ExecutarCRUD(); break;
        default:
            tela.Pausa("Opção inválida. Pressione Enter.");
            Console.ReadKey();
            break;
    }
}