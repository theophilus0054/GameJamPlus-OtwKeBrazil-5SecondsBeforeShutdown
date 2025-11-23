using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // When I add the comment *CHECKTHIS* please verify that the clip will be used in the game.

    [Header("Clips")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;
    public AudioClip walkingSound;
    public AudioClip jumpSound;
    public AudioClip charDiedSound; // "*CHECKTHIS*"
    public AudioClip gunShot;
    public AudioClip buttonSound;
    public AudioClip leverSound;
    public AudioClip doorSound;


    [Header("Pitch Settings")]
    [Range(0.5f, 2f)] public float minPitch = 0.90f;
    [Range(0.5f, 2f)] public float maxPitch = 1.1f;

    [Header("Limiter Settings")]
    [Tooltip("Minimum time (in seconds) between identical clip plays")]
    public float sameClipCooldown = 0.2f;
    [Tooltip("Volume multiplier when several same clips play close together")]
    [Range(0f, 1f)] public float overlapVolumeScale = 0.7f;

    private Dictionary<AudioClip, float> lastPlayTimes = new Dictionary<AudioClip, float>();

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ============================================================
    // 🔊 GENERIC PLAY METHODS
    // ============================================================
    public void Play(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;

        // Limiter
        if (lastPlayTimes.TryGetValue(clip, out float lastTime))
        {
            if (Time.time - lastTime < sameClipCooldown)
                return;
        }
        lastPlayTimes[clip] = Time.time;

        // Pitch & volume control
        source.pitch = Random.Range(minPitch, maxPitch);
        float elapsed = Time.time - lastTime;
        float volume = (elapsed < sameClipCooldown * 2f) ? overlapVolumeScale : 1f;

        source.PlayOneShot(clip, volume);
    }

    public void Play(AudioClip clip)
    {
        AudioSource source = gameObject.GetComponent<AudioSource>();
        if (source == null || clip == null) return;

        if (lastPlayTimes.TryGetValue(clip, out float lastTime))
        {
            if (Time.time - lastTime < sameClipCooldown)
                return;
        }
        lastPlayTimes[clip] = Time.time;

        source.pitch = Random.Range(minPitch, maxPitch);
        float elapsed = Time.time - lastTime;
        float volume = (elapsed < sameClipCooldown * 2f) ? overlapVolumeScale : 1f;

        source.PlayOneShot(clip, volume);
    }

    // ============================================================
    // 🧩 SHORTCUT METHODS
    // ============================================================
    public void PlayButtonClick(AudioSource source = null) => Play(source ?? GetMainSource(), buttonClick);
    public void PlayButtonHover(AudioSource source = null) => Play(source ?? GetMainSource(), buttonHover);
    public void PlayWalkingSound(AudioSource source = null) => Play(source ?? GetMainSource(), walkingSound);
    public void PlayJumpSound(AudioSource source = null) => Play(source ?? GetMainSource(), jumpSound);
    public void PlayCharDiedSound(AudioSource source = null) => Play(source ?? GetMainSource(), charDiedSound);
    public void PlayGunShot(AudioSource source = null) => Play(source ?? GetMainSource(), gunShot);
    public void PlayButtonSound(AudioSource source = null) => Play(source ?? GetMainSource(), buttonSound);
    public void PlayLeverSound(AudioSource source = null) => Play(source ?? GetMainSource(), leverSound);
    public void PlayDoorSound(AudioSource source = null) => Play(source ?? GetMainSource(), doorSound);

    

    // ============================================================
    // 🔧 Utility
    // ============================================================
    private AudioSource GetMainSource()
    {
        AudioSource src = gameObject.GetComponent<AudioSource>();
        if (src == null) src = gameObject.AddComponent<AudioSource>();
        return src;
    }
}
