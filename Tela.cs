using System;
using System.Collections.Generic;

class Tela {
    // Cores
    public static readonly ConsoleColor FRG_COR_PRETAS = ConsoleColor.DarkBlue;
    public static readonly ConsoleColor FRG_COR_COORD = ConsoleColor.Yellow;
    public static readonly ConsoleColor BCK_COR_MOV = ConsoleColor.DarkGray;

    // Notação das peças
    public static readonly String[] NOMES = { "P", "C", "B", "T", "Q", "K" };

    public static void novaPartida() {
        Partida ptd = new Partida();
        Posicao p1 = null, p2 = null;
        bool[,] moves = null;
        string op = "";
        do {
            try {
                // Impressão e leitura 1-2
                Console.Clear();
                imprimirPartida(ptd, moves);
                if (!ptd.end) {
                    op = Console.ReadLine();
                    if (op.Length != 2) continue;
                    if (moves == null) {
                        // Validação 1
                        p1 = new Posicao(char.Parse(op.Substring(0, 1)), int.Parse(op.Substring(1, 1)));
                        ptd.validaOrigem(p1);
                        moves = ptd.board.getPeca(p1).movPossiveis();
                    } else {
                        // Validação 2
                        p2 = new Posicao(char.Parse(op.Substring(0, 1)), int.Parse(op.Substring(1, 1)));
                        ptd.validaDestino(p1, p2);
                        ptd.jogada(p1, p2);
                        if (ptd.promo) imprimePromoDialog(ptd, p2);
                        moves = null;
                    }
                } else {
                    break; // fim da partida
                }
            } catch (TabuleiroException e) {
                Console.WriteLine("\n" + e.Message);
                Console.ReadLine();
                moves = null;
            } catch (Exception e) {
                //Console.WriteLine("\n" + e.GetBaseException());
                moves = null;
            }
        } while (!op.Equals("end"));
        Console.ResetColor(); // medida de segurança
    }

    static void imprimirPartida(Partida p, bool[,] m) {
        imprimirTabuleiro(p.board.pecas, m, !p.jogador);
        if (p.end) {
            Console.WriteLine("(XEQUE-MATE)\n");
            Console.WriteLine("As " + (p.jogador ? "brancas" : "pretas") + " venceram.");
        } else {
            imprimirPecasCapturadas(p);
            Console.WriteLine("\nTurno " + Math.Ceiling((double)p.numJogadas/2) + "-" + (p.jogador ? "1" : "2"));
            Console.Write("Vez: ");
            if (p.jogador) {
                Console.WriteLine("Brancas\n");
            } else {
                Console.ForegroundColor = Tela.FRG_COR_PRETAS;
                Console.WriteLine("Pretas\n");
                Console.ResetColor();
            }
            if (p.xeque) {
                Console.WriteLine("(XEQUE)\n");
            }
            if (m == null) {
                Console.Write("Posição da peça: ");
            } else {
                Console.Write("Mover para: ");
            }
        }
    }

    static void imprimirTabuleiro(Peca[,] pecas, bool[,] matrizMov, bool flip) {
        ConsoleColor back = Console.BackgroundColor, alt = Tela.BCK_COR_MOV;
        if (flip) {
            // Orientação do tabuleiro: pretas em cima
            for (int j = 0; j < pecas.GetLength(1); j++) {
                Console.ForegroundColor = Tela.FRG_COR_COORD;
                if (matrizMov != null) Console.BackgroundColor = back;
                Console.Write((j+1) + " "); // linhas
                Console.ResetColor();
                for (int i = pecas.GetLength(0) - 1; i >= 0; i--) {
                    imprimirCasa(alt, back, matrizMov, pecas, i, j);
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = Tela.FRG_COR_COORD;
            if (matrizMov != null) Console.BackgroundColor = back;
            Console.WriteLine("  h g f e d c b a"); // colunas
        } else {
            // Orientação do tabuleiro: brancas em cima
            for (int j = pecas.GetLength(1) - 1; j >= 0; j--) {
                Console.ForegroundColor = Tela.FRG_COR_COORD;
                if (matrizMov != null) Console.BackgroundColor = back;
                Console.Write((j+1) + " "); // linhas
                Console.ResetColor();
                for (int i = 0; i < pecas.GetLength(0); i++) {
                    imprimirCasa(alt, back, matrizMov, pecas, i, j);
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = Tela.FRG_COR_COORD;
            if (matrizMov != null) Console.BackgroundColor = back;
            Console.WriteLine("  a b c d e f g h"); // colunas
        }
        Console.ResetColor();
    }

    static void imprimirCasa(ConsoleColor c1, ConsoleColor c2, bool[,] m, Peca[,] pcs, int i, int j) {
        if (m != null) {
            if (m[i, j]) {
                Console.BackgroundColor = c1;
            } else {
                Console.BackgroundColor = c2;
            }
        }
        if (pcs[i, j] == null) { // casa vazia
            Console.Write("-");
        } else {
            if (!pcs[i, j].cor) Console.ForegroundColor = Tela.FRG_COR_PRETAS;
            Console.Write(pcs[i, j].ToString());
            if (!pcs[i, j].cor) Console.ResetColor();
        }
        if (m != null) Console.BackgroundColor = c2;
        Console.Write(" ");
    }

    static void imprimirPecasCapturadas(Partida p) {
        Console.WriteLine("\nPeças capturadas");
        Console.Write("Brancas: ");
        imprimirConjunto(p.pecasCapturadas(true));
        Console.Write("Pretas: ");
        imprimirConjunto(p.pecasCapturadas(false));
    }

    static void imprimirConjunto(HashSet<Peca> conj) {
        Console.Write("[ ");
        foreach (Peca pc in conj) {
            if (!pc.cor) Console.ForegroundColor = Tela.FRG_COR_PRETAS;
            Console.Write(pc + " ");
            if (!pc.cor) Console.ResetColor();
        }
        Console.Write("]\n");
    }

    static void imprimePromoDialog(Partida p, Posicao pos) {
        List<Peca> repo = p.pecasRep(!p.jogador);
        int op = -1, i = 1;
        Console.WriteLine();
        foreach (Peca pc in repo) {
            Console.WriteLine("[ " + (i++) + " = " + pc.GetType().Name + " (" + pc + ") ]");
        }
        int c = Console.CursorTop;
        do {
            Console.SetCursorPosition(0, c);
            ClearCurrentConsoleLine();
            try {
                op = int.Parse(Console.ReadLine());
            } catch (Exception e) {
                op = -1;
            }
        } while (op <= 0 || op > repo.Count);
        p.promover(repo[op-1], pos);
    }

    static void ClearCurrentConsoleLine() {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth)); 
        Console.SetCursorPosition(0, currentLineCursor);
    }
}
