using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day18
{
    class Cube
    {
        public int x;
        public int y;
        public int z;

        public Cube(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    internal class SurfaceExplorer
    {
        List<Cube> cubeList = new List<Cube>();

        public void ParseInput(List<string> lines)
        { 
            foreach(var line in lines) 
            { 
                var coords = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                var cube = new Cube(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]));
                cubeList.Add(cube);
            }
        }

        public int Solve(int part = 1)
           => (part == 1) ? Explore() :0;

        int Explore()
        {
            if (cubeList.Count <= 0)
                return -1;

            int min_x = cubeList.Select(c => c.x).Min();
            int max_x = cubeList.Select(c => c.x).Max();
            int min_y = cubeList.Select(c => c.y).Min();
            int max_y = cubeList.Select(c => c.y).Max();
            int min_z = cubeList.Select(c => c.z).Min();
            int max_z = cubeList.Select(c => c.z).Max();

            int sideCounter = 0;

            // Count sides by planes, in both directions
            for (int ix = min_x; ix <= max_x; ix++)
                for (int iy = min_y; iy <= max_y; iy++)
                {
                    var cubeSlice = cubeList.Where(c => c.x == ix && c.y == iy).OrderBy(cc => cc.z).ToList();
                    if (cubeSlice.Count == 0)
                        continue;

                    if (cubeSlice.Count == 1)
                    {
                        sideCounter += 2;
                        continue;
                    }

                    sideCounter++;  // The first side has to be always added
                    for (int i = 0; i < cubeSlice.Count - 1; i++)
                        if(cubeSlice[i + 1].z - cubeSlice[i].z > 1)
                            sideCounter++;

                    sideCounter++; // The first side has to be always added
                    for (int i = cubeSlice.Count-1; i > 0; i--)
                        if (cubeSlice[i].z - cubeSlice[i-1].z > 1)
                            sideCounter++;
                }

            for (int ix = min_x; ix <= max_x; ix++)
                for (int iz = min_z; iz <= max_z; iz++)
                {
                    var cubeSlice = cubeList.Where(c => c.x == ix && c.z == iz).OrderBy(cc => cc.y).ToList();
                    if (cubeSlice.Count == 0)
                        continue;

                    if (cubeSlice.Count == 1)
                    {
                        sideCounter += 2;
                        continue;
                    }

                    sideCounter++;
                    for (int i = 0; i < cubeSlice.Count - 1; i++)
                        if (cubeSlice[i + 1].y - cubeSlice[i].y > 1)
                            sideCounter++;

                    sideCounter++;
                    for (int i = cubeSlice.Count - 1; i > 0; i--)
                        if (cubeSlice[i].y - cubeSlice[i-1].y > 1)
                            sideCounter++;
                }

            for (int iy = min_y; iy <= max_y; iy++)
                for (int iz = min_z; iz <= max_z; iz++)
                {
                    var cubeSlice = cubeList.Where(c => c.y == iy && c.z == iz).OrderBy(cc => cc.x).ToList();
                    if (cubeSlice.Count == 0)
                        continue;

                    if (cubeSlice.Count == 1)
                    {
                        sideCounter += 2;
                        continue;
                    }
                    
                    sideCounter++;
                    for (int i = 0; i < cubeSlice.Count - 1; i++)
                        if (cubeSlice[i + 1].x - cubeSlice[i].x > 1)
                            sideCounter++;

                    sideCounter++;
                    for (int i = cubeSlice.Count - 1; i > 0; i--)
                        if (cubeSlice[i].x - cubeSlice[i-1].x > 1)
                            sideCounter++;
                }
            
            return sideCounter;
        }
        

        int SolvePart2()
            => 0;

    }
}
