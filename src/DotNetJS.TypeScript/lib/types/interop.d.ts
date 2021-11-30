export declare function initializeInterop(): void;
export declare function invoke<TType>(assembly: string, method: string, ...args: any[]): TType;
export declare function invokeAsync<TType>(assembly: string, method: string, ...args: any[]): Promise<TType>;
export declare function createObjectReference(object: any): any;
export declare function disposeObjectReference(objectReference: any): void;
export declare function createStreamReference(buffer: Uint8Array | any): any;
