// Implemented by Mohamed Osman on 2026-08-03
// AI assistance: VS Code (autocomplete), Claude (architecture review)

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
}
