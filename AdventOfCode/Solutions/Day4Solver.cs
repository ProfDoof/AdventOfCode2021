using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;


public class BingoBoard
{
    private readonly int[,] _board;
    public int Id { get; }

    public BingoBoard(int id)
    {
        this._board = new int[5, 5];
        this.Id = id;
    }

    public int this[int i, int j]
    {
        get => _board[i, j];
        set => _board[i, j] = value;
    }

    public bool HasWon => this.HasWinningMarks();

    public void Mark(BingoPosition position)
    {
        this._board[position.Row, position.Column] = -1;
    }
    
    private bool IsWinningColumn(int column)
    {
        for (int i = 0; i < 5; i++)
        {
            if (this._board[i, column] != -1)
                return false;
        }

        return true;
    }

    private bool IsWinningRow(int row)
    {
        for (int i = 0; i < 5; i++)
        {
            if (this._board[row, i] != -1)
                return false;
        }

        return true;
    }
    
    private bool HasWinningMarks()
    {
        return IsWinningColumn(0) || IsWinningColumn(1) || 
               IsWinningColumn(2) || IsWinningColumn(3) || 
               IsWinningColumn(4) || IsWinningRow(0) || 
               IsWinningRow(1) || IsWinningRow(2) || 
               IsWinningRow(3) || IsWinningRow(4);
    }

    public int CalculateFinalScore(int lastCall)
    {
        
        int total = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (this[i, j] != -1)
                    total += this[i, j];
            }
        }

        return total * lastCall;
    }
}

public struct BingoPosition
{
    public BingoBoard Board;
    public int Row;
    public int Column;
}

public struct Day4BingoInput
{
    public List<int> Calls;
    public List<BingoBoard> Boards;
    public Dictionary<int, List<BingoPosition>> CallMap;
}

public class Day4Solver : AdventOfCodeSolver<Day4BingoInput>
{
    public Day4Solver() : base(4)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        this.Input = new Day4BingoInput
        {
            Calls = new List<int>(),
            Boards = new List<BingoBoard>(),
        };
        this.Input.Calls = (await inputReader
            .ReadLineAsync())!
            .Trim()
            .Split(',')
            .Select(s => s.Trim())
            .Select(int.Parse)
            .ToList();
        
        this.Input.CallMap = new Dictionary<int, List<BingoPosition>>();
        
        foreach (int bingoCall in this.Input.Calls)
        {
            this.Input.CallMap[bingoCall] = new List<BingoPosition>();
        }
        
        while (!inputReader.EndOfStream)
        {
            BingoBoard bingoBoard = new(this.Input.Boards.Count);
            await inputReader.ReadLineAsync();
            for (int i = 0; i < 5; i++)
            {
                List<int> row = (await inputReader
                    .ReadLineAsync())!
                    .Trim()
                    .Split(null as string[], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).Select(int.Parse)
                    .ToList();
                
                for (int j = 0; j < 5; j++)
                {
                    bingoBoard[i, j] = row[j];
                    this.Input.CallMap[row[j]].Add(new BingoPosition
                    {
                        Board = bingoBoard,
                        Column = j,
                        Row = i,
                    });
                }
            }
            this.Input.Boards.Add(bingoBoard);
        }
    }
    
    private (BingoBoard, int) FindFirstWinningBoard()
    {
        foreach (int bingoCall in this.Input.Calls)
        {
            foreach (BingoPosition bingoPosition in this.Input.CallMap[bingoCall])
            {
                bingoPosition.Board.Mark(bingoPosition);
                if (bingoPosition.Board.HasWon)
                    return (bingoPosition.Board, bingoCall);
            }
        }

        throw new Exception("Somehow no board won");
    }
    
    public override Task SolveProblemOneAsync()
    {
        (BingoBoard winningBoard, int lastCall) = this.FindFirstWinningBoard();
        Console.WriteLine($"Final Score: {winningBoard.CalculateFinalScore(lastCall)}");
        return Task.CompletedTask;
    }

    private (BingoBoard, int) FindLastWinningBoard()
    {
        HashSet<int> availableBoardIds = this.Input.Boards.Select(b => b.Id).ToHashSet();
        foreach (int bingoCall in this.Input.Calls)
        {
            IEnumerable<BingoPosition> availableBoards = this
                .Input
                .CallMap[bingoCall]
                .Where(bingoPosition => 
                    availableBoardIds.Contains(bingoPosition.Board.Id)
                );
            
            foreach (BingoPosition bingoPosition in availableBoards)
            {
                bingoPosition.Board.Mark(bingoPosition);

                if (!bingoPosition.Board.HasWon) continue;
                
                if (availableBoardIds.Count == 1)
                {
                    return (bingoPosition.Board, bingoCall);
                }
                availableBoardIds.Remove(bingoPosition.Board.Id);
            }
        }

        throw new Exception("Somehow no board won");
    }

    public override Task SolveProblemTwoAsync()
    {
        (BingoBoard winningBoard, int lastCall) = this.FindLastWinningBoard();
        Console.WriteLine($"Final Score: {winningBoard.CalculateFinalScore(lastCall)}");
        return Task.CompletedTask;
    }
}