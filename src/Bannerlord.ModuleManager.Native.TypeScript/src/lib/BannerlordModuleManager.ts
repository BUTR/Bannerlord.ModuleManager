import {
    ApplicationVersion,
    IBannerlordModuleManager,
    IEnableDisableManager,
    IValidationManager,
    ModuleInfoExtended,
    ModuleIssue,
    ModuleSorterOptions,
    SubModuleInfoExtended
} from "./types";

const blmanager: IBannerlordModuleManager = require('./../../blmanager.node');

export class BannerlordModuleManager implements IBannerlordModuleManager {
    static async createAsync(): Promise<BannerlordModuleManager> {
        const lib = new BannerlordModuleManager();
        return lib;
    }

    private constructor() { }

    sort(unsorted: ModuleInfoExtended[]): ModuleInfoExtended[] {
        return blmanager.sort(unsorted);
    }

    sortWithOptions(unsorted: ModuleInfoExtended[], options: ModuleSorterOptions): ModuleInfoExtended[] {
        return blmanager.sortWithOptions(unsorted, options);
    }

    areAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], targetModule: ModuleInfoExtended): boolean {
        return blmanager.areAllDependenciesOfModulePresent(unsorted, targetModule);
    }

    getDependentModulesOf(source: ModuleInfoExtended[], targetModule: ModuleInfoExtended): ModuleInfoExtended[] {
        return blmanager.getDependentModulesOf(source, targetModule);
    }

    getDependentModulesOfWithOptions(source: ModuleInfoExtended[], targetModule: ModuleInfoExtended, options: ModuleSorterOptions): ModuleInfoExtended[] {
        return blmanager.getDependentModulesOfWithOptions(source, targetModule, options);
    }

    validateLoadOrder(source: ModuleInfoExtended[], targetModule: ModuleInfoExtended): ModuleIssue[] {
        return blmanager.validateLoadOrder(source, targetModule);
    }

    validateModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IValidationManager): ModuleIssue[] {
        return blmanager.validateModule(modules, targetModule, manager);
    }

    enableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): void {
        blmanager.enableModule(modules, targetModule, manager);
    }

    disableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): void {
        blmanager.disableModule(modules, targetModule, manager);
    }

    getModuleInfo(xml: string): ModuleInfoExtended | undefined {
        return blmanager.getModuleInfo(xml);
    }

    getSubModuleInfo(xml: string): SubModuleInfoExtended | undefined {
        return blmanager.getSubModuleInfo(xml);
    }

    compareVersions(x: ApplicationVersion, y: ApplicationVersion): number {
        return blmanager.compareVersions(x, y);
    }

    async dispose(): Promise<void> {
    }
}