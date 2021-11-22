import { IApplicationVersion } from './index';

/** @see {isIDependentModule} ts-auto-guard:type-guard */
export interface IDependentModule {
  id: string;
  version: IApplicationVersion;
}
