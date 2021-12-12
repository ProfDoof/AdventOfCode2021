using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public record struct HeightMapIndex
{
    public int Row;
    public int Column;

    public (HeightMapIndex Above, HeightMapIndex Below, HeightMapIndex Left, HeightMapIndex Right) GetAdjacent()
    {
        return (this with {Row = this.Row - 1},
                this with {Row = this.Row + 1},
                this with {Column = this.Column - 1},
                this with {Column = this.Column + 1});
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}

public struct Day9Input
{
    public int[,] HeightMap;
}

public class Day9Solver : AdventOfCodeSolver<Day9Input>
{
    private IEnumerable<HeightMapIndex> HeightMapIndices;
    public Day9Solver() : base(9)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        List<List<int>> heatmap = new();
        while (!inputReader.EndOfStream)
        {
            string row = await inputReader.ReadLineAsync() ?? throw new InvalidOperationException();
            row = row.Trim();
            
            if (string.IsNullOrWhiteSpace(row)) continue;
            
            List<int> heatmapRow = row.Select(c => c - '0').ToList();
            heatmap.Add(heatmapRow);
        }

        this.Input.HeightMap = new int[heatmap.Count, heatmap.First().Count];
        this.HeightMapIndices = Enumerable
            .Range(0, heatmap.Count)
            .SelectMany(row => 
                Enumerable
                    .Range(0, heatmap[row].Count)
                    .Select(column => 
                        new HeightMapIndex
                        {
                            Column = column,
                            Row = row,
                        }
                    )
            );
        foreach (HeightMapIndex index in this.HeightMapIndices)
        {
            this.Input.HeightMap[index.Row, index.Column] = heatmap[index.Row][index.Column];
        }
    }

    private IEnumerable<HeightMapIndex>? _lowestPoints = null;
    private IEnumerable<HeightMapIndex> FindLowestPoints()
    {
        if (_lowestPoints is not null)
            return _lowestPoints;

        List<HeightMapIndex> lowestPoints = new();
        foreach (HeightMapIndex index in this.HeightMapIndices)
        {
            bool upIsHigher = index.Row == 0 
                              || this.Input.HeightMap[index.Row, index.Column] < this.Input.HeightMap[index.Row - 1, index.Column];
            bool belowIsHigher = index.Row == this.Input.HeightMap.GetLength(0) - 1 
                                 || this.Input.HeightMap[index.Row, index.Column] < this.Input.HeightMap[index.Row + 1, index.Column];
            bool leftIsHigher = index.Column == 0
                                || this.Input.HeightMap[index.Row, index.Column] < this.Input.HeightMap[index.Row, index.Column - 1];
            bool rightIsHigher = index.Column == this.Input.HeightMap.GetLength(1) - 1
                                 || this.Input.HeightMap[index.Row, index.Column] < this.Input.HeightMap[index.Row, index.Column + 1];
            if (upIsHigher && belowIsHigher && leftIsHigher && rightIsHigher)
            {
                lowestPoints.Add(index);
            }
        }

        this._lowestPoints = lowestPoints;
        return this._lowestPoints;
    }

    public override Task SolveProblemOneAsync()
    {
        int totalRiskLevel = this.FindLowestPoints().Sum(index => this.Input.HeightMap[index.Row, index.Column] + 1);
        Console.WriteLine($"The total risk level is {totalRiskLevel}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        SortedDictionary<int, int> basinSizes = new();
        HashSet<HeightMapIndex> seenHeightMapIndices = new();
        foreach (HeightMapIndex index in this.FindLowestPoints())
        {
            int basinSize = 1;
            Stack<HeightMapIndex> basinEdges = new();
            basinEdges.Push(index);
            while (basinEdges.Any())
            {
                HeightMapIndex current = basinEdges.Pop();
                (HeightMapIndex above, HeightMapIndex below, HeightMapIndex left, HeightMapIndex right) = 
                    current.GetAdjacent();
                if (above.Row >= 0
                    && !seenHeightMapIndices.Contains(above)
                    && this.Input.HeightMap[above.Row, above.Column] != 9
                    && this.Input.HeightMap[above.Row, above.Column] > this.Input.HeightMap[index.Row, index.Column])
                {
                    basinEdges.Push(above);
                    seenHeightMapIndices.Add(above);
                    basinSize += 1;
                }
                
                if (below.Row < this.Input.HeightMap.GetLength(0)
                    && !seenHeightMapIndices.Contains(below)
                    && this.Input.HeightMap[below.Row, below.Column] != 9
                    && this.Input.HeightMap[below.Row, below.Column] > this.Input.HeightMap[index.Row, index.Column])
                {
                    basinEdges.Push(below);
                    seenHeightMapIndices.Add(below);
                    basinSize += 1;
                }
                
                if (left.Column >= 0
                    && !seenHeightMapIndices.Contains(left)
                    && this.Input.HeightMap[left.Row, left.Column] != 9
                    && this.Input.HeightMap[left.Row, left.Column] > this.Input.HeightMap[index.Row, index.Column])
                {
                    basinEdges.Push(left);
                    seenHeightMapIndices.Add(left);
                    basinSize += 1;
                }
                
                if (right.Column < this.Input.HeightMap.GetLength(1)
                    && !seenHeightMapIndices.Contains(right)
                    && this.Input.HeightMap[right.Row, right.Column] != 9
                    && this.Input.HeightMap[right.Row, right.Column] > this.Input.HeightMap[index.Row, index.Column])
                {
                    basinEdges.Push(right);
                    seenHeightMapIndices.Add(right);
                    basinSize += 1;
                }
            }

            if (basinSizes.ContainsKey(basinSize))
            {
                basinSizes[basinSize] += 1;
            }
            else
            {
                basinSizes[basinSize] = 1;
            }
        }

        int found = 0;
        int result = 1;
        foreach ((int basinSize, int basinCount) in basinSizes.Reverse())
        {
            int repeatCount = basinCount;
            bool last = false;
            if (found + basinCount >= 3)
            {
                repeatCount = 3 - found;
                last = true;
            }

            result = Enumerable
                .Repeat(basinSize, repeatCount)
                .Aggregate(result, (initial, current) => initial * current);
            found += basinCount;
            if (last)
                break;
        }
        Console.WriteLine($"The three largest basin sizes multiplied together gives {result}");
        return Task.CompletedTask;
    }
}