import test from 'ava';

import { harmonyXml, uiExtenderExXml, invalidXml } from './_data';
import { IEnableDisableManager, IValidationManager } from '../lib/types';
import { BannerlordModuleManager } from '../lib/BannerlordModuleManager';

test('sort', async (t) => {
  const invalid = BannerlordModuleManager.getModuleInfo(invalidXml);
  if (invalid === null || invalid === undefined) {
    t.fail();
    return;
  }
  const harmony = BannerlordModuleManager.getModuleInfo(harmonyXml);
  if (harmony === null || harmony === undefined) {
    t.fail();
    return;
  }
  const uiExtenderEx = BannerlordModuleManager.getModuleInfo(uiExtenderExXml);
  if (uiExtenderEx === null || uiExtenderEx === undefined) {
    t.fail();
    return;
  }

  const unsorted = [uiExtenderEx, harmony];
  const unsortedInvalid = [invalid, uiExtenderEx, harmony];

  const sorted = BannerlordModuleManager.sort(unsorted);
  if (sorted === null || !Array.isArray(sorted)) {
    t.fail();
    return;
  }

  const sorted2 = BannerlordModuleManager.sortWithOptions(unsorted, { skipOptionals: true, skipExternalDependencies: true });
  if (sorted2 === null || !Array.isArray(sorted2)) {
    t.fail();
    return;
  }

  const validationResult = BannerlordModuleManager.validateLoadOrder(sorted, harmony);
  if (validationResult === null || !Array.isArray(validationResult)) {
    t.fail();
    return;
  }

  let isSelectedCalled = false;
  const validationManager: IValidationManager = {
    isSelected: function (moduleId: string): boolean {
      isSelectedCalled = true;
      if (moduleId == "") { return true; }
      return false;
    }
  };
  const validationResult1 = BannerlordModuleManager.validateModule(unsortedInvalid, uiExtenderEx, validationManager);
  if (validationResult1 === null || !Array.isArray(validationResult1)) {
    t.fail();
    return;
  }
  if (!isSelectedCalled) {
    t.fail();
    return;
  }

  const validationResult2 = BannerlordModuleManager.validateModule(unsortedInvalid, invalid, validationManager);
  if (validationResult2 === null || !Array.isArray(validationResult2) || validationResult2.length != 1) {
    t.fail();
    return;
  }

  let getSelectedCalled = false;
  let setSelectedCalled = false;
  /*let getDisabledCalled = false;
  let setDisabledCalled = false;*/
  const enableDisableManager: IEnableDisableManager = {
    getSelected: function (moduleId: string): boolean {
      getSelectedCalled = true;
      if (moduleId == "") { return true; }
      return false;
    },
    setSelected: function (moduleId: string, value: boolean): void {
      setSelectedCalled = true;
      if (moduleId == "" && value) { return; }
    },
    getDisabled: function (moduleId: string): boolean {
      /*getDisabledCalled = true;*/
      if (moduleId == "") { return true; }
      return false;
    },
    setDisabled: function (moduleId: string, value: boolean): void {
      /*setDisabledCalled = true;*/
      if (moduleId == "" && value) { return; }
    },
  };
  BannerlordModuleManager.enableModule(unsorted, uiExtenderEx, enableDisableManager);
  BannerlordModuleManager.disableModule(unsorted, uiExtenderEx, enableDisableManager);
  if (!getSelectedCalled || !setSelectedCalled/* || !getDisabledCalled || !setDisabledCalled*/) {
    t.fail();
    return;
  }

  t.deepEqual(uiExtenderEx.id, 'Bannerlord.UIExtenderEx');
  t.deepEqual(harmony.id, 'Bannerlord.Harmony');
  t.deepEqual(sorted.length, 2);
  t.deepEqual(sorted[0].id, harmony.id);
  t.deepEqual(sorted[1].id, uiExtenderEx.id);
  
  t.pass();
});