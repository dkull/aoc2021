using System.Diagnostics;

enum Opcode
{
    inp, add, mul, div, mod, eql
};

enum Register
{
    w, x, y, z
}

class Instruction
{
    public Opcode op;
    public Register target;
    public object source;

    public Instruction(string line)
    {
        var tokens = line.Split(' ');
        op = tokens[0] switch
        {
            "inp" => Opcode.inp,
            "add" => Opcode.add,
            "mul" => Opcode.mul,
            "div" => Opcode.div,
            "mod" => Opcode.mod,
            "eql" => Opcode.eql,
            _ => throw new Exception($"bad opcode {tokens[0]}")
        };

        target = tokens[1] switch
        {
            "w" => Register.w,
            "x" => Register.x,
            "y" => Register.y,
            "z" => Register.z,
            _ => throw new Exception($"bad register {tokens[1]}")
        };

        if (tokens.Length == 3)
        {
            source = tokens[2] switch
            {
                "w" => Register.w,
                "x" => Register.x,
                "y" => Register.y,
                "z" => Register.z,
                _ => long.Parse(tokens[2])
            };
        }
        else
        {
            source = -1;
        }
    }
}

class ALU
{
    Instruction[] instructions;
    long regW;
    long regX;
    long regY;
    long regZ;
    public int atInstruction;

    public ALU(Instruction[] instructions)
    {
        this.instructions = instructions;
        reset();
    }

    public ALU Clone()
    {
        var newAlu = new ALU(this.instructions);
        newAlu.atInstruction = this.atInstruction;
        newAlu.regW = this.regW;
        newAlu.regX = this.regX;
        newAlu.regY = this.regY;
        newAlu.regZ = this.regZ;
        return newAlu;
    }

    public ALU CloneClean()
    {
        return new ALU(this.instructions);
    }

    public void reset()
    {
        regW = 0;
        regX = 0;
        regY = 0;
        regZ = 0;
        atInstruction = 0;
    }

    public bool processInputs(long[] inputs)
    {
        var done = false;
        foreach (var input in inputs)
        {
            done = processInput(input);
        }
        return done;
    }

    public bool processInput(long? nextInput)
    {
        long readRegister(object reg)
        {
            switch (reg) {
                case Register.w: return regW;
                case Register.x: return regX;
                case Register.y: return regY;
                case Register.z: return regZ;
                case long n: return n;
            };
            throw new Exception("cannothappen");
        }
        void writeRegister(Register reg, long value)
        {
            switch (reg) {
                case Register.w: regW = value; break;
                case Register.x: regX = value; break;
                case Register.y: regY = value; break;
                case Register.z: regZ = value; break;
            };
        }

        foreach (var instruction in instructions[atInstruction..])
        {
            atInstruction++;
            switch (instruction.op)
            {
                case Opcode.inp:
                    if (nextInput == null)
                    {
                        atInstruction--;
                    } else {
                        writeRegister(instruction.target, nextInput.Value);
                    }
                    return false;
                case Opcode.add:
                    var sum = readRegister(instruction.target) + readRegister(instruction.source);
                    writeRegister(instruction.target, sum);
                    break;
                case Opcode.mul:
                    var prod = readRegister(instruction.target) * readRegister(instruction.source);
                    writeRegister(instruction.target, prod);
                    break;
                case Opcode.div:
                    var div = readRegister(instruction.target) / readRegister(instruction.source);
                    writeRegister(instruction.target, div);
                    break;
                case Opcode.mod:
                    var mod = readRegister(instruction.target) % readRegister(instruction.source);
                    writeRegister(instruction.target, mod);
                    break;
                case Opcode.eql:
                    var a = readRegister(instruction.target);
                    var b = readRegister(instruction.source);
                    writeRegister(instruction.target, a == b ? 1 : 0);
                    break;
            }
        }
        return true;
    }

    public (long w, long x, long y, long z) getRegisters()
    {
        return (regW, regX, regY, regZ);
    }

}

class Program
{
    static void Main(string[] args)
    {
        var data = LoadData(args[0]);
        Stopwatch sw = Stopwatch.StartNew();
        var p1 = P1(data);
        Console.WriteLine($"P1: {string.Join("", p1)} in {sw.ElapsedMilliseconds} ms");

        data = LoadData(args[0]);
        sw = Stopwatch.StartNew();
        var p2 = P2(data);
        Console.WriteLine($"P2: {string.Join("", p2)} in {sw.ElapsedMilliseconds} ms");
    }

    static Instruction[] LoadData(string filepath)
    {
        return File.ReadAllLines(filepath)
            .AsEnumerable()
            .Select(line => new Instruction(line))
            .ToArray();
    }

    /*
        P1
    */

    public static long[]? runner(ALU alu, long[] known, ref HashSet<(long, long)> cache, bool p1)
    {
        var max = 14;
        var knownCount = known.Length;
        if (knownCount >= max) return null;

        alu = alu.CloneClean();
        alu.processInputs(known);
        //alu.processInput(null);
        var regsFrom = alu.getRegisters();

        var range = Enumerable.Range(1, 9);
        foreach (var i in p1 ? range.Reverse() : range)
        {
            // optimization for p1
            //if (knownCount == 12 && i != 9) continue;
            var alu2 = alu.Clone();
            alu2.processInput(i);
            alu2.processInput(null);
            var regs = alu2.getRegisters();

            if (cache.Contains((knownCount + 1, regs.z))) continue;
            cache.Add((knownCount + 1, regs.z));

            if (knownCount == max - 1 && regs.z == 0)
            {
                // WIN!
                return known.Append(i).ToArray();
            }

            var newKnown = known.Append(i).ToArray();
            var result = runner(alu.CloneClean(), newKnown, ref cache, p1);
            if (result != null) return result;
        }
        return null;
    }

    public static long[] P1(Instruction[] instructions)
    {
        var cache = new HashSet<(long, long)>();
        var alu = new ALU(instructions);
        var numbers = new long[]{};

        var largest = runner(alu, numbers, ref cache, true);
        Console.WriteLine($"largest {string.Join("", largest)}");

        return largest;
    }

    /*
        P2
    */

    public static long[] P2(Instruction[] instructions)
    {
        var cache = new HashSet<(long, long)>();
        var alu = new ALU(instructions);
        var numbers = new long[]{};

        var smallest = runner(alu, numbers, ref cache, false);
        Console.WriteLine($"largest {string.Join("", smallest)}");

        return smallest;
    }
}
