using NUnit.Framework;

namespace Bannerlord.ModuleManager.Tests
{
    public class ApplicationVersionTests
    {
        [TestCase("v1.2.3", 1, 2, 3, 0)]
        [TestCase("v1.2.3.4", 1, 2, 3, 4)]
        [TestCase("e1.0.0", 1, 0, 0, 0)]
        public void TryParse_WellFormed_Succeeds(string input, int major, int minor, int revision, int changeSet)
        {
            Assert.That(ApplicationVersion.TryParse(input, out var version), Is.True);
            Assert.That(version.Major, Is.EqualTo(major));
            Assert.That(version.Minor, Is.EqualTo(minor));
            Assert.That(version.Revision, Is.EqualTo(revision));
            Assert.That(version.ChangeSet, Is.EqualTo(changeSet));
        }

        // Regression: the segment-count guard used && instead of ||, so anything other than "starts with a dot and is neither
        // 3 nor 4 parts" slipped through and TryParse threw IndexOutOfRangeException instead of returning false.
        [TestCase("v1.2")]
        [TestCase("v1")]
        [TestCase("v")]
        [TestCase("")]
        [TestCase(".1.2.3")]
        [TestCase("v1.2.3.4.5")]
        public void TryParse_MalformedSegmentCount_ReturnsFalseWithoutThrowing(string input)
        {
            bool result = false;
            var version = ApplicationVersion.Empty;
            Assert.DoesNotThrow(() => result = ApplicationVersion.TryParse(input, out version));
            Assert.That(result, Is.False);
            Assert.That(version, Is.EqualTo(ApplicationVersion.Empty));
        }

        [Test]
        public void TryParse_Null_ReturnsFalse()
        {
            Assert.That(ApplicationVersion.TryParse(null, out var version), Is.False);
            Assert.That(version, Is.EqualTo(ApplicationVersion.Empty));
        }
    }
}
