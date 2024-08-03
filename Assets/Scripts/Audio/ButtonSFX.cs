using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class ButtonSfx : MonoBehaviour
    {
        [SerializeField] private ESoundClip clip;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(()=> SoundSystem.Instance.PlaySFX(clip));
        }
    }
}
