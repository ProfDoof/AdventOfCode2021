using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AdventOfCode;

public static class AdventOfCodeSolverHelper
{
    public static async Task RunSolver<TSolver>() where TSolver : AdventOfCodeAbstractSolver, new()
    {
        IAdventOfCodeSolver solver = await new TSolver().InitializeAsync();
        Console.WriteLine($"Here are my solutions to Day {solver.DayOfMonth}:");
        Console.WriteLine("=============================================================");
        Console.WriteLine("Problem 1 Solution:");
        Console.WriteLine("-------------------------------------------------------------");
        await solver.SolveProblemOneAsync();
        Console.WriteLine("-------------------------------------------------------------");
        Console.WriteLine("Problem 2 Solution:");
        Console.WriteLine("-------------------------------------------------------------");
        await solver.SolveProblemTwoAsync();
        Console.WriteLine("-------------------------------------------------------------");
        Console.WriteLine("=============================================================");
    }

    public static async Task RunSolvers(params Type[] types)
    {
        await RunSolvers(types.ToList());
    }

    private static async Task RunSolvers(IEnumerable<Type> types)
    {
        MethodInfo runSolverMethod = typeof(AdventOfCodeSolverHelper).GetMethod("RunSolver")!;
        foreach (Type type in types)
        {
            try
            {
                MethodInfo currentRunner = runSolverMethod.MakeGenericMethod(type);
                await (Task) currentRunner.Invoke(null, Array.Empty<object>())!;
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"The provided type {type} does not inherit from the AdventOfCodeAbstractSolver");
            }
        }
    }

    public static async Task RunAllSolvers(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetExecutingAssembly();
        await RunSolvers(assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(AdventOfCodeAbstractSolver)) && type != typeof(AdventOfCodeSolver<>)));
    }

    public static async Task<List<TInput>> ParseEachLineAsync<TInput>(StreamReader inputReader, Func<string, TInput> parserFunc)
    {
        List<TInput> inputs = new();
        while (!inputReader.EndOfStream)
        {
            string? input = await inputReader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(input))
                inputs.Add(parserFunc(input.Trim()));
        }

        return inputs;
    }
}