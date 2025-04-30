using UnityEngine;

public class Customer_OnlyText : Customer_Base
{
    [SerializeField]
    private int _customerOnlyTextIndex = 0;
    [SerializeField]
    private string _customerOnlyTextName = "";



    private void Start()
    {
        Initialize();
        Debug.Log($"{_customerOnlyTextName}");
        foreach(var i in _orderText)
        {
            Debug.Log(i);
        }
        Debug.Log("¡÷πÆ≥°");
        MakeCustomerTextStruct();
    }

    private void Initialize()
    {
        CustomerType = CustomerTypes.OnlyText;
        _customerSpriterenderer = GetComponent<SpriteRenderer>();

        var data = ScriptManager.Instance.GetCustomerOnlyTextByIndex(_customerOnlyTextIndex);

        _customerOnlyTextName = data.name;

        _orderText.Add(data.ordertext);
        if(data.nexttext != "")
        {
            _orderText.Add(data.nexttext);
        }

        SpriteInitialize();
        _customerSpriterenderer.sprite = _customerSprite;
    }

    protected virtual void SpriteInitialize()
    {
        _customerSprite = ScriptManager.Instance.GetRandomCustomerNormalImages();
    }
}
