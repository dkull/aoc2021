using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var data = LoadData(args[0]);

        Stopwatch sw = Stopwatch.StartNew();
        var p1 = P1(data);
        Console.WriteLine($"P1: {p1} in {sw.ElapsedMilliseconds} ms");

        //sw = Stopwatch.StartNew();
        //var p2 = P2(crabs);
        //Console.WriteLine($"P1: {p1} P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static List<string> LoadData(string filepath)
    {
        var inputLines = File.ReadAllLines(filepath).ToList();
        return inputLines;
    }

    public static int P1(List<string> data)
    {
        var result = data
            .Select(line =>
                line
                    .Split("| ")
                    .Last()
                    .Split(" ")
                    .Where(x => x != "")
                    .Count(x => (new int[]{2,3,4,7}).ToList().Contains(x.Length))
            ).Sum();

        return result;
    }

    public static int P2(int[] crabs)
    {
        return 0;
    }
}
