namespace VocabularyTester.Words;

public class WordsDataSource
{
    private readonly List<string> orderedSourceByAscendingFreq = new();
    private readonly Dictionary<string, List<string>> dictionary = new(StringComparer.InvariantCultureIgnoreCase);

    private WordsDataSource() { }

    public static async Task<WordsDataSource> Create(HttpClient client)
    {
        var result = new WordsDataSource();

        var data = await client.GetStringAsync("dictionaries/en-cs_sortedByFreq.txt");
        foreach (var record in SplitEnumerate(data))
        {
            var split = record.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            var word = split[0];
            var translations = split.Skip(1).ToList();
            result.dictionary.Add(word, translations);
            result.orderedSourceByAscendingFreq.Add(word);
        }

        result.orderedSourceByAscendingFreq.Reverse();
        return result;
    }

    public string? NextWord => orderedSourceByAscendingFreq is [.., var word] ? word : null;

    public void Remove(string word) => orderedSourceByAscendingFreq.Remove(word);

    public List<string> Translate(string word) => dictionary[word];

    private static IEnumerable<string> SplitEnumerate(string data)
    {
        var start = 0;
        while (true)
        {
            if (start >= data.Length - 1)
            {
                yield break;
            }
            var span = data.AsSpan(start);
            var end = span.IndexOf('\n');
            if (end == -1)
            {
                yield break;

            }
            yield return span[..end].ToString();
            start += end + 1;
        }
    }
}
