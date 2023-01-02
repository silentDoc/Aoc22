using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public override string ToString()
            => x.ToString() + "-" + y.ToString() + "-" + z.ToString();
    }

    class CubeComparer : IEqualityComparer<Cube>
    {
        public bool Equals(Cube? a, Cube? b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null)
                return false;

            return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
        }

        public int GetHashCode([DisallowNull] Cube obj)
            => obj.ToString().GetHashCode();
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
           => (part==1) ? Explore(cubeList) : SolvePart2();

        int Explore(List<Cube> cList)
        {
            if (cList.Count <= 0)
                return -1;

            int min_x = cList.Min(c => c.x);
            int max_x = cList.Max(c => c.x);
            int min_y = cList.Min(c => c.y);
            int max_y = cList.Max(c => c.y);
            int min_z = cList.Min(c => c.z);
            int max_z = cList.Max(c => c.z);

            int sideCounter = 0;

            // Count sides by planes, in both directions
            for (int ix = min_x; ix <= max_x; ix++)
                for (int iy = min_y; iy <= max_y; iy++)
                {
                    var cubeSlice = cList.Where(c => c.x == ix && c.y == iy).OrderBy(cc => cc.z).ToList();
                    if (cubeSlice.Count == 0)
                        continue;

                    if (cubeSlice.Count == 1)
                    {
                        sideCounter += 2;
                        continue;
                    }

                    sideCounter++;  // The first side has to be always added
                    for (int i = 0; i < cubeSlice.Count - 1; i++)
                        if (cubeSlice[i + 1].z - cubeSlice[i].z > 1)
                            sideCounter++;

                    sideCounter++; // The first side has to be always added
                    for (int i = cubeSlice.Count - 1; i > 0; i--)
                        if (cubeSlice[i].z - cubeSlice[i - 1].z > 1)
                            sideCounter++;
                }

            for (int ix = min_x; ix <= max_x; ix++)
                for (int iz = min_z; iz <= max_z; iz++)
                {
                    var cubeSlice = cList.Where(c => c.x == ix && c.z == iz).OrderBy(cc => cc.y).ToList();
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
                    var cubeSlice = cList.Where(c => c.y == iy && c.z == iz).OrderBy(cc => cc.x).ToList();
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


        List<Cube> FindOpenAir()
        {
            int min_x = cubeList.Min(c => c.x) - 1;
            int max_x = cubeList.Max(c => c.x) + 1;
            int min_y = cubeList.Min(c => c.y) - 1;
            int max_y = cubeList.Max(c => c.y) + 1;
            int min_z = cubeList.Min(c => c.z) - 1;
            int max_z = cubeList.Max(c => c.z) + 1;

            Queue<Cube> activeCubes = new Queue<Cube>();
            List<Cube> openAir = new List<Cube>();
            CubeComparer comp = new CubeComparer();

            List<(int dx, int dy, int dz)> offsets = new List<(int dx, int dy, int dz)>();
            offsets.Add((0, 0, 1));
            offsets.Add((0, 0, -1));
            offsets.Add((0, 1, 0));
            offsets.Add((0, -1, 0));
            offsets.Add((1, 0, 0));
            offsets.Add((-1, 0, 0));

            var start = new Cube(min_x, min_y, min_z);
            activeCubes.Enqueue(start);

            while (activeCubes.Count > 0)   // BFS search starting outside the droplet, not allowing to walk through it
            {
                var consider = activeCubes.Dequeue();
                var x = consider.x;
                var y = consider.y;
                var z = consider.z;

                foreach (var offset in offsets)
                {
                    var nx = x + offset.dx;
                    if(nx<min_x || nx >max_x) 
                        continue;
                    var ny = y + offset.dy;
                    if (ny < min_y || ny > max_y)
                        continue;
                    var nz = z + offset.dz;
                    if (nz < min_z || nz > max_z)
                        continue;

                    var newCube = new Cube(nx, ny, nz);
                    
                    if (openAir.Contains(newCube, comp))
                        continue;
                    if (cubeList.Contains(newCube, comp))
                        continue;
                    if (activeCubes.Contains(newCube, comp))
                        continue;

                    activeCubes.Enqueue(newCube);
                    openAir.Add(newCube);
                }
            }

            return openAir;
        }

        List<Cube> FindTrappedAir(List<Cube> openAir)
        {

            var air = new List<Cube>();
            CubeComparer comp = new CubeComparer();

            int min_x = cubeList.Min(c => c.x);
            int max_x = cubeList.Max(c => c.x);
            int min_y = cubeList.Min(c => c.y);
            int max_y = cubeList.Max(c => c.y);

            for (int ix = min_x; ix <= max_x; ix++)
                for (int iy = min_y; iy <= max_y; iy++)
                {
                    var cubeSlice = cubeList.Where(c => c.x == ix && c.y == iy).OrderBy(cc => cc.z).ToList();
                    if (cubeSlice.Count <= 1)
                        continue;

                    var min_z = cubeSlice.Min(s => s.z);
                    var max_z = cubeSlice.Max(s => s.z);
                    var innerZ = cubeSlice.Select(s => s.z).ToList();

                    for (int z = min_z + 1; z < max_z; z++)
                        if (!innerZ.Contains(z)) // We can have some fragment inside
                        {
                            var nc = new Cube(ix, iy, z);
                            if(!openAir.Contains(nc, comp)) // We want to consider empty space that is not outside the droplet
                                air.Add(new Cube(ix, iy, z));
                        }
                }
            return air;
        }

        int SolvePart2()
        {
            var openAir = FindOpenAir();
            var airCubes = FindTrappedAir(openAir);
            var allSides = Explore(cubeList);   // Part 1 result
            var airSides = Explore(airCubes);
            return allSides - airSides;
        }
    }
}
