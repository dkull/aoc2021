using System.Diagnostics;

/*

This solution contains many hardcoded values and is not a generic solution
by any stretch, but this makes the solution MUCH shorter and more readable overall.
Optimizations abound. Some long/complex optimizations have been left out for brevity.

*/

class Program
{
    static void Main(string[] args)
    {
        var data = LoadData(args[0]);
        Stopwatch sw = Stopwatch.StartNew();
        var result = Solution(data);
        Console.WriteLine($"Result: {result} in {sw.ElapsedMilliseconds} ms");
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
        var cost = Math.Abs(fromCol - toCol) + Math.Abs(1 - fromRow) + Math.Abs(1 - toRow);
        return cost * ourCost;
    }

    public static int? moveToRoomIfPossible(char[,] map, int row, int col)
    {
        // target room info
        var ourName = map[row, col];
        var roomCol = ourName switch { 'A' => 3, 'B' => 5, 'C' => 7, 'D' => 9, _ => throw new Exception("BADNAME") };
        var roomName = roomCol switch { 3 => 'A', 5 => 'B', 7 => 'C', 9 => 'D', _ => throw new Exception("bad room"), };

        // already in this room
        if (roomCol == col) return null;

        // check if someone blocking my way out of my current room
        if (row >= 3 && Enumerable.Range(2, row - 2).Any(r => map[r, col] != '.')) return null;

        // check if someone is blocking the top-room
        if (map[2, roomCol] != '.') return null;

        // someone blocking us in the hallway
        for (var i = Math.Min(roomCol, col); i <= Math.Max(roomCol, col); i++)
        {
            // if we are in hallway, ignore us
            if (row == 1 && i == col) continue;
            if (map[1, i] != '.') return null;
        }

        // check if room is occupied by stranger
        var roomStrangers = Enumerable.Range(2, map.GetLength(0) - 3)
            .Any(r => {
                var val = map[r, roomCol];
                return val != '.' && val != ourName;
            });
        if (roomStrangers) return null;

        // move to deepest free room
        var targetRoom = Enumerable.Range(2, map.GetLength(0) - 3)
            .TakeWhile(n => map[n, roomCol] == '.')
            .Last();

        var cost = moveCost(map, row, col, targetRoom, roomCol);
        map[targetRoom, roomCol] = ourName;
        map[row, col] = '.';

        return cost;
    }

    public static int? moveToHallwayIfPossible(char[,] map, int row, int col, int hallway)
    {
        // already in hall
        // Covered with only calling for room objs
        //if (row == 1) {counter[8]++; return null;}

        // infront of door
        // Covered with optimization
        //if (hallway == 3 || hallway == 5 || hallway == 7 || hallway == 9) {counter[4]++; return null;}

        // someone already there
        if (map[1, hallway] != '.') return null;

        // check if someone blocking my way out of my current room
        // Covered with canMove-optimization
        //if (row >= 3 && Enumerable.Range(2, row - 2).Any(r => map[r, col] != '.')) {counter[6]++; return null;}

        var ourName = map[row, col];
        var roomName = col switch {
            3 => 'A', 5 => 'B', 7 => 'C', 9 => 'D',
            _ => throw new Exception("bad room"),
        };
        // if i am already at homeroom
        if (roomName == ourName)
        {
            // don't exit room if no strangers
            var roomStrangers = Enumerable.Range(2, map.GetLength(0) - 3)
                .Any(r => {
                    var val = map[r, col];
                    return val != '#' && val != '.' && val != ourName;
                });
            if (!roomStrangers) return null;
        }

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

    public static bool canMoveUp(char[,] map, int row, int col)
    {
        return !(new char[]{'A','B','C','D'}.Contains(map[row-1, col]));
    }

    public static bool run(char[,] map, ref int[] bestresult, int result)
    {
        if (result + 1 >= bestresult[0]) return false;

        if (allAreHome(map))
        {
            if (result < bestresult[0])
            {
                bestresult[0] = result;
                Console.WriteLine($"new best: {bestresult[0]}");
            }
            return true;
        }

        var rooms = new int[]{ 3, 5, 7, 9 };
        var _map = (char[,])map.Clone();

        // loop over all hallway things
        for (var col = 1; col < map.GetLength(1) - 1; col++)
        {
            var row = 1;
            var val = map[1, col];
            if (val == '.') continue;
            var cost = moveToRoomIfPossible(_map, row, col);
            if (cost != null)
            {
                var finished = run(_map, ref bestresult, result + cost.Value);
                if (finished) return false;
                _map = (char[,])map.Clone();
            }
        }

        // loop over all objects in rooms
        for (var col = 3; col < map.GetLength(1) - 3; col+=2)
        {
            for (var row = 2; row < map.GetLength(0) - 1; row++)
            {
                var val = map[row, col];
                if (val == '.' || val == '#') continue;
                // optimization
                if (!canMoveUp(map, row, col)) continue;

                for (var hallway = 1; hallway <= 11; hallway++)
                {
                    // optimization
                    if (hallway == 3 || hallway == 5 || hallway == 7 || hallway == 9) continue;

                    var cost = moveToHallwayIfPossible(_map, row, col, hallway);
                    if (cost != null)
                    {
                        var _finished = run(_map, ref bestresult, result + cost.Value);
                        _map = (char[,])map.Clone();
                    }
                }
            }
        }

        return false;
    }

    public static long Solution(char[,] map)
    {
        var result = new int[1]{int.MaxValue};
        run(map, ref result, 0);
        return result[0] == int.MaxValue ? -1 : result[0];
    }
}
