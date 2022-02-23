using System.Diagnostics;

public class BingoBoard
{
    int[] _marked;
    int[][] _rows;

    public BingoBoard(string chunk)
    {
        _marked = new int[0];
        _rows = ParseChunk(chunk);
    }

    int[][] ParseChunk(string chunk)
    {
        Console.WriteLine($"Parse Chunk:\n{chunk}");

        string[] rows = chunk.Split("\n");
        return rows
            .Where(row => row.Length > 1)
            .Select(row =>
                row
                .Split(" ")
                .Where(x => !x.Equals(""))
                .Select(x => int.Parse(x))
                .ToArray()
        ).ToArray();
    }

    public void CrossOut(int number)
    {
        _marked = _marked.Append(number).ToArray();
    }

    public int? LastCrossed()
    {
        return _marked[^1];
    }

    public void ResetCrossed()
    {
        _marked = new int[0];
    }

    public int CountCrossed()
    {
        return _marked.Length;
    }

    public bool IsWinner()
    {
        // if any row is full
        foreach (var row in _rows)
        {
            var matchCount = row
                .Intersect(_marked)
                .ToArray()
                .Length;

            if (matchCount == row.Length) return true;
        }

        // if any column is full
        int colCount = _rows[0].Length;
        for (var i = 0; i < colCount; i++)
        {
            int[] col = _rows.Select(r => r[i]).ToArray();
            var matchCount = col
                .Intersect(_marked)
                .ToArray()
                .Length;

            if (matchCount == colCount) return true;
        }

        return false;
    }

    public int CountUnmarked()
    {
        return _rows
            .SelectMany(x => x) // flatten 1/2
            .Distinct()         // flatten 2/2
            .Where(x => !_marked.Contains(x))
            .Sum();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();

        var data = LoadData(args[0]);

        var p1 = Part1(data.Numbers, data.Boards);

        // reset the boards for part2
        data.Boards.ToList().ForEach(x => x.ResetCrossed());

        var p2 = Part2(data.Numbers, data.Boards);
        Console.WriteLine($"P1: {p1} P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static (int[] Numbers, BingoBoard[] Boards) LoadData(string filepath)
    {
        string[] inputChunks = File.ReadAllText(filepath).Split("\n\n");

        // extract the first line as sequence of integers
        int[] numbers = inputChunks[0]
            .Split(",")
            .Select(n => int.Parse(n))
            .ToArray();

        BingoBoard[] boards = inputChunks[1..]
            .Select(chunk => new BingoBoard(chunk))
            .ToArray();

        return (numbers, boards);
    }

    static void PlayBoards(int[] numbers, ref BingoBoard[] boards)
    {
        foreach (int number in numbers)
        {
            foreach (BingoBoard board in boards)
            {
                if (!board.IsWinner()) board.CrossOut(number);
            }
        }
    }

    static int Part1(int[] numbers, BingoBoard[] boards)
    {
        PlayBoards(numbers, ref boards);
        BingoBoard firstWinner = boards
            .Where(x => x.IsWinner())
            .OrderBy(x => x.CountCrossed())
            .First();

        return firstWinner.CountUnmarked() * firstWinner.LastCrossed()?? throw new Exception("no winner");
    }

    static int Part2(int[] numbers, BingoBoard[] boards)
    {
        PlayBoards(numbers, ref boards);
        BingoBoard lastWinner = boards
            .Where(x => x.IsWinner())
            .OrderBy(x => x.CountCrossed())
            .Reverse()
            .First();

        return lastWinner.CountUnmarked() * lastWinner.LastCrossed()?? throw new Exception("no winner");
    }
}
