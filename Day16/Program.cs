using System.Diagnostics;
using System;

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

            /*data = LoadData(args[0]);
            sw = Stopwatch.StartNew();
            var p2 = P2(data);
            Console.WriteLine($"P2: {p2} in {sw.ElapsedMilliseconds} ms");*/
        }

        static int[][] LoadData(string filepath)
        {
            var translate = new Dictionary<char, int[]>();
            var symbols = new char[]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
            for (var i = 0; i < symbols.Length; i++)
            {
                var binaryRepr = Convert.ToString(i, 2).PadLeft(4, '0');
                var bitArray = binaryRepr.Select(x => int.Parse(x.ToString())).ToArray();
                Console.WriteLine($"translating {symbols[i]} to {binaryRepr}");
                translate.Add(symbols[i], bitArray);
            }

            var allLines = File.ReadAllLines(filepath);
            return allLines
                .Select(x => {
                    var translated = x.Select(y => translate[y]).ToArray();
                    var output = new List<int>();
                    foreach (var t in translated)
                    {
                        output = output.Concat(t).ToList();
                    }
                    return output.ToArray();
                })
                .ToArray();
        }

        /*
            P1
        */

        public static int readIntFromBits(ref int ptr, int[] bits, int nrOfBits)
        {
            var slice = bits[ptr..(ptr+nrOfBits)];
            ptr += nrOfBits;
            var result = 0;
            for (var i = 0; i < slice.Length; i++)
            {
                result <<= 1;
                result |= slice[i];
            }
            return result;
        }

        public static (int version, int type)? readHeader(ref int ptr, int[] bits)
        {
            if (bits.Length < ptr + 6) return null;
            return (version: readIntFromBits(ref ptr, bits, 3), type: readIntFromBits(ref ptr, bits, 3));
        }

        public static long readLiteral(ref int ptr, int[] bits)
        {
            var resultBits = "";
            while (true)
            {
                var last = bits[ptr++] == 0;
                var number = readIntFromBits(ref ptr, bits, 4);
                var binaryRepr = Convert.ToString(number, 2).PadLeft(4, '0');
                resultBits += binaryRepr;
                if (last) break;
            }
            return Convert.ToInt64(resultBits, 2);
        }

        public static long P1(int[][] data)
        {
            var versionsSum = 0;
            foreach (var bits in data)
            {
                versionsSum = 0;
                var ptr = 0;
                Console.WriteLine($"# input {string.Join("", bits)}");
                while (true)
                {
                    try {
                        var _header = readHeader(ref ptr, bits);
                        if (_header == null ) break;
                        var header = ((int version, int type))_header;

                        versionsSum += header.version;
                        if (header.type == 4)
                        {
                            var literal = readLiteral(ref ptr, bits);
                            Console.WriteLine($"> literal(ptr:{ptr}): {literal}");
                        } else {
                            var lengthTypeId = bits[ptr++];
                            var subpacketLength = readIntFromBits(ref ptr, bits, lengthTypeId == 0 ? 15 : 11);
                            Console.WriteLine($"> subpacket len: {subpacketLength}");
                        }
                    } catch {
                        break;
                    }
                }
                Console.WriteLine($"SUM: {versionsSum}");
            }
            return versionsSum;
        }

        /*
            P2
        */

        public static long P2(int[][] map)
        {
            return 0;
        }
    }
}
