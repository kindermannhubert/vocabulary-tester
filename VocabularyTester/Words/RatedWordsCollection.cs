using System.Diagnostics;
using Blazored.LocalStorage;

namespace VocabularyTester.Words;

public class RatedWordsCollection
{
    public const double LearnedWordLimit = 0.8;
    public const int MaxUnlearnedWordCount = 5;
    public const string SerializationKey = "WordRatings";

    private readonly HashSet<string> superEasyWords = [];
    private readonly Dictionary<string, Rating> wordToRating = [];
    private readonly List<string> ratedWords = [];
    private readonly ILocalStorageService localStorage;
    private int unlearnedWords;
    private double totalUnfamiliarness;

    public RatedWordsCollection(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async Task Initialize()
    {
        if (await localStorage.ContainKeyAsync(SerializationKey))
        {
            var data = await localStorage.GetItemAsync<SerializationData>(SerializationKey);

            if (data != null)
            {
                Console.WriteLine($"Loading data from local storage: {data.SuperEasyWords.Count} super-easy words + {data.WordToRating.Count} rated words");

                foreach (var superEasyWord in data.SuperEasyWords)
                {
                    AddWithoutExport(superEasyWord, Rating.SuperEasy);
                }

                foreach (var (word, rating) in data.WordToRating)
                {
                    AddWithoutExport(word, new Rating(rating));
                }

                Console.WriteLine("Loading done");
            }
        }
    }

    public int NonSuperEasyWordsCount => ratedWords.Count;

    public bool CanAddNewUnfamiliarWord => unlearnedWords <= MaxUnlearnedWordCount;

    public bool TryGetRating(string word, out Rating rating)
    {
        if (superEasyWords.Contains(word))
        {
            rating = Rating.SuperEasy;
            return true;
        }

        return wordToRating.TryGetValue(word, out rating);
    }

    public IEnumerable<string> EnumerateRatedWords() => superEasyWords.Concat(wordToRating.Keys);

    public async Task Add(string word, Rating rating)
    {
        AddWithoutExport(word, rating);
        await localStorage.SetItemAsync(SerializationKey, new SerializationData(superEasyWords, wordToRating.ToDictionary(kv => kv.Key, kv => kv.Value.Familiarness)));
    }

    private void AddWithoutExport(string word, Rating rating)
    {
        var existingRating = Rating.Unknown;
        if (wordToRating.TryGetValue(word, out existingRating))
        {
            if (!IsLearned(existingRating))
            {
                unlearnedWords--;
            }
            totalUnfamiliarness -= existingRating.Unfamiliarness;

            if (rating.IsSuperEasy)
            {
                superEasyWords.Add(word);
                wordToRating.Remove(word);
                ratedWords.Remove(word);
            }
            else
            {
                rating = new Rating(0.5 * (existingRating.Familiarness + rating.Familiarness));
                wordToRating[word] = rating;
            }
        }
        else
        {
            if (rating.IsSuperEasy)
            {
                superEasyWords.Add(word);
            }
            else
            {
                rating = rating.GetFirstTimeDiscount();
                wordToRating.Add(word, rating);
                ratedWords.Add(word);
            }
        }

        if (!IsLearned(rating))
        {
            unlearnedWords++;
        }

        totalUnfamiliarness += rating.Unfamiliarness;
        Console.WriteLine($"'{word}' {existingRating.Familiarness:P1}->{rating.Familiarness:P1}: UnlearnedWords={unlearnedWords}; TotalUnfamiliarness={totalUnfamiliarness}");
    }

    public (string Word, Rating Rating) GetRandom(string? currentWord)
    {
        Debug.Assert(ratedWords.Count > 0);

        var r = totalUnfamiliarness * Random.Shared.NextDouble();

        double unfamiliarnessSum = 0;
        foreach (var (word, rating) in wordToRating)
        {
            unfamiliarnessSum += rating.Unfamiliarness;
            if (word != currentWord &&
                unfamiliarnessSum >= r)
            {
                return (word, rating);
            }
        }

        return (ratedWords[^1], wordToRating[ratedWords[^1]]);
    }

    private static bool IsLearned(Rating rating) => rating.Familiarness >= LearnedWordLimit;

    public class SerializationData
    {
        public SerializationData(HashSet<string> superEasyWords, Dictionary<string, double> wordToRating)
        {
            SuperEasyWords = superEasyWords;
            WordToRating = wordToRating;
        }

        public HashSet<string> SuperEasyWords { get; set; } = [];
        public Dictionary<string, double> WordToRating { get; set; } = [];
    }
}
