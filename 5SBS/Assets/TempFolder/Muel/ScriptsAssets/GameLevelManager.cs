using UnityEngine;

[CreateAssetMenu(fileName = "GameLevelManager", menuName = "Scriptable Objects/GameLevelManager")]
public class GameLevelManager : ScriptableObject
{
    [Header("Star")]
    public bool[,,] stars = new bool[1, 4, 3];
    public int TotalStars;
    public bool firstCutscene = false;
    public bool levelOneDone = false;

    [Header("Audio Settings")]
    [Range(-80f, 20f)] public float masterVolume = 0f;
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Graphics Settings")]
    public int qualityIndex = 2; // 0 = Low, 1 = Medium, etc.
    public int resolutionIndex = 8;
    public bool isFullScreen = true;

    public void SetStar(int levelIndex, int stageIndex, int starIndex, bool earned)
    {
        if (levelIndex >= 0 && levelIndex < stars.GetLength(0) &&
            stageIndex >= 0 && stageIndex < stars.GetLength(1) &&
            starIndex >= 0 && starIndex < stars.GetLength(2))
        {
            stars[levelIndex, stageIndex, starIndex] = earned;
            Debug.Log(levelIndex + " " + stageIndex + " " + starIndex);
        }
    }

    public bool GetStar(int levelIndex, int stageIndex, int starIndex)
    {
        if (levelIndex >= 0 && levelIndex < stars.GetLength(0) &&
            stageIndex >= 0 && stageIndex < stars.GetLength(1) &&
            starIndex >= 0 && starIndex < stars.GetLength(2))
        {
            return stars[levelIndex, stageIndex, starIndex];
        }
        return false;
    }

    public int GetTotalStars()
    {
        if (TotalStars != 0)
        {
            return TotalStars;
        }
        int total = 0;

        for (int l = 0; l < stars.GetLength(0); l++)          // Loop semua level
        {
            for (int s = 0; s < stars.GetLength(1); s++)      // Loop semua stage
            {
                for (int st = 0; st < stars.GetLength(2); st++) // Loop semua bintang
                {
                    if (stars[l, s, st])
                        total++;
                }
            }
        }

        return total;
    }

    public void ResetStars()
    {
        for (int l = 0; l < stars.GetLength(0); l++)
            for (int s = 0; s < stars.GetLength(1); s++)
                for (int st = 0; st < stars.GetLength(2); st++)
                    stars[l, s, st] = false;
    }

    public void ResetSettings()
    {
        masterVolume = 0f;
        bgmVolume = 1f;
        sfxVolume = 1f;

        qualityIndex = 2;      // default quality
        resolutionIndex = 0;   // default resolution
        isFullScreen = true;
    }
    
    public void ResetAllSettings()
    {
        ResetSettings();

        // Apply langsung ke game
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        QualitySettings.SetQualityLevel(qualityIndex);

        Debug.Log("Settings reset to default");
    }

}
