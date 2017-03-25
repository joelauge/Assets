using UnityEngine;
using UnityEngine.UI;

namespace Rarebyte.REK.Examples {

  public class UnityReplayKitExampleGameUnityUI : MonoBehaviour {

    public enum GameState {
      MainMenu,
      Run,
      GameFinished,
    }

    #region GUI Elements

    [SerializeField]
    private GameObject goWidgetStart = null;

    [SerializeField]
    private GameObject goWidgetEnd = null;

    [SerializeField]
    private GameObject goButtonPreview = null;

    [SerializeField]
    private Text textCounter = null;

    [SerializeField]
    private Toggle toggleStartUseMicrophone = null;

    [SerializeField]
    private Toggle toggleEndUseMicrophone = null;

    [SerializeField]
    private float runDuration = 10;

    #endregion

    private float TimeInGame { get; set; }

    private GameState CurrentGameState { get; set; }

    public void Start() {
      CurrentGameState = GameState.MainMenu;
      goWidgetStart.SetActive(true);
      goWidgetEnd.SetActive(false);
      textCounter.gameObject.SetActive(false);

      toggleStartUseMicrophone.isOn = UnityReplayKit.Instance.UseMicrophone;
      toggleEndUseMicrophone.isOn = UnityReplayKit.Instance.UseMicrophone;
    }

    public void Update() {
      if (CurrentGameState == GameState.Run) {
        TimeInGame += Time.deltaTime;

        if (TimeInGame >= runDuration) {
          CurrentGameState = GameState.GameFinished;

          goWidgetEnd.SetActive(true);
          goButtonPreview.SetActive(true);
          textCounter.gameObject.SetActive(false);
          toggleStartUseMicrophone.isOn = UnityReplayKit.Instance.UseMicrophone;
          toggleEndUseMicrophone.isOn = UnityReplayKit.Instance.UseMicrophone;
        }

        textCounter.text = (runDuration - TimeInGame).ToString("00.00") + "s";
      }
    }

    public void StartNewGame() {
      CurrentGameState = GameState.Run;

      goWidgetStart.SetActive(false);
      goWidgetEnd.SetActive(false);
      textCounter.gameObject.SetActive(true);

      CurrentGameState = GameState.Run;
      TimeInGame = 0;
    }
  }

}