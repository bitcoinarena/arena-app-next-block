// SPDX-License-Identifier: LicenseRef-Bitcoin-Arena-1.0
// Copyright (c) 2026 The Bitcoin Arena authors. See LICENSE.
using System.Runtime.InteropServices;
using UnityEngine;

namespace Arena
{
    /// <summary>
    /// PRACTICE IS AN ADMIN'S (kanban #1092, the owner 2026-11-03: "Practice nur dann anzeigen wenn es ein bestaetigter Admin ist ueber das
    /// Frontend Flag"). The page holds the flag: opened once with ?admin=&lt;code&gt;, it checks the code against its SHA-256 and keeps it in
    /// the browser (index.html, window.ARENA_ADMIN). Unity asks once a second. Without it the PRACTICE box (and the SIM switch under it) is
    /// not drawn and practice is off - ?practice=1 cannot turn it on. A signed run never sent practice anyway (PracticeMode.InputFor).
    /// </summary>
    public static class AdminFlag
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern int ArenaIsAdmin();
#else
        // the editor and its tests are the developer's own
        private static int ArenaIsAdmin() => 1;
#endif
        /// <summary>Whether this browser holds the confirmed admin flag.</summary>
        public static bool On { get; private set; }

        static float _askedAt = -10f;
        static bool _said;

        /// <summary>Called once a frame by DriveHud.Update: asks the page once a second, and keeps practice off for a player who is not admin.</summary>
        public static void Poll()
        {
            if (Time.unscaledTime - _askedAt >= 1f)
            {
                _askedAt = Time.unscaledTime;
                bool on;
                try { on = ArenaIsAdmin() == 1; } catch { on = false; }
                if (on != On || !_said) { On = on; _said = true; Debug.Log($"[admin] {(on ? "admin: PRACTICE shown" : "not admin: PRACTICE hidden and off")}"); }
            }
            if (!On && PracticeMode.On) PracticeMode.Withdraw();
        }
    }
}
