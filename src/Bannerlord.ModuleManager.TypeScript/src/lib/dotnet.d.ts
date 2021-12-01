export declare const BannerlordModuleManager: {
    Sort: (source: any) => any,
    AreAllDependenciesOfModulePresent: (source: any, module: any) => boolean,
    GetDependentModulesOf: (source: any, module: any, skipExternalDependencies: boolean) => any,
    GetModuleInfo: (xmlContent: string) => any,
    GetSubModuleInfo: (xmlContent: string) => any,
};
export interface Assembly {
    name: string;
    data: Uint8Array | string;
}
export declare function initializeMono(assemblies: Assembly[]): void;
export declare function callEntryPoint(assemblyName: string): Promise<any>;

export interface BootData {
    wasm: Uint8Array | string;
    assemblies: Assembly[];
    entryAssemblyName: string;
}
export declare enum BootStatus {
    Standby = "Standby",
    Booting = "Booting",
    Terminating = "Terminating",
    Booted = "Booted"
}
export declare function getBootStatus(): BootStatus;
export declare function boot(): Promise<void>;
export declare function terminate(): Promise<void>;
interface HeapLock {
    release(): any;
}
export declare let currentHeapLock: ManagedHeapLock | null;
export declare function assertHeapNotLocked(): void;
declare class ManagedHeapLock implements HeapLock {
    stringCache: Map<number, string | null>;
    private postReleaseActions?;
    release(): void;
}
export {};
export declare function initializeInterop(): void;
export declare const invoke: <T>(assembly: string, method: string, ...args: any[]) => T;
export declare const invokeAsync: <T>(assembly: string, method: string, ...args: any[]) => Promise<T>;
export declare const createObjectReference: (object: any) => any;
export declare const disposeObjectReference: (objectReference: any) => void;
export declare const createStreamReference: (buffer: Uint8Array | any) => any;
export interface Assembly {
    name: string;
    data: Uint8Array | string;
}
export declare function initializeMono(assemblies: Assembly[]): void;
export declare function callEntryPoint(assemblyName: string): Promise<any>;
/// <reference types="emscripten" />
export declare let wasm: DotNetModule;
export interface DotNetModule extends EmscriptenModule {
    MONO: any;
    BINDING: any;
    ccall(ident: string, returnType: Emscripten.JSType | null, argTypes: Emscripten.JSType[], args: Emscripten.TypeCompatibleWithC[], opts?: Emscripten.CCallOpts): any;
}
export declare function initializeWasm(wasmBinary: Uint8Array): Promise<void>;
export declare function destroyWasm(): void;