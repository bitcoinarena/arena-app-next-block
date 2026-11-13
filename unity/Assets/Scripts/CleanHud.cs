// SPDX-License-Identifier: LicenseRef-Bitcoin-Arena-1.0
// Copyright (c) 2026 The Bitcoin Arena authors. See LICENSE.
using UnityEngine;

/// <summary>
/// CLEAN SCREEN FOR THE TRAILER (2026-09-27: "remove all HUDs with a secret key, player controls included -
/// they still work - so the trailer does not have an overloaded UI"). Shift+H toggles it. While it is on, every
/// gameplay OnGUI returns before drawing - DriveHud, ChartHud, the pads, the loadout, the spectate EXIT and the
/// other players' labels - and the page hides its own HTML overlays (wallet, backup warning, node line).
/// Only the drawing stops: the pads, the keyboard and the spectate camera read their input in Update, so the
/// truck still drives. The title screen is not covered - it is a menu, not a HUD.
/// </summary>
public static class CleanHud
{
    public static bool On { get; private set; }

    static bool _asked, _fromQuery;
    static SplashScreen _splash;

    /// <summary>?hud=0 on the page's address cleans the screen for camera shots on a device with no keyboard (kanban #955). The title
    /// screen stays usable: the lobby reads no rounds while the clean screen is on, so it holds only while the title screen is down.</summary>
    public static void FromQuery(string hud) { if (hud == "0") _fromQuery = true; }

    static void Set(bool on)
    {
        if (on == On) return;
        On = on;
        Arena.DailyLobby.HudClean(on);
    }

    /// <summary>Called once a frame by DriveHud.Update.</summary>
    public static void Poll()
    {
        if (!_asked) { _asked = true; FromQuery(Arena.ArenaCore.QueryParam("hud")); }
        if (_fromQuery)
        {
            if (_splash == null) _splash = Object.FindFirstObjectByType<SplashScreen>();
            Set(_splash == null || !_splash.Showing);
        }
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        if (k == null || !k.hKey.wasPressedThisFrame || !k.shiftKey.isPressed) return;
#else
        if (!Input.GetKeyDown(KeyCode.H) || !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) return;
#endif
        _fromQuery = false;                              // the key decides from here on
        Set(!On);
    }
}
