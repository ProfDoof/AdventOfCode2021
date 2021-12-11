using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public struct SignalEntry
{
    public List<HashSet<char>> Signals;
    public List<HashSet<char>> Outputs;
}

public struct Day8Input
{
    public List<SignalEntry> SignalEntries;
}

public class Day8Solver : AdventOfCodeSolver<Day8Input>
{
    public Day8Solver() : base(8)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        this.Input = new Day8Input
        {
            SignalEntries = await AdventOfCodeSolverHelper.ParseEachLineAsync(inputReader, line =>
            {
                string[] splitLine = line.Split('|',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                return new SignalEntry
                {
                    Signals = splitLine[0]
                        .Split(null as string[], 
                            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => new HashSet<char>(s))
                        .ToList(),
                    Outputs = splitLine[1]
                        .Split(null as string[], 
                            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => new HashSet<char>(s))
                        .ToList(),
                };
            }),
        };
    }

    private static readonly Dictionary<int, int> KnownSegmentMapper = new() { {2, 1}, {4, 4}, {3, 7}, {7, 8}};
    
    public override Task SolveProblemOneAsync()
    {
        int count = this.Input.SignalEntries
            .SelectMany(se => se.Outputs)
            .Count(o => KnownSegmentMapper.ContainsKey(o.Count));
        Console.WriteLine($"1, 4, 7, and 8 appear {count} times");
        return Task.CompletedTask;
    }

    private static Dictionary<string, int> AnalyzeSignals(List<HashSet<char>> signals)
    {
        Dictionary<string, int> mapper = new();
        HashSet<char>[] hashMapper = new HashSet<char>[10];
        signals = signals.Where(s =>
        {
            if (!KnownSegmentMapper.TryGetValue(s.Count, out int mapsTo)) 
                return true;

            List<char> hashSetList = s.ToList();
            hashSetList.Sort();
            mapper[string.Join("", hashSetList)] = mapsTo;
            hashMapper[mapsTo] = s;
            return false;

        }).ToList();
        
        // Known: {1, 4, 7, 8}
        // Unknown: {0, 2, 3, 5, 6, 9}
        // Contains 1: {0, 3, 9}
        // Contains 4: {9}
        // Contains 7: {0, 3, 9}
        signals = signals.Where(s =>
        {
            if (!s.IsSupersetOf(hashMapper[4]))
                return true;

            List<char> hashSetList = s.ToList();
            hashSetList.Sort();
            mapper[string.Join("", hashSetList)] = 9;
            hashMapper[9] = s;
            return false;
        }).ToList();
        
        // Known: {1, 4, 7, 8, 9}
        // Unknown: {0, 2, 3, 5, 6}
        // Contains 1: {0, 3}
        // Contains 7: {0, 3}
        // Has 5 Segments: { 2, 3, 5 }
        // Has 6 Segments: { 0, 6 }
        signals = signals.Where(s =>
        {
            if (!s.IsSupersetOf(hashMapper[1]) || s.Count is not (5 or 6))
                return true;

            List<char> hashSetList = s.ToList();
            hashSetList.Sort();
            int mapsTo = s.Count == 5 ? 3 : 0;
            mapper[string.Join("", hashSetList)] = mapsTo;
            hashMapper[mapsTo] = s;
            return false;
        }).ToList();
        
        // Known: {0, 1, 3, 4, 7, 8, 9}
        // Unknown: {2, 5, 6}
        // Has 5 Segments: { 2, 5 }
        // Has 6 Segments: { 6 }
        signals = signals.Where(s =>
        {
            if (s.Count is not 6)
                return true;

            List<char> hashSetList = s.ToList();
            hashSetList.Sort();
            mapper[string.Join("", hashSetList)] = 6;
            hashMapper[6] = s;
            return false;
        }).ToList();

        // Known: {0, 1, 3, 4, 6, 7, 8, 9}
        // Unknown: {2, 5}
        // Has 5 Segments: { 2, 5 }
        // 2 ^ 9 = {a, c, d, g} with count 4
        // 5 ^ 9 = {a, b, d, f, g} with count 5
        bool firstIs2 = signals.First().Intersect(hashMapper[9]).Count() == 4;
        int first;
        int second;
        if (firstIs2)
        {
            first = 2;
            second = 5;
        }
        else
        {
            first = 5;
            second = 2;
        }
        
        List<char> hashSetListOne = signals.First().ToList();
        hashSetListOne.Sort();
        mapper[string.Join("", hashSetListOne)] = first;
        List<char> hashSetListTwo = signals.Skip(1).First().ToList();
        hashSetListTwo.Sort();
        mapper[string.Join("", hashSetListTwo)] = second;
        return mapper;
    }

    public override Task SolveProblemTwoAsync()
    {
        int total = 0;
        foreach (SignalEntry signalEntry in this.Input.SignalEntries)
        {
            Dictionary<string, int> mapper = AnalyzeSignals(signalEntry.Signals);
            total += signalEntry.Outputs.Select(o =>
            {
                List<char> hashSetList = o.ToList();
                hashSetList.Sort();
                return string.Join("", hashSetList);
            }).Aggregate(0, (i, set) => i * 10 + mapper[set]);
            // Console.WriteLine(string
            //     .Join(',',
            //         mapper.Select(kvp => $"{{{string.Join(',', kvp.Key)}}} => {kvp.Value}")));
        }
        
        Console.WriteLine($"The total of all of the analyzed signals and provided outputs is {total}");
        
        return Task.CompletedTask;
    }
}