using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance;

    [SerializeField]
    private Image fadeImage;
    private float fadeDuration = 1f;

    private Sequence _resultSequence;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _resultSequence = DOTween.Sequence();

        _resultSequence.AppendCallback(()=> 
                        {
                            StartCoroutine(FadeIn());
                            fadeImage.gameObject.SetActive(false);
                        })
                        .AppendCallback(()=> StartStage());

        //SaveManager.SaveGame((GameManager.Instance._currentLevel + 1), GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);
        //GameManager.Instance.SetCurrentLevel(GameManager.Instance._currentLevel + 1);
        //  SaveManager.SaveGame(GameManager.Instance.CurrentLevel + 1, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);

    }

    private void StartStage()
    {
        UI_Result.Instance.AnimateResultPanel();
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.resultAppearSound);
    }

    public IEnumerator FadeIn()
    {
        if (fadeImage == null)
        {
            yield break;
        }

        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime/fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }


    public void OnNextStageButtonClick()
    {
        UI_Result.Instance.DOKill();
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        if(GameManager.Instance.CurrentLevel > 2)//(GameManager.Instance.CheckEnding())
        {
            Debug.Log("ResultManager 엔딩으로 가는 거 확인2");
            //LoadingSceneManager.LoadScene("Match3Game", LoadingSceneTypes.ResultToMatch3);
            LoadingSceneManager.LoadScene("EndingScene", LoadingSceneTypes.ResultToEnding);
            //엔딩으로 가는 거


            //수익과 잃은거 리셋
            CurrencyManager.Instance.ResetCurrencyInOutData();

            //SaveManager.SaveGame((GameManager.Instance.CurrentLevel+1), GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);
            SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);
            //GameManager.Instance.CurrentLevel + 1 이었음
            Debug.Log("날이 초기화되었기에 하루 수익과 잃은거 초기화");
        }
        else
        {
            //GameManager.Instance.EndStage();
            LoadingSceneManager.LoadScene("Match3Game", LoadingSceneTypes.ResultToMatch3);

            //수익과 잃은거 리셋
            CurrencyManager.Instance.DayChangeCurrencyInOutData();

            //SaveManager.SaveGame((GameManager.Instance.CurrentLevel+1), GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);
            SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);
            Debug.Log("현재 행복과 돈 유지. 하루 수익과 잃은거 초기화");
            //GameManager.Instance.CurrentLevel + 1 이었음
        }

       
    }

    public void OnMainMenuButtonClick()//게임 종료로 변경
    {
/*#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // 에디터에서는 플레이 모드 종료
#else
    Application.Quit(); // 실제 빌드에서는 종료
#endif*/
         UI_Result.Instance.DOKill();
         SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
         //GameManager.Instance.EndStage();
         LoadingSceneManager.LoadScene("StartScene", LoadingSceneTypes.ResultToMatch3);

         //수익과 잃은거 리셋
         CurrencyManager.Instance.DayChangeCurrencyInOutData();
         Debug.Log("현재 행복과 돈 유지. 하루 수익과 잃은거 초기화");
         SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);
        
    }
}
