import { boot, BootStatus, getBootStatus, terminate } from "./dotnet";
import dotnet from "./dotnet";
import { ApplicationVersion, ModuleInfoExtended, ModuleSorterOptions } from "./types";

export class BannerlordModuleManager {
    static async createAsync(): Promise<BannerlordModuleManager> {
        const lib = new BannerlordModuleManager();
        await lib.init();
        return lib;
    }
    private async init(): Promise<void> {
        const status = getBootStatus();
        if (status == BootStatus.Standby) {
            await boot();
        }
    }

    sort(unsorted: ModuleInfoExtended[]): ModuleInfoExtended[] {
        return dotnet.BannerlordModuleManager.Sort(unsorted);
    }

    sortWithOptions(unsorted: ModuleInfoExtended[], options: ModuleSorterOptions): ModuleInfoExtended[] {
        return dotnet.BannerlordModuleManager.SortWithOptions(unsorted, options);
    }

    areAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], module: ModuleInfoExtended): boolean {
        return dotnet.BannerlordModuleManager.AreAllDependenciesOfModulePresent(unsorted, module);
    }

    getDependentModulesOf(source: ModuleInfoExtended[], module: ModuleInfoExtended): ModuleInfoExtended[] {
        return dotnet.BannerlordModuleManager.GetDependentModulesOf(source, module);
    }

    getDependentModulesOfWithOptions(source: ModuleInfoExtended[], module: ModuleInfoExtended, options: ModuleSorterOptions): ModuleInfoExtended[] {
        return dotnet.BannerlordModuleManager.GetDependentModulesOfWithOptions(source, module, options);
    }

    getModuleInfo(xml: string): ModuleInfoExtended {
        return dotnet.BannerlordModuleManager.GetModuleInfo(xml);
    }

    getSubModuleInfo(xml: string): ModuleInfoExtended {
        return dotnet.BannerlordModuleManager.GetSubModuleInfo(xml);
    }

    compareVersions(x: ApplicationVersion, y: ApplicationVersion): number {
        return dotnet.BannerlordModuleManager.CompareVersions(x, y);
    }

    async dispose(): Promise<void> {
        await terminate();
    }
}