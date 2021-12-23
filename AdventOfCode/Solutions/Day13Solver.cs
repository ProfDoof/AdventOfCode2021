using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public class DotGrid
{
    private bool[,] _dotGrid;

    public DotGrid(bool[,] initialDotGrid)
    {
        this._dotGrid = initialDotGrid;
    }

    public int Fold((FoldDirection direction, int location) fold)
    {
        bool[,] newDotGrid;
        int count = 0;
        if (fold.direction == FoldDirection.Up)
        {
            int aboveRow = fold.location - 1;
            int belowRow = fold.location + 1;
            int maxColumn = this._dotGrid.GetLength(0);
            int maxRow = this._dotGrid.GetLength(1);
            newDotGrid = new bool[maxColumn, fold.location];
            while (aboveRow >= 0 && belowRow < maxRow)
            {
                for (int column = 0; column < maxColumn; column += 1)
                {
                    newDotGrid[column, aboveRow] = this._dotGrid[column, aboveRow] || this._dotGrid[column, belowRow];
                    count += newDotGrid[column, aboveRow] ? 1 : 0;
                }
                aboveRow -= 1;
                belowRow += 1;
            }
        }
        else // FoldDirection.Left
        {
            int leftColumn = fold.location - 1;
            int rightColumn = fold.location + 1;
            int maxColumn = this._dotGrid.GetLength(0);
            int maxRow = this._dotGrid.GetLength(1);
            newDotGrid = new bool[fold.location, maxRow];
            while (leftColumn >= 0 && rightColumn < maxColumn)
            {
                for (int row = 0; row < maxRow; row += 1)
                {
                    newDotGrid[leftColumn, row] = this._dotGrid[leftColumn, row] || this._dotGrid[rightColumn, row];
                    count += newDotGrid[leftColumn, row] ? 1 : 0;
                }
                leftColumn -= 1;
                rightColumn += 1;
            }
        }

        this._dotGrid = newDotGrid;
        return count;
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        for (int row = 0; row < this._dotGrid.GetLength(1); row += 1)
        {
            for (int column = 0; column < this._dotGrid.GetLength(0); column += 1)
            {
                builder.Append(this._dotGrid[column, row] ? '#' : '.');
            }

            builder.AppendLine();
        }
        return builder.ToString();
    }
}

public enum FoldDirection
{
    Up,
    Left,
}

public struct Day13Input
{
    public DotGrid DotGrid;
    public List<(FoldDirection foldDirection, int foldLocation)> Folds;
}

public class Day13Solver : AdventOfCodeSolver<Day13Input>
{
    public Day13Solver() : base(13)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        List<(int column, int row)> dots = new();
        string contents = (await inputReader.ReadLineAsync())?
            .Trim() ?? throw new Exception("Contents shouldn't be null");
        while (!string.IsNullOrEmpty(contents))
        {
            string[] position = contents
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            dots.Add((int.Parse(position[0]), int.Parse(position[1])));
            contents = (await inputReader.ReadLineAsync())?
                .Trim() ?? throw new Exception("Contents shouldn't be null");
        }

        int rowMax = 0;
        int columnMax = 0;
        foreach ((int column, int row) in dots)
        {
            if (row > rowMax)
                rowMax = row;
            if (column > columnMax)
                columnMax = column;
        }

        bool[,] initialDotGrid = new bool[columnMax+1, rowMax+1];
        foreach ((int column, int row) in dots)
        {
            initialDotGrid[column, row] = true;
        }

        this.Input = new Day13Input
        {
            DotGrid = new DotGrid(initialDotGrid),
            Folds = new List<(FoldDirection foldDirection, int foldLocation)>(),
        };
        
        while (!inputReader.EndOfStream)
        {
            contents = (await inputReader.ReadLineAsync())?
                .Trim() ?? throw new Exception("Line is null for some reason");
            if (string.IsNullOrEmpty(contents)) continue;
            string[] input = contents["fold along ".Length..].Split('=',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            this.Input.Folds.Add((
                input[0] switch{ 
                    "x" => FoldDirection.Left, 
                    "y" => FoldDirection.Up,
                    _ => throw new ArgumentOutOfRangeException("input[0]", input[0]),
                }, int.Parse(input[1])));
        }
    }

    public override Task SolveProblemOneAsync()
    {
        Console.WriteLine($"How many dots visible: {this.Input.DotGrid.Fold(this.Input.Folds.First())}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        foreach ((FoldDirection foldDirection, int foldLocation) fold in this.Input.Folds.Skip(1))
        {
            this.Input.DotGrid.Fold(fold);
        }
        
        Console.WriteLine(this.Input.DotGrid);
        return Task.CompletedTask;
    }
}