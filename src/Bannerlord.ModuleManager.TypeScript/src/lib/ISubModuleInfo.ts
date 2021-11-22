import { SubModuleTags } from './index';

/** @see {isISubModuleInfo} ts-auto-guard:type-guard */
export interface ISubModuleInfo {
  name: string;
  dllName: string;
  assemblies: readonly string[];
  subModuleClassType: string;
  tags: {
    [index: string]: SubModuleTags;
  };
}
