using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();

        var crabs = LoadData(args[0]);

        var p1 = P1(crabs);
        var p2 = P2(crabs);

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

    public static int P1(int[] crabs)
    {
        var min = crabs.Min();
        var max = crabs.Max();

        var bestFuel = -1; // reasonable max
        for (var pos = min; pos < max; pos++)
        {
            var fuelCost = crabs
                .ToList()
                .Select(x => Math.Abs(x - pos))
                .Sum();

            if (fuelCost < bestFuel || bestFuel == -1)
            {
                bestFuel = fuelCost;
            }
        }

        return bestFuel;
    }

    public static int P2(int[] crabs)
    {
        var min = crabs.Min();
        var max = crabs.Max();

        var bestFuel = -1; // reasonable max
        for (var pos = min; pos < max; pos++)
        {
            var fuelCost = crabs
                .ToList()
                .Select(x => Enumerable.Range(1, (Math.Abs(x - pos))).ToList().Sum(x=>x))
                .Sum();

            if (fuelCost < bestFuel || bestFuel == -1)
            {
                bestFuel = fuelCost;
            }
        }

        return bestFuel;
    }
}
