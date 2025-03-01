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
    using System.Collections;
    using System.Collections.Generic;

#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        class ApplicationVersionComparer : IComparer<ApplicationVersion?>, IComparer
    {
        /// <inheritdoc/>
        public int Compare(object? x, object? y) => Compare(x as ApplicationVersion, y as ApplicationVersion);

        /// <inheritdoc/>
        public virtual int Compare(ApplicationVersion? x, ApplicationVersion? y) => CompareStandard(x, y);

        public static int CompareStandard(ApplicationVersion? x, ApplicationVersion? y)
        {
            if (x is null && y is null)
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

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
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}