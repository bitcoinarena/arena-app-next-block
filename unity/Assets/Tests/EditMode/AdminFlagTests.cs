// SPDX-License-Identifier: LicenseRef-Bitcoin-Arena-1.0
// Copyright (c) 2026 The Bitcoin Arena authors. See LICENSE.
using System.IO;
using NUnit.Framework;
using UnityEngine;
using Arena;

/// <summary>
/// PRACTICE IS AN ADMIN'S (kanban #1092). The page's confirmed flag reaches Unity through arena.jslib ArenaIsAdmin; without it the PRACTICE
/// box is not drawn and practice is withdrawn.
/// </summary>
public class AdminFlagTests
{
    [Test]
    public void TheBridgeAnswersOnlyTheConfirmedFlag()
    {
        var lib = File.ReadAllText(Path.Combine(Application.dataPath, "Plugins/WebGL/arena.jslib"));
        StringAssert.Contains("ArenaIsAdmin: function () {", lib);
        StringAssert.Contains("window.ARENA_ADMIN === true ? 1 : 0", lib);
    }

    [Test]
    public void ThePracticeBoxIsDrawnOnlyForAnAdmin()
    {
        var hud = File.ReadAllText(Path.Combine(Application.dataPath, "Scripts/DriveHud.cs"));
        int draw = hud.IndexOf("void DrawPractice(int w, int h)");
        Assert.Greater(draw, 0);
        int guard = hud.IndexOf("if (!Arena.AdminFlag.On) return;", draw);
        int firstDraw = hud.IndexOf("GUI.DrawTexture", draw);
        Assert.Greater(guard, draw, "DrawPractice asks AdminFlag");
        Assert.Less(guard, firstDraw, "before anything is drawn");
        StringAssert.Contains("Arena.AdminFlag.Poll();", hud);
    }

    [Test]
    public void WithdrawTurnsPracticeOff()
    {
        bool was = PracticeMode.On;
        try
        {
            PracticeMode.On = true;
            PracticeMode.Withdraw();
            Assert.IsFalse(PracticeMode.On);
            Assert.AreEqual("not admin", PracticeMode.Source);
            Assert.AreEqual(0, PracticeMode.InputFor(false), "no practice reaches the core");
        }
        finally { PracticeMode.On = was; }
    }
}
