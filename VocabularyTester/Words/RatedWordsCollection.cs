namespace VocabularyTester.Words;

public class RatedWordsCollection
{
    public const double MaxTotalUnfamiliarness = 1;

    private readonly HashSet<string> superEasyWords = [];
    private readonly Dictionary<string, Rating> wordToRating = [];
    private readonly List<string> ratedWords = [];
    private double totalUnfamiliarness;

    public RatedWordsCollection()
    {
        //TODO load - pouzivat Add


    }

    public bool AnyNonSuperEasyWords => ratedWords.Count > 0;

    public bool HasCapacityForNextUnfamiliarWord => totalUnfamiliarness <= MaxTotalUnfamiliarness;

    public bool TryGetRating(string word, out Rating rating)
    {
        if (superEasyWords.Contains(word))
        {
            rating = Rating.SuperEasy;
            return true;
        }

        return wordToRating.TryGetValue(word, out rating);
    }

    public void Add(string word, Rating rating)
    {
        if (rating.IsSuperEasy)
        {
            superEasyWords.Add(word);
            wordToRating.Remove(word);
            ratedWords.Remove(word);
        }
        else if (wordToRating.TryGetValue(word, out var existingRating))
        {
            totalUnfamiliarness -= existingRating.Unfamiliarness;
            rating = new Rating(0.5 * (existingRating.Familiarness + rating.Familiarness));
            wordToRating[word] = rating;
        }
        else
        {
            rating = rating.GetFirstTimeDiscount();
            wordToRating.Add(word, rating);
            ratedWords.Add(word);
        }

        totalUnfamiliarness += rating.Unfamiliarness;
        Console.WriteLine($"TotalUnfamiliarness: {totalUnfamiliarness}");
    }

    public (string Word, Rating Rating) GetRandom()
    {
        var word = ratedWords[Random.Shared.Next(ratedWords.Count)];
        return (word, wordToRating[word]);
    }
}
