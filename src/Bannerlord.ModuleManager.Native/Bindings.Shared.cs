using BUTR.NativeAOT.Shared;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bannerlord.ModuleManager.Native;

public static unsafe partial class Bindings
{
    [UnmanagedCallersOnly(EntryPoint = "common_alloc", CallConvs = new[] { typeof(CallConvCdecl) }), IsNotConst<IsPtrConst>]
    public static void* CommonAlloc(nuint size)
    {
        Logger.LogInput(size);
        try
        {
            var result = Allocator.Alloc(size);

            Logger.LogOutput(new IntPtr(result).ToString("x16"), nameof(CommonAlloc));
            return result;
        }
        catch (Exception e)
        {
            Logger.LogException(e);
            return null;
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "common_dealloc", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static void Common_Dealloc([IsConst<IsPtrConst>] param_ptr* ptr)
    {
        Logger.LogInput(new IntPtr(ptr).ToString("x16"), nameof(Common_Dealloc));
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

    [UnmanagedCallersOnly(EntryPoint = "common_alloc_alive_count", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static int CommonAllocAliveCount()
    {
        Logger.LogInput();
        try
        {
#if TRACK_ALLOCATIONS
            var result = Allocator.GetCurrentAllocations();
#else
                var result = 0;
#endif

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