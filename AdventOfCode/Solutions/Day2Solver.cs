using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public struct SubmarineCommand
{
    public Direction Direction;
    public int Amount;
}

public enum Direction
{
    Forward,
    Down,
    Up
}

public struct SubmarinePosition
{
    public int Depth;
    public int HorizontalPosition;
    public int Aim;

    public new string ToString()
    {
        return $"Current Depth: {Depth}\nCurrent Horizontal Position: {HorizontalPosition}\n";
    }
}

public struct Day2Input
{
    public List<SubmarineCommand> InputCommands;
}

public class Day2Solver : AdventOfCodeSolver<Day2Input>
{
    public Day2Solver() : base(2)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        this.Input = new Day2Input
        {
            InputCommands = await AdventOfCodeSolverHelper.ParseEachLineAsync(
                inputReader,
                input =>
                {
                    string[] command = input.Trim().Split();
                    return new SubmarineCommand
                    {
                        Direction = Enum.Parse<Direction>(command[0], true),
                        Amount = int.Parse(command[1]),
                    };
                }),
        };
    }

    public override Task SolveProblemOneAsync()
    {
        SubmarinePosition currentPosition = new()
        {
            Depth = 0,
            HorizontalPosition = 0,
        };
        for (int i = 0; i < this.Input.InputCommands.Count; i++)
        {
            switch (this.Input.InputCommands[i].Direction)
            {
                case Direction.Forward:
                    currentPosition.HorizontalPosition += this.Input.InputCommands[i].Amount;
                    break;
                case Direction.Down:
                    currentPosition.Depth += this.Input.InputCommands[i].Amount;
                    break;
                case Direction.Up:
                    currentPosition.Depth -= this.Input.InputCommands[i].Amount;
                    break;
                default:
                    throw new Exception("Something done went wrong");
            }
        }
        
        Console.WriteLine(currentPosition.ToString());
        Console.WriteLine($"Final Answer: {currentPosition.Depth * currentPosition.HorizontalPosition}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        SubmarinePosition currentPosition = new()
        {
            Depth = 0,
            HorizontalPosition = 0,
        };
        for (int i = 0; i < this.Input.InputCommands.Count; i++)
        {
            switch (this.Input.InputCommands[i].Direction)
            {
                case Direction.Forward:
                    currentPosition.HorizontalPosition += this.Input.InputCommands[i].Amount;
                    currentPosition.Depth += currentPosition.Aim * this.Input.InputCommands[i].Amount;
                    break;
                case Direction.Down:
                    currentPosition.Aim += this.Input.InputCommands[i].Amount;
                    break;
                case Direction.Up:
                    currentPosition.Aim -= this.Input.InputCommands[i].Amount;
                    break;
                default:
                    throw new Exception("Something done went wrong");
            }
        }
        
        Console.WriteLine(currentPosition.ToString());
        Console.WriteLine($"Final Answer: {currentPosition.Depth * currentPosition.HorizontalPosition}");
        return Task.CompletedTask;
    }
}