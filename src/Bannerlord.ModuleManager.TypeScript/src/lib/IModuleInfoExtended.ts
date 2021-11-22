import { IModuleInfo, IDependentModuleMetadata } from './index';

/** @see {isIModuleInfoExtended} ts-auto-guard:type-guard */
export interface IModuleInfoExtended extends IModuleInfo {
  url: string;
  dependentModuleMetadatas: readonly IDependentModuleMetadata[];
}
