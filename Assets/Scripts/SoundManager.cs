using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;

    private AudioSource controlClips;

    private void Awake()
    {
        controlClips = GetComponent<AudioSource>();
    }

    public void SelectClip(int index, float volume)
    {
        controlClips.PlayOneShot(audioClips[index], volume);
    }
}
