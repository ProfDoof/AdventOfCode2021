using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public class Cave : IComparable<Cave>
{
    public bool IsSmall { get; }
    public HashSet<Cave> ConnectedCaves { get; }
    public string Name { get; }

    public Cave(string name)
    {
        this.Name = name;
        this.IsSmall = !char.IsUpper(name, 0);
        this.ConnectedCaves = new HashSet<Cave>();
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append(this.Name)
            .Append(" -> ")
            .AppendLine(this.ConnectedCaves.FirstOrDefault()?.Name ?? "None");
        string spacer = new(' ', this.Name.Length);
        foreach (Cave connectedCave in this.ConnectedCaves.Skip(1))
        {
            builder.Append(spacer)
                .Append("  | ")
                .AppendLine(connectedCave.Name);
        }
        
        return builder.ToString();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Cave) obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public bool Equals(Cave? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public int CompareTo(Cave? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }
}

public struct Day12Input
{
    public HashSet<Cave> Caves;
}

public class Day12Solver : AdventOfCodeSolver<Day12Input>
{
    public Day12Solver() : base(12)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        this.Input = new Day12Input
        {
            Caves = new HashSet<Cave>(),
        };
        while (!inputReader.EndOfStream)
        {
            string line = (await inputReader.ReadLineAsync())?.Trim() ?? throw new InvalidOperationException();
            if (string.IsNullOrEmpty(line)) continue;
            string[] caves = line
                .Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            
            Cave temp = new(caves[0]);
            if (!this.Input.Caves.TryGetValue(temp, out Cave? first))
            {
                first = temp;
                this.Input.Caves.Add(first);
            }
            temp = new Cave(caves[1]);
            if (!this.Input.Caves.TryGetValue(temp, out Cave? second))
            {
                second = temp;
                this.Input.Caves.Add(second);
            }

            first.ConnectedCaves.Add(second);
            second.ConnectedCaves.Add(first);
        }
    }

    public override Task SolveProblemOneAsync()
    {
        Stack<List<Cave>> paths = new();
        if (!this.Input.Caves.TryGetValue(new Cave("start"), out Cave? start))
            throw new Exception("No start cave");
        
        if (!this.Input.Caves.TryGetValue(new Cave("end"), out Cave? end))
            throw new Exception("No end cave");
        paths.Push(new List<Cave> {start});
        List<List<Cave>> validPaths = new();
        while (paths.Any())
        {
            List<Cave> currentPath = paths.Pop();
            foreach (Cave connectedCave in currentPath.Last().ConnectedCaves)
            {
                if (connectedCave.Equals(end))
                {
                    validPaths.Add(currentPath.Append(end).ToList());
                    continue;
                }

                if (!connectedCave.IsSmall || connectedCave.IsSmall && !currentPath.Contains(connectedCave))
                {
                    paths.Push(currentPath.Append(connectedCave).ToList());
                }
            }
        }
        
        Console.WriteLine($"There are {validPaths.Count} valid paths from start to end");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {Stack<(List<Cave> paths, bool smallTwice)> paths = new();
        if (!this.Input.Caves.TryGetValue(new Cave("start"), out Cave start))
            throw new Exception("No start cave");
        
        if (!this.Input.Caves.TryGetValue(new Cave("end"), out Cave end))
            throw new Exception("No end cave");
        paths.Push((new List<Cave> {start}, false));
        List<List<Cave>> validPaths = new();
        while (paths.Any())
        {
            (List<Cave> currentPath, bool smallTwice) = paths.Pop();
            foreach (Cave connectedCave in currentPath.Last().ConnectedCaves)
            {
                if (connectedCave.Equals(end))
                {
                    validPaths.Add(currentPath.Append(end).ToList());
                    continue;
                }

                if (connectedCave.Equals(start))
                    continue;

                bool containsConnectedCave = currentPath.Contains(connectedCave) && connectedCave.IsSmall;
                if (!connectedCave.IsSmall || 
                    connectedCave.IsSmall && !smallTwice ||
                    connectedCave.IsSmall && smallTwice && !containsConnectedCave)
                {
                    paths.Push((currentPath.Append(connectedCave).ToList(), smallTwice || containsConnectedCave));
                }
            }
        }
        
        // Console.WriteLine(string.Join('\n', validPaths.Select(path => string.Join(',', path.Select(cave => cave.Name)))));
        Console.WriteLine($"There are {validPaths.Count} valid paths from start to end");
        return Task.CompletedTask;
    }
}