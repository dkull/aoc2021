using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var data = LoadData(args[0]);
        Stopwatch sw = Stopwatch.StartNew();
        var p1 = P1(data);
        Console.WriteLine($"P1: {p1} in {sw.ElapsedMilliseconds} ms");

        data = LoadData(args[0]);
        sw = Stopwatch.StartNew();
        var p2 = P2(data);
        Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static int[][] LoadData(string filepath)
    {
        return File
            .ReadAllLines(filepath)
            .Select(line => line.Select(c => int.Parse(c.ToString())).ToArray())
            .ToArray();
    }

    private static void incrementAll(ref int[][] data)
    {
        for (var y = 0; y < data.Length; y++)
            for (var x = 0; x < data[0].Length; x++)
                data[y][x]++;
    }

    private static void resetFlashed(ref int[][] data)
    {
        for (var y = 0; y < data.Length; y++)
            for (var x = 0; x < data[0].Length; x++)
                if (data[y][x] == -1) data[y][x] = 0;
    }

    private static void incrementNeighbors(ref int[][] data, int x, int y)
    {
        var pairs = new (int, int)[] {
            (x, y - 1),
            (x + 1, y - 1),
            (x + 1, y),
            (x + 1, y + 1),
            (x, y + 1),
            (x - 1, y + 1),
            (x - 1, y),
            (x - 1, y - 1)
        };
        foreach (var (px, py) in pairs)
        {
            try {
                if (data[py][px] >= 0) data[py][px]++;
            } catch {};
        }
    }

    private static int propagateFlashes(ref int[][] data)
    {
        var flashes = 0;
        for (var y = 0; y < data.Length; y++)
        {
            for (var x = 0; x < data[0].Length; x++)
            {
                if (data[y][x] > 9)
                {
                    flashes++;
                    data[y][x] = -1; // mark it as 'flashed'
                    incrementNeighbors(ref data, x, y);
                }
            }
        }
        return flashes;
    }

    public static int P1(int[][] data)
    {
        var result = 0;
        for  (var step = 1; step <= 100; step++)
        {
            incrementAll(ref data);
            var flashes = 0;
            while ((flashes = propagateFlashes(ref data)) > 0)
            {
                result += flashes;
            }
            resetFlashed(ref data);
        }
        return result;
    }

    public static int P2(int[][] data)
    {
        var allFlashCount = data.Length * data[0].Length;

        for  (var step = 1;; step++)
        {
            incrementAll(ref data);
            var stepFlashes = 0;
            var flashes = 0;
            while ((flashes = propagateFlashes(ref data)) > 0)
            {
                stepFlashes += flashes;
            }
            resetFlashed(ref data);

            if (stepFlashes == allFlashCount) return step;
        }
    }
}
