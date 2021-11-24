import { boot } from "./boot";
import { invoke, invokeAsync } from "./interop";

export interface DotNetWasmWrapper {
  boot: typeof boot;

  invoke: <TType>(method: string, ...args: any[]) => TType;
  invokeAsync: <TType>(method: string, ...args: any[]) => Promise<TType>;

  invokeAssembly: typeof invoke;
  invokeAssemblyAsync: typeof invokeAsync;
}