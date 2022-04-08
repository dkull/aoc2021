using System.Diagnostics;

public class Line
{
    int _fromX;
    int _fromY;
    int _toX;
    int _toY;

    public int FromX { get; set; }
    public int FromY { get; set; }
    public int ToX { get; set; }
    public int ToY { get; set; }

    public Line(string rawLine)
    {
        string[] tokens = rawLine.Split(" -> ");

        var fromCoords = ParseCoords(tokens[0]);
        var toCoords = ParseCoords(tokens[1]);

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

    public (int X, int Y)[] overlappers(Line other)
    {
        var mycoords = coveredCoordinates();
        var othercoords = other.coveredCoordinates();
        return mycoords.Intersect(othercoords).ToArray();
    }

    private (int X, int Y)[] coveredCoordinates()
    {
        var coords = new List<(int X, int Y)>();

        var xDelta = this.ToX - this.FromX;
        var yDelta = this.ToY - this.FromY;
        // calc unit value of delta
        var xStep = xDelta != 0 ? xDelta / Math.Abs(xDelta) : 0;
        var yStep = yDelta != 0 ? yDelta / Math.Abs(yDelta) : 0;

        var atX = this.FromX;
        var atY = this.FromY;

        // how do we do the check?
        while (atX != this.ToX+xStep || atY != this.ToY+yStep) {
                coords.Add((atX, atY));
                atX += xStep;
                atY += yStep;
        }
        return coords.ToArray();
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

        var p1 = calcOverlaps(
            coords
                .Where(x => x.IsStraight())
                .ToArray()
        );

        var p2 = calcOverlaps( coords.ToArray() );

        Console.WriteLine($"P1: {p1} P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static Line[] LoadData(string filepath)
    {
        string[] inputLines = File.ReadAllLines(filepath);

        return inputLines
            .Select(line => new Line(line))
            .ToArray();
    }


    static int calcOverlaps(Line[] lines)
    {
        var overlaps = new HashSet<(int, int)>();
        foreach (var line_a in lines)
        {
            foreach (var line_b in lines)
            {
                if (line_a == line_b) continue;
                var overlappingCoords = line_a.overlappers(line_b);
                foreach (var overlapper in overlappingCoords)
                {
                    overlaps.Add(overlapper);
                }
            }
        }
        return overlaps.Count();
    }
}
