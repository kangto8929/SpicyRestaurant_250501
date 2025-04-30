using UnityEngine;
using UnityEngine.UI;
using TMPro;    

public class BlockCountUI : MonoBehaviour
{
    public Image blockImage;
    public TextMeshProUGUI countText;

    private string blockType;

    public void SetBlockType(string type)
    {
        var ingredientImage = Resources.Load<Sprite>($"Sprites/{type}");
        if (ingredientImage == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            blockType = type;
            blockImage.sprite = ingredientImage;
        }

    }

    public void UpdateCount(int count)
    {
        countText.text = count.ToString();
    }
}
