namespace Bannerlord.ModuleManager
{
    public sealed record DependentModule
    {
        public string Id { get; init; } = string.Empty;
        public ApplicationVersion Version { get; init; } = ApplicationVersion.Empty;

        public DependentModule() { }
        public DependentModule(string id, ApplicationVersion version)
        {
            Id = id;
            Version = version;
        }
    }
}