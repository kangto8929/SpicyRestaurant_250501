using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopOpenManager : MonoBehaviour
{
    public static ShopOpenManager Instance;

    [SerializeField]
    private TextMeshProUGUI DayText;
    [SerializeField]
    private Image fadeImage;
    [SerializeField]
    private float fadeDuration = 1f;

    private Sequence _dayTextSequence;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //0413
        if (SoundManager.Instance.matchBGMusic == null || !SoundManager.Instance.BGMmusicSource.isPlaying)
        {
            SoundManager.Instance.PlayBackgroundMusic();
        }

        /*if(SoundManager.Instance.BGMmusicSource.clip == null)
        {
            SoundManager.Instance.PlayBackgroundMusic();
        }*/
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.DoorOpenSound);

        
        StartStage();
    }

    private void StartStage()
    {
         _dayTextSequence = DOTween.Sequence();
         DayText.transform.position = new Vector3(-10f, 0, 0);
         DayText.text = $"Day {GameManager.Instance.CurrentLevel+1}";

        //이거 일단 여기로...
        UI_Shop.Instance.DayText.text = $"Day {GameManager.Instance.CurrentLevel + 1}";

        StartCoroutine(FadeIn());
         _dayTextSequence.Append(DayText.transform.DOMoveX(-0.02f, 2f).SetDelay(1f))//0, 2f
                         .Append(DayText.transform.DOMoveX(10f, 2f))
                         .Append(DOTween.To(x=>
                         {
                             fadeImage.gameObject.SetActive(false);
                             CustomerManager.Instance.GetCustomerInStage();
                         }, 0, 0, 0));
        


    }

    public void EndStage()
    {
        StartCoroutine(FadeAndLoadScene("ResultScene"));
    }

        private IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return FadeOut();
        SceneManager.LoadScene(sceneName);
        SoundManager.Instance.StopBackgroundMusic();
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
            color.a = Mathf.Lerp(0f, 1f, elapsedTime/fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
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

    private void OnDestroy()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.DoorCloseSound);
    }
}
