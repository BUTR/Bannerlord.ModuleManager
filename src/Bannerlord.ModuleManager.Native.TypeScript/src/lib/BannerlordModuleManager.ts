import * as types from "./types";

const blmanager: types.IBannerlordModuleManager & { allocAliveCount(): number; } = require('./../../blmanager.node');

export const allocAliveCount = (): number => {
    return blmanager.allocAliveCount();
}

export class BannerlordModuleManager {
    /* istanbul ignore next */
    private constructor() { }

    static sort(unsorted: types.ModuleInfoExtended[]): types.ModuleInfoExtended[] {
        return blmanager.sort(unsorted);
    }

    static sortWithOptions(unsorted: types.ModuleInfoExtended[], options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
        return blmanager.sortWithOptions(unsorted, options);
    }

    static areAllDependenciesOfModulePresent(unsorted: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): boolean {
        return blmanager.areAllDependenciesOfModulePresent(unsorted, module);
    }

    static getDependentModulesOf(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended): types.ModuleInfoExtended[] {
        return blmanager.getDependentModulesOf(source, module);
    }

    static getDependentModulesOfWithOptions(source: types.ModuleInfoExtended[], module: types.ModuleInfoExtended, options: types.ModuleSorterOptions): types.ModuleInfoExtended[] {
        return blmanager.getDependentModulesOfWithOptions(source, module, options);
    }

    static validateLoadOrder(source: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended): types.ModuleIssue[] {
        return blmanager.validateLoadOrder(source, targetModule);
    }

    static validateModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IValidationManager): types.ModuleIssue[] {
        return blmanager.validateModule(modules, targetModule, manager);
    }

    static enableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): void {
        return blmanager.enableModule(modules, targetModule, manager);
    }

    static disableModule(modules: types.ModuleInfoExtended[], targetModule: types.ModuleInfoExtended, manager: types.IEnableDisableManager): void {
        return blmanager.disableModule(modules, targetModule, manager);
    }

    static getModuleInfo(xml: string): types.ModuleInfoExtended | undefined {
        return blmanager.getModuleInfo(xml);
    }

    static getSubModuleInfo(xml: string): types.SubModuleInfoExtended | undefined {
        return blmanager.getSubModuleInfo(xml);
    }

    static compareVersions(x: types.ApplicationVersion, y: types.ApplicationVersion): number {
        return blmanager.compareVersions(x, y);
    }
  
    static getDependenciesAll(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        return blmanager.getDependenciesAll(module);
    }
    static getDependenciesToLoadBeforeThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        return blmanager.getDependenciesToLoadBeforeThis(module);
    }
    static getDependenciesToLoadAfterThis(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        return blmanager.getDependenciesToLoadAfterThis(module);
    }
    static getDependenciesIncompatibles(module: types.ModuleInfoExtended): types.DependentModuleMetadata[] {
        return blmanager.getDependenciesIncompatibles(module);
    }
}