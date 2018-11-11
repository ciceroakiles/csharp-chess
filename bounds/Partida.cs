using System.Collections.Generic;

class Partida {
    public bool jogador { get; private set; } // true: brancas, false: pretas
    public bool xeque { get; private set; }
    public bool promo;
    public bool end { get; private set; }
    public int numJogadas { get; private set; }
    public Tabuleiro board;
    public Peca enPassant { get; private set; }
    private HashSet<Peca> p_emjogo, p_capturadas;

    public Partida() {
        board = new Tabuleiro(8);
        p_emjogo = new HashSet<Peca>();
        p_capturadas = new HashSet<Peca>();
        enPassant = null;
        numJogadas = 1;
        jogador = true;
        promo = false;
        xeque = false;
        end = false;
        setup(board);
    }

    private void novaPeca(Peca p, Posicao pos) {
        board.colocarPeca(p, pos);
        p_emjogo.Add(p);
    }

    private void setup(Tabuleiro t) {
        for (int i = 1; i < 9; i+=7) {
            Peca[] ranks = {
                new Torre(t, (i == 1)), new Cavalo(t, (i == 1)), new Bispo(t, (i == 1)), new Rainha(t, (i == 1)),
                new Rei(t, (i == 1), this), new Bispo(t, (i == 1)), new Cavalo(t, (i == 1)), new Torre(t, (i == 1))
            };
            for (char c = 'a'; c <= 'h'; c++) { // 'a' == 97, 'h' == 104
                novaPeca(ranks[(int)c-97], new Posicao(c, i));
                novaPeca(new Peao(t, (i == 1), this), new Posicao(c, (i == 1 ? i+1 : i-1)));
            }
        }
    }

    public void validaOrigem(Posicao pos) {
        if (board.getPeca(pos) == null) {
            throw new TabuleiroException("SYS > Cadê a peça?");
        }
        if (board.getPeca(pos).cor != jogador) {
            throw new TabuleiroException("SYS > Essa peça é do outro jogador.");
        }
        if (!board.getPeca(pos).pecaLivre()) {
            throw new TabuleiroException("SYS > Não há como mover essa peça.");
        }
    }

    public void validaDestino(Posicao p1, Posicao p2) {
        if (!board.getPeca(p1).podeMover(p2)) {
            throw new TabuleiroException("SYS > Não pode mover para essa casa.");
        }
    }

    public void jogada(Posicao p1, Posicao p2) {
        Peca pc = movimento(p1, p2);
        if (emXeque(jogador)) {
            desfazMov(p1, p2, pc);
            throw new TabuleiroException("SYS > Não pode deixar o rei em xeque!");
        }
        Peca pecaMovida = board.getPeca(p2); // necessário em (en_p)
        // Jogada especial: Promoção
        if (pecaMovida is Peao) {
            if ((pecaMovida.cor && p2.y == 8) || (!pecaMovida.cor && p2.y == 1)) {
                pecaMovida = board.retirarPeca(p2);
                p_emjogo.Remove(pecaMovida);
                promo = true;
            }
        }
        xeque = emXeque(!jogador);
        if (ProjetoXequemate(!jogador)) {
            end = true;
        } else {
            numJogadas++;
            jogador = (numJogadas % 2 != 0);
        }
        // Jogada especial: En passant (en_p)
        if (pecaMovida is Peao && (p2.y == p1.y - 2 || p2.y == p1.y + 2)) {
            enPassant = pecaMovida;
        } else {
            enPassant = null;
        }
    }

    private Peca movimento(Posicao p1, Posicao p2) {
        Peca pecaMovida = board.retirarPeca(p1);
        pecaMovida.qtdeMov++;
        Peca pecaCapt = board.retirarPeca(p2);
        if (pecaCapt != null) {
            p_capturadas.Add(pecaCapt);
        }
        board.colocarPeca(pecaMovida, p2);
        // Jogada especial: Roque pequeno
        if (pecaMovida is Rei && (int)p2.x == (int)p1.x + 2) {
            Posicao posT1 = new Posicao((char)((int)p1.x+3), p1.y);
            Posicao posT2 = new Posicao((char)((int)p1.x+1), p1.y);
            Peca tRoque = board.retirarPeca(posT1);
            tRoque.qtdeMov++;
            board.colocarPeca(tRoque, posT2);
        }
        // Jogada especial: Roque grande
        if (pecaMovida is Rei && (int)p2.x == (int)p1.x - 2) {
            Posicao posT1 = new Posicao((char)((int)p1.x-4), p1.y);
            Posicao posT2 = new Posicao((char)((int)p1.x-1), p1.y);
            Peca tRoque = board.retirarPeca(posT1);
            tRoque.qtdeMov++;
            board.colocarPeca(tRoque, posT2);
        }
        // Jogada especial: En passant
        if (pecaMovida is Peao) {
            if (p1.x != p2.x && pecaCapt == null) {
                Posicao posP;
                if (pecaMovida.cor) {
                    posP = new Posicao(p2.x, p2.y-1);
                } else {
                    posP = new Posicao(p2.x, p2.y+1);
                }
                pecaCapt = board.retirarPeca(posP);
                p_capturadas.Add(pecaCapt);
            }
        }
        return pecaCapt;
    }

    private void desfazMov(Posicao p1, Posicao p2, Peca pecaCapt) {
        Peca pecaMovida = board.retirarPeca(p2);
        pecaMovida.qtdeMov--;
        if (pecaCapt != null) {
            board.colocarPeca(pecaCapt, p2);
            p_capturadas.Remove(pecaCapt);
        }
        board.colocarPeca(pecaMovida, p1);
        // Jogada especial: Roque pequeno
        if (pecaMovida is Rei && (int)p2.x == (int)p1.x + 2) {
            Posicao posT1 = new Posicao((char)((int)p1.x+3), p1.y);
            Posicao posT2 = new Posicao((char)((int)p1.x+1), p1.y);
            Peca tRoque = board.retirarPeca(posT2);
            tRoque.qtdeMov--;
            board.colocarPeca(tRoque, posT1);
        }
        // Jogada especial: Roque grande
        if (pecaMovida is Rei && (int)p2.x == (int)p1.x - 2) {
            Posicao posT1 = new Posicao((char)((int)p1.x-4), p1.y);
            Posicao posT2 = new Posicao((char)((int)p1.x-1), p1.y);
            Peca tRoque = board.retirarPeca(posT2);
            tRoque.qtdeMov--;
            board.colocarPeca(tRoque, posT1);
        }
        // Jogada especial: En passant
        if (pecaMovida is Peao) {
            if (p1.x != p2.x && pecaCapt == enPassant) {
                Peca pc = board.retirarPeca(p2);
                Posicao posP;
                if (pecaMovida.cor) {
                    posP = new Posicao(p2.x, 5);
                } else {
                    posP = new Posicao(p2.x, 4);
                }
                board.colocarPeca(pc, posP);
            }
        }
    }

    public bool emXeque(bool jogador) {
        Peca r = null;
        foreach (Peca pc in pecasEmJogo(jogador)) {
            if (pc is Rei) {
                r = pc;
                break;
            }
        }
        foreach (Peca pc in pecasEmJogo(!jogador)) {
            if (pc.movPossiveis()[r.pos.x-97, r.pos.y-1]) {
                return true;
            }
        }
        return false;
    }

    public bool ProjetoXequemate(bool jogador) {
        if (!emXeque(jogador)) {
            return false;
        }
        foreach (Peca pc in pecasEmJogo(jogador)) {
            bool[,] m = pc.movPossiveis();
            for (int j = 0; j < m.GetLength(1); j++) {
                for (int i = 0; i < m.GetLength(0); i++) {
                    if (m[i, j]) {
                        Posicao p1 = pc.pos;
                        Posicao p2 = new Posicao((char)(i+97), j+1);
                        Peca capt = movimento(p1, p2);
                        bool xequeTest = emXeque(jogador);
                        desfazMov(p1, p2, capt);
                        if (!xequeTest) {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public void promover(Peca pc, Posicao pos) {
        foreach (Peca pc2 in p_capturadas) {
            if (pc2.GetType().Name == pc.GetType().Name) {
                novaPeca(pc2, pos);
                p_capturadas.Remove(pc2);
                break;
            }
        }
        promo = false;
    }

    public HashSet<Peca> pecasCapturadas(bool cor) {
        HashSet<Peca> conj = new HashSet<Peca>();
        foreach (Peca pc in p_capturadas) {
            if (pc.cor == cor) conj.Add(pc);
        }
        return conj;
    }

    public HashSet<Peca> pecasEmJogo(bool cor) {
        HashSet<Peca> conj = new HashSet<Peca>();
        foreach (Peca pc in p_emjogo) {
            if (pc.cor == cor) conj.Add(pc);
        }
        conj.ExceptWith(p_capturadas);
        return conj;
    }

    public List<Peca> pecasRep(bool cor) {
        bool[] temPeca = { false, false, false, false };
        List<Peca> conj = new List<Peca>();
        foreach (Peca pc in pecasCapturadas(cor)) {
            if (pc is Rainha) temPeca[0] = true;
            if (pc is Torre) temPeca[1] = true;
            if (pc is Bispo) temPeca[2] = true;
            if (pc is Cavalo) temPeca[3] = true;
        }
        if (temPeca[0]) conj.Add(new Rainha(board, cor));
        if (temPeca[1]) conj.Add(new Torre(board, cor));
        if (temPeca[2]) conj.Add(new Bispo(board, cor));
        if (temPeca[3]) conj.Add(new Cavalo(board, cor));
        return conj;
    }
}
