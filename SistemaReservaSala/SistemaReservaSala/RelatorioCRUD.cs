public class RelatorioCRUD
{
    private Tela tela;
    private ReservaCRUD reservaCRUD;
    private SalaCRUD salaCRUD;

    public RelatorioCRUD(Tela tela, ReservaCRUD resCRUD, SalaCRUD sCRUD)
    {
        this.tela = tela;
        this.reservaCRUD = resCRUD;
        this.salaCRUD = sCRUD;
    }
    
    public void ExecutarCRUD()
    {
        string opcao;
        
        List<string> opcoes = new List<string>(); 
        opcoes.Add("[1] Taxa de Ocupação       ");
        opcoes.Add("[2] Faturamento por Serviço");
        opcoes.Add("[3] Histórico de Reservas  ");
        opcoes.Add("[0] Voltar                 ");
        
        while (true)
        {
            tela.PrepararTelaPrincipal("Gestão de Aluguéis de Salas de Reunião - Relatórios");

            tela.LimparJanelaAcao();
            
            tela.LimparJanelaMenu();
            opcao = tela.DesenharMenu("RELATÓRIOS", opcoes);

            switch (opcao)
            {
                case "1": GerarRelatorioTaxaOcupacao(); break;
                case "2": GerarRelatorioFaturamentoServicos(); break;
                case "3": GerarRelatorioHistorico(); break; 
                case "0": return;
                default:
                    tela.Pausa("Opção inválida. Pressione Enter.");
                    break;
            }
        }
    }

    private void GerarRelatorioTaxaOcupacao()
    {
        tela.PrepararTelaPrincipal("RELATÓRIO: TAXA DE OCUPAÇÃO");
        
        var reservas = reservaCRUD.Reservas().Where(r => r.StatusReserva == "Confirmada").ToList();
        
        if (reservas.Count == 0)
        {
            tela.Pausa("Nenhuma reserva confirmada para exibir. Pressione Enter.");
            return;
        }

        var ocupacaoPorSala = reservas
            .GroupBy(r => r.sala.nome)
            .Select(g => new
            {
                Sala = g.Key,
                TotalHoras = g.Sum(r => (r.DataHoraFim - r.DataHoraInicio).TotalHours),
                QtdReservas = g.Count()
            })
            .OrderByDescending(x => x.TotalHoras);

        int linhaAtual = 4;
        int colSala = 2;
        int colQtd = 30;
        int colHoras = 48;
        
        Console.SetCursorPosition(colSala, linhaAtual); Console.Write("Sala");
        Console.SetCursorPosition(colQtd, linhaAtual); Console.Write("Qtd. Reservas");
        Console.SetCursorPosition(colHoras, linhaAtual); Console.Write("Total Horas Ocupadas");
        linhaAtual++;
        Console.SetCursorPosition(colSala, linhaAtual); Console.Write(new string('─', 100));
        linhaAtual++;

        foreach (var item in ocupacaoPorSala)
        {
            if (linhaAtual >= 25) 
            {
                tela.Pausa("Muitos dados para exibir. Pressione Enter...");
                tela.PrepararTelaPrincipal("TAXA DE OCUPAÇÃO");
                linhaAtual = 5;
            }
            
            Console.SetCursorPosition(colSala, linhaAtual); Console.Write(item.Sala);
            Console.SetCursorPosition(colQtd, linhaAtual); Console.Write(item.QtdReservas.ToString());
            Console.SetCursorPosition(colHoras, linhaAtual); Console.Write($"{item.TotalHoras:F1} horas");
            linhaAtual++;
        }
        
        tela.Pausa("Fim do relatório. Pressione Enter para voltar...");
    }
    private void GerarRelatorioFaturamentoServicos()
    {
        tela.PrepararTelaPrincipal("RELATÓRIO: FATURAMENTO POR SERVIÇO");
        
        var reservas = reservaCRUD.Reservas().Where(r => r.StatusReserva == "Confirmada").ToList();
        
        if (reservas.Count == 0)
        {
            tela.Pausa("Nenhuma reserva confirmada para exibir. Pressione Enter.");
            return;
        }

        var faturamentoItens = reservas
            .SelectMany(r => r.ItensConsumidos) 
            .GroupBy(item => item.Recurso.nome) 
            .Select(g => new
            {
                Recurso = g.Key,
                TotalQtd = g.Sum(item => item.QuantidadeSolicitada),
                TotalValor = g.Sum(item => item.QuantidadeSolicitada * item.Recurso.CustoPorUnidade)
            })
            .OrderByDescending(x => x.TotalValor);

        int linhaAtual = 4;
        int colRec = 2;
        int colQtd = 30;
        int colValor = 48;
        
        Console.SetCursorPosition(colRec, linhaAtual); Console.Write("Recurso Adicional");
        Console.SetCursorPosition(colQtd, linhaAtual); Console.Write("Qtd. Vendida");
        Console.SetCursorPosition(colValor, linhaAtual); Console.Write("Faturamento Total");
        linhaAtual++;
        Console.SetCursorPosition(colRec, linhaAtual); Console.Write(new string('─', 100));
        linhaAtual++;

        foreach (var item in faturamentoItens)
        {
             if (linhaAtual >= 25)
            {
                tela.Pausa("Muitos dados para exibir. Pressione Enter...");
                tela.PrepararTelaPrincipal("FATURAMENTO POR SERVIÇO");
                linhaAtual = 5;
            }
             
            Console.SetCursorPosition(colRec, linhaAtual); Console.Write(item.Recurso);
            Console.SetCursorPosition(colQtd, linhaAtual); Console.Write(item.TotalQtd.ToString());
            Console.SetCursorPosition(colValor, linhaAtual); Console.Write($"R$ {item.TotalValor:F2}");
            linhaAtual++;
        }
        
        tela.Pausa("Fim do relatório. Pressione Enter para voltar...");
    }
    
    private void GerarRelatorioHistorico()
    {
        tela.PrepararTelaPrincipal("RELATÓRIO: HISTÓRICO DE RESERVAS");
        string filtro = tela.PerguntarRodape("Filtrar por Cliente ou Sala (C/S)? ");
        
        List<Reserva> reservasFiltradas = new List<Reserva>();
        string tituloFiltro = "Histórico Completo";
        
        var todasReservas = reservaCRUD.Reservas();

        if (filtro.ToUpper() == "C")
        {
            string cpf = tela.PerguntarRodape("Digite o CPF do Cliente: ");
            reservasFiltradas = todasReservas.Where(r => r.cliente.cpf == cpf).ToList();
            tituloFiltro = $"Histórico para CPF: {cpf}";
        }
        else if (filtro.ToUpper() == "S")
        {
            string nomeSala = tela.PerguntarRodape("Digite o Nome da Sala: ");
            reservasFiltradas = todasReservas.Where(r => r.sala.nome.Equals(nomeSala, StringComparison.OrdinalIgnoreCase)).ToList();
            tituloFiltro = $"Histórico para Sala: {nomeSala}";
        }
        else
        {
            tela.Pausa("Opção de filtro inválida. Pressione Enter.");
            return;
        }
        
        tela.PrepararTelaPrincipal(tituloFiltro);

        if (reservasFiltradas.Count == 0)
        {
            tela.Pausa("Nenhum histórico encontrado para este filtro. Pressione Enter.");
            return;
        }

        int linhaAtual = 4;
        int colId = 2;
        int colDinamica = 8; 
        int colIni = 30;
        int colFim = 52; 
        int colStatus = 74;

        Console.SetCursorPosition(colId, linhaAtual); Console.Write("ID");
        if(filtro.ToUpper() == "C") 
        {
            Console.SetCursorPosition(colDinamica, linhaAtual); Console.Write("Sala");
        }
        else 
        {
            Console.SetCursorPosition(colDinamica, linhaAtual); Console.Write("Cliente");
        }
        Console.SetCursorPosition(colIni, linhaAtual); Console.Write("Início (dd/MM HH:mm)");
        Console.SetCursorPosition(colFim, linhaAtual); Console.Write("Fim (dd/MM HH:mm)");
        Console.SetCursorPosition(colStatus, linhaAtual); Console.Write("Status");
        linhaAtual++;
        Console.SetCursorPosition(colId, linhaAtual); Console.Write(new string('─', 100));
        linhaAtual++;
        
        foreach (var r in reservasFiltradas.OrderBy(r => r.DataHoraInicio))
        {
            if (linhaAtual >= 25) 
            {
                tela.Pausa("Muitos registros. Pressione Enter para continuar...");
                tela.PrepararTelaPrincipal(tituloFiltro);
                linhaAtual = 5;
            }
            
            Console.SetCursorPosition(colId, linhaAtual); Console.Write(r.id.ToString());
            
            if (filtro.ToUpper() == "C")
            {
                Console.SetCursorPosition(colDinamica, linhaAtual); Console.Write(r.sala.nome);
            } 
            else 
            {
                Console.SetCursorPosition(colDinamica, linhaAtual); Console.Write(r.cliente.nome);
            }

            Console.SetCursorPosition(colIni, linhaAtual); Console.Write(r.DataHoraInicio.ToString("dd/MM HH:mm"));
            Console.SetCursorPosition(colFim, linhaAtual); Console.Write(r.DataHoraFim.ToString("dd/MM HH:mm"));
            Console.SetCursorPosition(colStatus, linhaAtual); Console.Write(r.StatusReserva);
            
            linhaAtual++;
        }
        
        tela.Pausa("Fim do relatório. Pressione Enter para voltar...");
    }
}