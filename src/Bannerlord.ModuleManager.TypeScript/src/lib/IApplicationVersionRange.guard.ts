/*
 * Generated type guards for "IApplicationVersionRange.ts".
 * WARNING: Do not manually change this file.
 */
import { IApplicationVersionRange, isIApplicationVersion } from "./index";

export function isIApplicationVersionRange(obj: any, _argumentName?: string): obj is IApplicationVersionRange {
    return (
        (obj !== null &&
            typeof obj === "object" ||
            typeof obj === "function") &&
        isIApplicationVersion(obj.min) as boolean &&
        isIApplicationVersion(obj.max) as boolean
    )
}
