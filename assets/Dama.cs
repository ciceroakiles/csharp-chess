class Dama : Peca {
    public Dama(Tabuleiro tab, bool cor) : base(tab, cor) {
    }

    public override string ToString() {
        return Tela.NOMES[4];
    }

    public override bool[,] movPossiveis() {
        bool[,] matriz = new bool[tab.pecas.GetLength(0), tab.pecas.GetLength(1)];
        validarJogada(pos.x, pos.y+1, matriz, 1);
        validarJogada((char)((int)pos.x+1), pos.y+1, matriz, 2);
        validarJogada((char)((int)pos.x+1), pos.y, matriz, 3);
        validarJogada((char)((int)pos.x+1), pos.y-1, matriz, 4);
        validarJogada(pos.x, pos.y-1, matriz, 5);
        validarJogada((char)((int)pos.x-1), pos.y-1, matriz, 6);
        validarJogada((char)((int)pos.x-1), pos.y, matriz, 7);
        validarJogada((char)((int)pos.x-1), pos.y+1, matriz, 8);
        return matriz;
    }

    private void validarJogada(char x, int v, bool[,] m, int loopD) {
        Posicao destino = new Posicao(x, v);
        while (tab.posicaoExiste(destino) && casaValida(destino)) {
            m[destino.x-97, destino.y-1] = true;
            if (tab.getPeca(destino) != null && tab.getPeca(destino).cor != cor) {
                break;
            }
            switch (loopD) {
                case 1: {
                    destino.y += 1;
                } break;
                case 2: {
                    destino.x = (char)((int)destino.x+1);
                    destino.y += 1;
                } break;
                case 3: {
                    destino.x = (char)((int)destino.x+1);
                } break;
                case 4: {
                    destino.x = (char)((int)destino.x+1);
                    destino.y -= 1;
                } break;
                case 5: {
                    destino.y -= 1;
                } break;
                case 6: {
                    destino.x = (char)((int)destino.x-1);
                    destino.y -= 1;
                } break;
                case 7: {
                    destino.x = (char)((int)destino.x-1);
                } break;
                case 8: {
                    destino.x = (char)((int)destino.x-1);
                    destino.y += 1;
                } break;
            }
        }
    }
}
