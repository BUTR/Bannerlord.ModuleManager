/*
 * Generated type guards for "IModuleInfo.ts".
 * WARNING: Do not manually change this file.
 */
import { IModuleInfo, isIApplicationVersion, isISubModuleInfo, isIDependentModule } from "./index";

export function isIModuleInfo(obj: any, _argumentName?: string): obj is IModuleInfo {
    return (
        (obj !== null &&
            typeof obj === "object" ||
            typeof obj === "function") &&
        typeof obj.id === "string" &&
        typeof obj.name === "string" &&
        typeof obj.isOfficial === "boolean" &&
        isIApplicationVersion(obj.version) as boolean &&
        typeof obj.isSingleplayerModule === "boolean" &&
        typeof obj.isMultiplayerModule === "boolean" &&
        Array.isArray(obj.subModules) &&
        obj.subModules.every((e: any) =>
            isISubModuleInfo(e) as boolean
        ) &&
        Array.isArray(obj.dependentModules) &&
        obj.dependentModules.every((e: any) =>
            isIDependentModule(e) as boolean
        )
    )
}
