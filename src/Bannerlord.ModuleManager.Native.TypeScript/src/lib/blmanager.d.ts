declare module "*blmanager.node" {

    export type ModuleInfoExtended = {
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
    export type ApplicationVersion = {
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
    export type ModuleSorterOptions = {
        skipOptionals: boolean;
        skipExternalDependencies: boolean;
    }
    export type ModuleIssue = {
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
    export type ApplicationVersionRange = {
        min: ApplicationVersion;
        max: ApplicationVersion;
    }
    export type SubModuleInfoExtended = {
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

    function sort(unsorted: ModuleInfoExtended[]): ModuleInfoExtended[];
    function sortWithOptions(unsorted: ModuleInfoExtended[], options: ModuleSorterOptions): ModuleInfoExtended[];

    function areAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], module: ModuleInfoExtended): boolean;

    function getDependentModulesOf(source: ModuleInfoExtended[], module: ModuleInfoExtended): ModuleInfoExtended[];
    function getDependentModulesOfWithOptions(source: ModuleInfoExtended[], module: ModuleInfoExtended, options: ModuleSorterOptions): ModuleInfoExtended[];

    function validateModuleDependenciesDeclarations(module: ModuleInfoExtended): ModuleIssue[];
    function validateModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IValidationManager): ModuleIssue[];

    function enableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): ModuleIssue[];
    function disableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): ModuleIssue[];

    function getModuleInfo(xml: string): ModuleInfoExtended | undefined;
    function getSubModuleInfo(xml: string): SubModuleInfoExtended | undefined;

    function compareVersions(x: ApplicationVersion, y: ApplicationVersion): number;
}