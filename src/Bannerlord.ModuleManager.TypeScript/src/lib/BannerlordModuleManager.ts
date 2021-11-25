import wrapper from '@butr/blmodulemanager-raw';

import { IModuleInfoExtended, isIModuleInfoExtended, ISubModuleInfoExtended, isISubModuleInfoExtended } from './index';

export class BannerlordModuleManager {
  static async createAsync(): Promise<BannerlordModuleManager> {
    const bmm = new BannerlordModuleManager();
    await bmm.init();
    return bmm;
  }
  private async init() {
    await wrapper.boot();
  }

  sort(unsorted: readonly IModuleInfoExtended[]): IModuleInfoExtended[] | null {
    const result = wrapper.invoke<IModuleInfoExtended[]>('Sort', unsorted);
    if (Array.isArray(result) && result.every((x) => isIModuleInfoExtended(x))) {
      return result;
    }
    return null;
  }

  areAllDependenciesOfModulePresent(unsorted: readonly IModuleInfoExtended[], module: IModuleInfoExtended): boolean | null {
    const result = wrapper.invoke<boolean>('AreAllDependenciesOfModulePresent', unsorted, module);
    if (typeof result === 'boolean') {
      return result;
    }
    return null;
  }

  getDependentModulesOf(source: readonly IModuleInfoExtended[], module: IModuleInfoExtended, skipExternalDependencies: boolean): readonly IModuleInfoExtended[] | null {
    const result = wrapper.invoke<readonly IModuleInfoExtended[]>('GetDependentModulesOf', source, module, skipExternalDependencies);
    if (Array.isArray(result) && result.every((x) => isIModuleInfoExtended(x))) {
      return result;
    }
    return null;
  }

  getModuleInfo(xml: string): IModuleInfoExtended | null {
    const result = wrapper.invoke<IModuleInfoExtended>('GetModuleInfo', xml);
    if (isIModuleInfoExtended(result)) {
      return result;
    }
    return null;
  }

  getSubModuleInfo(xml: string): ISubModuleInfoExtended | null {
    const result = wrapper.invoke<ISubModuleInfoExtended>('GetSubModuleInfo', xml);
    if (isISubModuleInfoExtended(result)) {
      return result;
    }
    return null;
  }
}
