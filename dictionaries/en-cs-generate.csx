using System;
using System.IO;
using System.Text;

System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
ulong totalCount = 0;
var records = new List<(string Word, ulong Count)>();
foreach (var line in File.ReadAllLines("en-cs_source.txt").Take(10000))
{
    var split = line.Split('\t');
    var record = (Word: split[0], Count: ulong.Parse(split[1]));
    records.Add(record);
    totalCount += record.Count;
}

/*
var sb = new StringBuilder();
sb.AppendLine("{");
foreach (var (word, count) in records)
{
    sb.AppendLine($"\t{");
    sb.AppendLine($"\t\t{word}\t{(double)count / totalCount:F4}");
    sb.AppendLine($"\t}");
}
sb.AppendLine("}");

File.WriteAllText("en-cs.json", sb.ToString());
*/
File.WriteAllText(
    "en-cs.txt", 
    string.Join('\n', records.Select(r => $"{r.Word}\t{(double)r.Count / totalCount:F6}")));