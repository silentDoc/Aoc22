using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    internal class VisibleTreeGrid
    {
        int[,] treeGrid;
        int[][] visibilityGrid;
        int width = -1;
        int height = -1;


        public VisibleTreeGrid(List<string> input)
        {
            width = input[0].Length;
            height = input.Count;

            treeGrid = new int[width, height];

            visibilityGrid = new int[width][];
            for (var i = 0; i < width; i++)
                visibilityGrid[i] = new int[height];

            for (var i = 0; i < height; i++)
                for (var j = 0; j < width; j++)
                {
                    treeGrid[j, i] = int.Parse(input[i][j].ToString());
                    visibilityGrid[j][i] = -1;
                }

            CalcVisibility();
        }

        void CalcVisibility()
        {
            // Edges
            for (var i = 0; i < height; i++)
            {
                visibilityGrid[0][i] = 1;
                visibilityGrid[width-1][i] = 1;
            }
            for (var j = 0; j < width; j++)
            {
                visibilityGrid[j][0] = 1;
                visibilityGrid[j][height-1] = 1;
            }

            // Inside
            for (var i = 1; i < height-1; i++)
                for (var j = 1; j < width-1; j++)
                    visibilityGrid[j][i] = (treeGrid[i, j] > minOcclusion(i,j)) ? 1 : 0;

           /* for (var i = 0; i < height; i++)
            {
                string str = "";
                for (var j = 0; j < width; j++)
                    str += visibilityGrid[j][i].ToString();

                Trace.WriteLine(str);
            }*/


        }

        int minOcclusion(int i, int j)
        {
            // treeGrid = new int[width, height];

            int minHeight = 999;
            int tempMax = -1;

          
            for(int k = 0; k < i; k++)
                tempMax = Math.Max(tempMax, treeGrid[k, j]);
            
            minHeight = Math.Min(minHeight, tempMax);
            tempMax = -1;

            for (int k = i+1; k < width; k++)
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
        

            //Trace.WriteLine("Position : " + i.ToString() + "," + j.ToString() + " GridVal : " + treeGrid[i, j].ToString() + ", max Oclusion: " + minHeight.ToString() + "Visible : " + (treeGrid[i, j]>minHeight).ToString());

            return minHeight;
        }
       

        public int HowManyVisibleTrees()
        {
            int sum = 0;
            for (int i = 0; i < height; i++)
                sum += visibilityGrid[i].Sum();

            return sum;
        }

    }
}
