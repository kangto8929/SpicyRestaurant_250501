using System.IO;
using UnityEngine;

[System.Serializable]
public class EndingData
{
    public bool IsGoodEnding = false;
}

public class EndingStateManager : MonoBehaviour
{
    public static EndingStateManager Instance { get; private set; }

    private string savePath;
    private EndingData currentData = new EndingData();

    public int GoodEndingMoney = 8100;

    // 외부에서 접근 가능한 프로퍼티
    public bool IsGoodEnding
    {
        get => currentData.IsGoodEnding;
        set
        {
            currentData.IsGoodEnding = value;
            SaveEnding(); // 값이 바뀌면 자동 저장
        }
    }

    private void Awake()
    {
        // 싱글턴 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "endingData.json");
        LoadEnding();
    }

    // 저장
    public void SaveEnding()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("[저장]현재 굿 엔딩으로 갈 수 있는지?: " + json);
    }

    // 불러오기
    public void LoadEnding()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentData = JsonUtility.FromJson<EndingData>(json);
            Debug.Log("[불러오기]현재 굿 엔딩으로 갈 수 있는지?: " + json);
        }
        else
        {
            Debug.Log("엔딩 데이터 없음. 새로 생성함.");
            currentData = new EndingData();
        }
    }

    // 리셋
    public void ResetEnding()
    {
        IsGoodEnding = false;
        Debug.Log("엔딩 상태 리셋됨.");
    }
}
