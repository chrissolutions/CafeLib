using System;
using System.Text.RegularExpressions;

namespace CafeLib.Core.Versioning
{
    /// <summary>
    /// A semantic version implementation.
    /// Conforms to v2.0.0 of http://semver.org/
    /// </summary>
    public struct SemVer : IEquatable<SemVer>, IComparable<SemVer>, IComparable
    {
        private static readonly Regex ParseEx =
            new Regex(@"^(?<major>\d+)" +
                @"(\.(?<minor>\d+))?" +
                @"(\.(?<patch>\d+))?" +
                @"(\-(?<pre>[0-9A-Za-z\-\.]+))?" +
                @"(\+(?<build>[0-9A-Za-z\-\.]+))?$",
                RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

        public static SemVer Empty = new SemVer();

        /// <summary>
        /// Initializes a new instance of the <see cref="SemVer" /> class.
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="patch">The patch version.</param>
        /// <param name="prerelease">The prerelease version (eg. "alpha").</param>
        /// <param name="build">The build eg ("nightly.232").</param>
        public SemVer(int major = 0, int minor = 0, int patch = 0, string prerelease = null, string build = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;

            Prerelease = prerelease ?? string.Empty;
            Build = build ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemVer"/> class.
        /// </summary>
        /// <param name="version">The <see cref="System.Version"/> that is used to initialize 
        /// the Major, Minor, Patch and Build properties.</param>
        public SemVer(Version version)
        {
            if (version == null)
                throw new ArgumentNullException(nameof(version));

            Major = version.Major;
            Minor = version.Minor;
            Patch = (version.Revision >= 0) ? version.Revision : 0;
            Prerelease = string.Empty;
            Build = version.Build > 0 ? version.Build.ToString() : string.Empty;
        }

        /// <summary>
        /// Parses the specified string to a semantic version.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <param name="strict">If set to <c>true</c> minor and patch version are required, else they default to 0.</param>
        /// <returns>The SemVersion object.</returns>
        /// <exception cref="System.InvalidOperationException">When a invalid version string is passed.</exception>
        public static SemVer Parse(string version, bool strict = false)
        {
            var match = ParseEx.Match(version);
            if (!match.Success)
                throw new ArgumentException("Invalid version.", nameof(version));

            var major = int.Parse(match.Groups["major"].Value);

            var minorMatch = match.Groups["minor"];
            int minor = 0;
            if (minorMatch.Success)
            {
                minor = int.Parse(minorMatch.Value);
            }
            else if (strict)
            {
                throw new InvalidOperationException("Invalid version (no minor version given in strict mode)");
            }

            var patchMatch = match.Groups["patch"];
            var patch = 0;
            if (patchMatch.Success)
            {
                patch = int.Parse(patchMatch.Value);
            }
            else if (strict)
            {
                throw new InvalidOperationException("Invalid version (no patch version given in strict mode)");
            }

            var prerelease = match.Groups["pre"].Value;
            var build = match.Groups["build"].Value;

            return new SemVer(major, minor, patch, prerelease, build);
        }

        /// <summary>
        /// Parses the specified string to a semantic version.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <param name="semver">When the method returns, contains a SemVersion instance equivalent 
        /// to the version string passed in, if the version string was valid, or <c>null</c> if the 
        /// version string was not valid.</param>
        /// <param name="strict">If set to <c>true</c> minor and patch version are required, else they default to 0.</param>
        /// <returns><c>False</c> when a invalid version string is passed, otherwise <c>true</c>.</returns>
        public static bool TryParse(string version, out SemVer semver, bool strict = false)
        {
            try
            {
                semver = Parse(version, strict);
                return true;
            }
            catch (Exception)
            {
                semver = null;
                return false;
            }
        }

        /// <summary>
        /// Tests the specified versions for equality.
        /// </summary>
        /// <param name="versionA">The first version.</param>
        /// <param name="versionB">The second version.</param>
        /// <returns>If versionA is equal to versionB <c>true</c>, else <c>false</c>.</returns>
        public static bool Equals(SemVer versionA, SemVer versionB)
        {
            return versionA.Equals(versionB);
        }

        /// <summary>
        /// Compares the specified versions.
        /// </summary>
        /// <param name="versionA">The version to compare to.</param>
        /// <param name="versionB">The version to compare against.</param>
        /// <returns>If versionA &lt; versionB <c>&lt; 0</c>, if versionA &gt; versionB <c>&gt; 0</c>,
        /// if versionA is equal to versionB <c>0</c>.</returns>
        public static int Compare(SemVer versionA, SemVer versionB)
        {
            return versionA.CompareTo(versionB);
        }

        /// <summary>
        /// Make a copy of the current instance with optional altered fields. 
        /// </summary>
        /// <param name="major">The major version.</param>
        /// <param name="minor">The minor version.</param>
        /// <param name="patch">The patch version.</param>
        /// <param name="prerelease">The prerelease text.</param>
        /// <param name="build">The build text.</param>
        /// <returns>The new version object.</returns>
        public SemVer Change(int? major = null, int? minor = null, int? patch = null, string prerelease = null, string build = null)
        {
            return new SemVer(
                major ?? Major,
                minor ?? Minor,
                patch ?? Patch,
                prerelease ?? Prerelease,
                build ?? Build);
        }

        /// <summary>
        /// Gets the major version.
        /// </summary>
        /// <value>
        /// The major version.
        /// </value>
        public int Major { get; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        /// <value>
        /// The minor version.
        /// </value>
        public int Minor { get; }

        /// <summary>
        /// Gets the patch version.
        /// </summary>
        /// <value>
        /// The patch version.
        /// </value>
        public int Patch { get; }

        /// <summary>
        /// Gets the pre-release version.
        /// </summary>
        /// <value>
        /// The pre-release version.
        /// </value>
        public string Prerelease { get; }

        /// <summary>
        /// Gets the build version.
        /// </summary>
        /// <value>
        /// The build version.
        /// </value>
        public string Build { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var version = "" + Major + "." + Minor + "." + Patch;
            if (!string.IsNullOrEmpty(Prerelease))
                version += "-" + Prerelease;
            if (!string.IsNullOrEmpty(Build))
                version += "+" + Build;
            return version;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates 
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the 
        /// other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has these meanings: Value Meaning Less than zero 
        ///  This instance precedes <paramref name="obj" /> in the sort order. 
        ///  Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. i
        ///  Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            return CompareTo((SemVer)obj);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates 
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the 
        /// other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has these meanings: Value Meaning Less than zero 
        ///  This instance precedes <paramref name="other" /> in the sort order. 
        ///  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. i
        ///  Greater than zero This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo(SemVer other)
        {
            var r = CompareByPrecedence(other);
            if (r != 0)
                return r;

            r = CompareComponent(Build, other.Build);
            return r;
        }

        /// <summary>
        /// Compares to semantic versions by precedence. This does the same as a Equals, but ignores the build information.
        /// </summary>
        /// <param name="other">The semantic version.</param>
        /// <returns><c>true</c> if the version precedence matches.</returns>
        public bool PrecedenceMatches(SemVer other)
        {
            return CompareByPrecedence(other) == 0;
        }

        /// <summary>
        /// Compares to semantic versions by precedence. This does the same as a Equals, but ignores the build information.
        /// </summary>
        /// <param name="other">The semantic version.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has these meanings: Value Meaning Less than zero 
        ///  This instance precedes <paramref name="other" /> in the version precedence.
        ///  Zero This instance has the same precedence as <paramref name="other" />. i
        ///  Greater than zero This instance has creater precedence as <paramref name="other" />.
        /// </returns>
        public int CompareByPrecedence(SemVer other)
        {
            var r = Major.CompareTo(other.Major);
            if (r != 0) return r;

            r = Minor.CompareTo(other.Minor);
            if (r != 0) return r;

            r = Patch.CompareTo(other.Patch);
            if (r != 0) return r;

            r = CompareComponent(Prerelease, other.Prerelease, true);
            return r;
        }

        private static int CompareComponent(string a, string b, bool lower = false)
        {
            var aEmpty = string.IsNullOrEmpty(a);
            var bEmpty = string.IsNullOrEmpty(b);
            if (aEmpty && bEmpty)
                return 0;

            if (aEmpty)
                return lower ? 1 : -1;
            if (bEmpty)
                return lower ? -1 : 1;

            var aComps = a.Split('.');
            var bComps = b.Split('.');

            var minLen = Math.Min(aComps.Length, bComps.Length);
            for (var i = 0; i < minLen; i++)
            {
                var ac = aComps[i];
                var bc = bComps[i];
                var isanum = int.TryParse(ac, out var anum);
                var isbnum = int.TryParse(bc, out var bnum);
                int r;
                if (isanum && isbnum)
                {
                    r = anum.CompareTo(bnum);
                    if (r != 0) return anum.CompareTo(bnum);
                }
                else
                {
                    if (isanum)
                        return -1;
                    if (isbnum)
                        return 1;
                    r = string.CompareOrdinal(ac, bc);
                    if (r != 0)
                        return r;
                }
            }

            return aComps.Length.CompareTo(bComps.Length);
        }

        public bool Equals(SemVer other)
        {
            return Major == other.Major &&
                Minor == other.Minor &&
                Patch == other.Patch &&
                string.Equals(Prerelease, other.Prerelease, StringComparison.Ordinal) &&
                string.Equals(Build, other.Build, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            try
            {
                return obj != null && Equals((SemVer)obj);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = Major.GetHashCode();
                result = result * 31 + Minor.GetHashCode();
                result = result * 31 + Patch.GetHashCode();
                result = result * 31 + Prerelease.GetHashCode();
                result = result * 31 + Build.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Implicit conversion from string to SemVersion.
        /// </summary>
        /// <param name="version">The semantic version.</param>
        /// <returns>The SemVersion object.</returns>
        public static implicit operator SemVer(string version) => Parse(version);

        /// <summary>
        /// The override of the equals operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator ==(SemVer left, SemVer right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// The override of the un-equal operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is not equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator !=(SemVer left, SemVer right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// The override of the greater operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is greater than right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator >(SemVer left, SemVer right)
        {
            return Compare(left, right) > 0;
        }

        /// <summary>
        /// The override of the greater than or equal operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is greater than or equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator >=(SemVer left, SemVer right)
        {
            return left == right || left > right;
        }

        /// <summary>
        /// The override of the less operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is less than right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator <(SemVer left, SemVer right)
        {
            return Compare(left, right) < 0;
        }

        /// <summary>
        /// The override of the less than or equal operator. 
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>If left is less than or equal to right <c>true</c>, else <c>false</c>.</returns>
        public static bool operator <=(SemVer left, SemVer right)
        {
            return left == right || left < right;
        }
    }
}
