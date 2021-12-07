using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode;

public struct VentPoint : IEquatable<VentPoint>
{
    public int X;
    public int Y;


    public bool Equals(VentPoint other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is VentPoint other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(VentPoint left, VentPoint right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VentPoint left, VentPoint right)
    {
        return !(left == right);
    }
}

public struct VentLine
{
    public VentPoint Start;
    public VentPoint End;
}

public class Day5 : Day<VentLine>
{
    public Day5() : base(5, input =>
    {
        string[][] splitInput = input
            .Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(s =>
                s.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            ).ToArray();
        return new VentLine
        {
            Start = new VentPoint
            {
                X = int.Parse(splitInput[0][0]),
                Y = int.Parse(splitInput[0][1]),
            },
            End = new VentPoint
            {
                X = int.Parse(splitInput[1][0]),
                Y = int.Parse(splitInput[1][1]),
            },
        };
    }) { }

    private static void AddCoordinate(Dictionary<VentPoint, uint> coordinateCounts, int x, int y)
    {
        VentPoint key = new()
        {
            X = x,
            Y = y,
        };

        if (!coordinateCounts.TryGetValue(key, out uint value))
            value = 0;
        coordinateCounts[key] = value + 1;
    }

    public static void AddLineCoordinates(VentLine line, Dictionary<VentPoint, uint> coordinateCount)
    {
        int dx = Math.Abs(line.End.X - line.Start.X);
        int sx = line.Start.X < line.End.X ? 1 : -1;
        int dy = -Math.Abs(line.End.Y - line.Start.Y);
        int sy = line.Start.Y < line.End.Y ? 1 : -1;
        int err = dx + dy;
        int x0 = line.Start.X;
        int x1 = line.End.X;
        int y0 = line.Start.Y;
        int y1 = line.End.Y;
        while (true)
        {
            AddCoordinate(coordinateCount, x0, y0);
            if (x0 == x1 && y0 == y1) break;
                
            int e2 = 2 * err;
            if (e2 >= dy)
            {
                err += dy;
                x0 += sx;
            }

            if (e2 <= dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
    
    public override void Problem1()
    {
        Dictionary<VentPoint, uint> coordinateCount = new();
        foreach (VentLine line in this.Inputs.Where(line => line.Start.X == line.End.X || line.Start.Y == line.End.Y))
        {
            AddLineCoordinates(line, coordinateCount);
        }
            
        Console.WriteLine($"Final Count: {coordinateCount.Count(kvp => kvp.Value > 1)}");
    }

    public override void Problem2()
    {
        Dictionary<VentPoint, uint> coordinateCount = new();
        foreach (VentLine line in this.Inputs)
        {
            AddLineCoordinates(line, coordinateCount);
        }
            
        Console.WriteLine($"Final Count: {coordinateCount.Count(kvp => kvp.Value > 1)}");
    }
}