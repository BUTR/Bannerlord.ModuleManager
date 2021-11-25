import { DotNetRuntime } from "@butr/dotnet-runtime-ts/lib/types/dotnet"
import { DotNetWasmWrapper } from "@butr/dotnet-runtime-ts/lib/types/wrapper";

declare const wrapper: DotNetWasmWrapper;
declare module '@butr/blmodulemanager-raw' {
  export const dotnet: DotNetRuntime;
  export default wrapper;
}
