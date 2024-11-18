// See https://aka.ms/new-console-template for more information

using QuWordFinder;

var foundWords = WordFinderTester.Test();

Console.WriteLine($"We found these words{Environment.NewLine}{string.Join(",", foundWords)}");

Console.WriteLine("Press any key to exit...");

Console.ReadKey();
