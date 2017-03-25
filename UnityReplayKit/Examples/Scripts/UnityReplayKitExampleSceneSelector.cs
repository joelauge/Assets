using UnityEngine;
using System.Collections;

namespace Rarebyte.REK.Examples {

  public class UnityReplayKitExampleSceneSelector : MonoBehaviour {

    public void OnGUI() {
      GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
      GUILayout.BeginVertical();
      GUILayout.FlexibleSpace();

      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.BeginVertical();
      GUILayout.Label("reK - Unity ReplayKit Plugin for iOS");
      GUILayout.Label("Example scene selection");
      GUILayout.EndVertical();
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();

      GUILayout.FlexibleSpace();

      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.BeginVertical();

      float width = Screen.width * 0.5f;
      float height = Screen.height * 0.16f;

      if (GUILayout.Button("Example 1: Native Overlay", GUILayout.Width(width), GUILayout.Height(height))) {
        LoadScene("UnityReplayKitExampleNativeOverlay");
      }
      if (GUILayout.Button("Example 2: Unity UI", GUILayout.Width(width), GUILayout.Height(height))) {
        LoadScene("UnityReplayKitExampleUnityUI");
      }
      if (GUILayout.Button("Example 3: NGUI (optional)", GUILayout.Width(width), GUILayout.Height(height))) {
        LoadScene("UnityReplayKitExampleNGUI");
      }

      if (GUILayout.Button("Example 4: Custom Code", GUILayout.Width(width), GUILayout.Height(height))) {
        LoadScene("UnityReplayKitExampleOnGui");
      }

      GUILayout.EndVertical();

      GUILayout.BeginVertical();

      if (GUILayout.Button("Example 5: Multiple Scenes with Native Overlay", GUILayout.Width(width), GUILayout.Height(height))) {
        LoadScene("UnityReplayKitExampleMultiSceneNativeOverlay1");
      }

      if (GUILayout.Button("Example 6: Toggle Native Overlay", GUILayout.Width(width), GUILayout.Height(height))) {
        LoadScene("UnityReplayKitExampleNativeOverlayToggling");
      }

      if (GUILayout.Button("Example 7: Broadcasting (Live Stream)", GUILayout.Width(width), GUILayout.Height(height))) {
        LoadScene("UnityReplayKitExampleBroadcasting");
      }

      GUILayout.EndVertical();
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();


      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
      GUILayout.EndArea();
    }

    private void LoadScene(string sceneName) {
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
    Application.LoadLevel(sceneName);
#else
      UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
#endif
    }
  }

}