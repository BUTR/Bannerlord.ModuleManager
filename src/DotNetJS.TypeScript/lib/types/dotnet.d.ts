import { BootStatus, BootData, Assembly } from "./data";
import { boot, getBootStatus, terminate } from "./boot";
import { invoke, invokeAsync, createObjectReference, disposeObjectReference, createStreamReference } from "./interop";

export declare type DotNetRuntime = {
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

export {
    BootStatus,
    BootData,
    Assembly
};

export { 
    boot,
    getBootStatus,
    terminate,
    invoke,
    invokeAsync,
    createObjectReference,
    disposeObjectReference,
    createStreamReference
};
