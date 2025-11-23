using UnityEngine;
using UnityEngine.UI;

public class WinScreenController : MonoBehaviour
{
    // Biar muncul di Editor tapi ga sebagai public
    [Header("UI References")]
    [SerializeField] private Image[] starsArray;
    [SerializeField] private Sprite starLit, starUnlit;
    [SerializeField] private Button retryButton, nextLevelButton, stageButton;
    [SerializeField] private Text winScreenText;
    [Header("Stars")]
    public bool mainStar = false;
    public bool star2 = false;
    public bool star3 = false;
    public GameLevelManager gameData;

    private void Awake()
    {
        // Default setup
        SetStars(false, false, false, 1);
        winScreenText.text = "Level Completed!";

        // Button listeners
        // retryButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        // mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    void Start()
    {
        
    }

    public void Show(bool star1, bool star2, bool star3, int stage)
    {
        gameObject.SetActive(true);
        SetStars(star1, star2, star3, stage);
    }
    private void SetStars(bool star1, bool star2, bool star3, int stage)
    {
        // Fungsi dipisah bisi mau nambah animation or some shit
        starsArray[0].sprite = star1 ? starLit : starUnlit;
        starsArray[1].sprite = star2 ? starLit : starUnlit;
        starsArray[2].sprite = star3 ? starLit : starUnlit;

        if(star1) gameData.SetStar(0,stage,0,star1);
        if(star2) gameData.SetStar(0,stage,1,star2);
        if(star3) gameData.SetStar(0,stage,2,star3);
    }
}
