import test from 'ava';

import { harmonyXml, uiExtenderExXml, invalidXml, harmonySubModuleXml } from './_data';
import { BannerlordModuleManager, IEnableDisableManager, IValidationManager } from '../lib/BannerlordModuleManager';
import { Bannerlord } from '../lib/dotnet';
import ApplicationVersion = Bannerlord.ModuleManager.ApplicationVersion;
import ApplicationVersionType = Bannerlord.ModuleManager.ApplicationVersionType;

let blmmanager: BannerlordModuleManager;
test.before("Init WASM", async () => {
  blmmanager = await BannerlordModuleManager.createAsync();
});
test.after("Terminate WASM", async () => {
  blmmanager.dispose();
});

test('ApplicationVersion', async (t) => {
  const version1: ApplicationVersion = {
    applicationVersionType: ApplicationVersionType.Alpha,
    major: 0,
    minor: 0,
    revision: 0,
    changeSet: 0
  }
  const version2: ApplicationVersion = {
    applicationVersionType: ApplicationVersionType.Release,
    major: 1,
    minor: 0,
    revision: 0,
    changeSet: 0
  }

  const result = blmmanager.compareVersions(version1, version2);
  t.is(result, -1);

  t.pass();
});

test('SubModule', async (t) => {
  const harmonySubModule = blmmanager.getSubModuleInfo(harmonySubModuleXml);
  if (harmonySubModule === null || harmonySubModule === undefined) {
    t.fail();
    return;
  }

  t.pass();
});

test('Main', async (t) => {
  const invalid = blmmanager.getModuleInfo(invalidXml);
  if (invalid === null || invalid === undefined) {
    t.fail();
    return;
  }

  const harmony = blmmanager.getModuleInfo(harmonyXml);
  if (harmony === null || harmony === undefined) {
    t.fail();
    return;
  }
  t.deepEqual(harmony.id, 'Bannerlord.Harmony');

  const uiExtenderEx = blmmanager.getModuleInfo(uiExtenderExXml);
  if (uiExtenderEx === null || uiExtenderEx === undefined) {
    t.fail();
    return;
  }
  t.deepEqual(uiExtenderEx.id, 'Bannerlord.UIExtenderEx');

  const unsorted = [uiExtenderEx, harmony];
  const unsortedInvalid = [invalid, uiExtenderEx, harmony];

  const areUIExtenderExDependenciesPresent = blmmanager.areAllDependenciesOfModulePresent(unsorted, uiExtenderEx);
  t.is(areUIExtenderExDependenciesPresent, true);

  const uiExtenderExDependencies = blmmanager.getDependentModulesOf(unsorted, uiExtenderEx);
  if (uiExtenderExDependencies === null || !Array.isArray(uiExtenderExDependencies)) {
    t.fail();
    return;
  }
  t.deepEqual(uiExtenderExDependencies.length, 1);
  t.deepEqual(uiExtenderExDependencies[0].id, harmony.id);

  const uiExtenderExDependencies2 = blmmanager.getDependentModulesOfWithOptions(unsorted, uiExtenderEx, { skipOptionals: true, skipExternalDependencies: true });
  if (uiExtenderExDependencies2 === null || !Array.isArray(uiExtenderExDependencies2)) {
    t.fail();
    return;
  }
  t.deepEqual(uiExtenderExDependencies2.length, 1);
  t.deepEqual(uiExtenderExDependencies2[0].id, harmony.id);

  const sorted = blmmanager.sort(unsorted);
  if (sorted === null || !Array.isArray(sorted)) {
    t.fail();
    return;
  }
  t.deepEqual(sorted.length, 2);
  t.deepEqual(sorted[0].id, harmony.id);
  t.deepEqual(sorted[1].id, uiExtenderEx.id);

  const sorted2 = blmmanager.sortWithOptions(unsorted, { skipOptionals: true, skipExternalDependencies: true });
  if (sorted2 === null || !Array.isArray(sorted2)) {
    t.fail();
    return;
  }
  t.deepEqual(sorted2.length, 2);
  t.deepEqual(sorted2[0].id, harmony.id);
  t.deepEqual(sorted2[1].id, uiExtenderEx.id);

  const validationResult = blmmanager.validateLoadOrder(sorted, harmony);
  if (validationResult === null || !Array.isArray(validationResult)) {
    t.fail();
    return;
  }
  t.deepEqual(validationResult.length, 0);

  let isSelectedCalled = false;
  const validationManager: IValidationManager = {
    isSelected: function (moduleId: string): boolean {
      isSelectedCalled = true;
      if (moduleId == "") { return true; }
      return false;
    }
  };
  const validationResult1 = blmmanager.validateModule(unsortedInvalid, uiExtenderEx, validationManager);
  if (validationResult1 === null || !Array.isArray(validationResult1)) {
    t.fail();
    return;
  }
  t.deepEqual(validationResult1.length, 0);
  if (!isSelectedCalled) {
    t.fail();
    return;
  }

  const validationResult2 = blmmanager.validateModule(unsortedInvalid, invalid, validationManager);
  if (validationResult2 === null || !Array.isArray(validationResult2) || validationResult2.length != 1) {
    t.fail();
    return;
  }
  t.deepEqual(validationResult2.length, 1);

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
  blmmanager.enableModule(unsorted, uiExtenderEx, enableDisableManager);
  blmmanager.disableModule(unsorted, uiExtenderEx, enableDisableManager);
  if (!getSelectedCalled || !setSelectedCalled/* || !getDisabledCalled || !setDisabledCalled*/) {
    t.fail();
    return;
  }

  const dependenciesAll = blmmanager.getDependenciesAll(uiExtenderEx);
  t.deepEqual(dependenciesAll.length, 6);
  const dependenciesLoadBefore = blmmanager.getDependenciesToLoadBeforeThis(uiExtenderEx);
  t.deepEqual(dependenciesLoadBefore.length, 1);
  const dependenciesLoadAfter = blmmanager.getDependenciesToLoadAfterThis(uiExtenderEx);
  t.deepEqual(dependenciesLoadAfter.length, 5);
  const dependenciesIncompatibles = blmmanager.getDependenciesIncompatibles(uiExtenderEx);
  t.deepEqual(dependenciesIncompatibles.length, 0);

  t.pass();
});