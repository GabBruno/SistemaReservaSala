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
    
    private List<string> opcoes()
    {
        return new List<string>
        {
            "[1] Taxa de Ocupação",
            "[2] Faturamento por Serviço",
            "[0] Voltar"
        };
    }
    
    public void ExecutarCRUD()
    {
        string opcao;
        List<string> opcoes = new List<string>(); 
        opcoes.Add("[1] Taxa de Ocupação       ");
        opcoes.Add("[2] Faturamento por Serviço");
        opcoes.Add("[0] Voltar                 ");
        
        while (true)
        {
            tela.LimparJanelaAcao();
            
            opcao = tela.DesenharMenu("RELATÓRIOS", opcoes);

            switch (opcao)
            {
                case "1": GerarRelatorioTaxaOcupacao(); break;
                case "2": GerarRelatorioFaturamentoServicos(); break;
                case "0": tela.LimparJanelaMenu(); return; 
                default:
                    tela.MostrarMensagemRodape("Opção inválida. Pressione Enter.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void GerarRelatorioTaxaOcupacao()
    {
        tela.DesenharJanelaAcao("TAXA DE OCUPAÇÃO");
        
        var reservas = reservaCRUD.GetReservas().Where(r => r.StatusReserva == "Confirmada").ToList();
        
        if (reservas.Count == 0)
        {
            tela.MostrarMensagemRodape("Nenhuma reserva confirmada para exibir. Pressione Enter.");
            Console.ReadKey();
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

        int linhaAtual = 3;
        
        int colSala = 2;
        int colQtd = 30;
        int colHoras = 48;
        
        tela.EscreverNaAcao(linhaAtual, colSala, "Sala");
        tela.EscreverNaAcao(linhaAtual, colQtd, "Qtd. Reservas");
        tela.EscreverNaAcao(linhaAtual, colHoras, "Total Horas Ocupadas");
        linhaAtual++;
        tela.EscreverNaAcao(linhaAtual++, new string('-', 66));

        foreach (var item in ocupacaoPorSala)
        {
            if (linhaAtual >= 24) 
            {
                tela.MostrarMensagemRodape("Muitos dados para exibir. Pressione Enter...");
                Console.ReadKey();
                tela.LimparJanelaAcao();
                tela.DesenharJanelaAcao("TAXA DE OCUPAÇÃO");
                linhaAtual = 3;
            }
            
            tela.EscreverNaAcao(linhaAtual, colSala, item.Sala);
            tela.EscreverNaAcao(linhaAtual, colQtd, item.QtdReservas.ToString());
            tela.EscreverNaAcao(linhaAtual, colHoras, $"{item.TotalHoras:F1} horas");
            linhaAtual++;
        }
        
        tela.MostrarMensagemRodape("Pressione Enter para voltar ao menu de relatórios...");
        Console.ReadKey();
    }

    private void GerarRelatorioFaturamentoServicos()
    {
        tela.DesenharJanelaAcao("FATURAMENTO POR SERVIÇO");
        
        var reservas = reservaCRUD.GetReservas().Where(r => r.StatusReserva == "Confirmada").ToList();
        
        if (reservas.Count == 0)
        {
            tela.MostrarMensagemRodape("Nenhuma reserva confirmada para exibir. Pressione Enter.");
            Console.ReadKey();
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

        int linhaAtual = 3;
        
        int colRec = 2;
        int colQtd = 30;
        int colValor = 48;
        
        tela.EscreverNaAcao(linhaAtual, colRec, "Recurso Adicional");
        tela.EscreverNaAcao(linhaAtual, colQtd, "Qtd. Vendida");
        tela.EscreverNaAcao(linhaAtual, colValor, "Faturamento Total");
        linhaAtual++;
        tela.EscreverNaAcao(linhaAtual++, new string('-', 66));

        foreach (var item in faturamentoItens)
        {
             if (linhaAtual >= 24)
            {
                tela.MostrarMensagemRodape("Muitos dados para exibir. Pressione Enter...");
                Console.ReadKey();
                tela.LimparJanelaAcao();
                tela.DesenharJanelaAcao("FATURAMENTO POR SERVIÇO");
                linhaAtual = 3;
            }
             
            tela.EscreverNaAcao(linhaAtual, colRec, item.Recurso);
            tela.EscreverNaAcao(linhaAtual, colQtd, item.TotalQtd.ToString());
            tela.EscreverNaAcao(linhaAtual, colValor, $"R$ {item.TotalValor:F2}");
            linhaAtual++;
        }
        
        tela.MostrarMensagemRodape("Pressione Enter para voltar ao menu de relatórios...");
        Console.ReadKey();
    }
}