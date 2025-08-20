namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public class Rating
{
    public double Rate { get; private set; }
    public int Count { get; private set; }

    public Rating(double rate, int count)
    {
        Rate = rate;
        Count = count;
    }
}
