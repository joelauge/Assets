using UnityEngine;

namespace Rarebyte.REK.Examples {

  public class UnityReplayKitExampleGameOnGui : MonoBehaviour {

    public enum GameState {
      MainMenu,
      Run,
      GameFinished,
    }

    private float TimeInGame { get; set; }

    private GameState CurrentGameState { get; set; }

    #region Inspector

    [SerializeField]
    private float runDuration = 10;

    [SerializeField]
    private GameObject goGame = null;

    [SerializeField]
    private bool showPreviewImmediately = false;

    #endregion

    public void Start() {
      CurrentGameState = GameState.MainMenu;
      goGame.SetActive(false);

      UnityReplayKit.Instance.PreStart += () => Debug.Log("Preparing...");
      UnityReplayKit.Instance.PreShowPreview += () => Debug.Log("Showing preview...");
      UnityReplayKit.Instance.PreStop += () => Debug.Log("Stopping...");
      UnityReplayKit.Instance.PreviewCompleted += () => Debug.Log("Preview completed");
      UnityReplayKit.Instance.Started += () => Debug.Log("Recording started");
      UnityReplayKit.Instance.StartFailed += (error) => Debug.Log("Unable to start recording: " + error.ToString());
      UnityReplayKit.Instance.Stopped += () => Debug.Log("Recording stopped");
      UnityReplayKit.Instance.StopFailed += (error) => Debug.Log("Unable to stop recording: " + error.ToString());
      UnityReplayKit.Instance.PreDiscard += () => Debug.Log("Discarding...");
      UnityReplayKit.Instance.Discarded += () => Debug.Log("Recording discarded");
    }

    public void OnGUI() {
      GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.BeginVertical();

      GUILayout.Label("IsReplayKitAvailable: " + UnityReplayKit.Instance.IsReplayKitAvailable.ToString());
      GUILayout.Label("IsBroadcastAvailable: " + UnityReplayKit.Instance.IsBroadcastAvailable.ToString());

      if (CurrentGameState == GameState.Run) {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.color = Color.black;
        GUILayout.Label((runDuration - TimeInGame).ToString("00.00") + "s");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
      } else {
        GUILayout.FlexibleSpace();
        GUILayout.Label("Game State: " + CurrentGameState.ToString());
        GUILayout.Space(50);
      }

      switch (CurrentGameState) {
        case GameState.MainMenu:
          if (GUILayout.Button("New Game")) {
            StartNewGame();
          }
          break;
        case GameState.Run:

          break;
        case GameState.GameFinished:
          if (GUILayout.Button("New Game")) {
            StartNewGame();
          }
          foreach (var previewPresentationStyle in System.Enum.GetValues(typeof(UnityReplayKit.PreviewPresentationStyle))) {
            if (GUILayout.Button("Preview Recording - " + previewPresentationStyle)) {
              UnityReplayKit.Instance.PreviewStyle = (UnityReplayKit.PreviewPresentationStyle)previewPresentationStyle;
              PreviewRecording();
            }
          }
          if (GUILayout.Button("Main Menu")) {
            CurrentGameState = GameState.MainMenu;
          }
          break;
      }

      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();
      GUILayout.EndArea();
    }

    public void Update() {
      if (CurrentGameState == GameState.Run) {
        TimeInGame += Time.deltaTime;
        if (TimeInGame >= runDuration) {
          StopGame();
        }
      }
    }

    private void StopGame() {
      if (showPreviewImmediately) {
        UnityReplayKit.Instance.StopRecordingAndShowPreview();
      } else {
        UnityReplayKit.Instance.StopRecording();
      }
      goGame.SetActive(false);
      CurrentGameState = GameState.GameFinished;
    }

    private void PreviewRecording() {
      UnityReplayKit.Instance.ShowPreview();
    }

    private void StartNewGame() {
      CurrentGameState = GameState.Run;
      goGame.SetActive(true);
      UnityReplayKit.Instance.StartRecording();
      TimeInGame = 0;
    }
  }

}