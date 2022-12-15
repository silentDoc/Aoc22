using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Aoc22.Day08
{
    // TODO - Refactor with c# 8 Range operators : https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges

    internal class VisibleTreeGrid
    {
        int[,] treeGrid;
        int[][] visibilityGrid;
        int[][] scenicScoreGrid;
        int width = -1;
        int height = -1;


        public VisibleTreeGrid(List<string> input)
        {
            width = input[0].Length;
            height = input.Count;

            treeGrid = new int[width, height];

            visibilityGrid = new int[width][];
            scenicScoreGrid = new int[width][];
            for (var i = 0; i < width; i++)
            {
                visibilityGrid[i] = new int[height];
                scenicScoreGrid[i] = new int[height];
            }

            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                {
                    treeGrid[j, i] = int.Parse(input[i][j].ToString());
                    visibilityGrid[j][i] = -1;
                    scenicScoreGrid[j][i] = -1;
                }

            CalcVisibility();
            CalcScore();
        }

        void CalcVisibility()
        {
            // Edges
            for (var i = 0; i < height; i++)
            {
                visibilityGrid[0][i] = 1;
                visibilityGrid[width - 1][i] = 1;
            }
            for (var j = 0; j < width; j++)
            {
                visibilityGrid[j][0] = 1;
                visibilityGrid[j][height - 1] = 1;
            }

            // Inside
            for (var i = 1; i < height - 1; i++)
                for (var j = 1; j < width - 1; j++)
                    visibilityGrid[j][i] = treeGrid[i, j] > minOcclusion(i, j) ? 1 : 0;
        }

        void CalcScore()
        {
            for (var i = 1; i < height - 1; i++)
                for (var j = 1; j < width - 1; j++)
                {
                    scenicScoreGrid[j][i] = treeScore(i, j);
                }
        }

        int minOcclusion(int i, int j)
        {
            int minHeight = 999;
            int tempMax = -1;


            for (int k = 0; k < i; k++)
                tempMax = Math.Max(tempMax, treeGrid[k, j]);

            minHeight = Math.Min(minHeight, tempMax);
            tempMax = -1;

            for (int k = i + 1; k < width; k++)
                tempMax = Math.Max(tempMax, treeGrid[k, j]);
            minHeight = Math.Min(minHeight, tempMax);
            tempMax = -1;

            for (int k = 0; k < j; k++)
                tempMax = Math.Max(tempMax, treeGrid[i, k]);
            minHeight = Math.Min(minHeight, tempMax);
            tempMax = -1;

            for (int k = j + 1; k < height; k++)
                tempMax = Math.Max(tempMax, treeGrid[i, k]);
            minHeight = Math.Min(minHeight, tempMax);

            return minHeight;
        }


        int treeScore(int i, int j)
        {
            //treeGrid = new int[width, height];
            int current = treeGrid[i, j];
            int[] scores = new int[4];

            scores[0] = scores[1] = scores[2] = scores[3] = 0;

            for (int k = i - 1; k >= 0; k--)
            {
                scores[0]++;
                if (treeGrid[k, j] >= current) break;
            }

            for (int k = i + 1; k < width; k++)
            {
                scores[1]++;
                if (treeGrid[k, j] >= current)
                    break;
            }

            for (int k = j - 1; k >= 0; k--)
            {
                scores[2]++;
                if (treeGrid[i, k] >= current) break;
            }

            for (int k = j + 1; k < height; k++)
            {
                scores[3]++;
                if (treeGrid[i, k] >= current) break;
            }

            var score = scores[0] * scores[1] * scores[2] * scores[3];

            return score;
        }

        public int HowManyVisibleTrees()
        {
            int sum = 0;
            for (int i = 0; i < height; i++)
                sum += visibilityGrid[i].Sum();

            return sum;
        }

        public int MaxScore()
        {
            List<int> maxims = new();

            for (int i = 0; i < height; i++)
                maxims.Add(scenicScoreGrid[i].Max());


            return maxims.Max();
        }

    }
}
