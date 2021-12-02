using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AdventOfCode;

public class Day1 : Day<int>
{
    public override Task<Day<int>> InitializeAsync()
    {
        return this.InitializeAsync(int.Parse);
    }

    public override int DayOfMonth => 1;

    public override void Problem1()
    {
        int total = 0;
        for (int i = 0; i+1 < this.Inputs.Length; i++)
        {
            total += unchecked((int) ((uint) (this.Inputs[i] - this.Inputs[i + 1]) >> 31));
        }
        Console.WriteLine($"The number of increasing values is {total}");
    }

    public override void Problem2()
    {
        int total = 0;
        int last = this.Inputs[0] + this.Inputs[1] + this.Inputs[2];
        for (int i = 1; i+2 < this.Inputs.Length; i++)
        {
            int current = this.Inputs[i] + this.Inputs[i + 1] + this.Inputs[i + 2];
            total += unchecked((int) ((uint) (last - current) >> 31));
            last = current;
        }
        
        Console.WriteLine($"The number of increasing values is {total}");
    }
}