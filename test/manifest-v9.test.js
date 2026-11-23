// SPDX-License-Identifier: LicenseRef-Bitcoin-Arena-1.0
// Copyright (c) 2026 The Bitcoin Arena authors. See LICENSE.
// NEXT BLOCK VERSION 9 (bitcoin-arena kanban #2694, follow-up of #2655 option 2): version 8 in every term - the same core 7d197042, the same
// clients - under a new game id, whose rule is that a staked run plays the liquidations its committee attests (attestation kind 4) and
// no others. The rule is the registration's `attestedLiquidations` and the Arena's known-games entry; the manifest's version and parent
// make the id a new one, so a v8 round keeps v8's rule (no liquidation in a staked run) byte for byte.
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { MANIFEST_V8, GAME_ID_V8 } from '../plugin/nextblock-v8.js';
import { GAME as V9, MANIFEST_V9, GAME_ID_V9 } from '../plugin/nextblock-v9.js';
import { KNOWN_GAME } from '../plugin/known-v9.js';
import { manifestV9Body } from '../tools/sign-manifest-v9.mjs';
import { encodeManifestBody, manifestStands } from 'arena-sdk/manifest';
import { toHex } from 'arena-sdk/hash';
import { verifyEd25519 } from 'arena-sdk/ed25519';

test('#2694 version 9 stands on version 8, signed by the same developer key, with v8\'s cores and clients', () => {
  const stands = manifestStands(MANIFEST_V9, { verify: verifyEd25519, parentOf: (id) => (toHex(id) === toHex(GAME_ID_V8) ? MANIFEST_V8 : null) });
  assert.equal(stands.ok, true, stands.why);
  assert.equal(MANIFEST_V9.version, 9);
  assert.equal(toHex(MANIFEST_V9.parentManifestId), toHex(GAME_ID_V8));
  assert.equal(toHex(MANIFEST_V9.gameDevPubkey), toHex(MANIFEST_V8.gameDevPubkey));
  assert.notEqual(toHex(GAME_ID_V9), toHex(GAME_ID_V8));
  assert.deepEqual(MANIFEST_V9.cores.map((c) => toHex(c.hash)), MANIFEST_V8.cores.map((c) => toHex(c.hash)), 'the same core');
  assert.deepEqual(MANIFEST_V9.clients.map(toHex), MANIFEST_V8.clients.map(toHex), 'the same clients');
  assert.equal(toHex(encodeManifestBody(MANIFEST_V9)), toHex(encodeManifestBody(manifestV9Body())), 'the signed bytes are the tool\'s body');
});

test('#2694 the v9 registration takes attested liquidations, v8\'s does not; app.json hosts v9; known-v9 lists its bytes', async () => {
  assert.equal(V9.attestedLiquidations, true);
  assert.equal(toHex(V9.gameId), toHex(GAME_ID_V9));
  const { GAME: V8 } = await import('../plugin/nextblock-v8.js');
  assert.ok(!V8.attestedLiquidations);
  const app = JSON.parse(readFileSync(new URL('../app.json', import.meta.url), 'utf8'));
  assert.ok(app.registrations.includes('plugin/nextblock-v9.js'));
  assert.equal(typeof KNOWN_GAME.manifestHex, 'string');
  assert.equal(KNOWN_GAME.attestedLiquidations, true);
});
