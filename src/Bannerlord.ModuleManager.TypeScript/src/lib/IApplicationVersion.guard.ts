/*
 * Generated type guards for "IApplicationVersion.ts".
 * WARNING: Do not manually change this file.
 */
import { IApplicationVersion, isApplicationVersionType } from "./index";

export function isIApplicationVersion(obj: any, _argumentName?: string): obj is IApplicationVersion {
    return (
        (obj !== null &&
            typeof obj === "object" ||
            typeof obj === "function") &&
        isApplicationVersionType(obj.applicationVersionType) as boolean &&
        typeof obj.major === "number" &&
        typeof obj.minor === "number" &&
        typeof obj.revision === "number" &&
        typeof obj.changeSet === "number"
    )
}
