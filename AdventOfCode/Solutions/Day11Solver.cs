using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public record struct OctupiPosition
{
    public int Row;
    public int Column;

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Row, this.Column);
    }

    public (OctupiPosition Above,
        OctupiPosition AboveLeft,
        OctupiPosition AboveRight,
        OctupiPosition Below, 
        OctupiPosition BelowLeft,
        OctupiPosition BelowRight,
        OctupiPosition Left, 
        OctupiPosition Right) GetAdjacents()
    {
        return (this with {Row = this.Row - 1},
            this with {Row = this.Row - 1, Column = this.Column - 1},
            this with {Row = this.Row - 1, Column = this.Column + 1},
            this with {Row = this.Row + 1},
            this with {Row = this.Row + 1, Column = this.Column - 1},
            this with {Row = this.Row + 1, Column = this.Column + 1},
            this with {Column = this.Column - 1},
            this with {Column = this.Column + 1});
    }
}

public class OctupiGrid
{
    private readonly int[,] _energyLevels;
    private readonly List<OctupiPosition> _positions;
    private readonly Dictionary<int, HashSet<OctupiPosition>> _energyLevelMap;

    public OctupiGrid(int[,] energyLevels)
    {
        this._energyLevels = new int[energyLevels.GetLength(0), energyLevels.GetLength(1)];
        Array.Copy(energyLevels, this._energyLevels, energyLevels.Length);
        this._positions = Enumerable
            .Range(0, this._energyLevels.GetLength(0))
            .SelectMany(row => Enumerable.Range(0, this._energyLevels.GetLength(1)).Select(column => new OctupiPosition
            {
                Column = column,
                Row = row,
            }))
            .ToList();
        this._energyLevelMap = new Dictionary<int, HashSet<OctupiPosition>>();
        foreach (int initial in Enumerable.Range(0, 11))
        {
            this._energyLevelMap[initial] = new HashSet<OctupiPosition>();
        }
        
        foreach (OctupiPosition position in this._positions)
        {
            this._energyLevelMap[this._energyLevels[position.Row, position.Column]].Add(position);
        }
    }

    public bool InSync => this._energyLevels.Cast<int>().All(el => el == 0);

    /// <summary>
    /// Simulates a single step in time and returns the number of flashes that would occur
    /// </summary>
    /// <returns></returns>
    public int Step()
    {
        foreach (OctupiPosition position in this._positions)
        {
            int energyLevel = this._energyLevels[position.Row, position.Column];
            this._energyLevelMap[energyLevel].Remove(position);
            this._energyLevelMap[energyLevel + 1].Add(position);
            this._energyLevels[position.Row, position.Column] = energyLevel + 1;
        }
        
        HashSet<OctupiPosition> flashingOctupi;
        do
        {
            flashingOctupi = this._energyLevelMap[10];
            this._energyLevelMap[10] = new HashSet<OctupiPosition>();
            foreach (OctupiPosition position in flashingOctupi)
            {
                OctupiPosition above;
                OctupiPosition aboveLeft;
                OctupiPosition aboveRight;
                OctupiPosition below;
                OctupiPosition belowLeft;
                OctupiPosition belowRight;
                OctupiPosition left;
                OctupiPosition right;
                (above, aboveLeft, aboveRight, below, belowLeft, belowRight, left, right) = position.GetAdjacents();
                if (above.Row >= 0)
                {
                    PropagateFlash(above);
                }

                if (aboveLeft.Row >= 0 && aboveLeft.Column >= 0)
                {
                    PropagateFlash(aboveLeft);
                }

                if (aboveRight.Row >= 0 && aboveRight.Column < this._energyLevels.GetLength(1))
                {
                    PropagateFlash(aboveRight);
                }

                if (below.Row < this._energyLevels.GetLength(0))
                {
                    PropagateFlash(below);
                }

                if (belowLeft.Row < this._energyLevels.GetLength(0) && belowLeft.Column >= 0)
                {
                    PropagateFlash(belowLeft);
                }

                if (belowRight.Row < this._energyLevels.GetLength(0) && belowRight.Column < this._energyLevels.GetLength(1))
                {
                    PropagateFlash(belowRight);
                }

                if (left.Column >= 0)
                {
                    PropagateFlash(left);
                }

                if (right.Column < this._energyLevels.GetLength(1))
                {
                    PropagateFlash(right);
                }

                this._energyLevelMap[0].Add(position);
                this._energyLevels[position.Row, position.Column] = 0;
            }
        } while (flashingOctupi.Any());

        return this._energyLevelMap[0].Count;
    }

    private void PropagateFlash(OctupiPosition positionToFlash)
    {
        int energyLevel = this._energyLevels[positionToFlash.Row, positionToFlash.Column];
        if (energyLevel is 0 or 10) return;
        
        this._energyLevelMap[energyLevel].Remove(positionToFlash);
        this._energyLevels[positionToFlash.Row, positionToFlash.Column] = energyLevel + 1;
        this._energyLevelMap[energyLevel + 1].Add(positionToFlash);
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        for (int row = 0; row < this._energyLevels.GetLength(0); row++)
        {
            builder.Append(':');
            for (int column = 0; column < this._energyLevels.GetLength(1); column++)
            {
                builder.Append(this._energyLevels[row, column]).Append(':');
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }
}

public struct Day11Input
{
    public int[,] OctupiEnergyLevels;
}

public class Day11Solver : AdventOfCodeSolver<Day11Input>
{
    public Day11Solver() : base(11)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        List<List<int>> octupiEnergyLevels = new();
        while (!inputReader.EndOfStream)
        {
            string row = await inputReader.ReadLineAsync() ?? throw new InvalidOperationException();
            row = row.Trim();
            
            if (string.IsNullOrWhiteSpace(row)) continue;
            
            List<int> heatmapRow = row.Select(c => c - '0').ToList();
            octupiEnergyLevels.Add(heatmapRow);
        }

        int[,] octupiEnergyMatrix = new int[octupiEnergyLevels.Count, octupiEnergyLevels.First().Count];

        for (int row = 0; row < octupiEnergyMatrix.GetLength(0); row += 1)
        {
            for (int column = 0; column < octupiEnergyMatrix.GetLength(1); column += 1)
            {
                octupiEnergyMatrix[row, column] = octupiEnergyLevels[row][column];
            }
        }

        this.Input = new Day11Input
        {
            OctupiEnergyLevels = octupiEnergyMatrix,
        };
    }

    public override Task SolveProblemOneAsync()
    {
        OctupiGrid octupiGrid = new(this.Input.OctupiEnergyLevels);
        int result = Enumerable
            .Repeat(0, 100)
            .Sum(_ => octupiGrid.Step());
        Console.WriteLine($"Number of flashes: {result}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        OctupiGrid octupiGrid = new OctupiGrid(this.Input.OctupiEnergyLevels);
        int step = 0;
        while (!octupiGrid.InSync)
        {
            step += 1;
            octupiGrid.Step();
            Console.WriteLine(octupiGrid);
        }
        
        Console.WriteLine($"Step Number Synchronized: {step}");
        return Task.CompletedTask;
    }
}