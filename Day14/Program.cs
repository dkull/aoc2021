using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var data = LoadData(args[0]);
        Stopwatch sw = Stopwatch.StartNew();
        var p1 = P1(data.Item1, data.Item2);
        Console.WriteLine($"P1: {p1} in {sw.ElapsedMilliseconds} ms");

        data = LoadData(args[0]);
        sw = Stopwatch.StartNew();
        var p2 = P2(data.Item1, data.Item2);
        Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");
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

    /*
        P1
    */

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

    /*
        P2
    */

    public static Dictionary<string, long> transformP2(Dictionary<string, long> input, Dictionary<string, string> rules)
    {
        var output = new Dictionary<string, long>();
        foreach (var (k, v) in input)
        {
            if (rules.ContainsKey(k))
            {
                var thing = rules[k];
                var newthing1 = k[0..1] + thing;
                var newthing2 = thing + k[1..2];
                if (!output.ContainsKey(newthing1)) output[newthing1] = 0;
                output[newthing1] += v;
                if (!output.ContainsKey(newthing2)) output[newthing2] = 0;
                output[newthing2] += v;
            } else { Console.WriteLine($"rules do not contain {k}"); }
        }
        return output;
    }

    public static long P2(string seed, Dictionary<string, string> rules)
    {
        Console.WriteLine($"seed: {seed}");
        var inCounter = new Dictionary<string, long>();

        for (var i = 0; i < seed.Length - 1; i++)
        {
            var key = seed[i..(i+2)];
            if (!inCounter.ContainsKey(key)) inCounter[key] = 0;
            inCounter[key] += 1;
        }

        for (var step = 1; step <= 40; step++)
        {
            inCounter = transformP2(inCounter, rules);
        }

        // count all the characters

        var counter = new Dictionary<char, long>();
        foreach (var (k, v) in inCounter)
        {
            //if (!counter.ContainsKey(k[0])) counter[k[0]] = 0;
            //counter[k[0]] += v;
            // count each character once, because every character is part of two
            // pairs
            if (!counter.ContainsKey(k[1])) counter[k[1]] = 0;
            counter[k[1]] += v;
        }

        long mostCommon = counter.Values.Max();
        long leastCommon = counter.Values.Min();

        Console.WriteLine($"{mostCommon} - {leastCommon} - {mostCommon - leastCommon}");
        var result = mostCommon - leastCommon;

        // FIXME:
        // P2 is off by one with real case. probably something to do with counting
        // overlaps

        return result;
    }
}
