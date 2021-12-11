using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public struct Day1Input
{
    public List<int> Inputs;
}

public class Day1Solver : AdventOfCodeSolver<Day1Input>
{
    public Day1Solver() : base(1)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        this.Input = new Day1Input
        {
            Inputs = await AdventOfCodeSolverHelper.ParseEachLineAsync(inputReader, int.Parse),
        };
    }

    public override Task SolveProblemOneAsync()
    {
        int total = 0;
        for (int i = 0; i+1 < this.Input.Inputs.Count; i++)
        {
            total += unchecked((int) ((uint) (this.Input.Inputs[i] - this.Input.Inputs[i + 1]) >> 31));
        }
        Console.WriteLine($"The number of increasing values is {total}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        int total = 0;
        int last = this.Input.Inputs[0] + this.Input.Inputs[1] + this.Input.Inputs[2];
        for (int i = 1; i+2 < this.Input.Inputs.Count; i++)
        {
            int current = this.Input.Inputs[i] + this.Input.Inputs[i + 1] + this.Input.Inputs[i + 2];
            total += unchecked((int) ((uint) (last - current) >> 31));
            last = current;
        }
        
        Console.WriteLine($"The number of increasing values is {total}");
        return Task.CompletedTask;
    }
}