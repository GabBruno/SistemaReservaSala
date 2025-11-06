using System.Globalization; // utilizado para formatação da data/hora
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
            tela.LimparJanelaAcao();
            opcao = tela.DesenharMenu("GESTÃO DE RESERVAS", opcoes);

            switch (opcao)
            {
                case "1": CriarReserva(); break;
                case "2": CancelarReserva(); break;
                case "3": ListarReservas(); break;
                case "4": RegistrarPagamentoExtra(); break;
                case "0": tela.LimparJanelaMenu(); return; 
                default:
                    tela.MostrarMensagemRodape("Opção inválida. Pressione Enter.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void CriarReserva()
    {
        tela.DesenharJanelaAcao("CRIAR NOVA RESERVA");
        this.reserva = new Reserva();

        string doc = tela.PerguntarNaAcao(3, "CPF do Cliente: ");
        Cliente cliente = clienteCRUD.ProcurarPorDocumento(doc); 
        if (cliente == null)
        {
            tela.MostrarMensagemRodape("Erro: Cliente não cadastrado. Pressione Enter.");
            Console.ReadKey();
            return;
        }
        this.reserva.cliente = cliente;
        tela.EscreverNaAcao(3, $"CPF do Cliente: {cliente.cpf} - {cliente.nome}");

        string nomeSala = tela.PerguntarNaAcao(4, "Nome da Sala: ");
        Sala sala = salaCRUD.ProcurarPorNome(nomeSala); 
        if (sala == null)
        {
            tela.MostrarMensagemRodape("Erro: Sala não encontrada. Pressione Enter.");
            Console.ReadKey();
            return;
        }
        this.reserva.sala = sala;
        tela.EscreverNaAcao(4, $"Nome da Sala: {sala.nome} (R$ {sala.valorHora}/h)");
        
        try
        {
            string dataInicioStr = tela.PerguntarNaAcao(5, "Data/Hora Início (dd/MM/yyyy HH:mm): ");
            reserva.DataHoraInicio = DateTime.ParseExact(dataInicioStr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

            string dataFimStr = tela.PerguntarNaAcao(6, "Data/Hora Fim (dd/MM/yyyy HH:mm): ");
            reserva.DataHoraFim = DateTime.ParseExact(dataFimStr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

            if (reserva.DataHoraFim <= reserva.DataHoraInicio)
            {
                tela.MostrarMensagemRodape("Erro: A data/hora final deve ser maior que a inicial. Pressione Enter.");
                Console.ReadKey();
                return;
            }

            if (!VerificarDisponibilidadeSala(sala, reserva.DataHoraInicio, reserva.DataHoraFim))
            {
                tela.MostrarMensagemRodape("Erro: Sala ocupada neste período (Overbooking). Pressione Enter.");
                Console.ReadKey();
                return;
            }
        }
        catch (FormatException)
        {
            tela.MostrarMensagemRodape("Erro: Formato de data/hora inválido. Use dd/MM/yyyy HH:mm. Pressione Enter.");
            Console.ReadKey();
            return;
        }
        
        int linhaRecurso = 8;
        while (true)
        {
            if (linhaRecurso > 15) 
            {
                tela.MostrarMensagemRodape("Limite de recursos atingido. Pressione Enter.");
                Console.ReadKey();
                break;
            }
            
            string nomeRecurso = tela.PerguntarNaAcao(linhaRecurso, "Adicionar Recurso (Pule com Enter): ");
            if (string.IsNullOrWhiteSpace(nomeRecurso)) break;

            Recurso recurso = recursoCRUD.ProcurarPorNome(nomeRecurso);
            if (recurso == null)
            {
                tela.MostrarMensagemRodape("Recurso não encontrado. Tente novamente.");
                Console.ReadKey();
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
                tela.MostrarMensagemRodape($"Erro: Estoque insuficiente de '{recurso.nome}'. (Disponível: {recurso.QuantidadeEmEstoque})");
                Console.ReadKey();
                tela.ApagarArea(36, 3 + linhaRecurso, 100, 3 + linhaRecurso + 1);
            }
        }

        // calcular custo
        reserva.CalcularCustoTotal();
        tela.MostrarMensagemRodape($"Custo Total da Reserva: R$ {reserva.ValorTotalCalculado:F2}. Pressione Enter para pagar.");
        Console.ReadKey();
        
        // registrar pagamento
        RegistrarPagamento(reserva, true); 
        
        // salvar e baixar estoque
        if (reserva.StatusReserva == "Confirmada") 
        {
            reserva.id = this.proximoID++;
            this.reservas.Add(reserva);
            
            foreach (var item in reserva.ItensConsumidos)
            {
                recursoCRUD.BaixarEstoque(item.Recurso, item.QuantidadeSolicitada);
            }

            tela.MostrarMensagemRodape($"Reserva (ID {reserva.id}) criada. Status: {reserva.StatusReserva}. Pressione Enter.");
            Console.ReadKey();
        }
        else
        {
            tela.MostrarMensagemRodape("Pagamento mínimo não efetuado. Reserva não confirmada. Pressione Enter.");
            Console.ReadKey();
        }
    }

    // cancelamento
    private void CancelarReserva()
    {
        string idBuscaStr = tela.PerguntarRodape("Digite o ID da Reserva para cancelar: ");
        int.TryParse(idBuscaStr, out int idBusca);

        Reserva res = reservas.Find(r => r.id == idBusca && r.StatusReserva != "Cancelada");
        if (res == null)
        {
            tela.MostrarMensagemRodape("Reserva não encontrada ou já cancelada. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        tela.DesenharJanelaAcao("CANCELAR RESERVA");
        tela.EscreverNaAcao(3, $"Cliente: {res.cliente.nome}");
        tela.EscreverNaAcao(4, $"Sala: {res.sala.nome}");
        tela.EscreverNaAcao(5, $"Início: {res.DataHoraInicio:g}");
        tela.EscreverNaAcao(6, $"Valor Pago: R$ {res.GetValorPagoTotal():F2}");

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
        
        tela.MostrarMensagemRodape("Reserva cancelada. Pressione Enter.");
        Console.ReadKey();
    }
    
    private void ListarReservas()
    {
        tela.DesenharJanelaAcao("LISTAGEM DE RESERVAS ATIVAS");
        var reservasAtivas = reservas.Where(r => r.StatusReserva != "Cancelada" && r.DataHoraFim > DateTime.Now).ToList();
        
        if (reservasAtivas.Count == 0)
        {
            tela.MostrarMensagemRodape("Nenhuma reserva ativa encontrada. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        int linhaAtual = 3;
        
        int colId = 2;
        int colCli = 7;
        int colSala = 23;
        int colIni = 39;
        int colStatus = 57;

        tela.EscreverNaAcao(linhaAtual, colId, "ID");
        tela.EscreverNaAcao(linhaAtual, colCli, "Cliente");
        tela.EscreverNaAcao(linhaAtual, colSala, "Sala");
        tela.EscreverNaAcao(linhaAtual, colIni, "Início (dd/MM HH:mm)");
        tela.EscreverNaAcao(linhaAtual, colStatus, "Status");
        linhaAtual++;
        tela.EscreverNaAcao(linhaAtual++, new string('-', 66));
        
        foreach (var r in reservasAtivas.OrderBy(r => r.DataHoraInicio))
        {
            if (linhaAtual >= 24) 
            {
                tela.MostrarMensagemRodape("Muitas reservas para exibir. Pressione Enter...");
                Console.ReadKey();
                tela.LimparJanelaAcao();
                tela.DesenharJanelaAcao("LISTAGEM DE RESERVAS ATIVAS");
                linhaAtual = 3;
            }
            
            tela.EscreverNaAcao(linhaAtual, colId, r.id.ToString());
            tela.EscreverNaAcao(linhaAtual, colCli, r.cliente.nome);
            tela.EscreverNaAcao(linhaAtual, colSala, r.sala.nome);
            tela.EscreverNaAcao(linhaAtual, colIni, r.DataHoraInicio.ToString("dd/MM HH:mm"));
            tela.EscreverNaAcao(linhaAtual, colStatus, r.StatusReserva);
            linhaAtual++;
        }

        tela.MostrarMensagemRodape("Pressione Enter para voltar ao menu de reservas...");
        Console.ReadKey();
    }

    private void RegistrarPagamentoExtra()
    {
        string idBuscaStr = tela.PerguntarRodape("Digite o ID da Reserva para pagar: ");
        int.TryParse(idBuscaStr, out int idBusca);

        Reserva res = reservas.Find(r => r.id == idBusca && r.StatusReserva != "Cancelada");
        if (res == null)
        {
            tela.MostrarMensagemRodape("Reserva não encontrada. Pressione Enter.");
            Console.ReadKey();
            return;
        }

        tela.DesenharJanelaAcao("REGISTRAR PAGAMENTO");
        RegistrarPagamento(res, false);

        tela.MostrarMensagemRodape($"Pagamento registrado. Novo status: {res.StatusReserva}. Pressione Enter.");
        Console.ReadKey();
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
        decimal valorPendente = res.ValorTotalCalculado - res.GetValorPagoTotal();

        tela.EscreverNaAcao(10, $"Valor Total: R$ {res.ValorTotalCalculado:F2}");
        tela.EscreverNaAcao(11, $"Valor Pendente: R$ {valorPendente:F2}");
        
        if (inicial)
        {
            tela.EscreverNaAcao(12, $"Pagamento inicial (Mínimo 50% = R$ {valorMinimo:F2})");
        }

        decimal.TryParse(tela.PerguntarNaAcao(14, "Valor do Pagamento (R$): "), out decimal valorPago);
        
        if (valorPago <= 0)
        {
            tela.MostrarMensagemRodape("Valor inválido. Pressione Enter.");
            Console.ReadKey();
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
    
    public List<Reserva> GetReservas()
    {
        return this.reservas;
    }
}