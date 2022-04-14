using System.Diagnostics;

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
        var p2 = P2(data);
        Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");*/
    }

    static (string, string)[] LoadData(string filepath)
    {
        return File
            .ReadAllLines(filepath)
            .Select(line => {
                var tokens = line.Split("-");
                return (tokens.First(), tokens.Last());
            }).ToArray();
    }

    private static Dictionary<string, string[]> buildMap((string a, string b)[] data)
    {
        var result = new Dictionary<string, string[]>();

        data = data
            .Concat(data.Select(d => (d.b, d.a)).ToArray())
            .ToArray();

        foreach (var pair in data)
        {
            if (!result.ContainsKey(pair.a)) { result.Add(pair.a, new string[]{}); }
            result[pair.a] = result[pair.a].Append(pair.b).ToArray();
        }

        return result;
    }

    private static int countUniquePaths(
        Dictionary<string, string[]> map,
        string at, string endAt, HashSet<string> blacklist)
    {
        if (at == endAt) return 1;

        if (char.IsLower(at.First()))
        {
            // do not allow paths to revisit this node
            blacklist.Add(at);
        }

        var result = 0;
        foreach (var connectedNode in map[at])
        {
            if (blacklist.Contains(connectedNode)) continue;
            Console.WriteLine($"exploring {at}->{connectedNode}");
            var subBlacklist = blacklist.ToHashSet();
            result += countUniquePaths(map, connectedNode, endAt, subBlacklist);
        }
        return result;
    }

    public static int P1((string, string)[] data)
    {
        var map = buildMap(data);
        foreach (var (k, v) in map)
        {
            Console.WriteLine($" {k} --> {string.Join(" ", v)}");
        }
        var paths = countUniquePaths(map, "start", "end", new HashSet<string>());
        return paths;
    }

    public static int P2(int[][] data)
    {
        return 0;
    }
}
