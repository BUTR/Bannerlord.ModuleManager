import test from 'ava';

import { harmonyXml, uiExtenderExXml } from './_data';
import { BannerlordModuleManager } from '../lib/BannerlordModuleManager';

test('BannerlordModuleManager', async (t) => {
  const blmmanager = await BannerlordModuleManager.createAsync();

  const uiExtenderEx = await blmmanager.getModuleInfo(uiExtenderExXml);
  if (uiExtenderEx === null) {
    t.fail();
    return;
  }
  const harmony = await blmmanager.getModuleInfo(harmonyXml);
  if (harmony === null) {
    t.fail();
    return;
  }

  const unsorted = [uiExtenderEx, harmony];

  const sorted = blmmanager.sort(unsorted);
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