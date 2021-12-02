using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode;

public interface IDay
{
    public int DayOfMonth { get; }
    public void Problem1();
    public void Problem2();
}

public abstract class Day<TInputType> : IDay
{
    public abstract Task<Day<TInputType>> InitializeAsync();
    
    [MemberNotNull(nameof(Inputs))]
    protected async Task<Day<TInputType>> InitializeAsync(Func<string, TInputType> parseFunc)
    {
        await using FileStream inputFileStream =
            new($"./Inputs/day{this.DayOfMonth}input.txt", FileMode.Open, FileAccess.Read);
        using StreamReader inputFileReader = new(inputFileStream);
        string contents = await inputFileReader.ReadToEndAsync();
        string[] allInputs = contents.Trim().Split('\n');
        this.Inputs = new TInputType[allInputs.Length];
        int count = 0;
        foreach (string input in allInputs)
        {
            this.Inputs[count] = parseFunc(input);
            count++;
        }

        return this;
    }
    
    
    protected TInputType[]? Inputs;
    public abstract int DayOfMonth { get; }
    public abstract void Problem1();
    public abstract void Problem2();
}