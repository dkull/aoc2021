﻿using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        int[][] data = LoadData(args[0]);

        Stopwatch sw = Stopwatch.StartNew();
        var p1 = P1(data);
        Console.WriteLine($"P1: {p1} in {sw.ElapsedMilliseconds} ms");

        //sw = Stopwatch.StartNew();
        //var p2 = P2(data);
        //Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static int[][] LoadData(string filepath)
    {
        return File
            .ReadAllLines(filepath)
            .Select(line => line.Select(c => int.Parse(c.ToString())).ToArray())
            .ToArray();
    }

    public static int P1(int[][] data)
    {
        var result = 0;
        for (var y = 0; y < data.Length; y++)
        {
            for (var x = 0; x < data[y].Length; x++)
            {
                var myValue = data[y][x];

                var neighbors = new List<(int, int)>();
                neighbors.Add((x, y - 1));
                neighbors.Add((x, y + 1));
                neighbors.Add((x - 1, y));
                neighbors.Add((x + 1, y));

                var allHigher = true;
                foreach (var (nX, nY) in neighbors)
                {
                    try
                    {
                        var neighborValue = data[nY][nX];
                        if (neighborValue <= myValue)
                        {
                            allHigher = false;
                            break;
                        }
                    } catch {}
                };

                if (allHigher)
                {
                    Console.WriteLine($"new lowpoint X: {x} Y: {y} => {myValue}");
                    result += 1 + myValue;
                }
            }
        }
        return result;
    }

    public static int P2(List<string> data)
    {
        var result = 0;
        return result;
    }
}
