import test from 'ava';

import { harmonyXml, uiExtenderExXml } from './_data';
import { LibraryWasm } from '../lib/librarywasm';

test('LibraryWasm', async (t) => {
  const blmodulemanager = await LibraryWasm.createAsync();

  const uiExtenderEx = blmodulemanager.getModuleInfo(uiExtenderExXml);
  if (uiExtenderEx === null) {
    t.fail();
    return;
  }
  const harmony = blmodulemanager.getModuleInfo(harmonyXml);
  if (harmony === null) {
    t.fail();
    return;
  }

  const unsorted = [uiExtenderEx, harmony];

  const sorted = blmodulemanager.sort(unsorted);
  if (sorted === null || !Array.isArray(sorted)) {
    t.fail();
    return;
  }

  t.deepEqual(uiExtenderEx.id, 'Bannerlord.UIExtenderEx');
  t.deepEqual(harmony.id, 'Bannerlord.Harmony');
  t.deepEqual(sorted.length, 2);
  t.deepEqual(sorted[0].id, harmony.id);
  t.deepEqual (sorted[1].id, uiExtenderEx.id);
});