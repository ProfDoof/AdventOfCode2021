using System;
using System.IO;

namespace AdventOfCode;

public interface IDay
{
    public int DayOfMonth { get; }
    public void Problem1();
    public void Problem2();
}

public abstract class Day<TInput> : IDay
{
    protected Day(int dayOfMonth, Func<string, TInput> parseFunc)
    {
        this.DayOfMonth = dayOfMonth;
        using FileStream inputFileStream =
            new($"./Inputs/day{this.DayOfMonth}input.txt", FileMode.Open, FileAccess.Read);
        using StreamReader inputFileReader = new(inputFileStream);
        string contents = inputFileReader.ReadToEnd();
        string[] allInputs = contents.Trim().Split('\n');
        this.Inputs = new TInput[allInputs.Length];
        int count = 0;
        foreach (string input in allInputs)
        {
            this.Inputs[count] = parseFunc(input);
            count++;
        }
    }
    
    
    protected readonly TInput[] Inputs;
    public int DayOfMonth { get; }
    public abstract void Problem1();
    public abstract void Problem2();
}

