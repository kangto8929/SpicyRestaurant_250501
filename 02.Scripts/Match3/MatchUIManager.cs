using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using NUnit.Framework;
using DG.Tweening;
using System.Linq;
using System.Runtime.ExceptionServices; // Add DOTween namespace

public class MatchUIManager : MonoBehaviour
{
    public static MatchUIManager Instance { get; private set; }

    public List<BlockCountUI> blockCountList;

    private Dictionary<string, BlockCountUI> blockCountUIs = new Dictionary<string, BlockCountUI>();

    [Header("게임 일시정지 관련")]
    [SerializeField]
    private Button pauseBtn;
    //[SerializeField]
    public GameObject PauseUI;
   // [SerializeField]
    public Transform PauseUIBG;
    
    public GameObject PauseUIBackground;

    [Header("게임 일시정지 내부 버튼")]
    [SerializeField]
    private Button resumeBtn;
    [SerializeField] 
    private Button optionBtn;
    [SerializeField]
    private Button endBtn;

    [Header("게임 오버 관련")]
    [SerializeField]
    private GameObject gameOverUI;
    [SerializeField]
    private GameObject gameOverImage;

    [Header("게임 시작 전 카운트 관련")]
    [SerializeField]
    private GameObject gameCountUI;
    [SerializeField]
    private Image countdownImage;

    [Header("재배치 관련")]
    [SerializeField]
    private GameObject gameReboardUI;
    [SerializeField]
    private GameObject gameReboardImage;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        pauseBtn.onClick.AddListener(OnClickPause);
        resumeBtn.onClick.AddListener(OnClikcResume);
        optionBtn.onClick.AddListener(OnClickOption);
        endBtn.onClick.AddListener(OnClickEnd);


    }

    private void Start()
    {

        InitializeBlockCountList();
        gameOverUI.SetActive(false);
        PauseUI.SetActive(false);
        gameReboardUI.SetActive(false);

    }

    private void OnClickPause()
    {
        

        // Play pop sound
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        // Scale up the pauseUI from zero to full size
        PauseUIBG.transform.localScale = Vector3.zero; // Start at zero scale
        PauseUI.SetActive(true); // Activate the UI
        PauseUIBG.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack); // Scale to full size with an easing effect

        ShapesManager.Instance.PauseTimer(); // Pause the timer in ShapesManager
        ShapesManager.Instance.PauseBlockMovement(); // Pause block movement in ShapesManager
    }
    #region 옵션 안 기능
    private void OnClikcResume()
    {
        // Play pop sound
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        // Scale down the pauseUI to zero before hiding it
        PauseUIBG.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            PauseUI.SetActive(false); // Deactivate the UI after the animation
            ShapesManager.Instance.ResumeTimer(); // Resume the timer in ShapesManager
            ShapesManager.Instance.ResumeBlockMovement(); // Resume block movement in ShapesManager
        });
    }

    private void OnClickOption()
    {
        // Play pop sound
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        UI_Option.Instance.BlackBackground.localScale = Vector3.one;//여기부터는 가능하게 해보자
        UI_Option.Instance.BackgroundBlackImage.enabled = false;//0418

       // UI_Option.Instance.GameOptionUI.SetActive(true);//
        UI_Option.Instance.BackgroundCanvasGroup.localScale = Vector3.one;

        //gameOptionUI.SetActive(true);
        PauseUIBackground.SetActive(false);//0418//일시정지 비활성
    }

    private void OnClickEnd()
    {
        // Play pop sound
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.popSound);

        PauseUI.SetActive(false);
        ShapesManager.Instance.GameOver();
    }
    #endregion


    private void InitializeBlockCountList()
    {
        // Filter out IngredientType.Stock from the ingredient list
        var ingredientList = GameManager.Instance.GetEnableIngredients()
            .Where(ingredient => ingredient != IngredientType.Stock)
            .ToList();

        int index = 0;

        foreach (var blockCountUI in blockCountList)
        {
            if (index < ingredientList.Count)
            {
                var ingredientType = ingredientList[index];
                blockCountUI.SetBlockType(ingredientType.ToString());
                blockCountUI.gameObject.SetActive(true); // Activate the block
                blockCountUIs[ingredientType.ToString()] = blockCountUI;
                index++;
            }
            else
            {
                blockCountUI.gameObject.SetActive(false); // Deactivate the block
            }
        }
    }

    public void UpdateBlockCount(string blockType, int count)
    {
        if (blockCountUIs.TryGetValue(blockType, out BlockCountUI blockCountUI))
        {
            blockCountUI.UpdateCount(count);
        }
        else
        {
            Debug.LogWarning($"Block type {blockType} not found in UI manager.");
        }
    }

    public void ShowGameOverUI()
    {
        gameOverUI.SetActive(true);
        gameOverImage.SetActive(true);
    
        // Reset the initial state of the gameOverImage
        gameOverImage.transform.localScale = Vector3.one; // Start scaled down
        gameOverImage.transform.localPosition = new Vector3(0, 0, 0); // Start off-screen

        // Animate the scale and position of the gameOverImage
        gameOverImage.transform.DOScale(Vector3.one*1.5f, 0.5f).SetEase(Ease.OutBack); // Scale up with a bounce effect
        gameOverUI.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack); // Move to the center
    }


    public void ShowReBoardUI()
    {
        gameReboardUI.SetActive(true);
        gameReboardImage.SetActive(true);

        gameReboardImage.transform.localScale = Vector3.zero; // Start scaled down
        gameReboardImage.transform.localPosition = new Vector3(0, 0, 0); // Start off-screen

        // Animate the scale and position of the gameOverImage
        gameReboardImage.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack); // Scale up with a bounce effect
        gameReboardImage.transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.OutBack); // Move to the center

    }

    // Add a method to hide the ReBoardUI
    public void HideReBoardUI()
    {
        gameReboardUI.SetActive(false);
    }

    //시작전 카운트
    public IEnumerator StartCountdown(int seconds)
    {
        // Play countdown sound
        if (SoundManager.Instance != null && SoundManager.Instance.countdownSound != null)
        {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.countdownSound);
        }
        gameCountUI.SetActive(true);

        for (int i = seconds; i > 0; i--)
        {
            countdownImage.sprite = Resources.Load<Sprite>($"UI/Ready_Count_{i}"); 
            yield return new WaitForSeconds(1f);
        }

        gameCountUI.SetActive(false);
    }
}
