using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bannerlord.ModuleManager.Native
{
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
    [JsonSerializable(typeof(ApplicationVersion))]
    [JsonSerializable(typeof(SubModuleInfoExtended))]
    [JsonSerializable(typeof(ModuleInfoExtended))]
    [JsonSerializable(typeof(ModuleInfoExtended[]))]
    [JsonSerializable(typeof(ModuleSorterOptions))]
    [JsonSerializable(typeof(ModuleIssue[]))]
    internal partial class SourceGenerationContext : JsonSerializerContext { }
}