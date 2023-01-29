using BUTR.NativeAOT.Shared;

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization.Metadata;

namespace Bannerlord.ModuleManager.Native
{
    public static class Logger
    {
        [Conditional("LOGGING")]
        public static void LogInput([CallerMemberName] string? caller = null)
        {
            Log($"Received call to {caller}!");
        }
        [Conditional("LOGGING")]
        public static void LogInput(nuint param1, [CallerMemberName] string? caller = null)
        {
            Log($"Received call to {caller}! {param1}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInput<T1>(T1* param1, [CallerMemberName] string? caller = null)
            where T1 : unmanaged, IParameter<T1>
        {
            Log($"Received call to {caller}! {T1.ToSpan(param1)}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInput<T1, T2>(T1* param1, T2* param2, [CallerMemberName] string? caller = null)
            where T1 : unmanaged, IParameter<T1>
            where T2 : unmanaged, IParameter<T2>
        {
            Log($"Received call to {caller}! {T1.ToSpan(param1)}; {T2.ToSpan(param2)}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInput<T1, T2, T3>(T1* param1, T2* param2, T3* param3, [CallerMemberName] string? caller = null)
            where T1 : unmanaged, IParameter<T1>
            where T2 : unmanaged, IParameter<T2>
            where T3 : unmanaged, IParameter<T3>
        {
            Log($"Received call to {caller}! {T1.ToSpan(param1)}; {T2.ToSpan(param2)}; {T3.ToSpan(param3)}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInput<T1, T2, T3, T4>(T1* param1, T2* param2, T3* param3, T4* param4, [CallerMemberName] string? caller = null)
            where T1 : unmanaged, IParameter<T1>
            where T2 : unmanaged, IParameter<T2>
            where T3 : unmanaged, IParameter<T3>
            where T4 : unmanaged, IParameter<T4>
        {
            Log($"Received call to {caller}! {T1.ToSpan(param1)}; {T2.ToSpan(param2)}; {T3.ToSpan(param3)}; {T4.ToSpan(param4)}");
        }

        [Conditional("LOGGING")]
        public static unsafe void LogInputChar(char* param1, [CallerMemberName] string? caller = null)
        {
            var p1 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param1);
            Log($"Received call to {caller}! {p1}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInputChar(char* param1, char* param2, [CallerMemberName] string? caller = null)
        {
            var p1 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param1);
            var p2 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param2);
            Log($"Received call to {caller}! {p1}; {p2}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInputChar(char* param1, char* param2, char* param3, [CallerMemberName] string? caller = null)
        {
            var p1 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param1);
            var p2 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param2);
            var p3 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param3);
            Log($"Received call to {caller}! {p1}; {p2}; {p3}");
        }
        [Conditional("LOGGING")]
        public static unsafe void LogInputChar(char* param1, char* param2, char* param3, char* param4, [CallerMemberName] string? caller = null)
        {
            var p1 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param1);
            var p2 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param2);
            var p3 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param3);
            var p4 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(param4);
            Log($"Received call to {caller}! {p1}; {p2}; {p3}; {p4}");
        }

        [Conditional("LOGGING")]
        public static void LogOutput([CallerMemberName] string? caller = null)
        {
            Log($"Result of {caller}");
        }
        [Conditional("LOGGING")]
        public static void LogOutputPrimitive<TResult>(TResult result, [CallerMemberName] string? caller = null) where TResult : unmanaged
        {
            Log($"Result of {caller}: {result}");
        }
        [Conditional("LOGGING")]
        public static void LogOutputManaged<TResult>(TResult result, [CallerMemberName] string? caller = null) where TResult : class
        {
            if (Bindings.CustomSourceGenerationContext.GetTypeInfo(typeof(TResult)) is JsonTypeInfo<TResult> jsonTypeInfo)
                Log($"Result of {caller}: {Utils.SerializeJson(result, jsonTypeInfo)}");
            else
                Log($"Result of {caller}: NON SERIALIZABLE");
        }

        [Conditional("LOGGING")]
        public static void LogException(Exception e, [CallerMemberName] string? caller = null)
        {
            Log($"Exception of {caller}: {e}");
        }

        private static void Log(string message)
        {
            File.AppendAllText("Bannerlord.ModuleManager.Native.log", $"{message}{Environment.NewLine}");
        }
    }
}