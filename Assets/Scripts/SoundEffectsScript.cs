using UnityEngine;

public class SoundEffectsScript : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource effectsSource;

    public void Hover()
    {
        PlayClip(0);
    }

    public void Click()
    {
        PlayClip(1);
    }

    public void OnDice()
    {
        PlayClip(2);
    }

    void PlayClip(int index)
    {
        if (effectsSource == null || audioClips == null || index >= audioClips.Length || audioClips[index] == null)
            return;

        // Sound effect volume from the Settings scene.
        effectsSource.volume = GameSettings.SfxVolume;
        effectsSource.PlayOneShot(audioClips[index]);
    }
}
