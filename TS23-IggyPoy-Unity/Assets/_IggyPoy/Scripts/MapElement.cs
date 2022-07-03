using UnityEngine;

[SelectionBase]
public class MapElement : MonoBehaviour
{
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AttackController attackController;

    private float pitchModification = 0.3f;
    
    public void PlayWalkAudio()
    {
        walkAudioSource.pitch = Random.Range(1-pitchModification, 1+pitchModification);
        walkAudioSource.Play();
    }

    public void PlayAttackAudio()
    {
        attackAudioSource.pitch = Random.Range(1-pitchModification, 1+pitchModification);
        attackAudioSource.Play();
        if (attackController != null)
            attackController.AttackTarget();
    }
}