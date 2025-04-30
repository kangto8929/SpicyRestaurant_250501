using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;


public enum LoadingSceneTypes
{
   // Match3ToStoryStartScene,//매치3에서 리셋
    //ShopOpenToStoryStartScene,//영업중에 리셋

    StartToMatch3,
    Match3ToShopOpen,
    ResultToMatch3,
    ResultToEnding,

    Count,
}

public class LoadingSceneManager : MonoBehaviour
{
    
    public static string nextScene;
    public static LoadingSceneTypes _loadingSceneType;

    // [SerializeField]
    // private Image _progressBar;
    [SerializeField]
    private Image _loadingImage;
    // [SerializeField]
    // private TextMeshProUGUI _loadingProgressText;
    [SerializeField]
    private TextMeshProUGUI _loadingText;
    [SerializeField]
    private Sprite _startToMatch3LoadingSprite;
    [SerializeField]
    private Sprite _match3ToShopOpenLoadingSprite;
    [SerializeField]
    private Sprite _resultToMatch3LoadingSprite;
    [SerializeField]
    private Sprite _resultToEndingLoadingSprite;
    [SerializeField]
    private TextMeshProUGUI _startToTouchText;

    private bool isNextSceneConfirmed = false;//다음씬으로 이동 관련 bool
    void Update()
    {

    }

    private void Start()
    {
        SoundManager.Instance.StopAllSoundEffects();

        switch (_loadingSceneType)
        {
           /* case LoadingSceneTypes.Match3ToStoryStartScene:
                _loadingImage.sprite = _startToMatch3LoadingSprite;
                break;
            case LoadingSceneTypes.ShopOpenToStoryStartScene:
                _loadingImage.sprite = _match3ToShopOpenLoadingSprite;
                break;
           */
            case LoadingSceneTypes.StartToMatch3:
                _loadingImage.sprite = _startToMatch3LoadingSprite;
                break;
            case LoadingSceneTypes.Match3ToShopOpen:
                _loadingImage.sprite = _match3ToShopOpenLoadingSprite;
                break;
            case LoadingSceneTypes.ResultToMatch3:
                _loadingImage.sprite = _resultToMatch3LoadingSprite;
                break;
            case LoadingSceneTypes.ResultToEnding:
                _loadingImage.sprite = _resultToEndingLoadingSprite;
                break;
        }

       // _loadingText.text = ScriptManager.Instance.GetRandomLoadingText().loadingtext;

        if (ScriptManager.Instance == null)
        {
            Debug.LogError(" ScriptManager.Instance가 null입니다!");
        }
        else if (ScriptManager.Instance.GetRandomLoadingText() == null)
        {
            Debug.LogError(" GetRandomLoadingText()의 반환값이 null입니다!");
        }
        else
        {
            _loadingText.text = ScriptManager.Instance.GetRandomLoadingText().loadingtext;
            Debug.Log(" 로딩 텍스트 성공적으로 설정됨: " + _loadingText.text);
        }

        StartCoroutine(LoadSceneProcess());

        //  StartCoroutine(LoadSceneProcess());   
    }

    public static void LoadScene(string sceneName, LoadingSceneTypes loadingSceneType)
    {
        nextScene = sceneName;
        _loadingSceneType = loadingSceneType;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            yield return null;

            // 로딩이 90% 이상 완료되면 사용자 입력을 기다림
            if (op.progress >= 0.9f)
            {
                _startToTouchText.gameObject.SetActive(true);

                if (isNextSceneConfirmed)
                {
                    op.allowSceneActivation = true;
                    Debug.Log("다음 씬으로 넘어갑니다!");
                    yield break;
                }
            }
        }
    }

    public void NextSceneButton()
    {
        isNextSceneConfirmed = true;
    }

}
