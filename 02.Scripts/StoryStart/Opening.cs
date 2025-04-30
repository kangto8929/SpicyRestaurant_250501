using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Opening : MonoBehaviour
{
    public GameObject FadeImage;
    public static Opening Instance;

    public TextMeshProUGUI DialogueText;
    [TextArea(2, 5)]
    public string[] Sentences;
    public float TypingSpeed = 0.05f;

    // public으로 설정하여 Unity 인스펙터에서 값을 확인하고 수정할 수 있게 함
    public int CurrentIndex = 0;
    public bool IsTyping = false;

    private Coroutine _typingCoroutine;
    public Action Action;

    [SerializeField] private List<GameObject> _fireworkList;
    private Coroutine _fireworkRoutine;
    public GameObject Banner;



    private void Start()
    {
        Instance = this;

        Banner.SetActive(false);
        int savedIndex = StorySaveManager.Instance.LoadIndex();

        if (savedIndex >= 2)//2
        {
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            StorySaveManager.Instance.DeleteSave(); // 기존 데이터 삭제
            foreach (GameObject firework in _fireworkList)
            {
                if (firework != null)
                {
                    firework.SetActive(false);
                }
            }
            CurrentIndex = 0;

            
            if(IsTyping == false)
            {
                StartTyping();
            }

            if (UI_Option.Instance.FadeImage.activeSelf == true)
            {
                UI_Option.Instance.FadeImageAnimation.SetTrigger("Empty");
                UI_Option.Instance.FadeImageRaycast.raycastTarget = false;
                FadeImage.SetActive(false);

                StartCoroutine(FadeImageFalse());

                IEnumerator FadeImageFalse()
                {
                    yield return new WaitForSeconds(1.5f);
                    UI_Option.Instance.FadeImage.SetActive(false);
                }
            }

            else if (UI_Option.Instance.FadeImage.activeSelf == false)
            {
                FadeImage.SetActive(true);
            }

        }
    }

    public void StartTyping()
    {
        if (CurrentIndex < Sentences.Length)
        {
            _typingCoroutine = StartCoroutine(TypeSentence(Sentences[CurrentIndex]));
        }
        /* else
         {
             SceneManager.LoadScene("StartScene");
         }*/
    }

    IEnumerator TypeSentence(string sentence)
    {
        IsTyping = true;
        DialogueText.text = "";

        // 줄바꿈 처리
        sentence = sentence.Replace("\\\\", "\n");

        // 줄 기준 분할
        string[] lines = sentence.Split('\n');
        foreach (string line in lines)
        {
            string[] words = line.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                foreach (char letter in words[i])
                {
                    DialogueText.text += letter;

                    // 매 문자 출력 시마다 사운드를 계속 반복 재생
                    SoundManager.Instance.PlayRandomSoundInList(SoundManager.Instance.typeSounds);

                    yield return new WaitForSeconds(TypingSpeed);
                }

                // 마지막 단어가 아니라면 공백 추가
                if (i < words.Length - 1)
                {
                    DialogueText.text += ' ';
                }
            }

            // 줄바꿈 후 줄 구분
            DialogueText.text += '\n';
        }

        // 타이핑이 완료되면 isTyping을 false로 설정
        IsTyping = false;

    }

    public void CompleteTyping()
    {
        if (!IsTyping) return;

        // 코루틴 정지
        StopCoroutine(_typingCoroutine);

        // 문장 완성해서 출력
        string completedSentence = Sentences[CurrentIndex].Replace("\\\\", "\n");
        DialogueText.text = completedSentence;

        // 타이핑이 끝난 후 isTyping을 false로 설정
        IsTyping = false;
    }


    public void OnNextButtonClick()
    {
        if (IsTyping)
        {
            // 타이핑 중일 때는 타이핑을 완료
            CompleteTyping();
        }
        else
        {
            // 타이핑이 끝난 후 다음 문장으로 넘어감
            if (CurrentIndex < Sentences.Length - 1)
            {
               

                CurrentIndex++;

                if (CurrentIndex == 2)
                {
                    if (_fireworkRoutine != null)
                        StopCoroutine(_fireworkRoutine);
                    _fireworkRoutine = StartCoroutine(RandomFireworkLoop());

                    Banner.SetActive(true);
                    FadeImage.SetActive(false);

                    // 직접 호출
                    Banner.GetComponent<BannerScale>().AnimateBanner();
                }


                else
                {
                    Banner.SetActive(false);
                    FadeImage.SetActive(true);
                    foreach (GameObject firework in _fireworkList)
                    {
                        if (firework != null)
                        {
                            firework.SetActive(false);
                        }
                    }
                }

                    StartTyping();
                StorySaveManager.Instance.SaveIndex(CurrentIndex);//추가

            }
            else
            {
               /* Banner.SetActive(true);

                FadeImage.SetActive(false);

                foreach (GameObject firework in _fireworkList)
                {
                    if (firework != null)
                    {
                        firework.SetActive(false);
                    }
                }*/

                StorySaveManager.Instance.SaveIndex(CurrentIndex);//추가
                //여기부터 추가

                // 모든 문장이 끝나면 TitleScene으로 넘어감
                SceneManager.LoadScene("StartScene");
                Debug.Log("스타뜨 씬으로!");
                StorySaveManager.Instance.SaveIndex(CurrentIndex);//추가
            }
        }
    }
    public IEnumerator RandomFireworkLoop()
    {
        while (CurrentIndex == 2)
        {
            if (_fireworkList == null || _fireworkList.Count == 0)
                yield break;

            List<GameObject> activeFireworks = new List<GameObject>();
            int randomCount = UnityEngine.Random.Range(4, 5);
            List<GameObject> shuffled = new List<GameObject>(_fireworkList);
            ShuffleList(shuffled);

            for (int i = 0; i < randomCount; i++)
            {
                GameObject firework = shuffled[i];
                firework.SetActive(true);
                activeFireworks.Add(firework);

                Animator animator = firework.GetComponent<Animator>();
                animator.SetTrigger("Firework");
            }

            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.FireworkSound1);
            yield return new WaitForSeconds(1.2f);

            foreach (var firework in activeFireworks)
            {
                firework.SetActive(false);
            }

            // 다음 루프 전에 한 프레임 대기 (조건 다시 확인하게)
            yield return null;
        }
    }

    private void ShuffleList(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, list.Count);
            GameObject temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }
}
