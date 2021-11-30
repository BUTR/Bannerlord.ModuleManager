import { ApplicationVersionType } from "./ApplicationVersionType";

export type ApplicationVersion = {
    applicationVersionType: ApplicationVersionType;
    major: number;
    minor: number;
    revision: number;
    changeSet: number;
}
