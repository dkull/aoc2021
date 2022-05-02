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
        var p2 = P2(data);
        Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");
    }

    static char[,] LoadData(string filepath)
    {
        var result = new char[5, 13];
        var lines = File.ReadAllLines(filepath);
        for (var i = 0; i < lines.Length; i++)
            for (var j = 0; j < 13; j++)
                result[i, j] = j >= lines[i].Length ? ' ' : lines[i][j];
        return result;
    }

    public static bool allAreHome(char[,] map)
    {
        return
            map[2,3] == 'A' &&
            map[3,3] == 'A' &&
            map[2,5] == 'B' &&
            map[3,5] == 'B' &&
            map[2,7] == 'C' &&
            map[3,7] == 'C' &&
            map[2,9] == 'D' &&
            map[3,9] == 'D';
    }

    public static int moveCost(char[,] map, int fromRow, int fromCol, int toRow, int toCol)
    {
        var ourName = map[fromRow, fromCol];
        var ourCost = ourName switch { 'A' => 1, 'B' => 10, 'C' => 100, 'D' => 1000, _ => throw new Exception("BADNAME") };
        if (toRow == 1)
        {
            return (Math.Abs(fromRow - toRow) + Math.Abs(fromCol - toCol)) * ourCost;
        } else {
            return (Math.Abs(fromCol - toCol) + fromRow-1 + toRow-1) * ourCost;
        }
    }

    public static int? moveToRoomIfPossible(char[,] map, int row, int col, int roomCol)
    {
        // target room info
        var ourName = map[row, col];
        var roomName = roomCol switch {
            3 => 'A', 5 => 'B', 7 => 'C', 9 => 'D', _ => throw new Exception("bad room"),
        };

        // already in this room
        if (roomCol == col) return null;

        // check if room is for us
        if (roomName != map[row, col]) return null;

        // check if someone is blocking the top-room
        if (map[2, roomCol] != '.') return null;

        // check if room-top is occupied by stranger
        if (new char[]{'A', 'B', 'C', 'D'}.Contains(map[2, roomCol]) && map[2, roomCol] != ourName) return null;

        // check if room-bottom is occupied by stranger
        if (new char[]{'A', 'B', 'C', 'D'}.Contains(map[3, roomCol]) && map[3, roomCol] != ourName) return null;

        // check if someone blocking my way out of my current room
        if (row == 3 && map[row-1,col] != '.') return null;

        // someone blocking us in the hallway
        for (var i = Math.Min(roomCol, col); i <= Math.Max(roomCol, col); i++)
        {
            // if we are in hallway, ignore us
            if (row == 1 && i == col) continue;
            if (map[1, i] != '.') return null;
        }

        // if bottom row is occupied then move to top room
        var targetRoom = map[3, roomCol] != '.' ? 2 : 3;
        var cost = moveCost(map, row, col, targetRoom, roomCol);
        map[targetRoom, roomCol] = ourName;
        map[row, col] = '.';

        return cost;
    }

    public static int? moveToHallwayIfPossible(char[,] map, int row, int col, int hallway)
    {
        var ourName = map[row, col];

        // already in hall
        if (row == 1) return null;

        // infrontof door FIXME: this extends beyond doors but probably not needed (yet?)
        if (hallway % 2 == 1) return null;

        // check if someone blocking my way out of my current room
        if (row == 3 && map[row-1,col] != '.') return null;

        // if i am already at home and stable
        var roomName = col switch {
            3 => 'A', 5 => 'B', 7 => 'C', 9 => 'D', _ => throw new Exception("bad room"),
        };
        if (row == 2 && roomName == ourName && map[3, col] == ourName) return null;
        if (row == 3 && roomName == ourName) return null;

        // someone blocking us in the hallway
        for (var i = Math.Min(hallway, col); i <= Math.Max(hallway, col); i++)
        {
            // ignore us
            if (i == col) continue;
            if (map[1, i] != '.') return null;
        }

        var cost = moveCost(map, row, col, 1, hallway);
        map[1, hallway] = ourName;
        map[row, col] = '.';
        return cost;
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

    public static bool run(char[,] map, ref int[] bestresult, int result, int depth)
    {
        if (allAreHome(map))
        {
            if (bestresult[0] == 0) bestresult[0] = result;
            if (result < bestresult[0])
            bestresult[0] = Math.Min(bestresult[0], result);
            return true;
        }

        for (var row = 0; row < 5; row++)
        {
            for (var col = 0; col < 13; col++)
            {
                var val = map[row, col];
                if (val == '.' || val == '#' || val == ' ') continue;

                var finished = false;

                // we have a creature, let's try all possible moves
                var rooms = new int[]{ 3, 5, 7, 9 };
                foreach (var room in rooms)
                {
                    //if (bestresult[0] != 0 && bestresult[0] <= result) return true;
                    var _map = (char[,])map.Clone();
                    var cost = moveToRoomIfPossible(_map, row, col, room);
                    if (cost != null)
                    {
                        finished = run(_map, ref bestresult, result + cost.Value, depth+1);
                        if (finished) break; // no more optimal thing available
                    }
                }

                for (var hallway = 1; hallway <= 11; hallway++)
                {
                    //if (bestresult[0] != 0 && bestresult[0] <= result) return true;
                    var _map = (char[,])map.Clone();
                    var cost = moveToHallwayIfPossible(_map, row, col, hallway);
                    if (cost != null)
                    {
                        finished = run(_map, ref bestresult, result + cost.Value, depth+1);
                        if (finished) break; // no more optimal thing available
                    }
                }
            }
        }

        return false;
    }


    /*
        P1
    */


    public static long P1(char[,] map)
    {
        var result = new int[1]{0};
        run(map, ref result, 0, 0);
        return result[0];
    }

    /*
        P2
    */

    public static long P2(char[,] map)
    {
        return -1;
    }
}
