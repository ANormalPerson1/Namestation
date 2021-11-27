using UnityEngine;

namespace Namestation.Player
{
    public class SoundManager : PlayerComponent
    {
        [SerializeField] AudioClip buildBlockClip;
        [SerializeField] GameObject audioSourcePrefab;

        public override void Initialize()
        {
            base.Initialize();
        }

        public void PlayBuildingSound(Transform parent, Vector3 localPosition)
        {
            GameObject audioSourceGO = Instantiate(audioSourcePrefab, parent);
            audioSourceGO.transform.localPosition = localPosition;

            AudioSource audioSource = audioSourceGO.GetComponent<AudioSource>();
            float soundDuration = buildBlockClip.length + 0.5f;
            audioSource.PlayOneShot(buildBlockClip);
            Destroy(audioSource.gameObject, soundDuration);
        }
    }
}
