﻿#region License
// MIT License
//
// Copyright (c) Bannerlord's Unofficial Tools & Resources
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#nullable enable
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning disable
#endif

namespace Bannerlord.ModuleManager
{
    using System;

#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        record ApplicationVersion : IComparable<ApplicationVersion>
    {
        public static ApplicationVersion Empty { get; } = new();

        public ApplicationVersionType ApplicationVersionType { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int ChangeSet { get; set; }

        public ApplicationVersion() { }
        public ApplicationVersion(ApplicationVersionType applicationVersionType, int major, int minor, int revision, int changeSet)
        {
            ApplicationVersionType = applicationVersionType;
            Major = major;
            Minor = minor;
            Revision = revision;
            ChangeSet = changeSet;
        }

        public bool IsSame(ApplicationVersion? other) =>
            Major == other?.Major && Minor == other.Minor && Revision == other.Revision;

        public bool IsSameWithChangeSet(ApplicationVersion? other) =>
            Major == other?.Major && Minor == other.Minor && Revision == other.Revision && ChangeSet == other.ChangeSet;

        public override string ToString()
        {
            // Most user mods skip this, so to be user-friendly we can omit it if it was not originally specified.
            return ChangeSet == 0 
                ? $"{GetPrefix(ApplicationVersionType)}{Major}.{Minor}.{Revision}" 
                : $"{GetPrefix(ApplicationVersionType)}{Major}.{Minor}.{Revision}.{ChangeSet}";
        }
        public string ToStringWithoutChangeset() => $"{GetPrefix(ApplicationVersionType)}{Major}.{Minor}.{Revision}";

        public int CompareTo(ApplicationVersion? other) => ApplicationVersionComparer.CompareStandard(this, other);

        public static bool operator <(ApplicationVersion left, ApplicationVersion right) => left.CompareTo(right) < 0;
        public static bool operator >(ApplicationVersion left, ApplicationVersion right) => left.CompareTo(right) > 0;
        public static bool operator <=(ApplicationVersion left, ApplicationVersion right) => left.CompareTo(right) <= 0;
        public static bool operator >=(ApplicationVersion left, ApplicationVersion right) => left.CompareTo(right) >= 0;

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
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}