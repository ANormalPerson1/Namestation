using UnityEngine;

public class SoundManager : PlayerComponent
{
    [SerializeField] AudioClip buildBlockClip;

    public override void Initialize()
    {
        base.Initialize();
    }

    public void PlayBuildingSound(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(buildBlockClip, position);
    }
}
