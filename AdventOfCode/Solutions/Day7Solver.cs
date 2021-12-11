using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public struct Day7Input
{
    public List<int> CrabPositions;
}

public class Day7Solver : AdventOfCodeSolver<Day7Input>
{
    public Day7Solver() : base(7)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        string contents = await inputReader.ReadToEndAsync();
        this.Input = new Day7Input
        {
            CrabPositions = contents
                .Trim()
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList(),
        };
    }

    private int CalculateMinimumDistance(Func<int, Func<int, int>> distanceFunction)
    {
        int min = int.MaxValue;
        for (int i = this.Input.CrabPositions.Min(); i <= this.Input.CrabPositions.Max(); i++)
        {
            int distance = this.Input.CrabPositions.Sum(distanceFunction(i));
            if (distance < min)
            {
                min = distance;
            }
        }

        return min;
    }
    

    public override Task SolveProblemOneAsync()
    {
        Console.WriteLine($"Minimum Fuel: {CalculateMinimumDistance(test => current => Math.Abs(current - test))}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        int minimumFuel = CalculateMinimumDistance(test => current =>
        {
            int distance = Math.Abs(current - test);
            return distance * (distance + 1) / 2;
        });
        Console.WriteLine($"Minimum Fuel: {minimumFuel}");
        return Task.CompletedTask;
    }
}