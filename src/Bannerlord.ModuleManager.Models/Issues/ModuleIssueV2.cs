using LegacyModuleIssue = Bannerlord.ModuleManager.ModuleIssue;

namespace Bannerlord.ModuleManager.Models.Issues;

/// <remarks>
/// Base record type for all module-related issues that can occur during validation. 
/// This record serves as the abstract base class for all specific issue variants,
/// providing a common structure and conversion capability to legacy formats.
/// </remarks>
/// <param name="Module">The module in which the issue was detected. This is always required.</param>
public abstract record ModuleIssueV2(ModuleInfoExtended Module)
{
    /// <summary>
    /// Converts this issue instance to the legacy ModuleIssue format for backwards compatibility
    /// </summary>
    /// <returns>A <see cref="LegacyModuleIssue"/> representation of this issue</returns>
    public abstract LegacyModuleIssue ToLegacy();
}

/// <remarks>
/// Represents an issue where a required module is missing from the module list.
/// This typically occurs when a module declares a dependency on another module
/// that is not present in the current module collection.
/// </remarks>
/// <param name="Module">The module that was found to be missing</param>
/// <param name="SourceVersion">The version range in which the module should exist</param>
/// <remarks>
/// This issue occurs when a module is completely missing from the game's module list.
/// 
/// Example scenario:
/// Your SubModule.xml references a required module, but that module is not installed:
/// ```xml
/// <DependedModules>
///   <DependedModule Id="Bannerlord.Harmony" />  <!-- But Harmony is not installed -->
/// </DependedModules>
/// ```
/// </remarks>
public sealed record ModuleMissingIssue(
    ModuleInfoExtended Module,
    ApplicationVersionRange SourceVersion
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, Module.Id, ModuleIssueType.Missing, $"Missing '{Module.Id}' {Module.Version} in modules list", SourceVersion);
}

/// <remarks>
/// Represents an issue where a module is missing one or more of its required dependencies.
/// This can occur when a module explicitly declares a dependency that cannot be found,
/// either because it's not installed or not loaded.
/// </remarks>
/// <param name="Module">The module with missing dependencies</param>
/// <param name="DependencyId">The ID of the missing dependency module</param>
/// <param name="SourceVersion">Optional version range specifying which versions of the dependency are acceptable</param>
public sealed record ModuleMissingDependenciesIssue(
    ModuleInfoExtended Module,
    string DependencyId,
    ApplicationVersionRange? SourceVersion = null
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy()
    {
        var reason = SourceVersion switch
        {
            null => $"Missing '{DependencyId}'",
            _ => $"Missing '{DependencyId}' {SourceVersion}"
        };
        return new(Module, DependencyId, ModuleIssueType.MissingDependencies, reason, SourceVersion ?? ApplicationVersionRange.Empty);
    }
}

/// <remarks>
/// Represents an issue where a module's dependency is itself missing required dependencies.
/// This is a cascading issue where a module's dependency has unresolved dependencies,
/// indicating a deeper problem in the dependency chain.
/// </remarks>
/// <param name="Module">The module whose dependency has missing dependencies</param>
/// <param name="DependencyId">The ID of the dependency module that is missing its own dependencies</param>
public sealed record ModuleDependencyMissingDependenciesIssue(
    ModuleInfoExtended Module,
    string DependencyId
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.DependencyMissingDependencies, $"'{DependencyId}' is missing its dependencies!", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where a module's dependency fails validation checks.
/// This indicates that while the dependency exists, it has its own validation
/// issues that need to be resolved.
/// </remarks>
/// <param name="Module">The module with the dependency that failed validation</param>
/// <param name="DependencyId">The ID of the dependency module that failed validation</param>
public sealed record ModuleDependencyValidationIssue(
    ModuleInfoExtended Module,
    string DependencyId
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.DependencyValidationError, $"'{DependencyId}' has unresolved issues!", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Base record type for version mismatch issues between modules and their dependencies.
/// This serves as an abstract base for both specific version and version range issues.
/// </remarks>
/// <param name="Module">The module with the version mismatch</param>
/// <param name="DependencyId">The ID of the dependency module with mismatched version</param>
public abstract record ModuleVersionMismatchIssue(
    ModuleInfoExtended Module,
    string DependencyId
) : ModuleIssueV2(Module);

/// <remarks>
/// Base record type for version mismatch issues involving specific versions.
/// Used when comparing against exact version numbers rather than ranges.
/// </remarks>
/// <param name="Module">The module with the version mismatch</param>
/// <param name="DependencyId">The ID of the dependency module with mismatched version</param>
/// <param name="Version">The specific version being compared against</param>
public abstract record ModuleVersionMismatchSpecificIssue(
    ModuleInfoExtended Module,
    string DependencyId,
    ApplicationVersion Version
) : ModuleVersionMismatchIssue(Module, DependencyId);

/// <remarks>
/// Base record type for version mismatch issues involving version ranges.
/// Used when comparing against version ranges rather than specific versions.
/// </remarks>
/// <param name="Module">The module with the version mismatch</param>
/// <param name="DependencyId">The ID of the dependency module with mismatched version</param>
/// <param name="VersionRange">The version range being compared against</param>
public abstract record ModuleVersionMismatchRangeIssue(
    ModuleInfoExtended Module,
    string DependencyId,
    ApplicationVersionRange VersionRange
) : ModuleVersionMismatchIssue(Module, DependencyId);

/// <remarks>
/// Represents an issue where a dependency's version is higher than the maximum allowed specific version.
/// This occurs when a dependency module's version exceeds an exact version requirement.
/// </remarks>
/// <param name="Module">The module with the version constraint</param>
/// <param name="DependencyId">The ID of the dependency module that exceeds the version requirement</param>
/// <param name="Version">The specific version that should not be exceeded</param>
public sealed record ModuleVersionMismatchLessThanOrEqualSpecificIssue(
    ModuleInfoExtended Module,
    string DependencyId,
    ApplicationVersion Version
) : ModuleVersionMismatchSpecificIssue(Module, DependencyId, Version)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.VersionMismatchLessThanOrEqual, $"'{DependencyId}' wrong version <= {Version}", new(Version, Version));
}

/// <remarks>
/// Represents an issue where a dependency's version is less than the minimum required version range.
/// This occurs when a dependency module's version is below the minimum version specified in a range.
/// </remarks>
/// <param name="Module">The module with the version requirement</param>
/// <param name="DependencyId">The ID of the dependency module that doesn't meet the minimum version</param>
/// <param name="VersionRange">The version range containing the minimum version requirement</param>
public sealed record ModuleVersionMismatchLessThanRangeIssue(
    ModuleInfoExtended Module,
    string DependencyId,
    ApplicationVersionRange VersionRange
) : ModuleVersionMismatchRangeIssue(Module, DependencyId, VersionRange)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.VersionMismatchLessThan, $"'{DependencyId}' wrong version < [{VersionRange}]", VersionRange);
}

/// <remarks>
/// Represents an issue where a dependency's version exceeds the maximum allowed version range.
/// This occurs when a dependency module's version is above the maximum version specified in a range.
/// </remarks>
/// <param name="Module">The module with the version constraint</param>
/// <param name="DependencyId">The ID of the dependency module that exceeds the version limit</param>
/// <param name="VersionRange">The version range containing the maximum version requirement</param>
public sealed record ModuleVersionMismatchGreaterThanRangeIssue(
    ModuleInfoExtended Module,
    string DependencyId,
    ApplicationVersionRange VersionRange
) : ModuleVersionMismatchRangeIssue(Module, DependencyId, VersionRange)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.VersionMismatchGreaterThan, $"'{DependencyId}' wrong version > [{VersionRange}]", VersionRange);
}

/// <remarks>
/// Represents an issue where two modules are incompatible with each other.
/// This occurs when one module explicitly declares it cannot work with another module.
/// </remarks>
/// <param name="Module">The module that has declared an incompatibility</param>
/// <param name="IncompatibleModuleId">The ID of the module that is incompatible with the target</param>
public sealed record ModuleIncompatibleIssue(
    ModuleInfoExtended Module,
    string IncompatibleModuleId
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, IncompatibleModuleId, ModuleIssueType.Incompatible, $"'{IncompatibleModuleId}' is incompatible with this module", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where a module is both depended upon and marked as incompatible.
/// This indicates a contradictory configuration where a module is required but also
/// marked as incompatible.
/// </remarks>
/// <param name="Module">The module with the conflicting dependency declaration</param>
/// <param name="ConflictingModuleId">The ID of the module that is both depended upon and marked incompatible</param>
public sealed record ModuleDependencyConflictDependentAndIncompatibleIssue(
    ModuleInfoExtended Module,
    string ConflictingModuleId
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, ConflictingModuleId, ModuleIssueType.DependencyConflictDependentAndIncompatible, $"Module '{ConflictingModuleId}' is both depended upon and marked as incompatible", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where a module is declared to load both before and after another module.
/// This indicates a contradictory load order configuration that cannot be satisfied.
/// </remarks>
/// <param name="Module">The module with the conflicting load order declaration</param>
/// <param name="ConflictingModuleId">The ID of the module that has conflicting load order requirements</param>
public sealed record ModuleDependencyConflictLoadBeforeAndAfterIssue(
    ModuleInfoExtended Module,
    string ConflictingModuleId
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, ConflictingModuleId, ModuleIssueType.DependencyConflictDependentLoadBeforeAndAfter, $"Module '{ConflictingModuleId}' is both depended upon as LoadBefore and LoadAfter", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where modules have circular dependencies on each other.
/// This occurs when two or more modules form a dependency cycle that cannot be resolved.
/// </remarks>
/// <param name="Module">One of the modules in the circular dependency chain</param>
/// <param name="CircularDependencyId">The ID of another module in the circular dependency chain</param>
public sealed record ModuleDependencyConflictCircularIssue(
    ModuleInfoExtended Module,
    string CircularDependencyId
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, CircularDependencyId, ModuleIssueType.DependencyConflictCircular, $"Circular dependencies. '{Module.Id}' and '{CircularDependencyId}' depend on each other", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where a module that should be loaded before the target module is loaded after it.
/// This indicates a violation of the specified load order requirements.
/// </remarks>
/// <param name="Module">The module with the load order requirement</param>
/// <param name="DependencyId">The ID of the module that should be loaded before the target</param>
public sealed record ModuleDependencyNotLoadedBeforeIssue(
    ModuleInfoExtended Module,
    string DependencyId
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.DependencyNotLoadedBeforeThis, $"'{DependencyId}' should be loaded before '{Module.Id}'", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where a module that should be loaded after the target module is loaded before it.
/// This indicates a violation of the specified load order requirements.
/// </remarks>
/// <param name="Module">The module with the load order requirement</param>
/// <param name="DependencyId">The ID of the module that should be loaded after the target</param>
public sealed record ModuleDependencyNotLoadedAfterIssue(
    ModuleInfoExtended Module,
    string DependencyId
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, DependencyId, ModuleIssueType.DependencyNotLoadedAfterThis, $"'{DependencyId}' should be loaded after '{Module.Id}'", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where a module is missing its required module ID.
/// This is required by every mod.
/// </remarks>
/// <param name="Module">The module missing its ID</param>
public sealed record ModuleMissingIdIssue(
    ModuleInfoExtended Module
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, "UNKNOWN", ModuleIssueType.MissingModuleId, $"Module Id is missing for '{Module.Name}'", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where a module is missing its required name.
/// This is a required field.
/// </remarks>
/// <param name="Module">The module missing its name</param>
public sealed record ModuleMissingNameIssue(
    ModuleInfoExtended Module
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, Module.Id, ModuleIssueType.MissingModuleName, $"Module Name is missing in '{Module.Id}'", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where a module has a null/empty dependency reference.
/// This indicates an invalid dependency configuration.
/// </remarks>
/// <param name="Module">The module with the null dependency</param>
public sealed record ModuleDependencyNullIssue(
    ModuleInfoExtended Module
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, "UNKNOWN", ModuleIssueType.DependencyIsNull, $"Found a null dependency in '{Module.Id}'", ApplicationVersionRange.Empty);
}

/// <remarks>
/// Represents an issue where a module's dependency is missing its required module ID.
/// This indicates an invalid or incomplete dependency configuration.
/// </remarks>
/// <param name="Module">The module with a dependency missing its ID</param>
public sealed record ModuleDependencyMissingIdIssue(
    ModuleInfoExtended Module
) : ModuleIssueV2(Module)
{
    public override LegacyModuleIssue ToLegacy() => new(Module, "UNKNOWN", ModuleIssueType.DependencyMissingModuleId, $"Module Id is missing for one of the dependencies of '{Module.Id}'", ApplicationVersionRange.Empty);
}