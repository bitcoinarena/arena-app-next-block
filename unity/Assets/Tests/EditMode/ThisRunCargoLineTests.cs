// SPDX-License-Identifier: LicenseRef-Bitcoin-Arena-1.0
// Copyright (c) 2026 The Bitcoin Arena authors. See LICENSE.
using NUnit.Framework;

/// <summary>
/// THE "THIS RUN" CARGO LINE (bitcoin-arena kanban #2943, feedback #273/#275): it read the block's delivered BTC under the words
/// "on the truck", so a loaded truck read "on the truck: 0 BTC" for the whole run (measured live on UI 705, 3.44675514 BTC loaded).
/// </summary>
public class ThisRunCargoLineTests
{
    [Test]
    public void ALoadedTruckShowsItsLoadNotTheDeliveredSum()
    {
        Assert.AreEqual("on the truck: 3.44675514 BTC", DriveHud.ThisRunCargoLine(true, 3.44675514, 0));
        Assert.AreEqual("on the truck: 0.619 BTC", DriveHud.ThisRunCargoLine(true, 0.619, 1.5), "loaded again after a drop-off: the load");
    }

    [Test]
    public void AnEmptyTruckShowsWhatWasDeliveredOrNothing()
    {
        Assert.AreEqual("delivered: 1.5 BTC", DriveHud.ThisRunCargoLine(false, 0, 1.5));
        Assert.AreEqual("on the truck: nothing", DriveHud.ThisRunCargoLine(false, 0, 0));
        StringAssert.DoesNotContain("0 BTC", DriveHud.ThisRunCargoLine(false, 0, 0), "never the old '0 BTC' that read as a load that does not count");
    }
}
