using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode;

public class BingoBoard
{
    private int[,] _board;

    public BingoBoard()
    {
        this._board = new int[5, 5];
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
    public int BoardNumber;
    public int Row;
    public int Column;
}

public class Day4 : IDay
{
    private readonly List<int> _bingoCalls;
    private readonly List<BingoBoard> _bingoBoards = new();
    private readonly Dictionary<int, List<BingoPosition>> _bingoCallMap = new ();
    public Day4()
    {
        using FileStream inputFileStream =
            new($"./Inputs/day4input.txt", FileMode.Open, FileAccess.Read);
        using StreamReader inputFileReader = new(inputFileStream);
        string temp = inputFileReader.ReadLine()!.Trim();
        Console.WriteLine(temp);
        this._bingoCalls = temp.Split(',').Select(s => s.Trim()).Select(int.Parse).ToList();
        foreach (int bingoCall in this._bingoCalls)
        {
            this._bingoCallMap[bingoCall] = new List<BingoPosition>();
        }
        while (!inputFileReader.EndOfStream)
        {
            BingoBoard bingoBoard = new();
            int bingoBoardNumber = this._bingoBoards.Count;
            inputFileReader.ReadLine();
            for (int i = 0; i < 5; i++)
            {
                List<int> row = inputFileReader.ReadLine()!.Trim().Split()
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim()).Select(int.Parse).ToList();
                for (int j = 0; j < 5; j++)
                {
                    bingoBoard[i, j] = row[j];
                    this._bingoCallMap[row[j]].Add(new BingoPosition
                    {
                        BoardNumber = bingoBoardNumber,
                        Column = j,
                        Row = i,
                    });
                }
            }
            this._bingoBoards.Add(bingoBoard);
        }
        
    }

    public int DayOfMonth => 4;

    private (BingoBoard, int) FindFirstWinningBoard()
    {
        foreach (int bingoCall in _bingoCalls)
        {
            foreach (BingoPosition bingoPosition in this._bingoCallMap[bingoCall])
            {
                BingoBoard board = this._bingoBoards[bingoPosition.BoardNumber];
                board.Mark(bingoPosition);
                if (board.HasWon)
                    return (board, bingoCall);
            }
        }

        throw new Exception("Somehow no board won");
    }
    
    public void Problem1()
    {
        (BingoBoard winningBoard, int lastCall) = this.FindFirstWinningBoard();
        Console.WriteLine($"Final Score: {winningBoard.CalculateFinalScore(lastCall)}");
    }

    private (BingoBoard, int) FindLastWinningBoard()
    {
        HashSet<int> availableBoardNumbers = this._bingoCallMap.Keys.ToHashSet();
        foreach (int bingoCall in _bingoCalls)
        {
            foreach (BingoPosition bingoPosition in this._bingoCallMap[bingoCall])
            {
                BingoBoard board = this._bingoBoards[bingoPosition.BoardNumber];
                board.Mark(bingoPosition);
                if (availableBoardNumbers.Contains(bingoPosition.BoardNumber) && board.HasWon)
                {
                    if (availableBoardNumbers.Count == 1)
                    {
                        return (board, bingoCall);
                    }
                    availableBoardNumbers.Remove(bingoPosition.BoardNumber);
                }
            }
        }

        throw new Exception("Somehow no board won");
    }
    
    public void Problem2()
    {
        (BingoBoard winningBoard, int lastCall) = this.FindLastWinningBoard();
        Console.WriteLine($"Final Score: {winningBoard.CalculateFinalScore(lastCall)}");
    }
}