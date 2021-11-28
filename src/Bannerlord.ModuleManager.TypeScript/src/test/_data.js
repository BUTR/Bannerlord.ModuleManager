const uiExtenderExXml = `<?xml version="1.0" encoding="UTF-8"?>
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
  <!-- Prototype -->
  <DependedModuleMetadatas>
    <DependedModuleMetadata id="Bannerlord.Harmony" order="LoadBeforeThis" version="v1.0.0" />

    <DependedModuleMetadata id="Native" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
    <DependedModuleMetadata id="SandBoxCore" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
    <DependedModuleMetadata id="Sandbox" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
    <DependedModuleMetadata id="StoryMode" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
    <DependedModuleMetadata id="CustomBattle" order="LoadAfterThis" version="e1.5.1.*" optional="true" />
  </DependedModuleMetadatas>
  <!-- Prototype -->
  <SubModules>
    <SubModule>
      <Name value="UIExtenderEx" />
      <DLLName value="Bannerlord.UIExtenderEx.dll" />
      <SubModuleClassType value="Bannerlord.UIExtenderEx.SubModule" />
      <Tags />
    </SubModule>
  </SubModules>
</Module>`;

const harmonyXml = `<?xml version="1.0" encoding="UTF-8"?>
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
  <!-- Prototype -->
  <DependedModuleMetadatas>
    <DependedModuleMetadata id="Native" order="LoadAfterThis" optional="true" />
    <DependedModuleMetadata id="SandBoxCore" order="LoadAfterThis" optional="true" />
    <DependedModuleMetadata id="Sandbox" order="LoadAfterThis" optional="true" />
    <DependedModuleMetadata id="StoryMode" order="LoadAfterThis" optional="true" />
    <DependedModuleMetadata id="CustomBattle" order="LoadAfterThis" optional="true" />
  </DependedModuleMetadatas>
  <!-- Prototype -->
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
</Module>`;

module.exports = { harmonyXml, uiExtenderExXml }