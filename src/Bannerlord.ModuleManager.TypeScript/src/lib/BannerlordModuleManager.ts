import { BannerlordModuleManager as blmmanager, boot, BootStatus, getBootStatus } from "./dotnet";
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
        return blmmanager.Sort(unsorted);   
    }

    areAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], module: ModuleInfoExtended): ModuleInfoExtended[] {
        return blmmanager.AreAllDependenciesOfModulePresent(unsorted, module);   
     }

    getDependentModulesOf(source: ModuleInfoExtended[], module: ModuleInfoExtended, skipExternalDependencies: boolean): ModuleInfoExtended[] {
        return blmmanager.GetDependentModulesOf(source, module, skipExternalDependencies);
    }
    
    getModuleInfo(xml: string): ModuleInfoExtended {
        return blmmanager.GetModuleInfo(xml);
    }
    
    getSubModuleInfo(xml: string): ModuleInfoExtended {
        return blmmanager.GetSubModuleInfo(xml);
    }
}