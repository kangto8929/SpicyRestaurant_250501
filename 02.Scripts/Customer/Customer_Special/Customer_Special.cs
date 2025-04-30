using UnityEngine;

public class Customer_Special : Customer_Base
{
    [SerializeField]
    private string _customerSpecialName;
    void Start()
    {
        Initialize();
        SpriteInit();
        MakeCustomerTextStruct();

        // 테스트용
        Debug.Log($"{_customerSpecialName}");
        foreach (var i in _orderText)
        {
            Debug.Log(i);
        }
        Debug.Log($"{_positiveText}");
        Debug.Log($"{_negativeText}");
        Debug.Log("주문끝");

    }

    protected virtual void Initialize()
    {
        // 타입 지정
        CustomerType = CustomerTypes.Special;
        _customerSpriterenderer = GetComponent<SpriteRenderer>();


        var data = ScriptManager.Instance.GetSpecialCustomerByName(_customerSpecialName);
        _orderText.Add(data.ordertext);
        if (data.nexttext != "")
        {
            _orderText.Add(data.nexttext);
        }
        _positiveText = data.positivetext;
        _negativeText = data.negativetext;

        
    }

    
    private void SpriteInit()
    {
        SpriteInitialize();
        _customerSpriterenderer.sprite = _customerSprite;
    }

    protected virtual void SpriteInitialize()
    {
        _customerSprite = ScriptManager.Instance.GetRandomCustomerNormalImages();
    }
    
}
