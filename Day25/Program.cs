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

    static char[,] LoadData(string filepath)
    {
        var lines = File.ReadAllLines(filepath);
        var rowCount = lines.Count();
        var colCount = lines[0].Trim().Count();

        var map = new char[rowCount, colCount];

        for (var r = 0; r < rowCount; r++)
            for (var c  = 0; c < colCount; c++)
                map[r,c] = lines[r][c];

        return map;
    }

    static void printMap(ref char[,] map)
    {
        for (var r = 0; r < map.GetLength(0); r++)
        {
            for (var c = 0; c < map.GetLength(1); c++)
            {
                Console.Write($"{map[r,c]}");
            }
            Console.WriteLine();
        }
    }

    /*
        P1
    */

    public static bool moveEasties(ref char[,] map)
    {
        var copy = (char[,])map.Clone();
        var moved = false;
        for (var r = 0; r < map.GetLength(0); r++)
        {
            for (var c = 0; c < map.GetLength(1); c++)
            {
                var newCol = (c + 1) % map.GetLength(1);
                if (copy[r, c] == '>' && copy[r, newCol] == '.')
                {
                    moved = true;
                    map[r, c] = '.';
                    map[r, newCol] = '>';
                }
            }
        }
        return moved;
    }

    public static bool moveSouthies(ref char[,] map)
    {
        var copy = (char[,])map.Clone();
        var moved = false;
        for (var r = 0; r < map.GetLength(0); r++)
        {
            for (var c = 0; c < map.GetLength(1); c++)
            {
                var newRow = (r + 1) % map.GetLength(0);
                if (copy[r, c] == 'v' && copy[newRow, c] == '.')
                {
                    moved = true;
                    map[r, c] = '.';
                    map[newRow, c] = 'v';
                }
            }
        }
        return moved;
    }

    public static int P1(char[,] map)
    {
        var moved = true;

        var i = 0;
        for (; moved; i++)
        {
            Console.WriteLine($"Done steps {i}");
            moved = moveEasties(ref map);
            moved = moveSouthies(ref map) || moved;
            //printMap(ref map);
        }
        printMap(ref map);

        return i;
    }

    /*
        P2
    */

    public static int P2(char[,] map)
    {
        return -1;
    }
}
