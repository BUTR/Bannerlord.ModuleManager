using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using static Bannerlord.ModuleManager.Native.Tests.Utils2;

namespace Bannerlord.ModuleManager.Native.Tests
{
    public partial class Tests
    {
        private const string DllPath = "../../../../../src/Bannerlord.ModuleManager.Native/bin/Release/net7.0/win-x64/native/Bannerlord.ModuleManager.Native.dll";


        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_sort(param_json* p_source);
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_sort_with_options(param_json* p_source, param_json* p_options);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_bool* bmm_are_all_dependencies_of_module_present(param_json* p_source, param_json* p_module);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_get_dependent_modules_of(param_json* p_source, param_json* p_module);
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_get_dependent_modules_of_with_options(param_json* p_source, param_json* p_module, param_json* p_options);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_validate_load_order(param_json* p_source, param_json* p_target_module);
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_validate_module(void* p_owner, param_json* p_modules, param_json* p_target_module, delegate*<void*, param_string*, return_value_bool*> p_is_selected);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_void* bmm_enable_module(void* p_owner, param_json* p_modules, param_json* p_target_module, delegate*<void*, param_string*, return_value_bool*> p_get_selected, delegate*<void*, param_string*, byte, return_value_void*> p_set_selected, delegate*<void*, param_string*, return_value_bool*> p_get_disabled, delegate*<void*, param_string*, byte, return_value_void*> p_set_disabled);
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_void* bmm_disable_module(void* p_owner, param_json* p_modules, param_json* p_target_module, delegate*<void*, param_string*, return_value_bool*> p_get_selected, delegate*<void*, param_string*, byte, return_value_void*> p_set_selected, delegate*<void*, param_string*, return_value_bool*> p_get_disabled, delegate*<void*, param_string*, byte, return_value_void*> p_set_disabled);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_get_module_info(param_string* p_xml_content);
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_get_sub_module_info(param_string* p_xml_content);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_int32* bmm_compare_versions(param_json* p_x, param_json* p_y);

        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_get_dependencies_all(param_json* p_module);
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_get_dependencies_load_before_this(param_json* p_module);
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_get_dependencies_load_after_this(param_json* p_module);
        [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        private static unsafe partial return_value_json* bmm_get_dependencies_incompatibles(param_json* p_module);

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

        private const string HarmonySubModuleXml = """
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
""";

        [Test]
        public unsafe void Test_ApplicationVersion()
        {
            Assert.DoesNotThrow(() =>
            {
                var version1 = new ApplicationVersion(ApplicationVersionType.Alpha, 0, 0, 0, 0);
                var version2 = new ApplicationVersion(ApplicationVersionType.Release, 1, 0, 0, 0);

                var result = GetResult(bmm_compare_versions(ToJson(version1), ToJson(version2)));
                Assert.That(result, Is.EqualTo(-1));
            });

            Assert.That(DanglingAllocationsCount, Is.EqualTo(0));
        }

        [Test]
        public unsafe void Test_SubModule()
        {
            Assert.DoesNotThrow(() =>
            {
                using var xml = Copy(HarmonySubModuleXml);
                var subModule = GetResult<ModuleInfoExtended>(bmm_get_sub_module_info((param_string*) xml.DangerousGetHandle()));
            });

            Assert.That(DanglingAllocationsCount, Is.EqualTo(0));
        }

        [Test]
        public unsafe void Test_Main()
        {
            Assert.DoesNotThrow(() =>
            {
                using var invalidXml = Copy(InvalidXml);
                var invalid = GetResult<ModuleInfoExtended>(bmm_get_module_info((param_string*) invalidXml.DangerousGetHandle()));
                Assert.That(invalid, Is.Not.Null);
                using var harmonyXml = Copy(HarmonyXml);
                var harmony = GetResult<ModuleInfoExtended>(bmm_get_module_info((param_string*) harmonyXml.DangerousGetHandle()));
                Assert.Multiple(() =>
                {
                    Assert.That(harmony, Is.Not.Null);
                    Assert.That(harmony!.Id, Is.EqualTo("Bannerlord.Harmony"));
                });
                using var uiExtenderExXml = Copy(UIExtenderExXml);
                var uiExtenderEx = GetResult<ModuleInfoExtended>(bmm_get_module_info((param_string*) uiExtenderExXml.DangerousGetHandle()));
                Assert.Multiple(() =>
                {
                    Assert.That(uiExtenderEx, Is.Not.Null);
                    Assert.That(uiExtenderEx!.Id, Is.EqualTo("Bannerlord.UIExtenderEx"));
                });

                var unsorted = new[] { uiExtenderEx, harmony };
                var unsortedInvalid = new[] { invalid, uiExtenderEx, harmony };

                var areDepsPresent = GetResult(bmm_are_all_dependencies_of_module_present(ToJson(unsorted), ToJson(uiExtenderEx)));
                Assert.That(areDepsPresent, Is.True);

                var dependencies = GetResult<ModuleInfoExtended[]>(bmm_get_dependent_modules_of(ToJson(unsorted), ToJson(uiExtenderEx)));
                Assert.Multiple(() =>
                {
                    Assert.That(dependencies, Has.Length.EqualTo(1));
                    Assert.That(dependencies![0].Id, Is.EqualTo(harmony!.Id), () => string.Join(", ", dependencies.Select(x => x.Id)));
                });
                var dependencies2 = GetResult<ModuleInfoExtended[]>(bmm_get_dependent_modules_of_with_options(ToJson(unsorted), ToJson(uiExtenderEx), ToJson(new ModuleSorterOptions(true, true))));
                Assert.Multiple(() =>
                {
                    Assert.That(dependencies2, Has.Length.EqualTo(1));
                    Assert.That(dependencies2![0].Id, Is.EqualTo(harmony!.Id), () => string.Join(", ", dependencies2.Select(x => x.Id)));
                });

                var sorted = GetResult<ModuleInfoExtended[]>(bmm_sort(ToJson(unsorted)));
                Assert.Multiple(() =>
                {
                    Assert.That(sorted, Has.Length.EqualTo(2), () => string.Join(", ", sorted?.Select(x => x.Id) ?? Enumerable.Empty<string>()));
                    Assert.That(sorted![0].Id, Is.EqualTo(harmony!.Id));
                    Assert.That(sorted![1].Id, Is.EqualTo(uiExtenderEx!.Id));
                });
                var sorted2 = GetResult<ModuleInfoExtended[]>(bmm_sort_with_options(ToJson(unsorted), ToJson(new ModuleSorterOptions { SkipOptionals = true, SkipExternalDependencies = true })));
                Assert.Multiple(() =>
                {
                    Assert.That(sorted2, Has.Length.EqualTo(2), () => string.Join(", ", sorted2?.Select(x => x.Id) ?? Enumerable.Empty<string>()));
                    Assert.That(sorted2![0].Id, Is.EqualTo(harmony!.Id));
                    Assert.That(sorted2![1].Id, Is.EqualTo(uiExtenderEx!.Id));
                });

                var validationResult = GetResult<ModuleIssue[]>(bmm_validate_load_order(ToJson(sorted), ToJson(harmony)));
                Assert.That(validationResult, Has.Length.EqualTo(0), () => string.Join(", ", validationResult?.Select(x => x.Reason) ?? Enumerable.Empty<string>()));

                var validationManager = new ValidationManager();
                var validationResult1 = GetResult<ModuleIssue[]>(bmm_validate_module(
                    Unsafe.AsPointer(ref validationManager), ToJson(unsortedInvalid), ToJson(uiExtenderEx), &ValidationManager.IsSelected));
                Assert.That(validationResult1, Has.Length.EqualTo(0), () => string.Join(", ", validationResult1?.Select(x => x.Reason) ?? Enumerable.Empty<string>()));

                var validationResult2 = GetResult<ModuleIssue[]>(bmm_validate_module(
                    Unsafe.AsPointer(ref validationManager), ToJson(unsortedInvalid), ToJson(invalid), &ValidationManager.IsSelected));
                Assert.That(validationResult2, Has.Length.EqualTo(1), () => string.Join(", ", validationResult2?.Select(x => x.Reason) ?? Enumerable.Empty<string>()));

                var enableDisableManager = new EnableDisableManager();
                GetResult(bmm_enable_module(
                    Unsafe.AsPointer(ref enableDisableManager), ToJson(unsorted), ToJson(uiExtenderEx),
                    &EnableDisableManager.GetSelected, &EnableDisableManager.SetSelected, &EnableDisableManager.GetDisabled, &EnableDisableManager.SetDisabled));

                GetResult(bmm_disable_module(
                    Unsafe.AsPointer(ref enableDisableManager), ToJson(unsorted), ToJson(uiExtenderEx),
                    &EnableDisableManager.GetSelected, &EnableDisableManager.SetSelected, &EnableDisableManager.GetDisabled, &EnableDisableManager.SetDisabled));

                var dependenciesAll = GetResult<DependentModuleMetadata[]>(bmm_get_dependencies_all(ToJson(uiExtenderEx)));
                Assert.That(dependenciesAll!, Has.Length.EqualTo(6));
                var dependenciesLoadBefore = GetResult<DependentModuleMetadata[]>(bmm_get_dependencies_load_before_this(ToJson(uiExtenderEx)));
                Assert.That(dependenciesLoadBefore!, Has.Length.EqualTo(1));
                var dependenciesLoadAfter = GetResult<DependentModuleMetadata[]>(bmm_get_dependencies_load_after_this(ToJson(uiExtenderEx)));
                Assert.That(dependenciesLoadAfter!, Has.Length.EqualTo(5));
                var dependenciesIncompatibles = GetResult<DependentModuleMetadata[]>(bmm_get_dependencies_incompatibles(ToJson(uiExtenderEx)));
                Assert.That(dependenciesIncompatibles!, Has.Length.EqualTo(0));
            });

            Assert.That(DanglingAllocationsCount, Is.EqualTo(0));
        }
    }
}