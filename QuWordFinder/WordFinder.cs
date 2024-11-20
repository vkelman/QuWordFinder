using System.Text;

namespace QuWordFinder;

public class WordFinder
{
    private readonly IList<string> _matrix;
    private readonly IList<string> _rotatedMatrix;

    public WordFinder(IEnumerable<string> matrix)
    {
        _matrix = NormalizeMatrix(matrix);
        _rotatedMatrix = GetRotatedMatrix(_matrix); //// Note, that we rotate the matrix only once during class initialization and just keep and use 2 matrices instead of one.
    }

    public IEnumerable<string> Find(IEnumerable<string> wordStream)
    {
        var foundWords = new List<string>();

        var normalizedWordStream = wordStream.Select(word => word.Trim().ToUpper()).Where(word => word.Length > 0).ToList();

        if (normalizedWordStream.Count == 0) return foundWords;

        //// Deduplicate the word stream. According to https://www.bytehide.com/blog/hashset-csharp#:~:text=Use%20HashSet%20when%20the%20order,due%20to%20the%20tree%20structure
        //// and other resources, deduplication using HashSet is faster than Distinct() method. So, it's better to use HashSet for the *large* stream of words.
        var deduplicatedWordStream = new HashSet<string>(normalizedWordStream).ToList();

        var foundWordsDict = new Dictionary<string, int>();

        foundWordsDict = FindAndPopulateWords(foundWordsDict, deduplicatedWordStream, _matrix);
        foundWordsDict = FindAndPopulateWords(foundWordsDict, deduplicatedWordStream, _rotatedMatrix);

        foundWords = foundWordsDict.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).Take(10).ToList();

        return foundWords;
    }

    private static IList<string> NormalizeMatrix(IEnumerable<string> matrix)
    {
        var matrixList = matrix.ToList();

        switch (matrixList.Count)
        {
            case 0:
                throw new ArgumentException("Matrix must not be empty");
            case > Constants.MaxMatrixSize:
                throw new ArgumentException($"Matrix size must be less or equal to {Constants.MaxMatrixSize}");
        }

        //// Normalize matrix: I assume that empty characters should not be present and that comparison is case-insensitive.
        matrixList = matrixList.Select(row => row.Trim().ToUpper()).ToList();

        if (matrixList.Any(row => row.Length != matrixList.Count))
        {
            throw new ArgumentException("Matrix must be square");
        }

        if (matrixList.Any(row => row.Any(letter => !char.IsLetter(letter))))
        {
            throw new ArgumentException("Matrix must contain only letters");
        }

        return matrixList;
    }

    private IList<string> GetRotatedMatrix(IList<string> matrix)
    {
        var rotatedMatrix = new List<string>();

        int matrixSize = matrix.Count;

        //// The maximum size of the matrix is 64x64, so the performance of this algorithm should be acceptable. It's 64x64 iterations at most.
        for (int i = 0; i < matrixSize; i++)
        {
            var verticalWord = new StringBuilder(matrixSize);  //// StringBuilder is supposedly faster than string concatenation.

            for (int j = 0; j < matrixSize; j++)
            {
                verticalWord.Append(matrix[j][i]);
            }

            rotatedMatrix.Add(verticalWord.ToString());
        }

        return rotatedMatrix;
    }

    private static Dictionary<string, int> FindAndPopulateWords(Dictionary<string, int> foundWords, IList<string> words, IList<string> matrix)
    {
        foreach (var word in words)
        {
            if (matrix.Any(row => row.Contains(word)))
            {
                if (foundWords.TryGetValue(word, out int value))
                {
                    foundWords[word] = ++value;
                }
                else
                {
                    foundWords.Add(word, 1);
                }
            }
        }

        return foundWords;
    }
}