using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.ModuleManager.Native.Tests;

public partial class Tests
{
    private const string DllPath = "C:/Users/vitalii.mikhailov/Git/Level0/Bannerlord.ModuleManager/src/Bannerlord.ModuleManager.Native/bin/Release/net7.0/win-x64/native/Bannerlord.ModuleManager.Native.dll";
    
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
    private static unsafe partial return_value_void* bmm_enable_module(void* p_owner, param_json* p_modules, param_json* p_target_module, delegate*<void*, param_string*, return_value_bool*> p_get_selected, delegate* <void*, param_string*, bool, return_value_void*> p_set_selected, delegate* <void*, param_string*, return_value_bool*> p_get_disabled, delegate* <void*, param_string*, bool, return_value_void*> p_set_disabled);
    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial return_value_void* bmm_disable_module(void* p_owner, param_json* p_modules, param_json* p_target_module, delegate*<void*, param_string*, return_value_bool*> p_get_selected, delegate* <void*, param_string*, bool, return_value_void*> p_set_selected, delegate* <void*, param_string*, return_value_bool*> p_get_disabled, delegate* <void*, param_string*, bool, return_value_void*> p_set_disabled);
    
    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial return_value_json* bmm_get_module_info(param_string* p_xml_content);
    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial return_value_json* bmm_get_sub_module_info(param_string* p_xml_content);
    
    [LibraryImport(DllPath), UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe partial return_value_int32* bmm_compare_versions(param_json* p_x, param_json* p_y);

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

    [Test]
    public unsafe void Test1()
    {
        var (invalidError, invalid) = Utils2.GetResult<ModuleInfoExtended>(bmm_get_module_info((param_string*) Utils.Copy(InvalidXml)));
        Assert.That(invalidError, Is.Empty);
        var (harmonyError, harmony) = Utils2.GetResult<ModuleInfoExtended>(bmm_get_module_info((param_string*) Utils.Copy(HarmonyXml)));
        Assert.That(harmonyError, Is.Empty);
        var (uiExtenderError, uiExtenderEx) = Utils2.GetResult<ModuleInfoExtended>(bmm_get_module_info((param_string*) Utils.Copy(UIExtenderExXml)));
        Assert.That(uiExtenderError, Is.Empty);

        var unsorted = new[] { uiExtenderEx, harmony };
        var unsortedInvalid = new[] { invalid, uiExtenderEx, harmony };

        var (sortedError, sorted) = Utils2.GetResult<ModuleInfoExtended[]>(bmm_sort(Utils2.ToJson(unsorted)));
        Assert.That(sortedError, Is.Empty);
        
        var (sorted2Error, sorted2) = Utils2.GetResult<ModuleInfoExtended[]>(bmm_sort_with_options(Utils2.ToJson(unsorted), Utils2.ToJson(new ModuleSorterOptions { SkipOptionals = true, SkipExternalDependencies = true })));
        Assert.That(sorted2Error, Is.Empty);
        var (validationResultError, validationResult) = Utils2.GetResult<ModuleIssue[]>(bmm_validate_load_order(Utils2.ToJson(sorted), Utils2.ToJson(harmony)));
        Assert.That(validationResultError, Is.Empty);

        var validationManager = new ValidationManager();
        var (validationResult1Error, validationResult1) = Utils2.GetResult<ModuleIssue[]>(bmm_validate_module(
            Unsafe.AsPointer(ref validationManager), Utils2.ToJson(unsortedInvalid), Utils2.ToJson(uiExtenderEx), &ValidationManager.IsSelected));
        Assert.That(validationResult1Error, Is.Empty);

        var (validationResult2Error, validationResult2) = Utils2.GetResult<ModuleIssue[]>(bmm_validate_module(
            Unsafe.AsPointer(ref validationManager), Utils2.ToJson(unsortedInvalid), Utils2.ToJson(invalid), &ValidationManager.IsSelected));
        Assert.That(validationResult2Error, Is.Empty);

        var enableDisableManager = new EnableDisableManager();
        var (enableResultError, enableResult) = Utils2.GetResult(bmm_enable_module(
            Unsafe.AsPointer(ref enableDisableManager), Utils2.ToJson(unsorted), Utils2.ToJson(uiExtenderEx),
            &EnableDisableManager.GetSelected, &EnableDisableManager.SetSelected, &EnableDisableManager.GetDisabled, &EnableDisableManager.SetDisabled));
        Assert.That(enableResultError, Is.Empty);
        
        var (disableResultError, disableResult) = Utils2.GetResult(bmm_disable_module(
            Unsafe.AsPointer(ref enableDisableManager), Utils2.ToJson(unsorted), Utils2.ToJson(uiExtenderEx),
            &EnableDisableManager.GetSelected, &EnableDisableManager.SetSelected, &EnableDisableManager.GetDisabled, &EnableDisableManager.SetDisabled));
        Assert.That(disableResultError, Is.Empty);
    }
}