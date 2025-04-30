using UnityEngine;

public enum IngredientType
{
    BokChoy,
    EnokiMushroom,
    Meat,
    Tofu,
    GlassNoodle,//
    WoodEarMushroom,//格捞滚几
    Shrimp,//货快
    Cilantro,//绊荐
    Stock,//必股

    Count
}

public class Ingredient
{
    public IngredientType Type;
    public Sprite Icon;
    public uint Price;
    public uint Amount;
    public bool IsBasic;

    public Ingredient(IngredientDataSO data, uint amount)
    {
        Type = data.Type;
        Price = data.Price;
        Icon = data.Icon;
        Amount = amount;
        IsBasic = data.IsBasic;
    }
}
