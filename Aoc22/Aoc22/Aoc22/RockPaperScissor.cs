using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    internal class RockPaperScissor
    {
        string handOponent;
        string handPlayer;
        int oponent;
        int player;
        int result;
        public int Score;

        static int PlayTransform(string play) => play switch
        {
            "X" => 1,
            "Y" => 2,
            "Z" => 3,
            "A" => 1,
            "B" => 2,
            "C" => 3,
            _ => throw new ArgumentException("Invalid play string : " + play),
        };

        static int PlayResult(int oponent, int player) => (oponent, player) switch
        {
            (1, 1) => 3,
            (1, 2) => 6,
            (1, 3) => 0,
            (2, 1) => 0,
            (2, 2) => 3,
            (2, 3) => 6,
            (3, 1) => 6,
            (3, 2) => 0,
            (3, 3) => 3,
            _ => throw new ArgumentException("Invalid values entered : "+ oponent.ToString() + " - " + player.ToString()),
        };

        public RockPaperScissor(string hOponent, string hPlayer)
        {
            handOponent = hOponent;
            handPlayer = hPlayer; 
            
            oponent = PlayTransform(handOponent);
            player = PlayTransform(handPlayer);
            result = PlayResult(oponent, player);
            Score = player + result;
        }

        public override string ToString()
        {
            String ret = "Round : " + handOponent + " vs " + handPlayer + " ;; ";
            ret += "Result = " + result.ToString() + " ; Score = " + Score.ToString();
            return ret;
        }
        

    }
}
