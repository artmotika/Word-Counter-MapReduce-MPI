using System;
using MPI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;


public class Pair<TF, TS>
{
    public Pair()
    {
    }

    public Pair(TF first, TS second)
    {
        this.First = first;
        this.Second = second;
    }

    public TF First { get; set; }
    public TS Second { get; set; }
};

namespace mpi_problem
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1) throw new Exception("Incorrect number of the arguments");
            MPI.Environment.Run(ref args, communicator =>
            {
                var listOfListOfWords = new List<List<string>>();
                if (communicator.Rank == 0)
                {
                    try
                    {
                        using (StreamReader file = new StreamReader(args[0]))
                        {
                            String line = file.ReadLine();
                            while (line != null)
                            {
                                var words = new List<string>();
                                var separators = new[] {' ', '\t', '\n', '\r'};
                                String[] strOfWords = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var word in strOfWords)
                                {
                                    words.Add(word);
                                }

                                listOfListOfWords.Add(words);
                                line = file.ReadLine();
                            }

                            file.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("The file could not be read:");
                        Console.WriteLine(e.Message);
                    }
                }

                communicator.Broadcast(ref listOfListOfWords, 0);
                var countOfLines = listOfListOfWords.Count / communicator.Size;
                var listOfListOfWordsProc = new List<List<string>>();
                if (communicator.Rank == communicator.Size - 1)
                {
                    for (int i = countOfLines * communicator.Rank;
                         i < countOfLines * (communicator.Rank + 1) +
                         listOfListOfWords.Count % communicator.Size;
                         i++)
                    {
                        listOfListOfWordsProc.Add(listOfListOfWords[i]);
                    }
                }
                else
                {
                    for (int i = countOfLines * communicator.Rank; i < countOfLines * (communicator.Rank + 1); i++)
                    {
                        listOfListOfWordsProc.Add(listOfListOfWords[i]);
                    }
                }

                var allWords = new Dictionary<String, int>();
                foreach (var list in listOfListOfWordsProc)
                {
                    foreach (var pair in Map(list))
                    {
                        allWords[pair.First] = allWords.GetValueOrDefault(pair.First) + pair.Second;
                    }
                }

                var resultAllWords = communicator.Reduce(allWords, Reduce, 0);
                if (communicator.Rank == 0)
                {
                    foreach (var word in resultAllWords.OrderBy(x => x.Key))
                    {
                        Console.WriteLine(word.Key + " " + word.Value);
                    }
                }
            });
        }

        private static Dictionary<String, int> Reduce(Dictionary<String, int> dict1, Dictionary<String, int> dict2)
        {
            var dict = new Dictionary<String, int>();
            foreach (var key in dict1.Keys.Concat(dict2.Keys))
            {
                dict[key] = dict1.GetValueOrDefault(key) + dict2.GetValueOrDefault(key);
            }

            return dict;
        }

        private static List<Pair<String, int>> Map(List<String> words)
        {
            var listOfWords = new List<Pair<String, int>>();
            foreach (var word in words)
            {
                listOfWords.Add(new Pair<String, int>(word, 1));
            }

            return listOfWords;
        }
    }
}