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
        Cliente cliente = clienteCRUD.ProcurarPorDocumento(doc); 
        if (cliente == null)
        {
            tela.Pausa("Erro: Cliente não cadastrado. Pressione Enter.");
            return;
        }
        this.reserva.cliente = cliente;
        tela.EscreverNaAcao(3, $"CPF do Cliente: {cliente.cpf} - {cliente.nome}");

        string nomeSala = tela.PerguntarNaAcao(4, "Nome da Sala: ");
        Sala sala = salaCRUD.ProcurarPorNome(nomeSala); 
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

        if (reserva.DataHoraInicio < DateTime.Now)
        {
            tela.Pausa("Erro: A data/hora de INÍCIO não pode ser no passado. Pressione Enter.");
            return;
        }
        
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
            tela.Pausa("Erro: Sala ocupada neste período. Pressione Enter.");
            return;
        }
        
        int linhaRecurso = 8;
        while (true)
        {
            if (linhaRecurso > 15)
            {
                tela.Pausa("Limite visual de recursos atingido. Pressione Enter.");
                tela.EscreverNaAcao(linhaRecurso, new string(' ', 60));
                break;
            }
            
            string nomeRecurso = tela.PerguntarNaAcao(linhaRecurso, "Adicionar Recurso (Enter p/ pular): ");
            if (string.IsNullOrWhiteSpace(nomeRecurso))
            {
                 tela.EscreverNaAcao(linhaRecurso, new string(' ', 60));
                 break;
            }

            Recurso recurso = recursoCRUD.ProcurarPorNome(nomeRecurso);
            if (recurso == null)
            {
                tela.Pausa("Recurso não encontrado.");
                tela.EscreverNaAcao(linhaRecurso, new string(' ', 60));
                continue;
            }

            int.TryParse(tela.PerguntarNaAcao(linhaRecurso + 1, $"Qtd de '{recurso.nome}': "), out int qtd);
            if (qtd <= 0)
            {
                tela.EscreverNaAcao(linhaRecurso, new string(' ', 60));
                tela.EscreverNaAcao(linhaRecurso + 1, new string(' ', 60));
                continue; 
            } 

            if (recursoCRUD.VerificarDisponibilidade(recurso, qtd))
            {
                reserva.ItensConsumidos.Add(new ItemReserva(recurso, qtd));
                tela.EscreverNaAcao(linhaRecurso + 1, new string(' ', 60));
                tela.EscreverNaAcao(linhaRecurso, $"> {qtd}x {recurso.nome} (OK)".PadRight(60));
                linhaRecurso++; 
            }
            else
            {
                tela.Pausa($"Estoque insuficiente de '{recurso.nome}'.");
                tela.EscreverNaAcao(linhaRecurso, new string(' ', 60));
                tela.EscreverNaAcao(linhaRecurso + 1, new string(' ', 60));
            }
        }

        reserva.CalcularCustoTotal();

        tela.MostrarMensagemRodape($"Total da Reserva: R$ {reserva.ValorTotalCalculado:F2}. Confirma a criação? (S/N): ");
        string conf = Console.ReadLine();
        tela.MostrarMensagemRodape("");
        
        if (conf.ToUpper() != "S")
        {
             tela.Pausa("Criação de reserva cancelada. Pressione Enter.");
             return;
        }
        
        RegistrarPagamento(reserva, true, linhaRecurso + 1);
        
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

        Reserva res = reservas.Find(r => r.id == idBusca && r.StatusReserva != "Cancelada");
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
        
        string resp = tela.PerguntarRodape("Tem certeza que deseja CANCELAR esta reserva? (S/N): ");
        if (resp.ToUpper() == "S")
        {
            res.StatusReserva = "Cancelada";
            tela.Pausa("Reserva CANCELADA com sucesso. Pressione Enter.");
        }
        else
        {
            tela.Pausa("Operação de cancelamento abortada. Pressione Enter.");
        }
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
        int colSala = 23;
        int colIni = 33;
        int colFim = 48;       
        int colStatus = 64;
        int colPago = 78;
        int colTotal = 91;

        Console.SetCursorPosition(colId, linhaAtual); Console.Write("ID");
        Console.SetCursorPosition(colCli, linhaAtual); Console.Write("Cliente");
        Console.SetCursorPosition(colSala, linhaAtual); Console.Write("Sala");
        Console.SetCursorPosition(colIni, linhaAtual); Console.Write("Início");
        Console.SetCursorPosition(colFim, linhaAtual); Console.Write("Fim");  
        Console.SetCursorPosition(colStatus, linhaAtual); Console.Write("Status");
        Console.SetCursorPosition(colPago, linhaAtual); Console.Write("Val. Pago");
        Console.SetCursorPosition(colTotal, linhaAtual); Console.Write("Val. Total");
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
            Console.SetCursorPosition(colCli, linhaAtual); Console.Write(r.cliente.nome.Length > 14 ? r.cliente.nome.Substring(0, 14) : r.cliente.nome); 
            Console.SetCursorPosition(colSala, linhaAtual); Console.Write(r.sala.nome.Length > 10 ? r.sala.nome.Substring(0, 10) : r.sala.nome);
            Console.SetCursorPosition(colIni, linhaAtual); Console.Write(r.DataHoraInicio.ToString("dd/MM HH:mm"));
            Console.SetCursorPosition(colFim, linhaAtual); Console.Write(r.DataHoraFim.ToString("dd/MM HH:mm")); 
            Console.SetCursorPosition(colStatus, linhaAtual); Console.Write(r.StatusReserva);
            Console.SetCursorPosition(colPago, linhaAtual); Console.Write($"R$ {r.ValorPagoTotal():F2}");
            Console.SetCursorPosition(colTotal, linhaAtual); Console.Write($"R$ {r.ValorTotalCalculado:F2}"); 
            
            linhaAtual++;
        }

        tela.Pausa("Pressione Enter para voltar ao menu de reservas...");
    }
    
    private void RegistrarPagamentoExtra()
    {
        string idBuscaStr = tela.PerguntarRodape("Digite o ID da Reserva para pagar: ");
        int.TryParse(idBuscaStr, out int idBusca);

        Reserva res = reservas.Find(r => r.id == idBusca && r.StatusReserva != "Cancelada");
        if (res == null)
        {
            tela.Pausa("Reserva não encontrada. Pressione Enter.");
            return;
        }
        
        tela.DesenharJanelaAcao("REGISTRAR PAGAMENTO");
        RegistrarPagamento(res, false);
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

    private void RegistrarPagamento(Reserva res, bool inicial, int linhaInicial = 10)
    {
        decimal valorMinimo = res.ValorTotalCalculado * 0.5m;
        decimal valorPendente = res.ValorTotalCalculado - res.ValorPagoTotal();

        if (valorPendente <= 0 && !inicial)
        {
            tela.Pausa("Esta reserva já está totalmente paga. Pressione Enter.");
            return;
        }

        tela.EscreverNaAcao(linhaInicial, $"Valor Total: R$ {res.ValorTotalCalculado:F2}");
        tela.EscreverNaAcao(linhaInicial + 1, $"Valor Pendente: R$ {valorPendente:F2}");
        
        if (inicial)
        {
            tela.EscreverNaAcao(linhaInicial + 2, $"Pagamento inicial (Mínimo 50% = R$ {valorMinimo:F2})");
        }
        else
        {
             tela.EscreverNaAcao(linhaInicial + 2, new string(' ', 60));
        }

        decimal.TryParse(tela.PerguntarNaAcao(linhaInicial + 4, "Valor do Pagamento (R$): "), out decimal valorPago);
        
        if (valorPago <= 0)
        {
            tela.Pausa("Valor inválido. Pressione Enter.");
            return;
        }

        string metodo = tela.PerguntarNaAcao(linhaInicial + 5, "Método (PIX, Cartão, Dinheiro): ");

        string resp = tela.PerguntarRodape($"Confirma pagamento de R$ {valorPago:F2}? (S/N): ");
        if (resp.ToUpper() == "S")
        {
            decimal troco = 0;
            decimal valorARegistrar = valorPago;
            
            if (valorPago > valorPendente)
            {
                troco = valorPago - valorPendente;
                valorARegistrar = valorPendente; 
            }

            Pagamento p = new Pagamento();
            p.id = res.PagamentosRegistrados.Count + 1;
            p.Valor = valorARegistrar; 
            p.Metodo = metodo;
            p.DataPagamento = DateTime.Now;
            res.PagamentosRegistrados.Add(p);
            
            res.AtualizarStatusReserva();

            if (troco > 0)
            {
                tela.Pausa($"Pagamento registrado. TROCO: R$ {troco:F2}. Pressione Enter.");
            }
            else
            {
                tela.Pausa("Pagamento registrado com sucesso! Pressione Enter.");
            }
        }
        else
        {
            tela.Pausa("Pagamento cancelado. Pressione Enter.");
        }
    }
    
    public List<Reserva> Reservas()
    {
        return this.reservas;
    }
}