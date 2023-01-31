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
                var result = Allocator.Alloc(size);

                Logger.LogOutput(new IntPtr(result).ToString("x16"), nameof(Alloc));
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
            Logger.LogInput(new IntPtr(ptr).ToString("x16"), nameof(Dealloc));
            try
            {
                Allocator.Free(ptr);

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

                Logger.LogOutput(result);
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