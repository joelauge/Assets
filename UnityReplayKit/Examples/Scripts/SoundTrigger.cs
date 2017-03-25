using UnityEngine;

namespace Rarebyte.REK.Examples {

  public class SoundTrigger : MonoBehaviour {

    private AudioSource audioSource;

    public void Awake() {
      audioSource = GetComponent<AudioSource>();
    }

    public void Update() {
      if (Input.GetMouseButtonDown(0)) {
        audioSource.Play();
      }
    }

  }

}
