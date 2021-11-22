import { IApplicationVersion } from './index';

/** @see {isIApplicationVersionRange} ts-auto-guard:type-guard */
export interface IApplicationVersionRange {
  min: IApplicationVersion;
  max: IApplicationVersion;
}
