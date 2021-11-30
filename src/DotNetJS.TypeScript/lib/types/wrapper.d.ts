import { getBootStatus, terminate } from "./boot";
import { BootStatus } from "./data";
import { createObjectReference, createStreamReference, disposeObjectReference } from "./interop";

declare function boot(): Promise<void>;
declare function invoke<TType>(method: string, ...args: any[]): TType;
declare function invokeAsync<TType>(method: string, ...args: any[]): Promise<TType>;

export declare type DotNetWasmWrapper = {
  BootStatus: typeof BootStatus;
  getBootStatus: typeof getBootStatus;
  boot: typeof boot;
  terminate: typeof terminate;

  createObjectReference: typeof createObjectReference;
  disposeObjectReference: typeof disposeObjectReference;
  createStreamReference: typeof createStreamReference;

  invoke: typeof invoke;
  invokeAsync: typeof invokeAsync;
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
  invokeAsync
};

export default wrapper;
