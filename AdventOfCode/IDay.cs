using System.Threading.Tasks;

namespace AdventOfCode;

public interface IDay
{
    public static Task InitializeAsync() {return Task.CompletedTask;}
    public int Day { get; }
    public void Problem1();
    public void Problem2();
}