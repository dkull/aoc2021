using System.Diagnostics;

class Zoomer
{
    char[] algo;
    char[][] image;
    char infinityPixel;
    bool canFullLight;
    bool canFullDark;

    public Zoomer(char[] algo, char[][] image)
    {
        this.algo = algo;
        this.image = image;
        this.infinityPixel = '.';

        var fullLight = this.bitsToOffset("#########".ToArray());
        var fullDark = this.bitsToOffset(".........".ToArray());

        canFullDark = this.getReplacement(fullLight) == '.';
        canFullLight = this.getReplacement(fullDark) == '#';

        Console.WriteLine($" canFullDark {canFullDark} canFullLight {canFullLight}");
    }

    public char getPixel(int x, int y)
    {
        if (y < 0 || y >= image.Length) return infinityPixel;
        if (x < 0 || x >= image[0].Length) return infinityPixel;
        return image[y][x];
    }

    public char[] getBits(int xx, int yy)
    {
        var result = new char[9];
        var i = 0;
        for (var y = -1; y <= 1; y++)
        {
            for (var x = -1; x <= 1; x++)
            {
                result[i++] = getPixel(xx + x, yy + y);
            }
        }
        return result;
    }

    public int bitsToOffset(char[] bits)
    {
        var number = 0;
        for (var i = 0; i < bits.Length; i++)
        {
            number <<= 1;
            var bit = bits[i];
            number |= bit == '#' ? 1 : 0;
        }
        return number;
    }

    public char getReplacement(int offset)
    {
        return algo[offset];
    }

    public void toggleInfinity()
    {
        if (infinityPixel == '.')
        {
            if (canFullLight) infinityPixel = '#';
        }
        else
        {
            if (canFullDark) infinityPixel = '.';
        }
        Console.WriteLine($"toggled infinity to {infinityPixel}");
    }

    public void enhance()
    {
        var padding = 1;

        var result = new List<char[]>();
        for (var y = 0; y < (image.Length + (padding*2)); y++)
        {
            char[] row = new char[image[0].Length + (padding*2)];
            result.Add(row);
        }
        for (var y = -padding; y < (image.Length + padding); y++)
        {
            for (var x = -padding; x < (image[0].Length + padding); x++)
            {
                var bits = getBits(x, y);
                var offset = bitsToOffset(bits);
                var replacement = getReplacement(offset);
                result[y+padding][x+padding] = replacement;
            }
        }
        image = result.ToArray();
        toggleInfinity();
    }

    public int countPixels(char pixel)
    {
        var result = 0;
        for (var y = 0; y < image.Length; y++)
            for (var x = 0; x < image[0].Length; x++)
                _ = image[y][x] == pixel ? result++ : result;
        return result;
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

    static Zoomer LoadData(string filepath)
    {
        var allLines = File.ReadAllLines(filepath);
        var algo = allLines[0].ToArray();
        var input = allLines[2..].Select(line => line.ToArray()).ToArray();
        return new Zoomer(algo, input);
    }

    /*
        P1
    */

    public static long P1(Zoomer z)
    {
        for (var i = 0; i < 2; i++)
        {
            z.enhance();
        }
        return z.countPixels('#');
    }

    /*
        P2
    */

    public static long P2(Zoomer z)
    {
        for (var i = 0; i < 50; i++)
        {
            z.enhance();
        }
        return z.countPixels('#');
    }
}
