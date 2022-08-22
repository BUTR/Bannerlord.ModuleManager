export interface ModuleInfoExtended {
    id: string;
    name: string;
    isOfficial: boolean;
    version: ApplicationVersion;
    isSingleplayerModule: boolean;
    isMultiplayerModule: boolean;
    subModules: any;
    dependentModules: any;
    modulesToLoadAfterThis: any;
    incompatibleModules: any;
    url: string;
    dependentModuleMetadatas: any;
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
export interface ApplicationVersionRange {
    min: ApplicationVersion;
    max: ApplicationVersion;
}
export interface SubModuleInfoExtended {
    name: string;
    dLLName: string;
    assemblies: any;
    subModuleClassType: string;
    tags: any;
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
    sort(unsorted: ModuleInfoExtended[]): ModuleInfoExtended[];
    sortWithOptions(unsorted: ModuleInfoExtended[], options: ModuleSorterOptions): ModuleInfoExtended[];

    areAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], module: ModuleInfoExtended): boolean;

    getDependentModulesOf(source: ModuleInfoExtended[], module: ModuleInfoExtended): ModuleInfoExtended[];
    getDependentModulesOfWithOptions(source: ModuleInfoExtended[], module: ModuleInfoExtended, options: ModuleSorterOptions): ModuleInfoExtended[];

    validateModuleDependenciesDeclarations(module: ModuleInfoExtended): ModuleIssue[];
    validateModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IValidationManager): ModuleIssue[];

    enableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): ModuleIssue[];
    disableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): ModuleIssue[];

    getModuleInfo(xml: string): ModuleInfoExtended | undefined;
    getSubModuleInfo(xml: string): SubModuleInfoExtended | undefined;

    compareVersions(x: ApplicationVersion, y: ApplicationVersion): number;
}