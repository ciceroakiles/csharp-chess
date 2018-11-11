class Cavalo : Peca {
    public Cavalo(Tabuleiro tab, bool cor) : base(tab, cor) {
    }

    public override string ToString() {
        return Tela.NOMES[1];
    }

    public override bool[,] movPossiveis() {
        bool[,] matriz = new bool[tab.pecas.GetLength(0), tab.pecas.GetLength(1)];
        validarJogada((char)((int)pos.x+1), pos.y+2, matriz);
        validarJogada((char)((int)pos.x+2), pos.y+1, matriz);
        validarJogada((char)((int)pos.x+2), pos.y-1, matriz);
        validarJogada((char)((int)pos.x+1), pos.y-2, matriz);
        validarJogada((char)((int)pos.x-1), pos.y-2, matriz);
        validarJogada((char)((int)pos.x-2), pos.y-1, matriz);
        validarJogada((char)((int)pos.x-2), pos.y+1, matriz);
        validarJogada((char)((int)pos.x-1), pos.y+2, matriz);
        return matriz;
    }

    private void validarJogada(char c, int v, bool[,] m) {
        Posicao destino = new Posicao(c, v);
        if (tab.posicaoExiste(destino) && casaValida(destino)) {
            m[destino.x-97, destino.y-1] = true;
        }
    }
}
