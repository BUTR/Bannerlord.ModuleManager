/*
 * Generated type guards for "IDependentModule.ts".
 * WARNING: Do not manually change this file.
 */
import { IDependentModule, isIApplicationVersion } from "./index";

export function isIDependentModule(obj: any, _argumentName?: string): obj is IDependentModule {
    return (
        (obj !== null &&
            typeof obj === "object" ||
            typeof obj === "function") &&
        typeof obj.id === "string" &&
        isIApplicationVersion(obj.version) as boolean
    )
}
