using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Text;
//using static UnityEditor.Progress;

public class ShapesManager : MonoBehaviour
{
    public static ShapesManager Instance { get; private set; }

    public TextMeshProUGUI DebugText, ScoreText;
    public bool ShowDebugInfo = false;
    public ShapesArray shapes;

    private int score;
    private int comboCount; // Add a combo counter

    private Vector2 BottomRight;
    public Vector2 CandySize;

    [SerializeField]
    private GameState state = GameState.None;
    private GameObject hitGo = null;
    private Vector2[] SpawnPositions;
    public GameObject[] BlockPrefabs;
    public GameObject[] ExplosionPrefabs;
    public GameObject[] BonusPrefabs;

    private List<GameObject> filteredBlockPrefabs; // Filtered list of BlockPrefabs

    private IEnumerator CheckPotentialMatchesCoroutine;
    private IEnumerator AnimatePotentialMatchesCoroutine;

    IEnumerable<GameObject> potentialMatches;
     
    public SoundManager soundManager;

    public ConstantsSO ConstantsData;

    private float timeRemaining;
    public TextMeshProUGUI TimerText; // Add a UI element to display the timer
    //public TextMeshProUGUI ComboText; // Add a UI element to display the combo count

    private Dictionary<string, int> destroyedBlocksCount; // Add a dictionary to track destroyed blocks

    private Coroutine gameTimerCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DebugText.enabled = ShowDebugInfo;


    }

    // Use this for initialization
    void Start()
    {
        GameManager.Instance.LoadLevelData();

        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 60;

        state = GameState.None; // Ensure the game state is set to None when the game starts
        BottomRight = gameObject.transform.position;

        // Start the countdown before initializing the game
        StartCoroutine(StartGameAfterCountdown());
    }

    private IEnumerator StartGameAfterCountdown()
    {
        yield return MatchUIManager.Instance.StartCountdown(3);

        timeRemaining = ConstantsData.GameTimeLimit; 
        UpdateTimerUI();
        destroyedBlocksCount = new Dictionary<string, int>(); 
        state = GameState.None;
        InitializeTypesOnPrefabShapesAndBonuses();
        InitializeCandyAndSpawnPositions();
        StartCheckForPotentialMatches();
        gameTimerCoroutine = StartCoroutine(GameTimer());

        //0413
        if (SoundManager.Instance.matchBGMusic == null || !SoundManager.Instance.BGMmusicSource.isPlaying)
        {
            SoundManager.Instance.PlayBackgroundMusic();
        }
        //SoundManager.Instance.PlayBackgroundMusic();//0413z
    }

    /// <summary>
    /// Initialize shapes
    /// </summary>
    private void InitializeTypesOnPrefabShapesAndBonuses()
    {
        // Get enabled ingredient types from GameManager
        var enabledIngredients = GameManager.Instance.GetEnableIngredients();

        foreach (var item in BlockPrefabs)
        {
            item.GetComponent<Shape>().Type = item.name;
        }

        // Filter BlockPrefabs based on enabled ingredient types
        filteredBlockPrefabs = BlockPrefabs
            .Where(prefab => enabledIngredients.Contains((IngredientType)System.Enum.Parse(typeof(IngredientType), prefab.GetComponent<Shape>().Type)))
            .ToList();

        foreach (var item in filteredBlockPrefabs)
        {
            item.GetComponent<Shape>().Type = item.name;
        }

        int index = 0;
        foreach (var item in BonusPrefabs)
        {
            item.GetComponent<Shape>().Type = null;
            item.GetComponent<Shape>().Bonus = BonusTypeUtilities.GetBonusType(index);
            index++;
        }
    }

    public void InitializeCandyAndSpawnPositionsFromPremadeLevel()
    {
        //InitializeVariables();

        var premadeLevel = DebugUtilities.FillShapesArrayFromResourcesData();

        if (shapes != null)
            DestroyAllCandy();

        shapes = new ShapesArray(ConstantsData);
        SpawnPositions = new Vector2[ConstantsData.Columns];

        for (int row = 0; row < ConstantsData.Rows; row++)
        {
            for (int column = 0; column < ConstantsData.Columns; column++)
            {
                GameObject newCandy = GetSpecificCandyOrBonusForPremadeLevel(premadeLevel[row, column]);
                InstantiateAndPlaceNewCandy(row, column, newCandy);
            }
        }

        SetupSpawnPositions();
    }

    public void InitializeCandyAndSpawnPositions()
    {
        //InitializeVariables();

        if (shapes != null)
            DestroyAllCandy();

        shapes = new ShapesArray(ConstantsData);
        SpawnPositions = new Vector2[ConstantsData.Columns];

        for (int row = 0; row < ConstantsData.Rows; row++)
        {
            for (int column = 0; column < ConstantsData.Columns; column++)
            {
                GameObject newCandy = GetRandomCandy();

                //check if two previous horizontal are of the same type
                while (column >= 2 && shapes[row, column - 1].GetComponent<Shape>()
                    .IsSameType(newCandy.GetComponent<Shape>())
                    && shapes[row, column - 2].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>()))
                {
                    newCandy = GetRandomCandy();
                }

                //check if two previous vertical are of the same type
                while (row >= 2 && shapes[row - 1, column].GetComponent<Shape>()
                    .IsSameType(newCandy.GetComponent<Shape>())
                    && shapes[row - 2, column].GetComponent<Shape>().IsSameType(newCandy.GetComponent<Shape>()))
                {
                    newCandy = GetRandomCandy();
                }

                InstantiateAndPlaceNewCandy(row, column, newCandy);
            }
        }

        SetupSpawnPositions();
    }

    private void InstantiateAndPlaceNewCandy(int row, int column, GameObject newCandy)
    {
        GameObject go = Instantiate(newCandy,
            BottomRight + new Vector2(column * CandySize.x, row * CandySize.y), Quaternion.identity)
            as GameObject;

        //assign the specific properties
        go.GetComponent<Shape>().Assign(newCandy.GetComponent<Shape>().Type, row, column);
        shapes[row, column] = go;
    }

    private void SetupSpawnPositions()
    {
        //create the spawn positions for the new shapes (will pop from the 'ceiling')
        for (int column = 0; column < ConstantsData.Columns; column++)
        {
            SpawnPositions[column] = BottomRight + new Vector2(column * CandySize.x, ConstantsData.Rows * CandySize.y);
        }
    }

    /// <summary>
    /// Destroy all candy gameobjects
    /// </summary>
    private void DestroyAllCandy()
    {
        for (int row = 0; row < ConstantsData.Rows; row++) // Updated to use ConstantsData
        {
            for (int column = 0; column < ConstantsData.Columns; column++) // Updated to use ConstantsData
            {
                Destroy(shapes[row, column]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ShowDebugInfo)
            DebugText.text = DebugUtilities.GetArrayContents(shapes);

        if (state == GameState.GameOver || state == GameState.Paused) // Prevent user input when the game is over or paused
        {
            return; // Prevent user input when the game is over
        }

        if (state == GameState.None)
        {
            // 사용자 입력을 처리합니다.
            if (Input.GetMouseButtonDown(0))
            {
                // 터치 위치를 가져옵니다.
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null) // 터치가 감지되었습니다.
                {
                    // 아이템을 터치했는지 확인합니다.
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Item"))
                    {
                        // 아이템을 터치하여 사용합니다.
                        StartCoroutine(OnClickItem(hit));
                        hitGo = null;
                        //state = GameState.None;
                    }
                    else
                    {
                        hitGo = hit.collider.gameObject;
                        state = GameState.SelectionStarted;
                    }
                }
            }
        }
        else if (state == GameState.SelectionStarted)
        {
            // 사용자가 드래그했습니다.
            if (Input.GetMouseButton(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hitGo != hit.collider.gameObject)
                {
                    StopCheckForPotentialMatches();

                    if (!Utilities.AreVerticalOrHorizontalNeighbors(hitGo.GetComponent<Shape>(), hit.collider.gameObject.GetComponent<Shape>()))
                    {
                        state = GameState.None;
                    }
                    else
                    {
                        state = GameState.Animating;
                        FixSortingLayer(hitGo, hit.collider.gameObject);
                        StartCoroutine(FindMatchesAndCollapse(hit));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Modifies sorting layers for better appearance when dragging/animating
    /// </summary>
    /// <param name="hitGo"></param>
    /// <param name="hitGo2"></param>
    private void FixSortingLayer(GameObject hitGo, GameObject hitGo2)
    {
        SpriteRenderer sp1 = hitGo.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitGo2.GetComponent<SpriteRenderer>();
        if (sp1.sortingOrder <= sp2.sortingOrder)
        {
            sp1.sortingOrder = 1;
            sp2.sortingOrder = 0;
        }
    }
    private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit2)
    {
        // Get the second item that was part of the swipe
        var hitGo2 = hit2.collider.gameObject;
        shapes.Swap(hitGo, hitGo2);

        // Play the swap sound
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.swapSound);

        // Move the swapped ones
        hitGo.transform.DOMove(hitGo2.transform.position, ConstantsData.AnimationDuration);
        hitGo2.transform.DOMove(hitGo.transform.position, ConstantsData.AnimationDuration);
        yield return new WaitForSeconds(ConstantsData.AnimationDuration);

        //get the matches via the helper methods
        var hitGomatchesInfo = shapes.GetMatches(hitGo);
        var hitGo2matchesInfo = shapes.GetMatches(hitGo2);

        var totalMatches = hitGomatchesInfo.MatchedCandy
            .Union(hitGo2matchesInfo.MatchedCandy).Distinct();

        //if user's swap didn't create at least a 3-match, undo their swap
        if (totalMatches.Count() < ConstantsData.MinimumMatches)
        {
            hitGo.transform.DOMove(hitGo2.transform.position, ConstantsData.AnimationDuration);
            hitGo2.transform.DOMove(hitGo.transform.position, ConstantsData.AnimationDuration);
            yield return new WaitForSeconds(ConstantsData.AnimationDuration);

            shapes.UndoSwap();
        }

        //if more than 3 matches and no Bonus is contained in the line, we will award a new Bonus
        bool addBonus = totalMatches.Count() >= ConstantsData.MinimumMatchesForBonus &&
            !BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGomatchesInfo.BonusesContained) &&
            !BonusTypeUtilities.ContainsDestroyWholeRowColumn(hitGo2matchesInfo.BonusesContained);

        Shape hitGoCache = null;
        if (addBonus)
        {
            //get the game object that was of the same type
            var sameTypeGo = hitGomatchesInfo.MatchedCandy.Count() > 0 ? hitGo : hitGo2;
            hitGoCache = sameTypeGo.GetComponent<Shape>();
        }

        int timesRun = 1;

        while (totalMatches.Count() >= ConstantsData.MinimumMatches)
        {
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.matchSound);

            //remove matched items
            foreach (var item in totalMatches)
            {
                shapes.Remove(item);
                RemoveFromScene(item);
            }

            //check and instantiate Bonus if needed
            if (addBonus)
                CreateBonus(hitGoCache);

            addBonus = false;

            //get the columns that we had a collapse
            var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();

            //collapse the ones gone
            var collapsedCandyInfo = shapes.Collapse(columns);
            //create new ones
            var newCandyInfo = CreateNewCandyInSpecificColumns(columns);

            int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);

            MoveAndAnimate(newCandyInfo.AlteredCandy, maxDistance);
            MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);

            //will wait for both of the above animations
            yield return new WaitForSeconds(ConstantsData.MoveAnimationMinDuration * maxDistance);

            //search if there are matches with the new/collapsed items
            totalMatches = shapes.GetMatches(collapsedCandyInfo.AlteredCandy)
                .Union(shapes.GetMatches(newCandyInfo.AlteredCandy)).Distinct();

            timesRun++;
            comboCount++; // Increase combo count for each consecutive match
            UpdateComboUI(); // Update the combo UI
        }

        comboCount = 0; // Reset combo count when no more matches are found
        UpdateComboUI(); // Update the combo UI
        state = GameState.None;
        StartCheckForPotentialMatches();
    }

    /// <summary>
    /// Creates a new Bonus based on the shape parameter
    /// </summary>
    /// <param name="hitGoCache"></param>
    private void CreateBonus(Shape hitGoCache)
    {
        GameObject Bonus = Instantiate(GetRandomBonus(), BottomRight
            + new Vector2(hitGoCache.Column * CandySize.x,
            hitGoCache.Row * CandySize.y), Quaternion.identity);

        shapes[hitGoCache.Row, hitGoCache.Column] = Bonus;

        var BonusShape = Bonus.GetComponent<Shape>();
        string bonusName = Bonus.name.Replace("(Clone)", "").Trim();
        int bonusIndex = System.Array.FindIndex(BonusPrefabs, item => item.name == bonusName);

        if (bonusIndex >= 0)
            BonusShape.Bonus = BonusTypeUtilities.GetBonusType(bonusIndex);

        BonusShape.Assign(null, hitGoCache.Row, hitGoCache.Column, BonusShape.Bonus);
    }

    /// <summary>
    /// Spawns new candy in columns that have missing ones
    /// </summary>
    /// <param name="columnsWithMissingCandy"></param>
    /// <returns>Info about new candies created</returns>
    private AlteredCandyInfo CreateNewCandyInSpecificColumns(IEnumerable<int> columnsWithMissingCandy)
    {
        AlteredCandyInfo newCandyInfo = new AlteredCandyInfo();

        //find how many null values the column has
        foreach (int column in columnsWithMissingCandy)
        {
            var emptyItems = shapes.GetEmptyItemsOnColumn(column);
            foreach (var item in emptyItems)
            {
                var go = GetRandomCandy();
                GameObject newCandy = Instantiate(go, SpawnPositions[column], Quaternion.identity)
                    as GameObject;

                newCandy.GetComponent<Shape>().Assign(go.GetComponent<Shape>().Type, item.Row, item.Column);

                if (ConstantsData.Rows - item.Row > newCandyInfo.MaxDistance)
                    newCandyInfo.MaxDistance = ConstantsData.Rows - item.Row;

                shapes[item.Row, item.Column] = newCandy;
                newCandyInfo.AddCandy(newCandy);
            }
        }
        return newCandyInfo;
    }

    /// <summary>
    /// Animates gameobjects to their new position
    /// </summary>
    /// <param name="movedGameObjects"></param>
    private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance)
    {
        foreach (var item in movedGameObjects)
        {
            item.transform.DOMove(
                BottomRight + new Vector2(item.GetComponent<Shape>().Column * CandySize.x,
                    item.GetComponent<Shape>().Row * CandySize.y),
                ConstantsData.MoveAnimationMinDuration * distance
            );
        }
    }

    /// <summary>
    /// Destroys the item from the scene and instantiates a new explosion gameobject
    /// </summary>
    /// <param name="item"></param>
    private void RemoveFromScene(GameObject item)
    {
        string blockType = item.GetComponent<Shape>().Type;
        int count = Random.Range(3, 5);

        for (int i = 0; i < count; i++)
        {
            //GameObject popEffect = PoolManager.Instance.GetObject(EObjectType.Pop);
            GameObject popEffect = PoolManager.Instance.GetObject(EObjectType.Pop, blockType);
            popEffect.transform.position = item.transform.position;
            StartCoroutine(ReturnToPoolAfterDelay(popEffect, 1f));
        }

        GameObject explosion = GetRandomExplosion();
        var newExplosion = Instantiate(explosion, item.transform.position, Quaternion.identity) as GameObject;
        Destroy(newExplosion, ConstantsData.ExplosionDuration);

        if (blockType != null) 
        {
            if (destroyedBlocksCount.ContainsKey(blockType))
            {
                destroyedBlocksCount[blockType]++;
            }
            else
            {
                destroyedBlocksCount[blockType] = 1;
            }
            MatchUIManager.Instance.UpdateBlockCount(blockType, destroyedBlocksCount[blockType]); 
        }

        Destroy(item);
    }

    /// <summary>
    /// Returns the object to the pool after a delay
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset the position of the object before returning it to the pool
        obj.transform.position = Vector3.zero;

        PoolManager.Instance.ReturnObject(obj, obj.GetComponent<Fragment>().GetObjectType());
    }

    /// <summary>
    /// Get a random candy
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomCandy()
    {
        return filteredBlockPrefabs[Random.Range(0, filteredBlockPrefabs.Count)];
    }

    private void InitializeVariables()
    {
        score = 0;
        ShowScore();
    }

    private void IncreaseScore(int amount)
    {
        // Apply combo multiplier to the score
        score += amount * (comboCount + 1);
        ShowScore();
    }

    private void ShowScore()
    {
        ScoreText.text = "Score: " + score.ToString();
    }

    /// <summary>
    /// Get a random explosion
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomExplosion()
    {
        return ExplosionPrefabs[Random.Range(0, ExplosionPrefabs.Length)];
    }

    /// <summary>
    /// Gets the specified Bonus for the specific type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetBonusFromType(string type)
    {
        foreach (var item in BonusPrefabs)
        {
            if (item.GetComponent<Shape>().Type == type)
                return item;
        }
        throw new System.Exception("Wrong type");
    }

    private GameObject GetRandomBonus()
    {
        var item = BonusPrefabs[Random.Range(0, BonusPrefabs.Length)];
        return item;
    }

    /// <summary>
    /// Starts the coroutines, keeping a reference to stop later
    /// </summary>
    private void StartCheckForPotentialMatches()
    {
        StopCheckForPotentialMatches();
        //get a reference to stop it later
        CheckPotentialMatchesCoroutine = CheckPotentialMatches();
        StartCoroutine(CheckPotentialMatchesCoroutine);
    }

    /// <summary>
    /// Stops the coroutines
    /// </summary>
    private void StopCheckForPotentialMatches()
    {
        if (AnimatePotentialMatchesCoroutine != null)
            StopCoroutine(AnimatePotentialMatchesCoroutine);
        if (CheckPotentialMatchesCoroutine != null)
            StopCoroutine(CheckPotentialMatchesCoroutine);
        ResetOpacityOnPotentialMatches();
    }

    /// <summary>
    /// Resets the opacity on potential matches (probably user dragged something?)
    /// </summary>
    private void ResetOpacityOnPotentialMatches()
    {
        if (potentialMatches != null)
            foreach (var item in potentialMatches)
            {
                if (item == null) break;

                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = 1.0f;
                item.GetComponent<SpriteRenderer>().color = c;
            }
    }

    // Add a new method to handle the ReBoardUI logic
    private IEnumerator ShowReBoardUI()
    {
        // Pause the timer and block movement
        PauseTimer();
        PauseBlockMovement();

        // Show the ReBoardUI
        MatchUIManager.Instance.ShowReBoardUI();

        // Play the ReBoard sound
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.reBoardSound);

        // Wait for the ReBoardUI animation to complete (e.g., 2 seconds)
        yield return new WaitForSeconds(2f);

        // Hide the ReBoardUI
        MatchUIManager.Instance.HideReBoardUI();

        // Resume the timer and block movement
        ResumeTimer();
        ResumeBlockMovement();
    }

    /// <summary>
    /// Finds potential matches
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckPotentialMatches()
    {
        yield return new WaitForSeconds(ConstantsData.WaitBeforePotentialMatchesCheck);
        potentialMatches = Utilities.GetPotentialMatches(shapes);

        if (potentialMatches == null || !potentialMatches.Any())
        {
            Debug.LogWarning("No potential matches found. Resetting the board.");
            ResetAndGenerateNewBoard(); // Trigger board reset
            yield break;
        }

        while (true)
        {
            AnimatePotentialMatchesCoroutine = Utilities.AnimatePotentialMatches(potentialMatches);
            StartCoroutine(AnimatePotentialMatchesCoroutine);
            yield return new WaitForSeconds(ConstantsData.WaitBeforePotentialMatchesCheck);
        }
    }

    // Add a new method to reset the board and generate a new one
    private void ResetAndGenerateNewBoard()
    {
        StartCoroutine(ResetAndGenerateNewBoardCoroutine());
    }

    // Add a coroutine to handle the reset and UI display sequence
    private IEnumerator ResetAndGenerateNewBoardCoroutine()
    {
        yield return StartCoroutine(ShowReBoardUI()); // Show the ReBoard UI before resetting the board

        StopCheckForPotentialMatches(); // Stop any ongoing match checks
        DestroyAllCandy(); // Remove all current blocks
        InitializeCandyAndSpawnPositions(); // Generate a new board
        StartCheckForPotentialMatches(); // Restart match checking
    }

    /// <summary>
    /// Gets a specific candy or Bonus based on the premade level information.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private GameObject GetSpecificCandyOrBonusForPremadeLevel(string info)
    {
        var tokens = info.Split('_');

        if (tokens.Count() == 1)
        {
            foreach (var item in BlockPrefabs)
            {
                if (item.GetComponent<Shape>().Type.Contains(tokens[0].Trim()))
                    return item;
            }
        }
        else if (tokens.Count() == 2 && tokens[1].Trim() == "B")
        {
            foreach (var item in BonusPrefabs)
            {
                if (item.name.Contains(tokens[0].Trim()))
                    return item;
            }
        }

        throw new System.Exception("Wrong type, check your premade level");
    }

    private IEnumerator OnClickItem(RaycastHit2D raycastHit2D)
    {
        if (raycastHit2D.collider == null) 
            yield break;

        var clickedItem = raycastHit2D.collider.gameObject;
        var shape = clickedItem.GetComponent<Shape>();

        if (shape == null) 
            yield break;

        // Play item use sound
        SoundManager.Instance.PlayItemUseSound(shape.Bonus);

        MatchesInfo matchesInfo = null;

        switch (shape.Bonus)
        {
            case BonusType.DestroyWholeRow:
                matchesInfo = shapes.GetHorizontalMatch(clickedItem);

                // Retrieve prefab from PoolManager and set position and rotation
                GameObject horizontalEffect = PoolManager.Instance.GetObject(EObjectType.Slash);
                horizontalEffect.transform.position = clickedItem.transform.position;
                horizontalEffect.transform.rotation = Quaternion.identity; // Rotate for horizontal effect
                //StartCoroutine(ReturnToPoolAfterDelay(horizontalEffect, 1f));
                break;

            case BonusType.DestroyWholeColumn:
                matchesInfo = shapes.GetVerticalMatch(clickedItem);

                // Retrieve prefab from PoolManager and set position and rotation
                GameObject verticalEffect = PoolManager.Instance.GetObject(EObjectType.Slash);
                verticalEffect.transform.position = clickedItem.transform.position;
                verticalEffect.transform.rotation = Quaternion.Euler(0, 0, 90); // Default rotation for vertical effect
                //StartCoroutine(ReturnToPoolAfterDelay(verticalEffect, 1f));
                break;

            case BonusType.DestroySurrounding:
                matchesInfo = shapes.GetSurroundingMatches(clickedItem);
                break;

            default:
                Debug.Log("Need New Type Progress");
                break;
        }

        if (matchesInfo == null || !matchesInfo.MatchedCandy.Any())
            yield break;

        // 매치된 아이템을 모두 제거합니다.
        var totalMatches = matchesInfo.MatchedCandy.Distinct();
        foreach (var item in totalMatches)
        {
            shapes.Remove(item);
            RemoveFromScene(item);
        }

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.matchSound);

        // 매치된 아이템의 행과 열을 가져옵니다.
        var rows = totalMatches.Select(go => go.GetComponent<Shape>().Row).Distinct();
        var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();

        // 사탕을 붕괴시키고 새로운 사탕을 생성합니다.
        var collapsedCandyInfo = shapes.Collapse(columns);
        var newCandyInfo = CreateNewCandyInSpecificColumns(columns);

        int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);

        MoveAndAnimate(newCandyInfo.AlteredCandy, maxDistance);
        MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);

        // 애니메이션이 끝난 후 매치 검사를 수행합니다.
        state = GameState.Animating; // Set state to Animating to block input
        yield return new WaitForSeconds(ConstantsData.MoveAnimationMinDuration * maxDistance);
        StartCoroutine(CheckForMatchesAfterAnimation(collapsedCandyInfo, newCandyInfo));
    }

    // 새로운 메서드 추가
    private IEnumerator CheckForMatchesAfterAnimation(AlteredCandyInfo collapsedCandyInfo, AlteredCandyInfo newCandyInfo)
    {
        var totalMatches = shapes.GetMatches(collapsedCandyInfo.AlteredCandy)
            .Union(shapes.GetMatches(newCandyInfo.AlteredCandy)).Distinct();

        // 추가적인 매치 검사를 수행합니다.
        while (totalMatches.Count() >= ConstantsData.MinimumMatches)
        {
            foreach (var item in totalMatches)
            {
                shapes.Remove(item);
                RemoveFromScene(item);
            }
            SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.matchSound);

            var rows = totalMatches.Select(go => go.GetComponent<Shape>().Row).Distinct();
            var columns = totalMatches.Select(go => go.GetComponent<Shape>().Column).Distinct();

            collapsedCandyInfo = shapes.Collapse(columns);
            newCandyInfo = CreateNewCandyInSpecificColumns(columns);

            int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);

            MoveAndAnimate(newCandyInfo.AlteredCandy, maxDistance);
            MoveAndAnimate(collapsedCandyInfo.AlteredCandy, maxDistance);

            yield return new WaitForSeconds(ConstantsData.MoveAnimationMinDuration * maxDistance);

            totalMatches = shapes.GetMatches(collapsedCandyInfo.AlteredCandy)
                .Union(shapes.GetMatches(newCandyInfo.AlteredCandy)).Distinct();
        }

        state = GameState.None; // Reset state to None to allow input
        StartCheckForPotentialMatches();
    }

    // Add a new coroutine for the game timer
    private IEnumerator GameTimer()
    {
        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            UpdateTimerUI();
        }
        GameOver();
    }

    // Update the timer UI
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60); // Calculate minutes
        int seconds = Mathf.FloorToInt(timeRemaining % 60); // Calculate seconds
        TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Format as 00:00
    }

    // Handle game over logic
    public void GameOver()
    {
        state = GameState.GameOver; // Set state to GameOver to block input
        StopCheckForPotentialMatches();
        DebugText.text = "Game Over!";

        // Play game over sound
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.gameOverSound);

        Debug.Log("Game Over");
        // Display the Game Over UI
        MatchUIManager.Instance.ShowGameOverUI();

        //저장 //0420
        //SaveManager.Instance.SaveCurrentGame();

        // Transition to the next scene after a delay
        StartCoroutine(TransitionToNextScene());//


        //민주 추가//딜레이 없게
        GameManager.Instance.SetIngredientManage(destroyedBlocksCount);
        SaveManager.SaveGame(GameManager.Instance.CurrentLevel, GameManager.Instance.PlayerCurrencyData, GameManager.Instance.GetIngredientData(), Scenes.ShopOpen);

        //수익과 잃은거 리셋
        CurrencyManager.Instance.DayChangeCurrencyInOutData();
        Debug.Log("현재 행복과 돈 유지. 하루 수익과 잃은거 초기화");

    }

    // Add a new coroutine to handle scene transition
    private IEnumerator TransitionToNextScene()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        // Load the next scene (replace "NextSceneName" with the actual scene name)
        //UnityEngine.SceneManagement.SceneManager.LoadScene("ShopOpen");
        LoadingSceneManager.LoadScene("ShopOpen", LoadingSceneTypes.Match3ToShopOpen);


        //0420
        Debug.Log("현재 재료:");
        var ingredientData = GameManager.Instance.GetIngredientData();
        foreach (var ingredient in ingredientData)
        {
            Debug.Log($"- {ingredient.Key}: {ingredient.Value}");
        }
        //저장된 거 확인 함

        //CustomerManager.Instance.DeleteCustomerSave();//손님 저장도 리셋
        //------------
    }

    // Add a method to update the combo UI
    private void UpdateComboUI()
    {
        //ComboText.text = "Combo: " + comboCount.ToString();
    }

    private void RestartGame()
    {
        state = GameState.None; // Ensure the game state is set to None when the game restarts
        StopAllCoroutines();
        Start();
    }

    // Add a method to resume the timer
    public void ResumeTimer()
    {
        if (gameTimerCoroutine == null)
        {
            gameTimerCoroutine = StartCoroutine(GameTimer());
        }
    }

    // Add a method to pause the timer
    public void PauseTimer()
    {
        if (gameTimerCoroutine != null)
        {
            StopCoroutine(gameTimerCoroutine);
            gameTimerCoroutine = null;
        }
    }

    public void PauseBlockMovement()
    {
        state = GameState.Paused; // Set the game state to Paused to block input
    }

    public void ResumeBlockMovement()
    {
        state = GameState.None; // Reset the game state to None to allow input
    }


    void OnDrawGizmos()
    {
        if (ConstantsData == null) return;

        Gizmos.color = Color.green;

        // Draw the grid
        for (int row = 0; row < ConstantsData.Rows; row++)
        {
            for (int column = 0; column < ConstantsData.Columns; column++)
            {
                Vector2 position = transform.position + new Vector3(column * CandySize.x, row * CandySize.y);
                Gizmos.DrawWireCube(position, CandySize);
            }
        }
    }
}
