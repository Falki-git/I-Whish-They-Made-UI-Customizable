using KSP.Game;
using KSP.UI;
using KSP.UI.Binding;
using UnityEngine;

namespace CustomizableUI
{
    public class UI
    {
        private static UI _instance;

        public bool IsWindowOpen;
        private Rect _windowRect = new Rect(Screen.width / 2 - Styles.WindowWidth / 2, 200, 0, 0);
        private Rect _overlay = new Rect();
        //private Rect scaleFactorRect = new Rect(0, 0, 0, 0);


        private UI()
        { }

        public static UI Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UI();

                return _instance;
            }
        }

        public void OnGUI()
        {
            GUI.skin = Styles.SpaceWarpUISkin;

            if (IsWindowOpen)
            {
                _windowRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    _windowRect,
                    FillWindow,
                    "// I Wish They Made UI Customizable",
                    GUILayout.Height(0),
                    GUILayout.Width(Styles.WindowWidth)
                );

                DrawOverlayOverSelectedGroup();

                /*
                scaleFactorRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    scaleFactorRect,
                    a =>
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("GameManager.Instance.Game.UI._mainCanvas.scaleFactor: ");
                        GUILayout.Label(String.Format("{0:F5}", GameManager.Instance.Game.UI._mainCanvas.scaleFactor));
                        GUILayout.EndHorizontal();
                        GameManager.Instance.Game.UI._mainCanvas.scaleFactor = GUILayout.HorizontalSlider(GameManager.Instance.Game.UI._mainCanvas.scaleFactor, 0, 2);
                        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
                    },
                    "",
                    GUILayout.Height(0),
                    GUILayout.Width(Styles.WindowWidth)
                );
                */
            }
        }

        private void FillWindow(int _)
        {
            if (CloseButton())
                CloseWindow();

            GUILayout.BeginHorizontal();
            var topGroup = Manager.Instance.Groups[TopLevelGroup.SelectedIndex];
            var previousPosition = topGroup.Transform.position;

            GUILayout.FlexibleSpace();
            if (SelectButton("<"))
                if (TopLevelGroup.SelectedIndex > 0)
                    TopLevelGroup.SelectPreviousTopGroup();
            GroupLabel(topGroup.Name);
            if (SelectButton(">"))
                if (Manager.Instance.Groups.Count > TopLevelGroup.SelectedIndex + 1)
                    TopLevelGroup.SelectNextTopGroup();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (topGroup is Navball)
                GUI.enabled = false;
            topGroup.AttachToNavball = Toggle(topGroup.AttachToNavball, "Follow Navball");
            GUI.enabled = true;
            topGroup.IsActive = Toggle(topGroup.IsActive, "Show");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Label($"X:");
            topGroup.Position = topGroup.Transform.position = new Vector3(float.Parse(TextField(string.Format(Styles.PixelFormat, topGroup.Transform.position.x))), topGroup.Transform.position.y, topGroup.Transform.position.z);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Y:");
            topGroup.Position = topGroup.Transform.position = new Vector3(topGroup.Transform.position.x, float.Parse(TextField(string.Format(Styles.PixelFormat, topGroup.Transform.position.y))), topGroup.Transform.position.z);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"Z:");
            topGroup.Position = topGroup.Transform.position = new Vector3(topGroup.Transform.position.x, topGroup.Transform.position.y, float.Parse(TextField(string.Format(Styles.PixelFormat, topGroup.Transform.position.z))));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            /*
            float horizontalMinLimit = topGroup is AppBar ? 0 :  -1920 / 2;
            float horizontalMaxLimit = topGroup is AppBar ? Screen.width :  1920 / 2;
            float verticalMinLimit = topGroup is AppBar ? 0 : - 1080 / 2;
            float verticalMaxLimit = topGroup is AppBar ? Screen.height : 1080 / 2;
            */
            float horizontalMinLimit = 0 + topGroup.OffsetToZero.x;
            float horizontalMaxLimit = Screen.width + topGroup.OffsetToZero.x - topGroup.RectTransform.rect.width + topGroup.ToMaxOffset.x;
            float verticalMinLimit = 0 + topGroup.OffsetToZero.y;
            float verticalMaxLimit = Screen.height + topGroup.OffsetToZero.y - topGroup.RectTransform.rect.height + topGroup.ToMaxOffset.y;

            topGroup.Position = topGroup.Transform.position = new Vector3(GUILayout.HorizontalSlider(topGroup.Transform.position.x, horizontalMinLimit, horizontalMaxLimit), topGroup.Transform.position.y, topGroup.Transform.position.z);
            topGroup.Position = topGroup.Transform.position = new Vector3(topGroup.Transform.position.x, GUILayout.HorizontalSlider(topGroup.Transform.position.y, verticalMinLimit, verticalMaxLimit), topGroup.Transform.position.z);
            topGroup.Position = topGroup.Transform.position = new Vector3(topGroup.Transform.position.x, topGroup.Transform.position.y, GUILayout.HorizontalSlider(topGroup.Transform.position.z, -5000, 5000));
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            if (NormalButton("Move to Horizontal center"))
                topGroup.MoveToHorizontalCenter();
            if (NormalButton("Move to Vertical center"))
                topGroup.MoveToVerticalCenter();
            if (NormalButton("Reset"))
                topGroup.ResetToDefault();
            GUILayout.EndHorizontal();

            Manager.Instance.RecalculatePositionsOfGroupsAttachedToNavball(topGroup, previousPosition);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
                Utility.Instance.SaveData();
            if (GUILayout.Button("Load"))
                Utility.Instance.LoadData();
            if (NormalButton("FUBAR"))
                Manager.Instance.ResetAllToDefault();
            GUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        private void DrawOverlayOverSelectedGroup()
        {
            var topGroup = Manager.Instance.Groups[TopLevelGroup.SelectedIndex];
            Rect topGroupRect = ((RectTransform)topGroup.Transform).rect;

            // We calculate at what % of the screen the UI group is.
            // Since Transform.position (0, 0, z) indicates a pixel at the middle of the screen and since each UI group can be offset differently, we need to do some algebra hacks.
            // Reference resolution is 1920 x 1080, but Main canvas uses something more like 1815 x 1023 (?!). 907.5 is half of horizontal res, 511.5 half of vertical.

            float topGroupPercent_X = (topGroup.Transform.position.x + 907.5f - (topGroup.ToCenterOffset.x + topGroupRect.width / 2)) / 1815.0f;            
            _overlay.x = Screen.width * topGroupPercent_X;

            // For Y, Transform.position and IMGUI coordinates are reversed. Transfer.position counts bottom-up and IMGUI draws up-bottom. So we reverse the percent.
            float topGroupPercent_Y = (topGroup.Transform.position.y + 511.5f - topGroup.ToCenterOffset.y + topGroupRect.height / 2) / 1023.0f;            
            _overlay.y = Screen.height * (1 - topGroupPercent_Y);

            _overlay.width = topGroupRect.width * Manager.Instance.ScaleFactor;
            _overlay.height = topGroupRect.height * Manager.Instance.ScaleFactor;

            var previousColor = GUI.color;
            // color the overlay in transparent yellow
            GUI.color = new Color(1f, 1f, 0f, 0.75f);
            _overlay = GUI.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                _overlay,
                w => { },
                ""
                );
            GUI.color = previousColor;
        }

        /*
        private void DrawChild(ChildGroup group, int level)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20 * level);
            GUILayout.Label(group.Name);
            GUILayout.Label($"X: {group.Transform.position.x}");
            GUILayout.Label($"Y: {group.Transform.position.y}");
            GUILayout.Label($"Z: {group.Transform.position.z}");
            GUILayout.EndHorizontal();

            if (group.Children != null && group.Children.Count > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20 * level + 1);
                if (GUILayout.Button("<"))
                    group.SelectedChild--;
                GUILayout.Label(group.Children[group.SelectedChild].Name);
                if (GUILayout.Button(">"))
                    group.SelectedChild++;

                DrawChild(group.Children[group.SelectedChild], level + 1);
            }
        }
        */

        private void GroupLabel(string text)
        {
            GUILayout.Label(text, Styles.GroupLabel);
        }

        private bool SelectButton(string text)
        {
            return GUILayout.Button(text, Styles.SelectButton);
        }

        private bool NormalButton(string text)
        {
            return GUILayout.Button(text, Styles.NormalButton);
        }

        private string TextField(string text)
        {
            return GUILayout.TextField(text, Styles.SelectTextField);
        }

        private bool Toggle(bool value, string text)
        {
            return GUILayout.Toggle(value, text, Styles.AttachToggle);
        }

        private bool CloseButton()
        {
            Rect rect = new Rect(Styles.WindowWidth - 22, 2, 20, 20);
            return GUI.Button(rect, Styles.CloseButtonTexture, Styles.CloseButton);
        }

        private void CloseWindow()
        {
            GameObject.Find(CustomizableUIPlugin.ToolbarFlightButtonID)?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(false);
            IsWindowOpen = false;
        }
    }
}
