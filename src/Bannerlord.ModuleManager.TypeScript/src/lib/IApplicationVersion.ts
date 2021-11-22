import { ApplicationVersionType } from './index';

/** @see {isIApplicationVersion} ts-auto-guard:type-guard */
export interface IApplicationVersion {
  applicationVersionType: ApplicationVersionType;
  major: number;
  minor: number;
  revision: number;
  changeSet: number;
}
