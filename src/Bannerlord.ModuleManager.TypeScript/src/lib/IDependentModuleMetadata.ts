import { IApplicationVersion, IApplicationVersionRange, LoadType } from './index';

/** @see {isIDependentModuleMetadata} ts-auto-guard:type-guard */
export interface IDependentModuleMetadata {
  id: string;
  loadType: LoadType;
  isOptional: boolean;
  isIncompatible: boolean;
  version: IApplicationVersion;
  versionRange: IApplicationVersionRange;
}
