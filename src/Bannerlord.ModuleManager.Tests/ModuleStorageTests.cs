using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Bannerlord.ModuleManager.Tests
{
    public class ModuleStorageTests
    {
        private const string InvalidXml = """
<?xml version="1.0" encoding="UTF-8"?>
<Module>
  <Name value="Invalid" />
  <Id value="Invalid" />
  <Version value="v1.0.0" />
  <Official value="false" />
  <DefaultModule value="false" />
  <SingleplayerModule value="true" />
  <MultiplayerModule value="false" />
  <DependedModules>
    <DependedModule Id="Bannerlord.Harmony" DependentVersion="v1.0.0" />
  </DependedModules>
  <DependedModuleMetadatas>
    <DependedModuleMetadata id="Bannerlord.Harmony" order="LoadAfterThis" version="v1.0.0" />
  </DependedModuleMetadatas>
  <SubModules>
    <SubModule>
      <Name value="Invalid" />
      <DLLName value="Invalid.dll" />
      <SubModuleClassType value="Invalid.SubModule" />
      <Tags />
    </SubModule>
  </SubModules>
</Module>
""";

        private const string UIExtenderExXml = """
<?xml version="1.0" encoding="UTF-8"?>
<Module>
  <Name value="UIExtenderEx" />
  <Id value="Bannerlord.UIExtenderEx" />
  <Version value="v1.0.0" />
  <Official value="false" />
  <DefaultModule value="false" />
  <SingleplayerModule value="true" />
  <MultiplayerModule value="false" />
  <Url value="https://www.nexusmods.com/mountandblade2bannerlord/mods/2102" />
  <DependedModules>
    <DependedModule Id="Bannerlord.Harmony" DependentVersion="v1.0.0" />
  </DependedModules>
  <DependedModuleMetadatas>
    <DependedModuleMetadata id="Bannerlord.Harmony" order="LoadBeforeThis" version="v1.0.0" />

    <DependedModuleMetadata id="Native" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
    <DependedModuleMetadata id="SandBoxCore" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
    <DependedModuleMetadata id="Sandbox" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
    <DependedModuleMetadata id="StoryMode" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
    <DependedModuleMetadata id="CustomBattle" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
  </DependedModuleMetadatas>
  <SubModules>
    <SubModule>
      <Name value="UIExtenderEx" />
      <DLLName value="Bannerlord.UIExtenderEx.dll" />
      <SubModuleClassType value="Bannerlord.UIExtenderEx.SubModule" />
      <Tags />
    </SubModule>
  </SubModules>
</Module>
""";

        private const string HarmonyXml = """
<?xml version="1.0" encoding="UTF-8"?>
<Module>
  <Name value="Harmony" />
  <Id value="Bannerlord.Harmony" />
  <Version value="v1.0.0" />
  <Official value="false" />
  <DefaultModule value="false" />
  <SingleplayerModule value="true" />
  <MultiplayerModule value="false" />
  <Url value="https://www.nexusmods.com/mountandblade2bannerlord/mods/2006" />
  <DependedModules />
  <DependedModuleMetadatas>
    <DependedModuleMetadata id="Native" order="LoadAfterThis" optional="true" />
    <DependedModuleMetadata id="SandBoxCore" order="LoadAfterThis" optional="true" />
    <DependedModuleMetadata id="Sandbox" order="LoadAfterThis" optional="true" />
    <DependedModuleMetadata id="StoryMode" order="LoadAfterThis" optional="true" />
    <DependedModuleMetadata id="CustomBattle" order="LoadAfterThis" optional="true" />
  </DependedModuleMetadatas>
  <SubModules>
    <SubModule>
      <Name value="Harmony" />
      <DLLName value="Bannerlord.Harmony.dll" />
      <SubModuleClassType value="Bannerlord.Harmony.SubModule" />
      <!-- For compatibility with Legacy versions-->
      <Tags>
        <Tag key="DedicatedServerType" value="none" />
        <Tag key="IsNoRenderModeElement" value="false" />
      </Tags>
    </SubModule>
  </SubModules>
</Module>
""";

        public static ModuleInfoExtended? GetModuleInfo(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return ModuleInfoExtended.FromXml(doc);
        }

        [Test]
        public void Test_Main()
        {
            var invalid = GetModuleInfo(InvalidXml);
            Assert.That(invalid, Is.Not.Null);
            var harmony = GetModuleInfo(HarmonyXml);
            Assert.That(harmony, Is.Not.Null);
            Assert.That(harmony.Id, Is.EqualTo("Bannerlord.Harmony"));
            var uiExtenderEx = GetModuleInfo(UIExtenderExXml);
            Assert.That(uiExtenderEx, Is.Not.Null);
            Assert.That(uiExtenderEx.Id, Is.EqualTo("Bannerlord.UIExtenderEx"));

            var unsorted = new[] { uiExtenderEx, harmony };
            var unsortedInvalid = new[] { invalid, uiExtenderEx, harmony };

            var areDepsPresent = ModuleUtilities.AreDependenciesPresent(unsorted, uiExtenderEx);
            Assert.That(areDepsPresent, Is.True);

            var dependencies = ModuleUtilities.GetDependencies(unsorted, uiExtenderEx).ToArray();
            Assert.That(dependencies!.Length, Is.EqualTo(1));
            Assert.That(dependencies[0].Id, Is.EqualTo(harmony.Id));

            var dependencies2 = ModuleUtilities.GetDependencies(unsorted, uiExtenderEx, new ModuleSorterOptions(true, true)).ToArray();
            Assert.That(dependencies2!.Length, Is.EqualTo(1));
            Assert.That(dependencies2[0].Id, Is.EqualTo(harmony.Id));

            var sorted = ModuleSorter.Sort(unsorted);
            Assert.That(sorted.Count, Is.EqualTo(2));

            var sorted2 = ModuleSorter.Sort(unsorted, new ModuleSorterOptions { SkipOptionals = true, SkipExternalDependencies = true });
            Assert.That(sorted2, Is.EqualTo(2));

            var validationResult = ModuleUtilities.ValidateLoadOrder(sorted.AsReadOnly(), harmony);
            Assert.That(validationResult, Is.Empty);

            var validationManager = new ValidationManager();
            var validationResult1 = ModuleUtilities.ValidateModule(unsortedInvalid, uiExtenderEx, validationManager.IsSelected).ToArray();
            Assert.That(validationResult1.Length, Is.Empty);

            var validationResult2 = ModuleUtilities.ValidateModule(unsortedInvalid, invalid, validationManager.IsSelected).ToArray();
            Assert.That(validationResult2.Length, Is.Empty);

            var enableDisableManager = new EnableDisableManager();
            ModuleUtilities.EnableModule(unsorted, uiExtenderEx, enableDisableManager.GetSelected, enableDisableManager.SetSelected, enableDisableManager.GetDisabled, enableDisableManager.SetDisabled);

            ModuleUtilities.DisableModule(unsorted, uiExtenderEx, enableDisableManager.GetSelected, enableDisableManager.SetSelected, enableDisableManager.GetDisabled, enableDisableManager.SetDisabled);
        }

        [Test]
        public void Test([Values] ModuleListTemplates templateEnum)
        {
            var template = new ModuleStorage(templateEnum);
            var sorted = ModuleSorter.Sort(template.ModuleInfoExtendeds);
            Assert.AreEqual(template.ExpectedIdOrder, sorted.Select(x => x.Id));
        }

        [Test]
        public void Test_GetDependencies()
        {
            var dependencies = ModuleUtilities.GetDependencies(new[]
            {
                ModuleTemplates.NativeModuleBase,
                ModuleTemplates.CustomModuleWithNativeDM
            }, ModuleTemplates.CustomModuleWithNativeDM);
            Assert.AreEqual(new[] { ModuleTemplates.NativeModuleBase }, dependencies);
        }
    }
}