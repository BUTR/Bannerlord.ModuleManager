using BUTR.NativeAOT.Shared;

using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;

namespace Bannerlord.ModuleManager.Native.Tests;

public static class Utils2
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        IncludeFields = false,
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin)
    };
    internal static readonly SourceGenerationContext CustomSourceGenerationContext = new(Options);
    
    public static unsafe ReadOnlySpan<char> ToSpan(char* value) => new SafeStringMallocHandle(value).ToSpan();
    public static unsafe ReadOnlySpan<char> ToSpan(param_string* value) => new SafeStringMallocHandle((char*)value).ToSpan();
    public static unsafe param_json* ToJson<T>(T value) => (param_json*) Utils.SerializeJsonCopy(value, (JsonTypeInfo<T>) CustomSourceGenerationContext.GetTypeInfo(typeof(T)));
    public static unsafe (string Error, T? Result) GetResult<T>(return_value_json* ret)
    {
        var result = Unsafe.AsRef<return_value_json>(ret);
        return (ToSpan(result.Error).ToString(), Utils.DeserializeJson(new SafeStringMallocHandle(result.Value), (JsonTypeInfo<T>) CustomSourceGenerationContext.GetTypeInfo(typeof(T))));
    }
    public static unsafe (string Error, string Result) GetResult(return_value_string* ret)
    {
        var result = Unsafe.AsRef<return_value_string>(ret);
        return (ToSpan(result.Error).ToString(), ToSpan(result.Value).ToString());
    }
    public static unsafe (string Error, bool Result) GetResult(return_value_bool* ret)
    {
        var result = Unsafe.AsRef<return_value_bool>(ret);
        return (ToSpan(result.Error).ToString(), result.Value);
    }
    public static unsafe (string Error, int Result) GetResult(return_value_int32* ret)
    {
        var result = Unsafe.AsRef<return_value_int32>(ret);
        return (new string(result.Error), result.Value);
    }
    public static unsafe (string Error, string Result) GetResult(return_value_void* ret)
    {
        var result = Unsafe.AsRef<return_value_void>(ret);
        return (new string(result.Error), string.Empty);
    }
}