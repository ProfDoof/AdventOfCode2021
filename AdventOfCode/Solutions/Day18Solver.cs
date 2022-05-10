using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Solutions;

public abstract class SnailNumber
{
    protected SnailNumber(int depth)
    {
        Depth = depth;
    }

    public int Depth { get; set; }
    public SnailNumber? Parent { get; set; }

    public abstract int Magnitude();
    public abstract SnailNumber Clone();
}

public class SnailNumberPair : SnailNumber
{
    public SnailNumberPair(int depth, SnailNumber left, SnailNumber right) : base(depth)
    {
        this.Left = left;
        this.Left.Parent = this;
        this.Right = right;
        this.Right.Parent = this;
    }

    private SnailNumber Left { get; set; }
    private SnailNumber Right { get; set; }

    private void ExplodeLeft()
    {
        SnailNumberLiteral left = this.Left as SnailNumberLiteral ?? throw new InvalidOperationException();
        SnailNumberPair? parent = this.Parent as SnailNumberPair ?? throw new InvalidOperationException("Somehow, I have a depth of 4 but no parent");
        
        SnailNumber current = this;
        while (parent.Left == current)
        {
            current = parent;
            parent = current.Parent as SnailNumberPair;
            if (parent is null)
                return;
        }

        current = parent.Left;
        while (current is not SnailNumberLiteral)
        {
            current = (current as SnailNumberPair)?.Right ?? throw new InvalidOperationException();
        }
        SnailNumberLiteral literal = (current as SnailNumberLiteral)!;
        literal.Value = left.Value + literal.Value;
    }

    private void ExplodeRight()
    {
        SnailNumberLiteral right = this.Right as SnailNumberLiteral ?? throw new InvalidOperationException();
        SnailNumberPair? parent = this.Parent as SnailNumberPair ?? throw new InvalidOperationException("Somehow, I have a depth of 4 but no parent");
        
        SnailNumber current = this;
        while (parent.Right == current)
        {
            current = parent;
            parent = current.Parent as SnailNumberPair;
            if (parent is null)
                return;
        }

        current = parent.Right;
        while (current is not SnailNumberLiteral)
        {
            current = (current as SnailNumberPair)?.Left ?? throw new InvalidOperationException();
        }
        SnailNumberLiteral literal = (current as SnailNumberLiteral)!;
        literal.Value = right.Value + literal.Value;
    }

    private void Explode()
    {
        ExplodeLeft();
        ExplodeRight();
    }

    private void Simplify()
    {
        bool changed = true;
        while (changed)
        {
            if (this.SimplifyByExplosion())
                continue;
            // Console.WriteLine($"= {this}");
            changed = this.SimplifyByReduce();
            // Console.WriteLine($"= {this}");
        }
    }

    private bool SimplifyByExplosion()
    {
        if (this.Left is SnailNumberPair lp)
        {
            if (this.Depth == 3)
            {
                lp.Explode();
                this.Left = new SnailNumberLiteral(this.Left.Depth, 0);
                this.Left.Parent = this;
                return true;
            }

            if (lp.SimplifyByExplosion())
                return true;
        }
        
        if (this.Right is SnailNumberPair rp)
        {
            if (this.Depth == 3)
            {
                rp.Explode();
                this.Right = new SnailNumberLiteral(this.Right.Depth, 0);
                this.Right.Parent = this;
                return true;
            }

            if (rp.SimplifyByExplosion())
                return true;
        }
        
        return false;
    }

    private bool SimplifyByReduce()
    {
        switch (this.Left)
        {
            case SnailNumberLiteral {Value: >= 10} ll:
            {
                double half = ll.Value / 2.0;
                SnailNumberLiteral halfDown = new(ll.Depth + 1, (int) Math.Floor(half));
                SnailNumberLiteral halfUp = new(ll.Depth + 1, (int) Math.Ceiling(half));
                this.Left = new SnailNumberPair(
                    ll.Depth,
                    halfDown,
                    halfUp
                );
                this.Left.Parent = this;
                return true;
            }
            case SnailNumberPair lp:
                if (lp.SimplifyByReduce())
                    return true;
                break;
        }

        switch (this.Right)
        {
            case SnailNumberLiteral {Value: >= 10} rl:
            {
                double half = rl.Value / 2.0;
                SnailNumberLiteral halfDown = new(rl.Depth + 1, (int) Math.Floor(half));
                SnailNumberLiteral halfUp = new(rl.Depth + 1, (int) Math.Ceiling(half));
                this.Right = new SnailNumberPair(
                    rl.Depth,
                    halfDown,
                    halfUp
                );
                this.Right.Parent = this;
                return true;
            }
            case SnailNumberPair rp:
                if (rp.SimplifyByReduce())
                    return true;
                break;
        }

        return false;
    }

    private void UpdateDepth()
    {
        this.Depth = (this.Parent?.Depth ?? -1) + 1;
        switch (this.Left)
        {
            case SnailNumberPair lp:
                lp.UpdateDepth();
                break;
            case SnailNumberLiteral ll:
                ll.Depth = this.Depth + 1;
                break;
        }
        
        
        switch (this.Right)
        {
            case SnailNumberPair rp:
                rp.UpdateDepth();
                break;
            case SnailNumberLiteral rl:
                rl.Depth = this.Depth + 1;
                break;
        }
    }
    
    public static SnailNumberPair operator+(SnailNumberPair first, SnailNumberPair second)
    {
        // Console.WriteLine($"  {first}");
        // Console.WriteLine($"+ {second}");
        SnailNumberPair newSnp = new(0, first.Clone(), second.Clone());
        newSnp.UpdateDepth();
        newSnp.Simplify();
        // Console.WriteLine($"= {newSnp}");
        return newSnp;
    }

    public override int Magnitude()
    {
        return this.Left.Magnitude() * 3 + this.Right.Magnitude() * 2;
    }

    public override SnailNumber Clone()
    {
        return new SnailNumberPair(this.Depth, this.Left.Clone(), this.Right.Clone());
    }

    public override string ToString()
    {
        return $"[{this.Left}, {this.Right}]";
    }
}

public class SnailNumberLiteral : SnailNumber
{
    public SnailNumberLiteral(int depth, int value) : base(depth)
    {
        Value = value;
    }

    public int Value { get; set; }
    public override int Magnitude()
    {
        return Value;
    }

    public override SnailNumber Clone()
    {
        return new SnailNumberLiteral(this.Depth, this.Value);
    }

    public override string ToString()
    {
        return this.Value.ToString();
    }
}

public struct Day18Input
{
    public List<SnailNumberPair> Inputs;
}

public class Day18Solver : AdventOfCodeSolver<Day18Input>
{
    public Day18Solver() : base(18)
    {
    }

    protected override async Task InitializeInputAsync(StreamReader inputReader)
    {
        this.Input = new Day18Input
        {
            Inputs = await AdventOfCodeSolverHelper.ParseEachLineAsync(inputReader, line =>
            {
                Stack<SnailNumber> parsed = new();
                int depth = 0;
                for (int index = 0; index < line.Length; index += 1)
                {
                    ReadOnlySpan<char> current = line.AsSpan(index, 1);
                    switch (current[0])
                    {
                        case '[':
                            depth += 1;
                            break;
                        case ']':
                            depth -= 1;
                            SnailNumber right = parsed.Pop();
                            SnailNumber left = parsed.Pop();
                            parsed.Push(new SnailNumberPair(depth, left, right));
                            break;
                        default:
                            if (char.IsDigit(current[0]))
                            { 
                                parsed.Push(new SnailNumberLiteral(depth, int.Parse(current)));
                            }

                            break;
                    }
                }

                return parsed.Pop() as SnailNumberPair ?? throw new InvalidOperationException("The final snail number after parsing should be a pair no matter what");
            }),
        };
    }

    public override Task SolveProblemOneAsync()
    {
        Console.WriteLine($"Magnitude: {this.Input.Inputs.Skip(1).Aggregate(this.Input.Inputs.First(), (initial, add) => initial + add).Magnitude()}");
        return Task.CompletedTask;
    }

    public override Task SolveProblemTwoAsync()
    {
        int max = 0;
        for (int i = 0; i < this.Input.Inputs.Count; i += 1)
        {
            for (int j = 0; j < this.Input.Inputs.Count; j += 1)
            {
                if (i == j) continue;

                SnailNumberPair sum = this.Input.Inputs[i] + this.Input.Inputs[j];
                int magnitude = sum.Magnitude();
                if (max < magnitude)
                    max = magnitude;
            }
        }
        
        Console.WriteLine($"Max Magnitude of any two snailfish additions: {max}");
        return Task.CompletedTask;
    }
}