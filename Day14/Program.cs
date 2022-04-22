using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var data = LoadData(args[0]);
        Stopwatch sw = Stopwatch.StartNew();
        var p1 = P1(data.Item1, data.Item2);
        Console.WriteLine($"P1: {p1} in {sw.ElapsedMilliseconds} ms");

        /*data = LoadData(args[0]);
        sw = Stopwatch.StartNew();
        var p2 = P2(data.Item1, data.Item2);
        Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");*/
    }

    static (string seed, Dictionary<string, string>) LoadData(string filepath)
    {
        var allLines = File.ReadAllLines(filepath);

        var seed = allLines.First();
        var rules = new Dictionary<string, string>();
        allLines
            .Where(x => x.Contains(" -> "))
            .Select(x => x.Split(" -> "))
            .ToList()
            .ForEach(x => rules.Add(x.First(), x.Last()));

        return (seed, rules);
    }

    public static string transform(string polymer, Dictionary<string, string> rules)
    {
        var output = new List<string>();
        for (var i = 0; i < polymer.Length - 1; i++)
        {
            var sub = polymer[i..(i+2)];
            if (rules.ContainsKey(sub))
            {
                output.Add(sub[0] + rules[sub]);
            } else {
                output.Add(sub);
            }
        }
        output.Add(polymer[^1..]);
        return string.Join("", output);
    }

    public static int P1(string seed, Dictionary<string, string> rules)
    {
        var polymer = seed;
        for (var step = 1; step <= 10; step++)
        {
            polymer = transform(polymer, rules);
            //Console.WriteLine($"after step {step} have {polymer.Length} {polymer}");
        }

        var counts = new Dictionary<char, int>();
        foreach (var elem in polymer)
        {
            if (!counts.ContainsKey(elem)) counts[elem] = 0;
            counts[elem]++;
        }

        var mostCommon = counts.Values.Max();
        var leastCommon = counts.Values.Min();

        return mostCommon - leastCommon;
    }
}
