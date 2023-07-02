using KSP.UI.Binding;
using UnityEngine;

namespace CustomizableUI
{
    public class UI
    {
        private static UI _instance;
        private Rect _windowRect = new Rect(Screen.width / 2 - Styles.WindowWidth / 2, 200, 0, 0);
        private Rect _overlay = new Rect();

        private bool _nudgeLeft;
        private bool _nudgeRight;
        private bool _nudgeUp;
        private bool _nudgeDown;

        private bool _printMessage;
        private string _message;
        private float _messageTime;

        public bool IsWindowOpen;

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
            // ▼▶◀▲▿▹◃▵

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
            {
                float horizontalMinLimit = 0 + topGroup.OffsetToZero.x;
                float horizontalMaxLimit = Screen.width + topGroup.OffsetToZero.x - topGroup.RectTransform.rect.width + topGroup.ToMaxOffset.x;
                float verticalMinLimit = 0 + topGroup.OffsetToZero.y;
                float verticalMaxLimit = Screen.height + topGroup.OffsetToZero.y - topGroup.RectTransform.rect.height + topGroup.ToMaxOffset.y;

                topGroup.Position = topGroup.Transform.position = new Vector3(GUILayout.HorizontalSlider(topGroup.Transform.position.x, horizontalMinLimit, horizontalMaxLimit), topGroup.Transform.position.y, topGroup.Transform.position.z);
                topGroup.Position = topGroup.Transform.position = new Vector3(topGroup.Transform.position.x, GUILayout.HorizontalSlider(topGroup.Transform.position.y, verticalMinLimit, verticalMaxLimit), topGroup.Transform.position.z);
                topGroup.Position = topGroup.Transform.position = new Vector3(topGroup.Transform.position.x, topGroup.Transform.position.y, GUILayout.HorizontalSlider(topGroup.Transform.position.z, -5000, 5000));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            {

                GUILayout.BeginVertical(new GUIStyle { fixedWidth = 50 });
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    // TOP ROW
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (JumpUpDownButton("▲ ▲"))
                        {
                            if (topGroup.Position.y < topGroup.VerticalLowerMiddle)
                                topGroup.MoveToVerticalLowerMiddle();
                            else if (topGroup.Position.y < topGroup.VerticalCenter)
                                topGroup.MoveToVerticalCenter();
                            else if (topGroup.Position.y < topGroup.VerticalUpperMiddle)
                                topGroup.MoveToVerticalUpperMiddle();
                            else
                                topGroup.MoveToVerticalUpperTop();
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }

                    // MIDDLE ROW
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (JumpLeftRightButton("◀\n◀"))
                        {
                            if (topGroup.Position.x > topGroup.HorizontalRightMiddle)
                                topGroup.MoveToHorizontalRightMiddle();
                            else if (topGroup.Position.x > topGroup.HorizontalCenter)
                                topGroup.MoveToHorizontalCenter();
                            else if (topGroup.Position.x > topGroup.HorizontalLeftMiddle)
                                topGroup.MoveToHorizontalLeftMiddle();
                            else
                                topGroup.MoveToHorizontalLeftFar();
                        }

                        GUILayout.BeginVertical();
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(40);
                                MoveButton("▲", "up");
                                GUILayout.Space(40);
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.BeginHorizontal();
                            {
                                MoveButton("◀", "left");
                                GUILayout.Space(40);
                                MoveButton("▶", "right");
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(40);
                                MoveButton("▼", "down");
                                GUILayout.Space(40);
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                        }

                        if (JumpLeftRightButton("▶\n▶"))
                        {
                            if (topGroup.Position.x < topGroup.HorizontalLeftMiddle)
                                topGroup.MoveToHorizontalLeftMiddle();
                            else if (topGroup.Position.x < topGroup.HorizontalCenter)
                                topGroup.MoveToHorizontalCenter();
                            else if (topGroup.Position.x < topGroup.HorizontalRightMiddle)
                                topGroup.MoveToHorizontalRightMiddle();
                            else
                                topGroup.MoveToHorizontalRightFar();
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }

                    // BOTTOM ROW
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (JumpUpDownButton("▼ ▼"))
                        {
                            if (topGroup.Position.y > topGroup.VerticalUpperMiddle)
                                topGroup.MoveToVerticalUpperMiddle();
                            else if (topGroup.Position.y > topGroup.VerticalCenter)
                                topGroup.MoveToVerticalCenter();
                            else if (topGroup.Position.y > topGroup.VerticalLowerMiddle)
                                topGroup.MoveToVerticalLowerMiddle();
                            else
                                topGroup.MoveToVerticalLowerBottom();
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }

                    NudgeGroup(topGroup);

                    GUILayout.EndVertical();
                }

                GUILayout.Space(-100);
                GUILayout.BeginVertical(new GUIStyle { fixedWidth = 100 });
                {
                    GUILayout.FlexibleSpace();

                    if (NormalButton("Reset"))
                    {
                        topGroup.ResetToDefault();
                        PrintMessage($"Group {topGroup.Name} reset.");
                    }
                    if (NormalButton("FUBAR"))
                    {
                        Manager.Instance.ResetAllToDefault();
                        PrintMessage("Layout reset to initial state.");
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
            }
            Manager.Instance.RecalculatePositionsOfGroupsAttachedToNavball(topGroup, previousPosition);

            GUILayout.Space(20);

            
            // Save and Load buttons
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save"))
                {
                    Utility.Instance.SaveData();
                    PrintMessage("Layout saved.");
                }
                if (GUILayout.Button("Load"))
                {
                    Utility.Instance.LoadData();
                    PrintMessage("Layout loaded.");
                }
                GUILayout.EndHorizontal();
            }

            // Draw message
            if (Time.time - _messageTime < 2)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("--");
                GUILayout.Label(_message, Styles.MessageLabel);
                GUILayout.EndVertical();
            }

            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }        

        private void NudgeGroup(TopLevelGroup group)
        {
            if (_nudgeLeft)
                group.NudgeLeft();
            if (_nudgeRight)
                group.NudgeRight();
            if (_nudgeUp)
                group.NudgeUp();
            if (_nudgeDown)
                group.NudgeDown();
        }

        private void DrawOverlayOverSelectedGroup()
        {
            var topGroup = Manager.Instance.Groups[TopLevelGroup.SelectedIndex];
            Rect topGroupRect = ((RectTransform)topGroup.Transform).rect;

            _overlay.x = 0f + topGroup.Position.x - topGroup.OffsetToZero.x;
            _overlay.y = 0f + topGroup.Position.y - topGroup.OffsetToZero.y;

            // Since IMGUI draws from the top-left corner and position is from the bottom-left, we need to inverse to y coordinate
            _overlay.y = Screen.height - _overlay.y - (topGroupRect.height * Manager.Instance.ScaleFactor);
            
            _overlay.width = topGroupRect.width * Manager.Instance.ScaleFactor;            
            _overlay.height = (topGroupRect.height + topGroup.ToMaxOffset.y) * Manager.Instance.ScaleFactor;

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

        private void MoveButton(string text, string direction)
        {
            if (direction == "up")
            {
                if (GUILayout.RepeatButton(text, Styles.MoveButton))
                    _nudgeUp = true;
                else
                    _nudgeUp = false;
            }

            if (direction == "down")
            {
                if (GUILayout.RepeatButton(text, Styles.MoveButton))
                    _nudgeDown = true;
                else
                    _nudgeDown = false;
            }

            if (direction == "left")
            {
                if(GUILayout.RepeatButton(text, Styles.MoveButton))
                    _nudgeLeft = true;
                else
                    _nudgeLeft = false;
            }

            if (direction == "right")
            {
                if(GUILayout.RepeatButton(text, Styles.MoveButton))
                    _nudgeRight = true;
                else
                    _nudgeRight = false;
            }
        }

        private bool JumpLeftRightButton(string text)
        {
            return GUILayout.Button(text, Styles.JumpLeftRightButton);
        }

        private bool JumpUpDownButton(string text)
        {
            return GUILayout.Button(text, Styles.JumpUpDownButton);
        }

        private string TextField(string text)
        {
            return GUILayout.TextField(text, Styles.SelectTextField);
        }

        private bool Toggle(bool value, string text)
        {
            return GUILayout.Toggle(value, text, Styles.AttachToggle);
        }

        private void PrintMessage(string text)
        {
            _message = text;
            _messageTime = Time.time;
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
    }
}
