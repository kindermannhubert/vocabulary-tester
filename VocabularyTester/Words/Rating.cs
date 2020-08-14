namespace VocabularyTester.Words;

public readonly struct Rating
{
    public static Rating SuperEasy => new(1);
    public static Rating Easy => new(0.95);
    public static Rating Medium => new(0.5);
    public static Rating Hard => new(0.1);
    public static Rating Unknown => new(0);

    public readonly double Value;

    public Rating(double value)
    {
        Value = value;
    }
}
