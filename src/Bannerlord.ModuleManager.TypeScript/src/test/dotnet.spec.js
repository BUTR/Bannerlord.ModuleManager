const test = require("ava");

const { BannerlordModuleManager, boot, terminate } = require("../lib/dotnet");

test.beforeEach("init", async (t) => {
  await boot();
});

test.afterEach.always("cleanup", (t) => {
  terminate();
});

test("api available", async (t) => {
  if ("Sort" in BannerlordModuleManager === false) {
    t.fail();
  }

  if ("SortWithOptions" in BannerlordModuleManager === false) {
    t.fail();
  }

  if ("AreAllDependenciesOfModulePresent" in BannerlordModuleManager === false) {
    t.fail();
  }

  if ("GetDependentModulesOf" in BannerlordModuleManager === false) {
    t.fail();
  }

  if ("GetDependentModulesOfWithOptions" in BannerlordModuleManager === false) {
    t.fail();
  }

  if ("GetModuleInfo" in BannerlordModuleManager === false) {
    t.fail();
  }

  if ("GetSubModuleInfo" in BannerlordModuleManager === false) {
    t.fail();
  }

  t.pass();
});
