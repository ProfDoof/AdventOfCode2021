using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public readonly record struct Position : IComparable<Position>
{
    public int Row { get; }
    public int Column { get; }

    public Position(int column, int row)
    {
        Column = column;
        Row = row;
    }

    public int CompareTo(Position other)
    {
        int rowComparison = Row.CompareTo(other.Row);
        return rowComparison != 0 ? rowComparison : Column.CompareTo(other.Column);
    }
    
}

public struct Day15Input
{
    public int[,] RiskLevels;
    public override string ToString()
    {
        StringBuilder builder = new();
        for (int row = 0; row < this.RiskLevels.GetLength(0); row += 1)
        {
            for (int column = 0; column < this.RiskLevels.GetLength(1); column += 1)
            {
                builder.Append(this.RiskLevels[row, column]);
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }
}

public class Day15Solver : AdventOfCodeSolver<Day15Input>
{
    public Day15Solver() : base(15)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        List<List<int>> riskLevels = new();
        while (!inputReader.EndOfStream)
        {
            string contents = (await inputReader.ReadLineAsync())?.Trim() ?? throw new Exception("Null line");
            if (string.IsNullOrWhiteSpace(contents)) continue;

            List<int> lineRiskLevels = new();
            for (int index = 0; index < contents.Length; index += 1)
            {
                lineRiskLevels.Add(int.Parse(contents.AsSpan(index, 1)));
            }
            riskLevels.Add(lineRiskLevels);
        }

        this.Input = new Day15Input
        {
            RiskLevels = new int[riskLevels.Count, riskLevels.First().Count],
        };

        for (int row = 0; row < riskLevels.Count; row += 1)
        {
            List<int> currentRow = riskLevels[row];
            for (int column = 0; column < currentRow.Count; column += 1)
            {
                this.Input.RiskLevels[row, column] = currentRow[column];
            }
        }
    }
    
    private class PositionComparer : IEqualityComparer<(int row, int column)>
    {
        public bool Equals((int row, int column) x, (int row, int column) y)
        {
            return x.row == y.row && x.column == y.column;
        }

        public int GetHashCode((int row, int column) obj)
        {
            return HashCode.Combine(obj.row, obj.column);
        }
    }

    private static int FindMinimumRisk(int[,] riskLevels)
    {
        PriorityQueue<(int row, int column, int risk), int> minPositionFound = new();
        PositionComparer comparer = new();
        HashSet<(int row, int column)> positions = new(comparer);
        int endRow = riskLevels.GetLength(0) - 1;
        int endColumn = riskLevels.GetLength(1) - 1;
        (int row, int column) end = (endRow, endColumn);

        minPositionFound.Enqueue((0, 0, 0), 0);

        while (true)
        {
            (int row, int column, int risk) = minPositionFound.Dequeue();
            (int row, int column) current = (row, column);

            if (comparer.Equals(current, end))
            {
                return risk;
            }

            if (positions.Contains(current)) continue;
            positions.Add(current);
            int tempRow = current.row - 1;
            int tempColumn = current.column;
            if (tempRow >= 0)
            {
                int tempRisk = risk + riskLevels[tempRow, tempColumn];
                minPositionFound.Enqueue((tempRow, tempColumn, tempRisk), tempRisk);
            }

            tempRow = current.row + 1;
            tempColumn = current.column;
            if (tempRow < riskLevels.GetLength(0))
            {
                int tempRisk = risk + riskLevels[tempRow, tempColumn];
                minPositionFound.Enqueue((tempRow, tempColumn, tempRisk), tempRisk);
            }

            tempRow = current.row;
            tempColumn = current.column - 1;
            if (tempColumn >= 0)
            {
                int tempRisk = risk + riskLevels[tempRow, tempColumn];
                minPositionFound.Enqueue((tempRow, tempColumn, tempRisk), tempRisk);
            }

            tempRow = current.row;
            tempColumn = current.column + 1;
            if (tempColumn < riskLevels.GetLength(1))
            {
                int tempRisk = risk + riskLevels[tempRow, tempColumn];
                minPositionFound.Enqueue((tempRow, tempColumn, tempRisk), tempRisk);
            }
        }
    }
    
    public override Task SolveProblemOneAsync()
    {
        int minRisk = FindMinimumRisk(this.Input.RiskLevels);

        Console.WriteLine($"Minimum Risk: {minRisk}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        int numRows = this.Input.RiskLevels.GetLength(0);
        int numColumns = this.Input.RiskLevels.GetLength(1);
        
        int[,] bigRiskLevels = new int[numRows * 5, numColumns * 5];
        for (int row = 0; row < numRows; row += 1)
        {
            for (int column = 0; column < numColumns; column += 1)
            {
                for (int right = 0; right < 5; right += 1)
                {
                    for (int down = 0; down < 5; down += 1)
                    {
                        // ReSharper disable once ArrangeRedundantParentheses -- Adds Clarity
                        int temp = ((this.Input.RiskLevels[row, column] - 1 + down + right) % 9) + 1;
                        bigRiskLevels[row + numRows * down, column + numColumns * right] = temp;
                    }
                }
            }
        }
        
        Console.WriteLine($"Minimum Risk: {FindMinimumRisk(bigRiskLevels)}");
        return Task.CompletedTask;
    }
}