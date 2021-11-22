namespace Bannerlord.ModuleManager
{
    public sealed record ApplicationVersionRange
    {
        public static ApplicationVersionRange Empty => new();

        public ApplicationVersion Min { get; init; } = ApplicationVersion.Empty;
        public ApplicationVersion Max { get; init; } = ApplicationVersion.Empty;

        public ApplicationVersionRange() { }

        public ApplicationVersionRange(ApplicationVersion min, ApplicationVersion max)
        {
            Max = max;
            Min = min;
        }

        public override string ToString() => $"{Min} - {Max}";

        public static bool TryParse(string versionRangeAsString, out ApplicationVersionRange versionRange)
        {
            versionRange = Empty;

            if (string.IsNullOrWhiteSpace(versionRangeAsString))
                return false;

            versionRangeAsString = versionRangeAsString.Replace(" ", string.Empty);
            var index = versionRangeAsString.IndexOf('-');
            if (index < 0)
                return false;

            var minAsString = versionRangeAsString.Substring(0, index);
            var maxAsString = versionRangeAsString.Substring(index + 1, versionRangeAsString.Length - 1 - index);

            if (ApplicationVersion.TryParse(minAsString, out var min, true) && ApplicationVersion.TryParse(maxAsString, out var max, false))
            {
                versionRange = new ApplicationVersionRange
                {
                    Min = min,
                    Max = max
                };
                return true;
            }

            return false;
        }

        //public bool Equals(ApplicationVersionRange? other) => Min.IsSame(other.Min) && Max.IsSame(other.Max);
    }
}