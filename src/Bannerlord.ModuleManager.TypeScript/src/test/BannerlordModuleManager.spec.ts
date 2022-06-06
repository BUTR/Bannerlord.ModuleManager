import test from 'ava';

import { harmonyXml, uiExtenderExXml } from './_data';
import { BannerlordModuleManager } from '../lib/BannerlordModuleManager';

test('sort', async (t) => {
  const blmmanager = await BannerlordModuleManager.createAsync();

  const uiExtenderEx = blmmanager.getModuleInfo(uiExtenderExXml);
  if (uiExtenderEx === null || uiExtenderEx === undefined) {
    t.fail();
    return;
  }
  const harmony = blmmanager.getModuleInfo(harmonyXml);
  if (harmony === null || harmony === undefined) {
    t.fail();
    return;
  }

  const unsorted = [uiExtenderEx, harmony];

  const sorted = blmmanager.sort(unsorted);
  if (sorted === null || !Array.isArray(sorted)) {
    t.fail();
    return;
  }

  const sorted2 = blmmanager.sortWithOptions(unsorted, { skipOptionals: true, skipExternalDependencies: true });
  if (sorted2 === null || !Array.isArray(sorted2)) {
    t.fail();
    return;
  }

  t.deepEqual(uiExtenderEx.id, 'Bannerlord.UIExtenderEx');
  t.deepEqual(harmony.id, 'Bannerlord.Harmony');
  t.deepEqual(sorted.length, 2);
  t.deepEqual(sorted[0].id, harmony.id);
  t.deepEqual(sorted[1].id, uiExtenderEx.id);

  await blmmanager.dispose();
});