import { boot, invoke } from '@butr/blmodulemanager-raw';

import { IModuleInfoExtended, isIModuleInfoExtended, ISubModuleInfoExtended, isISubModuleInfoExtended } from './index';

export class BannerlordModuleManager {
  static async createAsync(): Promise<BannerlordModuleManager> {
    const bmm = new BannerlordModuleManager();
    await bmm.init();
    return bmm;
  }
  private async init() {
    await boot();
  }

  sort(unsorted: readonly IModuleInfoExtended[]): IModuleInfoExtended[] | null {
    const result = invoke<IModuleInfoExtended[]>('Sort', unsorted);
    if (Array.isArray(result) && result.every((x) => isIModuleInfoExtended(x))) {
      return result;
    }
    return null;
  }

  areAllDependenciesOfModulePresent(unsorted: readonly IModuleInfoExtended[], module: IModuleInfoExtended): boolean | null {
    const result = invoke<boolean>('AreAllDependenciesOfModulePresent', unsorted, module);
    if (typeof result === 'boolean') {
      return result;
    }
    return null;
  }

  getDependentModulesOf(source: readonly IModuleInfoExtended[], module: IModuleInfoExtended, skipExternalDependencies: boolean): readonly IModuleInfoExtended[] | null {
    const result = invoke<readonly IModuleInfoExtended[]>('GetDependentModulesOf', source, module, skipExternalDependencies);
    if (Array.isArray(result) && result.every((x) => isIModuleInfoExtended(x))) {
      return result;
    }
    return null;
  }

  getModuleInfo(xml: string): IModuleInfoExtended | null {
    const result = invoke<IModuleInfoExtended>('GetModuleInfo', xml);
    if (isIModuleInfoExtended(result)) {
      return result;
    }
    return null;
  }

  getSubModuleInfo(xml: string): ISubModuleInfoExtended | null {
    const result = invoke<ISubModuleInfoExtended>('GetSubModuleInfo', xml);
    if (isISubModuleInfoExtended(result)) {
      return result;
    }
    return null;
  }
}
