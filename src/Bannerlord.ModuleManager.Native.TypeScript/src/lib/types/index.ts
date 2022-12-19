export interface ModuleInfoExtended {
    id: string;
    name: string;
    isOfficial: boolean;
    version: ApplicationVersion;
    isSingleplayerModule: boolean;
    isMultiplayerModule: boolean;
    subModules: Array<SubModuleInfoExtended>;
    dependentModules: Array<DependentModule>;
    modulesToLoadAfterThis: Array<DependentModule>;
    incompatibleModules: Array<DependentModule>;
    url: string;
    dependentModuleMetadatas: Array<DependentModuleMetadata>;
}
export interface ApplicationVersion {
    applicationVersionType: ApplicationVersionType;
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
export interface SubModuleInfoExtended {
    name: string;
    dLLName: string;
    assemblies: Array<string>;
    subModuleClassType: string;
    tags: Map<string, Array<string>>;
}
export interface DependentModule {
    id: string;
    version: ApplicationVersion;
    isOptional: boolean;
}
export interface DependentModuleMetadata {
    id: string;
    loadType: LoadType;
    isOptional: boolean;
    isIncompatible: boolean;
    version: ApplicationVersion;
    versionRange: ApplicationVersionRange;
}
export enum LoadType {
    None,
    LoadAfterThis,
    LoadBeforeThis
}
export interface ApplicationVersionRange {
    min: ApplicationVersion;
    max: ApplicationVersion;
}
export interface ModuleSorterOptions {
    skipOptionals: boolean;
    skipExternalDependencies: boolean;
}
export interface ModuleIssue {
    target: ModuleInfoExtended;
    sourceId: string;
    type: ModuleIssueType;
    reason: string;
    sourceVersion: ApplicationVersionRange;
}
export enum ModuleIssueType {
    MissingDependencies,
    DependencyMissingDependencies,
    DependencyValidationError,
    VersionMismatch,
    Incompatible,
    DependencyConflict
}

export interface IValidationManager {
    isSelected(moduleId: string): boolean,
}

export interface IEnableDisableManager {
    getSelected(moduleId: string): boolean,
    setSelected(moduleId: string, value: boolean): void,
    getDisabled(moduleId: string): boolean,
    setDisabled(moduleId: string, value: boolean): void,
}

export interface IBannerlordModuleManager {
    sort(unsorted: Array<ModuleInfoExtended>): Array<ModuleInfoExtended>;
    sortWithOptions(unsorted: Array<ModuleInfoExtended>, options: ModuleSorterOptions): Array<ModuleInfoExtended>;

    areAllDependenciesOfModulePresent(unsorted: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended): boolean;

    getDependentModulesOf(source: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended): Array<ModuleInfoExtended>;
    getDependentModulesOfWithOptions(source: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended, options: ModuleSorterOptions): Array<ModuleInfoExtended>;

    validateLoadOrder(modules: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended): Array<ModuleIssue>;
    validateModule(modules: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended, manager: IValidationManager): Array<ModuleIssue>;

    enableModule(modules: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended, manager: IEnableDisableManager): void;
    disableModule(modules: Array<ModuleInfoExtended>, targetModule: ModuleInfoExtended, manager: IEnableDisableManager): void;

    getModuleInfo(xml: string): ModuleInfoExtended | undefined;
    getSubModuleInfo(xml: string): SubModuleInfoExtended | undefined;

    compareVersions(x: ApplicationVersion, y: ApplicationVersion): number;
}