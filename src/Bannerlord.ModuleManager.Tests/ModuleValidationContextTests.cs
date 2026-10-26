using Bannerlord.ModuleManager.Models.Issues;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bannerlord.ModuleManager.Tests
{
    /// <summary>
    /// The validation with the ModuleValidationContext against <see cref="ReferenceModuleUtilities"/>, the validation
    /// before it: the same issues in the same order
    /// </summary>
    public class ModuleValidationContextTests
    {
        private static IEnumerable<int> Seeds() => Enumerable.Range(0, 60);

        private static readonly string[] Versions = { "v1.0.0", "v1.2.0", "v2.0.0", "e1.8.0", "v1.0.0.5" };

        /// <summary>
        /// Random modules for the validation. A module only requires modules made before it, the reference validation
        /// recursed into cycles of three until the stack overflowed. Two modules that require each other are fine, it
        /// reports those before it recurses. Duplicate IDs, the same instance twice, incompatibilities, versions,
        /// missing dependencies and BLSE's included
        /// </summary>
        private static List<ModuleInfoExtended> Generate(int seed)
        {
            var random = new Random(seed);
            var count = 1 + seed * 7 % 70;
            var ids = Enumerable.Range(0, count).Select(i => $"M{i}").ToArray();
            ApplicationVersion RandomVersion() => random.Next(3) == 0
                ? ApplicationVersion.Empty
                : ApplicationVersion.TryParse(Versions[random.Next(Versions.Length)], out var version) ? version : ApplicationVersion.Empty;
            string Earlier(int index) => random.Next(14) switch
            {
                0 => $"Missing{random.Next(3)}",
                1 => random.Next(2) == 0 ? "BLSE.LoadingInterceptor" : "BLSE.AssemblyResolver",
                _ => index == 0 ? "Missing0" : ids[random.Next(index)],
            };

            ModuleInfoExtended NewModule(string id, int index) => new()
            {
                Id = id,
                Name = random.Next(30) == 0 ? "" : id,
                Version = RandomVersion(),
                IsOfficial = random.Next(8) == 0,
                IsSingleplayerModule = true,
                DependentModules = Enumerable.Range(0, random.Next(3))
                    .Select(_ => new DependentModule(Earlier(index), RandomVersion(), random.Next(4) == 0))
                    .ToList(),
                // Load after relations don't make the validation recurse, they can point anywhere
                ModulesToLoadAfterThis = Enumerable.Range(0, random.Next(2))
                    .Select(_ => new DependentModule(ids[random.Next(ids.Length)], ApplicationVersion.Empty, random.Next(3) == 0))
                    .ToList(),
                IncompatibleModules = random.Next(6) == 0
                    ? new List<DependentModule> { new(ids[random.Next(ids.Length)], ApplicationVersion.Empty, false) }
                    : new List<DependentModule>(),
                DependentModuleMetadatas = Enumerable.Range(0, random.Next(4)).Select(_ =>
                {
                    var loadType = (LoadType) random.Next(3);
                    var incompatible = random.Next(8) == 0;
                    return new DependentModuleMetadata
                    {
                        // Requirements go to earlier modules, the rest anywhere
                        Id = loadType == LoadType.LoadBeforeThis && !incompatible ? Earlier(index) : ids[random.Next(ids.Length)],
                        LoadType = loadType,
                        IsOptional = random.Next(3) == 0,
                        IsIncompatible = incompatible,
                        Version = RandomVersion(),
                        VersionRange = random.Next(5) == 0
                            ? new ApplicationVersionRange(RandomVersion(), RandomVersion())
                            : ApplicationVersionRange.Empty,
                    };
                }).ToList(),
            };

            var modules = ids.Select((id, i) => NewModule(id, i)).ToList();
            if (count > 3)
            {
                // Two modules that require each other
                var a = random.Next(count);
                var b = (a + 1 + random.Next(count - 1)) % count;
                modules[a] = modules[a] with { DependentModules = modules[a].DependentModules.Append(new DependentModule(ids[b], ApplicationVersion.Empty, false)).ToList() };
                modules[b] = modules[b] with { DependentModules = modules[b].DependentModules.Append(new DependentModule(ids[a], ApplicationVersion.Empty, false)).ToList() };

                var duplicate = random.Next(count);
                modules.Add(NewModule(ids[duplicate], duplicate));
                modules.Add(modules[random.Next(count)]);
                modules.Add(modules[random.Next(count)] with { });
            }
            for (var i = modules.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                (modules[i], modules[j]) = (modules[j], modules[i]);
            }
            return modules;
        }

        private static Func<ModuleInfoExtended, bool> RandomSelection(IEnumerable<ModuleInfoExtended> modules, int seed)
        {
            var random = new Random(seed);
            var selected = new Dictionary<ModuleInfoExtended, bool>(ReferenceEqualityComparer.Instance);
            foreach (var module in modules)
                selected[module] = random.Next(4) != 0;
            return module => selected.TryGetValue(module, out var value) && value;
        }

        private static void AssertSameIssues(IEnumerable<ModuleIssue> expected, IEnumerable<ModuleIssue> actual, string message)
        {
            static string Describe(ModuleIssue issue) => $"{issue.Type} {issue.SourceId} {issue.Reason} {issue.SourceVersion}";
            var expectedList = expected.ToList();
            var actualList = actual.ToList();
            Assert.That(actualList.Select(Describe), Is.EqualTo(expectedList.Select(Describe)), message);
            for (var i = 0; i < expectedList.Count; i++)
                Assert.That(actualList[i].Target, Is.SameAs(expectedList[i].Target), $"{message}, target at {i}");
        }

        [TestCaseSource(nameof(Seeds))]
        public void ValidateModule_MatchesReference(int seed)
        {
            var modules = Generate(seed);
            var isSelected = RandomSelection(modules, seed);
            foreach (var module in modules)
            {
                AssertSameIssues(
                    ReferenceModuleUtilities.ValidateModule(modules, module, isSelected),
                    ModuleUtilities.ValidateModule(modules, module, isSelected),
                    module.Id);
                AssertSameIssues(
                    ReferenceModuleUtilities.ValidateModuleEx(modules, module, isSelected).Select(x => x.ToLegacy()),
                    ModuleUtilities.ValidateModuleEx(modules, module, isSelected).Select(x => x.ToLegacy()),
                    $"{module.Id}, Ex");
            }
        }

        [TestCaseSource(nameof(Seeds))]
        public void ValidateModule_WithIsValid_MatchesReference(int seed)
        {
            var modules = Generate(seed);
            var isSelected = RandomSelection(modules, seed);
            var isValid = RandomSelection(modules, seed + 1000);
            foreach (var module in modules)
            {
                AssertSameIssues(
                    ReferenceModuleUtilities.ValidateModule(modules, module, isSelected, isValid),
                    ModuleUtilities.ValidateModule(modules, module, isSelected, isValid),
                    module.Id);
                foreach (var validateDependencies in new[] { true, false })
                {
                    AssertSameIssues(
                        ReferenceModuleUtilities.ValidateModuleEx(modules, module, isSelected, isValid, validateDependencies).Select(x => x.ToLegacy()),
                        ModuleUtilities.ValidateModuleEx(modules, module, isSelected, isValid, validateDependencies).Select(x => x.ToLegacy()),
                        $"{module.Id}, Ex, validateDependencies {validateDependencies}");
                }
            }
        }

        [TestCaseSource(nameof(Seeds))]
        public void ValidateModuleParts_MatchReference(int seed)
        {
            var modules = Generate(seed);
            var isSelected = RandomSelection(modules, seed);
            var isValid = RandomSelection(modules, seed + 1000);
            foreach (var module in modules)
            {
                AssertSameIssues(
                    ReferenceModuleUtilities.ValidateModuleDependenciesDeclarations(modules, module),
                    ModuleUtilities.ValidateModuleDependenciesDeclarations(modules, module),
                    $"{module.Id}, declarations");

                var expectedVisited = new HashSet<ModuleInfoExtended>();
                var actualVisited = new HashSet<ModuleInfoExtended>();
                AssertSameIssues(
                    ReferenceModuleUtilities.ValidateModuleDependencies(modules, module, expectedVisited, isSelected, isValid),
                    ModuleUtilities.ValidateModuleDependencies(modules, module, actualVisited, isSelected, isValid),
                    $"{module.Id}, dependencies");
                Assert.That(actualVisited.SetEquals(expectedVisited), Is.True, $"{module.Id}, visited");
            }
        }

        [Test]
        public void ValidateModule_DependencyCycleOfThree_ReportsInsteadOfOverflowing()
        {
            static ModuleInfoExtended Requiring(string id, string requirement) => new()
            {
                Id = id,
                Name = id,
                IsSingleplayerModule = true,
                DependentModules = new List<DependentModule> { new(requirement, ApplicationVersion.Empty, false) },
            };
            var modules = new List<ModuleInfoExtended> { Requiring("A", "B"), Requiring("B", "C"), Requiring("C", "A") };

            foreach (var module in modules)
            {
                var issues = ModuleUtilities.ValidateModule(modules, module, _ => true).ToList();
                Assert.That(issues.Select(x => x.Type), Does.Contain(ModuleIssueType.DependencyValidationError), module.Id);
            }
        }

        // A null ID was never supported, ModuleInfoExtended.GetHashCode throws on it. The indexes must not fail where
        // the lookups they replace didn't: the same result, or the same exception
        private static string Outcome(Func<IEnumerable<object>> action)
        {
            try
            {
                return string.Join(" | ", action().Select(x => x is ModuleIssue issue ? $"{issue.Type} {issue.SourceId} {issue.Reason}" : x.ToString()));
            }
            catch (Exception e)
            {
                return e.GetType().Name;
            }
        }

        [Test]
        public void NullIds_FailOrNotLikeBefore()
        {
            var modules = new List<ModuleInfoExtended>
            {
                new() { Id = "A", Name = "A", DependentModuleMetadatas = new List<DependentModuleMetadata> { new() { Id = null!, LoadType = LoadType.LoadBeforeThis } } },
                new() { Id = null!, Name = "Unnamed", DependentModules = new List<DependentModule> { new("A", ApplicationVersion.Empty, false) } },
                new() { Id = "B", Name = "B", ModulesToLoadAfterThis = new List<DependentModule> { new(null!, ApplicationVersion.Empty, false) } },
            };
            var isSelected = RandomSelection(modules, 1);

            foreach (var module in modules)
            {
                var name = module.Id ?? "null";
                foreach (var options in new[] { new ModuleSorterOptions(), new ModuleSorterOptions(true, true) })
                {
                    Assert.That(
                        Outcome(() => ModuleUtilities.GetDependencies(modules, module, options)),
                        Is.EqualTo(Outcome(() => ReferenceModuleUtilities.GetDependencies(modules, module, options))),
                        $"{name}, {options}");
                }
                Assert.That(
                    Outcome(() => ModuleUtilities.ValidateModule(modules, module, isSelected)),
                    Is.EqualTo(Outcome(() => ReferenceModuleUtilities.ValidateModule(modules, module, isSelected))),
                    $"{name}, ValidateModule");
                Assert.That(
                    Outcome(() => ModuleUtilities.ValidateModuleDependenciesDeclarations(modules, module)),
                    Is.EqualTo(Outcome(() => ReferenceModuleUtilities.ValidateModuleDependenciesDeclarations(modules, module))),
                    $"{name}, declarations");
            }
        }
    }
}
