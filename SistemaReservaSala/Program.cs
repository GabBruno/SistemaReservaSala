Tela tela = new Tela();
ClienteCRUD clienteCRUD = new ClienteCRUD(tela);
//SalaCRUD salaCRUD = new SalaCRUD(tela);
//RecursoCRUD recursoCRUD = new RecursoCRUD(tela);
//RelatorioCRUD relatorioCRUD = new RelatorioCRUD(tela, reservaCRUD, salaCRUD);
//ReservaCRUD reservaCRUD = new ReservaCRUD(tela, clienteCRUD, salaCRUD, recursoCRUD);

string opcao;
List<string> opcoes = new List<string>();
opcoes.Add("       MENU      ");
opcoes.Add("[1] Reservas     ");
opcoes.Add("[2] Clientes     ");
opcoes.Add("[3] Salas        ");
opcoes.Add("[4] Recursos     ");
opcoes.Add("[5] Relatórios   ");
opcoes.Add("[0] Sair         ");

while (true)
{
    tela.PrepararTela("Sistema de Gestão de Salas de reunião");
    opcao = tela.MostrarMenu(opcoes, 2, 2);
    switch(opcao)
    {
        case "0": break;
        //case "1": reservaCRUD.ExecutarCRUD(); break;
        case "2": clienteCRUD.ExecutarCRUD(); break;
        //case "3": salaCRUD.ExecutarCRUD(); break;
        //case "4": recursoCRUD.ExecutarCRUD(); break;
        //case "5": relatorioCRUD.ExecutarCRUD(); break;
        default:
            tela.MostrarMensagem("Opção inválida. Pressione uma tecla para continuar...");
            Console.ReadKey();
            break;
    }
}