using System.Diagnostics;

public class LineDrawer
{
    int _fromX;
    int _fromY;
    int _toX;
    int _toY;

    public int FromX { get; set; }
    public int FromY { get; set; }
    public int ToX { get; set; }
    public int ToY { get; set; }

    public LineDrawer(string rawLine)
    {
        string[] tokens = rawLine.Split(" -> ");

        var fromCoords = ParseCoords(tokens[0]);
        var toCoords = ParseCoords(tokens[1]);
        Console.WriteLine(fromCoords);

        FromX = fromCoords.X;
        FromY = fromCoords.Y;
        ToX = toCoords.X;
        ToY = toCoords.Y;
    }

    (int X, int Y) ParseCoords(string data)
    {
        var tokens = data
            .Split(",")
            .Select(x => int.Parse(x))
            .ToArray();
        return (tokens[0], tokens[1]);
    }

    public bool IsStraight()
    {
        return FromX == ToX || FromY == ToY;
    }

    public override string ToString()
    {
        return $"{FromX},{FromY} -> {ToX},{ToY}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();

        var coords = LoadData(args[0]);

        var p1 = Part1(
            coords
                .Where(x => x.IsStraight())
                .ToArray()
        );

        var p2 = Part2(1);
        Console.WriteLine($"P1: {p1} P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static LineDrawer[] LoadData(string filepath)
    {
        string[] inputLines = File.ReadAllLines(filepath);

        return inputLines
            .Select(line => new LineDrawer(line))
            .ToArray();
    }

    static int Part1(LineDrawer[] lines)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        return 1;
    }

    static int Part2(int i)
    {
        return 2;
    }
}
