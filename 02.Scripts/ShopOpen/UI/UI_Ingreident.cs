using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//*****여기서 재료별로 크기 다르게 해야할 듯
//타입별로 크기 줄이기

public class UI_Ingreident : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Ingredient Ingredient;

    public Image IngredientPrefab;
    public Image IconImage;
    public TextMeshProUGUI AmountText;

    public GameObject StockPrefab;

    [SerializeField]
    private const float PREMIUM = 1.2f;

    private uint _previousAmount;
    private Image _dragImage;

    private GameObject _bowl;

    private void Start()
    {
        IconImage.sprite = Ingredient.Icon;

        if(Ingredient.IsBasic)
        {
            AmountText.gameObject.SetActive(false);
        }

        //Debug.Log("아래에 재료 저장한거 다시 확인 들어갑니다");
        //0422
        //SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);
        //SaveManager.LoadGame();
        //Refresh();

 
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        UI_Shop.Instance.AllowAnimation = true;
        _previousAmount = Ingredient.Amount;
        if(!Ingredient.IsBasic)
        {
            if (Ingredient.Amount == 0)
            {
                
                // TODO
                // 버튼 진동 & 경고음
                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.BeepSound);
                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.ReceiptSound);
                CurrencyManager.Instance.RemoveCurrency(CurrencyType.Money, (int)(Ingredient.Price * PREMIUM));

                Debug.Log($"{Ingredient.Type} 재료가 없습니다! {(int)(Ingredient.Price * PREMIUM)}에 구매합니다!");

                
            }
            else
            {
                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.NotifiySound);
                Ingredient.Amount--;
            }
        }

        _dragImage = Instantiate(IngredientPrefab, UI_Shop.Instance.gameObject.transform);
        _dragImage.sprite = Ingredient.Icon;

        //0416
        // 타입별 크기 조절
        switch (Ingredient.Type)
        {
            case IngredientType.BokChoy:
                _dragImage.rectTransform.sizeDelta = new Vector2(265, 265);//0
                break;
            case IngredientType.EnokiMushroom:
                _dragImage.rectTransform.sizeDelta = new Vector2(265, 265);//0
                break;
            case IngredientType.Meat:
                _dragImage.rectTransform.sizeDelta = new Vector2(245, 245);//0
                break;
            case IngredientType.Tofu:
                _dragImage.rectTransform.sizeDelta = new Vector2(190, 190);
                break;
            case IngredientType.GlassNoodle:
                _dragImage.rectTransform.sizeDelta = new Vector2(250, 250);
                break;
            case IngredientType.WoodEarMushroom:
                _dragImage.rectTransform.sizeDelta = new Vector2(190, 190);
                break;
            case IngredientType.Shrimp:
                _dragImage.rectTransform.sizeDelta = new Vector2(190, 190);
                break;
            case IngredientType.Cilantro:
                _dragImage.rectTransform.sizeDelta = new Vector2(265, 265);
                break;
            case IngredientType.Stock:
                _dragImage.rectTransform.sizeDelta = new Vector2(230, 230);//0
                break;
            default:
                _dragImage.rectTransform.sizeDelta = new Vector2(110, 110); // 기본값
                break;
        }

        

        Refresh();
        //IngredientManager.Instance.SaveAllIngredientCounts();
        //SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);
        //Debug.Log("재료 저장하기");

    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _dragImage.rectTransform.position = mousePosition;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        _bowl = GameObject.FindWithTag("Bowl");
        CapsuleCollider2D collider = _bowl.GetComponent<CapsuleCollider2D>();
        // Collider2D collider = _bowl.GetComponent<Collider2D>();//원래 있던거

        if (collider.OverlapPoint(mousePosition))
        {
            _dragImage.rectTransform.position = mousePosition;
            _dragImage.transform.SetParent(_bowl.transform);
            _bowl.GetComponent<Bowl>().AddIngredeint(Ingredient);


            if(Ingredient.IsBasic)
            {
                Destroy(_dragImage.gameObject);
                SoundManager.Instance.PlayRandomSoundInList(SoundManager.Instance.WaterPouringSounds);
                return;
            }

            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.SquishSound);
            //고기 확인
            BowlMeatChecker.Instance.CheckBowlChildren();
            return;
        }

        if(!Ingredient.IsBasic)
        {
            Ingredient.Amount++;
        }

        Destroy(_dragImage.gameObject);
        Refresh();
        //SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);
        //Debug.Log("재료 저장하기");
    }


    public void Refresh()
    {
        AmountText.text = $"x{Ingredient.Amount}";

    }
}
