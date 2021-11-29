using System;
using CafeLib.Core.Support;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class SemVerTests
    {
        [Fact]
        public void SemVer_Empty_Test()
        {
            var semVer = new SemVer();
            Assert.Equal(SemVer.Empty, semVer);
        }

        [Fact]
        public void SemVer_Parse_Test()
        {
            const string semverTest = "1.0.0-alpha.1+001";
            var semVer = SemVer.Parse(semverTest);
            Assert.Equal("001", semVer.Build);
            Assert.Equal(1, semVer.Major);
            Assert.Equal(0, semVer.Minor);
            Assert.Equal(0, semVer.Patch);
            Assert.Equal("alpha.1", semVer.Prerelease);
        }

        [Fact]
        public void SemVer_Parse_Throw_Test()
        {
            const string badSemver1 = "bad semver";
            Assert.Throws<ArgumentException>(() => SemVer.Parse(badSemver1));

            const string badSemver2 = "1.0.a-alpha.1+001";
            Assert.Throws<ArgumentException>(() => SemVer.Parse(badSemver2));

            const string badSemver3 = "1";
            Assert.Throws<InvalidOperationException>(() => SemVer.Parse(badSemver3, true));
        }

        [Fact]
        public void SemVer_TryParse_Test()
        {
            const string badSemver = "bad semver";
            Assert.False(SemVer.TryParse(badSemver, out _));

            const string semverTest = "1.0.0-alpha.1+001";
            Assert.True(SemVer.TryParse(semverTest, out var semVer));
            Assert.Equal("001", semVer.Build);
            Assert.Equal(1, semVer.Major);
            Assert.Equal(0, semVer.Minor);
            Assert.Equal(0, semVer.Patch);
            Assert.Equal("alpha.1", semVer.Prerelease);
        }

        [Fact]
        public void SemVer_SystemVersion_Test()
        {
            var version = new Version(2, 2, 1000);
            var semVer = new SemVer(version);
            Assert.Equal("1000", semVer.Build);
            Assert.Equal(2, semVer.Major);
            Assert.Equal(2, semVer.Minor);
            Assert.Equal(0, semVer.Patch);
            Assert.True(string.IsNullOrWhiteSpace(semVer.Prerelease));
        }

        [Fact]
        public void SemVer_Comparison_Test()
        {
            const string semverTest = "1.0.0-alpha.1+001";
            var semVer1 = SemVer.Parse(semverTest);

            var version = new Version(2, 2, 1000);
            var semVer2 = new SemVer(version);

            Assert.True(semVer1 < semVer2);

            var semVer3 = new SemVer(new Version(2, 2, 1001));
            Assert.True(semVer2 < semVer3);

            Assert.True(semVer3.MatchPrecedence(semVer2));
        }

        [Fact]
        public void SemVer_Copy_Test()
        {
            const string semverTest = "1.0.0-alpha.1+001";
            var semVer1 = SemVer.Parse(semverTest);

            var semVer2 = semVer1.Copy();
            Assert.Equal(semVer1.Major, semVer2.Major);
            Assert.Equal(semVer1.Minor, semVer2.Minor);
            Assert.Equal(semVer1.Patch, semVer2.Patch);
            Assert.Equal(semVer1.Prerelease, semVer2.Prerelease);
            Assert.Equal(semVer1.Build, semVer2.Build);
        }

        [Fact]
        public void SemVer_Copy_Change_Test()
        {
            const string semverTest = "1.0.0-alpha.1+001";
            var semVer1 = SemVer.Parse(semverTest);

            var semVer2 = semVer1.Copy(null, null, null, null, "002");
            Assert.Equal(semVer1.Major, semVer2.Major);
            Assert.Equal(semVer1.Minor, semVer2.Minor);
            Assert.Equal(semVer1.Patch, semVer2.Patch);
            Assert.Equal(semVer1.Prerelease, semVer2.Prerelease);
            Assert.Equal("002", semVer2.Build);
        }
    }
}