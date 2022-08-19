import { Library, Callback } from 'ffi-napi';
import { Bannerlord } from './types';
import ModuleInfoExtended = Bannerlord.ModuleManager.ModuleInfoExtended;
import SubModuleInfoExtended = Bannerlord.ModuleManager.SubModuleInfoExtended;
import ModuleSorterOptions = Bannerlord.ModuleManager.ModuleSorterOptions;
import ApplicationVersion = Bannerlord.ModuleManager.ApplicationVersion;
import ModuleIssue = Bannerlord.ModuleManager.ModuleIssue;

const wrapper = Library(`${__dirname}\\Bannerlord.ModuleManager.Native.dll`, {
  "sort": ["string", ["string"]],
  "sortWithOptions": ["string", ["string", "string"]],
  "areAllDependenciesOfModulePresent": ["bool", ["string", "string"]],
  "validateModuleDependenciesDeclarations": ["string", ["string"]],
  "validateModule": ["string", ["string", "string", "pointer"]],
  "enableModule": ["string", ["string", "string", "pointer", "pointer", "pointer", "pointer"]],
  "disableModule": ["string", ["string", "string", "pointer", "pointer", "pointer", "pointer"]],
  "getDependentModulesOf": ["string", ["string", "string"]],
  "getDependentModulesOfWithOptions": ["string", ["string", "string", "string"]],
  "getSubModuleInfo": ["string", ["string"]],
  "getModuleInfo": ["string", ["string"]],
  "compareVersions": ["int", ["string", "string"]],
});

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

  escapeNonLatin(str: string) {
    return [...str].map(c => /^[\x00-\x7F]$/.test(c) ? c : c.split("").map(a => "\\u" + a.charCodeAt(0).toString(16).padStart(4, "0")).join("")).join("");
  }

  unescapeNonLatin(str: string) {
    return str.replace(new RegExp("\\\\\\\\u(?<Value>[a-zA-Z0-9]{4})", "gi"), (_, arg1) => String.fromCharCode(parseInt(arg1, 16)));
  }

  toJson<T>(obj: T): string {
    return this.escapeNonLatin(JSON.stringify(obj));
  }

  fromJson<T>(str: string | null, def: T): T {
    return str == null ? def : JSON.parse(this.unescapeNonLatin(str)) as T || def;
  }


  sort(unsorted: ModuleInfoExtended[]): ModuleInfoExtended[] {
    const response = wrapper.sort(this.toJson(unsorted));
    return this.fromJson<ModuleInfoExtended[]>(response, []);
  }

  sortWithOptions(unsorted: ModuleInfoExtended[], options: ModuleSorterOptions): ModuleInfoExtended[] {
    const response = wrapper.sortWithOptions(this.toJson(unsorted), this.toJson(options));
    return this.fromJson<ModuleInfoExtended[]>(response, []);
  }

  areAllDependenciesOfModulePresent(unsorted: ModuleInfoExtended[], module: ModuleInfoExtended): boolean {
    const response = wrapper.areAllDependenciesOfModulePresent(this.toJson(unsorted), this.toJson(module));
    return response;
  }

  getDependentModulesOf(source: ModuleInfoExtended[], module: ModuleInfoExtended): ModuleInfoExtended[] {
    const response = wrapper.getDependentModulesOf(this.toJson(source), this.toJson(module));
    return this.fromJson<ModuleInfoExtended[]>(response, []);
  }

  getDependentModulesOfWithOptions(source: ModuleInfoExtended[], module: ModuleInfoExtended, options: ModuleSorterOptions): ModuleInfoExtended[] {
    const response = wrapper.getDependentModulesOfWithOptions(this.toJson(source), this.toJson(module), this.toJson(options));
    return this.fromJson<ModuleInfoExtended[]>(response, []);
  }

  validateModuleDependenciesDeclarations(module: ModuleInfoExtended): ModuleIssue[] {
    const response = wrapper.validateModuleDependenciesDeclarations(this.toJson(module));
    return this.fromJson<ModuleIssue[]>(response, []);
  }

  validateModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IValidationManager): ModuleIssue[] {
    var isSelected = Callback('bool', ['string'], id => { return manager.isSelected(id); });
    const response = wrapper.validateModule(this.toJson(modules), this.toJson(targetModule), isSelected);
    return this.fromJson<ModuleIssue[]>(response, []);
  }

  enableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): ModuleIssue[] {
    var getSelected = Callback('bool', ['string'], id => { return manager.getSelected(id) });
    var setSelected = Callback('void', ['string', 'bool'], (id, value) => { manager.setSelected(id, value as unknown as boolean) });
    var getDisabled = Callback('bool', ['string'], id => { return manager.getDisabled(id) });
    var setDisabled = Callback('void', ['string', 'bool'], (id, value) => { manager.setDisabled(id, value as unknown as boolean) });
    const response = wrapper.enableModule(this.toJson(modules), this.toJson(targetModule), getSelected, setSelected, getDisabled, setDisabled);
    return this.fromJson<ModuleIssue[]>(response, []);
  }

  disableModule(modules: ModuleInfoExtended[], targetModule: ModuleInfoExtended, manager: IEnableDisableManager): ModuleIssue[] {
    var getSelected = Callback('bool', ['string'], id => { return manager.getSelected(id) });
    var setSelected = Callback('void', ['string', 'bool'], (id, value) => { manager.setSelected(id, value as unknown as boolean) });
    var getDisabled = Callback('bool', ['string'], id => { return manager.getDisabled(id) });
    var setDisabled = Callback('void', ['string', 'bool'], (id, value) => { manager.setDisabled(id, value as unknown as boolean) });
    const response = wrapper.disableModule(this.toJson(modules), this.toJson(targetModule), getSelected, setSelected, getDisabled, setDisabled);
    return this.fromJson<ModuleIssue[]>(response, []);
  }

  getModuleInfo(xml: string): ModuleInfoExtended | undefined {
    const response = wrapper.getModuleInfo(this.escapeNonLatin(xml));
    return response === undefined ? undefined : this.fromJson<ModuleInfoExtended | undefined>(response, undefined);
  }

  getSubModuleInfo(xml: string): SubModuleInfoExtended | undefined {
    const response = wrapper.getSubModuleInfo(this.escapeNonLatin(xml));
    return response === undefined ? undefined : this.fromJson<SubModuleInfoExtended | undefined>(response, undefined);
  }

  compareVersions(x: ApplicationVersion, y: ApplicationVersion): number {
    return wrapper.compareVersions(this.toJson(x), this.toJson(y));
  }
}