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

        // make all lines left-to-right, top-to-bottom
        if ((fromCoords.X > toCoords.X) || (fromCoords.Y > toCoords.Y))
        {
            (fromCoords, toCoords) = (toCoords, fromCoords);
        }

        Console.WriteLine($"NewLine: {fromCoords} -> {toCoords}");

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
        /*Console.WriteLine("===========");
        mycoords.ToList().ForEach(x => Console.WriteLine($"MY --> {x}"));
        othercoords.ToList().ForEach(x => Console.WriteLine($"OT --> {x}"));
        othercoords.ToList().ForEach(x => Console.WriteLine($" --> {x}"));*/

        return mycoords.Intersect(othercoords).ToArray();
    }

    private (int X, int Y)[] coveredCoordinates()
    {
        var coords = new List<(int X, int Y)>();
        if (this.FromX < this.ToX)
        {
            var xAt = this.FromX;
            while (xAt <= this.ToX)
            {
                coords.Add((xAt, this.ToY));
                xAt += 1;
            }
        }
        if (this.FromY < this.ToY)
        {
            var yAt = this.FromY;
            while (yAt <= this.ToY)
            {
                coords.Add((this.ToX, yAt));
                yAt += 1;
            }
        }
        //Console.WriteLine($"{this} contains");
        //coords.ForEach(x => Console.WriteLine($"--> {x}"));
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

        var p1 = Part1(
            coords
                .Where(x => x.IsStraight())
                .ToArray()
        );

        var p2 = Part2(1);
        Console.WriteLine($"P1: {p1} P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static Line[] LoadData(string filepath)
    {
        string[] inputLines = File.ReadAllLines(filepath);

        return inputLines
            .Select(line => new Line(line))
            .ToArray();
    }

    static int Part1(Line[] lines)
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

    static int Part2(int i)
    {
        return 2;
    }
}
