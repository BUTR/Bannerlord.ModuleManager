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
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        record ApplicationVersionRange
    {
        public static ApplicationVersionRange Empty => new();

        public ApplicationVersion Min { get; set; } = ApplicationVersion.Empty;
        public ApplicationVersion Max { get; set; } = ApplicationVersion.Empty;

        public ApplicationVersionRange() { }
        public ApplicationVersionRange(ApplicationVersion min, ApplicationVersion max)
        {
            Max = max;
            Min = min;
        }

        public bool IsSame(ApplicationVersionRange? other) =>
            Min.IsSame(other?.Min) && Max.IsSame(other?.Max);

        public bool IsSameWithChangeSet(ApplicationVersionRange? other) =>
            Min.IsSameWithChangeSet(other?.Min) && Max.IsSameWithChangeSet(other?.Max);

        public override string ToString() 
        {
            // If the min and max changeset are at their default (0 and i32::MAX), we can
            // simplify how we print the version. End user mods rarely use the changeset,
            // it's only really used in official packages.
            if (Min.ChangeSet == 0 && Max.ChangeSet == int.MaxValue)
                return $"{Min.ToStringWithoutChangeset()} - {Max.ToStringWithoutChangeset()}";

            return $"{Min} - {Max}";
        }

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
    }
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}