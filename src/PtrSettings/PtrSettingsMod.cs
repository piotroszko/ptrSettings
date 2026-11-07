using MelonLoader;
using PtrLib.Diagnostics;
using PtrLib.Game;
using PtrLib.Settings;
using PtrSettings.Settings;

[assembly: MelonInfo(typeof(PtrSettings.PtrSettingsMod), PtrSettings.SettingsInfo.Name, PtrSettings.SettingsInfo.Version, PtrSettings.SettingsInfo.Author)]
[assembly: MelonGame("Waseku", "Data Center")]

namespace PtrSettings;

internal static class SettingsInfo
{
    internal const string Name = "PtrSettings";
    internal const string Version = "0.1.0";
    internal const string Author = "piotr";
}

public sealed class PtrSettingsMod : MelonMod
{
    private readonly ModSettings _settings = new ModSettings();
    private PauseMenuSettings? _ui;
    private bool _gameplay;

    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg($"{SettingsInfo.Name} {SettingsInfo.Version} loaded.");

        foreach (var registration in ModuleSettingsRegistry.All)
            Merge(registration);

        ModuleSettingsRegistry.Registered += Merge;
        _ui = new PauseMenuSettings(_settings, new ModuleLog(LoggerInstance, "Settings"));
    }

    public override void OnUpdate()
    {
        if (_gameplay)
            _ui?.OnUpdate();
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (!string.Equals(sceneName, SceneNames.Gameplay, System.StringComparison.Ordinal))
            return;

        _gameplay = true;
        _ui?.OnGameplayEnter();
    }

    public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
    {
        if (!string.Equals(sceneName, SceneNames.Gameplay, System.StringComparison.Ordinal))
            return;

        _gameplay = false;
        _ui?.OnGameplayExit();
    }

    public override void OnDeinitializeMelon()
    {
        ModuleSettingsRegistry.Registered -= Merge;
        _ui?.OnGameplayExit();
        _ui = null;
    }

    private void Merge(ModuleSettingsRegistration registration)
    {
        foreach (var section in registration.Settings.Sections)
            _settings.AddExternal(section);
    }
}
