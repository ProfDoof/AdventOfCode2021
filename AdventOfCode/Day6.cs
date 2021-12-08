using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AdventOfCode;

public class Day6 : IDay
{
    public int DayOfMonth => 6;
    private ulong[] Inputs;

    public Day6()
    {
        using FileStream inputFileStream =
            new($"./Inputs/day{this.DayOfMonth}input.txt", FileMode.Open, FileAccess.Read);
        using StreamReader inputFileReader = new(inputFileStream);
        string contents = inputFileReader.ReadToEnd();
        string[] allInputs = contents.Trim().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        this.Inputs = new ulong[allInputs.Length];
        int count = 0;
        foreach (string input in allInputs)
        {
            this.Inputs[count] = ulong.Parse(input);
            count++;
        }
    }

    private void SimulateLanternfishBreeding(int maximum)
    {
        int current = 1;
        BigInteger[] countsForOldFish = new BigInteger[7];
        foreach (ulong input in this.Inputs)
        {
            countsForOldFish[input] += 1;
        }
        BigInteger[] countsForNewFish = new BigInteger[9];
        while (current <= maximum)
        {
            BigInteger newFish = countsForOldFish[0] + countsForNewFish[0];
            for (int i = 0; i + 1 < countsForOldFish.Length; i++)
            {
                countsForOldFish[i] = countsForOldFish[i + 1];
                countsForNewFish[i] = countsForNewFish[i + 1];
            }

            for (int i = countsForOldFish.Length - 1; i + 1 < countsForNewFish.Length; i++)
            {
                countsForNewFish[i] = countsForNewFish[i + 1];
            }

            countsForNewFish[^1] = newFish;
            countsForOldFish[^1] = newFish;
            current += 1;
        }

        BigInteger total = countsForOldFish.Aggregate(
            countsForNewFish.Aggregate(
                    new BigInteger(0), 
                    (current1, count) => current1 + count
            ), 
            (current1, count) => current1 + count);
        Console.WriteLine($"Final count of Lanternfish: {total}");
    }
    
    public void Problem1()
    {
        SimulateLanternfishBreeding(80);
    }

    public void Problem2()
    {
        SimulateLanternfishBreeding(256);
    }
}