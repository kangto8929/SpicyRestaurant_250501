using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class EndGameManager : MonoSingleton<EndGameManager>
{
    public GameObject EndPanel;

    //리셋하시겠습니까 패널이 나와있는지에 따라 바꿀 거
    public GameObject ResetPanel;
    public Image EndGameBlack;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(EndPanel.activeSelf == true)
            {
                //활성화 되어있는 상태
                EndPanel.SetActive(false);
            }

            else
            {
                //만약에 리셋 패널이 나와있는 상태라면
                if(ResetPanel.activeSelf == true)
                {
                    EndGameBlack.enabled = false;
                }

                //그렇지 않다면
                else
                {
                    EndGameBlack.enabled = true;
                }

                EndPanel.SetActive(true);

                if (ShapesManager.Instance != null)
                {
                    ShapesManager.Instance.PauseTimer();
                    ShapesManager.Instance.PauseBlockMovement();
                }
                else
                {
                    Debug.Log("ShapesManager가 현재 씬에 존재하지 않습니다.");
                }
            }

        }
    }

    public void EndGameYes()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // 에디터에서는 플레이 모드 종료
#else
    Application.Quit(); // 실제 빌드에서는 종료
#endif
    }

    public void Continue()
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);
        EndPanel.SetActive(false);

        if (ShapesManager.Instance != null)
        {
            ShapesManager.Instance.ResumeTimer();
            ShapesManager.Instance.ResumeBlockMovement();
        }
        else
        {
            Debug.Log("ShapesManager가 현재 씬에 존재하지 않습니다.");
        }
    }
}
