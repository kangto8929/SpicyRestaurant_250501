using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // Add this import for DOTween
using System.IO;


[System.Serializable]
public class CustomerSaveData
{
    public int savedIndex;
    public int completedCustomers; // 현재까지 등장한 손님 수 저장
}

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;

    public List<GameObject> StageCustomerList;

    public List<GameObject> StageAllCustomerSpecialPrefabList;
   
    
    public List<GameObject> Stage2CustomerSpecialPrefabList;
    public List<GameObject> Stage3CustomerSpecialPrefabList;
    public List<GameObject> CustomerOnlyTextPrefabList;
    public GameObject CustomerNormalPrefab;
    public GameObject CustomerSpecialBeggarPrefab;
    public Transform CustomerParentTransform;

    [SerializeField]
    private int _customerNormalCountPerStage = 5;
    [SerializeField]
    private int _customerOnlyTextCountPerStage = 2;
    [SerializeField]
    private int _customerSpecialCountPerStage = 2;

    // [SerializeField]
    // private int _completedCustomers;
    // [SerializeField]
    // private int _maxCustomers;
    public int MaxCustomers = 6;
    public int CompletedCustomers;

    [SerializeField]
    public Vector3 CustomerSpawnPosition = new Vector3(0, -0.42f, 0);

    //private GameObject _instancedCustomer;

    public GameObject InstancedCustomer;

    private string savePath;//0420


    /*private void Update()//0417//현재 화면에 보이는게 몇 번째 손님인지
    {
        if (InstancedCustomer != null)
        {
            // 자식 포함해서 SpriteRenderer 찾기
            SpriteRenderer spriteRenderer = InstancedCustomer.GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                string spriteName = spriteRenderer.sprite.name;
                Debug.Log("현재 손님 스프라이트 이름: " + spriteName);
            }
        }
     }*/


    private void Awake()
    {
        if (Instance == null)
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
        savePath = Path.Combine(Application.persistentDataPath, "savecustomer.json");//0420

        System.Random random = new System.Random();
        switch (GameManager.Instance.CurrentLevel)
        {
            case 0:
                {
                    GameManager.Instance.RandomSpeicalPrefabsPerStage.AddRange(StageAllCustomerSpecialPrefabList);
                    break;
                }
            case 1:
                {
                    if(GameManager.Instance.RandomSpeicalPrefabsPerStage.Count <= 0)
                    {
                        GameManager.Instance.RandomSpeicalPrefabsPerStage.AddRange(StageAllCustomerSpecialPrefabList);
                    }
                    GameManager.Instance.RandomSpeicalPrefabsPerStage.AddRange(Stage2CustomerSpecialPrefabList);
                    break;
                }
            case 2:
                {
                    if (GameManager.Instance.RandomSpeicalPrefabsPerStage.Count <= 0)
                    {
                        GameManager.Instance.RandomSpeicalPrefabsPerStage.AddRange(Stage2CustomerSpecialPrefabList);
                    }
                    GameManager.Instance.RandomSpeicalPrefabsPerStage.AddRange(Stage3CustomerSpecialPrefabList);
                    break;
                }
        }
        MakeStage();

        UI_CustomerText.Instance.OnNextCustomer += DestroyCustomer;
        UI_CustomerText.Instance.OnNextCustomer += GetCustomerInStage;

        Debug.Log("손님 매니저에서 다시 로드 실행");


    }

    private void MakeStage()
    {
        // Customer_Normal 스테이지당 손님수만큼 추가하기
        for (int i = 0; i < _customerNormalCountPerStage; i++)
        {
            StageCustomerList.Add(CustomerNormalPrefab);
        }
        // Customer_OnlyText 스테이지당 손님수만큼 추가하기
        for (int i = 0; i < _customerOnlyTextCountPerStage; i++)
        {
            StageCustomerList.Add(RandomCustomerInList(CustomerOnlyTextPrefabList));
        }
        // Customer_Special 스테이지당 손님수만큼 추가하기
        for (int i = 0; i < _customerOnlyTextCountPerStage; i++)
        {
            StageCustomerList.Add(RandomCustomerInList(GameManager.Instance.RandomSpeicalPrefabsPerStage));
        }
    }
    // 랜덤으로 Prefab 꺼내기.
    private GameObject RandomCustomerInList(List<GameObject> customerList)
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(customerList.Count);
        GameObject randomCustomer = customerList[randomIndex];
        customerList.RemoveAt(randomIndex);
        return randomCustomer;
    }
    // 호출하면 랜덤 손님 생성.
    public void GetCustomerInStage()
    {
        if (StageCustomerList.Count <= 0)
        {
            StageCustomerList.Add(CustomerNormalPrefab);
        }
        GameObject randomCustomer = RandomCustomerInList(StageCustomerList);

        System.Random random = new System.Random();
        if (random.Next(2) == 0)
        {
            InstancedCustomer = Instantiate(randomCustomer, new Vector3(10, CustomerSpawnPosition.y, CustomerSpawnPosition.z), Quaternion.identity, CustomerParentTransform);
            InstancedCustomer.transform.DOMove(CustomerSpawnPosition, 1.0f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                UI_CustomerText.Instance.SetCustomerText();
            });
        }
        else
        {
            Vector3 startPosition = new Vector3(CustomerSpawnPosition.x, CustomerSpawnPosition.y - 1.0f, CustomerSpawnPosition.z);
            InstancedCustomer = Instantiate(randomCustomer, startPosition, Quaternion.identity, CustomerParentTransform);
            InstancedCustomer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            InstancedCustomer.transform.DOMove(CustomerSpawnPosition, 1.0f).SetEase(Ease.OutQuad);
            InstancedCustomer.transform.DOScale(Vector3.one, 1.0f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                UI_CustomerText.Instance.SetCustomerText();
            });
        }
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.customerArriveSound);
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.DoorOpenSound);

       // Debug.Log($"현재 손님 번호: {_completedCustomers + 1} / {_maxCustomers}");
    }


    public void DestroyCustomer()
    {
        CompletedCustomers++;
        Debug.Log("몇 번째 손님이 나올 차례인지?" + CompletedCustomers + "번째 손님");

       IngredientManager.Instance.SaveAllIngredientCounts();
       CurrencyManager.Instance.SaveCurrencyData();//재화 및 만족도 저장
 

        //손님 사라졌을 때 저장
        SaveCustomerIndex(CompletedCustomers);
;

        if (CompletedCustomers >= MaxCustomers) 
        {
            //GameManager.Instance.EndStage();
            ShopOpenManager.Instance.EndStage();//결과창
            //GameManager.Instance.SetCurrentLevel(GameManager.Instance.CurrentLevel+1);
            GameManager.Instance.EnabledIngredients = null;
            //_enabledIngredients = null;
            // GameManager.Instance.LoadLevelData();
            SaveManager.SaveGame((GameManager.Instance.CurrentLevel+1), GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.Match3Game);
   //+1없었음

            Debug.Log("손님 나오지 말고 결과창으로");
            Debug.Log("몇 번째 손님이길래 끝내는지?" + CompletedCustomers + "번째 손님");

            //손님 리셋
            DeleteCustomerSave();
           
        }

        else
        {
            //GameManager.Instance.EndStage();//추가0424
            //GameManager.Instance.SetCurrentLevel(GameManager.Instance.CurrentLevel);
            //GameManager.Instance.EnabledIngredients = null;
           // SaveManager.SaveGame(GameManager.Instance.GetCurrentLevel(), GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);
            SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);
            Debug.Log("손님 이어서 나오기");

            Debug.Log("몇 번째 손님이 나올 차례인지?" + CompletedCustomers + "번째 손님");

            CurrencyManager.Instance.SaveCurrencyInOutData();//오늘 하루동안 벌고 잃은거 저장
        }

        Destroy(InstancedCustomer.gameObject);

        //IngredientManager.Instance.SaveAllIngredientCounts();
         //CurrencyManager.Instance.SaveCurrencyData();//재화 및 만족도 저장

    }

    //몇 번째 손님인지 저장/불러오기
    public int LoadCustomerIndex()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CustomerSaveData data = JsonUtility.FromJson<CustomerSaveData>(json);

            Debug.Log("불러온 손님 인덱스: " + data.savedIndex);
            Debug.Log("현재 몇 번째 손님인지: " + data.completedCustomers); // 확인 로그

            // 손님 수 불러와서 적용
            CompletedCustomers = data.completedCustomers;

            // 만약 _completedCustomers가 6보다 크면 리셋
            if (CompletedCustomers > 6)
            {
                DeleteCustomerSave();
                //_completedCustomers = 0;
                Debug.Log("손님 수가 6보다 크므로 리셋되었습니다.");

                //수익과 잃은거 리셋
                CurrencyManager.Instance.DayChangeCurrencyInOutData();
                Debug.Log("현재 행복이랑 돈만 유지하고 바뀐거는 리셋");
            }

            return data.savedIndex;
        }

        Debug.Log("저장된 인덱스 없음, 기본값 0 반환");
        return 0;
    }

    public void SaveCustomerIndex(int index)
    {
        CustomerSaveData data = new CustomerSaveData();
        data.savedIndex = index;
        data.completedCustomers = CompletedCustomers; //  손님 수 저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    //손님 초기화
    public void DeleteCustomerSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);  // 저장 파일 삭제
            Debug.Log("손님 수 저장 파일 삭제 완료");
        }
        else
        {
            Debug.Log("삭제할 저장 파일이 없습니다");
        }

        CompletedCustomers = 0;
        Debug.Log("맞이한 손님 수 리셋 완료: 0");

        SaveCustomerIndex(0);
        int resetIndex = LoadCustomerIndex();  // 기본값 0 반환
        Debug.Log("리셋된 인덱스: " + resetIndex);  // 0
    }



}
