using DG.Tweening;
using UnityEngine;

public class ShopMover : MonoBehaviour
{
    private void Start()
    {
        UI_Shop.Instance.OnKitchenToggled += ToggleShopObjects;
    }

    public void ToggleShopObjects()
    {
        if(UI_Shop.Instance.IsKitchenOpened)
        {
            transform.DOMoveY(transform.position.y - UI_Shop.Instance.KitchenMoveAmount, 1f);
        }
        else
        {
            transform.DOMoveY(transform.position.y + UI_Shop.Instance.KitchenMoveAmount, 1f);
        }
    }
}
