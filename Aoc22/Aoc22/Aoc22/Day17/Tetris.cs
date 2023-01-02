using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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

    class TetrisState
    {
        public string snapshot;
        public int pieceid;
        public int jetid;
        public int piececount;
        public long height;

        public TetrisState(string snapshot, int pieceid, int jetid, int piececount, long height)
        {
            this.snapshot = snapshot;
            this.pieceid = pieceid;
            this.jetid = jetid;
            this.piececount = piececount;
            this.height = height;
        }   
    }

    internal class Tetris
    {
        readonly char[,] piece1 = new char[1, 4] { { '#', '#', '#', '#' } };
        readonly char[,] piece2 = new char[3, 3] { { '.', '#', '.' }, 
                                                   { '#', '#', '#' }, 
                                                   { '.', '#', '.' } };
        readonly char[,] piece3 = new char[3, 3] { { '.', '.', '#' }, 
                                                   { '.', '.', '#' }, 
                                                   { '#', '#', '#' } };
        readonly char[,] piece4 = new char[4, 1] { { '#' }, 
                                                   { '#' }, 
                                                   { '#' }, 
                                                   { '#' } };
        readonly char[,] piece5 = new char[2, 2] { { '#' , '#' }, 
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

            //Console.ReadLine();

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


        string Snapshot(int maxHeight, int numRows)
        {
            StringBuilder sb = new();
            for (int i = maxHeight; i > maxHeight - numRows; i--)
                sb.Append(game[i]);
            return sb.ToString();
        }

        long Play(string sequence, long numPieces, int part = 1)
        {
            long piecesCount = 0;
            int whichPiece = 0;
            int currentHeight = -1;
            int currentLeft = 2;
            int currentMove = 0;

            var stateList = new List<TetrisState>();   

            List<TetrisPiece> pieces = new();
            game.Add(new char[] { '#', '#', '#', '#', '#', '#', '#' });
            pieces.Add(new TetrisPiece(piece1));
            pieces.Add(new TetrisPiece(piece2));
            pieces.Add(new TetrisPiece(piece3));
            pieces.Add(new TetrisPiece(piece4));
            pieces.Add(new TetrisPiece(piece5));

            bool jet = false;

            while (piecesCount < numPieces)
            {
                var piece = pieces[whichPiece];

                if (currentHeight == -1)
                {
                    var currentTop = GetCurrentHeight();

                    currentHeight = currentTop + 3 + piece.h;

                    var dif = currentHeight - game.Count;
                    if (dif >= 0)
                        for (int i = 0; i < dif + 1; i++)
                            game.Add(new char[] { '.', '.', '.', '.', '.', '.', '.' });
                   

                    if (part == 2 && currentTop > 8)
                    {
                        int smallPieceCounter = (int)piecesCount;
                        var currentSnapshot = Snapshot(currentTop, 8);

                        var existingStatus = stateList.Where(x => x.pieceid == whichPiece && x.jetid == currentMove && x.snapshot == currentSnapshot).FirstOrDefault();

                        if (existingStatus != null)
                        {
                            // Calculations
                            var numPiecesBefore = existingStatus.piececount;    // Before pattern
                            var heightBefore = existingStatus.height;

                            long numPiecesLoop = smallPieceCounter - numPiecesBefore;   // Pattern Loop
                            long numLoops = (numPieces - numPiecesBefore) / numPiecesLoop;
                            var heightLoop = currentTop - heightBefore;

                            var numPiecesAfter = (int)(numPieces - numPiecesBefore - (numLoops * numPiecesLoop));   // After the last pattern
                            var statusAfter = stateList.Where(x => x.piececount == existingStatus.piececount + numPiecesAfter).FirstOrDefault();
                            long heightAfter = statusAfter.height - existingStatus.height;

                            long retValue = heightBefore + numLoops * heightLoop + heightAfter;

                            return retValue;
                        }
                        else
                            stateList.Add(new TetrisState(currentSnapshot, whichPiece, currentMove, smallPieceCounter, currentTop));

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
                    
                    whichPiece++;
                    whichPiece %= 5;
                    currentLeft = 2;
                    currentHeight = -1;
                }
                else // Jet
                {
                    char dir = sequence[currentMove];
                    currentMove++;
                    currentMove %= sequence.Length;

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

        public long Solve(int part = 1)
            => (part == 1) ? SolvePart1() : SolvePart2();

        long SolvePart1()
            => Play(sequence, 2022);

        long SolvePart2()
            => Play(sequence, 1000000000000, 2);

    }
}
