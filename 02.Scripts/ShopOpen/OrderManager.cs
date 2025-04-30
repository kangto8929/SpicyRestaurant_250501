using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance;

    public Dictionary<IngredientType, uint> Order => _order;

    [SerializeField]
    private Dictionary<IngredientType, uint> _order = new Dictionary<IngredientType, uint>();

    [SerializeField]
    private int _minSatisfaction = 3;

    [SerializeField]
    private int _maxSatisfaction = 8;



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

    public void SetOrder(Dictionary<IngredientType, uint> customerOrder)
    {
        _order = customerOrder;

        // TODO
        // 기본 재료 추가
        _order.Add(IngredientType.Stock, 1);
    }

    public void IsCompleteOrder(Dictionary<IngredientType, uint> bowl)
    {
        if (_order.Count == bowl.Count && !_order.Except(bowl).Any())
        {
            Debug.Log($"주문 성공");
            UI_CustomerText.Instance.PrintResutlText(true);

            int price = CalculatePrice(_order);
            CurrencyManager.Instance.AddCurrency(CurrencyType.Money, price);
            CurrencyManager.Instance.AddCurrency(CurrencyType.Satisfaction, Random.Range(_minSatisfaction, _maxSatisfaction));
            //0417

            
            
            //SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.customerCashSound?.OrderBy(_ => Random.value).FirstOrDefault());
            _order.Clear();
            return;
        }

        Debug.Log($"주문 실패");
        UI_CustomerText.Instance.PrintResutlText(false);

        CurrencyManager.Instance.RemoveCurrency(CurrencyType.Satisfaction, Random.Range(_minSatisfaction, _maxSatisfaction));

        _order.Clear();
    }

    private int CalculatePrice(Dictionary<IngredientType, uint> order)
    {
        int price = 0;
        foreach (var keyValuePair in order)
        {
            Ingredient ingredient = IngredientManager.Instance.GetIngredient(keyValuePair.Key);
            if(ingredient == null)
            {
                Debug.LogWarning($"{keyValuePair.Key} 재료 불러오기 실패");
                continue;
            }

            price += (int)(ingredient.Price * keyValuePair.Value);
            
        }

        Debug.Log($"보너스 붙기 전 가격: {price}");
        price = (int)(price * (CurrencyManager.Instance.Satisfaction + 50) * 0.01);
        Debug.Log($"보너스 붙기 후 가격: {price}");
        return price;
    }
}
