import { IApplicationVersion, IDependentModule, ISubModuleInfo } from './index';

/** @see {isIModuleInfo} ts-auto-guard:type-guard */
export interface IModuleInfo {
  id: string;
  name: string;
  isOfficial: boolean;
  version: IApplicationVersion;
  isSingleplayerModule: boolean;
  isMultiplayerModule: boolean;
  subModules: readonly ISubModuleInfo[];
  dependentModules: readonly IDependentModule[];
}
