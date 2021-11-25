import { BootData } from "./data/BootData";
import { BootStatus } from "./data/BootStatus";

declare function getBootStatus(): BootStatus;
declare function boot(bootData: BootData): Promise<void>;
declare function terminate(): Promise<void>;
