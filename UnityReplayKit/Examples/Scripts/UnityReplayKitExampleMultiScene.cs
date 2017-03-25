using UnityEngine;

namespace Rarebyte.REK.Examples {

  public class UnityReplayKitExampleMultiScene : MonoBehaviour {

    [SerializeField]
    private float runDuration = 10;

    private float TimeInGame { get; set; }

    public void Update() {
      TimeInGame += Time.deltaTime;
      if (TimeInGame >= runDuration) {
        LoadScene("UnityReplayKitExampleMultiSceneNativeOverlay2");
      }
    }

    public void OnGUI() {
      GUI.color = Color.black;
      GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
      GUILayout.BeginVertical();
      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Label("Switching to next scene in " + Mathf.Max(0, (runDuration - TimeInGame)).ToString("00.00"));
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
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