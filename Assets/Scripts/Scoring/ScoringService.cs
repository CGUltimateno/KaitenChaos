using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-03
// AI assistance: Claude (autocomplete, architecture review)

public static class ScoringService
{
    public static int ComputeCravingMatch(Dish dish, Ticket ticket)
    {
        int score = 0;

        foreach (string id in dish.chosenIngredientIds)
        {
            IngredientData ingredient = GameDatabase.Instance.GetIngredient(id);
            if (ingredient != null && ingredient.traits != null)
            {
                score += ingredient.traits.GetValue(ticket.cravingTrait);
            }
        }

        int bestPossible = GameDatabase.Instance.GetBestAchievable(ticket.cravingTrait, ticket.requestedCount);

        if (bestPossible <= 0)
            return 0;

        int calculatedScore = (score * 100) / bestPossible;
        if (calculatedScore < 0) calculatedScore = 0;
        if (calculatedScore > 100) calculatedScore = 100;

        return calculatedScore;
    }

    public static int BindTicketToDish(Dish dish, Ticket ticket)
    {
        dish.boundTicket = ticket;
        int score = ComputeCravingMatch(dish, ticket);
        dish.cravingMatchScore = score;
        return score;
    }

    public static int ComputeFillingScore(Dish dish, float lineStartX, float lineWidth)
    {
        int totalIngredients = dish.chosenIngredientIds.Count;
        if (totalIngredients == 0)
            return dish.ricePlaced ? 100 : 0;

        List<float> placedX = new List<float>();
        foreach (string id in dish.chosenIngredientIds)
        {
            if (dish.fillingPlacements.TryGetValue(id, out Vector3 pos))
                placedX.Add(pos.x);
        }
        placedX.Sort();

        float totalDeviation = 0f;
        for (int i = 0; i < placedX.Count; i++)
        {
            float idealX = lineStartX + (i + 1) * (lineWidth / (totalIngredients + 1));
            totalDeviation += Mathf.Abs(placedX[i] - idealX);
        }

        int missing = totalIngredients - placedX.Count;
        totalDeviation += missing * lineWidth;

        float maxPossibleDeviation = totalIngredients * lineWidth;
        float placementAccuracy = maxPossibleDeviation > 0
            ? 1f - Mathf.Clamp01(totalDeviation / maxPossibleDeviation)
            : 1f;

        float overall = (placementAccuracy * totalIngredients + (dish.ricePlaced ? 1 : 0)) / (totalIngredients + 1);
        return Mathf.RoundToInt(overall * 100f);
    }

    public static int ComputeRollCutScore(List<float> cutPositionsX, float lineStartX, float lineWidth, int expectedCutCount)
    {
        if (expectedCutCount == 0)
            return 100;

        List<float> sorted = new List<float>(cutPositionsX);
        sorted.Sort();

        float totalDeviation = 0f;
        for (int i = 0; i < sorted.Count; i++)
        {
            float idealX = lineStartX + (i + 1) * (lineWidth / (expectedCutCount + 1));
            totalDeviation += Mathf.Abs(sorted[i] - idealX);
        }

        int missing = expectedCutCount - sorted.Count;
        totalDeviation += missing * lineWidth;

        float maxPossibleDeviation = expectedCutCount * lineWidth;
        float accuracy = maxPossibleDeviation > 0
            ? 1f - Mathf.Clamp01(totalDeviation / maxPossibleDeviation)
            : 1f;

        return Mathf.RoundToInt(accuracy * 100f);
    }

    public static int ComputeOverallScore(Dish dish)
    {
        return Mathf.RoundToInt(dish.cravingMatchScore * 0.4f + dish.fillingScore * 0.2f + dish.rollCutScore * 0.2f + dish.patienceScore * 0.2f);
    }

    public static int ComputePatienceScore(float patienceTimer, float maxPatience)
    {
        if (maxPatience <= 0f)
            return 100;

        float fraction = Mathf.Clamp01(patienceTimer / maxPatience);
        return Mathf.RoundToInt((1f - fraction) * 100f);
    }

    public static float ScoreToStars(int overallScore)
    {
        float raw = overallScore / 100f * 5f;
        return Mathf.Round(raw * 2f) / 2f;
    }

    private static readonly string[] Traits = { "classic", "sweet", "spicy", "healthy", "rich" };

    public static string GetDominantTrait(Dish dish)
    {
        string bestTrait = Traits[0];
        int bestScore = int.MinValue;

        foreach (string trait in Traits)
        {
            int score = 0;
            foreach (string id in dish.chosenIngredientIds)
            {
                IngredientData ingredient = GameDatabase.Instance.GetIngredient(id);
                if (ingredient != null && ingredient.traits != null)
                    score += ingredient.traits.GetValue(trait);
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestTrait = trait;
            }
        }

        return bestTrait;
    }
}
