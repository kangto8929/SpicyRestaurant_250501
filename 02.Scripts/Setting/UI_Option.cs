using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class UI_Option : MonoBehaviour
{
    public static UI_Option Instance;

    public Slider BgmBar;
    [SerializeField] public Slider SfxBar;

    [SerializeField] private Button howPlayButton;
    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;

    public Transform BlackBackground;
    public Transform BackgroundCanvasGroup;
    public Image BackgroundBlackImage;

    [Header("How to Play UI")]
    [SerializeField] private GameObject howPlayUI;
    [SerializeField] private GameObject howPlayUIBG;
    [SerializeField] private Button howPlayCloseButton;
    [SerializeField] private Button howPlayLeftButton;
    [SerializeField] private Button howPlayRightButton;
    [SerializeField] private Image explanationImage;
    [SerializeField] private List<Sprite> explanationImages;
    [SerializeField] private TextMeshProUGUI explanationText;

    [Header("Credit UI")]
    [SerializeField] private GameObject creditUI;
    [SerializeField] private GameObject creditUIBG;
    [SerializeField] private Button creditcancelButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private RectTransform creditContent;

    [Header("Reset UI")]
    [SerializeField] private GameObject resetUI;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button yesResetButton;
    [SerializeField] private Button noResetButton;


    [Header("Fade Image")]
    public GameObject FadeImage;
    public Image FadeImageRaycast;
    public Animator FadeImageAnimation;

    public SoundManager soundManager;

    private List<string> explanationTexts = new List<string>
    {
        "같은 재료를 3개 이상 맞춰서\n재료를 얻으세요.",//1
        "4개 이상 연결 시 생성되는\n조리도구 아이템을 터치하면,\n재료를 더 많이 획득할 수 있어요.",//2
        "영업 단계에서는 손님의 주문을\n수락하거나 거절할 수 있어요.",//3
        "재료 버튼을 눌러 선택하고,\n드래그해서 냄비에 넣으세요.",//4
        "모든 재료를 넣었다면\n가스레인지의 버너 버튼을 누르세요.",//5
        "조리가 끝나면 냄비를 위로\n드래그해서 손님에게 제출하세요.",//6
        "재료를 잘못 넣었을 땐 냄비를\n좌우로 드래그해 버릴 수 있어요.",//7
        "없는 재료를 넣으려 하면,\n재료값만큼 돈이 줄어들어요.",//8
        "주문에 맞춰 마라탕을 만들고,\n최고의 맛집에 도전하세요!",//9
    };

    public float initialBgmVolume;
    public float initialSfxVolume;
    private int currentIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (howPlayButton != null)
            howPlayButton.onClick.AddListener(OnClickHowPlay);

        if (okButton != null)
            okButton.onClick.AddListener(OnClickOK);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnClickCancel);

        if (howPlayCloseButton != null)
            howPlayCloseButton.onClick.AddListener(OnClickHowPlayClose);

        if (howPlayLeftButton != null)
            howPlayLeftButton.onClick.AddListener(OnClickLeft);

        if (howPlayRightButton != null)
            howPlayRightButton.onClick.AddListener(OnClickRight);


        if (creditButton != null)
            creditButton.onClick.AddListener(GoCredit);

        if (creditcancelButton != null)
            creditcancelButton.onClick.AddListener(ExitCredit);

        //리셋
        if (resetButton != null)
            resetButton.onClick.AddListener(GameReset);

        if (yesResetButton != null)
            yesResetButton.onClick.AddListener(GameResetYes);

        if (noResetButton != null)
            noResetButton.onClick.AddListener(GameResetNo);
    }

    private void Start()
    {
        FadeImage.SetActive(false);//페이드 이미지 비활성

        BlackBackground.localScale = Vector3.zero;

        Instance = this;

        if (soundManager != null)
        {
            if (BgmBar != null)
            {
                BgmBar.value = soundManager.bgmVolume;
                BgmBar.onValueChanged.AddListener(OnBGMVolumeChange);
            }

            if (SfxBar != null)
            {
                SfxBar.value = soundManager.sfxVolume;
                SfxBar.onValueChanged.AddListener(OnSFXVolumeChange);
            }
        }


        //SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);
        UpdateExplanationImage();

        
    }

    private void OnEnable()
    {
        if (soundManager != null)
        {
            if (BgmBar != null)
            {
                initialBgmVolume = soundManager.bgmVolume;
                BgmBar.value = initialBgmVolume;
            }

            if (SfxBar != null)
            {
                initialSfxVolume = soundManager.sfxVolume;
                SfxBar.value = initialSfxVolume;
            }
        }

        BackgroundCanvasGroup.localScale = Vector3.zero;
        BackgroundCanvasGroup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        

    }

    private void OnDisable()
    {
        BackgroundCanvasGroup.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    #region 슬라이더 기능
    /*private void OnBGMVolumeChange(float value)
    {
        soundManager.SetMusicVolume(value);
        soundManager.SaveVolume();
        Debug.Log("배경음 저장");
    }

    private void OnSFXVolumeChange(float value)
    {
        soundManager.SetEffectsVolume(value);
        soundManager.SaveVolume();
        Debug.Log("효과음 저장");
    }*/
    private void OnBGMVolumeChange(float value)
    {
        SoundManager.Instance.SetMusicVolume(value);
        Debug.Log("배경음 미리듣기 (저장 안 함)");
    }

    private void OnSFXVolumeChange(float value)
    {
        SoundManager.Instance.SetEffectsVolume(value);
        Debug.Log("효과음 미리듣기 (저장 안 함)");

    }
    #endregion

    #region 버튼 기능들
    private void OnClickHowPlay()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        howPlayUI.SetActive(true);
        currentIndex = 0;
        UpdateExplanationImage();

        if (howPlayUIBG != null)
        {
            howPlayUIBG.transform.localScale = Vector3.zero;
            howPlayUIBG.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }
    }

    //크레딧
    public void GoCredit()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        creditUI.SetActive(true);

        if (howPlayUIBG != null)
        {
            creditUIBG.transform.localScale = Vector3.zero;
            creditUIBG.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        creditContent.offsetMin = new Vector2(creditContent.offsetMin.x, -1103.565f);
        creditContent.offsetMax = new Vector2(creditContent.offsetMax.x, 0.0004882813f);
    }

    public void ExitCredit()
    {
        if (creditUIBG != null)
        {
            creditUIBG.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                creditUI.SetActive(false);
            });
        }

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
    }

    //게임 리셋
    public void GameReset()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        if (MatchUIManager.Instance != null)
        {
            MatchUIManager.Instance.PauseUIBackground.SetActive(true);
        }
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        resetUI.SetActive(true);

    }

    public void GameResetYes()
    {
        Debug.Log("게임 리셋할 거임");

        BlackBackground.localScale = Vector3.zero;
        if (MatchUIManager.Instance != null)
        {
            MatchUIManager.Instance.PauseUIBackground.SetActive(true);
        }
        resetUI.SetActive(false);
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        SoundManager.Instance.StopBackgroundMusic();
        
        if(MatchUIManager.Instance != null)
        {
            MatchUIManager.Instance.PauseUI.SetActive(false);
            ShapesManager.Instance.PauseTimer(); // Pause the timer in ShapesManager
            ShapesManager.Instance.PauseBlockMovement(); // Pause block movement in ShapesManager
        }
        
        StorySaveManager.Instance.DeleteSave();

        FadeImageRaycast.raycastTarget = true;
        FadeImage.SetActive(true);
        FadeImageAnimation.SetTrigger("Black");

        StartCoroutine(ResetAndGoStart());

        IEnumerator ResetAndGoStart()
        {
            yield return new WaitForSeconds(1.0f);
            //  StorySaveManager.Instance.DeleteSave();
            CurrencyManager.Instance.ResetCurrencyData();
            GameManager.Instance.ResetLevel();
            //수익과 잃은거 리셋
            CurrencyManager.Instance.ResetCurrencyInOutData();
            // 엔딩 상태 리셋
            EndingStateManager.Instance.ResetEnding();

            if (CustomerManager.Instance != null)
            {
                CustomerManager.Instance.DeleteCustomerSave();//손님 저장도 리셋
            }

            if (IngredientManager.Instance != null)
            {
                IngredientManager.Instance.ClearAllPlayerPrefsData();
            }

            //씬 이동 관련
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;

            SceneManager.LoadScene("StoryStartScene");

            SoundManager.Instance.DeleteVolumeData(); // 예시: 사운드 볼륨 세이브 삭제
            SoundManager.Instance.SaveVolume();
            SoundManager.Instance.LoadVolume();
            
        }
 
    }

    public void GameResetNo()
    {

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        resetUI.SetActive(false);
    }


    private void OnClickOK()
    {
        SoundManager.Instance.SetMusicVolume(BgmBar.value);
        SoundManager.Instance.SetEffectsVolume(SfxBar.value);

        // 저장은 여기서만
        soundManager.SaveVolume();

        // 현재 값을 새 초기값으로 갱신
        initialBgmVolume = BgmBar.value;
        initialSfxVolume = SfxBar.value;

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        BlackBackground.localScale = Vector3.zero;

        if (MatchUIManager.Instance != null)
        {
            MatchUIManager.Instance.PauseUIBackground.SetActive(true);
        }
    }


    private void OnClickCancel()
    {
        // 저장된 값 불러오기
        soundManager.LoadVolume();

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        BlackBackground.localScale = Vector3.zero;

        if (MatchUIManager.Instance != null)
        {
            MatchUIManager.Instance.PauseUIBackground.SetActive(true);
        }

        initialBgmVolume = soundManager.bgmVolume;
        BgmBar.value = initialBgmVolume;
        initialSfxVolume = soundManager.sfxVolume;
        SfxBar.value = initialSfxVolume;
        soundManager.SaveVolume();
        Debug.Log("비지엠" + BgmBar.value);
        Debug.Log("효과음" + SfxBar.value);
    }


    private void OnClickHowPlayClose()
    {
        if (howPlayUIBG != null)
        {
            howPlayUIBG.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                howPlayUI.SetActive(false);
            });
        }

        soundManager.PlaySoundEffect(soundManager.popSound);
    }
    #endregion

    #region 설명 이미지 기능들

    private void OnClickLeft()
    {
        if (explanationImages == null || explanationImages.Count == 0) return;

        currentIndex = (currentIndex - 1 + explanationImages.Count) % explanationImages.Count;
        UpdateExplanationImage();
        soundManager.PlaySoundEffect(soundManager.popSound);
    }

    private void OnClickRight()
    {
        if (explanationImages == null || explanationImages.Count == 0) return;

        currentIndex = (currentIndex + 1) % explanationImages.Count;
        UpdateExplanationImage();
        soundManager.PlaySoundEffect(soundManager.popSound);
    }

    private void UpdateExplanationImage()
    {
        if (explanationImage != null && explanationImages.Count > 0)
        {
            explanationImage.sprite = explanationImages[currentIndex];
        }

        if (explanationText != null && explanationTexts.Count > 0)
        {
            explanationText.text = explanationTexts[currentIndex];
        }
    }
    #endregion
}
