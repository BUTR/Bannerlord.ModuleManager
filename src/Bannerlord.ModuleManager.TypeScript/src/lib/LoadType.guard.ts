/*
 * Generated type guards for "LoadType.ts".
 * WARNING: Do not manually change this file.
 */
import { LoadType } from "./index";

export function isLoadType(obj: any, _argumentName?: string): obj is LoadType {
    return (
        (obj === LoadType.None ||
            obj === LoadType.LoadAfterThis ||
            obj === LoadType.LoadBeforeThis)
    )
}
