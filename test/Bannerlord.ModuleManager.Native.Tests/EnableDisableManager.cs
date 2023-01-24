using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;

namespace Bannerlord.ModuleManager.Native.Tests;

internal class EnableDisableManager
{
    public static unsafe return_value_bool* GetSelected(void* owner, param_string* moduleId)
    {
        var manager = Unsafe.AsRef<EnableDisableManager>(owner);
        return return_value_bool.AsValue(manager.GetSelected(Utils2.ToSpan(moduleId).ToString()));
    }
    public static unsafe return_value_void* SetSelected(void* owner, param_string* moduleId, bool value)
    {
        var manager = Unsafe.AsRef<EnableDisableManager>(owner);
        manager.SetSelected(Utils2.ToSpan(moduleId).ToString(), value);
        return return_value_void.AsValue();
    }
    public static unsafe return_value_bool* GetDisabled(void* owner, param_string* moduleId)
    {
        var manager = Unsafe.AsRef<EnableDisableManager>(owner);
        return return_value_bool.AsValue(manager.GetDisabled(Utils2.ToSpan(moduleId).ToString()));
    }
    public static unsafe return_value_void* SetDisabled(void* owner, param_string* moduleId, bool value)
    {
        var manager = Unsafe.AsRef<EnableDisableManager>(owner);
        manager.SetDisabled(Utils2.ToSpan(moduleId).ToString(), value);
        return return_value_void.AsValue();
    }

    public bool GetSelected(string moduleId)
    {
        return true;
    }

    public void SetSelected(string moduleId, bool value)
    {

    }

    public bool GetDisabled(string moduleId)
    {
        return true;
    }

    public void SetDisabled(string moduleId, bool value)
    {

    }
}