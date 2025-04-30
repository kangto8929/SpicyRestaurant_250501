using UnityEngine;
using System.IO;

using UnityEngine.SceneManagement;//

[System.Serializable]
public class StorySaveData
{
    public int savedIndex;
}

public class StorySaveManager : MonoBehaviour
{
    public static StorySaveManager Instance;

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "saveOpeingIndex.json");
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;//

    }


    //
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("씬 바뀜 확인: " + scene.name);

        // 1번 또는 2번일 때 음악 재생
        if (scene.name == "StoryStartScene" || scene.name == "StartScene")//StoryStartScene//StartScene
        {
            if (SoundManager.Instance.OpeningBGM != null &&
                !SoundManager.Instance.BGMmusicSource.isPlaying)
            {
                SoundManager.Instance.PlayOpenTitleMusic();
            }
        }

    }


    public void SaveIndex(int index)
    {
        StorySaveData data = new StorySaveData();
        data.savedIndex = index;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public int LoadIndex()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            StorySaveData data = JsonUtility.FromJson<StorySaveData>(json);

            Debug.Log("불러온 인덱스: " + data.savedIndex);

            return data.savedIndex;
        }

        Debug.Log("저장된 인덱스 없음, 기본값 0 반환");
        return 0;
    }


    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);  // 저장 파일 삭제
            Debug.Log("저장 파일 삭제 완료");

            if(Opening.Instance != null)
            {
                Opening.Instance.StartTyping();
            }
           
        }
        else
        {
            Debug.Log("삭제할 저장 파일이 없습니다");
        }

        int resetIndex = LoadIndex();  // 파일 삭제 후, 기본값 0을 반환받음
        Debug.Log("리셋된 인덱스: " + resetIndex);  // 0 반환
    }

    public bool HasSaveData()
    {
        return File.Exists(savePath);
    }
}
