import { boot, BootStatus, getBootStatus, invoke, invokeAsync } from "./blmodulemanager";

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
    static async createAsync(): Promise<LibraryWasm> {
        const lib = new LibraryWasm();
        await lib.init();
        return lib;
    }
    private async init(): Promise<void> {
        const status = getBootStatus();
        if (status == BootStatus.Standby){
            await boot();
        }
    }

    sort(unsorted: IModuleInfoExtended[]): IModuleInfoExtended[] {
        return invoke('Sort', unsorted);   
    }
    async sortAsync(unsorted: IModuleInfoExtended[]): Promise<IModuleInfoExtended[]> {
        return await invokeAsync('Sort', unsorted);   
    }

    areAllDependenciesOfModulePresent(unsorted: IModuleInfoExtended[], module: IModuleInfoExtended): IModuleInfoExtended[] {
        return invoke('AreAllDependenciesOfModulePresent', unsorted, module);   
     }
    async areAllDependenciesOfModulePresentAsync(unsorted: IModuleInfoExtended[], module: IModuleInfoExtended): Promise<IModuleInfoExtended[]> {
        return await invokeAsync('AreAllDependenciesOfModulePresent', unsorted, module);   
    }

    getDependentModulesOf(source: IModuleInfoExtended[], module: IModuleInfoExtended, skipExternalDependencies: boolean): IModuleInfoExtended[] {
        return invoke('GetDependentModulesOf', source, module, skipExternalDependencies);
    }
    async getDependentModulesOfAsync(source: IModuleInfoExtended[], module: IModuleInfoExtended, skipExternalDependencies: boolean): Promise<IModuleInfoExtended[]> {
        return await invokeAsync('GetDependentModulesOf', source, module, skipExternalDependencies);
    }
    
    getModuleInfo(xml: string): IModuleInfoExtended {
        return invoke('GetModuleInfo', xml);
    }
    async getModuleInfoAsync(xml: string): Promise<IModuleInfoExtended> {
        return await invokeAsync('GetModuleInfo', xml);
    }
    
    getSubModuleInfo(xml: string): IModuleInfoExtended {
        return invoke('GetSubModuleInfo', xml);
    }
    async getSubModuleInfoAsync(xml: string): Promise<IModuleInfoExtended> {
        return await invokeAsync('GetSubModuleInfo', xml);
    }
}