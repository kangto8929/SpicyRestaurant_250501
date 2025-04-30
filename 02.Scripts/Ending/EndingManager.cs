using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    [Header("Good Ending")]
    //[SerializeField, TextArea(3, 10)]
    public string _goodEndingText;// =
        /*"회사에선 매일 지쳐 있었지만,\n마라탕을 만들 땐 이상하게 마음이 편했다.\n" +
        "그 선택이, 이렇게 날 여기까지 데려다 줄 줄은 몰랐다.\n" +
        "지금, 이 가게는 수천 개의 좋아요와 함께...\n내 두 번째 인생이 되었다.";
        */

    [SerializeField] private Sprite _goodEndingSprite;

    [Header("Bad Ending")]
    //[SerializeField, TextArea(3, 10)]
    public string _badEndingText;// =
        /*"초반엔 입소문을 타며 손님이 몰렸지만,\n맛이 들쭉날쭉하다는 평이 하나둘 늘었고\n" +
        "리뷰 별 하나가 쌓일 때마다 사람들은 조용히...\n그러나 확실히 떠나갔다.";*/
    [SerializeField] private Sprite _badEndingSprite;

    [Header("UI")]
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private TextMeshProUGUI _endingText;

    //private float _timer = 0f;
    //[SerializeField] private float _minEndingTimer = 3f;

    private float textSpeed = 0.05f;
    private bool _isTyping = false;
    private Tween _typingTween;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance.IsGoodEnding || EndingStateManager.Instance.IsGoodEnding == true)
        {
            LoadEnding(_goodEndingText, _goodEndingSprite, true);
        }
        else if (!GameManager.Instance.IsGoodEnding || EndingStateManager.Instance.IsGoodEnding == false)
        {
            LoadEnding(_badEndingText, _badEndingSprite, false);
        }

        //초기화할 거
        SaveManager.DeleteSave(); // 예시: SaveManager에서 스토리 세이브 삭제
       // SoundManager.Instance.DeleteVolumeData(); // 예시: 사운드 볼륨 세이브 삭제

        CustomerManager.Instance.DeleteCustomerSave();//손님 저장도 리셋
        CurrencyManager.Instance.ResetCurrencyData();
        IngredientManager.Instance.ClearAllPlayerPrefsData();

        GameManager.Instance.ResetLevel();

        //수익과 잃은거 리셋
        CurrencyManager.Instance.ResetCurrencyInOutData();

        // 엔딩 상태 리셋
        EndingStateManager.Instance.ResetEnding();
        Debug.Log("날이 초기화되었기에 하루 수익과 잃은거 초기화");
    }



    private void LoadEnding(string text, Sprite bgSprite, bool isGood)
    {
        _backGroundImage.sprite = bgSprite;
        _endingText.text = text;
        _endingText.maxVisibleCharacters = 0;

        TypeText(_endingText);

        SoundManager.Instance.PlayEndingBGM(isGood);
    }

    private void TypeText(TextMeshProUGUI textUI)
    {
        float duration = textUI.text.Length * textSpeed;
        int previousCharCount = 0;

        _isTyping = true;

        _typingTween = DOTween.To(x =>
        {
            int currentCharCount = (int)x;
            textUI.maxVisibleCharacters = currentCharCount;

            if (currentCharCount > previousCharCount)
            {
                SoundManager.Instance.PlayRandomSoundInList(SoundManager.Instance.typeSounds);
                previousCharCount = currentCharCount;
            }
        },
        0f, textUI.text.Length, duration)
        .SetEase(Ease.Linear)
        .SetId("TextTween")
        .OnComplete(() => _isTyping = false);
    }

    public void OnNextButtonClicked()
    {
        if (_isTyping)
        {
            // 타이핑 중이면 중단하고 텍스트 전체 출력
            _typingTween.Kill();
            _endingText.maxVisibleCharacters = _endingText.text.Length;
            _isTyping = false;
        }
        else
        {
            // 타이핑이 끝났으면 씬 전환 및 데이터 삭제
            SoundManager.Instance.StopBackgroundMusic();
            SceneManager.LoadScene("StoryStartScene");
            StorySaveManager.Instance.DeleteSave();

            SaveManager.DeleteSave(); // 예시: SaveManager에서 스토리 세이브 삭제
            SoundManager.Instance.DeleteVolumeData(); // 예시: 사운드 볼륨 세이브 삭제

            SoundManager.Instance.SetMusicVolume(UI_Option.Instance.BgmBar.value);//
            SoundManager.Instance.SetEffectsVolume(UI_Option.Instance.SfxBar.value);


            CustomerManager.Instance.DeleteCustomerSave();//손님 저장도 리셋
            CurrencyManager.Instance.ResetCurrencyData();
            IngredientManager.Instance.ClearAllPlayerPrefsData();

            GameManager.Instance.ResetLevel();
      
            //수익과 잃은거 리셋
            CurrencyManager.Instance.ResetCurrencyInOutData();

            // 엔딩 상태 리셋
            EndingStateManager.Instance.ResetEnding();
            Debug.Log("****날이 초기화되었기에 하루 수익과 잃은거 초기화****");
        }
    }

}


