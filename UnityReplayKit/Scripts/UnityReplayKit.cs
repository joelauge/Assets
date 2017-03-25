using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace Rarebyte.REK {

  [ExecuteInEditMode]
  public class UnityReplayKit : MonoBehaviour {

    #region Error Codes

    public enum ReplayKitError {

      /// <summary>
      /// Unable to determine the cause of the error
      /// </summary>
      RPRecordingErrorUnknown = -5800,

      /// <summary>
      /// The user declined app recording
      /// </summary>
      RPRecordingErrorUserDeclined = -5801,

      /// <summary>
      /// App recording has been disabled via parental controls
      /// </summary>
      RPRecordingErrorDisabled = -5802,

      /// <summary>
      /// Recording failed to start
      /// </summary>
      RPRecordingErrorFailedToStart = -5803,

      /// <summary>
      /// Failed during recording
      /// </summary>
      RPRecordingErrorFailed = -5804,

      /// <summary>
      /// Insufficient storage for recording
      /// </summary>
      RPRecordingErrorInsufficientStorage = -5805,

      /// <summary>
      /// Recording interrupted by other app
      /// </summary>
      RPRecordingErrorInterrupted = -5806,

      /// <summary>
      /// Recording interrupted by multitasking and Content Resizing
      /// </summary>
      RPRecordingErrorContentResize = -5807,

      /// <summary>
      /// Attempted to start a broadcast without a prior session
      /// </summary>
      RPRecordingErrorBroadcastInvalidSession = -5808,

      /// <summary>
      /// Recording was forced to end when user pressed the power button
      /// </summary>
      RPRecordingErrorSystemDormancy = -5809,
    }

    #endregion

    public enum PreviewPresentationStyle {
      FullScreen = 0,
      PageSheet,
      FormSheet,
      CurrentContext,
      Custom,
      OverFullScreen,
      OverCurrentContext,
      //Popover // Note: Popover is currently not supported
    }

    public enum WidgetAnchor {
      TopLeft,
      TopRight,
      BottomLeft,
      BottomRight,
      Custom,
    }

    private static UnityReplayKit instance;
    public static UnityReplayKit Instance {
      get {
        return instance;
      }
    }

    #region Inspector

    [SerializeField]
    private bool dontDestroyOnLoad = true;

    [SerializeField]
    private bool useMicrophone = true;

    [SerializeField]
    private bool useCamera = true;

    [SerializeField]
    private bool useNativeOverlay = true;

    [SerializeField]
    private WidgetAnchor widgetAnchor = WidgetAnchor.TopRight;

    [SerializeField]
    private Vector2 widgetPosition;

    [SerializeField]
    private float widgetSize = 100;

    [SerializeField]
    private float widgetSizeRelative = 0.1f;

    [SerializeField]
    private bool isWidgetSizeRelative = true;

    [SerializeField]
    private Texture textureWidgetIcon = null;

    [SerializeField]
    private Texture textureMicrophone = null;

    [SerializeField]
    private Texture textureCamera = null;

    [SerializeField]
    private Texture textureBanner = null;

    [SerializeField]
    private PreviewPresentationStyle previewPresentationStyle;

    #endregion

    public bool DoNotDestroyOnLoad {
      get {
        return dontDestroyOnLoad;
      }
      set {
        dontDestroyOnLoad = value;
      }
    }

    public Texture TextureMicrophone {
      get {
        return textureMicrophone;
      }
    }

    public Texture TextureCamera {
      get {
        return textureCamera;
      }
    }

    public Texture TextureBanner {
      get {
        return textureBanner;
      }
    }

    public bool UseMicrophone {
      get {
        return useMicrophone;
      }
      set {
        useMicrophone = value;
#if UNITY_IOS && !UNITY_EDITOR
      replayKitSetUseMicrophone(value);
#endif
      }
    }

    public bool UseCamera {
      get {
        return useCamera;
      }
      set {
        useCamera = value;
#if UNITY_IOS && !UNITY_EDITOR
      replayKitSetUseCamera(value);
#endif
      }
    }

    public bool UseNativeOverlay {
      get {
        return useNativeOverlay;
      }
      set {
        useNativeOverlay = value;
      }
    }

    private bool isReplayKitNativeUIVisible = false;
    public bool IsReplayKitNativeUIVisible {
      get {
        return isReplayKitNativeUIVisible;
      }
      set {
        isReplayKitNativeUIVisible = value;

#if UNITY_IOS && !UNITY_EDITOR
      replayKitSetOverlayVisible(value);
#endif
      }
    }

    private bool ShowPreviewImmediately {
      get;
      set;
    }

    public MicrophoneLoopback MicrophoneLoopback { get; private set; }

    public Vector2 WidgetPosition {
      get {
        return widgetPosition;
      }
      set {
        widgetPosition = value;
        UpdateWidgetPositionAndSize();
      }
    }

    private void UpdateWidgetPositionAndSize() {
#if UNITY_IOS && !UNITY_EDITOR
      Vector2 finalWidgetPosition = CalculateFinalWidgetPosition();
      replayKitSetWidgetPositionAndSize(finalWidgetPosition.x, finalWidgetPosition.y, CalculateFinalWidgetSize());
#endif
    }

    private Vector2 CalculateFinalWidgetPosition() {
      Vector2 finalWidgetPosition = widgetPosition;
      float finalWidgetSize = CalculateFinalWidgetSize();

      switch (widgetAnchor) {
        case WidgetAnchor.BottomLeft:
          finalWidgetPosition.x = 0;
          finalWidgetPosition.y = Screen.height - finalWidgetSize;
          break;
        case WidgetAnchor.BottomRight:
          finalWidgetPosition.x = Screen.width - finalWidgetSize;
          finalWidgetPosition.y = Screen.height - finalWidgetSize;
          break;
        case WidgetAnchor.TopLeft:
          finalWidgetPosition.x = 0;
          finalWidgetPosition.y = 0;
          break;
        case WidgetAnchor.TopRight:
          finalWidgetPosition.x = Screen.width - finalWidgetSize;
          finalWidgetPosition.y = 0;
          break;
        case WidgetAnchor.Custom:
        default:
          // custom: widget position is used directly
          break;
      }

      // widget position is used as offset if pre-defined anchoring is used
      if (widgetAnchor != WidgetAnchor.Custom) {
        finalWidgetPosition += widgetPosition;
      }

      return finalWidgetPosition;
    }

    private float CalculateFinalWidgetSize() {
      if (IsWidgetSizeRelative == true) {
        return WidgetSizeRelative * Screen.height;
      } else {
        return WidgetSize;
      }
    }

    public WidgetAnchor Anchor {
      get {
        return widgetAnchor;
      }
      set {
        widgetAnchor = value;

        // when changing the widget anchor, position has to be updated as well
        UpdateWidgetPositionAndSize();
      }
    }

    public float WidgetSize {
      get {
        return widgetSize;
      }
      set {
        widgetSize = value;

        // when changing the widget anchor, position has to be updated as well
        UpdateWidgetPositionAndSize();
      }
    }

    public bool IsWidgetSizeRelative {
      get {
        return isWidgetSizeRelative;
      }
      set {
        isWidgetSizeRelative = value;
      }
    }

    public float WidgetSizeRelative {
      get {
        return widgetSizeRelative;
      }
      set {
        widgetSizeRelative = value;
      }
    }

    public PreviewPresentationStyle PreviewStyle {
      get {
        return previewPresentationStyle;
      }
      set {
        previewPresentationStyle = value;
#if UNITY_IOS && !UNITY_EDITOR
      replayKitSetPreviewPresentationStyle((int)value);
#endif
      }
    }

    private bool isRKAvailable = false;
    public bool IsReplayKitAvailable {
      get {
        return isRKAvailable;
      }
    }

    public bool IsRecording { get; private set; }

    public bool IsBroadcasting { get; private set; }

    private bool isLiveBroadcastAvailable = false;
    public bool IsBroadcastAvailable {
      get {
        return isLiveBroadcastAvailable;
      }
    }

    public bool IsBroadcastPaused { get; private set; }

    public void Awake() {
      instance = this;

      if (Application.isPlaying == true && dontDestroyOnLoad == true) {
        DontDestroyOnLoad(gameObject);
      }

      // do not change this, otherwise the callbacks will not work
      name = "unity_replay_kit";

#if UNITY_IOS && !UNITY_EDITOR
      isRKAvailable = isReplayKitAvailable();

      if (IsReplayKitAvailable) {
        // init
        WidgetPosition = widgetPosition;
        UseMicrophone = useMicrophone;
        UseCamera = useCamera;
        UseNativeOverlay = useNativeOverlay;
        PreviewStyle = previewPresentationStyle;

        isLiveBroadcastAvailable = isBroadcastAvailable();
      }
#endif

      MicrophoneLoopback = GetComponent<MicrophoneLoopback>();
    }

    public void Start() {
      if (IsReplayKitAvailable == true) {
        IsReplayKitNativeUIVisible = UseNativeOverlay;
      }
    }

    #region Recording

    public void StartRecording() {
      if (PreStart != null) {
        PreStart();
      }

#if UNITY_IOS && !UNITY_EDITOR
    replayKitStartRecording(UseMicrophone);
#endif
    }

    public void StopRecording() {
      if (PreStop != null) {
        PreStop();
      }

#if UNITY_IOS && !UNITY_EDITOR
    replayKitStopRecording();
#endif
    }

    public void StopRecordingAndShowPreview() {
      if (IsRecording) {
        if (PreStop != null) {
          PreStop();
        }

        ShowPreviewImmediately = true;
#if UNITY_IOS && !UNITY_EDITOR
      replayKitStopRecording();
#endif
      } else {
        ShowPreview();
      }
    }

    public void ShowPreview() {
      if (PreShowPreview != null) {
        PreShowPreview();
      }
#if UNITY_IOS && !UNITY_EDITOR
    if (IsRecording == false) {
      replayKitSetPreviewPresentationStyle((int)previewPresentationStyle);
      replayKitShowPreview();
    }
#endif
    }

    public void Discard() {
      if (PreDiscard != null) {
        PreDiscard();
      }

#if UNITY_IOS && !UNITY_EDITOR
    replayKitDiscardRecording();
#endif
    }

    #endregion

    #region Broadcasting

    public void SelectBroadcastingService() {
#if UNITY_IOS && !UNITY_EDITOR
    replayKitSelectBroadcastService();
#endif
    }

    public void StartBroadcast() {

      if ((UseCamera || UseMicrophone) && MicrophoneLoopback != null) {
        MicrophoneLoopback.StartMicrophone();
      }

#if UNITY_IOS && !UNITY_EDITOR
    replayKitStartBroadcast();
#endif
    }

    public void PauseBroadcast() {

#if UNITY_IOS && !UNITY_EDITOR
    replayKitPauseBroadcast();
    IsBroadcastPaused = replayKitIsBroadcastingPaused();
#else
      IsBroadcastPaused = true;
#endif
    }

    public void ResumeBroadcast() {
#if UNITY_IOS && !UNITY_EDITOR
    replayKitResumeBroadcast();
    IsBroadcastPaused = replayKitIsBroadcastingPaused();
#else
      IsBroadcastPaused = false;
#endif
    }

    public void StopBroadcast() {
      if (MicrophoneLoopback != null) {
        MicrophoneLoopback.StopMicrophone();
      }

#if UNITY_IOS && !UNITY_EDITOR
    replayKitStopBroadcast();
#endif
    }

    #endregion

    #region Native

#if UNITY_IOS

  [DllImport("__Internal")]
  private static extern void replayKitStartRecording(bool microphoneEnabled);

  [DllImport("__Internal")]
  private static extern void replayKitStopRecording();

  [DllImport("__Internal")]
  private static extern bool isReplayKitAvailable();

  [DllImport("__Internal")]
  private static extern void replayKitShowPreview();

  [DllImport("__Internal")]
  private static extern void replayKitDiscardRecording();

  [DllImport("__Internal")]
  private static extern void replayKitSetWidgetPositionAndSize(float x, float y, float size);

  [DllImport("__Internal")]
  private static extern void replayKitSetOverlayVisible(bool visible);

  [DllImport("__Internal")]
  private static extern void replayKitSetUseMicrophone(bool useMicrophone);

  [DllImport("__Internal")]
  private static extern void replayKitSetUseCamera(bool useCamera);
  
  [DllImport("__Internal")]
  private static extern void replayKitSetPreviewPresentationStyle(int previewPresentationStyle);

  [DllImport("__Internal")]
  private static extern bool isBroadcastAvailable();

  [DllImport("__Internal")]
  private static extern void replayKitSelectBroadcastService();

  [DllImport("__Internal")]
  private static extern void replayKitStartBroadcast();

  [DllImport("__Internal")]
  private static extern void replayKitPauseBroadcast();

  [DllImport("__Internal")]
  private static extern void replayKitResumeBroadcast();

  [DllImport("__Internal")]
  private static extern void replayKitStopBroadcast();

  [DllImport("__Internal")]
  private static extern bool replayKitIsBroadcasting();

  [DllImport("__Internal")]
  private static extern bool replayKitIsBroadcastingPaused();

#endif

    #endregion

    #region Native Callbacks

    private ReplayKitError GetErrorFromString(string error) {
      int errorCode = int.Parse(error);
      return (ReplayKitError)errorCode;
    }

    public void OnStartRecordingFailure(string error) {
      IsRecording = false;
      if (StartFailed != null) {
        StartFailed(GetErrorFromString(error));
      }
    }

    public void OnPreStartRecording() {
      if (PreStart != null) {
        PreStart();
      }
    }

    public void OnStartRecordingSuccess() {
      IsRecording = true;
      if (Started != null) {
        Started();
      }
    }

    public void OnPreStopRecording() {
      if (PreStop != null) {
        PreStop();
      }
    }

    public void OnStopRecordingFailure(string error) {
      IsRecording = false;
      ShowPreviewImmediately = false;
      if (StopFailed != null) {
        StopFailed(GetErrorFromString(error));
      }
    }

    public void OnStopRecordingSuccess() {
      IsRecording = false;

      if (Stopped != null) {
        Stopped();
      }

      // show preview immediately if native UI is used
      if (UseNativeOverlay == true || ShowPreviewImmediately == true) {
        ShowPreviewImmediately = false;

        // hide native UI during preview
        if (UseNativeOverlay == true) {
          IsReplayKitNativeUIVisible = false;
        }

        ShowPreview();
      }
    }

    public void OnDiscardRecordingSuccess() {
      if (Discarded != null) {
        Discarded();
      }
    }

    public void OnPreviewRecordingCompleted() {
      if (UseNativeOverlay) {
        IsReplayKitNativeUIVisible = true;
      }

      if (PreviewCompleted != null) {
        PreviewCompleted();
      }
    }

    public void OnSelectBroadcastServiceSuccess() {
      if (BroadcastServiceSelected != null) {
        BroadcastServiceSelected();
      }
    }

    public void OnSelectBroadcastServiceFailure(string error) {
      if (BroadcastServiceSelectionFailed != null) {
        BroadcastServiceSelectionFailed(GetErrorFromString(error));
      }
    }

    public void OnStartBroadcastSuccess() {
      IsBroadcasting = true;
      IsBroadcastPaused = false;
      if (BroadcastStarted != null) {
        BroadcastStarted();
      }
    }

    public void OnStartBroadcastFailure(string error) {
      IsBroadcasting = false;
      IsBroadcastPaused = false;
      if (BroadcastStartFailed != null) {
        BroadcastStartFailed(GetErrorFromString(error));
      }
    }
    public void OnStopBroadcastSuccess() {
      IsBroadcasting = false;
      IsBroadcastPaused = false;
      if (BroadcastStopped != null) {
        BroadcastStopped();
      }
    }

    public void OnStopBroadcastFailure(string error) {
      IsBroadcasting = false;
      IsBroadcastPaused = false;
      if (BroadcastStopFailed != null) {
        BroadcastStopFailed(GetErrorFromString(error));
      }
    }

    public void OnBroadcastFinishedSuccess() {
      IsBroadcasting = false;
      IsBroadcastPaused = false;
      if (BroadcastFinished != null) {
        BroadcastFinished();
      }
    }

    public void OnBroadcastFinishedFailure(string error) {
      IsBroadcasting = false;
      IsBroadcastPaused = false;
      if (BroadcastFinishFailed != null) {
        BroadcastFinishFailed(GetErrorFromString(error));
      }
    }

    #endregion

    #region Actions/Delegates

    /// <summary>
    /// Is called when there is an error starting the recording
    /// </summary>
    public Action<ReplayKitError> StartFailed;

    /// <summary>
    /// Is called after the recording has been started successfully
    /// </summary>
    public Action Started;

    /// <summary>
    /// Is called when there is an error stopping the recording
    /// </summary>
    public Action<ReplayKitError> StopFailed;

    /// <summary>
    /// Is called after the recording has been stopped successfully
    /// </summary>
    public Action Stopped;

    /// <summary>
    /// Is called after the recording has been discarded
    /// </summary>
    public Action Discarded;

    /// <summary>
    /// Is called after the preview dialog is dismissed
    /// </summary>
    public Action PreviewCompleted;

    /// <summary>
    /// Is called just before starting the recording
    /// </summary>
    public Action PreStart;

    /// <summary>
    /// Is called just before stopping the recording
    /// </summary>
    public Action PreStop;

    /// <summary>
    /// Is called just before displaying the preview dialog
    /// </summary>
    public Action PreShowPreview;

    /// <summary>
    /// Is called just before discarding a recording
    /// </summary>
    public Action PreDiscard;

    /// <summary>
    /// Is called just after a broadcast service has been selected
    /// </summary>
    public Action BroadcastServiceSelected;

    /// <summary>
    /// Is called when the broadcast service selection has failed
    /// </summary>
    public Action<ReplayKitError> BroadcastServiceSelectionFailed;

    /// <summary>
    /// Is called just after broadcasting has started successfully
    /// </summary>
    public Action BroadcastStarted;

    /// <summary>
    /// Is called when the broadcast has failed to start
    /// </summary>
    public Action<ReplayKitError> BroadcastStartFailed;

    /// <summary>
    /// Is called just after the broadcast has initiated stopping successfully
    /// </summary>
    public Action BroadcastStopped;

    /// <summary>
    /// Is called when trying to stop the broadcast has failed
    /// </summary>
    public Action<ReplayKitError> BroadcastStopFailed;

    /// <summary>
    /// Is called just after the broadcast has been finished
    /// </summary>
    public Action BroadcastFinished;

    /// <summary>
    /// Is called when finishing the broadcast has failed
    /// </summary>
    public Action<ReplayKitError> BroadcastFinishFailed;

    #endregion

    #region Editor Helpers

#if UNITY_EDITOR
    public void OnGUI() {
      if (UseNativeOverlay) {
        Vector2 finalWidgetPosition = CalculateFinalWidgetPosition();
        float finalWidgetSize = CalculateFinalWidgetSize();
        Rect nativeOverlayRect = new Rect(finalWidgetPosition.x, finalWidgetPosition.y, finalWidgetSize, finalWidgetSize);
        GUI.DrawTexture(nativeOverlayRect, textureWidgetIcon, ScaleMode.ScaleToFit);
      }
    }
#endif

    #endregion
  }
}