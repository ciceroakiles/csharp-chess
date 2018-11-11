class Rei : Peca {
    private Partida game;

    public Rei(Tabuleiro tab, bool cor, Partida game) : base(tab, cor) {
        this.game = game;
    }

    public override string ToString() {
        return Tela.NOMES[5];
    }

    private bool ProjetoTorre(Posicao pos) {
        Peca p = tab.getPeca(pos);
        return p != null && p is Torre && p.cor == cor && p.qtdeMov == 0;
    }

    public override bool[,] movPossiveis() {
        bool[,] matriz = new bool[tab.pecas.GetLength(0), tab.pecas.GetLength(1)];
        validarJogada(pos.x, pos.y+1, matriz);
        validarJogada((char)((int)pos.x+1), pos.y+1, matriz);
        validarJogada((char)((int)pos.x+1), pos.y, matriz);
        validarJogada((char)((int)pos.x+1), pos.y-1, matriz);
        validarJogada(pos.x, pos.y-1, matriz);
        validarJogada((char)((int)pos.x-1), pos.y-1, matriz);
        validarJogada((char)((int)pos.x-1), pos.y, matriz);
        validarJogada((char)((int)pos.x-1), pos.y+1, matriz);
        // Jogada especial: Roque
        if (qtdeMov == 0 && !game.xeque) {
            // Pequeno
            Posicao posT1 = new Posicao((char)((int)pos.x+3), pos.y);
            if (ProjetoTorre(posT1)) {
                Posicao p1 = new Posicao((char)((int)pos.x+1), pos.y);
                Posicao p2 = new Posicao((char)((int)pos.x+2), pos.y);
                if (tab.getPeca(p1) == null && tab.getPeca(p2) == null) {
                    validarJogada(p2.x, p2.y, matriz);
                }
            }
            // Grande
            Posicao posT2 = new Posicao((char)((int)pos.x-4), pos.y);
            if (ProjetoTorre(posT2)) {
                Posicao p1 = new Posicao((char)((int)pos.x-1), pos.y);
                Posicao p2 = new Posicao((char)((int)pos.x-2), pos.y);
                Posicao p3 = new Posicao((char)((int)pos.x-3), pos.y);
                if (tab.getPeca(p1) == null && tab.getPeca(p2) == null && tab.getPeca(p3) == null) {
                    validarJogada(p2.x, p2.y, matriz);
                }
            }
        }
        return matriz;
    }

    private void validarJogada(char c, int v, bool[,] m) {
        Posicao destino = new Posicao(c, v);
        if (tab.posicaoExiste(destino) && casaValida(destino)) {
            m[destino.x-97, destino.y-1] = true;
        }
    }
}
