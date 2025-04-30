using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bowl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float XMoveTreshold;
    public float YMoveTreshold;

    public bool IsCooked = false;

    [SerializeField]
    private Burner _burner;
    [SerializeField]
    private SpriteRenderer _trashIcon;
    [SerializeField]
    private GameObject _stockImagePrefab;
    private GameObject _stock;

    private Dictionary<IngredientType, uint> _bowlContent;

    public GameObject NoicePanel;

    private Vector2 _defaultPosition;

    private void Start()
    {
        _bowlContent = new Dictionary<IngredientType, uint>();
        UI_Shop.Instance.OnKitchenToggled += InitializeBowl;
        gameObject.SetActive(false);
        
    }


    public void InitializeBowl()
    {
        _bowlContent.Clear();
        gameObject.SetActive(true);

        _stock = Instantiate(_stockImagePrefab, this.transform);
        _stock.SetActive(false);

        IsCooked = false;
    }

    public void AddIngredeint(Ingredient ingredient)
    {
        uint ingredientAmount = 0;


        if (_bowlContent.TryGetValue(ingredient.Type, out ingredientAmount))
        {
            if (ingredient.IsBasic) return;
            _bowlContent[ingredient.Type] = ++ingredientAmount;
        }
        else
        {
            _bowlContent.Add(ingredient.Type, 1);
        }

        if (ingredient.IsBasic)
        {
            _stock.SetActive(true);
        }

        _stock.transform.SetAsLastSibling();
        IsCooked = false;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        _defaultPosition = transform.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;

        if(_defaultPosition.x - transform.position.x >= XMoveTreshold)
        {
            _trashIcon.gameObject.SetActive(true);
            _trashIcon.transform.position = mousePosition + new Vector2(XMoveTreshold, 0f);
            _trashIcon.flipX = false;
        }
        else if((_defaultPosition.x - transform.position.x <= -XMoveTreshold))
        {
            _trashIcon.gameObject.SetActive(true);
            _trashIcon.transform.position = mousePosition - new Vector2(XMoveTreshold, 0f);
            _trashIcon.flipX = true;
        }
        else
        {
            _trashIcon.gameObject.SetActive(false);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (_defaultPosition.x - transform.position.x >= XMoveTreshold
            || _defaultPosition.x - transform.position.x <= -XMoveTreshold)
        {
            Debug.Log("버리기!!");
            _trashIcon.gameObject.SetActive(false);
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.MetalPlaceSound);

            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
            InitializeBowl();

            transform.position = _defaultPosition;
            return;
        }

        if(transform.position.y - _defaultPosition.y >= YMoveTreshold)
        {
            if(!IsCooked)
            {
                transform.position = _defaultPosition;

                //TODO
                // 경고사운드 재생
                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.BeepBeepSound);
                // 가스버너 스위치위치에 화살표 띄우기(시간 나면)
                NoicePanel.SetActive(true);//끓이라는 안내문
                return;
            }

            Debug.Log("주문 제출");

            UI_Shop.Instance.StopCurrentProcess();//아래 나오던거 그만

          /*  if(UI_Shop.Instance.AllowAnimation == true)
            {
                UI_Shop.Instance.MoveFrontDeskPanelDown();//말풍선 올려
                //다시 주문 나오고 있을 때만 위로 올려
                Debug.Log("다시 주문 나올 때 위로 올려");
            }*/



            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.WooshSound);
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
            transform.position = _defaultPosition;
            gameObject.SetActive(false);
            IsCooked = false;
            
            OrderManager.Instance.IsCompleteOrder(_bowlContent);

            UI_Shop.Instance.ToggleKitchen();
            _burner.InitBurner();
            //UI_Shop.Instance.AllowAnimation = true;

            // UI_Shop.Instance.AllowAnimation = false;
             UI_Shop.Instance.AskButton.interactable = true;



            /*IngredientManager.Instance.SaveAllIngredientCounts();
            CurrencyManager.Instance.SaveCurrencyData();//재화 및 만족도 저장

            SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);
            */

            return;
        }

        transform.position = _defaultPosition;
    }

    //안내문 확인 버튼
    public void OkButton()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        NoicePanel.SetActive(false);//끓이라는 안내문
    }
}
