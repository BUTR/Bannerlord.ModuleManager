import { ModuleInfoExtended } from "./types";

export declare type BannerlordModuleManagerType = {
    Sort(unsorted: ModuleInfoExtended[]): ModuleInfoExtended[];
    AreAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], module: ModuleInfoExtended): ModuleInfoExtended[];
    GetDependentModulesOf(source: ModuleInfoExtended[], module: ModuleInfoExtended, skipExternalDependencies: boolean): ModuleInfoExtended[];
    GetModuleInfo(xml: string): ModuleInfoExtended;
    GetSubModuleInfo(xml: string): ModuleInfoExtended;
}

export declare const BannerlordModuleManager: BannerlordModuleManagerType;
export * from "@butr/dotnet-runtime-ts/lib/types/wrapper"