using NUnit.Framework;

using System.Linq;

namespace Bannerlord.ModuleManager.Tests
{
    public class ModuleStorageTests
    {
        [Test]
        public void Test([Values] ModuleListTemplates templateEnum)
        {
            var template = new ModuleStorage(templateEnum);
            var sorted = ModuleSorter.Sort(template.ModuleInfoExtendeds);
            Assert.AreEqual(template.ExpectedIdOrder, sorted.Select(x => x.Id));
        }
    }
}