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
                var result = NativeMemory.Alloc(size);

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
                NativeMemory.Free(ptr);

                Logger.LogOutput();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }
    }
}