using System;
using System.Text.RegularExpressions;
public class Reader
{
    protected Guid Id { get; set; }
    protected String Text { get; set; }

    public Reader(String text)
    {
        Id = Guid.NewGuid();
        Text = text;
    }

    public static async Task Print(IEnumerable<String> words, int timeRate)
    {
        int counter = words.Count();
        int index = 0;
        string[] nextWord = words.ToArray();

        while (counter < index)
        {
            Thread.Sleep(timeRate);
            Console.WriteLine($"{nextWord[index]}");
            index++;
        }

        Console.WriteLine("FIM!");
    }
    public IEnumerable<String> GetWords() => Regex.Split(Text, @"[^\w0-9-]+").Where(x => !String.IsNullOrWhiteSpace(x));

}
