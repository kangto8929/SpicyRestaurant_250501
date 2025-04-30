using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BowlMeatChecker : MonoBehaviour
{
    public static BowlMeatChecker Instance;
    public GameObject Bowl; // Bowl 오브젝트
    public Sprite BrownMeatSprite;

    private List<GameObject> _putMeatObjects = new List<GameObject>(); // GameObject 리스트로 변경

    void Start()
    {
        Instance = this;
    }

    public void CheckBowlChildren()
    {
        _putMeatObjects.Clear(); // 리스트 초기화

        Image[] childImages = Bowl.GetComponentsInChildren<Image>(true);

        foreach (Image img in childImages)
        {
            if (img.sprite != null && img.sprite.name == "put_meat")
            {
                _putMeatObjects.Add(img.gameObject); // GameObject를 저장
                Debug.Log("**덜 익은 고기 찾았다! " + img.gameObject.name);
            }
        }

        Debug.Log($"put_meat 스프라이트를 가진 자식 오브젝트 수: {_putMeatObjects.Count}");

    }

    public void ReplacePutMeatSprites()
    {
        if (BrownMeatSprite == null)
        {
            Debug.Log("익은 고기가 설정되어 있지 않음");
            return;
        }

        foreach (GameObject obj in _putMeatObjects)
        {
            if (obj != null)
            {
                Image img = obj.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = BrownMeatSprite;
                    Debug.Log("**익은 걸로 교체 완료: " + obj.name);
                }
                else
                {
                    Debug.LogWarning($"오브젝트 {obj.name} 에 Image 컴포넌트가 없습니다!");
                }
            }
        }

        Debug.Log("**덜 익은 거 모두 익은 고기로 교체 완료");
    }
}
