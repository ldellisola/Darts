using Darts.Entities;

namespace Darts.Core;

public static class DartsCombinations
{
    private static readonly int[] Scores = { 25, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
    private static readonly int[] Multipliers = { 1, 2, 3 };
    private static readonly int BullseyeScore = 50;
    private const int MaxShots = 3;

    public static List<DartsThrow> FindCombinations(int currentScore, int goal, int shotNumber, int maxCombinations)
    {
        var combinations = new List<List<(int, int)>>();
        FindCombinationsRecursive(goal - currentScore, new List<(int, int)>(), combinations, shotNumber, maxCombinations, 0);
        return combinations.Select(t=> new DartsThrow(t)).ToList();
    }

    private static void FindCombinationsRecursive(int target, List<(int Score, int Multiplier)> currentCombination, List<List<(int, int)>> combinations, int shotNumber, int maxCombinations, int scoreStartIndex)
    {
        if (combinations.Count >= maxCombinations) return;

        if (target == 0 || shotNumber == MaxShots)
        {
            if (target == 0)
            {
                var sortedCombination = currentCombination.OrderBy(tuple => tuple.Score * 10 + tuple.Multiplier).ToList();  // Custom sort
                if (!combinations.Any(comb => comb.SequenceEqual(sortedCombination)))
                {
                    combinations.Add(sortedCombination);
                }
            }
            return;
        }

        if (target < 0) return;

        for (int i = scoreStartIndex; i < Scores.Length; i++)
        {
            var score = Scores[i];

            foreach (var multiplier in Multipliers)
            {
                if (score == 25 && multiplier == 3) continue;
                int value = score * multiplier;
                currentCombination.Add((score, multiplier));
                FindCombinationsRecursive(target - value, currentCombination, combinations, shotNumber + 1, maxCombinations, i);
                currentCombination.RemoveAt(currentCombination.Count - 1); // backtrack
            }
        }

        // Handling inner bullseye
        if (target >= BullseyeScore)
        {
            currentCombination.Add((BullseyeScore / 2, 2));  // Representing inner bullseye as 25x2
            FindCombinationsRecursive(target - BullseyeScore, currentCombination, combinations, shotNumber + 1, maxCombinations, Scores.Length - 1);
            currentCombination.RemoveAt(currentCombination.Count - 1); // backtrack
        }
    }
}
