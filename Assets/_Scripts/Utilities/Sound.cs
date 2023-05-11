using UnityEngine;
using UnityEngine.SceneManagement;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip GamePlayScene;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        
        audioSource.Play();
        
        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Gameplay")
        {
            audioSource.clip = GamePlayScene;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
