export namespace Bannerlord.ModuleManager {
  export type ModuleInfoExtended = {
    id: string;
    name: string;
    isOfficial: boolean;
    version: Bannerlord.ModuleManager.ApplicationVersion;
    isSingleplayerModule: boolean;
    isMultiplayerModule: boolean;
    subModules: any;
    dependentModules: any;
    modulesToLoadAfterThis: any;
    incompatibleModules: any;
    url: string;
    dependentModuleMetadatas: any;
  }
  export type ApplicationVersion = {
    applicationVersionType: Bannerlord.ModuleManager.ApplicationVersionType;
    major: number;
    minor: number;
    revision: number;
    changeSet: number;
  }
  export enum ApplicationVersionType {
    Alpha,
    Beta,
    EarlyAccess,
    Release,
    Development,
    Invalid
  }
  export type ModuleSorterOptions = {
    skipOptionals: boolean;
    skipExternalDependencies: boolean;
  }
  export type ModuleIssue = {
    target: Bannerlord.ModuleManager.ModuleInfoExtended;
    sourceId: string;
    type: Bannerlord.ModuleManager.ModuleIssueType;
    reason: string;
    sourceVersion: Bannerlord.ModuleManager.ApplicationVersionRange;
  }
  export enum ModuleIssueType {
    MissingDependencies,
    DependencyMissingDependencies,
    DependencyValidationError,
    VersionMismatch,
    Incompatible,
    DependencyConflict
  }
  export type ApplicationVersionRange = {
    min: Bannerlord.ModuleManager.ApplicationVersion;
    max: Bannerlord.ModuleManager.ApplicationVersion;
  }
  export type SubModuleInfoExtended = {
    name: string;
    dLLName: string;
    assemblies: any;
    subModuleClassType: string;
    tags: any;
  }
}