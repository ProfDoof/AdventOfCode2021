// See https://aka.ms/new-console-template for more information

using System;
using AdventOfCode;

static void PrintDaysSolutions(IDay dayObject)
{
    Console.WriteLine($"Here are my solutions to Day {dayObject.DayOfMonth}:");
    Console.WriteLine("=============================================================");
    Console.WriteLine("Problem 1 Solution:");
    Console.WriteLine("-------------------------------------------------------------");
    dayObject.Problem1();
    Console.WriteLine("-------------------------------------------------------------");
    Console.WriteLine("Problem 2 Solution:");
    Console.WriteLine("-------------------------------------------------------------");
    dayObject.Problem2();
    Console.WriteLine("-------------------------------------------------------------");
    Console.WriteLine("=============================================================");
}

PrintDaysSolutions(await new Day1().InitializeAsync());