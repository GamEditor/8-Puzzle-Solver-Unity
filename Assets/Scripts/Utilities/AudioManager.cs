using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource m_AudioSource;
    public AudioClip m_FoundAudio;
    
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip audio)
    {
        if(audio != null)
            m_AudioSource.PlayOneShot(audio);
    }
}