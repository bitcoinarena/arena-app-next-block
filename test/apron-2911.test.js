// SPDX-License-Identifier: LicenseRef-Bitcoin-Arena-1.0
// Copyright (c) 2026 The Bitcoin Arena authors. See LICENSE.
// bitcoin-arena kanban #2911 (feedback #191): the score's apron test measured from START_Z, bay 13's door, for every truck; since #828 a truck
// starts at its own door, so a truck driving forward but still inside the 22 m apron earned exposure from doors 6-10, 16, 17 and 20.
// Core 7d197042 (8cbc74b2 with the test on the run's x alone) earns nothing inside the apron from any door - and per door a golden state
// hash pins the run, so the core cannot move unseen.
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { createHash } from 'node:crypto';

const run = async (core) => {
  const sim = await import(`../frozen/${core}/sim.js`), state = await import(`../frozen/${core}/state.js`), rng = await import(`../frozen/${core}/rng.js`);
  const seeds = new Map();
  for (let w = 1; seeds.size < sim.BAY_COUNT && w < 1e6; w++) { const b = rng.spawnBay(w); if (!seeds.has(b)) seeds.set(b, w); }
  const out = [];
  for (let b = 0; b < sim.BAY_COUNT; b++) {
    const st = sim.init(seeds.get(b)), s = st.i32;
    for (let t = 0; t < 3000 && s[state.O_TRUCK + state.T_X] - sim.START_X < 20 * 65536; t++) sim.step(st, 45875, 0, 0, 0, 0);
    out.push({ bay: b, exposure: s[state.O_SCORE + state.S_EXPOSURE], hash: createHash('sha256').update(Buffer.from(state.hash(st))).digest('hex').slice(0, 16) });
  }
  return out;
};

test('#2911 inside the apron no door earns exposure (core 7d197042); 8cbc74b2 did from eight doors', async () => {
  const now = await run('7d197042'), before = await run('8cbc74b2');
  assert.deepEqual(now.filter((r) => r.exposure !== 0).map((r) => r.bay), []);
  assert.deepEqual(before.filter((r) => r.exposure !== 0).map((r) => r.bay), [6, 7, 8, 9, 10, 16, 17, 20]);
});

test('#2911 golden vectors: the run 20 m forward from each of the 24 doors, core 7d197042', async () => {
  const got = (await run('7d197042')).map((r) => r.hash);
  assert.deepEqual(got, GOLDEN);
});

const GOLDEN = ["f59248933e92cc85", "bb964da2b5d24f9a", "02ceed47a68756d0", "ee5e633082e30207", "6e5d6da4ef8e21ff", "91b1f3be82331498", "d12cfc344f86207a", "34f580de6d0afcc2", "0b5327b36614334a", "abcc2c910578ec2d", "e974dbc68264fce0", "b2f096da94c542c5", "90a2b431b3cb7b9f", "af2c7f40d4b62897", "e8c458bdb5091a38", "69f9b0c5f0bee118", "17079a3f33d91800", "32ba78c778edde16", "5c89bff362fd02c0", "063c38cf25e1ad9f", "9a9553decb13f8b8", "92c950af8a0b5df3", "d2b2555470b60a01", "95d12d57482d00f6"];
