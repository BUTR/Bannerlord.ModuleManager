import { boot, BootStatus, getBootStatus, terminate, Bannerlord } from "./dotnet";
import dotnet from "./dotnet";
import ModuleInfoExtended = Bannerlord.ModuleManager.ModuleInfoExtended;
import SubModuleInfoExtended = Bannerlord.ModuleManager.SubModuleInfoExtended;
import ModuleSorterOptions = Bannerlord.ModuleManager.ModuleSorterOptions;
import ApplicationVersion = Bannerlord.ModuleManager.ApplicationVersion;

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
        return dotnet.Bannerlord.ModuleManager.Sort(unsorted);
    }

    sortWithOptions(unsorted: ModuleInfoExtended[], options: ModuleSorterOptions): ModuleInfoExtended[] {
        return dotnet.Bannerlord.ModuleManager.SortWithOptions(unsorted, options);
    }

    areAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], module: ModuleInfoExtended): boolean {
        return dotnet.Bannerlord.ModuleManager.AreAllDependenciesOfModulePresent(unsorted, module);
    }

    getDependentModulesOf(source: ModuleInfoExtended[], module: ModuleInfoExtended): ModuleInfoExtended[] {
        return dotnet.Bannerlord.ModuleManager.GetDependentModulesOf(source, module);
    }

    getDependentModulesOfWithOptions(source: ModuleInfoExtended[], module: ModuleInfoExtended, options: ModuleSorterOptions): ModuleInfoExtended[] {
        return dotnet.Bannerlord.ModuleManager.GetDependentModulesOfWithOptions(source, module, options);
    }

    getModuleInfo(xml: string): ModuleInfoExtended | undefined {
        return dotnet.Bannerlord.ModuleManager.GetModuleInfo(xml);
    }

    getSubModuleInfo(xml: string): SubModuleInfoExtended | undefined {
        return dotnet.Bannerlord.ModuleManager.GetSubModuleInfo(xml);
    }

    compareVersions(x: ApplicationVersion, y: ApplicationVersion): number {
        return dotnet.Bannerlord.ModuleManager.CompareVersions(x, y);
    }

    async dispose(): Promise<void> {
        await terminate();
    }
}