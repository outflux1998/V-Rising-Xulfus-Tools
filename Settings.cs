using RetroCamera.Configuration;
using RetroCamera.Patches;
using UnityEngine;
using static RetroCamera.Configuration.Keybinding;
using static RetroCamera.Configuration.KeybindsManager;
using static RetroCamera.Configuration.OptionsManager;
using static RetroCamera.Utilities.CameraState;
using static RetroCamera.Utilities.Persistence;
using static RetroCamera.Configuration.QuipManager;
using BoolChanged = RetroCamera.Configuration.MenuOption<bool>.OptionChangedHandler<bool>;
using FloatChanged = RetroCamera.Configuration.MenuOption<float>.OptionChangedHandler<float>;
using RetroCamera.ESP;

namespace RetroCamera;
internal static class Settings
{
    public static bool Enabled { get => _enabledOption.Value; set => _enabledOption.SetValue(value); }
    public static bool FirstPersonEnabled { get => _firstPersonEnabledOption.Value; set => _firstPersonEnabledOption.SetValue(value); }
    public static bool CommandWheelEnabled { get => _commandWheelEnabled.Value; set => _commandWheelEnabled.SetValue(value); }
    public static bool AlwaysShowCrosshair { get => _alwaysShowCrosshairOption.Value; set => _alwaysShowCrosshairOption.SetValue(value); }
    public static bool ActionModeCrosshair { get => _actionModeCrosshairOption.Value; set => _actionModeCrosshairOption.SetValue(value); }
    public static float FieldOfView { get => _fieldOfViewOption.Value; set => _fieldOfViewOption.SetValue(value); }
    public static float CrosshairSize { get => _crosshairSize.Value; set => _crosshairSize.SetValue(value); }
    public static bool HideCharacterInfoPanel { get => _hideCharacterInfoPanel.Value; set => _hideCharacterInfoPanel.SetValue(value); }

    // public static CameraAimMode CameraAimMode { get => _cameraAimModeOption.GetEnumValue<CameraAimMode>(); set => _cameraAimModeOption.SetValue((int)value); }
    public static int AimOffsetX { get => (int)(Screen.width * (_aimOffsetXOption.Value / 100)); set => _aimOffsetXOption.SetValue(Mathf.Clamp(value / Screen.width, -25, 25)); }
    public static int AimOffsetY { get => (int)(Screen.height * (_aimOffsetYOption.Value / 100)); set => _aimOffsetYOption.SetValue(Mathf.Clamp(value / Screen.width, -25, 25)); }
    public static bool LockZoom { get => _lockCameraZoomOption.Value; set => _lockCameraZoomOption.SetValue(value); }
    public static float LockZoomDistance { get => _lockCameraZoomDistanceOption.Value; set => _lockCameraZoomDistanceOption.SetValue(value); }
    public static float MinZoom { get => _minZoomOption.Value; set => _minZoomOption.SetValue(value); }
    public static float MaxZoom { get => _maxZoomOption.Value; set => _maxZoomOption.SetValue(value); }
    public static bool LockPitch { get => _lockCamerPitchOption.Value; set => _lockCamerPitchOption.SetValue(value); }
    public static float LockPitchAngle { get => _lockCameraPitchAngleOption.Value * Mathf.Deg2Rad; set => _lockCameraPitchAngleOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static float MinPitch { get => _minPitchOption.Value * Mathf.Deg2Rad; set => _minPitchOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static float MaxPitch { get => _maxPitchOption.Value * Mathf.Deg2Rad; set => _maxPitchOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static bool OverTheShoulder { get => _overTheShoulderOption.Value; set => _overTheShoulderOption.SetValue(value); }
    public static float OverTheShoulderX { get => _overTheShoulderXOption.Value; set => _overTheShoulderXOption.SetValue(value); }
    public static float OverTheShoulderY { get => _overTheShoulderYOption.Value; set => _overTheShoulderYOption.SetValue(value); }

    public const float FIRST_PERSON_FORWARD_OFFSET = 1.65f;
    public const float MOUNTED_OFFSET = 1.6f;
    public const float HEAD_HEIGHT_OFFSET = 1.05f;
    public const float SHOULDER_RIGHT_OFFSET = 0.8f;

    const float ZOOM_OFFSET = 2f;

    /* unused, if overall merited will do actual buff checks
    public static readonly Dictionary<string, Vector2> FirstPersonShapeshiftOffsets = new()
    {
        { "AB_Shapeshift_Bat_Buff", new Vector2(0, 2.5f) },
        { "AB_Shapeshift_Bear_Buff", new Vector2(0.25f, 5f) },
        { "AB_Shapeshift_Bear_Skin01_Buff", new Vector2(0.25f, 5f) },
        { "AB_Shapeshift_Human_Grandma_Skin01_Buff", new Vector2(-0.1f, 1.55f) },
        { "AB_Shapeshift_Human_Buff", new Vector2(0.5f, 1.4f) },
        { "AB_Shapeshift_Rat_Buff", new Vector2(-1.85f, 2f) },
        { "AB_Shapeshift_Toad_Buff", new Vector2(-0.6f, 4.2f) },
        { "AB_Shapeshift_Wolf_Buff", new Vector2(-0.25f, 4.3f) },
        { "AB_Shapeshift_Wolf_Skin01_Buff", new Vector2(-0.25f, 4.3f) }
    };
    */

    static Toggle _enabledOption;
    static Toggle _firstPersonEnabledOption;
    static Toggle _commandWheelEnabled;
    static Slider _fieldOfViewOption;
    static Slider _crosshairSize;
    static Toggle _alwaysShowCrosshairOption;
    static Toggle _actionModeCrosshairOption;
    static Toggle _hideCharacterInfoPanel;
    // static Toggle _defaultBuildModeOption;

    // static Dropdown _cameraAimModeOption;
    static Slider _aimOffsetXOption;
    static Slider _aimOffsetYOption;

    static Toggle _lockCameraZoomOption;
    static Slider _lockCameraZoomDistanceOption;
    static Slider _minZoomOption;
    static Slider _maxZoomOption;

    static Toggle _lockCamerPitchOption;
    static Slider _lockCameraPitchAngleOption;
    static Slider _minPitchOption;
    static Slider _maxPitchOption;

    static Toggle _overTheShoulderOption;
    static Slider _overTheShoulderXOption;
    static Slider _overTheShoulderYOption;

    public static Toggle AimbotSwordToggle;
    public static Toggle AimbotSwordPredictionToggle;
    public static Slider AimbotSwordSmoothing;

    public static Toggle AimbotAxesToggle;
    public static Toggle AimbotAxesPredictionToggle;
    public static Slider AimbotAxesSmoothing;

    public static Toggle AimbotMaceToggle;
    public static Toggle AimbotMacePredictionToggle;
    public static Slider AimbotMaceSmoothing;

    public static Toggle AimbotSpearToggle;
    public static Toggle AimbotSpearPredictionToggle;
    public static Slider AimbotSpearSmoothing;

    public static Toggle AimbotReaperToggle;
    public static Toggle AimbotReaperPredictionToggle;
    public static Slider AimbotReaperSmoothing;

    public static Toggle AimbotGreatswordToggle;
    public static Toggle AimbotGreatswordPredictionToggle;
    public static Slider AimbotGreatswordSmoothing;

    public static Toggle AimbotWhipToggle;
    public static Toggle AimbotWhipPredictionToggle;
    public static Slider AimbotWhipSmoothing;

    public static Toggle AimbotSlashersToggle;
    public static Toggle AimbotSlashersPredictionToggle;
    public static Slider AimbotSlashersSmoothing;

    public static Toggle AimbotClawsToggle;
    public static Toggle AimbotClawsPredictionToggle;
    public static Slider AimbotClawsSmoothing;

    public static Toggle AimbotTwinbladeToggle;
    public static Toggle AimbotTwinbladePredictionToggle;
    public static Slider AimbotTwinbladeSmoothing;

    public static Toggle AimbotCrossbowToggle;
    public static Toggle AimbotCrossbowPredictionToggle;
    public static Slider AimbotCrossbowSmoothing;

    public static Toggle AimbotLongbowToggle;
    public static Toggle AimbotLongbowPredictionToggle;
    public static Slider AimbotLongbowSmoothing;

    public static Toggle AimbotDaggersToggle;
    public static Toggle AimbotDaggersPredictionToggle;
    public static Slider AimbotDaggersSmoothing;

    public static Toggle AimbotPistolsToggle;
    public static Toggle AimbotPistolsPredictionToggle;
    public static Slider AimbotPistolsSmoothing;


    static Keybinding _enabledKeybind;
    static Keybinding _actionModeKeybind;
    static Keybinding _toggleHUDKeybind;
    static Keybinding _toggleFogKeybind;
    static Keybinding _completeTutorialKeybind;
    static Keybinding _toggleSocialWheel;

    public static Keybinding _spellAimbotKey;


    public static Keybinding WeaponAimbotSwordKey;
    public static Keybinding WeaponAimbotAxesKey;
    public static Keybinding WeaponAimbotMaceKey;
    public static Keybinding WeaponAimbotSpearKey;
    public static Keybinding WeaponAimbotReaperKey;
    public static Keybinding WeaponAimbotGreatswordKey;
    public static Keybinding WeaponAimbotWhipKey;
    public static Keybinding WeaponAimbotSlashersKey;
    public static Keybinding WeaponAimbotClawsKey;
    public static Keybinding WeaponAimbotTwinbladeKey;
    public static Keybinding WeaponAimbotCrossbowKey;
    public static Keybinding WeaponAimbotLongbowKey;
    public static Keybinding WeaponAimbotDaggersKey;
    public static Keybinding WeaponAimbotPistolsKey;


    public static void Initialize()
    {
        try
        {
            RegisterOptions();
            RegisterKeybinds();

            TryLoadOptions();
            TryLoadKeybinds();
            TryLoadCommands();

            SaveOptions();
            SaveKeybinds();
        }
        catch (Exception ex)
        {
            Core.Log.LogError(ex);
        }
    }
    public static void AddEnabledListener(BoolChanged handler) =>
    _enabledOption.AddListener(handler);
    public static void AddFieldOfViewListener(FloatChanged handler) =>
        _fieldOfViewOption.AddListener(handler);
    public static void AddHideHUDListener(KeyHandler action) => 
        _toggleHUDKeybind.AddKeyDownListener(action);
    public static void AddHideFogListener(KeyHandler action) =>
        _toggleFogKeybind.AddKeyDownListener(action);
    public static void AddCompleteTutorialListener(KeyHandler action) =>
        _completeTutorialKeybind.AddKeyDownListener(action);
    public static void AddSocialWheelPressedListener(KeyHandler action) =>
        _toggleSocialWheel.AddKeyPressedListener(action);
    public static void AddSocialWheelUpListener(KeyHandler action) =>
        _toggleSocialWheel.AddKeyUpListener(action);

    public static bool _wasDisabled = false;
    static void RegisterOptions()
    {
        // Core.Log.LogWarning("Registering options...");

        // ESP Options
        ESP.ESPOptions.ESPEnabled = AddToggle("Enable ESP", "Enables the ESP system (Wallhacks)", true);
        ESP.ESPOptions.ShowPlayers = AddToggle("Show Players", "Shows players in the ESP", true);
        ESP.ESPOptions.ShowContainers = AddToggle("Show Containers", "Enables ESP for containers (Chests) // MAY CAUSE LAG", false);
        ESP.ESPOptions.ShowBloodCarriers = AddToggle("Show NPC Blood", "Shows the blood quality of NPCs", true);
        ESP.ESPOptions.MinBloodQualitySlider = AddSlider("Minimum NPC Blood Quality", "Minimum blood quality filter for showing in ESP", 0f, 100f, 60f, 1, 1f);
        Aimbot.AimbotEnabled = AddToggle("Aimbot", "Enables Aimbot (Check keybinds for complete keybind list)", true);
        //Aimbot.AimbotPredictionEnabled = AddToggle("Prediction", "Enable Aimbot target position prediction", true);
        AddDivider("Map Mods");

        ESP.TweaksOptions.RemoveForrestFog = AddToggle("Remove Cursed Forest Fog", "Removes the fog generated by the forest curse", false);
        ESP.TweaksOptions.RemoveMinimapFog = AddToggle("Remove Cursed Forest Minimap Fog", "Removes the fog generated by the forest curse on the MiniMap", false);

        AddDivider("PvP Mods");
        ESP.ESPOptions.AntiCounter = AddToggle("Anti Counter Tool", "Cancels outgoing attacks if enemy closest to mouse casts a counter // HIGH CPU USAGE", false);
        ESP.ESPOptions.AntiCounterMethod2 = AddToggle("Method 2", "Turns cursor away from target instead of cancelling attacks", false);

        AddDivider("Per-Weapon Aimbot");

        // Sword
        AimbotSwordToggle = AddToggle("Sword Toggle", "Enable for Sword", false);
        AimbotSwordPredictionToggle = AddToggle("Sword Prediction", "Enable prediction for Sword", false);
        AimbotSwordSmoothing = AddSlider("Sword Smoothing", "Smoothing factor for Sword", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Axes
        AimbotAxesToggle = AddToggle("Axes Toggle", "Enable for Axes", false);
        AimbotAxesPredictionToggle = AddToggle("Axes Prediction", "Enable prediction for Axes", false);
        AimbotAxesSmoothing = AddSlider("Axes Smoothing", "Smoothing factor for Axes", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Mace
        AimbotMaceToggle = AddToggle("Mace Toggle", "Enable for Mace", false);
        AimbotMacePredictionToggle = AddToggle("Mace Prediction", "Enable prediction for Mace", false);
        AimbotMaceSmoothing = AddSlider("Mace Smoothing", "Smoothing factor for Mace", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Spear
        AimbotSpearToggle = AddToggle("Spear Toggle", "Enable for Spear", true);
        AimbotSpearPredictionToggle = AddToggle("Spear Prediction", "Enable prediction for Spear", false);
        AimbotSpearSmoothing = AddSlider("Spear Smoothing", "Smoothing factor for Spear", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Reaper
        AimbotReaperToggle = AddToggle("Reaper Toggle", "Enable for Reaper", false);
        AimbotReaperPredictionToggle = AddToggle("Reaper Prediction", "Enable prediction for Reaper", false);
        AimbotReaperSmoothing = AddSlider("Reaper Smoothing", "Smoothing factor for Reaper", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Greatsword
        AimbotGreatswordToggle = AddToggle("Greatsword Toggle", "Enable for Greatsword", false);
        AimbotGreatswordPredictionToggle = AddToggle("Greatsword Prediction", "Enable prediction for Greatsword", false);
        AimbotGreatswordSmoothing = AddSlider("Greatsword Smoothing", "Smoothing factor for Greatsword", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Whip
        AimbotWhipToggle = AddToggle("Whip Toggle", "Enable for Whip", false);
        AimbotWhipPredictionToggle = AddToggle("Whip Prediction", "Enable prediction for Whip", false);
        AimbotWhipSmoothing = AddSlider("Whip Smoothing", "Smoothing factor for Whip", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Slashers
        AimbotSlashersToggle = AddToggle("Slashers Toggle", "Enable for Slashers", false);
        AimbotSlashersPredictionToggle = AddToggle("Slashers Prediction", "Enable prediction for Slashers", false);
        AimbotSlashersSmoothing = AddSlider("Slashers Smoothing", "Smoothing factor for Slashers", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Claws
        AimbotClawsToggle = AddToggle("Claws Toggle", "Enable for Claws", false);
        AimbotClawsPredictionToggle = AddToggle("Claws Prediction", "Enable prediction for Claws", false);
        AimbotClawsSmoothing = AddSlider("Claws Smoothing", "Smoothing factor for Claws", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Twinblade
        AimbotTwinbladeToggle = AddToggle("Twinblade Toggle", "Enable for Twinblade", false);
        AimbotTwinbladePredictionToggle = AddToggle("Twinblade Prediction", "Enable prediction for Twinblade", true);
        AimbotTwinbladeSmoothing = AddSlider("Twinblade Smoothing", "Smoothing factor for Twinblade", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Crossbow
        AimbotCrossbowToggle = AddToggle("Crossbow Toggle", "Enable for Crossbow", true);
        AimbotCrossbowPredictionToggle = AddToggle("Crossbow Prediction", "Enable prediction for Crossbow", true);
        AimbotCrossbowSmoothing = AddSlider("Crossbow Smoothing", "Smoothing factor for Crossbow", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Longbow
        AimbotLongbowToggle = AddToggle("Longbow Toggle", "Enable for Longbow", true);
        AimbotLongbowPredictionToggle = AddToggle("Longbow Prediction", "Enable prediction for Longbow", true);
        AimbotLongbowSmoothing = AddSlider("Longbow Smoothing", "Smoothing factor for Longbow", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Daggers
        AimbotDaggersToggle = AddToggle("Daggers Toggle", "Enable for Daggers", true);
        AimbotDaggersPredictionToggle = AddToggle("Daggers Prediction", "Enable prediction for Daggers", true);
        AimbotDaggersSmoothing = AddSlider("Daggers Smoothing", "Smoothing factor for Daggers", 0f, 1.0f, 0.65f, 1, 0.05f);

        // Pistols
        AimbotPistolsToggle = AddToggle("Pistols Toggle", "Enable for Pistols", true);
        AimbotPistolsPredictionToggle = AddToggle("Pistols Prediction", "Enable prediction for Pistols", true);
        AimbotPistolsSmoothing = AddSlider("Pistols Smoothing", "Smoothing factor for Pistols", 0f, 1.0f, 0.65f, 1, 0.05f);


        AddDivider("Camera Features");
        _enabledOption = AddToggle("Enabled", "Enable or disable Camera Mod", true);
        _firstPersonEnabledOption = AddToggle("First Person", "Enable zooming in far enough for first-person view", true);
        _commandWheelEnabled = AddToggle("Command Wheel", "Enable command wheel", false);
        _alwaysShowCrosshairOption = AddToggle("Always Show Crosshair", "Keep crosshair visible always", false);
        _actionModeCrosshairOption = AddToggle("Action Mode Crosshair", "Show crosshair during action mode", false);
        _hideCharacterInfoPanel = AddToggle("Hide Character Info Panel", "Hide character info panel", false);
        _fieldOfViewOption = AddSlider("FOV", "Camera field of view", 50, 90, 60);
        _crosshairSize = AddSlider("Crosshair Size", "Crosshair size scaling", 1f, 5f, 1f);

        AddDivider("Third Person Zoom");
        _minZoomOption = AddSlider("Min Zoom", "Minimum zoom", 1f, 15f, 1f);
        _maxZoomOption = AddSlider("Max Zoom", "Maximum zoom", 5f, 30f, 15f);
        _lockCameraZoomOption = AddToggle("Lock Zoom", "Lock zoom distance", false);
        _lockCameraZoomDistanceOption = AddSlider("Locked Zoom Distance", "Fixed zoom distance when locked", 1f, 20f, 15f);

        AddDivider("Third Person Pitch");
        _minPitchOption = AddSlider("Min Pitch", "Minimum camera pitch", 0f, 90f, 10f);
        _maxPitchOption = AddSlider("Max Pitch", "Maximum camera pitch", 0f, 90f, 90f);
        _lockCamerPitchOption = AddToggle("Lock Pitch", "Lock camera pitch", false);
        _lockCameraPitchAngleOption = AddSlider("Locked Pitch Angle", "Fixed pitch angle when locked", 0f, 90f, 60f);

        AddDivider("Third Person Aiming");
        // _cameraAimModeOption = AddDropdown("Aiming Mode", "Ability aiming style", (int)CameraAimMode.Default, Enum.GetNames(typeof(CameraAimMode)));
        _aimOffsetXOption = AddSlider("Aiming Horizontal Offset", "Aim horizontal offset", -25f, 25f, 0f);
        _aimOffsetYOption = AddSlider("Aiming Vertical Offset", "Aim vertical offset", -25f, 25f, 0f);

        AddDivider("Over Shoulder");
        _overTheShoulderOption = AddToggle("Enable Shoulder Offset", "Enable over-the-shoulder camera", false);
        _overTheShoulderXOption = AddSlider("Shoulder Horizontal Offset", "Shoulder view horizontal offset", -10f, 10f, 0f);
        _overTheShoulderYOption = AddSlider("Shoulder Vertical Offset", "Shoulder view vertical offset", -10f, 10f, 0f);

        _minZoomOption.AddListener(value =>
        {
            if (value + ZOOM_OFFSET > MaxZoom && value + ZOOM_OFFSET < _maxZoomOption.MaxValue)
                _maxZoomOption.SetValue(value + ZOOM_OFFSET);
            else if (value + ZOOM_OFFSET > _maxZoomOption.MaxValue)
                _minZoomOption.SetValue(_maxZoomOption.MaxValue - ZOOM_OFFSET);
        });

        _maxZoomOption.AddListener(value =>
        {
            if (value - ZOOM_OFFSET < MinZoom && value - ZOOM_OFFSET > _minZoomOption.MinValue)
                _minZoomOption.SetValue(value - ZOOM_OFFSET);
            else if (value - ZOOM_OFFSET < _minZoomOption.MinValue)
                _maxZoomOption.SetValue(_minZoomOption.MinValue + ZOOM_OFFSET);
        });

        _minPitchOption.AddListener(value =>
        {
            if (value > _maxPitchOption.Value && value < _maxPitchOption.MaxValue)
                _maxPitchOption.SetValue(value);
            else if (value > _maxPitchOption.MaxValue)
                _minPitchOption.SetValue(_maxPitchOption.MaxValue);
        });

        _maxPitchOption.AddListener(value =>
        {
            if (value < _minPitchOption.Value && value > _minPitchOption.MinValue)
                _minPitchOption.SetValue(value);
            else if (value < _minPitchOption.MinValue)
                _maxPitchOption.SetValue(_minPitchOption.MinValue);
        });
    }
    static void RegisterKeybinds()
    {
        // Core.Log.LogWarning("Registering keybinds...");

        _enabledKeybind = AddKeybind("Toggle RetroCamera", "Enable or disable RetroCamera functions", KeyCode.LeftBracket);
        _enabledKeybind.AddKeyDownListener(() =>
        {
            if (!EscapeMenuViewPatch._isServerPaused) _enabledOption.SetValue(!Enabled);

            if (Enabled && (_isFirstPerson || _isActionMode))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Cursor.lockState.Equals(CursorLockMode.Locked) && !Enabled)
            {
                Cursor.lockState = CursorLockMode.None;
            }
        });

        _actionModeKeybind = AddKeybind("Toggle Action Mode", "Toggle action mode", KeyCode.RightBracket);
        _actionModeKeybind.AddKeyDownListener(() =>
        {
            if (_wasDisabled && Enabled && !_isFirstPerson)
            {
                _wasDisabled = false;
                _isMouseLocked = !_isMouseLocked;
                _isActionMode = !_isActionMode;

                if (IsMenuOpen) IsMenuOpen = false;
                if (ActionWheelSystemPatch._wheelVisible) ActionWheelSystemPatch._wheelVisible = false;
                if (Cursor.lockState.Equals(CursorLockMode.Locked) && (!_isActionMode || !_isMouseLocked))
                {
                    Cursor.lockState = CursorLockMode.None;
                }

                _enabledOption.SetValue(false);
            }
            else if (Enabled && !_isFirstPerson)
            {
                _isMouseLocked = !_isMouseLocked;
                _isActionMode = !_isActionMode;

                if (IsMenuOpen) IsMenuOpen = false;
                if (ActionWheelSystemPatch._wheelVisible) ActionWheelSystemPatch._wheelVisible = false;
                if (Cursor.lockState.Equals(CursorLockMode.Locked) && (!_isActionMode || !_isMouseLocked))
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            else if (!Enabled && !_isFirstPerson && !IsMenuOpen && !EscapeMenuViewPatch._isServerPaused)
            {
                _wasDisabled = true;
                _enabledOption.SetValue(true);

                _isMouseLocked = !_isMouseLocked;
                _isActionMode = !_isActionMode;

                if (IsMenuOpen) IsMenuOpen = false;
                if (ActionWheelSystemPatch._wheelVisible) ActionWheelSystemPatch._wheelVisible = false;
                if (Cursor.lockState.Equals(CursorLockMode.Locked) && (!_isActionMode || !_isMouseLocked))
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        });

        _toggleHUDKeybind = AddKeybind("Toggle HUD", "Toggle HUD visibility", KeyCode.Backslash);

        _toggleFogKeybind = AddKeybind("Toggle Fog/Clouds", "Toggle visibility of fog and clouds (cloud ground shadows also affected)", KeyCode.Equals);

        _completeTutorialKeybind = AddKeybind("Complete Tutorial", "Pushes button for completed tutorials if applicable", KeyCode.Minus);

        _toggleSocialWheel = AddKeybind("Use Social Wheel", "Toggle social wheel visibility", KeyCode.RightAlt);

        //AIMBOT BINDS

        WeaponAimbotSwordKey = AddKeybind("Aimbot Sword", "Ativa o Aimbot com Espada", KeyCode.Mouse0);
        WeaponAimbotAxesKey = AddKeybind("Aimbot Axes", "Ativa o Aimbot com Machados", KeyCode.Mouse0);
        WeaponAimbotMaceKey = AddKeybind("Aimbot Mace", "Ativa o Aimbot com Maça", KeyCode.Mouse0);
        WeaponAimbotSpearKey = AddKeybind("Aimbot Spear", "Ativa o Aimbot com Lança", KeyCode.Mouse0);
        WeaponAimbotReaperKey = AddKeybind("Aimbot Reaper", "Ativa o Aimbot com Foice", KeyCode.Mouse0);
        WeaponAimbotGreatswordKey = AddKeybind("Aimbot Greatsword", "Ativa o Aimbot com Espadona", KeyCode.Mouse0);
        WeaponAimbotWhipKey = AddKeybind("Aimbot Whip", "Ativa o Aimbot com Chicote", KeyCode.Mouse0);
        WeaponAimbotSlashersKey = AddKeybind("Aimbot Slashers", "Ativa o Aimbot com Adagas Duplas", KeyCode.Mouse0);
        WeaponAimbotClawsKey = AddKeybind("Aimbot Claws", "Ativa o Aimbot com Garras", KeyCode.Mouse0);
        WeaponAimbotTwinbladeKey = AddKeybind("Aimbot Twinblade", "Ativa o Aimbot com Lâminas Gêmeas", KeyCode.Mouse0);
        WeaponAimbotCrossbowKey = AddKeybind("Aimbot Crossbow", "Ativa o Aimbot com Besta", KeyCode.Mouse0);
        WeaponAimbotLongbowKey = AddKeybind("Aimbot Longbow", "Ativa o Aimbot com Arco Longo", KeyCode.Mouse0);
        WeaponAimbotDaggersKey = AddKeybind("Aimbot Daggers", "Ativa o Aimbot com Adagas", KeyCode.Mouse0);
        WeaponAimbotPistolsKey = AddKeybind("Aimbot Pistols", "Ativa o Aimbot com Pistolas", KeyCode.Mouse0);
        _spellAimbotKey = AddKeybind("Spell Aimbot Key", "Keybind para ativar o aimbot com feitiço", KeyCode.CapsLock);



    }
    public static bool TryLoadOptions()
    {
        var loaded = LoadOptions();

        if (loaded == null)
        {
            Core.Log.LogWarning("No options saved!");
            return false;
        }
            

        foreach (var (key, loadedOption) in loaded)
        {
            if (Options.TryGetValue(key, out var registeredOption))
                registeredOption.ApplySaved(loadedOption);
        }

        return true;
    }
    public static bool TryLoadKeybinds()
    {
        var loaded = LoadKeybinds();

        if (loaded == null)
        {
            Core.Log.LogWarning("No keybinds saved!");
            return false;
        }

        foreach (var (key, loadedBind) in loaded)
        {
            if (Keybinds.TryGetValue(key, out var registeredBind))
                registeredBind.ApplySaved(loadedBind);
        }

        return true;
    }
}