import { Assembly } from "./Assembly";

export declare type BootData = {
    wasm: Uint8Array | string;
    assemblies: Assembly[];
    entryAssemblyName: string;
}
