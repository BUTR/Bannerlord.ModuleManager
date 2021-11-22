/*
 * Generated type guards for "IModuleInfoExtended.ts".
 * WARNING: Do not manually change this file.
 */
import { IModuleInfoExtended, isIModuleInfo, isIDependentModuleMetadata } from "./index";

export function isIModuleInfoExtended(obj: any, _argumentName?: string): obj is IModuleInfoExtended {
    return (
        isIModuleInfo(obj) as boolean &&
        typeof obj.url === "string" &&
        Array.isArray(obj.dependentModuleMetadatas) &&
        obj.dependentModuleMetadatas.every((e: any) =>
            isIDependentModuleMetadata(e) as boolean
        )
    )
}
