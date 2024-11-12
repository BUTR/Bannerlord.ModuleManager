#region License
// MIT License
//
// Copyright (c) Bannerlord's Unofficial Tools & Resources
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#nullable enable
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning disable
#endif

// ReSharper disable MemberCanBePrivate.Global

namespace Bannerlord.ModuleManager.Models.Issues;

using LegacyModuleIssue = ModuleIssue;

/// <summary>
///     Base record type for all module-related issues that can occur during validation.
///     This record serves as the abstract base class for all specific issue variants,
///     providing a common structure and conversion capability to legacy formats.
/// </summary>
/// <param name="Module">The module in which the issue was detected. This is always required.</param>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    abstract record ModuleIssueV2(ModuleInfoExtended Module)
{
    /// <summary>
    ///     Converts this issue instance to the legacy ModuleIssue format for backwards compatibility
    /// </summary>
    /// <returns>A <see cref="LegacyModuleIssue" /> representation of this issue</returns>
    public abstract LegacyModuleIssue ToLegacy();
}

/// <summary>
/// Represents an issue where a required module is missing from the module list.
/// This indicates some sort of error with the API usage, you called a method with a
/// module list, but the module you provided in another parameter was not in that list.
/// </summary>
/// <param name="Module">The module that was found to be missing</param>
/// <param name="SourceVersion">The version range in which the module should exist</param>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleMissingIssue(
    ModuleInfoExtended Module,
    ApplicationVersionRange SourceVersion
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module '{Module.Id}' {Module.Version} is missing from modules list";
    public override LegacyModuleIssue ToLegacy() => new(Module, Module.Id, ModuleIssueType.Missing, ToString(), SourceVersion);
}

/// <summary>
///     Represents an issue where a required dependency module is missing and no version was specified
/// </summary>
/// <param name="Module">The module with the missing dependency</param>
/// <param name="Dependency">The missing dependency module</param>
/// <remarks>
/// This issue occurs when a required unversioned dependency is missing.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `SimpleTournaments` -->
///     <Id value="SimpleTournaments"/>
///     <DependedModules>
///         <!-- 👇 This dependency `TournamentOverhaul` is not installed -->
///         <DependedModule Id="TournamentOverhaul" />
///     </DependedModules>
/// </Module>
/// ```
/// If `TournamentOverhaul` is not installed at all, this issue will be raised if `SimpleTournaments` is enabled.
/// Note that it's recommended to use `DependedModuleMetadatas` with version specifications instead.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleMissingUnversionedDependencyIssue(
    ModuleInfoExtended Module,
    DependentModuleMetadata Dependency
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Missing '{Dependency.Id}' module";
    public override LegacyModuleIssue ToLegacy() => new(
        Module,
        Dependency.Id,
        ModuleIssueType.MissingDependencies,
        ToString(),
        ApplicationVersionRange.Empty);
}

/// <summary>
///     Represents an issue where a required dependency module is missing AND an exact version was specified
/// </summary>
/// <param name="Module">The module with the missing dependency</param>
/// <param name="Dependency">The missing dependency module</param>
/// <remarks>
/// This issue occurs when a required dependency with an exact version requirement is missing.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `AdvancedPartyAI` -->
///     <Id value="AdvancedPartyAI"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 This dependency `Bannerlord.UIExtenderEx` (version `v2.12.0`) is not installed -->
///         <DependedModuleMetadata id="Bannerlord.UIExtenderEx" order="LoadBeforeThis" version="v2.12.0" />
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// If `Bannerlord.UIExtenderEx` is not installed at all (any version), this issue will be raised.
/// This is different from version mismatch issues where the module is present but with wrong version.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleMissingExactVersionDependencyIssue(
    ModuleInfoExtended Module,
    DependentModuleMetadata Dependency
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Missing '{Dependency.Id}' with required version {Dependency.Version}";
    public override LegacyModuleIssue ToLegacy() => new(
        Module,
        Dependency.Id,
        ModuleIssueType.MissingDependencies,
        ToString(),
        new ApplicationVersionRange(Dependency.Version, Dependency.Version));
}

/// <summary>
///     Represents an issue where a required dependency module is missing and a version range was specified
/// </summary>
/// <param name="Module">The module with the missing dependency</param>
/// <param name="Dependency">The missing dependency module</param>
/// <remarks>
/// This issue occurs when a required dependency with a version range requirement is missing.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `DiplomacyFixes` -->
///     <Id value="DiplomacyFixes"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 This dependency `Bannerlord.UIExtenderEx` (version range `v1.0.0-v1.9.*`) is not installed -->
///         <DependedModuleMetadata id="Bannerlord.UIExtenderEx" order="LoadBeforeThis" version="v1.0.0-v1.9.*" />
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// If `Bannerlord.UIExtenderEx` module is not installed at all (any version), this issue will be raised.
/// This is different from version mismatch issues where the module is present but with wrong version.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleMissingVersionRangeDependencyIssue(
    ModuleInfoExtended Module,
    DependentModuleMetadata Dependency
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Missing '{Dependency.Id}' with required version range [{Dependency.VersionRange}]";
    public override LegacyModuleIssue ToLegacy() => new(Module,
        Dependency.Id,
        ModuleIssueType.MissingDependencies,
        ToString(),
        Dependency.VersionRange);
}

/// <summary>
///     Represents an issue where a module's dependency is itself missing required dependencies.
///     This is a cascading issue where a module's dependency has unresolved dependencies,
///     indicating a deeper problem in the dependency chain.
/// </summary>
/// <param name="Module">The module whose dependency has missing dependencies</param>
/// <param name="DependencyId">The ID of the dependency module that is missing its own dependencies</param>
/// <remarks>
/// This issue occurs when a dependency has missing dependencies of its own.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `RealisticWeather` -->
///     <Id value="RealisticWeather"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 `BetterTime` mod requires `Harmony` -->
///         <DependedModuleMetadata id="BetterTime" order="LoadBeforeThis" />
///         <!-- but `Harmony` is not installed -->
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// If `BetterTime` requires `Harmony` but `Harmony` is not installed, this issue will be raised for `RealisticWeather`
/// because its dependency (`BetterTime`) has missing dependencies.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleDependencyMissingDependenciesIssue(
    ModuleInfoExtended Module,
    string DependencyId
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module '{Module.Id}': Required dependency '{DependencyId}' is missing its own dependencies";
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.DependencyMissingDependencies, ToString(), ApplicationVersionRange.Empty);
}

/// <summary>
///     Represents an issue where a module's dependency fails validation checks.
///     This indicates that while the dependency exists, it has its own validation
///     issues that need to be resolved.
/// </summary>
/// <param name="Module">The module with the dependency that failed validation</param>
/// <param name="DependencyId">The ID of the dependency module that failed validation</param>
/// <remarks>
/// This issue occurs when a dependency has validation issues of its own.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `CustomSpawns` -->
///     <Id value="CustomSpawns"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 But `CustomSpawnsFramework` has issues -->
///         <DependedModuleMetadata id="CustomSpawnsFramework" order="LoadBeforeThis" />
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// If `CustomSpawnsFramework` has its own issues (like missing dependencies or invalid configuration),
/// this issue will be raised for `CustomSpawns` since its dependency needs to be fixed first.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleDependencyValidationIssue(
    ModuleInfoExtended Module,
    string DependencyId
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module '{Module.Id}': Dependency '{DependencyId}' has unresolved validation issues";
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.DependencyValidationError, ToString(), ApplicationVersionRange.Empty);
}

/// <summary>
///     Base record type for version mismatch issues between modules and their dependencies.
///     This serves as an abstract base for both specific version and version range issues.
/// </summary>
/// <param name="Module">The module with the version mismatch</param>
/// <param name="Dependency">The dependency module with mismatched version</param>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    abstract record ModuleVersionMismatchIssue(
    ModuleInfoExtended Module,
    ModuleInfoExtended Dependency
) : ModuleIssueV2(Module);

/// <summary>
///     Base record type for version mismatch issues involving specific versions.
///     Used when comparing against exact version numbers rather than ranges.
/// </summary>
/// <param name="Module">The module with the version mismatch</param>
/// <param name="Dependency">The dependency module with mismatched version</param>
/// <param name="Version">The specific version being compared against</param>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    abstract record ModuleVersionMismatchSpecificIssue(
    ModuleInfoExtended Module,
    ModuleInfoExtended Dependency,
    ApplicationVersion Version
) : ModuleVersionMismatchIssue(Module, Dependency);

/// <summary>
///     Base record type for version mismatch issues involving version ranges.
///     Used when comparing against version ranges rather than specific versions.
/// </summary>
/// <param name="Module">The module with the version mismatch</param>
/// <param name="Dependency">The dependency module with mismatched version</param>
/// <param name="VersionRange">The version range being compared against</param>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    abstract record ModuleVersionMismatchRangeIssue(
    ModuleInfoExtended Module,
    ModuleInfoExtended Dependency,
    ApplicationVersionRange VersionRange
) : ModuleVersionMismatchIssue(Module, Dependency);

/// <summary>
///     Represents an issue where a dependency's version is higher than the maximum allowed specific version.
///     This occurs when a dependency module's version exceeds an exact version requirement.
/// </summary>
/// <param name="Module">The module with the version constraint</param>
/// <param name="Dependency">The dependency module that exceeds the version requirement</param>
/// <param name="Version">The specific version that should not be exceeded</param>
/// <remarks>
/// This issue occurs when a module specifies incompatible versions.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `BetterSiege` -->
///     <Id value="BetterSiege"/>
///     <DependedModuleMetadatas>
///         <!-- ✅ `Bannerlord.Harmony` is installed -->
///         <DependedModuleMetadata id="Bannerlord.Harmony" order="LoadBeforeThis" version="v2.2.2" />
///         <!-- ❌ However the installed version of `Bannerlord.Harmony` (`v2.3.0`)
///                 is greater than requested version `v2.2.2` -->
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// 
/// If a higher version of Harmony (e.g., `v2.3.0`) is installed than allowed, this issue will be raised.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleVersionMismatchLessThanOrEqualSpecificIssue(
    ModuleInfoExtended Module,
    ModuleInfoExtended Dependency,
    ApplicationVersion Version
) : ModuleVersionMismatchSpecificIssue(Module, Dependency, Version)
{
    public override string ToString() => 
        $"The module '{Module.Id}' requires version {Version} or lower of '{Dependency.Id}', but version {Dependency.Version} is installed";

    public override LegacyModuleIssue ToLegacy() => new(Module, Dependency.Id, ModuleIssueType.VersionMismatchLessThanOrEqual, ToString(), new ApplicationVersionRange(Version, Version));
}

/// <summary>
///     Represents an issue where a dependency's version is less than the minimum required version range.
///     This occurs when a dependency module's version is below the minimum version specified in a range.
/// </summary>
/// <param name="Module">The module with the version requirement</param>
/// <param name="Dependency">The dependency module that doesn't meet the minimum version</param>
/// <param name="VersionRange">The version range containing the minimum version requirement</param>
/// <remarks>
/// This issue occurs when a dependency's version is below the minimum required version range.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `BannerColorPersistence` -->
///     <Id value="BannerColorPersistence"/>
///     <DependedModuleMetadatas>
///         <!-- ✅ `Bannerlord.ButterLib` is installed -->
///         <DependedModuleMetadata id="Bannerlord.ButterLib" order="LoadBeforeThis" version="v2.8.15" />
///         <!-- ❌ However the installed version of `Bannerlord.ButterLib` (`v2.8.14`)
///                 is older than requested version `v2.8.15` -->
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// If an older version of ButterLib (e.g., v2.8.14) is installed, this issue will be raised.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleVersionMismatchLessThanRangeIssue(
    ModuleInfoExtended Module,
    ModuleInfoExtended Dependency,
    ApplicationVersionRange VersionRange
) : ModuleVersionMismatchRangeIssue(Module, Dependency, VersionRange)
{
    public override string ToString() => 
        $"The module '{Module.Id}' requires '{Dependency.Id}' version {VersionRange}, but version {Dependency.Version} is installed (below minimum)";

    public override LegacyModuleIssue ToLegacy() => new(Module, Dependency.Id, ModuleIssueType.VersionMismatchLessThan, ToString(), VersionRange);
}

/// <summary>
///     Represents an issue where a dependency's version exceeds the maximum allowed version range.
///     This occurs when a dependency module's version is above the maximum version specified in a range.
/// </summary>
/// <param name="Module">The module with the version constraint</param>
/// <param name="Dependency">The dependency module that exceeds the version limit</param>
/// <param name="VersionRange">The version range containing the maximum version requirement</param>
/// <remarks>
/// This issue occurs when a dependency's version exceeds the maximum allowed version range.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `RtsCamera` -->
///     <Id value="RtsCamera"/>
///     <DependedModuleMetadatas>
///         <!-- ✅ `ModB` is installed -->
///         <DependedModuleMetadata id="ModB" order="LoadBeforeThis" version="v1.0.0-v1.1.0" />
///         <!-- ❌ However the installed version of `ModB` (`v1.2.0`)
///                 is greater than requested version range [`v1.0.0` to `v1.1.0`] -->
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// If a version of `ModB` that falls within the range is installed, this issue will be raised.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleVersionMismatchGreaterThanRangeIssue(
    ModuleInfoExtended Module,
    ModuleInfoExtended Dependency,
    ApplicationVersionRange VersionRange
) : ModuleVersionMismatchRangeIssue(Module, Dependency, VersionRange)
{
    public override string ToString() => 
        $"The module '{Module.Id}' requires '{Dependency.Id}' version {VersionRange}, but version {Dependency.Version} is installed (above maximum)";
    public override LegacyModuleIssue ToLegacy() => new(Module, Dependency.Id, ModuleIssueType.VersionMismatchGreaterThan, ToString(), VersionRange);
}

/// <summary>
///     Represents an issue where two modules are incompatible with each other.
///     This occurs when one module explicitly declares it cannot work with another module.
/// </summary>
/// <param name="Module">The module that has declared an incompatibility</param>
/// <param name="IncompatibleModuleId">The ID of the module that is incompatible with the target</param>
/// <remarks>
/// This issue occurs when a module explicitly marks another module as incompatible.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `AlternativeArmorSystem` -->
///     <Id value="AlternativeArmorSystem"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 Current mod is incompatible with `RealisticBattleArmor` -->
///         <DependedModuleMetadata id="RealisticBattleArmor" incompatible="true" />
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// If both `AlternativeArmorSystem` is enabled when `RealisticBattleArmor` is already enabled, this issue will be raised.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleIncompatibleIssue(
    ModuleInfoExtended Module,
    string IncompatibleModuleId
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"'{IncompatibleModuleId}' is incompatible with this module";
    public override LegacyModuleIssue ToLegacy() => new(Module, IncompatibleModuleId, ModuleIssueType.Incompatible, ToString(), ApplicationVersionRange.Empty);
}

/// <summary>
///     Represents an issue where a module is both depended upon and marked as incompatible.
///     This indicates a contradictory configuration where a module is required but also
///     marked as incompatible.
/// </summary>
/// <param name="Module">The module with the conflicting dependency declaration</param>
/// <param name="ConflictingModuleId">The ID of the module that is both depended upon and marked incompatible</param>
/// <remarks>
/// This issue occurs when a module has conflicting configurations for dependencies.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `CustomTroops` -->
///     <Id value="CustomTroops"/>
///     <DependedModules>
///         <!-- 👇 `TroopOverhaul` is marked as a required dependency -->
///         <DependedModule Id="TroopOverhaul" />
///     </DependedModules>
///     <DependedModuleMetadatas>
///         <!-- ❌ `TroopOverhaul` is marked as incompatible, despite being required -->
///         <DependedModuleMetadata id="TroopOverhaul" incompatible="true" />
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// This is a configuration error as `TroopOverhaul` cannot be both required and incompatible.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleDependencyConflictDependentAndIncompatibleIssue(
    ModuleInfoExtended Module,
    string ConflictingModuleId
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module '{Module.Id}' has conflicting configuration: '{ConflictingModuleId}' is marked as both required and incompatible";
    public override LegacyModuleIssue ToLegacy() => new(Module, ConflictingModuleId, ModuleIssueType.DependencyConflictDependentAndIncompatible, ToString(), ApplicationVersionRange.Empty);
}

/// <summary>
///     Represents an issue where a module is declared to load both before and after another module.
///     This indicates a contradictory load order configuration that cannot be satisfied.
/// </summary>
/// <param name="Module">The module with the conflicting load order declaration</param>
/// <param name="ConflictingModule">The module that has conflicting load order requirements</param>
/// <remarks>
/// This issue occurs when a module has conflicting load order requirements.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `ImprovedTournaments` -->
///     <Id value="ImprovedTournaments"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 `ArenaOverhaul` is marked as `LoadBeforeThis` -->
///         <DependedModuleMetadata id="ArenaOverhaul" order="LoadBeforeThis" />
///         <!-- 👇 `ArenaOverhaul` is marked as `LoadAfterThis` -->
///         <DependedModuleMetadata id="ArenaOverhaul" order="LoadAfterThis" />
///         <!-- ❌ `ArenaOverhaul` cannot be marked both `LoadBefore` and `LoadAfter` -->
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// This creates an impossible load order requirement as `ArenaOverhaul` cannot load both before and after `ImprovedTournaments`.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleDependencyConflictLoadBeforeAndAfterIssue(
    ModuleInfoExtended Module,
    DependentModuleMetadata ConflictingModule
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module '{Module.Id}' has conflicting load order requirements with '{ConflictingModule.Id}' (both LoadBefore and LoadAfter)";
    public override LegacyModuleIssue ToLegacy() => new(Module, ConflictingModule.Id, ModuleIssueType.DependencyConflictDependentLoadBeforeAndAfter, ToString(), ApplicationVersionRange.Empty);
}

/// <summary>
///     Represents an issue where modules have circular dependencies on each other.
///     This occurs when two or more modules form a dependency cycle that cannot be resolved.
/// </summary>
/// <param name="Module">One of the modules in the circular dependency chain</param>
/// <param name="CircularDependency">The other module in the circular dependency chain</param>
/// <remarks>
/// This issue occurs when modules create a circular dependency chain.
/// 
/// Example scenario:
/// Two modules depend on each other in a way that creates an unresolvable cycle:
/// 
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `EnhancedBattle` -->
///     <Id value="EnhancedBattle"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 `EnhancedBattle` requests that `BetterFormations` loads first -->
///         <DependedModuleMetadata id="BetterFormations" order="LoadBeforeThis" />
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// 
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `BetterFormations` -->
///     <Id value="BetterFormations"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 `BetterFormations` requests that `EnhancedBattle` loads first -->
///         <DependedModuleMetadata id="EnhancedBattle" order="LoadBeforeThis" />
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// 
/// ❌ This creates an impossible situation where each module must load before the other.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleDependencyConflictCircularIssue(
    ModuleInfoExtended Module,
    DependentModuleMetadata CircularDependency
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module '{Module.Id}' and '{CircularDependency.Id}' have circular dependencies";
    public override LegacyModuleIssue ToLegacy() => new(Module, CircularDependency.Id, ModuleIssueType.DependencyConflictCircular, ToString(), ApplicationVersionRange.Empty);
}

/// <summary>
///     Represents an issue where a module that should be loaded before the target module is loaded after it.
///     This indicates a violation of the specified load order requirements.
/// </summary>
/// <param name="Module">The module with the load order requirement</param>
/// <param name="DependencyId">The ID of the module that should be loaded before the target</param>
/// <remarks>
/// This issue occurs when a required "load before" dependency loads after the specified module.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `BetterBattles` -->
///     <Id value="BetterBattles"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 Current mod `BetterBattles` requests `Bannerlord.Harmony` is loaded first -->
///         <DependedModuleMetadata id="Bannerlord.Harmony" order="LoadBeforeThis" />
///         <!-- ❌ Current mod (`BetterBattles`) is set to load before `Bannerlord.Harmony`
///              in the current load order. This is invalid. -->
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// If `Harmony` is loading after `BetterBattles` in the actual load order, this issue will be raised.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleDependencyNotLoadedBeforeIssue(
    ModuleInfoExtended Module,
    string DependencyId
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"'{DependencyId}' should be loaded before '{Module.Id}'";
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.DependencyNotLoadedBeforeThis, ToString(), ApplicationVersionRange.Empty);
}

/// <summary>
///     Represents an issue where a module that should be loaded after the target module is loaded before it.
///     This indicates a violation of the specified load order requirements.
/// </summary>
/// <param name="Module">The module with the load order requirement</param>
/// <param name="DependencyId">The ID of the module that should be loaded after the target</param>
/// <remarks>
/// This issue occurs when a required "load after" dependency loads before the specified module.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `BetterSettlements` -->
///     <Id value="BetterSettlements"/>
///     <DependedModuleMetadatas>
///         <!-- 👇 Current mod `BetterSettlements` requests `ImprovedGarrisons` is loaded after it -->
///         <DependedModuleMetadata id="ImprovedGarrisons" order="LoadAfterThis" />
///         <!-- ❌ Current mod (`BetterSettlements`) is set to load after `ImprovedGarrisons`
///              in the current load order. This is invalid. -->
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// If `ImprovedGarrisons` is loading before `BetterSettlements`, this issue will be raised.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleDependencyNotLoadedAfterIssue(
    ModuleInfoExtended Module,
    string DependencyId
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"'{DependencyId}' should be loaded after '{Module.Id}'";
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.DependencyNotLoadedAfterThis, ToString(), ApplicationVersionRange.Empty);
}

/// <summary>
///     Represents an issue where a module is missing its required module ID.
///     This is required by every mod.
/// </summary>
/// <param name="Module">The module missing its ID</param>
/// <remarks>
/// This issue occurs when a module is missing its required Id field.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Required `Id` element is missing here.
///             Example:
///                 `<Id value="BetterSiege"/>`
///     -->
///     <Name value="Better Sieges"/>
/// </Module>
/// ```
/// The Id field is required for module identification and dependency management.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleMissingIdIssue(
    ModuleInfoExtended Module
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module with Name '{Module.Name}' is missing its Id field";
    public override LegacyModuleIssue ToLegacy() => new(Module, "UNKNOWN", ModuleIssueType.MissingModuleId, ToString(), ApplicationVersionRange.Empty);
}

/// <remarks>
///     Represents an issue where a module is missing its required name.
///     This is a required field.
/// </remarks>
/// <param name="Module">The module missing its name</param>
/// <remarks>
/// This issue occurs when a module has a malformed or missing Name field.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Required `Name` element is missing here.
///             Example:
///                 `<Name value="Better Sieges"/>`
///     -->
///     <Id value="BetterSiege"/>
/// </Module>
/// ```
/// The Name field is required for module identification and display purposes.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleMissingNameIssue(
    ModuleInfoExtended Module
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module with Id '{Module.Id}' is missing its Name field";
    public override LegacyModuleIssue ToLegacy() => new(Module, Module.Id, ModuleIssueType.MissingModuleName, ToString(), ApplicationVersionRange.Empty);
}

/// <remarks>
///     Represents an issue where a module has a null/empty dependency reference.
///     This indicates an invalid dependency configuration.
/// </remarks>
/// <param name="Module">The module with the null dependency</param>
/// <remarks>
/// This issue occurs when a dependency entry is malformed or null.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `ImprovedGarrisons` -->
///     <Id value="ImprovedGarrisons"/>
///     <DependedModules>
///         <!-- ❌ Empty/invalid dependency entry -->
///         <DependedModule />
///         <!-- 💡 Consider adding an `id` field
///              <DependedModuleMetadata id="GarrisonsExtensions" />
///         -->
///     </DependedModules>
/// </Module>
/// ```
/// All dependency entries must be properly formed with required attributes.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleDependencyNullIssue(
    ModuleInfoExtended Module
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module '{Module.Id}' has a null dependency entry";
    public override LegacyModuleIssue ToLegacy() => new(Module, "UNKNOWN", ModuleIssueType.DependencyIsNull, ToString(), ApplicationVersionRange.Empty);
}

/// <remarks>
///     Represents an issue where a module's dependency is missing its required module ID.
///     This indicates an invalid or incomplete dependency configuration.
/// </remarks>
/// <param name="Module">The module with a dependency missing its ID</param>
/// <remarks>
/// This issue occurs when a dependency entry is missing its required Id field.
/// 
/// Example scenario:
/// ```xml
/// <Module>
///     <!-- 👇 Current mod is `ImprovedGarrisons` -->
///     <Id value="DiplomacyTweaks"/>
///     <DependedModuleMetadatas>
///         <!-- ❌ Missing `id` attribute in dependency entry -->
///         <DependedModuleMetadata version="v2.2.2" />
///         <!-- 💡 Consider adding an `id` field
///              <DependedModuleMetadata id="DiplomacyExtensions" version="v2.2.2" />
///         -->
///     </DependedModuleMetadatas>
/// </Module>
/// ```
/// All dependency entries must include an Id to identify the required module.
/// </remarks>
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
internal
#else
public
# endif
    sealed record ModuleDependencyMissingIdIssue(
    ModuleInfoExtended Module
) : ModuleIssueV2(Module)
{
    public override string ToString() => $"Module '{Module.Id}' has a dependency entry missing its Id field";
    public override LegacyModuleIssue ToLegacy() => new(Module, "UNKNOWN", ModuleIssueType.DependencyMissingModuleId, ToString(), ApplicationVersionRange.Empty);
}

#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif