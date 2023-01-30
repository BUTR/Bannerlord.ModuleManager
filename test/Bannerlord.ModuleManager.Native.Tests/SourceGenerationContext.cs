using System.Text.Json.Serialization;

namespace Bannerlord.ModuleManager.Native.Tests
{
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
    [JsonSerializable(typeof(ApplicationVersion))]
    [JsonSerializable(typeof(SubModuleInfoExtended))]
    [JsonSerializable(typeof(ModuleInfoExtended))]
    [JsonSerializable(typeof(ModuleInfoExtended[]))]
    [JsonSerializable(typeof(ModuleSorterOptions))]
    [JsonSerializable(typeof(ModuleIssue[]))]
    [JsonSerializable(typeof(DependentModuleMetadata[]))]
    internal partial class SourceGenerationContext : JsonSerializerContext { }
}