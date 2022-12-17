namespace SimpleDependencyInjection;

public record class Test1 : ITest
{
    public int Do(int a, int b)
    {
        return a + b;
    }
}

public record class Test2 : ITest
{
    public int Do(int a, int b)
    {
        return a * b;
    }
}