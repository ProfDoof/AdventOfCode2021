using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public struct Day3Input
{
    public List<ushort> Inputs;
}

public class Day3Solver : AdventOfCodeSolver<Day3Input>
{
    public Day3Solver() : base(3)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        this.Input = new Day3Input
        {
            Inputs = await AdventOfCodeSolverHelper.ParseEachLineAsync(inputReader, input => Convert.ToUInt16(input, 2)),
        };
    }

    public override Task SolveProblemOneAsync()
    {
        int[] countOnes = new int[12];
        foreach (ushort t in this.Input.Inputs)
        {
            for (int j = 0; j < countOnes.Length; j++)
            {
                countOnes[j] += (t >> j) & 1;
            }
        }

        Console.WriteLine(string.Join(" ", countOnes));
        uint gamma = 0;
        uint epsilon = 0;
        int midlength = this.Input.Inputs.Count / 2;
        for (int i = countOnes.Length - 1; i >= 0; i--)
        {
            gamma <<= 1;
            epsilon <<= 1;
            bool onesAreMoreFrequent = countOnes[i] > midlength;
            gamma |= onesAreMoreFrequent ? (ushort)1 : (ushort)0;
            epsilon |= !onesAreMoreFrequent ? (ushort) 1 : (ushort) 0;
        }
        Console.WriteLine($"Gamma: {gamma}");
        Console.WriteLine($"Epsilon: {epsilon}");
        Console.WriteLine($"Final Answer: {gamma * epsilon}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        List<ushort> startWithOnes = new();
        List<ushort> startWithZeros = new();
        ushort[] inputClone = new ushort[this.Input.Inputs.Count];
        this.Input.Inputs.CopyTo(inputClone, 0);
        ushort mask = 1 << 11;
        while (inputClone.Length != 1)
        {
            foreach (ushort input in inputClone)
            {
                if ((input & mask) != 0)
                {
                    startWithOnes.Add(input);
                }
                else
                {
                    startWithZeros.Add(input);
                }
            }

            inputClone = (startWithOnes.Count < startWithZeros.Count)
                ? startWithZeros.ToArray()
                : startWithOnes.ToArray();
            startWithOnes.Clear();
            startWithZeros.Clear();
            mask >>= 1;
        }

        ushort oxygenGeneratorRating = inputClone.First();
        Console.WriteLine($"Oxygen Generator Rating: {oxygenGeneratorRating}");
        inputClone = new ushort[this.Input.Inputs.Count];
        this.Input.Inputs.CopyTo(inputClone, 0);
        mask = 1 << 11;
        while (inputClone.Length != 1)
        {
            foreach (ushort input in inputClone)
            {
                if ((input & mask) != 0)
                {
                    startWithOnes.Add(input);
                }
                else
                {
                    startWithZeros.Add(input);
                }
            }

            inputClone = (startWithOnes.Count >= startWithZeros.Count)
                ? startWithZeros.ToArray()
                : startWithOnes.ToArray();
            startWithOnes.Clear();
            startWithZeros.Clear();
            mask >>= 1;
        }

        ushort co2ScrubberRating = inputClone.First();
        Console.WriteLine($"CO2 Scrubber Rating: {co2ScrubberRating}");
        Console.WriteLine($"Life Support Rating: {oxygenGeneratorRating*co2ScrubberRating}");
        return Task.CompletedTask;
    }
}