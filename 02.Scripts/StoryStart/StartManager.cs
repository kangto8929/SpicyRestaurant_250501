using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using DG.Tweening;//0413

public class StartManager : MonoBehaviour
{
    [SerializeField]
    private GameObject titleLogo; 

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Image fadeImage; 

    //0413
   /* [SerializeField]
    private TextMeshProUGUI startText; 
   */

    [SerializeField]
    private float fadeDuration = 1f; 

    private Coroutine textFadeCoroutine;

    //0413
    [SerializeField]
    private GameObject startButtonObject;

    //0413
    [SerializeField]
    private Image titleLogoImage;

    private void Awake()
    {
        if (startButton == null)
        {
            Debug.LogError("Start button is not assigned in the inspector.");
            return;
        }
        startButton.onClick.AddListener(OnClickStart);
    }

    private void Start()
    {
        /*if (SoundManager.Instance.OpeningBGM != null || !SoundManager.Instance.BGMmusicSource.isPlaying)
        {
            SoundManager.Instance.PlayOpeningMusic();

            Debug.Log("스타트 화면 배경음악 나오기");
        }*/

        //0413
        /*   if (startText != null)
           {
               textFadeCoroutine = StartCoroutine(FadeTMPAlpha(startText));
           }
        */


        //0413
        startButtonObject.SetActive(false);//버튼 처음에 보이지 않도록
        FadeInLogo();//로고 보이게 하는 애니메이션
    }

    //0413
    private void FadeInLogo()
    {
        Color logoColor = titleLogoImage.color;
        logoColor.a = 0f;
        titleLogoImage.color = logoColor;

        // 점점 진해지게 만들기
        titleLogoImage.DOFade(1f, fadeDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                startButtonObject.SetActive(true);
            });
    }

    
        private void OnClickStart()
        {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        // Load the saved scene (using enum)
        Scenes nextSceneEnum = SaveManager.LoadGame();

        // Convert the enum to a string for loading the scene
        string nextScene = nextSceneEnum.ToString();

        // 씬 이동
        Debug.Log("이동할 씬: " + nextScene);

        // 씬 이동을 위한 코루틴 호출
        StartCoroutine(FadeAndLoadScene(nextScene));
    }

        private IEnumerator FadeAndLoadScene(string sceneName)
        {
            yield return FadeOut(); // Call the local FadeOut method
            //SceneManager.LoadScene(sceneName);
            LoadingSceneManager.LoadScene(sceneName, LoadingSceneTypes.StartToMatch3);
        }

        public IEnumerator FadeOut()
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
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }
        }

    /*private void OnClickStart()
    {

        // Play pop sound
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        // Load the saved scene (using enum)
        Scenes nextSceneEnum = SaveManager.LoadGame();

        // Convert the enum to a string for loading the scene
        string nextScene = nextSceneEnum.ToString();

        // 씬 이동
        Debug.Log("이동할 씬: " + nextScene);

        // 씬 이동을 위한 코루틴 호출
        StartCoroutine(FadeAndLoadScene(nextScene));
    }


    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return FadeOut(); // FadeOut 애니메이션 수행
                                // 여기에서 실제로 씬 이름을 사용하여 씬을 로드합니다.
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator FadeOut()
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
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }
    */


    //0414
    private IEnumerator FadeAlpha(Image image, float startAlpha, float endAlpha, float duration)
    {
        if (image == null)
        {
            yield break;
        }

        Color color = image.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }

    private IEnumerator FadeTMPAlpha(TextMeshProUGUI tmp, float startAlpha = 1f, float endAlpha = 0f, float duration = 1f)
    {
        if (tmp == null)
        {
            yield break;
        }

        Color color = tmp.color;
        float elapsedTime = 0f;

        while (true)
        {
            // Fade out
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                tmp.color = color;
                yield return null;
            }

            // Fade in
            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Lerp(endAlpha, startAlpha, elapsedTime / duration);
                tmp.color = color;
                yield return null;
            }
        }
    }
}
