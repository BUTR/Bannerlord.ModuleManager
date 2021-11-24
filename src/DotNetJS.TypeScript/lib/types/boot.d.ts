import { BootData } from "./data/BootData";
import { BootStatus } from "./data/BootStatus";

export declare function getBootStatus(): BootStatus;
export declare function boot(bootData: BootData): Promise<void>;
export declare function terminate(): Promise<void>;
