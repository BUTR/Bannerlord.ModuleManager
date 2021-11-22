/*
 * Generated type guards for "ISubModuleInfoExtended.ts".
 * WARNING: Do not manually change this file.
 */
import { ISubModuleInfoExtended } from "./index";

export function isISubModuleInfoExtended(obj: any, _argumentName?: string): obj is ISubModuleInfoExtended {
    return (
        (obj !== null &&
            typeof obj === "object" ||
            typeof obj === "function") &&
        typeof obj.name === "string" &&
        typeof obj.dllName === "string" &&
        Array.isArray(obj.assemblies) &&
        obj.assemblies.every((e: any) =>
            typeof e === "string"
        ) &&
        typeof obj.subModuleClassType === "string" &&
        (obj.tags !== null &&
            typeof obj.tags === "object" ||
            typeof obj.tags === "function")
    )
}
