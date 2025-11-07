public class Tela
{
    private int larguraTotal = 102; 
    private int alturaTotal = 28;  
    private int menuCol = 2;
    private int menuLin = 3;
    private int menuLarg = 30; 
    private int menuAlt = 22;  
    private int acaoCol; 
    private int acaoLin = 3;
    private int acaoLarg; 
    private int acaoAlt = 22; 

    public Tela()
    {
        this.acaoCol = menuCol + menuLarg + 2;
        this.acaoLarg = larguraTotal - acaoCol - 2; 

        Console.SetWindowSize(this.larguraTotal + 1, this.alturaTotal + 1);
        Console.Clear();
    }

    public void PrepararTelaPrincipal(string titulo)
    {
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();
        
        MontarMoldura(0, 0, larguraTotal, alturaTotal);
        MontarMoldura(0, 0, larguraTotal, 2);
        MontarMoldura(0, alturaTotal - 2, larguraTotal, alturaTotal);
        Centralizar(0, larguraTotal, 1, titulo);
    }
    
    public string DesenharMenu(string titulo, List<string> opcoes)
    {
        int alturaDinamica = opcoes.Count + 4;
        
        LimparJanelaMenu();
        
        MontarJanela(titulo, menuCol, menuLin, menuLarg, alturaDinamica);
        
        int linha = menuLin + 2;
        for (int i = 0; i < opcoes.Count; i++)
        {
            Console.SetCursorPosition(menuCol + 2, linha); 
            Console.Write(opcoes[i]);
            linha++;
        }
        
        DesenharDivisoriaHorizontal(menuCol, menuLin + alturaDinamica - 2, menuLarg);
        
        Console.SetCursorPosition(menuCol + 2, menuLin + alturaDinamica - 1); 
        Console.Write("OPÇÃO : ");
        
        return Console.ReadLine();
    }

    public void DesenharJanelaAcao(string titulo)
    {
        LimparJanelaAcao();
        MontarJanela(titulo, acaoCol, acaoLin, acaoLarg, acaoAlt);
    }
    
    public string MostrarSubMenu(string titulo, List<string> opcoes)
    {
        LimparJanelaAcao();
        int alturaDinamica = opcoes.Count + 4;
        MontarJanela(titulo, acaoCol, acaoLin, acaoLarg, alturaDinamica);
        
        int linha = acaoLin + 2;
        for (int i = 0; i < opcoes.Count; i++)
        {
            Console.SetCursorPosition(acaoCol + 2, linha); 
            Console.Write(opcoes[i]);
            linha++;
        }
        
        DesenharDivisoriaHorizontal(acaoCol, acaoLin + alturaDinamica - 2, acaoLarg);

        Console.SetCursorPosition(acaoCol + 2, acaoLin + alturaDinamica - 1);
        Console.Write("OPÇÃO : ");
        
        return Console.ReadLine() ;
    }
    
    public string PerguntarNaAcao(int linhaRelativa, string pergunta)
    {
        int col = this.acaoCol + 2;
        int lin = this.acaoLin + linhaRelativa;
        Console.SetCursorPosition(col, lin);
        Console.Write(pergunta);
        return Console.ReadLine() ;
    }
    
    public void EscreverNaAcao(int linhaRelativa, string texto)
    {
        EscreverNaAcao(linhaRelativa, 2, texto);
    }
    
    public void EscreverNaAcao(int linhaRelativa, int colRelativa, string texto)
    {
        int col = this.acaoCol + colRelativa;
        int lin = this.acaoLin + linhaRelativa;
        
        int maxWidth = (this.acaoCol + this.acaoLarg) - col - 1;
        if(maxWidth < 0) maxWidth = 0;

        if (texto.Length > maxWidth)
        {
            texto = texto.Substring(0, maxWidth);
        }
        
        Console.SetCursorPosition(col, lin);
        Console.Write(texto);
    }

    public void LimparJanelaAcao()
    {
        ApagarArea(acaoCol, acaoLin, acaoCol + acaoLarg, acaoLin + acaoAlt);
    }
    
    public void LimparJanelaMenu()
    {
         ApagarArea(menuCol, menuLin, menuCol + menuLarg, menuLin + menuAlt);
    }

    public void Centralizar(int ci, int cf, int lin, string msg)
    {
        int col = (cf - ci - msg.Length) / 2 + ci;
        Console.SetCursorPosition(col, lin);
        Console.Write(msg);
    }

    public void ApagarArea(int ci, int li, int cf, int lf)
    {
        string linhaVazia = new string(' ', cf - ci + 1);
        for (int i = li; i <= lf; i++)
        {
            Console.SetCursorPosition(ci, i);
            Console.Write(linhaVazia);
        }
    }

    public void MontarMoldura(int ci, int li, int cf, int lf)
    {
        int col, lin;
        this.ApagarArea(ci, li, cf, lf);

        for (col = ci + 1; col < cf; col++) 
        {
            Console.SetCursorPosition(col, li); Console.Write("═");
            Console.SetCursorPosition(col, lf); Console.Write("═");
        }
        for (lin = li + 1; lin < lf; lin++) 
        {
            Console.SetCursorPosition(ci, lin); Console.Write("║");
            Console.SetCursorPosition(cf, lin); Console.Write("║");
        }
        Console.SetCursorPosition(ci, li); Console.Write("╔");
        Console.SetCursorPosition(ci, lf); Console.Write("╚");
        Console.SetCursorPosition(cf, li); Console.Write("╗");
        Console.SetCursorPosition(cf, lf); Console.Write("╝");
    }
    
    public void MontarJanela(string titulo, int coluna, int linha, int largura, int altura)
    {
        this.MontarMoldura(coluna, linha, coluna + largura, linha + altura);
        
        string tituloFormatado = $" {titulo} ";
        int colTitulo = coluna + (largura - tituloFormatado.Length) / 2;

        Console.SetCursorPosition(colTitulo, linha);
        Console.Write(tituloFormatado);
        Console.SetCursorPosition(colTitulo - 1, linha); Console.Write("╣");
        Console.SetCursorPosition(colTitulo + tituloFormatado.Length, linha); Console.Write("╠");
    }
    
    public void DesenharDivisoriaHorizontal(int col, int lin, int larg, string titulo = "")
    {
        int ci = col;
        int cf = col + larg;
        int li = lin;
        
        Console.SetCursorPosition(ci, li); Console.Write("╠");
        for (int c = ci + 1; c < cf; c++)
        {
            Console.Write("─");
        }
        Console.SetCursorPosition(cf, li); Console.Write("╣");
        
        if (!string.IsNullOrEmpty(titulo))
        {
             Centralizar(ci, cf, li, $" {titulo} ");
        }
    }
    
    public void DesenharDivisoriaAcao(int linhaRelativa, string titulo)
    {
        DesenharDivisoriaHorizontal(this.acaoCol, this.acaoLin + linhaRelativa, this.acaoLarg, titulo);
    }

    public void MostrarMensagemRodape(string msg)
    {
        this.ApagarArea(1, alturaTotal - 1, larguraTotal - 1, alturaTotal - 1);
        int coluna = (larguraTotal - msg.Length) / 2;
        Console.SetCursorPosition(coluna, alturaTotal - 1);
        Console.Write(msg);
    }

    public void Pausa(string msg)
    {
        MostrarMensagemRodape(msg);
        Console.ReadKey();
        MostrarMensagemRodape("");
    }

    public string PerguntarRodape(string pergunta)
    {
        string resp = "";
        this.MostrarMensagemRodape(pergunta);
        resp = Console.ReadLine() ;
        this.MostrarMensagemRodape(""); 
        return resp;
    }
}