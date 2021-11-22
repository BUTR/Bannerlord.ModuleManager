/*
 * Generated type guards for "ApplicationVersionType.ts".
 * WARNING: Do not manually change this file.
 */
import { ApplicationVersionType } from "./index";

export function isApplicationVersionType(obj: any, _argumentName?: string): obj is ApplicationVersionType {
    return (
        (obj === ApplicationVersionType.Alpha ||
            obj === ApplicationVersionType.Beta ||
            obj === ApplicationVersionType.EarlyAccess ||
            obj === ApplicationVersionType.Release ||
            obj === ApplicationVersionType.Development ||
            obj === ApplicationVersionType.Invalid)
    )
}
