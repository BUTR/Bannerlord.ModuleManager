declare module '@butr/blmodulemanager-raw' {
  import { DotNetRuntime } from "@butr/dotnet-runtime-ts/lib/types/dotnet";
  import { DotNetWasmWrapper } from "@butr/dotnet-runtime-ts/lib/types/wrapper";

  export const dotnet: DotNetRuntime;
  export const wrapper: DotNetWasmWrapper;
  export default wrapper;
}