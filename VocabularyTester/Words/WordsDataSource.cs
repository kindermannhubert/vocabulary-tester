namespace VocabularyTester.Words;

public class WordsDataSource
{
    private readonly HttpClient client;

    public WordsDataSource(HttpClient client)
    {
        this.client = client;
    }

    public async IAsyncEnumerable<string> Enumerate()
    {
        var data = await client.GetStringAsync("dictionaries/en-cs.txt");
        foreach (var record in SplitEnumerate(data))
        {
            var firstTabIdx = record.IndexOf('\t', StringComparison.Ordinal);
            var word = record[..firstTabIdx];
            yield return word.ToString();
        }
    }

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
