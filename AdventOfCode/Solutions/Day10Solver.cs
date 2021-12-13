using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public class Day10Solver : AdventOfCodeSolver<Day10Input>
{
    public Day10Solver() : base(10)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        this.Input.Lines = await AdventOfCodeSolverHelper.ParseEachLineAsync(inputReader,
            input => input.Select(c => (ChunkToken)c).ToList()
        );
    }

    private static int HandleOpenToken(Stack<ChunkToken> stack, ChunkToken token)
    {
        stack.Push(token);
        return 0;
    }

    private static int HandleCorruptedLine(List<ChunkToken> tokens)
    {
        Stack<ChunkToken> stack = new();
        foreach (ChunkToken token in tokens)
        {
            switch (token)
            {
                case ChunkToken.OpenBrace:
                case ChunkToken.OpenParentheses:
                case ChunkToken.OpenBracket:
                case ChunkToken.OpenAngleBracket:
                    stack.Push(token);
                    break;
                case ChunkToken.CloseParentheses:
                    if (stack.Pop() != ChunkToken.OpenParentheses)
                        return 3;
                    break;
                case ChunkToken.CloseBrace:
                    if (stack.Pop() != ChunkToken.OpenBrace)
                        return 1197;
                    break;
                case ChunkToken.CloseBracket:
                    if (stack.Pop() != ChunkToken.OpenBracket)
                        return 57;
                    break;
                case ChunkToken.CloseAngleBracket:
                    if (stack.Pop() != ChunkToken.OpenAngleBracket)
                        return 25137;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(token), token, null);
            }
        }

        return 0;
    }

    public override Task SolveProblemOneAsync()
    {
        int score = this.Input.Lines.Sum(HandleCorruptedLine);
        Console.WriteLine($"Syntax Error Score: {score}");
        return Task.CompletedTask;
    }

    private ulong HandleIncompleteLines(List<ChunkToken> tokens)
    {
        Stack<ChunkToken> stack = new();
        foreach (ChunkToken token in tokens)
        {
            switch (token)
            {
                case ChunkToken.OpenBrace:
                case ChunkToken.OpenParentheses:
                case ChunkToken.OpenBracket:
                case ChunkToken.OpenAngleBracket:
                    stack.Push(token);
                    break;
                case ChunkToken.CloseParentheses:
                    if (stack.Pop() != ChunkToken.OpenParentheses)
                        return 0;
                    break;
                case ChunkToken.CloseBrace:
                    if (stack.Pop() != ChunkToken.OpenBrace)
                        return 0;
                    break;
                case ChunkToken.CloseBracket:
                    if (stack.Pop() != ChunkToken.OpenBracket)
                        return 0;
                    break;
                case ChunkToken.CloseAngleBracket:
                    if (stack.Pop() != ChunkToken.OpenAngleBracket)
                        return 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(token), token, null);
            }
        }

        ulong score = 0;
        while (stack.Any())
        {
            ChunkToken token = stack.Pop();
            score = score * 5 + token switch
            {
                ChunkToken.OpenParentheses => 1UL,
                ChunkToken.OpenBracket => 2UL,
                ChunkToken.OpenBrace => 3UL,
                ChunkToken.OpenAngleBracket => 4UL,
                _ => throw new ArgumentOutOfRangeException(nameof(token), "Invalid token found in stack"),
            };
        }

        return score;
    }
    
    public override Task SolveProblemTwoAsync()
    {
        List<ulong> scores = this.Input.Lines.Select(HandleIncompleteLines).Where(score => score != 0).ToList();
        scores.Sort();
        Console.WriteLine($"Middle Score: {scores[(int)Math.Floor(scores.Count / 2.0)]}");
        return Task.CompletedTask;
    }
}

public struct Day10Input
{
    public List<List<ChunkToken>> Lines;
}

public enum ChunkToken
{
    OpenParentheses = '(',
    OpenBrace = '{',
    OpenBracket = '[', 
    OpenAngleBracket = '<',
    CloseParentheses = ')',
    CloseBrace = '}',
    CloseBracket = ']',
    CloseAngleBracket = '>',
}