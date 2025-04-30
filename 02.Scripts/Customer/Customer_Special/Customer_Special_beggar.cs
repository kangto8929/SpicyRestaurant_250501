using System.Collections.Generic;
using System.Linq;
//using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class Customer_Special_beggar : Customer_Special
{
    private List<string> _beggarOrderTextList;
    [SerializeField]
    private List<string> _beggarPositiveTextList;
    [SerializeField]
    private List<string> _beggarNegativeTextList;
    [SerializeField]
    private bool _isHelpedFirst;
    [SerializeField]
    private bool _isFirstTime = true;
    protected override void Initialize()
    {
        base.Initialize();

        _requiredIngredients = new Dictionary<IngredientType, uint>()
        {
            {IngredientType.Meat, 1}
        };
        OrderManager.Instance.SetOrder(_requiredIngredients);
        // юс╫ц
        _beggarOrderTextList = _orderText[0].Split("/").ToList<string>();
        _beggarPositiveTextList = _positiveText.Split("/").ToList<string>();
        _beggarNegativeTextList = _negativeText.Split("/").ToList<string>();

        if(_isFirstTime)
        {
            _orderText.Add(_beggarOrderTextList[0]);
            _positiveText = _beggarPositiveTextList[0];
            _negativeText = _beggarNegativeTextList[0];
        }
        else
        {
            if(!_isHelpedFirst)
            {
                _orderText.RemoveAt(0);
                _orderText.Add(_beggarOrderTextList[1]);
                _positiveText = _beggarPositiveTextList[1];
                _negativeText = _beggarNegativeTextList[1];
            }
            else
            {
                _orderText.RemoveAt(0);
                _orderText.Add(_beggarOrderTextList[2]);
                _positiveText = _beggarPositiveTextList[2];
                _negativeText = _beggarNegativeTextList[2];
            }
        }

    }
    protected override void SpriteInitialize()
    {
        return;
    }

    public void SetIsHelped(bool isHelped)
    {
        _isHelpedFirst = isHelped;
    }
}
