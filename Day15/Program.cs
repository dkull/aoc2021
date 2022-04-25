using System.Diagnostics;

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

            /*data = LoadData(args[0]);
            sw = Stopwatch.StartNew();
            var p2 = P2(data.Item1, data.Item2);
            Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");*/
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

        public static void try2(
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
                try2(map, neighbor, pos, riskMap);
            }
        }

        public static long P1(int[][] map)
        {
            // clear the first risk manually
            map[0][0] = 0;

            var riskMap = new Dictionary<(int x, int y), int>();
            try2(map, (x: 0, y: 0), null, riskMap);
            return riskMap[(map[0].Length-1, map.Length-1)];
        }

        /*
            P2
        */

        public static long P2(string seed, Dictionary<string, string> rules)
        {
            return 1;
        }
    }
}
