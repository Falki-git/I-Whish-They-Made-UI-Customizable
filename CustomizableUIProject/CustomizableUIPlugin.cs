using BepInEx;
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

    public void Update() => Manager.Instance.Update();
}
