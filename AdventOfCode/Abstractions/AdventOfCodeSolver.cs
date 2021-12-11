using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode;

public interface IAdventOfCodeSolver
{
    public int DayOfMonth { get; }
    public Task SolveProblemOneAsync();
    public Task SolveProblemTwoAsync();
}

public abstract class AdventOfCodeAbstractSolver : IAdventOfCodeSolver
{
    public int DayOfMonth { get; }

    protected AdventOfCodeAbstractSolver(int dayOfMonth)
    {
        this.DayOfMonth = dayOfMonth;
    }

    public async Task<IAdventOfCodeSolver> InitializeAsync()
    {
        await using FileStream inputFileStream =
            new($"./Inputs/day{this.DayOfMonth}input.txt", FileMode.Open, FileAccess.Read);
        using StreamReader inputFileReader = new(inputFileStream);
        await InitializeInputAsync(inputFileReader);
        return this;
    }

    protected abstract Task InitializeInputAsync(StreamReader inputReader);

    public abstract Task SolveProblemOneAsync();
    public abstract Task SolveProblemTwoAsync();
}

public abstract class AdventOfCodeSolver<TInputStruct> : AdventOfCodeAbstractSolver where TInputStruct : struct
{
    protected AdventOfCodeSolver(int dayOfMonth) : base(dayOfMonth)
    {
    }
    
    protected TInputStruct Input;
    protected abstract override Task InitializeInputAsync(StreamReader inputReader);
}