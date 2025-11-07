using System.Globalization;

public class ReservaCRUD
{
    private Tela tela;
    private List<Reserva> reservas;
    private Reserva reserva;
    private int proximoID = 1;    
    private ClienteCRUD clienteCRUD;
    private SalaCRUD salaCRUD;
    private RecursoCRUD recursoCRUD;

    public ReservaCRUD(Tela tela, ClienteCRUD cCRUD, SalaCRUD sCRUD, RecursoCRUD rCRUD)
    {
        this.tela = tela;
        this.clienteCRUD = cCRUD;
        this.salaCRUD = sCRUD;
        this.recursoCRUD = rCRUD;
        
        this.reservas = new List<Reserva>();
        this.reserva = new Reserva();
    }

    public void ExecutarCRUD()
    {
        string opcao;
        List<string> opcoes = new List<string>(); 
        opcoes.Add("[1] Reservar Sala         ");
        opcoes.Add("[2] Cancelar Reserva      ");
        opcoes.Add("[3] Listar Reservas Ativas");
        opcoes.Add("[4] Registrar Pagamento   ");
        opcoes.Add("[0] Voltar                ");
        
        while (true)
        {
            tela.PrepararTelaPrincipal("Gestão de Aluguéis de Salas de Reunião - Gestão de Reservas");

            tela.LimparJanelaAcao();
            
            tela.LimparJanelaMenu();
            opcao = tela.DesenharMenu("GESTÃO DE RESERVAS", opcoes);

            switch (opcao)
            {
                case "1": CriarReserva(); break;
                case "2": CancelarReserva(); break;
                case "3": ListarReservas(); break; 
                case "4": RegistrarPagamentoExtra(); break;
                case "0": return; 
                default:tela.Pausa("Opção inválida. Pressione Enter.");break;
            }
        }
    }

    private void CriarReserva()
    {
        tela.DesenharJanelaAcao("RESERVAR SALA");
        this.reserva = new Reserva();

        string doc = tela.PerguntarNaAcao(3, "CPF do Cliente: ");
        Cliente? cliente = clienteCRUD.ProcurarPorDocumento(doc); 
        if (cliente == null)
        {
            tela.Pausa("Erro: Cliente não cadastrado. Pressione Enter.");
            return;
        }
        this.reserva.cliente = cliente;
        tela.EscreverNaAcao(3, $"CPF do Cliente: {cliente.cpf} - {cliente.nome}");

        string nomeSala = tela.PerguntarNaAcao(4, "Nome da Sala: ");
        Sala? sala = salaCRUD.ProcurarPorNome(nomeSala); 
        if (sala == null)
        {
            tela.Pausa("Erro: Sala não encontrada. Pressione Enter.");
            return;
        }
        this.reserva.sala = sala;
        tela.EscreverNaAcao(4, $"Nome da Sala: {sala.nome} (R$ {sala.valorHora}/h)");
        
        DateTime dataInicio;
        string dataInicioStr = tela.PerguntarNaAcao(5, "Data/Hora Início (dd/MM/yyyy HH:mm): ");
        if (!DateTime.TryParseExact(dataInicioStr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataInicio))
        {
            tela.Pausa("Erro: Formato de data/hora de INÍCIO inválido. Use dd/MM/yyyy HH:mm. Pressione Enter.");
            return;
        }

        DateTime dataFim;
        string dataFimStr = tela.PerguntarNaAcao(6, "Data/Hora Fim (dd/MM/yyyy HH:mm): ");
        if (!DateTime.TryParseExact(dataFimStr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataFim))
        {
            tela.Pausa("Erro: Formato de data/hora de FIM inválido. Use dd/MM/yyyy HH:mm. Pressione Enter.");
            return;
        }

        reserva.DataHoraInicio = dataInicio;
        reserva.DataHoraFim = dataFim;

        if (reserva.DataHoraFim <= reserva.DataHoraInicio)
        {
            tela.Pausa("Erro: A data/hora final deve ser maior que a inicial. Pressione Enter.");
            return;
        }
        
        TimeSpan inicioOperacao = new TimeSpan(8, 0, 0); 
        TimeSpan maxInicioOperacao = new TimeSpan(21, 30, 0);
        TimeSpan minFimOperacao = new TimeSpan(8, 30, 0); 
        TimeSpan fimOperacao = new TimeSpan(22, 0, 0);   

        if (dataInicio.DayOfWeek == DayOfWeek.Sunday || dataFim.DayOfWeek == DayOfWeek.Sunday)
        {
            tela.Pausa("Erro: O espaço não opera aos domingos. Pressione Enter.");
            return;
        }
        if (dataInicio.TimeOfDay < inicioOperacao || dataInicio.TimeOfDay > maxInicioOperacao)
        {
            tela.Pausa("Erro: O horário de INÍCIO deve ser entre 8h00 e 21h30. Pressione Enter.");
            return;
        }
        if (dataFim.TimeOfDay < minFimOperacao || dataFim.TimeOfDay > fimOperacao)
        {
            tela.Pausa("Erro: O horário de TÉRMINO deve ser entre 8h30 e 22h00. Pressione Enter.");
            return;
        }
        
        TimeSpan duracao = reserva.DataHoraFim - reserva.DataHoraInicio;
        if (duracao.TotalMinutes % 30 != 0)
        {
            tela.Pausa("Erro: O período (duração) da reserva deve ser em múltiplos de 30 minutos. Pressione Enter.");
            return;
        }

        if (!VerificarDisponibilidadeSala(sala, reserva.DataHoraInicio, reserva.DataHoraFim))
        {
            tela.Pausa("Erro: Sala ocupada neste período (Overbooking). Pressione Enter.");
            return;
        }
        
        int linhaRecurso = 8;
        while (true)
        {
            if (linhaRecurso > 15) 
            {
                tela.Pausa("Limite de recursos atingido. Pressione Enter.");
                break;
            }
            
            string nomeRecurso = tela.PerguntarNaAcao(linhaRecurso, "Adicionar Recurso (Pule com Enter): ");
            if (string.IsNullOrWhiteSpace(nomeRecurso)) break;

            Recurso? recurso = recursoCRUD.ProcurarPorNome(nomeRecurso);
            if (recurso == null)
            {
                tela.Pausa("Recurso não encontrado. Tente novamente.");
                tela.ApagarArea(36, 3 + linhaRecurso, 100, 3 + linhaRecurso);
                continue;
            }

            int.TryParse(tela.PerguntarNaAcao(linhaRecurso + 1, $"Qtd de '{recurso.nome}': "), out int qtd);
            if (qtd <= 0)
            {
                linhaRecurso += 2; 
                continue;
            } 

            if (recursoCRUD.VerificarDisponibilidade(recurso, qtd))
            {
                reserva.ItensConsumidos.Add(new ItemReserva(recurso, qtd));
                tela.EscreverNaAcao(linhaRecurso, $"Recurso: {qtd}x {recurso.nome} adicionados.");
                linhaRecurso += 2; 
            }
            else
            {
                tela.Pausa($"Erro: Estoque insuficiente de '{recurso.nome}'. (Disponível: {recurso.QuantidadeEmEstoque})");
                tela.ApagarArea(36, 3 + linhaRecurso, 100, 3 + linhaRecurso + 1);
            }
        }

        reserva.CalcularCustoTotal();
        tela.MostrarMensagemRodape($"Custo Total da Reserva: R$ {reserva.ValorTotalCalculado:F2}. Pressione Enter para pagar.");
        Console.ReadKey();
        tela.MostrarMensagemRodape(""); 
        
        RegistrarPagamento(reserva, true); 
        
        if (reserva.StatusReserva == "Confirmada") 
        {
            reserva.id = this.proximoID++;
            this.reservas.Add(reserva);
            
            foreach (var item in reserva.ItensConsumidos)
            {
                recursoCRUD.BaixarEstoque(item.Recurso, item.QuantidadeSolicitada);
            }

            tela.Pausa($"Reserva (ID {reserva.id}) criada. Status: {reserva.StatusReserva}. Pressione Enter.");
        }
        else
        {
            tela.Pausa("Pagamento mínimo não efetuado. Reserva não confirmada. Pressione Enter.");
        }
    }

    private void CancelarReserva()
    {
        string idBuscaStr = tela.PerguntarRodape("Digite o ID da Reserva para cancelar: ");
        int.TryParse(idBuscaStr, out int idBusca);

        Reserva? res = reservas.Find(r => r.id == idBusca && r.StatusReserva != "Cancelada");
        if (res == null)
        {
            tela.Pausa("Reserva não encontrada ou já cancelada. Pressione Enter.");
            return;
        }

        tela.DesenharJanelaAcao("CANCELAR RESERVA");
        tela.EscreverNaAcao(3, $"Cliente: {res.cliente.nome}");
        tela.EscreverNaAcao(4, $"Sala: {res.sala.nome}");
        tela.EscreverNaAcao(5, $"Início: {res.DataHoraInicio:g}");
        tela.EscreverNaAcao(6, $"Valor Pago: R$ {res.ValorPagoTotal()}");

        TimeSpan tempoAteInicio = res.DataHoraInicio - DateTime.Now;

        if (tempoAteInicio.TotalHours < 24)
        {
            tela.EscreverNaAcao(8, "Cancelamento com menos de 24h.");
            tela.EscreverNaAcao(9, "Tarifa de 100% da Tarifa Base será retida.");
        }
        else
        {
            tela.EscreverNaAcao(8, "Reserva cancelada (dentro do prazo).");
        }
        
        res.StatusReserva = "Cancelada";
        
        tela.Pausa("Reserva cancelada. Pressione Enter.");
    }
    
     private void ListarReservas()
    {
        tela.PrepararTelaPrincipal("LISTAGEM DE RESERVAS ATIVAS");
        
        var reservasAtivas = reservas.Where(r => r.StatusReserva != "Cancelada" && r.DataHoraFim > DateTime.Now).ToList();
        
        if (reservasAtivas.Count == 0)
        {
            tela.Pausa("Nenhuma reserva ativa encontrada. Pressione Enter.");
            return;
        }

        int linhaAtual = 4;
        
        int colId = 2;
        int colCli = 7;
        int colSala = 28;
        int colIni = 44;
        int colStatus = 68;
        int colPago = 82;

        Console.SetCursorPosition(colId, linhaAtual); Console.Write("ID");
        Console.SetCursorPosition(colCli, linhaAtual); Console.Write("Cliente");
        Console.SetCursorPosition(colSala, linhaAtual); Console.Write("Sala");
        Console.SetCursorPosition(colIni, linhaAtual); Console.Write("Início (dd/MM HH:mm)");
        Console.SetCursorPosition(colStatus, linhaAtual); Console.Write("Status");
        Console.SetCursorPosition(colPago, linhaAtual); Console.Write("Valor Pago");
        linhaAtual++;
        Console.SetCursorPosition(colId, linhaAtual); Console.Write(new string('─', 100));
        linhaAtual++;
        
        foreach (var r in reservasAtivas.OrderBy(r => r.DataHoraInicio))
        {
            if (linhaAtual >= 25) 
            {
                tela.Pausa("Muitas reservas para exibir. Pressione Enter...");
                tela.PrepararTelaPrincipal("LISTAGEM DE RESERVAS ATIVAS");
                linhaAtual = 5;
            }
            
            Console.SetCursorPosition(colId, linhaAtual); Console.Write(r.id.ToString());
            Console.SetCursorPosition(colCli, linhaAtual); Console.Write(r.cliente.nome);
            Console.SetCursorPosition(colSala, linhaAtual); Console.Write(r.sala.nome);
            Console.SetCursorPosition(colIni, linhaAtual); Console.Write(r.DataHoraInicio.ToString("dd/MM HH:mm"));
            Console.SetCursorPosition(colStatus, linhaAtual); Console.Write(r.StatusReserva);
            Console.SetCursorPosition(colPago, linhaAtual); Console.Write($"R$ {r.ValorPagoTotal():F2}");
            linhaAtual++;
        }

        tela.Pausa("Pressione Enter para voltar ao menu de reservas...");
    }
    
    private void RegistrarPagamentoExtra()
    {
        string idBuscaStr = tela.PerguntarRodape("Digite o ID da Reserva para pagar: ");
        int.TryParse(idBuscaStr, out int idBusca);

        Reserva? res = reservas.Find(r => r.id == idBusca && r.StatusReserva != "Cancelada");
        if (res == null)
        {
            tela.Pausa("Reserva não encontrada. Pressione Enter.");
            return;
        }
        
        tela.DesenharJanelaAcao("REGISTRAR PAGAMENTO");
        RegistrarPagamento(res, false);
        
        tela.Pausa($"Pagamento registrado. Novo status: {res.StatusReserva}. Pressione Enter.");
    }

    private bool VerificarDisponibilidadeSala(Sala sala, DateTime inicio, DateTime fim)
    {
        foreach (var res in reservas)
        {
            if (res.sala.id != sala.id || res.StatusReserva == "Cancelada")
                continue;
            
            if (inicio < res.DataHoraFim && fim > res.DataHoraInicio)
            {
                return false; 
            }
        }
        return true; 
    }

    private void RegistrarPagamento(Reserva res, bool inicial)
    {
        decimal valorMinimo = res.ValorTotalCalculado * 0.5m;
        decimal valorPendente = res.ValorTotalCalculado - res.ValorPagoTotal();

        tela.EscreverNaAcao(10, $"Valor Total: R$ {res.ValorTotalCalculado:F2}");
        tela.EscreverNaAcao(11, $"Valor Pendente: R$ {valorPendente:F2}");
        
        if (inicial)
        {
            tela.EscreverNaAcao(12, $"Pagamento inicial (Mínimo 50% = R$ {valorMinimo:F2})");
        }

        decimal.TryParse(tela.PerguntarNaAcao(14, "Valor do Pagamento (R$): "), out decimal valorPago);
        
        if (valorPago <= 0)
        {
            tela.Pausa("Valor inválido. Pressione Enter.");
            return;
        }

        string metodo = tela.PerguntarNaAcao(15, "Método (PIX, Cartão, Dinheiro): ");

        Pagamento p = new Pagamento();
        p.id = res.PagamentosRegistrados.Count + 1;
        p.Valor = valorPago;
        p.Metodo = metodo;
        p.DataPagamento = DateTime.Now;
        res.PagamentosRegistrados.Add(p);
        
        res.AtualizarStatusReserva();
    }
    
    public List<Reserva> Reservas()
    {
        return this.reservas;
    }
}