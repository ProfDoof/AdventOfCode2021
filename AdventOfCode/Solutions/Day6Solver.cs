using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public struct Day6Input
{
    public List<ulong> LanternfishAges;
}

public class Day6Solver : AdventOfCodeSolver<Day6Input>
{
    public Day6Solver() : base(6)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        string contents = await inputReader.ReadToEndAsync();
        this.Input = new Day6Input
        {
            LanternfishAges = contents
                .Trim()
                .Split(
                    ',',
                    StringSplitOptions.TrimEntries |
                    StringSplitOptions.RemoveEmptyEntries
                )
                .Select(ulong.Parse)
                .ToList(),
        };
    }
    
    private void SimulateLanternfishBreeding(int maximum)
    {
        int current = 1;
        ulong[] countsOfFish = new ulong[9];
        foreach (ulong input in this.Input.LanternfishAges)
        {
            countsOfFish[input] += 1;
        }
        while (current <= maximum)
        {
            ulong newFish = countsOfFish[0];
            for (int i = 0; i + 1 < countsOfFish.Length; i++)
            {
                countsOfFish[i] = countsOfFish[i + 1];
            }
            
            countsOfFish[^1] = newFish;
            countsOfFish[^3] += newFish;
            current += 1;
        }

        ulong total = countsOfFish.Aggregate(0UL, (initial, accumulate) => initial + accumulate);
        Console.WriteLine($"Final count of Lanternfish: {total}");
    }

    public override Task SolveProblemOneAsync()
    {
        SimulateLanternfishBreeding(80);
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        SimulateLanternfishBreeding(256);
        return Task.CompletedTask;
    }
}