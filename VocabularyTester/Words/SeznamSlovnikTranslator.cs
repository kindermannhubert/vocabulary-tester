using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace VocabularyTester.Words;

public partial class SeznamSlovnikTranslator
{
    private readonly HttpClient client ;

    public SeznamSlovnikTranslator(HttpClient client)
    {
        this.client = client;
    }

    public async Task<string?> Translate(string word)
    {
        //return """
        //        <div id="div-id">
        //         <iframe src="https://api.slovnik.seznam.cz/preklad/anglicky_cesky/the" id="iframe-id" scrolling="no"></iframe>
        //        </div>
        //        """;

        try
        {
            var response = await client.GetAsync($"https://corsproxy.io/?url=https://api.slovnik.seznam.cz/preklad/anglicky_cesky/{word}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var html = await response.Content.ReadAsStringAsync();
            //var json = ExtractJson(html);
            //if (json is null)
            //{
            //    return null;
            //}

            //using JsonDocument doc = JsonDocument.Parse(json);
            //var root = doc.RootElement;

            //var translations = root
            //    .GetProperty("props")
            //    .GetProperty("pageProps")
            //    .GetProperty("translations");

            //var count = translations.GetArrayLength();
            //for (int i = 0; i < count; i++)
            //{
            //    var sens = translations[0].GetProperty("sens");
            //    var sensCount = sens.GetArrayLength();
            //    for (int k = 0; k < sensCount; k++)
            //    {
            //        var trans = sens[k].GetProperty("trans");
            //    }
            //}

            var result = ArticleRegex().Match(html).Groups[1].Value; //TODO IsMatch
            result = Regex.Replace(result, "class=\"[^\"]+\"", "");
            result = Regex.Replace(result, "data-dot-data=\"[^\"]+\"", "");
            result = Regex.Replace(result, "lang=\"[^\"]+\"", "");
            return result;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    //private static string? ExtractJson(string html)
    //{
    //    var match = ScriptRegex().Match(html);
    //    return match.Success ? match.Groups[1].Value : null;
    //}

    //[GeneratedRegex("<script id=\"__NEXT_DATA__\"[^>]*>(.*?)</script>", RegexOptions.Singleline)]
    //private static partial Regex ScriptRegex();

    [GeneratedRegex("<article [^>]*>(.*?)</article>", RegexOptions.Singleline)]
    private static partial Regex ArticleRegex();
}
