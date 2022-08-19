const test = require("ava");

const { Bannerlord, boot, terminate } = require("../lib/dotnet");

test.beforeEach("init", async (t) => {
  await boot();
});

test.afterEach.always("cleanup", (t) => {
  terminate();
});

test("api available", async (t) => {
  if ("Sort" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("SortWithOptions" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("AreAllDependenciesOfModulePresent" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("GetDependentModulesOf" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("GetDependentModulesOfWithOptions" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("ValidateModule" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("EnableModule" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("DisableModule" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("GetModuleInfo" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("GetSubModuleInfo" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  if ("CompareVersions" in Bannerlord.ModuleManager === false) {
    t.fail();
  }

  t.pass();
});
