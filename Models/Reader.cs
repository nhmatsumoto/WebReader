using System;
using System.Text.RegularExpressions;

namespace WebReader
{
    public class Reader
    {
        protected Guid Id { get; set; }
        protected String Text { get; set; }

        public Reader(String text)
        {
            Id = Guid.NewGuid();
            Text = text;
        }

        public void Print(IEnumerable<String> words, int timeRate)
        {
            int counter = words.Count();
            string[] nextWord = words.ToArray();

            for (int i = 0; i <= counter - 1; i++)
            {
                Thread.Sleep(timeRate);
                Console.WriteLine($"{nextWord[i]}");
            }

            Console.WriteLine("FIM!");
        }
        public IEnumerable<String> GetWords() => Regex.Split(Text, @"[^\w0-9-]+").Where(x => !String.IsNullOrWhiteSpace(x));
    }
}



