﻿using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;

namespace Bannerlord.ModuleManager.Native.Tests
{
    internal class ValidationManager
    {
        public static unsafe return_value_bool* IsSelected(param_ptr* owner, param_string* moduleId)
        {
            var manager = Unsafe.AsRef<ValidationManager>(owner);
            return return_value_bool.AsValue(manager.IsSelected(Utils2.ToSpan(moduleId).ToString()), false);
        }

        public bool IsSelected(string moduleId)
        {
            return true;
        }
    }
}