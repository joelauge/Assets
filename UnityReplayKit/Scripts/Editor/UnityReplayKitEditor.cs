#if UNITY_5_3 || UNITY_5_4 || UNITY_5_5
#define SCENE_MANAGEMENT_AVAILABLE
#endif

using UnityEngine;
using UnityEditor;

#if SCENE_MANAGEMENT_AVAILABLE
using UnityEditor.SceneManagement;
#endif

namespace Rarebyte.REK {

  [CustomEditor(typeof(UnityReplayKit))]
  public class UnityReplayKitEditor : Editor {

    private readonly string[] modes = new string[] { "Native iOS Overlay", "Custom" };

    public override void OnInspectorGUI() {

      GUI.changed = false;

      UnityReplayKit replayKit = target as UnityReplayKit;

      EditorGUILayout.Separator();

      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.BeginVertical();

      GUILayout.Label(new GUIContent(replayKit.TextureBanner), GUILayout.Width(350), GUILayout.Height(40));

      // website and documentation
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("Website", GUILayout.Width(162))) {
        Application.OpenURL("http://rek.rarebyte.com");
      }
      if (GUILayout.Button("Documentation", GUILayout.Width(162))) {
        Application.OpenURL("http://devblog.rarebyte.com/?page_id=1152");
      }
      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();

      EditorGUILayout.Separator();

      // don't destroy on load setting
      replayKit.DoNotDestroyOnLoad = EditorGUILayout.Toggle(new GUIContent("Don't Destroy on Load", "Do not unload when the scene changes"), replayKit.DoNotDestroyOnLoad);

      EditorGUILayout.Separator();


      // microphone setting
      GUIStyle styleSelected = new GUIStyle(GUI.skin.button) {
        normal = GUI.skin.button.onActive
      };
      GUIStyle styleUnselected = new GUIStyle(GUI.skin.button) {
        normal = GUI.skin.button.normal
      };
      GUILayout.BeginHorizontal();
      if (GUILayout.Button(new GUIContent(replayKit.TextureMicrophone, "When enabled, the microphone of the device is used, so you can record commentary etc."), replayKit.UseMicrophone ? styleSelected : styleUnselected, GUILayout.Height(64), GUILayout.Width(64))) {
        replayKit.UseMicrophone = !replayKit.UseMicrophone;
      }
      GUILayout.BeginVertical(GUILayout.Height(64));
      GUILayout.FlexibleSpace();
      GUILayout.Label("Microphone " + (replayKit.UseMicrophone ? "Enabled" : "Disabled"));
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();

      // camera setting
      GUILayout.BeginHorizontal();
      if (GUILayout.Button(new GUIContent(replayKit.TextureCamera, "When enabled, the camera feed of the device is recorded as picture-in-picture. iOS 10+ only."), replayKit.UseCamera ? styleSelected : styleUnselected, GUILayout.Height(64), GUILayout.Width(64))) {
        replayKit.UseCamera = !replayKit.UseCamera;
      }
      GUILayout.BeginVertical(GUILayout.Height(50));
      GUILayout.FlexibleSpace();
      GUILayout.Label("Camera " + (replayKit.UseCamera ? "Enabled" : "Disabled"));
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();

      EditorGUILayout.Separator();

      // preview style
      GUILayout.BeginHorizontal();
      replayKit.PreviewStyle = (UnityReplayKit.PreviewPresentationStyle)EditorGUILayout.EnumPopup(new GUIContent("Preview Style", "Style of the modal preview (iPad only; on iPhone and iPod touch, the preview is always fullscreen"), replayKit.PreviewStyle);
      GUILayout.EndHorizontal();

      EditorGUILayout.Separator();

      replayKit.UseNativeOverlay = (0 == GUILayout.SelectionGrid(replayKit.UseNativeOverlay == true ? 0 : 1, modes, modes.Length));

      // native overlay settings
      if (replayKit.UseNativeOverlay == true) {

        EditorGUILayout.HelpBox("When using the Native iOS Overlay mode, a native widget will be displayed for starting and stopping recordings. \n\nAwesome fact: This native widget will NOT be visible in your recorded video.", MessageType.Info, true);

        // widget anchoring
        replayKit.Anchor = (UnityReplayKit.WidgetAnchor)EditorGUILayout.EnumPopup("Anchor", replayKit.Anchor);

        // widget position/offset
        string positionTitle = "Offset";
        if (replayKit.Anchor == UnityReplayKit.WidgetAnchor.Custom) {
          positionTitle = "Position";
        }
        replayKit.WidgetPosition = EditorGUILayout.Vector2Field(positionTitle, replayKit.WidgetPosition);

        // widget size
        replayKit.IsWidgetSizeRelative = EditorGUILayout.Toggle(new GUIContent("Auto-scale widget", "When enabled, the widget size is scaled automatically to the given percentage of the screen height."), replayKit.IsWidgetSizeRelative);
        if (replayKit.IsWidgetSizeRelative == true) {
          replayKit.WidgetSizeRelative = EditorGUILayout.Slider("Size", replayKit.WidgetSizeRelative, 0, 1);
        } else {
          replayKit.WidgetSize = EditorGUILayout.FloatField("Size", replayKit.WidgetSize);
        }


      } else {

        EditorGUILayout.HelpBox("Custom mode: This mode lets you implement your own logic via code.\n\nNote: You can also wire your own UI buttons to the methods provided in UnityReplayKit", MessageType.Info, true);

      }

      if (GUI.changed == true) {
        EditorUtility.SetDirty(target);
#if SCENE_MANAGEMENT_AVAILABLE
        var replayKitScene = replayKit.gameObject.scene;
        EditorSceneManager.MarkSceneDirty(replayKitScene);
#endif
      }
    }
  }

}