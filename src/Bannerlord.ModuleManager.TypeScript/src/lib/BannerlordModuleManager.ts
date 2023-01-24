import { boot, BootStatus, createObjectReference, getBootStatus, terminate, Bannerlord } from "./dotnet";
import dotnet from "./dotnet";
import ModuleInfoExtended = Bannerlord.ModuleManager.ModuleInfoExtended;
import SubModuleInfoExtended = Bannerlord.ModuleManager.SubModuleInfoExtended;
import ModuleSorterOptions = Bannerlord.ModuleManager.ModuleSorterOptions;
import ApplicationVersion = Bannerlord.ModuleManager.ApplicationVersion;
import ModuleIssue = Bannerlord.ModuleManager.ModuleIssue;

export interface IValidationManager {
    isSelected(moduleId: string): boolean,
}

export interface IEnableDisableManager {
    getSelected(moduleId: string): boolean,
    setSelected(moduleId: string, value: boolean): void,
    getDisabled(moduleId: string): boolean,
    setDisabled(moduleId: string, value: boolean): void,
}

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
        return dotnet.Bannerlord.ModuleManager.sort(unsorted);
    }

    sortWithOptions(unsorted: ModuleInfoExtended[], options: ModuleSorterOptions): ModuleInfoExtended[] {
        return dotnet.Bannerlord.ModuleManager.sortWithOptions(unsorted, options);
    }

    areAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], module: ModuleInfoExtended): boolean {
        return dotnet.Bannerlord.ModuleManager.areAllDependenciesOfModulePresent(unsorted, module);
    }

    getDependentModulesOf(source: ModuleInfoExtended[], module: ModuleInfoExtended): ModuleInfoExtended[] {
        return dotnet.Bannerlord.ModuleManager.getDependentModulesOf(source, module);
    }

    getDependentModulesOfWithOptions(source: ModuleInfoExtended[], module: ModuleInfoExtended, options: ModuleSorterOptions): ModuleInfoExtended[] {
        return dotnet.Bannerlord.ModuleManager.getDependentModulesOfWithOptions(source, module, options);
    }

    validateLoadOrder(source: ModuleInfoExtended[], module: ModuleInfoExtended): ModuleIssue[] {
        return dotnet.Bannerlord.ModuleManager.validateLoadOrder(source, module);
    }

    validateModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IValidationManager): ModuleIssue[] {
        return dotnet.Bannerlord.ModuleManager.validateModule(modules, targetModule, createObjectReference(manager));
    }

    enableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): void {
        return dotnet.Bannerlord.ModuleManager.enableModule(modules, targetModule, createObjectReference(manager));
    }

    disableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): void {
        return dotnet.Bannerlord.ModuleManager.disableModule(modules, targetModule, createObjectReference(manager));
    }

    getModuleInfo(xml: string): ModuleInfoExtended | null {
        return dotnet.Bannerlord.ModuleManager.getModuleInfo(xml);
    }

    getSubModuleInfo(xml: string): SubModuleInfoExtended | null {
        return dotnet.Bannerlord.ModuleManager.getSubModuleInfo(xml);
    }

    compareVersions(x: ApplicationVersion, y: ApplicationVersion): number {
        return dotnet.Bannerlord.ModuleManager.compareVersions(x, y);
    }

    async dispose(): Promise<void> {
        await terminate();
    }
}