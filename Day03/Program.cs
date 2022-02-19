// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();

        string input = File.ReadAllText(args[0]);

        var p1 = Part1(input);
        var p2 = Part2(input);
        Console.WriteLine($"P1: {p1} P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    public static int DominantColumnElement(int[][] matrix, int column)
    {
        int rowCount = matrix.Length;
        Dictionary<int, int> counts = new();

        for (int row = 0; row < rowCount; row++)
        {
            int value = matrix[row][column];
            if (!counts.ContainsKey(value))
            {
                counts.Add(value, 0);
            }
            counts[value] += 1;
        }

        var dominant = counts.Aggregate((result, kvPair) => kvPair.Value > result.Value ? kvPair : result);
        return dominant.Key;
    }

    public static uint Part1(string input)
    {
        // extract all bits to a matrix
        int[][] matrix = input.Split('\n')
            .Where(x => x.Length > 0)
            .Select(line => line.ToArray().Select(x => int.Parse(x.ToString())).ToArray())
            .ToArray();

        // create a column index iterator
        int columnCount = matrix[0].Length;
        int[] range = Enumerable.Range(0, columnCount).ToArray();

        // find the dominant bit in each column
        int[] dominantBits = range
            .Select(col => DominantColumnElement(matrix, col))
            .ToArray();

        // create a string of "0"s and "1"s from the dominant bit array
        var binString = string.Concat(dominantBits.Select(x => x.ToString()));

        var dominantNumber = Convert.ToUInt32(binString, 2);

        // do some bit magic to flip the bits
        var recessiveNumber = ~(dominantNumber << (32 - dominantBits.Length)) >> (32 - dominantBits.Length);

        return dominantNumber * recessiveNumber;
    }

    public static int Part2(string input)
    {
        return 0;
    }
}
