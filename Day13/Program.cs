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

    static ((int x, int y)[], (string axis, int coordinate)[]) LoadData(string filepath)
    {
        var allLines = File.ReadAllLines(filepath);

        var points = allLines
            .Where(x => x.Contains(','))
            .Select(x => x.Split(','))
            .Select(x => (int.Parse(x.First()), int.Parse(x.Last())))
            .ToArray();

        var folds = allLines
            .Where(x => x.Contains('='))
            .Select(x => x.Split(' ').Last().Split('='))
            .Select(x => (x.First(), int.Parse(x.Last())))
            .ToArray();

        return (points, folds);
    }

    public static (int x, int y) translate((int x, int y) point, (string axis, int coordinate) fold)
    {
        var newpoint = fold.axis switch {
            "x" => ((point.x < fold.coordinate) ? point.x : fold.coordinate * 2 - point.x, point.y),
            "y" => (point.x, (point.y < fold.coordinate) ? point.y : fold.coordinate * 2 - point.y),
            _ => throw new Exception("bad axis")
        };
        return newpoint;
    }

    public static int P1((int x, int y)[] points, (string axis, int coordinate)[] folds)
    {
        var translatedPoints = new HashSet<(int x, int y)>();
        foreach (var point in points)
        {
            translatedPoints.Add(translate(point, folds[0]));
        }
        return translatedPoints.Count;
    }

    public static int P2((int x, int y)[] points, (string axis, int coordinate)[] folds)
    {
        var translatedPoints = new HashSet<(int x, int y)>();
        foreach (var point in points)
        {
            var finalPoint = point;
            foreach (var fold in folds)
            {
                finalPoint = translate(finalPoint, fold);
            }
            translatedPoints.Add(finalPoint);
        }

        var maxX = translatedPoints.Select(n => n.x).Max();
        var maxY = translatedPoints.Select(n => n.y).Max();

        for (var y = 0; y <= maxY; y++)
        {
            for (var x = 0; x <= maxX; x++)
            {
                var outp = translatedPoints.Contains((x, y)) ? "#" : ".";
                Console.Write($"{outp}");
            }
            Console.WriteLine();
        }

        return translatedPoints.Count;
    }
}
