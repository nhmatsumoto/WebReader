using System;
using System.Text.RegularExpressions;

namespace Services.WebReader
{
    public class Reader
    {
        protected String Text { get; set; }

        public Reader(String text) => Text = text;

        public void Print(int timeRate)
        {
            IEnumerable<String> words = GetWords();
            int counter = words.Count();
            string[] nextWord = words.ToArray();

            for (int i = 0; i <= counter - 1; i++)
            {
                Thread.Sleep(timeRate);
                Console.WriteLine($"{nextWord[i]}");
            }

            Thread.Sleep(timeRate);
            Console.WriteLine("\nFIM!");
        }
        protected IEnumerable<String> GetWords() => Regex.Split(Text, @"[^\w0-9-]+").Where(x => !String.IsNullOrWhiteSpace(x));
    }
}



