declare module '@butr/blmodulemanager-raw' {
    import { DotNetRuntime } from "@butr/dotnet-runtime-ts/lib/types/dotnet";
    import { DotNetWasmWrapper } from "@butr/dotnet-runtime-ts/lib/types/wrapper";
  
    export enum ApplicationVersionType {
        Alpha,
        Beta,
        EarlyAccess,
        Release,
        Development,
        Invalid,
    }

    export interface IApplicationVersion {
        applicationVersionType: ApplicationVersionType;
        major: number;
        minor: number;
        revision: number;
        changeSet: number;
    }

    export interface IApplicationVersionRange {
        min: IApplicationVersion;
        max: IApplicationVersion;
    }

    export interface IDependentModule {
        id: string;
        version: IApplicationVersion;
    }

    export interface IDependentModuleMetadata {
        id: string;
        loadType: LoadType;
        isOptional: boolean;
        isIncompatible: boolean;
        version: IApplicationVersion;
        versionRange: IApplicationVersionRange;
    }

    export interface IModuleInfo {
        id: string;
        name: string;
        isOfficial: boolean;
        version: IApplicationVersion;
        isSingleplayerModule: boolean;
        isMultiplayerModule: boolean;
        subModules: readonly ISubModuleInfo[];
        dependentModules: readonly IDependentModule[];
    }

    export interface IModuleInfoExtended extends IModuleInfo {
        url: string;
        dependentModuleMetadatas: readonly IDependentModuleMetadata[];
    }

    export interface ISubModuleInfo {
        name: string;
        dllName: string;
        assemblies: readonly string[];
        subModuleClassType: string;
        tags: {
          [index: string]: SubModuleTags;
        };
    }

    export type ISubModuleInfoExtended = ISubModuleInfo

    export enum LoadType {
        None = 0,
        LoadAfterThis = 1,
        LoadBeforeThis = 2,
    }

    export enum SubModuleTags {
        RejectedPlatform,
        ExclusivePlatform,
        DedicatedServerType,
        IsNoRenderModeElement,
        DependantRuntimeLibrary,
    }

    export class LibraryWasm {
        static createAsync(): Promise<LibraryWasm>;
        private init(): Promise<LibraryWasm>;
    
        sort(unsorted: IModuleInfoExtended[]): IModuleInfoExtended[];
        sortAsync(unsorted: IModuleInfoExtended[]): Promise<IModuleInfoExtended[]>;

        areAllDependenciesOfModulePresent(): IModuleInfoExtended[];
        areAllDependenciesOfModulePresentAsync(): Promise<IModuleInfoExtended[]>;
    
        getDependentModulesOf(source: IModuleInfoExtended[], module: IModuleInfoExtended, skipExternalDependencies: boolean): IModuleInfoExtended[];
        getDependentModulesOfAsync(source: IModuleInfoExtended[], module: IModuleInfoExtended, skipExternalDependencies: boolean): Promise<IModuleInfoExtended[]>;
        
        getModuleInfo(xml: string): IModuleInfoExtended;
        getModuleInfoAsync(xml: string): Promise<IModuleInfoExtended>;
        
        getSubModuleInfo(xml: string): IModuleInfoExtended;
        getSubModuleInfoAsync(xml: string): Promise<IModuleInfoExtended>;
    }

    export const dotnet: DotNetRuntime;
    export const wrapper: DotNetWasmWrapper;
    export default wrapper;
}