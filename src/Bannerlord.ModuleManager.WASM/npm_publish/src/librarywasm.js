import { boot, invoke, invokeAsync } from "./blmodulemanager"

export class LibraryWasm {
    static async createAsync() {
        return await new LibraryWasm().init();
    }
    async init() {
        await boot();
        return this;
    }

    sort(unsorted) {
        return invoke('Sort', unsorted);   
     }
     async sortAsync(unsorted) {
         return await invokeAsync('Sort', unsorted);   
     }

    areAllDependenciesOfModulePresent(unsorted, module) {
       return invoke('AreAllDependenciesOfModulePresent', unsorted, module);   
    }
    async areAllDependenciesOfModulePresentAsync(unsorted, module) {
        return await invokeAsync('AreAllDependenciesOfModulePresent', unsorted, module);   
    }

    getDependentModulesOf(source, module, skipExternalDependencies) {
        return invoke('GetDependentModulesOf', source, module, skipExternalDependencies);
    }
    async getDependentModulesOfAsync(source, module, skipExternalDependencies) {
        return await invokeAsync('GetDependentModulesOf', source, module, skipExternalDependencies);
    }
    
    getModuleInfo(xml) {
        return invoke('GetModuleInfo', xml);
    }
    async getModuleInfoAsync(xml) {
        return await invokeAsync('GetModuleInfo', xml);
    }
    
    getSubModuleInfo(xml) {
        return invoke('GetSubModuleInfo', xml);
    }
    async getSubModuleInfoAsync(xml) {
        return await invokeAsync('GetSubModuleInfo', xml);
    }
}
  