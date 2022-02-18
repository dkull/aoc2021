// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        Stopwatch sw = Stopwatch.StartNew();

        string input = File.ReadAllText(args[0]);

        var p1 = Part1(input);
        var p2 = Part2(input);
        Console.WriteLine($"P1: {p1} P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    public static int Part1(string input)
    {
        int depth = 0;
        int horiz = 0;

        List<string> _ = input.Split('\n')
            .Where(x => x.Length > 0)
            .Select(line =>
                {
                    var tokens = line.Split(' ');
                    string command = tokens[0];
                    int value = int.Parse(tokens[1]);

                    var foo = command switch
                    {
                        "forward" => horiz += value,
                        "down" => depth += value,
                        "up" => depth -= value,
                        _ => depth += 0,
                    };

                    return line;
                }).ToList();

        return depth * horiz;
    }

    public static int Part2(string input)
    {
        int depth = 0;
        int horiz = 0;
        int aim = 0;

        List<string> _ = input.Split('\n')
            .Where(x => x.Length > 0)
            .Select(line =>
                {
                    var tokens = line.Split(' ');
                    string command = tokens[0];
                    int value = int.Parse(tokens[1]);

                    if (command.Equals("forward"))
                    {
                        horiz += value;
                        depth += (aim * value);
                    }
                    else if (command.Equals("down"))
                    {
                        aim += value;
                    }
                    else if (command.Equals("up"))
                    {
                        aim -= value;
                    };

                    return line;
                }).ToList();

        return depth * horiz;
    }
}
