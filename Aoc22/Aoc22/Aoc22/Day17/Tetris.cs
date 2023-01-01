using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day17
{
    class TetrisPiece
    {
        public int w;
        public int h;
        public char[,] piece;

        public TetrisPiece(char[,] rock)
        {
            w = rock.GetLength(1);
            h = rock.GetLength(0);
            piece = rock;
        }
    }

    internal class Tetris
    {
        char[,] piece1 = new char[1, 4] { { '#', '#', '#', '#' } };
        char[,] piece2 = new char[3, 3] { { '.', '#', '.' }, 
                                          { '#', '#', '#' }, 
                                          { '.', '#', '.' } };
        char[,] piece3 = new char[3, 3] { { '.', '.', '#' }, 
                                          { '.', '.', '#' }, 
                                          { '#', '#', '#' } };
        char[,] piece4 = new char[4, 1] { { '#' }, 
                                          { '#' }, 
                                          { '#' }, 
                                          { '#' } };
        char[,] piece5 = new char[2, 2] { { '#' , '#' }, 
                                          { '#' , '#' } };
        
        List<char[]> game = new();

        string sequence = "";

        public void ParseInput(List<string> lines)
            => sequence = lines[0];

        int GetCurrentHeight()
        {
            for (int i = game.Count - 1; i >= 0; i--)
                if (game[i].Any(x => x=='#'))
                    return i;
            return 0;
        }

        void LogGame(bool toFile = false)
        {
            Console.Clear();
            for (int i = game.Count - 1; i >= 0; i--)
                Console.WriteLine(new string(game[i]));
            Console.SetWindowPosition(0, 0);

            Console.ReadLine();

            if (toFile)
            {
                Dictionary<char, int> equiv = new();
                equiv['.'] = 0;
                equiv[' '] = 0;
                equiv['#'] = 255;

                var dt = DateTime.Now;
                var str = dt.Hour.ToString("00") + "_" + dt.Minute.ToString("00") + "_" + dt.Second.ToString("00") + "_" + dt.Millisecond.ToString("00") + ".pgm";

                Common.PGMFileHelper.Dump(str, game, equiv, true);
            }
        }

        void LogGamePiece(TetrisPiece p, int height, int left, int pieceCount=0)
        {
            Console.Clear();
            for (int i = game.Count - 1; i >= 0; i--)
            {
                StringBuilder str = new(new string(game[i]));
                if ((i <= height) && (height - i < p.h))
                {
                    for (int j = 0; j < p.w; j++)
                        str[j + left] = (p.piece[height - i, j] == '#' ? '@' : str[j + left]);
                }
                Console.WriteLine(str.ToString());
            }
            Console.SetWindowPosition(0, 0);
            Console.ReadKey();
            Thread.Sleep(10);
        }

        bool Collides(TetrisPiece tetrisPiece, int height, int left)
        {

            for (int i = 0; i < tetrisPiece.w; i++)
                for (int j = 0; j < tetrisPiece.h; j++)
                {
                    if (tetrisPiece.piece[j, i] == '#')
                        if (game[height - j][left + i] == '#')
                            return true;
                }
            return false;
        }

        void RestPiece(TetrisPiece tetrisPiece, int height, int left)
        {
            for (int i = 0; i < tetrisPiece.w; i++)
                for (int j = 0; j < tetrisPiece.h; j++)
                    game[height - j][left + i] = ( tetrisPiece.piece[j, i] == '#' ? '#' : game[height - j][left + i]);
        }


        int Play(string sequence, int numPieces)
        {
            int piecesCount = 0;
            int currentHeight = -1;
            int currentLeft = 2;
            List<TetrisPiece> pieces = new();
            int currentMove = 0;

            game.Add(new char[] { '#', '#', '#', '#', '#', '#', '#' });

            pieces.Add(new TetrisPiece(piece1));
            pieces.Add(new TetrisPiece(piece2));
            pieces.Add(new TetrisPiece(piece3));
            pieces.Add(new TetrisPiece(piece4));
            pieces.Add(new TetrisPiece(piece5));

            bool jet = false;

            while (piecesCount < numPieces)
            {
                var piece = pieces[piecesCount % 5];

                if (currentHeight == -1)
                {
                    currentHeight = GetCurrentHeight() + 3 + piece.h;

                    var dif = currentHeight - game.Count;
                    if (dif >= 0)
                    {
                        for (int i = 0; i < dif + 1; i++)
                            game.Add(new char[] { '.', '.', '.', '.', '.', '.', '.' });
                    }
                }

                jet = !jet;  // Alternate every step
                
                if (!jet) // Drop
                {
                    if (!Collides(piece, currentHeight - 1, currentLeft))
                    {
                        currentHeight--;
                        continue;
                    }

                    RestPiece(piece, currentHeight, currentLeft);
                    piecesCount++;
                    currentLeft = 2;
                    currentHeight = -1;
                }
                else // Jet
                {
                    char dir = sequence[currentMove % sequence.Length];
                    currentMove++;
                    int inc = dir == '>' ? 1 : -1;

                    if (currentLeft + inc + piece.w > 7)
                        continue;
                    if (currentLeft + inc < 0)
                        continue;

                    if (!Collides(piece, currentHeight, currentLeft + inc))
                        currentLeft += inc;
                }
            }

            return GetCurrentHeight();
        }

        public int Solve(int part = 1)
        {
            return (part == 1) ? SolvePart1() : SolvePart2();
        }

        int SolvePart1()
        {
            return Play(sequence, 2022);
        }

        int SolvePart2()
        {
            //1000000000000
            return 0;
        }

    }
}
