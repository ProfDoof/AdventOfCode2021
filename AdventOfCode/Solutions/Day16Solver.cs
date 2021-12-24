using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public enum BitsPacketType
{
    Sum = 0,
    Product = 1,
    Minimum = 2,
    Maximum = 3,
    Literal = 4,
    GreaterThan = 5,
    LessThan = 6,
    EqualTo = 7,
    Other,
}

public abstract class BitsPacket
{
    public ulong Version { get; }
    public BitsPacketType Type { get; }
    public ulong Length { get; set; }

    public BitsPacket(ulong version, BitsPacketType type)
    {
        this.Version = version;
        this.Type = type;
        this.Length = 6;
    }

    public override string ToString()
    {
        return $"Version: {this.Version}\nPacket Type: {this.Type}";
    }
    
    protected static ulong BitToUlong(BitArray bits, int index) => bits[index] ? 1UL : 0UL;

    public abstract decimal Value { get; }

    public static (ulong version, ulong type, int newIndex) ParseVersionAndType(BitArray bits, int index)
    {
        ulong version = (BitToUlong(bits, index) << 2) + (BitToUlong(bits, index + 1) << 1) + BitToUlong(bits, index + 2);
        ulong type = (BitToUlong(bits, index + 3) << 2) + (BitToUlong(bits, index + 4) << 1) + BitToUlong(bits, index + 5);
        return (version, type, index + 6);
    }

    public static (BitsPacket packet, int newIndex) Parse(BitArray bits, int index)
    {
        throw new NotImplementedException();
    }
}

public class BitsLiteral : BitsPacket
{
    public override decimal Value { get; }

    public BitsLiteral(ulong version, ulong value) : base(version, BitsPacketType.Literal)
    {
        this.Value = value;
    }

    public override string ToString()
    {
        return $"{base.ToString()}\nValue: {this.Value}";
    }

    public static (BitsPacket packet, int newIndex) Parse(BitArray bits, int index, ulong version)
    {
        int original = index;
        ulong value = 0;
        while (bits[index])
        {
            index += 1;
            value = (value << 4) + (BitToUlong(bits, index) << 3) +
                    (BitToUlong(bits, index + 1) << 2) + (BitToUlong(bits, index + 2) << 1)
                    + BitToUlong(bits, index + 3);
            index += 4;
        }
                
        index += 1;
        value = (value << 4) + (BitToUlong(bits, index) << 3) +
                (BitToUlong(bits, index + 1) << 2) + (BitToUlong(bits, index + 2) << 1)
                + BitToUlong(bits, index + 3);
        index += 4;
        BitsLiteral literal = new(version, value)
        {
            Length = (ulong)(6 + index - original),
        };
        return (literal, index);
    }
}

public enum BitsOperatorLengthType
{
    BitLength = 0,
    SubpacketLength = 1,
}

public class BitsOperator : BitsPacket
{
    public BitsOperatorLengthType LengthType { get; }
    public List<BitsPacket> Subpackets { get; }

    public BitsOperator(ulong version, BitsPacketType type, BitsOperatorLengthType operatorLengthType) : base(version, type)
    {
        this.LengthType = operatorLengthType;
        Subpackets = new List<BitsPacket>();
    }

    public override decimal Value => this.Type switch
    {
        BitsPacketType.Sum => this.Subpackets.Sum(bp => bp.Value),
        BitsPacketType.Product => this.Subpackets.Aggregate((decimal)1, (initial, bp) => initial * bp.Value),
        BitsPacketType.Minimum => this.Subpackets.Min(bp => bp.Value),
        BitsPacketType.Maximum => this.Subpackets.Max(bp => bp.Value),
        BitsPacketType.GreaterThan => this.Subpackets[0].Value > this.Subpackets[1].Value ? 1 : 0,
        BitsPacketType.LessThan => this.Subpackets[0].Value < this.Subpackets[1].Value ? 1 : 0,
        BitsPacketType.EqualTo => this.Subpackets[0].Value == this.Subpackets[1].Value ? 1 : 0,
        _ => throw new ArgumentOutOfRangeException("Type", $"{this.Type} is not a valid BITS packet operator type"),
    };

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(base.ToString()).AppendLine().Append("Subpackets =>\n=============\n");
        foreach (BitsPacket bitsPacket in Subpackets)
        {
            builder.Append(bitsPacket).Append("\n=============\n");
        }

        return builder.ToString();
    }

    public static (BitsPacket packet, int newIndex) Parse(BitArray bits, int index, ulong version, ulong opType)
    {
        ulong lengthType = BitToUlong(bits, index);
        BitsOperator op = new(version, (BitsPacketType) opType, (BitsOperatorLengthType) lengthType);
        index += 1;

        ulong lengthBits;
        if (op.LengthType is BitsOperatorLengthType.BitLength)
        {
            lengthBits = 15;
            ulong bitlength = 0;
            int maxIndex = index + (int)lengthBits;
            for (; index < maxIndex; index += 1)
            {
                bitlength = (bitlength << 1) + BitToUlong(bits, index);
            }

            while (bitlength > 0)
            {
                (BitsPacket packet, index) = ParseSubpacket(bits, index);
                op.Subpackets.Add(packet);
                bitlength -= packet.Length;
            }
        }
        else if (op.LengthType is BitsOperatorLengthType.SubpacketLength)
        {
            lengthBits = 11;
            ulong numSubpackets = 0;
            int maxIndex = index + (int)lengthBits;
            for (; index < maxIndex; index += 1)
            {
                numSubpackets = (numSubpackets << 1) + BitToUlong(bits, index);
            }

            for (ulong i = 0; i < numSubpackets; i += 1)
            {
                (BitsPacket packet, index) = ParseSubpacket(bits, index);
                op.Subpackets.Add(packet);
            }
        }
        else
        {
            throw new Exception("Something went horribly horribly wrong");
        }

        op.Length = 7 + lengthBits + (ulong)op.Subpackets.Sum(s => (decimal)s.Length);
        return (op, index);
    }

    private static (BitsPacket, int) ParseSubpacket(BitArray bits, int index)
    {
        (ulong subversion, ulong subtype, index) = ParseVersionAndType(bits, index);
        BitsPacket temp;
        if (subtype == 4)
        {
            (temp, index) = BitsLiteral.Parse(bits, index, subversion);
        }
        else
        {
            (temp, index) = BitsOperator.Parse(bits, index, subversion, subtype);
        }
        
        return (temp, index);
    }
}

public struct Day16Input
{
    public BitsPacket BitsPacket;
}

public class Day16Solver : AdventOfCodeSolver<Day16Input>
{
    public Day16Solver() : base(16)
    {
    }

    private static byte[] _reverseBitOrderTable = new[]
    {
        (byte)0b00000000,
        (byte)0b10000000,
        (byte)0b01000000,
        (byte)0b11000000,
        (byte)0b00100000,
        (byte)0b10100000,
        (byte)0b01100000,
        (byte)0b11100000,
        (byte)0b00010000,
        (byte)0b10010000,
        (byte)0b01010000,
        (byte)0b11010000,
        (byte)0b00110000,
        (byte)0b10110000,
        (byte)0b01110000,
        (byte)0b11110000,
        (byte)0b00001000,
        (byte)0b10001000,
        (byte)0b01001000,
        (byte)0b11001000,
        (byte)0b00101000,
        (byte)0b10101000,
        (byte)0b01101000,
        (byte)0b11101000,
        (byte)0b00011000,
        (byte)0b10011000,
        (byte)0b01011000,
        (byte)0b11011000,
        (byte)0b00111000,
        (byte)0b10111000,
        (byte)0b01111000,
        (byte)0b11111000,
        (byte)0b00000100,
        (byte)0b10000100,
        (byte)0b01000100,
        (byte)0b11000100,
        (byte)0b00100100,
        (byte)0b10100100,
        (byte)0b01100100,
        (byte)0b11100100,
        (byte)0b00010100,
        (byte)0b10010100,
        (byte)0b01010100,
        (byte)0b11010100,
        (byte)0b00110100,
        (byte)0b10110100,
        (byte)0b01110100,
        (byte)0b11110100,
        (byte)0b00001100,
        (byte)0b10001100,
        (byte)0b01001100,
        (byte)0b11001100,
        (byte)0b00101100,
        (byte)0b10101100,
        (byte)0b01101100,
        (byte)0b11101100,
        (byte)0b00011100,
        (byte)0b10011100,
        (byte)0b01011100,
        (byte)0b11011100,
        (byte)0b00111100,
        (byte)0b10111100,
        (byte)0b01111100,
        (byte)0b11111100,
        (byte)0b00000010,
        (byte)0b10000010,
        (byte)0b01000010,
        (byte)0b11000010,
        (byte)0b00100010,
        (byte)0b10100010,
        (byte)0b01100010,
        (byte)0b11100010,
        (byte)0b00010010,
        (byte)0b10010010,
        (byte)0b01010010,
        (byte)0b11010010,
        (byte)0b00110010,
        (byte)0b10110010,
        (byte)0b01110010,
        (byte)0b11110010,
        (byte)0b00001010,
        (byte)0b10001010,
        (byte)0b01001010,
        (byte)0b11001010,
        (byte)0b00101010,
        (byte)0b10101010,
        (byte)0b01101010,
        (byte)0b11101010,
        (byte)0b00011010,
        (byte)0b10011010,
        (byte)0b01011010,
        (byte)0b11011010,
        (byte)0b00111010,
        (byte)0b10111010,
        (byte)0b01111010,
        (byte)0b11111010,
        (byte)0b00000110,
        (byte)0b10000110,
        (byte)0b01000110,
        (byte)0b11000110,
        (byte)0b00100110,
        (byte)0b10100110,
        (byte)0b01100110,
        (byte)0b11100110,
        (byte)0b00010110,
        (byte)0b10010110,
        (byte)0b01010110,
        (byte)0b11010110,
        (byte)0b00110110,
        (byte)0b10110110,
        (byte)0b01110110,
        (byte)0b11110110,
        (byte)0b00001110,
        (byte)0b10001110,
        (byte)0b01001110,
        (byte)0b11001110,
        (byte)0b00101110,
        (byte)0b10101110,
        (byte)0b01101110,
        (byte)0b11101110,
        (byte)0b00011110,
        (byte)0b10011110,
        (byte)0b01011110,
        (byte)0b11011110,
        (byte)0b00111110,
        (byte)0b10111110,
        (byte)0b01111110,
        (byte)0b11111110,
        (byte)0b00000001,
        (byte)0b10000001,
        (byte)0b01000001,
        (byte)0b11000001,
        (byte)0b00100001,
        (byte)0b10100001,
        (byte)0b01100001,
        (byte)0b11100001,
        (byte)0b00010001,
        (byte)0b10010001,
        (byte)0b01010001,
        (byte)0b11010001,
        (byte)0b00110001,
        (byte)0b10110001,
        (byte)0b01110001,
        (byte)0b11110001,
        (byte)0b00001001,
        (byte)0b10001001,
        (byte)0b01001001,
        (byte)0b11001001,
        (byte)0b00101001,
        (byte)0b10101001,
        (byte)0b01101001,
        (byte)0b11101001,
        (byte)0b00011001,
        (byte)0b10011001,
        (byte)0b01011001,
        (byte)0b11011001,
        (byte)0b00111001,
        (byte)0b10111001,
        (byte)0b01111001,
        (byte)0b11111001,
        (byte)0b00000101,
        (byte)0b10000101,
        (byte)0b01000101,
        (byte)0b11000101,
        (byte)0b00100101,
        (byte)0b10100101,
        (byte)0b01100101,
        (byte)0b11100101,
        (byte)0b00010101,
        (byte)0b10010101,
        (byte)0b01010101,
        (byte)0b11010101,
        (byte)0b00110101,
        (byte)0b10110101,
        (byte)0b01110101,
        (byte)0b11110101,
        (byte)0b00001101,
        (byte)0b10001101,
        (byte)0b01001101,
        (byte)0b11001101,
        (byte)0b00101101,
        (byte)0b10101101,
        (byte)0b01101101,
        (byte)0b11101101,
        (byte)0b00011101,
        (byte)0b10011101,
        (byte)0b01011101,
        (byte)0b11011101,
        (byte)0b00111101,
        (byte)0b10111101,
        (byte)0b01111101,
        (byte)0b11111101,
        (byte)0b00000011,
        (byte)0b10000011,
        (byte)0b01000011,
        (byte)0b11000011,
        (byte)0b00100011,
        (byte)0b10100011,
        (byte)0b01100011,
        (byte)0b11100011,
        (byte)0b00010011,
        (byte)0b10010011,
        (byte)0b01010011,
        (byte)0b11010011,
        (byte)0b00110011,
        (byte)0b10110011,
        (byte)0b01110011,
        (byte)0b11110011,
        (byte)0b00001011,
        (byte)0b10001011,
        (byte)0b01001011,
        (byte)0b11001011,
        (byte)0b00101011,
        (byte)0b10101011,
        (byte)0b01101011,
        (byte)0b11101011,
        (byte)0b00011011,
        (byte)0b10011011,
        (byte)0b01011011,
        (byte)0b11011011,
        (byte)0b00111011,
        (byte)0b10111011,
        (byte)0b01111011,
        (byte)0b11111011,
        (byte)0b00000111,
        (byte)0b10000111,
        (byte)0b01000111,
        (byte)0b11000111,
        (byte)0b00100111,
        (byte)0b10100111,
        (byte)0b01100111,
        (byte)0b11100111,
        (byte)0b00010111,
        (byte)0b10010111,
        (byte)0b01010111,
        (byte)0b11010111,
        (byte)0b00110111,
        (byte)0b10110111,
        (byte)0b01110111,
        (byte)0b11110111,
        (byte)0b00001111,
        (byte)0b10001111,
        (byte)0b01001111,
        (byte)0b11001111,
        (byte)0b00101111,
        (byte)0b10101111,
        (byte)0b01101111,
        (byte)0b11101111,
        (byte)0b00011111,
        (byte)0b10011111,
        (byte)0b01011111,
        (byte)0b11011111,
        (byte)0b00111111,
        (byte)0b10111111,
        (byte)0b01111111,
        (byte)0b11111111,
    };
    private static byte ReverseBitOrder(byte b)
    {
        return _reverseBitOrderTable[b];
    }

    private static (byte[], bool ignoreFirstHalfByte) HexStringToByteArray(string hexstring)
    {
        int length = hexstring.Length >> 1;
        byte[] byteArray = new byte[length];
        int stringBackIndex = 1;
        for (int byteIndex = length-1; byteIndex > 0; byteIndex -= 1)
        {
            byte nextByte = (byte)((GetHexVal(hexstring[^(stringBackIndex + 1)]) << 4) + GetHexVal(hexstring[^stringBackIndex]));
            byteArray[byteIndex] = nextByte;
            stringBackIndex += 2;
        }
        
        if (hexstring.Length % 2 == 1)
        {
            byteArray[0] = (byte) GetHexVal(hexstring[0]);
        }
        else
        {
            byteArray[0] = (byte) ((GetHexVal(hexstring[0]) << 4) + GetHexVal(hexstring[1]));
        }
        return (byteArray, hexstring.Length % 2 == 1);
    }

    private static ulong GetHexVal(char hexChar) =>
        hexChar switch
        {
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '3' => 3,
            '4' => 4,
            '5' => 5,
            '6' => 6,
            '7' => 7,
            '8' => 8,
            '9' => 9,
            'A' or 'a' => 10,
            'B' or 'b' => 11,
            'C' or 'c' => 12,
            'D' or 'd' => 13,
            'E' or 'e' => 14,
            'F' or 'f' => 15,
            _ => throw new ArgumentException($"Invalid hex character {hexChar}"),
        };

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        (byte[] bytes, bool ignoreFirstHalfByte) = HexStringToByteArray((await inputReader.ReadToEndAsync()).Trim());
        BitArray bits = new(bytes.Select(ReverseBitOrder).ToArray());
        int index = ignoreFirstHalfByte ? 4 : 0;

        (ulong version, ulong type, index) = BitsPacket.ParseVersionAndType(bits, index);
        BitsPacket currentPacket;
        if (type == 4)
        {
            (currentPacket, _) = BitsLiteral.Parse(bits, index, version);
        }
        else
        {
            (currentPacket, _) = BitsOperator.Parse(bits, index, version, type);
        }

        this.Input.BitsPacket = currentPacket;
        
        Console.WriteLine(currentPacket);
    }

    public override Task SolveProblemOneAsync()
    {
        Stack<BitsPacket> children = new();
        children.Push(this.Input.BitsPacket);
        ulong versionSum = 0;
        while (children.Any())
        {
            BitsPacket current = children.Pop();
            versionSum += current.Version;
            if (current is BitsOperator op)
            {
                foreach (BitsPacket subpacket in op.Subpackets)
                {
                    children.Push(subpacket);
                }
            }
        }
        
        Console.WriteLine($"Version Sum: {versionSum}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        Console.WriteLine($"Value of outer packet: {this.Input.BitsPacket.Value}");
        return Task.CompletedTask;
    }
}