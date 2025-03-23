using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip rotateSound;

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
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMoveSound()
    {
        audioSource.PlayOneShot(moveSound);
    }

    public void PlayRotateSound()
    {
        audioSource.PlayOneShot(rotateSound);
    }

    public void PlayDropSound()
    {
        audioSource.PlayOneShot(dropSound);
    }
}
