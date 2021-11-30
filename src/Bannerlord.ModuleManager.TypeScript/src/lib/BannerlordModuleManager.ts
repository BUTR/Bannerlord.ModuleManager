import { boot, BootStatus, getBootStatus } from "./dotnet";
import dotnet from "./dotnet";
import { ModuleInfoExtended } from "./types";

export class BannerlordModuleManager {
    static async createAsync(): Promise<BannerlordModuleManager> {
        const lib = new BannerlordModuleManager();
        await lib.init();
        return lib;
    }
    private async init(): Promise<void> {
        const status = getBootStatus();
        if (status == BootStatus.Standby){
            await boot();
        }
    }

    sort(unsorted: ModuleInfoExtended[]): ModuleInfoExtended[] {
        return dotnet.dotnet.BannerlordModuleManager.Sort(unsorted);   
    }

    areAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], module: ModuleInfoExtended): boolean {
        return dotnet.dotnet.BannerlordModuleManager.AreAllDependenciesOfModulePresent(unsorted, module);   
     }

    getDependentModulesOf(source: ModuleInfoExtended[], module: ModuleInfoExtended, skipExternalDependencies: boolean): ModuleInfoExtended[] {
        return dotnet.dotnet.BannerlordModuleManager.GetDependentModulesOf(source, module, skipExternalDependencies);
    }
    
    getModuleInfo(xml: string): ModuleInfoExtended {
        return dotnet.dotnet.BannerlordModuleManager.GetModuleInfo(xml);
    }
    
    getSubModuleInfo(xml: string): ModuleInfoExtended {
        return dotnet.dotnet.BannerlordModuleManager.GetSubModuleInfo(xml);
    }
}