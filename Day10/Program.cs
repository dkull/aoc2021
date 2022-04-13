using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var data = LoadData(args[0]);

        Stopwatch sw = Stopwatch.StartNew();
        var p1 = P1(data);
        Console.WriteLine($"P1: {p1} in {sw.ElapsedMilliseconds} ms");

        sw = Stopwatch.StartNew();
        var p2 = P2(data);
        Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static char[][] LoadData(string filepath)
    {
        return File
            .ReadAllLines(filepath)
            .Select(line => line.Select(c => c).ToArray())
            .ToArray();
    }

    public static int P1(char[][] data)
    {
        var result = 0;

        var match = new Dictionary<char, char>();
        match.Add('(', ')');
        match.Add('[', ']');
        match.Add('{', '}');
        match.Add('<', '>');

        var openers = match.Keys.ToArray();
        var closers = match.Values.ToArray();

        var corruptionScore = new Dictionary<char, int>();
        corruptionScore.Add(')', 3);
        corruptionScore.Add(']', 57);
        corruptionScore.Add('}', 1197);
        corruptionScore.Add('>', 25137);

        foreach (var row in data)
        {
            var stack = new List<char>();
            foreach (var symbol in row)
            {
                if (openers.Contains(symbol))
                {
                    stack = stack.Prepend(match[symbol]).ToList();
                }
                if (closers.Contains(symbol))
                {
                    if (stack.First() == symbol)
                    {
                        stack.RemoveAt(0);
                    } else {
                        result += corruptionScore[symbol];
                        break;
                    }
                }
            }
        }

        return result;
    }

    public static ulong P2(char[][] data)
    {
        var match = new Dictionary<char, char>();
        match.Add('(', ')');
        match.Add('[', ']');
        match.Add('{', '}');
        match.Add('<', '>');

        var openers = match.Keys.ToArray();
        var closers = match.Values.ToArray();

        var completionScore = new Dictionary<char, ulong>();
        completionScore.Add(')', 1);
        completionScore.Add(']', 2);
        completionScore.Add('}', 3);
        completionScore.Add('>', 4);

        var rowScores = new List<ulong>();

        foreach (var row in data)
        {
            var stack = new List<char>();
            ulong rowCompletionScore = 0;
            var corrupt = false;
            foreach (var symbol in row)
            {
                if (openers.Contains(symbol))
                {
                    stack = stack.Prepend(match[symbol]).ToList();
                }
                if (closers.Contains(symbol))
                {
                    if (stack.First() == symbol)
                    {
                        stack.RemoveAt(0);
                    } else {
                        corrupt = true;
                        break; // skip corrupted
                    }
                }
            }

            if (!corrupt)
            {
                stack.ForEach(x => rowCompletionScore = (rowCompletionScore * 5) + completionScore[x]);
                rowScores.Add(rowCompletionScore);
            }
        }

        rowScores.Sort();
        return rowScores[(rowScores.Count() - 1) / 2];
    }
}
