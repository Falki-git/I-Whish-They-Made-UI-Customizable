using BepInEx.Logging;
using KSP.Game;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomizableUI
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TopLevelGroup : BaseGroup
    {
        private ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("CustomizableUI.TopLevelGroup");

        [JsonProperty]
        public bool AttachToNavball = false;

        public TopLevelGroup() { }

        public TopLevelGroup(string groupName, int transformIndex)
        {
            Transform = Manager.Instance.FlightHud.gameObject.GetChild(groupName).transform.GetChild(transformIndex);
            Name = Utility.Instance.GetGroupName(groupName);
            Children = ChildGroup.GetAllChildren(Transform, this);
            DefaultPosition = Position = Transform.position;
            IsActive = Transform.gameObject.activeSelf;
            IsActive = true;
            RectTransform = Transform.GetComponent<RectTransform>();
        }

        public static int SelectedIndex = 0;
        public static void SelectPreviousTopGroup() => SelectedIndex--;
        public static void SelectNextTopGroup() => SelectedIndex++;

        public void LoadData(TopLevelGroup loadedGroup)
        {
            Transform.position = Position = loadedGroup.Position;
            IsActive = loadedGroup.IsActive;
            AttachToNavball = loadedGroup.AttachToNavball;

            try
            {
                if (Children != null && loadedGroup.Children != null && Children.Count == loadedGroup.Children.Count)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        Children[i].LoadData(loadedGroup.Children[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error loading data from child groups. Children data won't be changed.\n{ex}");
            }
        }
    }

    public class GameView : TopLevelGroup
    {
        public GameView() : base ("group_gameview(Clone)", 0)
        {
            ToCenterOffset.x = 0;
            ToCenterOffset.y = -15;
            OffsetToZero.x = 0;
            OffsetToZero.y = 0;
        }
    }

    public class VerticalSpeed : TopLevelGroup
    {
        public VerticalSpeed() : base("widget_indicator_verticalspeed_horizontal_new(Clone)", 0)
        {
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 586;
            OffsetToZero.y = 305;
            ToMaxOffset.x = 20;
            ToMaxOffset.y = -39;
            AttachToNavball = true;
        }
    }

    public class GoButton : TopLevelGroup
    {
        public GoButton() : base("group_gobutton(Clone)", 0)
        {
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 208;
            OffsetToZero.y = 0;
            ToMaxOffset.x = 0;
            ToMaxOffset.y = 0;
        }
    }

    public class OrbitalReadout : TopLevelGroup
    {
        public OrbitalReadout() : base("OrbitalReadoutInstrument_Widget(Clone)", 0)
        {
            ToCenterOffset.x = -70f;
            ToCenterOffset.y = 22f;
            OffsetToZero.x = 838;
            OffsetToZero.y = 524;
            ToMaxOffset.x = -139;
            ToMaxOffset.y = 46;
        }
    }

    public class BurnTimer : TopLevelGroup
    {
        public BurnTimer() : base("group_burntimer(Clone)", 0)
        {
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 0;
            OffsetToZero.y = 0;
            ToMaxOffset.x = 0;
            ToMaxOffset.y = 0;
        }
    }

    public class Instruments : TopLevelGroup
    {
        public Instruments() : base("group_instruments(Clone)", 0)
        {
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 0;
            OffsetToZero.y = 0;
            ToMaxOffset.x = 0;
            ToMaxOffset.y = 0;
        }
    }

    public class AtmosphericIndicator : TopLevelGroup
    {
        public AtmosphericIndicator() : base("group_atmospheric_indicator(Clone)", 0)
        {
            ToCenterOffset.x = 0;
            ToCenterOffset.y = -14f;
            OffsetToZero.x = 586;
            OffsetToZero.y = 452;
            ToMaxOffset.x = 16;
            ToMaxOffset.y = -39;
            AttachToNavball = true;
        }
    }
    public class IvaPortraits : TopLevelGroup
    {
        public IvaPortraits() : base("group_ivaportraits(Clone)", 0)
        {
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 283;
            OffsetToZero.y = 120;
            ToMaxOffset.x = 0;
            ToMaxOffset.y = 0;
        }
    }

    public class FlightControl : TopLevelGroup
    {
        public FlightControl() : base("group_flightcontrol(Clone)", 0)
        {
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 0;
            OffsetToZero.y = 0;
            ToMaxOffset.x = 0;
            ToMaxOffset.y = 0;
        }
    }

    public class ActionBar : TopLevelGroup
    {
        public ActionBar() : base("group_actionbar(Clone)", 0)
        {
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 4;
            OffsetToZero.y = 216;
            ToMaxOffset.x = 0;
            ToMaxOffset.y = 0;
        }
    }

    public class Resources : TopLevelGroup
    {
        public Resources() : base("NonStageableResources(Clone)", 0)
        {
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 208;
            OffsetToZero.y = 120;
            ToMaxOffset.x = 0;
            ToMaxOffset.y = 0;
        }
    }

    public class Navball : TopLevelGroup
    {
        public Navball() : base("group_navball(Clone)", 0)
        {
            ToCenterOffset.x = -37f;
            ToCenterOffset.y = 0;
            OffsetToZero.x = 36;
            OffsetToZero.y = 0;
            ToMaxOffset.x = -73;
            ToMaxOffset.y = -20;
        }
    }

    public class Staging : TopLevelGroup
    {
        public Staging() : base("group_flightstaging(Clone)", 0)
        {
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 240;
            OffsetToZero.y = -24;
            ToMaxOffset.x = 57;
            ToMaxOffset.y = 0;
        }
    }

    public class Throttle : TopLevelGroup
    {
        public Throttle() : base("group_throttle(Clone)", 0)
        {
            ToCenterOffset.x = -0;
            ToCenterOffset.y = -110f;
            OffsetToZero.x = 60;
            OffsetToZero.y = 63;
            ToMaxOffset.x = 15;
            ToMaxOffset.y = -197;
            AttachToNavball = true;
        }
    }

    public class AppBar : TopLevelGroup
    {
        public AppBar()
        {
            Transform = GameManager.Instance.Game.UI._scaledPopupCanvas.gameObject.GetChild("Container").GetChild("ButtonBar").transform;
            Name = Utility.Instance.GetGroupName(Transform.name);
            Children = ChildGroup.GetAllChildren(Transform, this);
            DefaultPosition = Position = Transform.position;
            IsActive = Transform.gameObject.activeSelf;
            IsActive = true;
            RectTransform = Transform.GetComponent<RectTransform>();
            ToCenterOffset.x = 0f;
            ToCenterOffset.y = 0f;
            OffsetToZero.x = 84;
            OffsetToZero.y = 0;
            ToMaxOffset.x = 0;
            ToMaxOffset.y = 0;
        }
    }
}
