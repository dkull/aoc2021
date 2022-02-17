// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

namespace Day01;

public class Day01 {
    public static void Main(string[] args) {
        Stopwatch sw = Stopwatch.StartNew();

        string input = File.ReadAllText(args[0]);

        int[] inp = input.Split('\n')
            .Where(x => x.Length > 0)
            .Select(x => int.Parse(x))
            .ToArray();

        var pairs = inp.Zip(inp[1..]);

        int result = pairs.Count(x => x.First < x.Second);

        Console.WriteLine($"Result: {result} in {sw.ElapsedMilliseconds}ms");
    }
}
