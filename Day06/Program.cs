using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();

        var fish = LoadData(args[0]);

        var p1 = P1(fish, 80);
        var p2 = P2(fish, 256);

        Console.WriteLine($"P1: {p1} P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static int[] LoadData(string filepath)
    {
        string[] inputLines = File.ReadAllLines(filepath);
        return inputLines[0]
            .Split(",")
            .Select(x => int.Parse(x))
            .ToArray();
    }

    public static int P1(int[] _fish, int days)
    {
        List<int> fish = _fish.ToList();
        for (int day = 0; day < days; day++)
        {
            //Console.WriteLine($"Day {day}, fish {fish.Count()}");

            List<int> spawn = new();

            fish
                .Where(f => f == 0)
                .ToList()
                .ForEach((f) => {
                    spawn.Add(8);
                    spawn.Add(6);
                });

            fish = fish
                .Where(f => f != 0)
                .Select(f=>f - 1)
                .Concat(spawn)
                .ToList();

            //Console.WriteLine($"added {spawn.Count()} spawn");
        }

        return fish.Count();
    }

    public static ulong P2(int[] fish_orig, int days)
    {
        var fish = new Dictionary<int, ulong>();

        for (var i = 0; i <= 8; i++)
        {
            fish.Add(i, 0);
        }

        fish_orig.ToList().ForEach(f => fish[f] += 1);


        ulong fishCount = 0;
        for (int day = 0; day <= days; day++)
        {
            fishCount = fish.Values.ToList().Aggregate((ulong)0, (res, now) => res + now);
            Console.WriteLine($"{day} has {fishCount} fish");

            var spawnCount = fish[0];

            for (var i = 0; i < 8; i++)
            {
                fish[i] = fish[i+1];
            }

            fish[8] = spawnCount;
            fish[6] += spawnCount;
        }

        return fishCount;
    }
}
