using BepInEx.Logging;
using KSP.Game;
using Newtonsoft.Json;
using System.Reflection;

namespace CustomizableUI
{
    public class Utility
    {
        private static Utility _instance;
        private string _path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SavedData.json");
        private ManualLogSource _logger = Logger.CreateLogSource("CustomizableUI.Utility");

        public Dictionary<string, string> GroupNames;

        public GameState GameState => GameManager.Instance?.Game?.GlobalGameState?.GetGameState()?.GameState ?? GameState.Invalid;
        
        private Utility()
        {
            InitializeGroupNames();
        }

        public static Utility Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Utility();

                return _instance;
            }
        }

        private void InitializeGroupNames()
        {
            GroupNames = new Dictionary<string, string>
            {
                { "group_gameview(Clone)", "GAME.VIEW" },
                { "widget_indicator_verticalspeed_horizontal_new(Clone)", "VERTICAL.SPEED" },
                { "group_gobutton(Clone)", "GO.BUTTON" },
                { "OrbitalReadoutInstrument_Widget(Clone)", "ORBITAL.INFO" },
                { "group_burntimer(Clone)", "BURN.TIMER" },
                { "group_instruments(Clone)", "TIME.WARP" },
                { "group_atmospheric_indicator(Clone)", "ATMOSPHERIC.INDICATOR" },
                { "group_ivaportraits(Clone)", "IVA.PORTRAITS" },
                { "group_flightcontrol(Clone)", "SAS.CONTROL" },
                { "group_actionbar(Clone)", "VESSEL.ACTIONS" },
                { "NonStageableResources(Clone)", "VESSEL.RESOURCES" },
                { "group_navball(Clone)", "NAVBALL" },
                { "group_flightstaging(Clone)", "STAGING" },
                { "group_throttle(Clone)", "THROTTLE" },
                { "ButtonBar", "APP.BAR" }
            };
        }

        public string GetGroupName(string key)
        {
            string toReturn;
            GroupNames.TryGetValue(key, out toReturn);
            return toReturn ?? key;
        }

        public void SaveData()
        {
            try
            {
                var serializedData = JsonConvert.SerializeObject(Manager.Instance.Groups);
                File.WriteAllText(_path, serializedData);

                _logger.LogInfo("Save data successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving data. Full error:\n" + ex);
            }
        }

        public void LoadData()
        {
            try
            {
                List<TopLevelGroup> deserializedGroups = JsonConvert.DeserializeObject<List<TopLevelGroup>>(File.ReadAllText(_path));

                foreach (var group in Manager.Instance.Groups)
                {
                    var loadedGroup = deserializedGroups.Find(g => g.Name == group.Name);
                    if (loadedGroup != null)
                        group.LoadData(loadedGroup);
                }

                _logger.LogInfo("Load data successful.");
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning($"Error loading data. File was not found at the expected location. Full error:\n" + ex);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error trying to load data. Full error:\n" + ex);
            }
        }
    }
}
