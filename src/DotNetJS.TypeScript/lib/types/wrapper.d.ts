import { invoke as invokeAssembly, invokeAsync as invokeAssemblyAsync } from "./interop";

declare function boot(): Promise<void>;
declare function invoke<TType>(method: string, ...args: any[]): TType;
declare function invokeAsync<TType>(method: string, ...args: any[]): Promise<TType>;

export interface DotNetWasmWrapper {
  boot: typeof boot;

  invoke: typeof invoke;
  invokeAsync: typeof invokeAsync;

  invokeAssembly: typeof invokeAssembly;
  invokeAssemblyAsync: typeof invokeAssemblyAsync;
}

export declare const wrapper: DotNetWasmWrapper;
export { boot, invoke, invokeAsync, invokeAssembly, invokeAssemblyAsync };
