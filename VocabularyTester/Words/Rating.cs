using System.Diagnostics;

namespace VocabularyTester.Words;

[DebuggerDisplay("Familiarness: {Familiarness:P1}")]
public readonly struct Rating
{
    public static Rating SuperEasy => new(1, true);

    public static Rating Easy => new(1, false); //Needs convergence to 1.
    private static Rating FirstTimeEasy => new(0.9, false);

    public static Rating Medium => new(0.5, false);
    public static Rating Hard => new(0.1, false);
    public static Rating Unknown => new(0, false);

    public bool IsSuperEasy { get; }
    public double Familiarness { get; }
    public double Unfamiliarness => 1 - Familiarness;

    public Rating(double value)
        : this(value, false)
    { }

    private Rating(double value, bool superEasy)
    {
        Familiarness = value;
        IsSuperEasy = superEasy;
    }

    public Rating GetFirstTimeDiscount()
    {
        // We rate the word for the first time.
        if (!IsSuperEasy && Familiarness == Easy.Familiarness)
        {
            // Discount. Otherwise it would be same as SuperEasy.
            return FirstTimeEasy;
        }
        return this;
    }
}
