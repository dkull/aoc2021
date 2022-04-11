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

    static List<string> LoadData(string filepath)
    {
        var inputLines = File.ReadAllLines(filepath).ToList();
        return inputLines;
    }

    private static Dictionary<int, int[]> getSegmentMap()
    {
        var result = new Dictionary<int, int[]>();
        result.Add(0, new int[]{0,1,2,4,5,6});
        result.Add(1, new int[]{2,5});
        result.Add(2, new int[]{0,2,3,4,6});
        result.Add(3, new int[]{0,2,3,5,6});
        result.Add(4, new int[]{1,2,3,5});
        result.Add(5, new int[]{0,1,3,5,6});
        result.Add(6, new int[]{0,1,3,4,5,6});
        result.Add(7, new int[]{0,2,5});
        result.Add(8, new int[]{0,1,2,3,4,5,6});
        result.Add(9, new int[]{0,1,2,3,5,6});

        return result;
    }

    private static List<int[]> getPermutations(int[] data, int n)
    {
        var result = new List<int[]>();

        var c = new List<int>();
        for (var i = 0; i <= n; i++) c.Add(0);

        for (var i = 0; i < n;)
        {
            if (c[i] < i)
            {
                if (i % 2 == 0)
                {
                    (data[0], data[i]) = (data[i], data[0]);
                } else {
                    (data[c[i]], data[i]) = (data[i], data[c[i]]);
                }
                result.Add(data.ToArray()); // we clone the data
                c[i] += 1;
                i = 0;
            } else {
                c[i] = 0;
                i++;
            }
        }
        return result;
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

    private static int charToNum(char c)
    {
        return c - 97;
    }

    public static int? getDigit(int[] mapping, int[] input, Dictionary<int, int[]> truth)
    {
        var translated = input.Select(x => mapping[x]).ToArray();
        foreach (var (candidate_k, candidate_v) in truth)
        {
            if (translated.Length == candidate_v.Length && translated.All(x => candidate_v.Contains(x)))
            {
                return candidate_k;
            }
        }
        return null;
    }

    public static int[] solver(List<int[]> data, List<int[]> task)
    {
        data = (from d in data
               orderby d.Length
               select d).ToList();

        var allPerms = getPermutations(new int[]{0,1,2,3,4,5,6}, 7);
        var segments = getSegmentMap();

        foreach (var perm in allPerms)
        {
            var numbers = new int[]{0,1,2,3,4,5,6,7,8,9};
            foreach (var litSegments in data)
            {
                var digit = getDigit(perm, litSegments, segments);

                if (digit != null)
                {
                    numbers = numbers.Where(x => x != digit).ToArray();
                }
            }
            if (numbers.Length == 0)
            {
                return task
                    .Select(x => getDigit(perm, x, segments).Value)
                    .ToArray();
            }
        }

        throw new Exception("no perm found");
    }

    public static int P2(List<string> data)
    {
        var result = 0;

        foreach (var line in data)
        {
            Console.WriteLine($"Line: {line}");

            string[] input = line.Split(" | ").First().Split(" ");
            string[] output = line.Split(" | ").Last().Split(" ");

            var solverInputs = input.Select(i => i.ToCharArray().Select(c => charToNum(c)).ToArray()).ToList();
            var answerInputs = output.Select(i => i.ToCharArray().Select(c => charToNum(c)).ToArray()).ToList();
            var numbers = solver(solverInputs, answerInputs);

            result += int.Parse(string.Join("", numbers));
        }

        return result;
    }
}
