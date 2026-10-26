using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.ModuleManager.Tests
{
    /// <summary>
    /// The indexed GetDependencies, the sorts and EnableModule/DisableModule against <see cref="ReferenceModuleUtilities"/>,
    /// the implementation before the index: same modules, same instances, same order
    /// </summary>
    public class ModuleDependencyIndexTests
    {
        private static readonly ModuleSorterOptions[] AllOptions =
        {
            new() { SkipOptionals = false, SkipExternalDependencies = false },
            new() { SkipOptionals = true, SkipExternalDependencies = false },
            new() { SkipOptionals = false, SkipExternalDependencies = true },
            new() { SkipOptionals = true, SkipExternalDependencies = true },
        };

        private static IEnumerable<int> Seeds() => Enumerable.Range(0, 60);

        /// <summary>
        /// Random modules with every kind of dependency, missing and optional ones, cycles, self references,
        /// two modules with the same ID, the same instance twice and a copy equal by value
        /// </summary>
        private static List<ModuleInfoExtended> Generate(int seed)
        {
            var random = new Random(seed);
            var count = 1 + seed * 7 % 90;
            var ids = Enumerable.Range(0, count).Select(i => $"M{i}").ToArray();
            string RandomId() => random.Next(12) == 0 ? $"Missing{random.Next(4)}" : ids[random.Next(ids.Length)];

            List<DependentModule> DependentModules(int max) => Enumerable.Range(0, random.Next(max + 1))
                .Select(_ => new DependentModule(RandomId(), ApplicationVersion.Empty, random.Next(4) == 0))
                .ToList();

            ModuleInfoExtended NewModule(string id) => new()
            {
                Id = id,
                Name = id,
                IsOfficial = random.Next(8) == 0,
                IsSingleplayerModule = true,
                DependentModules = DependentModules(3),
                ModulesToLoadAfterThis = DependentModules(2),
                IncompatibleModules = random.Next(10) == 0 ? DependentModules(1) : new List<DependentModule>(),
                DependentModuleMetadatas = Enumerable.Range(0, random.Next(4)).Select(_ => new DependentModuleMetadata
                {
                    Id = RandomId(),
                    LoadType = (LoadType) random.Next(3),
                    IsOptional = random.Next(3) == 0,
                    IsIncompatible = random.Next(10) == 0,
                }).ToList(),
            };

            var modules = ids.Select(NewModule).ToList();
            if (count > 2)
            {
                modules.Add(NewModule(ids[random.Next(count)]));
                modules.Add(modules[random.Next(count)]);
                modules.Add(modules[random.Next(count)] with { });
            }
            // Fisher-Yates, the order of the list matters to every algorithm here
            for (var i = modules.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (modules[i], modules[j]) = (modules[j], modules[i]);
            }
            return modules;
        }

        private static void AssertSameInstances(IEnumerable<ModuleInfoExtended> expected, IEnumerable<ModuleInfoExtended> actual, string message)
        {
            var expectedList = expected.ToList();
            var actualList = actual.ToList();
            Assert.That(actualList.Select(x => x.Id), Is.EqualTo(expectedList.Select(x => x.Id)), message);
            for (var i = 0; i < expectedList.Count; i++)
                Assert.That(actualList[i], Is.SameAs(expectedList[i]), $"{message}, at {i}");
        }

        [TestCaseSource(nameof(Seeds))]
        public void GetDependencies_MatchesReference(int seed)
        {
            var modules = Generate(seed);
            foreach (var options in AllOptions)
            {
                var index = new ModuleDependencyIndex(modules, options);
                foreach (var module in modules)
                {
                    var expected = ReferenceModuleUtilities.GetDependencies(modules, module, options).ToList();
                    var message = $"{module.Id}, {options}";
                    AssertSameInstances(expected, ModuleUtilities.GetDependencies(modules, module, options), message);
                    AssertSameInstances(expected, ModuleUtilities.GetDependencies(index, module), $"{message}, reused index");
                }
            }

            foreach (var module in modules)
            {
                AssertSameInstances(ReferenceModuleUtilities.GetDependencies(modules, module), ModuleUtilities.GetDependencies(modules, module), module.Id);
            }
        }

        [TestCaseSource(nameof(Seeds))]
        public void GetDependencies_WithVisited_MatchesReference(int seed)
        {
            var modules = Generate(seed);
            var random = new Random(seed);
            foreach (var options in AllOptions)
            {
                foreach (var module in modules)
                {
                    // Modules a caller already visited are skipped, the set is shared with the caller
                    var alreadyVisited = modules.Where(_ => random.Next(5) == 0).ToList();
                    var expectedVisited = new HashSet<ModuleInfoExtended>(alreadyVisited);
                    var actualVisited = new HashSet<ModuleInfoExtended>(alreadyVisited);

                    var expected = ReferenceModuleUtilities.GetDependencies(modules, module, expectedVisited, options);
                    var actual = ModuleUtilities.GetDependencies(modules, module, actualVisited, options);

                    AssertSameInstances(expected, actual, $"{module.Id}, {options}");
                    Assert.That(actualVisited.SetEquals(expectedVisited), Is.True, $"{module.Id}, {options}, visited");
                }
            }
        }

        [TestCaseSource(nameof(Seeds))]
        public void Sort_MatchesReference(int seed)
        {
            var modules = Generate(seed);
            AssertSameInstances(ReferenceModuleUtilities.Sort(modules), ModuleSorter.Sort(modules), "Sort");
            foreach (var options in AllOptions)
                AssertSameInstances(ReferenceModuleUtilities.Sort(modules, options), ModuleSorter.Sort(modules, options), $"Sort, {options}");
        }

        [TestCaseSource(nameof(Seeds))]
        public void TopologySort_WithIndex_MatchesReference(int seed)
        {
            // How the launcher sorts: its own order, then a topology sort of it with a shared index
            var modules = Generate(seed).OrderByDescending(x => x.IsOfficial).ThenBy(x => x.Id, StringComparer.Ordinal).ToArray();
            foreach (var options in AllOptions)
            {
                var expected = ModuleSorter.TopologySort(modules, x => ReferenceModuleUtilities.GetDependencies(modules, x, options));
                var index = new ModuleDependencyIndex(modules, options);
                var actual = ModuleSorter.TopologySort(modules, x => ModuleUtilities.GetDependencies(index, x));
                AssertSameInstances(expected, actual, $"TopologySort, {options}");
            }
        }

        private sealed class States
        {
            private readonly Dictionary<ModuleInfoExtended, bool> _selected = new(ReferenceEqualityComparer.Instance);
            private readonly Dictionary<ModuleInfoExtended, bool> _disabled = new(ReferenceEqualityComparer.Instance);

            public States(IEnumerable<ModuleInfoExtended> modules, Random random)
            {
                foreach (var module in modules.Distinct(ReferenceEqualityComparer.Instance).Cast<ModuleInfoExtended>())
                {
                    _selected[module] = random.Next(2) == 0;
                    _disabled[module] = random.Next(5) == 0;
                }
            }

            private States(States other)
            {
                _selected = new Dictionary<ModuleInfoExtended, bool>(other._selected, ReferenceEqualityComparer.Instance);
                _disabled = new Dictionary<ModuleInfoExtended, bool>(other._disabled, ReferenceEqualityComparer.Instance);
            }

            public States Copy() => new(this);

            public bool GetSelected(ModuleInfoExtended module) => _selected[module];
            public void SetSelected(ModuleInfoExtended module, bool value) => _selected[module] = value;
            public bool GetDisabled(ModuleInfoExtended module) => _disabled[module];
            public void SetDisabled(ModuleInfoExtended module, bool value) => _disabled[module] = value;

            public void AssertEqual(States expected, string message)
            {
                foreach (var (module, selected) in expected._selected)
                {
                    Assert.That(_selected[module], Is.EqualTo(selected), $"{message}, {module.Id} selected");
                    Assert.That(_disabled[module], Is.EqualTo(expected._disabled[module]), $"{message}, {module.Id} disabled");
                }
            }
        }

        [TestCaseSource(nameof(Seeds))]
        public void EnableDisableModule_MatchReference(int seed)
        {
            var modules = Generate(seed);
            var random = new Random(seed);
            foreach (var target in modules)
            {
                var initial = new States(modules, random);

                var expected = initial.Copy();
                var actual = initial.Copy();
                ReferenceModuleUtilities.EnableModule(modules, target, expected.GetSelected, expected.SetSelected, expected.GetDisabled, expected.SetDisabled);
                ModuleUtilities.EnableModule(modules, target, actual.GetSelected, actual.SetSelected, actual.GetDisabled, actual.SetDisabled);
                actual.AssertEqual(expected, $"Enable {target.Id}");

                expected = initial.Copy();
                actual = initial.Copy();
                ReferenceModuleUtilities.DisableModule(modules, target, expected.GetSelected, expected.SetSelected, expected.GetDisabled, expected.SetDisabled);
                ModuleUtilities.DisableModule(modules, target, actual.GetSelected, actual.SetSelected, actual.GetDisabled, actual.SetDisabled);
                actual.AssertEqual(expected, $"Disable {target.Id}");
            }
        }
    }
}
