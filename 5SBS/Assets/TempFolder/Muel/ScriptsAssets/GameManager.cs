using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player")]
    public GameObject player;
    public Transform playerSpawnPoint;

    [Header("Dead Body Mechanic")]
    public TransformHistory transformHistory; // menyimpan posisi player & object
    public ObjectManager objectManager;       // spawn/despawn dead bodies

    [Header("Timer")]
    public float stageTime = 5f;
    [HideInInspector] public float currentTime;
    public TextMeshProUGUI timeText;

    [Header("WinState")]
    public GameObject uiWinScreen;

    [Header("Door")]
    public List<GameObject> doors;
    public Sprite openDoorSprite;
    public Sprite closedDoorSprite;

    private bool hasPlayerStartedMoving = false;
    private Stack<List<bool>> doorStateHistory = new Stack<List<bool>>();


    [Header("Level State")]
    private bool isLevelPaused = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        RespawnPlayer();
        ResetTimer();
        InitializeDoorStates();
    }

    private void InitializeDoorStates()
    {
        List<bool> initial = new List<bool>();

        foreach (var door in doors)
        {
            Collider2D c = door.GetComponent<Collider2D>();

            // jika collider aktif → pintu tertutup (false)
            // jika collider mati → pintu terbuka (true)
            bool isOpen = (c != null && c.enabled == false);
            initial.Add(isOpen);
        }

        // simpan sebagai state awal ke dalam stack
        doorStateHistory.Push(initial);
    }

    public void SetPlayerMoving()
    {
        hasPlayerStartedMoving = true;
    }


    public void TimeStart()
    {
        if (isLevelPaused || player == null || !hasPlayerStartedMoving) return;

        currentTime -= Time.deltaTime;

        UpdateTimerText(); // <--- Update UI tiap frame

        if (currentTime <= 0)
        {
            currentTime = 5;
            UpdateTimerText(); // pastikan 00:00 tampil
            OnPlayerDeath();
            RespawnPlayer();
            ResetTimer();
        }
    }

    void UpdateTimerText()
    {
        // Detik utuh
        int seconds = Mathf.FloorToInt(currentTime);

        // Milidetik (0–99)
        int milliseconds = Mathf.FloorToInt((currentTime - seconds) * 100);

        // Format menjadi: 05:32 (5 detik, 32 ms)
        timeText.text = $"{seconds:00}:{milliseconds:00}";
    }


    public void ResetTimer()
    {
        currentTime = 5;
        hasPlayerStartedMoving = false;
    }

    // ===========================================
    // PLAYER DEATH
    // ===========================================
    public void OnPlayerDeath()
    {
        if (player == null) return;

        if (transformHistory != null) transformHistory.SaveLog();

        player.SetActive(false);

        SaveDoorState();

    }

    // ===========================================
    // RESPAWN PLAYER
    // ===========================================
    public void RespawnPlayer()
    {
        if (player == null) return;
        Vector3 respawnPosition = playerSpawnPoint.position;
        player.transform.position = respawnPosition;
        player.SetActive(true);
    }

    // ===========================================
    // RESET LEVEL
    // ===========================================
    public void ResetLevel()
    {
        // Reset player
        if (player != null)
        {
            RespawnPlayer();
        }

        // Clear all spawned objects and dead bodies
        // if (objectManager != null)
        // {
        //     objectManager.ClearAllObjects();
        //     objectManager.ClearDeadBodies();
        // }

        if (doorStateHistory.Count > 0)
        {
            List<bool>[] array = doorStateHistory.ToArray();
            List<bool> initialState = array[array.Length - 1];
            ApplyDoorState(initialState);

            // bersihkan stack, simpan hanya initial
            doorStateHistory.Clear();
            doorStateHistory.Push(new List<bool>(initialState));
        }

        foreach (var door in doors) // Logic khusus untuk reset pintu yang tertutup semua pada awal level
        {
            if (door == null) continue;

            Collider2D col = door.GetComponent<Collider2D>();
            if (col != null && !col.enabled)
                col.enabled = true;

            // Jika ada SpriteRenderer → ganti sprite pintu tertutup
            SpriteRenderer sr = door.GetComponent<SpriteRenderer>();
            if (sr != null && closedDoorSprite != null)
                sr.sprite = closedDoorSprite;
        } // Belum ada logic untuk pintu yang terbuka pada awal level

        // Logic untuk reset dead bodies via TransformHistory
        if (transformHistory != null) transformHistory.ClearDeadBodies();

        // Clear TransformHistory
        if (transformHistory != null) transformHistory.ClearHistory();

        // Reset flags
        isLevelPaused = false;
        ResetTimer();
        UpdateTimerText();
    }

    // ===========================================
    // UNDO LAST ACTION
    // ===========================================

    public void UndoLastAction()
    {
        
        // Undo player & object positions
        if (transformHistory != null)
        {
            transformHistory.Undo();
        }

        // Undo door states
        if (doorStateHistory.Count > 0)
        {
            List<bool> lastState = doorStateHistory.Pop();
            ApplyDoorState(lastState);
        }

        RespawnPlayer();
        ResetTimer();
    }

    // Pause / Resume
    public void PauseGame()
    {
        isLevelPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isLevelPaused = false;
        Time.timeScale = 1f;
    }

    // ===========================================
    // WIN LEVEL
    // ===========================================
    
    public void OnPlayerWin()
    {
        PauseGame();

        if (uiWinScreen != null)
        {
            uiWinScreen.SetActive(true);
        }

        // Set MainManager Instance level completed ex: MainManager.Instance.SetLevel(currentLevelIndex, levelCompleted);
    }

    // ===========================================
    // DOOR INTERACTION
    // ===========================================

    public void doorInteraction(int doorIndex)
    {
        if (doorIndex < 0 || doorIndex >= doors.Count) return;

        GameObject door = doors[doorIndex];
        Collider2D col = door.GetComponent<Collider2D>();
        SpriteRenderer sr = door.GetComponent<SpriteRenderer>();

        if (col == null) return;

        bool newState = !col.enabled;

        col.enabled = newState;

        // FIX wajib
        Physics2D.SyncTransforms();

        if (sr != null)
            sr.sprite = newState ? openDoorSprite : closedDoorSprite;

        if (doorStateHistory.Count > 0)
        {
            var top = doorStateHistory.Pop();
            top[doorIndex] = newState;
            doorStateHistory.Push(top);
        }
    }



    private void SaveDoorState()
    {
        List<bool> snapshot = new List<bool>();

        for (int i = 0; i < doors.Count; i++)
        {
            var door = doors[i];
            var col = door.GetComponent<Collider2D>();

            // collider.enabled == false berarti pintu terbuka
            bool isOpen = col != null && col.enabled == false;
            snapshot.Add(isOpen);
        }

        doorStateHistory.Push(snapshot);
    }

    private void ApplyDoorState(List<bool> state)
    {
        for (int i = 0; i < doors.Count; i++)
        {
            GameObject door = doors[i];
            if (door == null) continue;

            bool isOpen = state[i];

            Collider2D col = door.GetComponent<Collider2D>();
            SpriteRenderer sr = door.GetComponent<SpriteRenderer>();

            if (col != null)
                col.enabled = !isOpen;   // collider off = pintu terbuka

            if (sr != null)
                sr.sprite = isOpen ? openDoorSprite : closedDoorSprite;
        }
    }



}