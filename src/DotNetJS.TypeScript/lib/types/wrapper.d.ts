import { getBootStatus, terminate } from "./boot";
import { BootStatus } from "./data/BootStatus";
import { createObjectReference, createStreamReference, disposeObjectReference, invoke as invokeAssembly, invokeAsync as invokeAssemblyAsync } from "./interop";

declare function boot(): Promise<void>;
declare function invoke<TType>(method: string, ...args: any[]): TType;
declare function invokeAsync<TType>(method: string, ...args: any[]): Promise<TType>;

export interface DotNetWasmWrapper {
  BootStatus: typeof BootStatus;
  getBootStatus: typeof getBootStatus;
  boot: typeof boot;
  terminate: typeof terminate;

  createObjectReference: typeof createObjectReference;
  disposeObjectReference: typeof disposeObjectReference;
  createStreamReference: typeof createStreamReference;

  invoke: typeof invoke;
  invokeAsync: typeof invokeAsync;

  invokeAssembly: typeof invokeAssembly;
  invokeAssemblyAsync: typeof invokeAssemblyAsync;
}

declare const wrapper: DotNetWasmWrapper;
export { 
  BootStatus,
  getBootStatus,
  boot,
  terminate,
  createObjectReference,
  disposeObjectReference,
  createStreamReference,
  invoke,
  invokeAsync,
  invokeAssembly,
  invokeAssemblyAsync
};
export default wrapper;
