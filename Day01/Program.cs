// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

namespace Day01;

public class Program
{
    public static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();

        string input = File.ReadAllText(args[0]);

        int[] inp = input.Split('\n')
            .Where(x => x.Length > 0)
            .Select(x => int.Parse(x))
            .ToArray();

        int p1Result = CalcResult(inp, 1);
        int p2Result = CalcResult(inp, 3);
        Console.WriteLine($"P1 result: {p1Result} P2 result: {p2Result} in {sw.ElapsedMilliseconds}ms");
    }

    public static int CalcResult(IEnumerable<int> data, int windowSize)
    {
        int[] summed = CollectionToWindowed(data, windowSize)
            .Select(window => window.Sum()).ToArray();
        return summed.Zip(summed[1..])
            .Count(x => x.First < x.Second);
    }

    public static List<List<int>> CollectionToWindowed(IEnumerable<int> _data, int windowSize)
    {
        int[] data = _data.ToArray();
        int elemCount = data.Length;

        List<List<int>> result = new();
        for (int i = 0; i < (elemCount - windowSize + 1); i++)
        {
            List<int> currentWindow = data[i..(i+windowSize)].ToList();
            result.Add(currentWindow);
        }

        return result;
    }
}
