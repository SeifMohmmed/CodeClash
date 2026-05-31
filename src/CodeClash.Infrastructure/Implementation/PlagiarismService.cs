using System.Text.RegularExpressions;
using CodeClash.Application.Abstractions.Plagiarism;
using CodeClash.Application.DTO;
using CodeClash.Application.Mapping;
using CodeClash.Domain.Abstractions;

namespace CodeClash.Infrastructure.Implementation;

/// <summary>
/// Detects potential plagiarism between accepted submissions
/// using N-Grams, FNV-1 hashing, Winnowing fingerprinting,
/// and Jaccard similarity.
///
/// Reference: Fowler–Noll–Vo Hash Function specification.
/// https://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function#FNV_hash_parameters
/// </summary>
internal sealed partial class PlagiarismService(
    ISubmissionRepository submissionRepository) : IPlagiarismService
{
    // FNV-1 prime constant used for hashing.
    private const uint FNV_PRIME = 16777619;

    // Initial offset basis for the FNV-1 hash algorithm.
    private const uint OFFSET_BASIS = 2166136261;

    /// Size of each N-Gram token.
    /// Ex: N = 6 => "abcdefghi" => "abcdef", "bcdefg", "cdefgh", "defghi".
    private const uint N = 6;

    // Winnowing sliding window size.
    // The minimum hash in each window becomes a fingerprint.
    private const uint WINDOW_SIZE = 5;

    /// <summary>
    /// Compares accepted submissions belonging to the same problem
    /// and reports pairs whose similarity exceeds the given threshold.
    /// </summary>
    public async Task<IEnumerable<PlagiarismCaseDto>> GetPlagiarismCases(
        Guid contestId,
        List<Guid> ProblemIds,
        decimal threshold)
    {
        // Get all accepted submissions for the selected contest problems.
        var submissions = await submissionRepository
            .GetContestACSubmissionsByProblemIdsAsync(contestId, ProblemIds);

        var plagiarismCases = new List<PlagiarismCaseDto>();

        // Compare submissions only within the same problem.
        var groups = submissions.GroupBy(s => s.ProblemId);

        foreach (var group in groups)
        {
            var subList = group.ToList();

            // Compare every unique pair once.
            // Example: A-B, A-C, B-C
            for (var i = 0; i < subList.Count; i++)
            {
                for (int j = i + 1; j < subList.Count; j++)
                {
                    // Ignore comparisons from the same user.
                    if (subList[i].UserId == subList[j].UserId)
                    {
                        continue;
                    }

                    var similarity = CalculateJaccardSimilarity(
                        subList[i].Code,
                        subList[j].Code);

                    // Record the pair if similarity exceeds threshold.
                    if (similarity >= threshold)
                    {
                        plagiarismCases.Add(new PlagiarismCaseDto
                        {
                            FirstSubmission = subList[i].ToDto(),
                            SecondSubmission = subList[j].ToDto(),
                            Similarity = similarity,
                            ProblemId = subList[i].ProblemId
                        });
                    }
                }
            }
        }

        return plagiarismCases;
    }

    /// <summary>
    /// Computes similarity percentage between two source codes.
    /// The comparison is performed on Winnowing fingerprints
    /// using the Jaccard similarity metric.
    /// </summary>
    private decimal CalculateJaccardSimilarity(
        string code1,
        string code2)
    {
        // Normalize code to reduce the impact of formatting differences.
        code1 = PreProcess(code1);
        code2 = PreProcess(code2);

        // Break source code into overlapping N-Grams.
        var ngrams1 = GenerateN_Grams(code1);
        var ngrams2 = GenerateN_Grams(code2);

        // Convert N-Grams into numeric hashes.
        var hashes1 = HashN_grams(ngrams1);
        var hashes2 = HashN_grams(ngrams2);

        // Select representative hashes using the Winnowing algorithm.
        var fingerprints1 = GetFingerPrints(hashes1);
        var fingerprints2 = GetFingerPrints(hashes2);

        // Jaccard = |Intersection| / |Union|
        var intersection = fingerprints1.Intersect(fingerprints2).Count();
        var union = fingerprints1.Union(fingerprints2).Count();

        return (decimal)intersection / union * 100;
    }

    /// <summary>
    /// Normalizes source code before comparison.
    /// - Converts to lowercase.
    /// - Removes comments.
    /// - Removes whitespace.
    /// </summary>
    private static string PreProcess(
        string code)
    {
        code = code.ToLower();

        // Remove single-line and multi-line comments.
        code = Regex.Replace(
            code,
            @"(//.*?$)|(/\*.*?\*/)", "",
            RegexOptions.Multiline);

        // Remove all whitespace characters.
        code = Regex.Replace(code, @"\s+", "");

        return code;
    }

    /// <summary>
    /// Generates overlapping character N-Grams.
    /// Example:
    /// Input  : "abcdef"
    /// Output : ["abc", "bcd", "cde", "def"]
    /// </summary>
    private static List<string> GenerateN_Grams(
        string code)
    {
        var ngrams = new List<string>();
        for (int i = 0; i < code.Length - N + 1; i++)
        {
            ngrams.Add(code.Substring(i, (int)N));
        }
        return ngrams;
    }
    /// <summary>
    /// Computes a 32-bit Fnv1a hash for an N-Gram.
    ///
    /// The hash converts a string into a compact numeric value,
    /// allowing faster storage and comparison during plagiarism detection.
    ///
    /// Example:
    /// "abc" -> 440920331
    /// </summary>
    private static uint Fnv1a(string word)
    {
        // Start with the standard FNV offset basis.
        uint hash = OFFSET_BASIS;
        foreach (var c in word)
        {
            // Mix the current character into the hash value.
            hash ^= c;

            // Multiply by the FNV prime to spread the bits
            // and reduce the chance of collisions.
            hash *= FNV_PRIME;
        }

        // Return the final hash representing the entire string.
        return hash;
    }

    /// <summary>
    /// Converts all generated N-Grams into FNV-1 hash values.
    /// Working with numeric hashes is more efficient than
    /// comparing raw string N-Grams.
    /// </summary>
    private static List<uint> HashN_grams(List<string> ngrams)
    {
        var hashes = new List<uint>();
        foreach (var ngram in ngrams)
        {
            // Generate a numeric hash for the current N-Gram.
            hashes.Add(Fnv1a(ngram));
        }
        return hashes;
    }

    /// <summary>
    /// Applies the Winnowing algorithm.
    /// Instead of comparing all hashes, Winnowing keeps only
    /// representative hashes (fingerprints).
    /// For each sliding window, the minimum hash is selected.
    /// This dramatically reduces comparison cost while preserving
    /// similarity detection accuracy.
    /// </summary>
    private static List<uint> GetFingerPrints(List<uint> hashs)
    {
        var fingerPrints = new List<uint>();

        // Stores hashes in sorted order so the minimum hash
        // can be retrieved in O(1) using window.Min.
        var window = new SortedSet<(uint value, uint index)>();

        // Tracks already selected fingerprints to avoid duplicates.
        var count = new Dictionary<(uint, uint), uint>();

        uint i = 0;

        // Build the initial window.
        for (; i < WINDOW_SIZE; i++)
        {
            window.Add((hashs[(int)i], i));
        }

        // The minimum hash of the first window is the first fingerprint.
        fingerPrints.Add(window.Min.value);

        count[window.Min] = 1;

        // Slide the window one hash at a time.
        for (; i < hashs.Count; i++)
        {
            // Add new hash entering the window.
            window.Add((hashs[(int)i], i));

            // Remove hash leaving the window.
            window.Remove((hashs[(int)(i - WINDOW_SIZE)], i - WINDOW_SIZE));

            if (!count.TryGetValue(window.Min, out uint value))
            {
                value = 0;
                count[window.Min] = value;
            }

            count[window.Min] = ++value;

            // Add a fingerprint only the first time it becomes
            // the minimum hash of a window.
            if (value == 1)
            {
                fingerPrints.Add(window.Min.value);
            }
        }

        return fingerPrints;
    }

    public decimal GetSimilarity(
        string code1,
        string code2)
    {
        return CalculateJaccardSimilarity(code1, code2);
    }
}
