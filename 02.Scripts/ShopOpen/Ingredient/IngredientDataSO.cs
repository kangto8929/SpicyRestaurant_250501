using UnityEngine;

[CreateAssetMenu(fileName = "IngredientDataSO", menuName = "Scriptable Objects/IngredientDataSO")]
public class IngredientDataSO : ScriptableObject
{
    public IngredientType Type;
    public uint Price;
    public Sprite Icon;
    public bool IsBasic;
}
