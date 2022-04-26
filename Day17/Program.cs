using System.Diagnostics;
using System.Text.RegularExpressions;

namespace App
{
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

        static (int x1, int x2, int y1, int y2) LoadData(string filepath)
        {
            var allLines = File.ReadAllLines(filepath);
            var firstLine = allLines[0];

            string pattern = @"[\-0-9]+";
            Regex rg = new Regex(pattern);
            var matches = rg.Matches(firstLine)
                .Select(x => int.Parse(x.Value))
                .ToArray();
            foreach (var t in matches)
            {
                Console.WriteLine($"{t}");
            }
            return (x1: matches[0], x2: matches[1], y1: matches[2], y2: matches[3]);
        }

        /*
            P1
        */

        public static void step(ref int x, ref int y, ref int velX, ref int velY)
        {
            x += velX;
            y += velY;
            _ = velX < 0 ? velX++ : velX > 0 ? velX-- : velX;
            velY--;
        }

        public static bool inTargetArea((int x, int y) pos, (int x1, int x2, int y1, int y2) target)
        {
            return (pos.x >= target.x1 && pos.x <= target.x2 && pos.y >= target.y1 && pos.y <= target.y2);
        }

        public static long P1((int x1, int x2, int y1, int y2) target)
        {
            // this logic doesn't catch all possible cases
            // we need to start to the left+above the target
            var highestY = 0;
            for (var x = 0; x < target.x2; x++)
            {
                for (var y = target.y1; y < 1000; y++)
                {
                    (int x, int y) pos = (0, 0);
                    (int x, int y) velocity = (x, y);

                    var loopHighestY = int.MinValue;
                    for (var loop = 0;; loop++)
                    {
                        if (pos.y < target.y1) break;

                        if (inTargetArea(pos, target))
                        {
                            var myhighest = highestY;
                            if (loopHighestY > highestY) highestY = loopHighestY;
                            break;
                        } else {
                            step(ref pos.x, ref pos.y, ref velocity.x, ref velocity.y);
                            if (pos.y > loopHighestY) loopHighestY = pos.y;
                        }
                    }
                }
            }
            return highestY;
        }

        /*
            P2
        */

        public static long P2((int x1, int x2, int y1, int y2) target)
        {
            // this logic doesn't catch all possible cases
            // we need to start to the left+above the target
            var goodPos = 0;
            for (var x = 0; x <= target.x2; x++)
            {
                for (var y = target.y1; y < 1000; y++)
                {
                    (int x, int y) pos = (0, 0);
                    (int x, int y) velocity = (x, y);

                    for (var loop = 0;; loop++)
                    {
                        if (pos.y < target.y1) break;

                        if (inTargetArea(pos, target))
                        {
                            goodPos++;
                            break;
                        } else {
                            step(ref pos.x, ref pos.y, ref velocity.x, ref velocity.y);
                        }
                    }
                }
            }
            return goodPos;
        }
    }
}
