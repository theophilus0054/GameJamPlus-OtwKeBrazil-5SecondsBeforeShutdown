using UnityEngine;
using UnityEngine.UI;  // untuk akses komponen Image

public class StarDisplay : MonoBehaviour
{
    public Sprite[] star; // ganti ke Sprite agar bisa dipakai di Image UI
    public GameLevelManager gameData;
    public GameObject[] stages; // tiap stage punya 3 child image bintang

    // public StageSelectManager manager;

    // void Start()
    // {
    //     if (gameData.GetStar(0, 3, 0) && !gameData.levelOneDone)
    //     {
    //         gameData.levelOneDone = true;
    //         UnityEngine.SceneManagement.SceneManager.LoadScene("CutsceneOutro");
    //     }
    // }

    // void Update()
    // {
    //     if (gameData.GetStar(0, 0, 0))
    //     {
    //         manager.isStage2Unlocked = true;
    //     }

    //     if (gameData.GetStar(0, 1, 0))
    //     {
    //         manager.isStage3Unlocked = true;
    //     }

    //     if (gameData.GetStar(0, 2, 0))
    //     {
    //         manager.isStage4Unlocked = true;
    //     }

    //     for (int s = 0; s < stages.Length; s++) // loop semua stage
    //     {
    //         Transform stage = stages[s].transform; // ambil parent transform

    //         for (int i = 0; i < 3; i++) // 3 bintang per stage
    //         {
    //             // Ambil status bintang dari GameLevelManager
    //             bool hasStar = gameData.GetStar(0, s, i); // Level 1 (index 1), Stage s

    //             // Ambil child ke-i di dalam stage
    //             Image starImage = stage.GetChild(i).GetComponent<Image>();

    //             // Set sprite sesuai data
    //             starImage.sprite = hasStar ? star[1] : star[0];
    //         }
    //     }
    // }
}
