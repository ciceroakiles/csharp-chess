class Peao : Peca {
    private Partida game;

    public Peao(Tabuleiro tab, bool cor, Partida game) : base(tab, cor) {
        this.game = game;
    }

    public override string ToString() {
        return Tela.NOMES[0];
    }

    private bool casaOponente(Posicao pos) {
        Peca p = tab.getPeca(pos);
        return (p != null && p.cor != cor);
    }

    public override bool[,] movPossiveis() {
        bool[,] matriz = new bool[tab.pecas.GetLength(0), tab.pecas.GetLength(1)];
        if (cor) {
            if (tab.getPeca(new Posicao(pos.x, pos.y+1)) == null) {
                if (qtdeMov == 0) validarJogada(pos.x, pos.y+2, matriz);
                validarJogada(pos.x, pos.y+1, matriz);
            }
            validarCaptura((char)((int)pos.x-1), pos.y+1, matriz);                
            validarCaptura((char)((int)pos.x+1), pos.y+1, matriz);
            // Jogada especial: En passant
            if (pos.y == 5) {
                Posicao esq = new Posicao((char)((int)pos.x-1), pos.y);
                if (casaOponente(esq) && tab.getPeca(esq) == game.enPassant) {
                    matriz[esq.x-97, (esq.y-1)+1] = true;
                }
                Posicao dir = new Posicao((char)((int)pos.x+1), pos.y);
                if (casaOponente(dir) && tab.getPeca(dir) == game.enPassant) {
                    matriz[dir.x-97, (dir.y-1)+1] = true;
                }
            }
        } else {
            if (tab.getPeca(new Posicao(pos.x, pos.y-1)) == null) {
                if (qtdeMov == 0) validarJogada(pos.x, pos.y-2, matriz);
                validarJogada(pos.x, pos.y-1, matriz);
            }
            validarCaptura((char)((int)pos.x-1), pos.y-1, matriz);                
            validarCaptura((char)((int)pos.x+1), pos.y-1, matriz);
            // Jogada especial: En passant
            if (pos.y == 4) {
                Posicao esq = new Posicao((char)((int)pos.x-1), pos.y);
                if (casaOponente(esq) && tab.getPeca(esq) == game.enPassant) {
                    matriz[esq.x-97, (esq.y-1)-1] = true;
                }
                Posicao dir = new Posicao((char)((int)pos.x+1), pos.y);
                if (casaOponente(dir) && tab.getPeca(dir) == game.enPassant) {
                    matriz[dir.x-97, (dir.y-1)-1] = true;
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

    private void validarCaptura(char c, int v, bool[,] m) {
        Posicao destino = new Posicao(c, v);
        if (tab.posicaoExiste(destino) && casaOponente(destino)) {
            m[destino.x-97, destino.y-1] = true;
        }
    }
}
