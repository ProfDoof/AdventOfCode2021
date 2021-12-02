using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AdventOfCode;

public class Day1 : IDay
{
    public int Day => 1;
    
    private readonly int[] _inputs;
    private Day1(int[] inputs)
    {
        this._inputs = inputs;
    }

    public static async Task<Day1> InitializeAsync()
    {
        await using FileStream inputFileStream = new("./Inputs/day1input.txt", FileMode.Open);
        using StreamReader inputFileReader = new(inputFileStream);
        string input = await inputFileReader.ReadToEndAsync();
        IEnumerable<string> allInputs = input.Trim().Split();
        int[] inputs = new int[allInputs.Count()];
        int count = 0;
        foreach (string str in allInputs)
        {
            inputs[count] = int.Parse(str);
            count++;
        }
        
        return new Day1(inputs);
    }

    public void Problem1()
    {
        int total = 0;
        for (int i = 0; i+1 < this._inputs.Length; i++)
        {
            total += unchecked((int) ((uint) (this._inputs[i] - this._inputs[i + 1]) >> 31));
        }
        Console.WriteLine($"The number of increasing values is {total}");
    }

    public void Problem2()
    {
        int total = 0;
        int last = this._inputs[0] + this._inputs[1] + this._inputs[2];
        for (int i = 1; i+2 < this._inputs.Length; i++)
        {
            int current = this._inputs[i] + this._inputs[i + 1] + this._inputs[i + 2];
            total += unchecked((int) ((uint) (last - current) >> 31));
            last = current;
        }
        
        Console.WriteLine($"The number of increasing values is {total}");
    }
}