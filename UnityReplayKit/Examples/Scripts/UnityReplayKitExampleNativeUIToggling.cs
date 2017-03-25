using UnityEngine;
using System.Collections;

namespace Rarebyte.REK.Examples {

  public class UnityReplayKitExampleNativeUIToggling : MonoBehaviour {

    public void Start() {
      Debug.Log("Disabling native UI");

      UnityReplayKit.Instance.IsReplayKitNativeUIVisible = false;
      StartCoroutine(EnableNativeUI());
    }

    private IEnumerator EnableNativeUI() {
      yield return new WaitForSeconds(5f);

      Debug.Log("Enabling native UI");
      UnityReplayKit.Instance.IsReplayKitNativeUIVisible = true;
    }

    public void OnGUI() {
      GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

      bool isReplayKitAvailable = UnityReplayKit.Instance.IsReplayKitAvailable;
#if UNITY_EDITOR
      isReplayKitAvailable = true;
#endif

      if (isReplayKitAvailable) {

        if (GUILayout.Button("Toggle Native Overlay (" + (UnityReplayKit.Instance.IsReplayKitNativeUIVisible ? "On" : "Off") + ")", GUILayout.Width(Screen.width * 0.2f), GUILayout.Height(Screen.height * 0.2f))) {
          UnityReplayKit.Instance.IsReplayKitNativeUIVisible = !UnityReplayKit.Instance.IsReplayKitNativeUIVisible;
        }

      } else {
        GUI.contentColor = Color.black;
        GUILayout.Label("ReplayKit is not available");
      }

      GUILayout.EndArea();
    }
  }

}