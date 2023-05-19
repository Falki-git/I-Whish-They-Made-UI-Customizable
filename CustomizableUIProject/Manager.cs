using BepInEx.Logging;
using KSP.Game;
using KSP.Messages;
using KSP.UI.Binding;
using UnityEngine;

namespace CustomizableUI
{
    public class Manager
    {
        private static Manager _instance;
        private ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("CustomizableUI.Manager");
        private Canvas _mainCanvas => GameManager.Instance?.Game?.UI?._mainCanvas;

        public Transform FlightHud => _mainCanvas?.gameObject?.GetChild("FlightHudRoot(Clone)")?.transform;
        public List<TopLevelGroup> Groups { get; set; }
        public float ScaleFactor { get; set; }
        public Vector2 Resolution { get; set; }
        public bool IsInitialized { get; set; }

        private Manager()
        { }

        public static Manager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Manager();

                return _instance;
            }
        }

        public void Initialize()
        {
            try
            {
                CreateGroups();
                ScaleFactor = _mainCanvas.scaleFactor;
                Resolution = _mainCanvas.renderingDisplaySize;
                IsInitialized = true;

                _logger.LogInfo($"Initialization successful. Top level UI groups created: {Groups.Count}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error initializating top level groups.\n{ex}");
            }
        }

        public void OnGameStateEntered(MessageCenterMessage obj)
        {
            _logger.LogInfo("OnGameStateEntered triggered.");

            if (/*!IsInitialized && */Utility.Instance.GameState == GameState.FlightView)
            {
                Initialize();
                Utility.Instance.LoadData();

                // Temporary
                /*
                UI.Instance.IsWindowOpen = true;
                GameObject.Find("BTN-CustomizableUI")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(true);
                */
            }

        }

        private void CreateGroups()
        {
            Groups = new List<TopLevelGroup>
            {
                new GameView(),
                new VerticalSpeed(),
                new GoButton(),
                new OrbitalReadout(),
                new BurnTimer(),
                new Instruments(),
                new AtmosphericIndicator(),
                new IvaPortraits(),
                new FlightControl(),
                new ActionBar(),
                new Resources(),
                new Navball(),
                new Staging(),
                new Throttle(),
                new AppBar()
            };
        }

        public void RecalculatePositionsOfGroupsAttachedToNavball(TopLevelGroup topGroup, Vector3 previousPosition)
        {
            if (topGroup is Navball)
            {
                // Move groups attached to Navball to follow the Navball
                var attachedGroups = Groups.FindAll(g => g.AttachToNavball);

                if (attachedGroups.Count == 0)
                    return;

                var deltaDistance = topGroup.Transform.position - previousPosition;

                foreach (var group in attachedGroups)
                {
                    group.Transform.position += deltaDistance;
                    group.Position += deltaDistance;
                }

            }
        }

        public void ResetAllToDefault()
        {
            foreach (var group in Groups)
                group.ResetToDefault();
        }
    }
}
