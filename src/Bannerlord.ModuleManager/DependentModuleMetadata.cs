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

        private static string GetLoadType(LoadType loadType) => loadType switch
        {
            LoadType.None => "",
            LoadType.LoadAfterThis => "Before       ",
            LoadType.LoadBeforeThis => "After        ",
            _ => "ERROR        "
        };
        private static string GetVersion(ApplicationVersion av) => av.IsSameWithChangeSet(ApplicationVersion.Empty) ? "" : $" {av}";
        private static string GetVersionRange(ApplicationVersionRange avr) => avr == ApplicationVersionRange.Empty ? "" : $" {avr}";
        private static string GetOptional(bool isOptional) => isOptional ? " Optional" : "";
        private static string GetIncompatible(bool isOptional) => isOptional ? "Incompatible " : "";
        public override string ToString() => GetLoadType(LoadType) + GetIncompatible(IsIncompatible) + Id + GetVersion(Version) + GetVersionRange(VersionRange) + GetOptional(IsOptional);
    }
}