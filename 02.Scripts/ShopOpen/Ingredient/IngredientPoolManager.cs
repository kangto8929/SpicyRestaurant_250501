using System.Collections.Generic;
using UnityEngine;

public class IngredientPoolManager : MonoBehaviour
{
    public static IngredientPoolManager Instance;

    public GameObject IngredientPrefab;
    public List<GameObject> IngredientPool;

    [SerializeField]
    private uint poolSize;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void InstantinatePrefabs(uint ingredientCount)
    {
        poolSize = ingredientCount;
        IngredientPool = new List<GameObject>((int)poolSize);
        for(int i=0; i< poolSize; i++)
        {
            GameObject ingredient = Instantiate(IngredientPrefab, this.transform);
            ingredient.SetActive(false);
            IngredientPool.Add(ingredient);
        }
    }


}
