using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode;

public struct SubmarineCommand
{
    public Direction Direction;
    public uint Amount;
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

public class Day2 : Day<SubmarineCommand>
{
    public override int DayOfMonth => 2;

    public override Task<Day<SubmarineCommand>> InitializeAsync()
    {
        return this.InitializeAsync(input =>
        {
            string[] command = input.Trim().Split();
            return new SubmarineCommand
            {
                Direction = Enum.Parse<Direction>(command[0], true),
                Amount = uint.Parse(command[1]),
            };
        });
    }
    public override void Problem1()
    {
        SubmarinePosition currentPosition = new SubmarinePosition
        {
            Depth = 0,
            HorizontalPosition = 0,
        };
        for (int i = 0; i < Inputs.Length; i++)
        {
            switch (Inputs[i].Direction)
            {
                case Direction.Forward:
                    currentPosition.HorizontalPosition += (int)Inputs[i].Amount;
                    break;
                case Direction.Down:
                    currentPosition.Depth += (int) Inputs[i].Amount;
                    break;
                case Direction.Up:
                    currentPosition.Depth -= (int) Inputs[i].Amount;
                    break;
                default:
                    throw new Exception("Something done went wrong");
            }
        }
        
        Console.WriteLine(currentPosition.ToString());
        Console.WriteLine($"Final Answer: {currentPosition.Depth * currentPosition.HorizontalPosition}");
    }

    public override void Problem2()
    {
        SubmarinePosition currentPosition = new SubmarinePosition
        {
            Depth = 0,
            HorizontalPosition = 0,
        };
        for (int i = 0; i < Inputs.Length; i++)
        {
            switch (Inputs[i].Direction)
            {
                case Direction.Forward:
                    currentPosition.HorizontalPosition += (int)Inputs[i].Amount;
                    currentPosition.Depth += currentPosition.Aim * (int) Inputs[i].Amount;
                    break;
                case Direction.Down:
                    currentPosition.Aim += (int) Inputs[i].Amount;
                    break;
                case Direction.Up:
                    currentPosition.Aim -= (int) Inputs[i].Amount;
                    break;
                default:
                    throw new Exception("Something done went wrong");
            }
        }
        
        Console.WriteLine(currentPosition.ToString());
        Console.WriteLine($"Final Answer: {currentPosition.Depth * currentPosition.HorizontalPosition}");
    }
}