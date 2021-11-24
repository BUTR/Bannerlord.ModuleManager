import { boot, getBootStatus, terminate } from "./boot";
import { BootStatus } from "./data/BootStatus";
import { BootData } from "./data/BootData";
import { invoke, invokeAsync, createObjectReference, disposeObjectReference, createStreamReference } from "./interop";
import { Assembly } from "./data/Assembly";

export interface DotNetRuntime {
    BootStatus: typeof BootStatus;
    getBootStatus: typeof getBootStatus;
    boot: typeof boot;
    terminate: typeof terminate;
    invoke: typeof invoke;
    invokeAsync: typeof invokeAsync;
    createObjectReference: typeof createObjectReference;
    disposeObjectReference: typeof disposeObjectReference;
    createStreamReference: typeof createStreamReference;
}

export declare const dotnet: DotNetRuntime;
export { BootStatus, BootData, Assembly };
export { boot, getBootStatus, terminate, invoke, invokeAsync, createObjectReference, disposeObjectReference, createStreamReference };
