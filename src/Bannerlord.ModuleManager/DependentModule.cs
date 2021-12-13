namespace Bannerlord.ModuleManager
{
    public sealed record DependentModule
    {
        public string Id { get; init; } = string.Empty;
        public ApplicationVersion Version { get; init; } = ApplicationVersion.Empty;
        public bool IsOptional { get; init; } = false;

        public DependentModule() { }
        public DependentModule(string id, ApplicationVersion version, bool isOptional)
        {
            Id = id;
            Version = version;
            IsOptional = isOptional;
        }
    }
}