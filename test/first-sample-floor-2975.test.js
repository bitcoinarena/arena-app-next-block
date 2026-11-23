// The first live sample moves the seeded tape by one offset; a first price far below the seed must not drive any
// tape price to zero or below (bitcoin-arena kanban #2975, feedback #329).
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { init, submitSample } from '../src/sim.js';

test('#2975 a first price ~99 % below the seed leaves every tape price positive', () => {
  const st = init(7);
  const lo = Math.min(...st.tape);
  assert.ok(lo > 1000, 'the seed is far above a cent');
  submitSample(st, 100);   // $1.00, far below the seed's level
  assert.ok(Math.min(...st.tape) > 0, `a tape price went to ${Math.min(...st.tape)}`);
  if (st.roundTape) assert.ok(Math.min(...st.roundTape) > 0);
  assert.equal(st.tape[st.tape.length - 1], 100, 'the sample itself is the last price');
});
