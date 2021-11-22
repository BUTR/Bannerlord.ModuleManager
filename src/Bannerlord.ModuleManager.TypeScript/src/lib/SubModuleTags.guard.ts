/*
 * Generated type guards for "SubModuleTags.ts".
 * WARNING: Do not manually change this file.
 */
import { SubModuleTags } from "./index";

export function isSubModuleTags(obj: any, _argumentName?: string): obj is SubModuleTags {
    return (
        (obj === SubModuleTags.RejectedPlatform ||
            obj === SubModuleTags.ExclusivePlatform ||
            obj === SubModuleTags.DedicatedServerType ||
            obj === SubModuleTags.IsNoRenderModeElement ||
            obj === SubModuleTags.DependantRuntimeLibrary)
    )
}
