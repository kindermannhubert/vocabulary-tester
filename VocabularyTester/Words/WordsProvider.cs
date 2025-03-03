using System.Diagnostics;
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
        while (await source.MoveNextAsync())
        {
            if (!ratedWords.TryGetRating(source.Current, out _))
            {
                break;
            }
        }

        if (ratedWords.AnyNonSuperEasyWords)
        {
            SelectRandomRatedWord();
        }
        else
        {
            SelectCurrentNewWord();
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
        bool nextWillBeAlreadyRated = !ratedWords.HasCapacityForNextUnfamiliarWord;
        if (nextWillBeAlreadyRated)
        {
            SelectRandomRatedWord();
        }
        else
        {
            Debug.Assert(source.Current != null);

            if (await source.MoveNextAsync())
            {
                SelectCurrentNewWord();
            }
            else
            {
                Current = null;
            }
        }
    }

    private void SelectCurrentNewWord()
    {
        Console.WriteLine($"Choosing new word '{source.Current}'.");
        Debug.Assert(source.Current != null);
        Debug.Assert(!ratedWords.TryGetRating(source.Current, out _));
        Current = CreateWord(source.Current, Rating.Unknown);
    }

    private void SelectRandomRatedWord()
    {
        var (word, rating) = ratedWords.GetRandom();
        Console.WriteLine($"Choosing already rated '{word}' / {rating.Familiarness:P1}.");
        Current = CreateWord(word, rating);
    }

    private static Word CreateWord(string word, Rating rating)
    {
        return new Word(word, rating, new MarkupString("TODO"));
    }
}

public record class Word(string Source, Rating Rating, MarkupString Trans);
