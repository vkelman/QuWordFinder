namespace QuWordFinder;

public static class WordFinderTester
{
    private static readonly IList<string> Matrix = new List<string>
    {
        "abcdca",
        "fgwiob",
        "chillc",
        "pqnsdd",
        "uvdxye",
        "coldyf"
    };

    private static readonly IList<string> Words = new List<string>
    {
        "cold",
        "wind",
        "wind",
        "snow",
        "chill"
    };


    public static IList<string> Test()
    {
        var wordFinder = new WordFinder(Matrix);
        var foundWords = wordFinder.Find(Words);

        return foundWords.ToList();
    }
}
