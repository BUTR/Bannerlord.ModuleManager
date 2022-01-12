namespace Bannerlord.ModuleManager
{
    public sealed record ApplicationVersion
    {
        public static ApplicationVersion Empty { get; } = new();

        public ApplicationVersionType ApplicationVersionType { get; init; }
        public int Major { get; init; }
        public int Minor { get; init; }
        public int Revision { get; init; }
        public int ChangeSet { get; init; }

        public ApplicationVersion() { }

        public ApplicationVersion(ApplicationVersionType applicationVersionType, int major, int minor, int revision, int changeSet)
        {
            ApplicationVersionType = applicationVersionType;
            Major = major;
            Minor = minor;
            Revision = revision;
            ChangeSet = changeSet;
        }

        public bool IsSame(ApplicationVersion other) =>
            Major == other.Major && Minor == other.Minor && Revision == other.Revision;

        public bool IsSameWithChangeSet(ApplicationVersion other) =>
            Major == other.Major && Minor == other.Minor && Revision == other.Revision && ChangeSet == other.ChangeSet;

        public override string ToString() => $"{GetPrefix(ApplicationVersionType)}{Major}.{Minor}.{Revision}.{ChangeSet}";

        public static char GetPrefix(ApplicationVersionType applicationVersionType) => applicationVersionType switch
        {
            ApplicationVersionType.Alpha => 'a',
            ApplicationVersionType.Beta => 'b',
            ApplicationVersionType.EarlyAccess => 'e',
            ApplicationVersionType.Release => 'v',
            ApplicationVersionType.Development => 'd',
            _ => 'i'
        };

        public static ApplicationVersionType FromPrefix(char applicationVersionType) => applicationVersionType switch
        {
            'a' => ApplicationVersionType.Alpha,
            'b' => ApplicationVersionType.Beta,
            'e' => ApplicationVersionType.EarlyAccess,
            'v' => ApplicationVersionType.Release,
            'd' => ApplicationVersionType.Development,
            _ => ApplicationVersionType.Invalid
        };

        public static bool TryParse(string? versionAsString, out ApplicationVersion version) => TryParse(versionAsString, out version, true);

        public static bool TryParse(string? versionAsString, out ApplicationVersion version, bool asMin)
        {
            var major = asMin ? 0 : int.MaxValue;
            var minor = asMin ? 0 : int.MaxValue;
            var revision = asMin ? 0 : int.MaxValue;
            var changeSet = asMin ? 0 : int.MaxValue;
            var skipCheck = false;
            version = Empty;
            if (versionAsString is null)
                return false;

            var array = versionAsString.Split('.');
            if (array.Length != 3 && array.Length != 4 && array[0].Length == 0)
                return false;

            var applicationVersionType = FromPrefix(array[0][0]);
            if (!skipCheck && !int.TryParse(array[0].Substring(1), out major))
            {
                if (array[0].Substring(1) != "*") return false;
                major = int.MinValue;
                minor = int.MinValue;
                revision = int.MinValue;
                changeSet = int.MinValue;
                skipCheck = true;
            }
            if (!skipCheck && !int.TryParse(array[1], out minor))
            {
                if (array[1] != "*") return false;
                minor = asMin ? 0 : int.MaxValue;
                revision = asMin ? 0 : int.MaxValue;
                changeSet = asMin ? 0 : int.MaxValue;
                skipCheck = true;
            }
            if (!skipCheck && !int.TryParse(array[2], out revision))
            {
                if (array[2] != "*") return false;
                revision = asMin ? 0 : int.MaxValue;
                changeSet = asMin ? 0 : int.MaxValue;
                skipCheck = true;
            }

            if (!skipCheck && array.Length == 4 && !int.TryParse(array[3], out changeSet))
            {
                if (array[3] != "*") return false;
                changeSet = asMin ? 0 : int.MaxValue;
                skipCheck = true;
            }

            version = new ApplicationVersion
            {
                ApplicationVersionType = applicationVersionType,
                Major = major,
                Minor = minor,
                Revision = revision,
                ChangeSet = changeSet
            };

            return true;
        }
    }
}