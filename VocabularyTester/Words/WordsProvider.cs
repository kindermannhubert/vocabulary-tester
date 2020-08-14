using Microsoft.AspNetCore.Components;

namespace VocabularyTester.Words;

public class WordsProvider
{
    private readonly IAsyncEnumerator<string> source;
    private readonly RatedWordsCollection ratedWords;

    public Word? Current { get; private set; }

    public WordsProvider(WordsDataSource source)
    {
        this.source = source.Enumerate().GetAsyncEnumerator();
        ratedWords = new RatedWordsCollection();
    }

    public async Task Initialize()
    {
        while (true)
        {
            await MoveToNextWord();
            if (Current is null ||
                !ratedWords.TryGetRating(Current.Source, out _))
            {
                break;
            }
        }
    }

    public void RateCurrentWord(Rating rating)
    {
        if (Current != null)
        {
            ratedWords.Add(Current.Source, rating);
        }
    }

    public async Task MoveToNextWord()
    {
        if (await source.MoveNextAsync())
        {
            ratedWords.TryGetRating(source.Current, out var rating);
            Current = new Word(source.Current, rating, new MarkupString("TODO"));
        }
        else
        {
            Current = null;
        }
    }
}

public record class Word(string Source, Rating Rating, MarkupString Trans);
