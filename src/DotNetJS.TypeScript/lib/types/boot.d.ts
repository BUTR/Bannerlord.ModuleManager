import { BootData, BootStatus } from "./data";

export declare function getBootStatus(): BootStatus;
export declare function boot(bootData: BootData): Promise<void>;
export declare function terminate(): Promise<void>;
