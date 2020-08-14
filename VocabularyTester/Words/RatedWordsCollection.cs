namespace VocabularyTester.Words;

public class RatedWordsCollection
{
    private readonly Dictionary<string, Rating> ratedWords = new();

    public RatedWordsCollection()
    {
        //TODO load


    }

    public bool TryGetRating(string word, out Rating rating) => ratedWords.TryGetValue(word, out rating);

    public void Add(string word, Rating rating)
    {
        if (ratedWords.TryGetValue(word, out var existingRating))
        {
            ratedWords[word] = new Rating(0.5 * (existingRating.Value + rating.Value));
        }
        else
        {
            ratedWords.Add(word, rating);
        }
    }
}
