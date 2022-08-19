﻿// <auto-generated>
//   This code file has automatically been added by the "Bannerlord.ModuleManager.Source" NuGet package (https://www.nuget.org/packages/Bannerlord.ModuleManager.Source).
//   Please see https://github.com/BUTR/Bannerlord.ModuleManager for more information.
//
//   IMPORTANT:
//   DO NOT DELETE THIS FILE if you are using a "packages.config" file to manage your NuGet references.
//   Consider migrating to PackageReferences instead:
//   https://docs.microsoft.com/en-us/nuget/consume-packages/migrate-packages-config-to-package-reference
//   Migrating brings the following benefits:
//   * The "Bannerlord.ModuleManager.Source" folder and the "ApplicationVersionRange.cs" file don't appear in your project.
//   * The added file is immutable and can therefore not be modified by coincidence.
//   * Updating/Uninstalling the package will work flawlessly.
// </auto-generated>

#region License
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

namespace Bannerlord.ModuleManager
{
#nullable enable
#pragma warning disable
#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        sealed record ApplicationVersionRange
    {
        public static ApplicationVersionRange Empty => new();

        public ApplicationVersion Min
        {
#if REFLECTION_FREE
            get; set;
#else
            get; init;
#endif
        } = ApplicationVersion.Empty;

        public ApplicationVersion Max
        {
#if REFLECTION_FREE
            get; set;
#else
            get; init;
#endif
        } = ApplicationVersion.Empty;

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
    }
#pragma warning restore
#nullable restore
}