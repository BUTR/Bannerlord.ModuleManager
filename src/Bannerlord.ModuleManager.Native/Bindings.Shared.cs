using BUTR.NativeAOT.Shared;

using System;
using System.Runtime.InteropServices;

namespace Bannerlord.ModuleManager.Native
{
    public static unsafe partial class Bindings
    {
        [UnmanagedCallersOnly(EntryPoint = "alloc")]
        public static void* Alloc(nuint size)
        {
            Logger.LogInput(size);
            try
            {
                var result = Allocator.Alloc(size, true);

                Logger.LogOutputPrimitive((int) result);
                return result;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return null;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "dealloc")]
        public static void Dealloc(param_ptr* ptr)
        {
            Logger.LogInput(ptr);
            try
            {
                Allocator.Free(ptr, true);

                Logger.LogOutput();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "alloc_alive_count")]
        public static int AllocAliveCount()
        {
            Logger.LogInput();
            try
            {
                var result = Allocator.GetCurrentAllocations();

                Logger.LogOutputPrimitive(result);
                return result;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return -1;
            }
        }
    }
}