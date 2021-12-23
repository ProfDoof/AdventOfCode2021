using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public struct Day14Input
{
    public string PolymerTemplate;
    public Dictionary<char, Dictionary<char, (string pair1, string pair2, char inserted)>> InsertionRules;
}

public class Day14Solver : AdventOfCodeSolver<Day14Input>
{
    public Day14Solver() : base(14)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        this.Input = new Day14Input
        {
            PolymerTemplate = (await inputReader.ReadLineAsync())?.Trim() ?? throw new Exception("First line was null"),
            InsertionRules = new Dictionary<char, Dictionary<char, (string pair1, string pair2, char inserted)>>(),
        };

        await inputReader.ReadLineAsync();
        while (!inputReader.EndOfStream)
        {
            string[] split = (await inputReader.ReadLineAsync())?
                             .Trim()
                             .Split("->", 
                                 StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                             ?? throw new Exception("Line was null");
            if (!this.Input.InsertionRules.ContainsKey(split[0][0]))
                this.Input.InsertionRules[split[0][0]] = new Dictionary<char, (string pair1, string pair2, char inserted)>();
            this.Input.InsertionRules[split[0][0]][split[0][1]] = ($"{split[0][0]}{split[1][0]}", $"{split[1][0]}{split[0][1]}", split[1][0]);
        }
    }

    public override Task SolveProblemOneAsync()
    {
        StrengthenPolymer(10);
        return Task.CompletedTask;
    }

    private static void AddCountToPairCount(IDictionary<char, IDictionary<char, ulong>> pairCounts, 
        char first, char second, ulong count)
    {
        if (!pairCounts.ContainsKey(first))
            pairCounts[first] = new Dictionary<char, ulong>();
        if (!pairCounts[first].ContainsKey(second))
            pairCounts[first][second] = 0;
        pairCounts[first][second] += count;
    }

    private static void AddCountToPairCount(IDictionary<char, IDictionary<char, ulong>> pairCounts,
        string pair, ulong count) => AddCountToPairCount(pairCounts, pair[0], pair[1], count);

    private void StrengthenPolymer(uint numInsertions)
    {
        Dictionary<char, ulong> charCounts = new();
        Dictionary<char, IDictionary<char, ulong>> pairCounts = new();
        char last = this.Input.PolymerTemplate[0];
        charCounts[last] = 1;
        foreach (char current in this.Input.PolymerTemplate.Skip(1))
        {
            AddCountToPairCount(pairCounts, last, current, 1);
            if (!charCounts.ContainsKey(current))
                charCounts[current] = 0;
            charCounts[current] += 1;
            last = current;
        }
        
        for (int i = 0; i < numInsertions; i++)
        {
            List<(char first, char second, ulong count)> prevPairs = pairCounts
                .SelectMany(first => 
                    first.Value.Select(second => 
                        (first.Key, second.Key, second.Value)
                    )
                ).ToList();
            foreach ((char first, char second, ulong _) in prevPairs)
            {
                pairCounts[first][second] = 0;
            }
            foreach ((char first, char second, ulong count) in prevPairs)
            {
                (string pair1, string pair2, char insert) = this.Input.InsertionRules[first][second];
                AddCountToPairCount(pairCounts, pair1, count);
                AddCountToPairCount(pairCounts, pair2, count);
                if (!charCounts.ContainsKey(insert))
                    charCounts[insert] = 0;
                charCounts[insert] += count;
            }
        }

        List<KeyValuePair<char, ulong>> charCountPairs = charCounts.ToList();
        charCountPairs.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
        Console.WriteLine(string.Join(", ", charCountPairs.Select(pair => $"{pair.Key}: {pair.Value}")));
        Console.WriteLine($"MCE - LCE = {charCountPairs.First().Value - charCountPairs.Last().Value}");
    }

    public override Task SolveProblemTwoAsync()
    {
        StrengthenPolymer(40);
        return Task.CompletedTask;
    }
}