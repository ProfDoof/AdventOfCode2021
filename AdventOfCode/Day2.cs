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

public class Day2 : IDay
{
    public int Day => 2;

    private SubmarineCommand[] inputs;

    private Day2(SubmarineCommand[] inputs)
    {
        this.inputs = inputs;
    }
    
    public static async Task<Day2> InitializeAsync()
    {
        await using FileStream inputFileStream = new("./Inputs/day2input.txt", FileMode.Open);
        using StreamReader inputFileReader = new(inputFileStream);
        string input = await inputFileReader.ReadToEndAsync();
        string[] allInputs = input.Trim().Split('\n');
        SubmarineCommand[] inputs = new SubmarineCommand[allInputs.Count()];
        int count = 0;
        foreach (string str in allInputs)
        {
            string[] command = str.Trim().Split();
            inputs[count] = new SubmarineCommand
            {
                Direction = Enum.Parse<Direction>(command[0], true),
                Amount = uint.Parse(command[1]),
            };
            count++;
        }
        
        return new Day2(inputs);
    }
    public void Problem1()
    {
        SubmarinePosition currentPosition = new SubmarinePosition
        {
            Depth = 0,
            HorizontalPosition = 0,
        };
        for (int i = 0; i < inputs.Length; i++)
        {
            switch (inputs[i].Direction)
            {
                case Direction.Forward:
                    currentPosition.HorizontalPosition += (int)inputs[i].Amount;
                    break;
                case Direction.Down:
                    currentPosition.Depth += (int) inputs[i].Amount;
                    break;
                case Direction.Up:
                    currentPosition.Depth -= (int) inputs[i].Amount;
                    break;
                default:
                    throw new Exception("Something done went wrong");
            }
        }
        
        Console.WriteLine(currentPosition.ToString());
        Console.WriteLine($"Final Answer: {currentPosition.Depth * currentPosition.HorizontalPosition}");
    }

    public void Problem2()
    {
        SubmarinePosition currentPosition = new SubmarinePosition
        {
            Depth = 0,
            HorizontalPosition = 0,
        };
        for (int i = 0; i < inputs.Length; i++)
        {
            switch (inputs[i].Direction)
            {
                case Direction.Forward:
                    currentPosition.HorizontalPosition += (int)inputs[i].Amount;
                    currentPosition.Depth += currentPosition.Aim * (int) inputs[i].Amount;
                    break;
                case Direction.Down:
                    currentPosition.Aim += (int) inputs[i].Amount;
                    break;
                case Direction.Up:
                    currentPosition.Aim -= (int) inputs[i].Amount;
                    break;
                default:
                    throw new Exception("Something done went wrong");
            }
        }
        
        Console.WriteLine(currentPosition.ToString());
        Console.WriteLine($"Final Answer: {currentPosition.Depth * currentPosition.HorizontalPosition}");
    }
}