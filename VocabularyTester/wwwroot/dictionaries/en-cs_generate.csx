using System;
using System.IO;
using System.Text;

System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

var enWords = new List<string>();
foreach (var line in File.ReadAllLines("source-data\\en.txt").Take(10000))
{
    var en = line.Split('\t')[0];
    
    var enLower = en.ToLower();
    if (enLower.Length == 1 && en is not ("a" or "i")) continue;

    enWords.Add(en);
}

var enCsDict = new Dictionary<string, List<string>>(StringComparer.InvariantCultureIgnoreCase);
foreach (var line in File.ReadAllLines("source-data\\en-cs.txt"))
{
    var split = line.Split('\t');
    var en = split[0];
    var cs = split[1];
    if (enCsDict.TryGetValue(en, out var csList))
    {
        csList.Add(cs);
    }
    else
    {
        enCsDict.Add(en, [cs]);
    }
}

File.WriteAllLines(
    "en-cs_sortedByFreq.txt",
    enWords.Where(enCsDict.ContainsKey)
        .Select(
            en =>
            {
                return $"{en}\t{string.Join("\t", enCsDict[en])}";
            }));