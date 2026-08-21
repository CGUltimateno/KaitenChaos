using UnityEngine;
// Implemented by Cise Nil Köksal on 21/08/2026
// AI Assistance: ChatGPT (Conceptualization)
public class GuidebookPage : MonoBehaviour
{

    public string ingredientID;
    public SpriteRenderer classicLock;
    public SpriteRenderer sweetLock;
    public SpriteRenderer spicyLock;
    public SpriteRenderer healthyLock;
    public SpriteRenderer richLock;

    public void UnlockTrait(string trait, int score)
    {
        Debug.Log(
        $"[GuidebookPage] {ingredientID}: unlocking {trait} with score {score}"
    );
        Sprite scoreSprite = GetTraitScore(score);

        switch (trait)
        {
            case "classic":
                classicLock.sprite = scoreSprite;
                classicLock.color = Color.white;
                classicLock.transform.localScale = Vector3.one;
                break;

            case "sweet":
                sweetLock.sprite = scoreSprite;
                sweetLock.color = Color.white;
                sweetLock.transform.localScale = Vector3.one;
                break;

            case "spicy":
                spicyLock.sprite = scoreSprite;
                spicyLock.color = Color.white;
                spicyLock.transform.localScale = Vector3.one;
                break;

            case "healthy":
                healthyLock.sprite = scoreSprite;
                healthyLock.color = Color.white;
                healthyLock.transform.localScale = Vector3.one;
                break;

            case "rich":
                richLock.sprite = scoreSprite;
                richLock.color = Color.white;
                richLock.transform.localScale = Vector3.one;
                break;
            default:
                Debug.LogError(
                    $"[GuidebookPage] Unknown trait '{trait}'"
                );
                break;
        }
    }

    private Sprite GetTraitScore(int score)
    {
        string spriteName = score switch
        {
            -2 => "book_minus_two",
            -1 => "book_minus_one",
            0 => "book_plus_zero",
            1 => "book_plus_one",
            2 => "book_plus_two",
            _ => null
        };

        if (spriteName == null)
            return null;

        Sprite s = Resources.Load<Sprite>(
            "Art/objects/book_ratings/" + spriteName
        );
        Debug.Log(
    s != null
        ? $"LOADED SUCCESSFULLY: {s.name}"
        : $"FAILED TO LOAD: Art/objects/book_ratings/{spriteName}"
);

        Debug.Log($"[GuidebookPage] Loading sprite '{spriteName}' -> {s}");
        return s;
    }

}