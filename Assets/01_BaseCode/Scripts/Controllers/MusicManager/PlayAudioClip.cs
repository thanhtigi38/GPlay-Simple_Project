using UnityEngine;

public class PlayAudioClip : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    public void PlayAudioOneShot()
    {
        GameController.Instance.musicManager.PlaySingle(_audioClip);
    }
}
