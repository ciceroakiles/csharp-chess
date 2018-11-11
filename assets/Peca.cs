abstract class Peca {
    public bool cor; // true: brancas, false: pretas
    public int qtdeMov;
    public Posicao pos;
    public Tabuleiro tab;

    public Peca(Tabuleiro tab, bool cor) {
        this.tab = tab;
        this.cor = cor;
        this.pos = null;
        this.qtdeMov = 0;
    }

    public bool casaValida(Posicao pos) {
        Peca p = tab.getPeca(pos);
        return (p == null || p.cor != cor);
    }

    public bool pecaLivre() {
        bool[,] m = movPossiveis();
        for (int j = 0; j < m.GetLength(1); j++) {
            for (int i = 0; i < m.GetLength(0); i++) {
                if (m[i, j]) return true;
            }
        }
        return false;
    }

    public bool podeMover(Posicao pos) {
        return movPossiveis()[pos.x-97, pos.y-1];
    }

    public abstract bool[,] movPossiveis();
}
