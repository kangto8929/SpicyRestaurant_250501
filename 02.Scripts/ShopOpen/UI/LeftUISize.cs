using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeftUISize : MonoBehaviour
{
    //글자 수 증감에 따른 길이 텍스트 상자 길이 변경
    public static LeftUISize Instance;

    public RectTransform MoneyPanel;//돈
    public TextMeshProUGUI MoneyText;

    public RectTransform SatisfactionPanel;//만족도
    public TextMeshProUGUI SatisfactionText;

    private string _lastMoneyText = "";
    private string _lastSatisfactionText = "";

    private void FixedUpdate()
    {
        _lastMoneyText = MoneyText.text;
        _lastSatisfactionText = SatisfactionText.text;
        UpdateMoneySize();

        UpdateSatisfactionSize();
    }


    //처음에 사이즈 확인
    //이후에는 텍스트가 변경되었을 때 불러오는 걸로
    /*private void Start()
    {
        Instance = this;

        _lastMoneyText = MoneyText.text;
        _lastSatisfactionText = SatisfactionText.text;
        UpdateMoneySize();
        UpdateSatisfactionSize();
    }*/

    //돈
    public void UpdateMoneySize()
    {
        //텍스트 여백 설정
        // 텍스트 여백 설정 (왼쪽, 위, 오른쪽, 아래)
        MoneyText.margin = new Vector4(35, 0, 10, 0);

        float contentWidth = MoneyText.preferredWidth;
        float totalWidth = contentWidth + 45;

        Vector2 size = MoneyPanel.sizeDelta;
        size.x = totalWidth;
        MoneyPanel.sizeDelta = size;
    }

    //행복도
    public void UpdateSatisfactionSize()
    {
        //텍스트 여백 설정
        // 텍스트 여백 설정 (왼쪽, 위, 오른쪽, 아래)
        SatisfactionText.margin = new Vector4(25, 0, 17, 0);

        float contentWidth = SatisfactionText.preferredWidth;
        float totalWidth = contentWidth + 42;

        Vector2 size = SatisfactionPanel.sizeDelta;
        size.x = totalWidth;
        SatisfactionPanel.sizeDelta = size;
    }

}
