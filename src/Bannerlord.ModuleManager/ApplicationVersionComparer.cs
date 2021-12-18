using System.Collections.Generic;

namespace Bannerlord.ModuleManager
{
    public class ApplicationVersionComparer : IComparer<ApplicationVersion>
    {
        public virtual int Compare(ApplicationVersion x, ApplicationVersion y)
        {
            var versionTypeComparison = x.ApplicationVersionType.CompareTo(y.ApplicationVersionType);
            if (versionTypeComparison != 0) return versionTypeComparison;

            var majorComparison = x.Major.CompareTo(y.Major);
            if (majorComparison != 0) return majorComparison;

            var minorComparison = x.Minor.CompareTo(y.Minor);
            if (minorComparison != 0) return minorComparison;

            var revisionComparison = x.Revision.CompareTo(y.Revision);
            if (revisionComparison != 0) return revisionComparison;

            var changeSetComparison = x.ChangeSet.CompareTo(y.ChangeSet);
            if (changeSetComparison != 0) return changeSetComparison;

            return 0;
        }
    }
}