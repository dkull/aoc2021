using System.Diagnostics;

class Dice
{
    int value = 0;
    int rolls = 0;
    int max;

    public Dice(int max)
    {
        this.max = max;
    }

    public int get()
    {
        rolls++;
        value = (value % max) + 1;
        return value;
    }

    public int getRollCount()
    {
        return rolls;
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

    static (int p1, int p2) LoadData(string filepath)
    {
        var allLines = File.ReadAllLines(filepath);
        var p1 = int.Parse(allLines[0].Split(" ")[^1]);
        var p2 = int.Parse(allLines[1].Split(" ")[^1]);
        return (p1, p2);
    }

    /*
        P1
    */

    public static long P1((int p1, int p2) players)
    {
        var dice = new Dice(100);
        var p1score = 0;
        var p2score = 0;
        var p1at = players.p1 - 1;
        var p2at = players.p2 - 1;
        while (true)
        {
            var rollSum = Enumerable.Range(0, 3).Select(_x => dice.get()).Sum();
            p1at = (p1at + rollSum) % 10;
            p1score += p1at + 1;
            //Console.WriteLine($"p1: {p1score}");
            if (Math.Max(p1score, p2score) >= 1000) break;

            rollSum = Enumerable.Range(0, 3).Select(_x => dice.get()).Sum();
            p2at = (p2at + rollSum) % 10;
            p2score += p2at + 1;
            //Console.WriteLine($"p2: {p2score}");
            if (Math.Max(p1score, p2score) >= 1000) break;
        }

        var dicerolls = dice.getRollCount();
        var minScore = Math.Min(p1score, p2score);

        return dicerolls * minScore;
    }


    public static (long p1, long p2) game(Dictionary<int, int> map, int _universes, int _p1score, int _p2score, int _p1at, int _p2at, bool p1turn)
    {
        if (_p1score >= 21) return (1, 0);
        if (_p2score >= 21) return (0, 1);

        var wins = (p1: (long)0, p2: (long)0);
        foreach (var (rollSum, universes) in map)
        {
            var pat = p1turn ? _p1at : _p2at;
            var pscore = p1turn ? _p1score : _p2score;

            pat = (pat + rollSum) % 10;
            pscore += pat + 1;

            (long p1, long p2) result;
            if (p1turn)
            {
                result = game(map, universes, pscore, _p2score, pat, _p2at, !p1turn);
            }
            else
            {
                result = game(map, universes, _p1score, pscore, _p1at, pat, !p1turn);
            }

            wins.p1 += result.p1 * universes;
            wins.p2 += result.p2 * universes;
        }

        return wins;
    }

    /*
        P2
    */

    public static long P2((int p1, int p2) players)
    {
        // 1313078869 is too low
        var map = new Dictionary<int, int>();
        map.Add(3, 1);
        map.Add(4, 3);
        map.Add(5, 6);
        map.Add(6, 7);
        map.Add(7, 6);
        map.Add(8, 3);
        map.Add(9, 1);

        var p1at = players.p1 - 1;
        var p2at = players.p2 - 1;

        var wins = game(map, 1, 0, 0, p1at, p2at, true);

        return Math.Max(wins.p1, wins.p2);
    }
}
