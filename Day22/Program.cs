using System.Text.RegularExpressions;
using System.Diagnostics;

class Instructions
{
    public int x1;
    public int x2;
    public int y1;
    public int y2;
    public int z1;
    public int z2;

    public bool turnOn;

    public Instructions(string line)
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

    static Instructions[] LoadData(string filepath)
    {
        return File.ReadAllLines(filepath)
            .AsEnumerable()
            .Select(line => new Instructions(line))
            .ToArray();
    }

    /*
        P1
    */

    public static long P1(Instructions[] insts)
    {
        var turnons = new HashSet<(int, int, int)>();
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

    public static long P2(Instructions[] inst)
    {
        return -1;
    }
}
