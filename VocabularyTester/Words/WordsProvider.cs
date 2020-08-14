using System.Diagnostics;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace VocabularyTester.Words;

public class WordsProvider
{
    private readonly WordsDataSource source;
    private readonly RatedWordsCollection ratedWords;
    private readonly Func<string, List<string>> translate;

    public WordCard? Current { get; private set; }

    public WordsProvider(WordsDataSource source, ILocalStorageService localStorage)
    {
        this.source = source;
        this.ratedWords = new RatedWordsCollection(localStorage);
        this.translate = source.Translate;
    }

    public async Task Initialize()
    {
        await ratedWords.Initialize();

        foreach (var ratedWord in ratedWords.EnumerateRatedWords())
        {
            source.Remove(ratedWord);
        }

        if (ratedWords.NonSuperEasyWordsCount > 0)
        {
            SelectRandomRatedWord();
        }
        else
        {
            SelectNewWord();
        }
    }

    public async Task RateCurrentWord(Rating rating)
    {
        if (Current != null)
        {
            await ratedWords.Add(Current.Word, rating);
            source.Remove(Current.Word);
        }
    }

    public void MoveToNextWord()
    {
        if (ratedWords.NonSuperEasyWordsCount > 1 &&
            (!ratedWords.CanAddNewUnfamiliarWord || Random.Shared.NextDouble() >= 0.5))
        {
            SelectRandomRatedWord();
        }
        else
        {
            if (source.NextWord != null)
            {
                SelectNewWord();
            }
            else
            {
                Current = null;
            }
        }
    }

    private void SelectNewWord()
    {
        Console.WriteLine($"Choosing new word '{source.NextWord}'.");
        Debug.Assert(source.NextWord != null);
        Debug.Assert(!ratedWords.TryGetRating(source.NextWord, out _));
        Current = CreateWord(source.NextWord, source.Translate(source.NextWord), Rating.Unknown);
    }

    private void SelectRandomRatedWord()
    {
        var (word, rating) = ratedWords.GetRandom(Current?.Word);
        Console.WriteLine($"Choosing already rated '{word}' / {rating.Familiarness:P1}.");
        Current = CreateWord(word, translate(word), rating);
    }

    private WordCard CreateWord(string word, List<string> translations, Rating rating)
    {
        return new WordCard(
            word,
            rating,
            new MarkupString($"<div>{string.Join("; ", translations)}</div>"));
    }
}

public record class WordCard(string Word, Rating Rating, MarkupString Trans);
