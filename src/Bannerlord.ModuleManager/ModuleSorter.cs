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

#nullable enable
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning disable
#endif

namespace Bannerlord.ModuleManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        sealed record ModuleSorterOptions
    {
        public bool SkipOptionals { get; set; }
        public bool SkipExternalDependencies { get; set; }

        public ModuleSorterOptions() { }
        public ModuleSorterOptions(bool skipOptionals, bool skipExternalDependencies)
        {
            SkipOptionals = skipOptionals;
            SkipExternalDependencies = skipExternalDependencies;
        }
    }

#if !BANNERLORDBUTRMODULEMANAGER_PUBLIC
    internal
#else
    public
# endif
        static class ModuleSorter
    {
        public static IList<ModuleInfoExtended> Sort(IReadOnlyCollection<ModuleInfoExtended> source)
        {
            var correctModules = source
                .Where(x => ModuleUtilities.AreDependenciesPresent(source, x))
                .OrderByDescending(x => x.IsOfficial)
                .ThenByDescending(mim => mim.Id, new AlphanumComparatorFast())
                .ToArray();

            return TopologySort(correctModules, module => ModuleUtilities.GetDependencies(correctModules, module));
        }
        public static IList<ModuleInfoExtended> Sort(IReadOnlyCollection<ModuleInfoExtended> source, ModuleSorterOptions options)
        {
            var correctModules = source
                .Where(x => ModuleUtilities.AreDependenciesPresent(source, x))
                .OrderByDescending(x => x.IsOfficial)
                .ThenByDescending(mim => mim.Id, new AlphanumComparatorFast())
                .ToArray();

            return TopologySort(correctModules, module => ModuleUtilities.GetDependencies(correctModules, module, options));
        }

        public static IList<T> TopologySort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var list = new List<T>();
            var visited = new HashSet<T>();
            foreach (var item in source)
            {
                Visit(item, getDependencies, item => list.Add(item), visited);
            }
            return list;
        }

        public static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, Action<T> addItem, HashSet<T> visited)
        {
            if (visited.Contains(item))
            {
                return;
            }

            visited.Add(item);
            if (getDependencies(item) is { } enumerable)
            {
                foreach (var item2 in enumerable)
                {
                    Visit(item2, getDependencies, addItem, visited);
                }
            }
            addItem(item);
        }
    }
#nullable restore
#if !BANNERLORDBUTRMODULEMANAGER_ENABLE_WARNING
#pragma warning restore
#endif
}