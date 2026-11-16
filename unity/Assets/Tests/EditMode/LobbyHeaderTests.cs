// SPDX-License-Identifier: LicenseRef-Bitcoin-Arena-1.0
// Copyright (c) 2026 The Bitcoin Arena authors. See LICENSE.
using NUnit.Framework;
using Arena;

/// <summary>
/// THE HEADER ABOVE THE ROUNDS AND THE LINE UNDER THEM (follow-up, 2026-10-06, plan comment 430): one
/// shared header line names each kind of round listed with that kind's OWN signer numbers - from its rows, the same source
/// as the row - and the line under the rows speaks only of the round PLAY joins, in that row's words. Measured before: the
/// header said "JOIN NEEDS 2 OF 2 SIGNER NODES" (the first row's number, a fallback 2, the same number both sides) beside a
/// paid row that needs 5 of 7, and the under-line "2 of 2 signer nodes agree" / "both signer nodes".
/// </summary>
public class LobbyHeaderTests
{
    static LobbyRow Paid(string id, int agree = 7, int needed = 5, int total = 7) =>
        new LobbyRow { id = id, free = false, stakeSats = 100, agree = agree, needed = needed, total = total, joinable = true, leftS = 40 };
    static LobbyRow Free(string id, int agree = 2, int needed = 2, int total = 2) =>
        new LobbyRow { id = id, free = true, agree = agree, needed = needed, total = total, joinable = true, leftS = 40 };

    [Test]
    public void PaidAndFreeTogether_TheHeaderNamesEachKindWithItsOwnNumbers()
    {
        Assert.AreEqual("ROUNDS · PAID: 5 OF 7 REFEREES SIGN · FREE: 2 OF 2",
            DailyLobby.HeaderWords(new[] { Free("a5004d15aa"), Paid("c71ee776bb") }));
        Assert.AreEqual("ROUNDS · PAID: 5 OF 7 REFEREES SIGN · FREE: 2 OF 2",
            DailyLobby.HeaderWords(new[] { Paid("c71ee776bb"), Free("a5004d15aa") }), "the order of the rows does not change the header");
    }

    [Test]
    public void PaidOnlyAndFreeOnly_JoinNeedsThatKindsOwnNumbers()
    {
        Assert.AreEqual("ROUNDS · 5 OF 7 REFEREES SIGN EACH RESULT", DailyLobby.HeaderWords(new[] { Paid("c71ee776bb"), Paid("5fbafe9bcc") }));
        Assert.AreEqual("ROUNDS · 2 OF 2 REFEREES SIGN EACH RESULT", DailyLobby.HeaderWords(new[] { Free("a5004d15aa") }));
        Assert.AreEqual("ROUNDS · 3 OF 4 REFEREES SIGN EACH RESULT", DailyLobby.HeaderWords(new[] { Free("a5004d15aa", 4, 3, 4) }),
            "a free round of another committee says its own numbers - no fixed 2");
    }

    [Test]
    public void TwoRowsOfOneKindWithDifferentCounts_ThatKindSaysPerRound()
    {
        Assert.AreEqual("ROUNDS · PAID: PER ROUND · FREE: 2 OF 2",
            DailyLobby.HeaderWords(new[] { Paid("c71ee776bb"), Paid("5fbafe9bcc", 4, 3, 4), Free("a5004d15aa") }));
        Assert.AreEqual("ROUNDS · REFEREES PER ROUND", DailyLobby.HeaderWords(new[] { Paid("c71ee776bb"), Paid("5fbafe9bcc", 4, 3, 4) }));
    }

    [Test]
    public void AnOlderPageWithNoCommitteeSize_SaysNNeededWithoutOf()
    {
        var old = Paid("c71ee776bb", 7, 5, -1);
        Assert.AreEqual("ROUNDS · 5 REFEREES SIGN EACH RESULT", DailyLobby.HeaderWords(new[] { old }));
        Assert.AreEqual("ROUNDS · PAID: 5 NEEDED · FREE: 2 OF 2", DailyLobby.HeaderWords(new[] { old, Free("a5004d15aa") }));
        foreach (var h in new[] { DailyLobby.HeaderWords(new[] { old }), DailyLobby.HeaderWords(new[] { old, Free("a5004d15aa") }) })
            StringAssert.DoesNotContain("5 OF", h);
    }

    [Test]
    public void NothingListed_TheHeaderSaysRounds()
    {
        Assert.AreEqual("ROUNDS", DailyLobby.HeaderWords(new LobbyRow[0]));
        Assert.AreEqual("ROUNDS", DailyLobby.HeaderWords(null));
    }

    [Test]
    public void TheLineUnderTheRows_SaysThePickedRoundInItsRowsWords()
    {
        Assert.AreEqual("PLAY joins round c71ee776 - 7 referees ready, 5 needed", DailyLobby.HintWords(Paid("c71ee776bb"), 2));
        Assert.AreEqual("PLAY joins round a5004d15 - 2 referees ready, 2 needed", DailyLobby.HintWords(Free("a5004d15aa"), 2));
        Assert.AreEqual("PLAY joins round a5004d15 - 1 referee ready, 2 needed", DailyLobby.HintWords(Free("a5004d15aa", 1), 1));
    }

    [Test]
    public void Waiting_TheLineNoLongerSaysBoth()
    {
        Assert.AreEqual("PLAY waits for a round with enough referees", DailyLobby.HintWords(null, 2));
        Assert.AreEqual("PLAY waits for an open round", DailyLobby.HintWords(null, 0));
        StringAssert.DoesNotContain("both", DailyLobby.HintWords(null, 2));
    }
}
