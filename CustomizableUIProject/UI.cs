using KSP.UI.Binding;
using UnityEngine;

namespace CustomizableUI
{
    public class UI
    {
        private static UI _instance;

        public bool IsWindowOpen;
        private Rect _windowRect = new Rect(Screen.width / 2 - Styles.WindowWidth / 2, 200, 0, 0);


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
                    "// I Whish They Made UI Customizable",
                    GUILayout.Height(0),
                    GUILayout.Width(Styles.WindowWidth)
                );
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
            float horizontalMinLimit = topGroup is AppBar ? 0 :  -1920 / 2;
            float horizontalMaxLimit = topGroup is AppBar ? Screen.width :  1920 / 2;
            float verticalMinLimit = topGroup is AppBar ? 0 : - 1080 / 2;
            float verticalMaxLimit = topGroup is AppBar ? Screen.height : 1080 / 2;
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
