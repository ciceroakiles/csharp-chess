using System;

class Tabuleiro {
    public Peca[,] pecas;

    public Tabuleiro(int n) {
        pecas = new Peca[n, n];
    }

    public Peca getPeca(Posicao pos) {
        try {
            return pecas[pos.x-97, pos.y-1];
        } catch (Exception e) {
            return null;
        }
    }

    public void colocarPeca(Peca p, Posicao pos) {
        if (existePeca(pos)) {
            throw new TabuleiroException("Já existe uma peça nessa posição.");
        }
        pecas[pos.x-97, pos.y-1] = p;
        p.pos = pos;
    }

    public Peca retirarPeca(Posicao pos) {
        if (pecas[pos.x-97, pos.y-1] != null) {
            Peca p = pecas[pos.x-97, pos.y-1];
            pecas[pos.x-97, pos.y-1] = null;
            return p;
        } else {
            return null;
        }
    }

    public bool existePeca(Posicao pos) {
        if (!posicaoExiste(pos)) {
            throw new TabuleiroException("Posição inválida.");
        }
        return getPeca(pos) != null;
    }

    public bool posicaoExiste(Posicao pos) {
        if (pos.x-97 < 0 || pos.x-97 >= pecas.GetLength(0) || pos.y-1 < 0 || pos.y-1 >= pecas.GetLength(1)) {
            return false;
        }
        return true;
    }
}
