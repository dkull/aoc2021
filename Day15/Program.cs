using System.Diagnostics;
using System;

namespace App
{
    class Program
    {

        static void Main(string[] args)
        {
            var data = LoadData(args[0]);
            Stopwatch sw = Stopwatch.StartNew();
            var p1 = P1(data);
            Console.WriteLine($"P1: {p1} in {sw.ElapsedMilliseconds} ms");

            data = LoadData(args[0]);
            sw = Stopwatch.StartNew();
            var p2 = P2(data);
            Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");
        }

        static int[][] LoadData(string filepath)
        {
            var allLines = File.ReadAllLines(filepath);
            return allLines
                .Select(x => x.ToList().Select(x => int.Parse(x.ToString())).ToArray())
                .ToArray();
        }

        /*
            P1
        */

        public static void mapper1(
            int[][] map,
            (int x, int y) pos,
            (int x, int y)? cameFrom,
            Dictionary<(int x, int y), int> riskMap)
        {
            if (pos.y < 0 || map.Length <= pos.y) return;
            if (pos.x < 0 || map[pos.y].Length <= pos.x) return;

            var previousRisk = 0;
            if (cameFrom != null)
            {
                previousRisk = riskMap[((int x, int y))cameFrom];
            }

            var currentPathRisk = map[pos.y][pos.x] + previousRisk;
            if (riskMap.ContainsKey(pos))
            {
                if (currentPathRisk >= riskMap[pos]) return;
            }
            riskMap[pos] = currentPathRisk;

            var targetPos = (map[0].Length-1, map.Length-1);
            if (riskMap.ContainsKey(targetPos) && targetPos != pos)
            {
                //Console.WriteLine($"{riskMap.Count} {riskMap[targetPos]} {pos}");
                //var manhattan = (targetPos.Item1 - pos.x) + (targetPos.Item2 - pos.y);
                if (riskMap[targetPos] < currentPathRisk) return;
                //if (riskMap[targetPos] <= currentPathRisk) return;
            }

            var neighbors = new (int x, int y)[] {
                (pos.x, pos.y + 1),
                (pos.x + 1, pos.y),
                (pos.x, pos.y - 1),
                (pos.x - 1, pos.y),
            };

            foreach (var neighbor in neighbors)
            {
                if (neighbor == cameFrom) continue;
                if (riskMap.ContainsKey(neighbor) && riskMap[neighbor] <= currentPathRisk) continue;
                mapper1(map, neighbor, pos, riskMap);
            }
        }

        public static long P1(int[][] map)
        {
            // clear the first risk manually
            map[0][0] = 0;
            var riskMap = new Dictionary<(int x, int y), int>();
            mapper1(map, (x: 0, y: 0), null, riskMap);
            return riskMap[(map[0].Length-1, map.Length-1)];
        }

        /*
            P2
        */

        public static int mapper2(int[,] map)
        {
            var rows = map.GetLength(0);
            var cols = map.GetLength(1);

            for (var loop = 1;; loop++)
            {
                var newMap = (int[,]) map.Clone();
                for (var y = 0; y < rows; y++)
                {
                    for (var x = 0; x < rows; x++)
                    {
                        if (map[y, x] == 0) continue;

                        var neighbors = new (int x, int y)[] {
                            (x, y + 1),
                            (x + 1, y),
                            (x, y - 1),
                            (x - 1, y),
                        }.Where(n => {
                            if (n.y < 0 || n.y >= map.GetLength(0)) return false;
                            if (n.x < 0 || n.x >= map.GetLength(1)) return false;
                            return true;
                        }).ToArray();

                        foreach (var n in neighbors)
                        {
                            if (map[n.y, n.x] == 0)
                            {
                                newMap[y, x]--;
                                break;
                            }
                        }
                    }
                }
                map = newMap;

                if (map[rows-1, cols-1] == 0)
                {
                    return loop;
                }
            }
        }

        public static int[,] extend(int[][] map, int howMuch)
        {
            var origDim = (x: map[0].Length, y: map.Length);
            var newDim = (x: origDim.x * howMuch, y: origDim.y * howMuch);

            int[,] newMap = new int[newDim.y, newDim.x];
            for (var y = 0; y < newDim.y; y++)
            {
                for (var x = 0; x < newDim.x; x++)
                {
                    int yDist = y / origDim.y;
                    int xDist = x / origDim.x;
                    var origValue = map[y%origDim.y][x%origDim.x];
                    var maxDistance = xDist + yDist;
                    newMap[y,x] = (origValue + maxDistance) % 10;
                    // +1 because we wrapover to 1
                    if (newMap[y,x] < origValue) newMap[y,x] += 1;
                }
            }

            return newMap;
        }

        public static long P2(int[][] map)
        {
            var optimizedMap = extend(map, 5);

            // clear the first risk manually
            optimizedMap[0,0] = 0;
            var rows = optimizedMap.GetLength(0);
            var cols = optimizedMap.GetLength(1);

            var riskMap = new int[rows, cols];
            return mapper2(optimizedMap);
        }
    }
}
