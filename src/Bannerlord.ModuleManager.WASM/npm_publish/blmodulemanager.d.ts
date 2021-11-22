 declare module '@butr/blmodulemanager-raw' {
  export function boot(): Promise<void>;

  export function invoke(method: string, ...args: unknown[]): void;
  export function invoke<TType>(method: string, ...array: unknown[]): TType;
  export function invokeAsync<TType>(method: string, ...array: unknown[]): Promise<TType>;
  export function invokeAsync(method: string, ...args: unknown[]): Promise<void>;

  export function invokeAssembly(assembly: string, method: string, ...args: unknown[]): void;
  export function invokeAssembly<TType>(assembly: string, method: string, ...args: unknown[]): TType;
  export function invokeAssemblyAsync(assembly: string, method: string, ...args: unknown[]): Promise<void>;
  export function invokeAssemblyAsync<TType>(assembly: string, method: string, ...args: unknown[]): Promise<TType>;
}
