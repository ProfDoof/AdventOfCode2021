using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public enum Comparison
{
    GreaterThan,
    Within,
    LessThan,
}

public record struct Point(int X, int Y);

public readonly record struct InclusiveRange(int Start, int End)
{
    public bool Contains(int coordinate)
    {
        return coordinate >= this.Start && coordinate <= this.End;
    }

    public Comparison CompareTo(int coordinate)
    {
        if (coordinate < this.Start)
            return Comparison.LessThan;
        
        if (coordinate > this.End)
            return Comparison.GreaterThan;

        return Comparison.Within;
    }

    public override string ToString()
    {
        return $"{this.Start}..{this.End}";
    }
}

public readonly record struct TargetArea(InclusiveRange XRange, InclusiveRange YRange)
{
    public bool Contains(Point point)
    {
        return this.XRange.Contains(point.X) && this.YRange.Contains(point.Y);
    }

    public (Comparison xComparison, Comparison yComparison) CompareTo(Point point)
    {
        return (this.XRange.CompareTo(point.X), this.YRange.CompareTo(point.Y));
    }

    public override string ToString()
    {
        return $"X: {this.XRange}, Y: {this.YRange}";
    }
}

public class Day17Solver : AdventOfCodeSolver<TargetArea>
{
    public Day17Solver() : base(17)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        List<InclusiveRange> ranges = (await inputReader.ReadToEndAsync())
            .Trim()["target area: ".Length..]
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(coord =>
            {
                string[] range = coord.Split('=')[1].Split("..");
                return new InclusiveRange(int.Parse(range[0]), int.Parse(range[1]));
            }).ToList();
        this.Input = new TargetArea(ranges[0], ranges[1]);
    }

    public override Task SolveProblemOneAsync()
    {
        Console.WriteLine(this.Input);
        int yhat0 = -1 - this.Input.YRange.Start;
        Console.WriteLine($"yhat_0 = {yhat0}");
        Console.WriteLine($"Max distance = {yhat0*(yhat0+1)/2}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        Console.WriteLine(this.Input);
        HashSet<int> distinct = new();
        decimal lowerBoundY;
        decimal upperBoundY;
        decimal numTicks = 1;
        decimal xTickLimit = Math.Ceiling((-1 + (decimal) Math.Sqrt(1 + 8 * this.Input.XRange.Start)) / 2);
        do
        {
            decimal lowerBoundX;
            decimal upperBoundX;
            if (numTicks < xTickLimit)
            {
                lowerBoundX = Math.Ceiling(this.Input.XRange.Start / numTicks +
                             (numTicks - 1) / 2);
                upperBoundX = Math.Floor(this.Input.XRange.End / numTicks +
                             (numTicks - 1) / 2);
                
            }
            else
            {
                lowerBoundX = Math.Ceiling((-1 + (decimal) Math.Sqrt(1 + 8 * this.Input.XRange.Start)) / 2);
                upperBoundX = Math.Floor((-1 + (decimal) Math.Sqrt(1 + 8 * this.Input.XRange.End)) / 2);
            }

            lowerBoundY = Math.Ceiling(this.Input.YRange.Start / numTicks +
                          (numTicks - 1) / 2);
            upperBoundY = Math.Floor(this.Input.YRange.End / numTicks +
                          (numTicks - 1) / 2);

            for (decimal i = lowerBoundX; i <= upperBoundX; i += 1)
            {
                for (decimal j = lowerBoundY; j <= upperBoundY; j += 1)
                {
                    Console.WriteLine($"{i}, {j}");
                    distinct.Add(HashCode.Combine(i, j));
                }
            }
            numTicks += 1;
        } while (Math.Abs(upperBoundY) < Math.Abs(this.Input.YRange.Start));
        
        Console.WriteLine($"Total number of possible initial starting velocities: {distinct.Count}");
        return Task.CompletedTask;
    }
}