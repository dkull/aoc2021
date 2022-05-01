using System.Text.RegularExpressions;
using System.Diagnostics;

class Instruction
{
    public long x1;
    public long x2;
    public long y1;
    public long y2;
    public long z1;
    public long z2;

    public bool turnOn;

    public Instruction(string line)
    {
        turnOn = line.StartsWith("on");

        var ints = Regex.Matches(line, @"[\-\d]+").Select(d => int.Parse(d.Value)).ToArray();
        x1 = ints[0];
        x2 = ints[1];
        y1 = ints[2];
        y2 = ints[3];
        z1 = ints[4];
        z2 = ints[5];
    }

    public Instruction(long x1, long x2, long y1, long y2, long z1, long z2, bool turnOn)
    {
        this.x1 = x1;
        this.x2 = x2;
        this.y1 = y1;
        this.y2 = y2;
        this.z1 = z1;
        this.z2 = z2;
        this.turnOn = turnOn;
    }

    public long calcArea()
    {
        return (x2 - x1 + 1) * (y2 - y1 + 1) * (z2 - z1 + 1);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var data = LoadData(args[0]);
        Stopwatch sw = Stopwatch.StartNew();
        var result = P1(data);
        Console.WriteLine($"P1: {result} in {sw.ElapsedMilliseconds} ms");

        data = LoadData(args[0]);
        sw = Stopwatch.StartNew();
        var p2 = P2(data);
        Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static Instruction[] LoadData(string filepath)
    {
        return File.ReadAllLines(filepath)
            .AsEnumerable()
            .Select(line => new Instruction(line))
            .ToArray();
    }

    /*
        P1
    */

    public static long P1(Instruction[] insts)
    {
        var turnons = new HashSet<(long, long, long)>();
        foreach (var inst in insts)
        {
            if ((inst.x1 < -50) || (inst.x2 > 50)) continue;
            if ((inst.y1 < -50) || (inst.y2 > 50)) continue;
            if ((inst.z1 < -50) || (inst.z2 > 50)) continue;

            for (var x = inst.x1; x <= inst.x2; x++)
            {
                for (var y = inst.y1; y <= inst.y2; y++)
                {
                    for (var z = inst.z1; z <= inst.z2; z++)
                    {
                        if (inst.turnOn)
                        {
                            turnons.Add((x, y, z));
                        }
                        else
                        {
                            turnons.Remove((x, y, z));
                        }
                    }
                }
            }
        }

        return turnons.LongCount();
    }

    /*
        P2
    */

    public static bool cubesOverlap(Instruction a, Instruction b)
    {
        return
            a.x1 <= b.x2 &&
            a.x2 >= b.x1 &&
            a.y1 <= b.y2 &&
            a.y2 >= b.y1 &&
            a.z1 <= b.z2 &&
            a.z2 >= b.z1;
    }

    public static Instruction? cubeOverlapCoordinates(Instruction a, Instruction b)
    {
        if (cubesOverlap(a, b) == false) return null;

        var x1 = Math.Max(a.x1, b.x1);
        var x2 = Math.Min(a.x2, b.x2);
        var y1 = Math.Max(a.y1, b.y1);
        var y2 = Math.Min(a.y2, b.y2);
        var z1 = Math.Max(a.z1, b.z1);
        var z2 = Math.Min(a.z2, b.z2);

        var state = a.turnOn == b.turnOn ? !b.turnOn : b.turnOn;

        return new Instruction(x1, x2, y1, y2, z1, z2, state);
    }

    public static long doStuff(Instruction[] insts)
    {
        var universe = new List<Instruction>();
        foreach (var main in insts)
        {
            var newUniverse = new List<Instruction>();
            foreach (var old in universe)
            {
                var shared = cubeOverlapCoordinates(old, main);
                if (shared != null)
                {
                    newUniverse.Add(shared);
                }
            }

            if (main.turnOn)
            {
                universe.Add(main);
            }
            universe = universe.Concat(newUniverse).ToList();
        }

        var cubes = universe.Where(c => c.turnOn).Select(c => c.calcArea()).Sum();
        var negative = universe.Where(c => !c.turnOn).Select(c => c.calcArea()).Sum();
        Console.WriteLine($"Positive: {cubes} Negative: {negative}");

        return cubes - negative;
    }

    public static long P2(Instruction[] insts)
    {
        return doStuff(insts);
    }
}
