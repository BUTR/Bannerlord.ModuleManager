/*
 * Generated type guards for "IDependentModuleMetadata.ts".
 * WARNING: Do not manually change this file.
 */
import { IDependentModuleMetadata, isLoadType, isIApplicationVersion, isIApplicationVersionRange } from "./index";

export function isIDependentModuleMetadata(obj: any, _argumentName?: string): obj is IDependentModuleMetadata {
    return (
        (obj !== null &&
            typeof obj === "object" ||
            typeof obj === "function") &&
        typeof obj.id === "string" &&
        isLoadType(obj.loadType) as boolean &&
        typeof obj.isOptional === "boolean" &&
        typeof obj.isIncompatible === "boolean" &&
        isIApplicationVersion(obj.version) as boolean &&
        isIApplicationVersionRange(obj.versionRange) as boolean
    )
}
