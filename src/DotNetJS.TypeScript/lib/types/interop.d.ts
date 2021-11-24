//import { ObjectReference } from "./data/ObjectReference";

export declare function initializeInterop(): void;
export declare const invoke: <TType>(assembly: string, method: string, ...args: any[]) => TType;
export declare const invokeAsync: <TType>(assembly: string, method: string, ...args: any[]) => Promise<TType>;
//export declare const createObjectReference: (object: any) => ObjectReference;
//export declare const disposeObjectReference: (objectReference: ObjectReference) => void;
export declare const createObjectReference: (object: any) => any;
export declare const disposeObjectReference: (objectReference: any) => void;
export declare const createStreamReference: (buffer: Uint8Array | any) => any;
