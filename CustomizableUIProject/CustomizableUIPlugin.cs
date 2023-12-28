using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using KSP.Game;
using KSP.Messages;
using KSP.UI.Binding;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using UnityEngine;

namespace CustomizableUI;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class CustomizableUIPlugin : BaseSpaceWarpPlugin
{
    public const string ModGuid = MyPluginInfo.PLUGIN_GUID;
    public const string ModName = MyPluginInfo.PLUGIN_NAME;
    public const string ModVer = MyPluginInfo.PLUGIN_VERSION;

    public const string ToolbarFlightButtonID = "BTN-CustomizableUI";

    public static CustomizableUIPlugin Instance { get; set; }

    private ManualLogSource _logger = BepInEx.Logging.Logger.CreateLogSource("CustomizableUI");
    
    private ConfigEntry<bool> _enableKeybinding;
    private ConfigEntry<KeyCode> _keybind1;
    private ConfigEntry<KeyCode> _keybind2;

    public override void OnInitialized()
    {
        base.OnInitialized();

        Instance = this;

        // Register Flight AppBar button
        Appbar.RegisterAppButton(
            "IWTM UI Customizable",
            ToolbarFlightButtonID,
            AssetManager.GetAsset<Texture2D>($"{Info.Metadata.GUID}/images/icon.png"),
            isOpen =>
            {
                UI.Instance.IsWindowOpen = isOpen;
                GameObject.Find(ToolbarFlightButtonID)?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(isOpen);
                if (isOpen && !Manager.Instance.IsInitialized)
                    Manager.Instance.Initialize();
            }
        );

        SubscribeToMessages();
        Styles.Initialize();
        InitializeConfigs();
    }

    private void SubscribeToMessages()
    {
        try
        {
            GameManager.Instance.Game.Messages.Subscribe<GameStateEnteredMessage>(new Action<MessageCenterMessage>(message => Manager.Instance.OnGameStateEntered(message)));
            _logger.LogInfo("Successfully subscribed to GameStateEntered message.");
        }
        catch(Exception ex)
        {
            _logger.LogInfo($"Error subscribing to GameStateEntered message.\n{ex}");
        }
    }

    private void OnGUI()
    {
        UI.Instance.OnGUI();
    }

    public void Update()
    {
        Manager.Instance.Update();

        if ((_enableKeybinding?.Value ?? false) &&
            (_keybind1.Value != KeyCode.None ? Input.GetKey(_keybind1.Value) : true) &&
            (_keybind2.Value != KeyCode.None ? Input.GetKeyDown(_keybind2.Value) : true))
        {
            UI.Instance.IsWindowOpen = !UI.Instance.IsWindowOpen;
        }
    }

    private void InitializeConfigs()
    {
        _enableKeybinding = Config.Bind(
            "Keybinding",
            "Enable keybinding",
            true,
            "Enables or disables keyboard shortcuts to show or hide the GUI"
        );
            
        _keybind1 = Config.Bind(
            "Keybinding",
            "Keycode 1",
            KeyCode.LeftControl,
            "First keycode."
        );
            
        _keybind2 = Config.Bind(
            "Keybinding",
            "Keycode 2",
            KeyCode.I,
            "Second keycode.");
    }
}
