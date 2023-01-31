using BUTR.NativeAOT.Shared;

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;

namespace Bannerlord.ModuleManager.Native
{
    public static class Logger
    {
        [Conditional("LOGGING")]
        public static void LogInput([CallerMemberName] string? caller = null)
        {
            Log($"{caller} - Starting");
        }
        [Conditional("LOGGING")]
        public static void LogInput(string param, [CallerMemberName] string? caller = null)
        {
            Log($"{caller} - Starting: {param}");
        }
        [Conditional("LOGGING")]
        public static void LogInput<T1>(T1 param1, [CallerMemberName] string? caller = null)
            where T1 : IFormattable
        {
            Log($"{caller} - Starting: {param1.ToString(null, CultureInfo.InvariantCulture)}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInput<T1>(T1* param1, [CallerMemberName] string? caller = null)
            where T1 : unmanaged, IParameterSpanFormattable<T1>
        {
            Log($"{caller} - Starting: {T1.ToSpan(param1)}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInput<T1, T2>(T1* param1, T2* param2, [CallerMemberName] string? caller = null)
            where T1 : unmanaged, IParameterSpanFormattable<T1>
            where T2 : unmanaged, IParameterSpanFormattable<T2>
        {
            Log($"{caller} - Starting: {T1.ToSpan(param1)}; {T2.ToSpan(param2)}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInput<T1, T2, T3>(T1* param1, T2* param2, T3* param3, [CallerMemberName] string? caller = null)
            where T1 : unmanaged, IParameterSpanFormattable<T1>
            where T2 : unmanaged, IParameterSpanFormattable<T2>
            where T3 : unmanaged, IParameterSpanFormattable<T3>
        {
            Log($"{caller} - Starting: {T1.ToSpan(param1)}; {T2.ToSpan(param2)}; {T3.ToSpan(param3)}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInput<T1, T2, T3, T4>(T1* param1, T2* param2, T3* param3, T4* param4, [CallerMemberName] string? caller = null)
            where T1 : unmanaged, IParameterSpanFormattable<T1>
            where T2 : unmanaged, IParameterSpanFormattable<T2>
            where T3 : unmanaged, IParameterSpanFormattable<T3>
            where T4 : unmanaged, IParameterSpanFormattable<T4>
        {
            Log($"{caller} - Starting: {T1.ToSpan(param1)}; {T2.ToSpan(param2)}; {T3.ToSpan(param3)}; {T4.ToSpan(param4)}");
        }

        [Conditional("LOGGING")]
        public static unsafe void LogPinned(char* param1, [CallerMemberName] string? caller = null)
        {
            var p1 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param1);
            Log($"{caller} - Pinned: {p1}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogPinned(char* param1, char* param2, [CallerMemberName] string? caller = null)
        {
            var p1 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param1);
            var p2 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param2);
            Log($"{caller} - Pinned: {p1}; {p2}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogPinned(char* param1, char* param2, char* param3, [CallerMemberName] string? caller = null)
        {
            var p1 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param1);
            var p2 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param2);
            var p3 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param3);
            Log($"{caller} - Pinned: {p1}; {p2}; {p3}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogPinned(char* param1, char* param2, char* param3, char* param4, [CallerMemberName] string? caller = null)
        {
            var p1 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param1);
            var p2 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param2);
            var p3 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param3);
            var p4 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param4);
            Log($"{caller} - Pinned: {p1}; {p2}; {p3}; {p4}");
        }

        [Conditional("LOGGING")]
        public static void LogOutput([CallerMemberName] string? caller = null)
        {
            Log($"{caller} - Finished");
        }
        [Conditional("LOGGING")]
        public static void LogOutput(string param, [CallerMemberName] string? caller = null)
        {
            Log($"{caller} - Finished: {param}");
        }
        [Conditional("LOGGING")]
        public static void LogOutput<T1>(T1 result, [CallerMemberName] string? caller = null)
        {
            if (Bindings.CustomSourceGenerationContext.GetTypeInfo(typeof(T1)) is JsonTypeInfo<T1> jsonTypeInfo)
                Log($"{caller} - Finished: JSON - {JsonSerializer.Serialize(result, jsonTypeInfo)}");
            else
                Log($"{caller} - Finished: RAW - {result?.ToString()}");
        }
        [Conditional("LOGGING")]
        public static void LogOutput(bool result, [CallerMemberName] string? caller = null)
        {
            Log($"{caller} - Finished: {result}");
        }

        [Conditional("LOGGING")]
        public static void LogException(Exception e, [CallerMemberName] string? caller = null)
        {
            Log($"{caller} - Exception: {e}");
        }

        private static readonly ReaderWriterLock _lock = new();
        private static void Log(string message)
        {
            while (true)
            {
                try
                {
                    _lock.AcquireWriterLock(100);

                    try 
                    { 
                        using var fs = new FileStream("Bannerlord.VortexExtension.Native.log", FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.SequentialScan);
                        using var sw = new StreamWriter(fs, Encoding.UTF8, -1, true);
                        sw.WriteLine(message);
                        return;
                    } 
                    finally 
                    { 
                        _lock.ReleaseWriterLock(); 
                    }
                }
                catch (Exception) { /* ignored */ }
            }
        }
    }
}