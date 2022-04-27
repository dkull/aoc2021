using System.Diagnostics;

class Slider
{
    public string[] data;

    public Slider(string line)
    {
        //Console.WriteLine($"Slider for {line}");
        var cleaned = line
            .Replace(",", " ")
            .Replace(",", " ")
            .Replace("[", "[ ")
            .Replace("]", " ]")
            .Split(' ')
            .Where(x => x != " ")
            .ToArray();
        data = cleaned;
    }

    public void propagateLeft(string value, int _i)
    {
        for (var i = _i; i >= 0; i--)
        {
            if (data[i] != "[" && data[i] != "]")
            {
                data[i] = (int.Parse(data[i]) + int.Parse(value)).ToString();
                return;
            }
        }
    }

    public void propagateRight(string value, int _i)
    {
        for (var i = _i; i < data.Length; i++)
        {
            if (data[i] != "[" && data[i] != "]")
            {
                data[i] = (int.Parse(data[i]) + int.Parse(value)).ToString();
                return;
            }
        }
    }

    public void replaceMeWithNumber(int a, int b, int number)
    {
        data = data[0..a].Concat(new string[]{number.ToString()}).Concat(data[(b+1)..]).ToArray();
    }

    public void replaceMeWithPair(int i, int fst, int snd)
    {
        data = data[0..i].Concat(new string[]{ "[", fst.ToString(), snd.ToString(), "]" }).Concat(data[(i+1)..]).ToArray();
    }

    public int? parseNumber(string value)
    {
        try {
            return int.Parse(value);
        } catch { return null; }
    }

    public bool process()
    {
        // all explodies first
        var depth = -1;
        for (var i = 0; i < data.Length; i++)
        {
            if (depth == 4)
            {
                //Console.WriteLine($"pre-explode : {string.Join(' ', data)}");
                var left = data[i];
                var right = data[i+1];
                propagateLeft(left, i-1);
                propagateRight(right, i+2);
                replaceMeWithNumber(i-1, i+2, 0);
                //Console.WriteLine($"post-explode: {string.Join(' ', data)}");
                return true;
            }
            if (data[i] == "[") depth++;
            if (data[i] == "]") depth--;
        }

        // and then all splits
        depth = -1;
        for (var i = 0; i < data.Length; i++)
        {
            var maybeNumber = parseNumber(data[i]);
            if (maybeNumber != null && maybeNumber >= 10)
            {
                //Console.WriteLine($"pre-split : {string.Join(' ', data)}");
                var fst = (int)Math.Floor(maybeNumber.Value / 2.0);
                var snd = (int)Math.Ceiling(maybeNumber.Value / 2.0);
                replaceMeWithPair(i, fst, snd);
                //Console.WriteLine($"post-split: {string.Join(' ', data)}");
                return true;
            }

            if (data[i] == "[") depth++;
            if (data[i] == "]") depth--;
        }
        return false;
    }

    public void add(Slider other)
    {
        this.data = new string[]{"["}.Concat(this.data).Concat(other.data).Concat(new string[]{"]"}).ToArray();
    }

    public int countScore()
    {
        // final answer boilds down to 1 number
        while (data.Length > 1)
        {
            for (var i = 0; i < data.Length; i++)
            {
                var maybeNumberLeft = parseNumber(data[i]);
                var maybeNumberRight = i+1 >= data.Length ? null : parseNumber(data[i+1]);
                if (maybeNumberLeft != null && maybeNumberRight != null)
                {
                    var magnitude = (maybeNumberLeft * 3) + (maybeNumberRight * 2);
                    replaceMeWithNumber(i-1, i+2, magnitude.Value);
                }
            }
        }

        return int.Parse(data[0]);
    }
}

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

    static string[] LoadData(string filepath)
    {
        var allLines = File.ReadAllLines(filepath);
        return allLines.ToArray();
    }

    /*
        P1
    */

    public static long P1(string[] data)
    {
        var sliders = data.Select(x => new Slider(x)).ToArray();

        Slider mainSlider = sliders[0];
        foreach (var slider in sliders[1..])
        {
            mainSlider.add(slider);
            while (true)
            {
                var doprocess = mainSlider.process();
                if (!doprocess) break;
            }
        }

        var result = mainSlider.countScore();
        return result;
    }

    /*
        P2
    */

    public static long P2(string[] data)
    {
        var largestMagnitude = 0;

        for (var a = 0; a < data.Length; a++)
        {
            for (var b = 0; b < data.Length; b++)
            {
                if (a == b) continue;
                var slider = new Slider(data[a]);
                var sliderOther = new Slider(data[b]);
                slider.add(sliderOther);

                while (true)
                {
                    var doprocess = slider.process();
                    if (!doprocess) break;
                }
                var magnitude = slider.countScore();
                largestMagnitude = Math.Max(largestMagnitude, magnitude);
            }
        }

        return largestMagnitude;
    }
}
