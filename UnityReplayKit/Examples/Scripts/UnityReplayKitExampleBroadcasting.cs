using UnityEngine;

namespace Rarebyte.REK.Examples {

  public class UnityReplayKitExampleBroadcasting : MonoBehaviour {


    public void Start() {
      UnityReplayKit.Instance.BroadcastServiceSelected += () => Debug.Log("Broadcast service selected");
      UnityReplayKit.Instance.BroadcastServiceSelectionFailed += (error) => Debug.Log("Broadcast selection failed: " + error.ToString());
      UnityReplayKit.Instance.BroadcastStarted += () => Debug.Log("Broadcast started");
      UnityReplayKit.Instance.BroadcastStartFailed += (error) => Debug.Log("Broadcast failed to start: " + error.ToString());
      UnityReplayKit.Instance.BroadcastStopped += () => Debug.Log("Broadcast stopped successfully");
      UnityReplayKit.Instance.BroadcastStopFailed += (error) => Debug.Log("Broadcast failed to stop: " + error.ToString());
      UnityReplayKit.Instance.BroadcastFinished += () => Debug.Log("Broadcast finished");
      UnityReplayKit.Instance.BroadcastFinishFailed += (error) => Debug.Log("Broadcast failed to finish: " + error.ToString());
    }

    public void OnGUI() {
      GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
      GUILayout.BeginHorizontal();
      GUILayout.BeginVertical();

      float buttonHeight = 0.1f * Screen.height;
      GUI.contentColor = Color.black;
      GUILayout.Label("reK - Broadcasting (Live Stream)");

      GUILayout.Label("IsReplayKitAvailable: " + UnityReplayKit.Instance.IsReplayKitAvailable.ToString());
      GUILayout.Label("IsBroadcastAvailable: " + UnityReplayKit.Instance.IsBroadcastAvailable.ToString());

      if (UnityReplayKit.Instance.IsBroadcasting) {

        if (UnityReplayKit.Instance.IsBroadcastPaused) {
          if (GUILayout.Button("Resume", GUILayout.Height(buttonHeight))) {
            UnityReplayKit.Instance.ResumeBroadcast();
          }
        } else {
          if (GUILayout.Button("Pause", GUILayout.Height(buttonHeight))) {
            UnityReplayKit.Instance.PauseBroadcast();
          }
        }
        GUILayout.Label("Is Paused: " + UnityReplayKit.Instance.IsBroadcastPaused);

        if (GUILayout.Button("Stop", GUILayout.Height(buttonHeight))) {
          UnityReplayKit.Instance.StopBroadcast();
        }
      } else {
        if (GUILayout.Button("Select service", GUILayout.Height(buttonHeight))) {
          UnityReplayKit.Instance.SelectBroadcastingService();
        }
        if (GUILayout.Button("Start", GUILayout.Height(buttonHeight))) {
          UnityReplayKit.Instance.StartBroadcast();
        }
      }

      GUILayout.FlexibleSpace();

      GUILayout.Label("Game Time: " + Time.time.ToString("00.00") + "s");

      GUILayout.EndVertical();
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.EndArea();
    }
  }
}