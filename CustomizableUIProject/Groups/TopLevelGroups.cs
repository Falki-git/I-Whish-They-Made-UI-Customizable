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
            RectTransform = Manager.Instance.FlightHud.gameObject.GetChild(groupName).transform.GetChild(transformIndex).GetComponent<RectTransform>();
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
            ToCenterOffset.x = -68;
            ToCenterOffset.y = -28;
        }
    }

    public class VerticalSpeed : TopLevelGroup
    {
        public VerticalSpeed() : base("widget_indicator_verticalspeed_horizontal_new(Clone)", 0)
        {
            ToCenterOffset.x = -42.5f;
            ToCenterOffset.y = -70f;
            AttachToNavball = true;
        }
    }

    public class GoButton : TopLevelGroup
    {
        public GoButton() : base("group_gobutton(Clone)", 0)
        {
            ToCenterOffset.x = 104f;
            ToCenterOffset.y = -36f;
        }
    }

    public class OrbitalReadout : TopLevelGroup
    {
        public OrbitalReadout() : base("OrbitalReadoutInstrument_Widget(Clone)", 0)
        {
            ToCenterOffset.x = -120f;
            ToCenterOffset.y = -28f;
        }
    }

    public class BurnTimer : TopLevelGroup
    {
        public BurnTimer() : base("group_burntimer(Clone)", 1)
        {
            ToCenterOffset.x = -204f;
            ToCenterOffset.y = -72f;
        }
    }

    public class Instruments : TopLevelGroup
    {
        public Instruments() : base("group_instruments(Clone)", 0)
        {
            ToCenterOffset.x = -155f;
            ToCenterOffset.y = -28f;
        }
    }

    public class AtmosphericIndicator : TopLevelGroup
    {
        public AtmosphericIndicator() : base("group_atmospheric_indicator(Clone)", 0)
        {
            ToCenterOffset.x = -42.5f;
            ToCenterOffset.y = -70f;
            AttachToNavball = true;
        }
    }
    public class IvaPortraits : TopLevelGroup
    {
        public IvaPortraits() : base("group_ivaportraits(Clone)", 0)
        {
            ToCenterOffset.x = 206.5f;
            ToCenterOffset.y = 60f;
        }
    }

    public class FlightControl : TopLevelGroup
    {
        public FlightControl() : base("group_flightcontrol(Clone)", 0)
        {
            ToCenterOffset.x = -88f;
            ToCenterOffset.y = -120f;
        }
    }

    public class ActionBar : TopLevelGroup
    {
        public ActionBar() : base("group_actionbar(Clone)", 0)
        {
            ToCenterOffset.x = -28f;
            ToCenterOffset.y = 108f;
        }
    }

    public class Resources : TopLevelGroup
    {
        public Resources() : base("NonStageableResources(Clone)", 0)
        {
            ToCenterOffset.x = 104f;
            ToCenterOffset.y = 60f;
        }
    }

    public class Navball : TopLevelGroup
    {
        public Navball() : base("group_navball(Clone)", 0)
        {
            ToCenterOffset.x = -150f;
            ToCenterOffset.y = -150f;
        }
    }

    public class Staging : TopLevelGroup
    {
        public Staging() : base("group_flightstaging(Clone)", 0)
        {
            ToCenterOffset.x = 130f;
            ToCenterOffset.y = -55f;
        }
    }

    public class Throttle : TopLevelGroup
    {
        public Throttle() : base("group_throttle(Clone)", 0)
        {
            ToCenterOffset.x = -42.5f;
            ToCenterOffset.y = -145f;
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

            ToCenterOffset.x = 1270f;
            ToCenterOffset.y = 672f;
        }
    }
}
