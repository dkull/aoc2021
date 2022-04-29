using System.Linq;
using System.Diagnostics;

class Dice
{
    int value = 0;
    int rolls = 0;

    public Dice() {}

    public int get()
    {
        rolls++;
        value = (value % 100) + 1;
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
        var dice = new Dice();
        var p1score = 0;
        var p2score = 0;
        var p1at = players.p1 - 1;
        var p2at = players.p2 - 1;
        while (true)
        {
            var rollSum = Enumerable.Range(0, 3).Select(_x => dice.get()).Sum();
            p1at = (p1at + rollSum) % 10;
            p1score += p1at + 1;
            if (Math.Max(p1score, p2score) >= 1000) break;

            rollSum = Enumerable.Range(0, 3).Select(_x => dice.get()).Sum();
            p2at = (p2at + rollSum) % 10;
            p2score += p2at + 1;
            if (Math.Max(p1score, p2score) >= 1000) break;
        }

        var dicerolls = dice.getRollCount();
        var minScore = Math.Min(p1score, p2score);

        return dicerolls * minScore;
    }

    /*
        P2
    */

    public static long P2((int p1, int p2) players)
    {
        return -1;
    }
}
