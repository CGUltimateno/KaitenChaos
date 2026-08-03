using System;
using System.Collections.Generic;
using UnityEngine;
// Implemented by Mohamed Osman on 2026-08-03
// AI assistance: VS Code (autocomplete), Claude (architecture review)

[Serializable]
public class Traits
{
    public int classic, sweet, spicy, healthy, rich;

    public int GetValue(string trait) { 
            switch (trait) {
            case "classic": return classic;
            case "sweet": return sweet;
            case "spicy": return spicy;
            case "healthy": return healthy;
            case "rich": return rich;
            default:
                Debug.LogError("Invalid trait: " + trait);
                return 0;
        }
    }
}

[Serializable] 
public class IngredientData { public string id; public Traits traits; }

[Serializable] 
public class IngredientList { public List<IngredientData> ingredients; }

[Serializable] 
public class OrderTypeData { public string type; public List<string> phrases; }
        
[Serializable] 
public class OrderList { public List<OrderTypeData> orders; }

