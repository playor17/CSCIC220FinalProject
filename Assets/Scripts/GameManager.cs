using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton pattern

    public AudioClip startSceneMusic; // Music for the Start scene
    public AudioClip endSceneMusic; // Music for the End scene
    private AudioSource audioSource; // Audio source component

    public int maxHp = 3; // Maximum player HP
    public int playerHp;  // Current player HP
    public GameObject currentPlayer; // Stores the current player object

    public TextMeshProUGUI hpText; // UI Text for displaying HP

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist GameManager across scenes
            audioSource = GetComponent<AudioSource>();
            playerHp = maxHp; // Initialize player HP
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene load events
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from scene load events
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource == null) return;

        // Handle music based on the scene
        if (scene.name == "Start")
        {
            PlayMusic(startSceneMusic);
        }
        else if (scene.name == "End")
        {
            PlayMusic(endSceneMusic);
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                PlayMusic(startSceneMusic);
            }
        }

        // Reset HP only in Level1 scene
        if (scene.name == "Level1")
        {
            ResetHp();
        }

        // Update HP UI if the scene is not Start or End
        if (scene.name != "Start" && scene.name != "End")
        {
            UpdateHpUI();
        }
    }

    public void SetCurrentPlayer(GameObject player)
    {
        currentPlayer = player; // Store the current player object
    }

    public GameObject GetCurrentPlayer()
    {
        return currentPlayer; // Return the current player object
    }

    public void ChangeHp(int amount)
    {
        playerHp += amount;

        // Handle HP reaching 0
        if (playerHp <= 0)
        {
            playerHp = 0; // Set HP to 0
            SceneManager.LoadScene("Failed"); // Transition to the Failed scene
            return;
        }

        UpdateHpText(); // Update HP UI
    }

    public void ResetHp()
    {
        playerHp = maxHp; // Reset HP to maximum
        UpdateHpText(); // Update HP UI
    }

    public void RestartGame()
    {
        ResetHp(); // Reset HP
        SceneManager.LoadScene("Level1"); // Load the Level1 scene
    }

    private void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip) return; // Skip if the same music is already playing
        audioSource.clip = clip;
        audioSource.loop = true; // Enable loop
        audioSource.Play();
    }

    private void UpdateHpUI()
    {
        // Find the HPText object in the current scene
        GameObject hpTextObject = GameObject.Find("HPText"); // Name of the TextMeshPro object
        if (hpTextObject != null)
        {
            hpText = hpTextObject.GetComponent<TextMeshProUGUI>();
            UpdateHpText(); // Update the HP UI
        }
        else
        {
            Debug.LogWarning("HPText object not found in the current scene. This is expected if the scene doesn't require an HP UI.");
        }
    }

    private void UpdateHpText()
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {playerHp} / {maxHp}";
        }
    }
}
