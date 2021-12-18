namespace Bannerlord.ModuleManager
{
    public sealed record DependentModuleMetadata
    {
        public string Id { get; init; } = string.Empty;
        public LoadType LoadType { get; init; }
        public bool IsOptional { get; init; }
        public bool IsIncompatible { get; init; }
        public ApplicationVersion Version { get; init; } = ApplicationVersion.Empty;
        public ApplicationVersionRange VersionRange { get; init; } = ApplicationVersionRange.Empty;

        public static string GetLoadType(LoadType loadType) => loadType switch
        {
            LoadType.None => "",
            LoadType.LoadAfterThis => "Before       ",
            LoadType.LoadBeforeThis => "After        ",
            _ => "ERROR        "
        };
        public static string GetVersion(ApplicationVersion av) => av.IsSameWithChangeSet(ApplicationVersion.Empty) ? "" : $" {av}";
        public static string GetVersionRange(ApplicationVersionRange avr) => avr == ApplicationVersionRange.Empty ? "" : $" {avr}";
        public static string GetOptional(bool isOptional) => isOptional ? " Optional" : "";
        public static string GetIncompatible(bool isOptional) => isOptional ? "Incompatible " : "";
        public override string ToString() => GetLoadType(LoadType) + GetIncompatible(IsIncompatible) + Id + GetVersion(Version) + GetVersionRange(VersionRange) + GetOptional(IsOptional);
    }
}