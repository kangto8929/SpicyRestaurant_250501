using UnityEngine;
using TMPro;

public class UIPositionManager : MonoBehaviour
{
    public RectTransform titleText;
    public RectTransform loadingText;
    public TextMeshProUGUI loadingTMP;
    public RectTransform touchToStartText;

    private RectTransform canvasRect;
    private float baseLoadingTextHeight = 0f;

    void Start()
    {
        //0413
        SoundManager.Instance.StopBackgroundMusic();

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        if (loadingTMP != null)
        {
            loadingTMP.ForceMeshUpdate();
            baseLoadingTextHeight = loadingTMP.preferredHeight;
        }
    }

    void Update()
    {
        if (canvasRect == null) return;
        UpdateUIPositions();
    }

    void UpdateUIPositions()
    {
        // 기준점: 화면 중심 (anchoredPosition = Vector2.zero)
        /*float centerY = 0f;

        // 제목은 위로 띄움
        float titleY = centerY + 650f;

        // loadingText는 중심보다 아래에 배치
        float loadingTextY = centerY + 180f;//190

        // 텍스트 줄 수에 따라 touchToStartText를 더 아래로 밀기
        float extraHeight = 0f;
        if (loadingTMP != null)
        {
            loadingTMP.ForceMeshUpdate();
            float currentHeight = loadingTMP.preferredHeight;
            extraHeight = Mathf.Max(0, currentHeight - baseLoadingTextHeight);
        }

        float touchToStartY = loadingTextY - 100f - extraHeight;//100
        */
        float centerY = 0f;
        float titleY = centerY + 650f;
        float loadingTextY = centerY + 180f;

        int lineCount = loadingTMP.textInfo.lineCount;
        float lineSpacingOffset = (lineCount - 1) * 30f; // 원하는 만큼만 띄움

        float touchToStartY = loadingTextY - 100f - lineSpacingOffset;

        // 배치 적용
        if (titleText)
            titleText.anchoredPosition = new Vector2(titleText.anchoredPosition.x, titleY);

        if (loadingText)
            loadingText.anchoredPosition = new Vector2(loadingText.anchoredPosition.x, loadingTextY);

        if (touchToStartText)
            touchToStartText.anchoredPosition = new Vector2(touchToStartText.anchoredPosition.x, touchToStartY);
    }
}
