using System.Diagnostics;

class Scanner
{
    public string scanner;
    public (int x, int y, int z)[] data;
    public (int x, int y, int z)[] originalData;
    (int x, int y, int z)[][]? _vectors; // just a cache
    public (int x, int y, int z) location;

    public Scanner(string chunk)
    {
        var lines = chunk.Split("\n");
        scanner = lines[0].Trim();
        Console.WriteLine($"creating '{scanner}'");
        data = lines[1..]
            .Where(line => line.Length > 1)
            .Select(line => {
                var tokens = line.Split(",").Select(x => int.Parse(x)).ToArray();
                return (tokens[0], tokens[1], tokens[2]);
            }).ToArray();

        originalData = data.ToArray();
        location = (0, 0, 0);
    }

    public static int manhattanDistance((int x, int y, int z) a, (int x, int y, int z) b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y)  + Math.Abs(a.z - b.z);
    }

    // sets the final merged location for this scanner - used for P2
    public void applyLocationOffset((int x, int y, int z) location)
    {
        this.location = location;
    }

    public void permutateAxes(int[] mask)
    {
        _vectors = null;
        data = data.Select(_row => {
            var row = new int[] {_row.x, _row.y, _row.z};
            return (x: row[mask[0]], y: row[mask[1]], z: row[mask[2]]);
        }).ToArray();
    }

    public void restoreOriginalData()
    {
        _vectors = null;
        data = originalData.ToArray();
    }

    public (int x, int y, int z) generateVector(int i, int j)
    {
        var a = data[i];
        var b = data[j];
        return (a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public (int x, int y, int z)[] generateVectors(int i)
    {
        var result = new List<(int x, int y, int z)>();
        for (var j = 0; j < data.Length; j++)
        {
            result.Add(generateVector(i, j));
        }
        return result.ToArray();
    }

    public (int x, int y, int z)[][] generateAllVectors()
    {
        // return cached data for Master sensor
        if (_vectors != null) return _vectors;

        var result = new List<(int x, int y, int z)[]>();
        for (var i = 0; i < data.Length; i++)
        {
            result.Add(generateVectors(i));
        }

        _vectors = result.ToArray();
        return result.ToArray();
    }

    public bool merge(Scanner other, (int ours, int theirs) matchingPair, int minMergeCount)
    {
        var ours = matchingPair.ours;
        var theirs = matchingPair.theirs;
        var offset = (x: data[ours].x - other.data[theirs].x, y: data[ours].y - other.data[theirs].y, z: data[ours].z - other.data[theirs].z);

        var newNodes = new List<(int x, int y, int z)>();
        foreach (var otherData in other.data)
        {
            var newnode = (otherData.x + offset.x, otherData.y + offset.y, otherData.z + offset.z);
            if (!data.Contains(newnode))
            {
                Console.WriteLine($"master does not contain {newnode}");
                newNodes.Add(newnode);
            }
        }

        if (other.data.Length - newNodes.Count() < minMergeCount)
        {
            return false;
        }

        data = data.Concat(newNodes).ToArray();
        // mark the position relative to Master
        other.applyLocationOffset(offset);

        _vectors = null;
        return true;
    }
}

class Program
{

    static void Main(string[] args)
    {
        var data = LoadData(args[0]);
        Stopwatch sw = Stopwatch.StartNew();
        var result = P1(data);
        Console.WriteLine($"P1: {result.p1} P2: {result.p2} in {sw.ElapsedMilliseconds} ms");

        /*data = LoadData(args[0]);
        sw = Stopwatch.StartNew();
        var p2 = P2(data);
        Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");*/
    }

    static Scanner[] LoadData(string filepath)
    {
        //var allLines = File.ReadAllLines(filepath);
        var data = File.ReadAllText(filepath)
            .Split("\n--- ")
            .Select(chunk => new Scanner(chunk))
            .ToArray();
        return data;
    }

    /*
        P1
    */

    public static bool manipulateScannerUntilCorrect(Scanner main, Scanner other, Func<Scanner, bool> testFunc)
    {
        // all permutations of axes
        var permutations = new int[][] {
            new int[]{0,1,2},
            new int[]{0,2,1},
            new int[]{1,0,2},
            new int[]{1,2,0},
            new int[]{2,0,1},
            new int[]{2,1,0},
        };
        var signs = new int[] { 1, -1 };
        other.restoreOriginalData();

        foreach (var perm in permutations)
        {
            foreach (var xsign in signs)
            {
                foreach (var ysign in signs)
                {
                    foreach (var zsign in signs)
                    {
                        other.permutateAxes(perm);
                        other.data = other.data.Select(r => (x: r.x * xsign, y: r.y * ysign, z: r.z * zsign)).ToArray();
                        if (testFunc(other)) return true;
                        other.restoreOriginalData();
                    }
                }
            }
        }
        return false;
    }

    public static bool findSamePoint(Scanner main, Scanner other, int minIntersects)
    {
        var mainVectors = main.generateAllVectors();

        var success = manipulateScannerUntilCorrect(main, other, scanner => {
            var otherVectors = other.generateAllVectors();
            for (var io = 0; io < otherVectors.Length; io++)
            {
                var o = otherVectors[io];
                for (var im = 0; im < mainVectors.Length; im++)
                {
                    var m = mainVectors[im];
                    var intersectCount = m.Intersect(o).Count();
                    if (intersectCount >= minIntersects)
                    {
                        var mergeResult = main.merge(other, (im, io), minIntersects);
                        if (mergeResult) return true;
                    }
                }
            }
            return false;
        });

        return success;
    }

    public static (long p1, long p2) P1(Scanner[] data)
    {
        const int minSimilars = 5; // this number can be tweaked, 5 works on all described cases

        var mainScanner = data[0];
        var ignoreScanners = new List<int>();

        while (ignoreScanners.Count() < data.Length - 1)
        {
            Console.WriteLine($"=== ITERATION (ignoring {ignoreScanners.Count()}/{data.Length-1}) ===");
            var foundAndMerged = false;
            for (var i = 1; i < data.Length; i++)
            {
                if (ignoreScanners.Contains(i)) continue;
                var otherScanner = data[i];
                Console.WriteLine($"--- {otherScanner.scanner}");
                foundAndMerged = findSamePoint(mainScanner, otherScanner, minSimilars);
                if (foundAndMerged)
                {
                    ignoreScanners.Add(i);
                    break;
                }
            }
            if (!foundAndMerged)
            {
                Console.WriteLine($"Failed to merge with {data.Length - 1 - ignoreScanners.Count()} Scanners left!");
                break;
            }
        }

        Console.WriteLine($"scanner 0 has {mainScanner.data.Length} nodes");

        var p2 = P2(data);

        return (mainScanner.data.Length, p2);
    }

    /*
        P2
    */

    public static long P2(Scanner[] scanners)
    {
        var longest = 0;
        foreach (var a in scanners)
        {
            foreach (var b in scanners)
            {
                longest = Math.Max(longest, Scanner.manhattanDistance(a.location, b.location));
            }
        }
        return longest;
    }
}
