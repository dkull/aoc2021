using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;


/*

This solution contains many hardcoded values and is not a generic solution
by any stretch, but this makes the solution MUCH shorter and more readable overall.

*/

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
        //var p2 = P2(data);
        //Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static char[,] LoadData(string filepath)
    {
        var lines = File.ReadAllLines(filepath);
        var result = new char[lines.Length, lines[0].Length];

        for (var i = 0; i < lines.Length; i++)
            for (var j = 0; j < lines[0].Length; j++)
                result[i, j] = j >= lines[i].Length ? ' ' : lines[i][j];
        return result;
    }

    public static bool allAreHome(char[,] map)
    {
        static bool colRoomsAre(char[,] map, int col, char name)
        {
            for (var i = 2; i < map.GetLength(0) - 1; i++)
            {
                if (map[i, col] != name) return false;
            }
            return true;
        }

         return colRoomsAre(map, 3, 'A') &&
            colRoomsAre(map, 5, 'B') &&
            colRoomsAre(map, 7, 'C') &&
            colRoomsAre(map, 9, 'D');
    }

    public static int moveCost(char[,] map, int fromRow, int fromCol, int toRow, int toCol)
    {
        var ourName = map[fromRow, fromCol];
        var ourCost = ourName switch { 'A' => 1, 'B' => 10, 'C' => 100, 'D' => 1000, _ => throw new Exception("BADNAME") };
        //var cost = Math.Abs(fromCol - toCol) + Math.Abs(1 - fromRow) + Math.Abs(1 - toRow);
        if (toRow == 1)
        {
            // manhattan distance to get to the hallway
            return (Math.Abs(fromRow - toRow) + Math.Abs(fromCol - toCol)) * ourCost;
        } else {
            // up+down+width to get to another room
            return (Math.Abs(fromCol - toCol) + (fromRow-1) + (toRow-1)) * ourCost;
        }
    }

    public static int? moveToRoomIfPossible(char[,] map, int row, int col, ref Dictionary<int, int> counter)
    {
        // target room info
        var ourName = map[row, col];
        var roomCol = ourName switch { 'A' => 3, 'B' => 5, 'C' => 7, 'D' => 9, _ => throw new Exception("BADNAME") };
        var roomName = roomCol switch { 3 => 'A', 5 => 'B', 7 => 'C', 9 => 'D', _ => throw new Exception("bad room"), };

        // already in this room
        //if (roomCol == col) { counter[1]++; return null; }

        // check if someone blocking my way out of my current room
        //if (row >= 3 && Enumerable.Range(2, row - 2).AsEnumerable().Any(r => map[r, col] != '.')) {counter[5]++; return null;}

        // check if someone is blocking the top-room
        if (map[2, roomCol] != '.') {counter[1]++; return null;}

        // someone blocking us in the hallway
        for (var i = Math.Min(roomCol, col); i <= Math.Max(roomCol, col); i++)
        {
            // if we are in hallway, ignore us
            if (row == 1 && i == col) continue;
            if (map[1, i] != '.') {counter[2]++; return null;}
        }

        // check if room is occupied by stranger
        var roomStrangers = Enumerable.Range(2, map.GetLength(0) - 3)
            //.Select(r => map[r, roomCol])
            //.Where(c => c != '.' && c != ourName)
            .Any(r => {
                var val = map[r, roomCol];
                return val != '.' && val != ourName;
            });
        if (roomStrangers) {counter[3]++; return null;}

        // move to last free room
        var targetRoom = Enumerable.Range(2, map.GetLength(0) - 3)
            .TakeWhile(n => map[n, roomCol] == '.')
            .Last();

        var cost = moveCost(map, row, col, targetRoom, roomCol);
        map[targetRoom, roomCol] = ourName;
        map[row, col] = '.';

        return cost;
    }

    public static int? moveToHallwayIfPossible(char[,] map, int row, int col, int hallway, ref Dictionary<int, int> counter)
    {
        // already in hall
        //if (row == 1) {counter[8]++; return null;}

        // infront of door
        if (hallway == 3 || hallway == 5 || hallway == 7 || hallway == 9) {counter[4]++; return null;}

        // someone already there
        if (map[1, hallway] != '.') { counter[5]++; return null;}

        // check if someone blocking my way out of my current room
        if (row >= 3 && Enumerable.Range(2, row - 2).Any(r => map[r, col] != '.')) {counter[6]++; return null;}

        // if i am already at home and stable
        var ourName = map[row, col];
        var roomStrangers = Enumerable.Range(2, map.GetLength(0) - 2)
            .Any(r => {
                var val = map[r, col];
                return val != '#' && val != '.' && val != ourName;
            });
        // don't exit room if no strangers
        var roomName = col switch {
            3 => 'A', 5 => 'B', 7 => 'C', 9 => 'D', _ => throw new Exception("bad room"),
        };
        if (roomName == ourName && !roomStrangers) {counter[7]++; return null;}

        // someone blocking us in the hallway
        for (var i = Math.Min(hallway, col); i <= Math.Max(hallway, col); i++)
        {
            // ignore us
            if (i == col) continue;
            if (map[1, i] != '.') {counter[8]++; return null;}
        }

        var cost = moveCost(map, row, col, 1, hallway);
        map[1, hallway] = ourName;
        map[row, col] = '.';
        return cost;
    }

    public static bool naiveDistanceFailure(char[,] map, ref int[] bestresult)
    {
        var costsum = 0;
        for (var row = 0; row < map.GetLength(0); row++)
        {
            for (var col = 0; col < map.GetLength(1); col++)
            {
                var ourName = map[row, col];
                if (ourName == '.' || ourName == '#' || ourName == ' ') continue;
                var (ourCost, homeCol) = ourName switch { 'A' => (1, 3), 'B' => (10, 5), 'C' => (100, 7), 'D' => (1000, 9), _ => throw new Exception("BADNAME") };
                //var xDelta = Math.Abs(homeCol - col);
                //var yDelta = xDelta > 0 ? Math.Abs(1 - row) + 1 : 0;
                //costsum += (xDelta + yDelta) * ourCost;
                costsum += Math.Abs(homeCol - col) * ourCost;
                if (costsum + 1 >= bestresult[0]) return true;
            }
        }
        return costsum >= bestresult[0];
    }

    public static bool canMove(char[,] map, int row, int col)
    {
        return !(new char[]{'A','B','C','D'}.Contains(map[row-1, col]));
    }

    public static void printMap(char[,] map)
    {
        for (var row = 0; row < map.GetLength(0); row++)
        {
            for (var col = 0; col < map.GetLength(1); col++)
            {
                Console.Write($"{map[row, col]}");
            }
            Console.WriteLine();
        }
    }

    public static bool run(char[,] map, ref int[] bestresult, int result, int depth, ref Dictionary<int, int> counter)
    {
        if (result >= bestresult[0]) return false;

        // optimization
        if (naiveDistanceFailure(map, ref bestresult)) return false;

        if (allAreHome(map))
        {
            if (result < bestresult[0])
            {
                bestresult[0] = result;
                Console.WriteLine($"{bestresult[0]}");
            }
            return true;
        }

        var rooms = new int[]{ 3, 5, 7, 9 };
        var _map = (char[,])map.Clone();


        for (var row = 2; row < map.GetLength(0) - 1; row++)
        {
            for (var col = 3; col < map.GetLength(1) - 3; col+=2)
            {
                var val = map[row, col];
                if (val == '.' || val == '#' || val == ' ') continue;
                // optimization
                if (!canMove(map, row, col)) continue;

                //List<int> hallways = Enumerable.Range(1, 11).ToList();
                //hallways.Sort((a, b) => Math.Abs(a - col).CompareTo(Math.Abs(b - col)));
                //foreach (var hallway in hallways)
                for (var hallway = 1; hallway <= 11; hallway++)
                {
                    var cost = moveToHallwayIfPossible(_map, row, col, hallway, ref counter);
                    if (cost != null)
                    {
                        var finished = run(_map, ref bestresult, result + cost.Value, depth+1, ref counter);
                        _map = (char[,])map.Clone();
                    }
                }
            }
        }

        // loop over all hallway things
        for (var col = 1; col < map.GetLength(1) - 1; col++)
        {
            var row = 1;
            var val = map[1, col];
            if (val == '.' || val == '#' || val == ' ') continue;
            // optimization
            if (!canMove(map, row, col)) continue;
            var cost = moveToRoomIfPossible(_map, row, col, ref counter);
            if (cost != null)
            {
                var finished = run(_map, ref bestresult, result + cost.Value, depth+1, ref counter);
                if (finished) return false;
                _map = (char[,])map.Clone();
            }
        }

        return false;
    }


    /*
        P1
    */


    public static long P1(char[,] map)
    {
        var counter = new Dictionary<int, int>();
        for (var i = 1; i <= 8; i++) { counter.Add(i, 0); }
        var result = new int[1]{int.MaxValue};
        run(map, ref result, 0, 0, ref counter);
        foreach (var (k, v) in counter)
        {
            Console.WriteLine($"{k} -> {v}");
        }
        return result[0];
    }

    /*
        P2
    */

    public static long P2(char[,] map)
    {
        var result = new int[1]{0};
        //run(map, ref result, 0, 0);
        //return result[0];
        return -1;
    }
}
