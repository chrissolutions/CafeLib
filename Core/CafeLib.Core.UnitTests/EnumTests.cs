using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CafeLib.Core.Support;
using CafeLib.Core.Extensions;
using Xunit;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace CafeLib.Core.UnitTests
{
    public class EnumTests
    {
        public enum TestStatus
        {
            [EnumMember(Value="none")]
            [Description("none")]
            None,
            [EnumMember(Value = "ready")]
            [Description("ready")]
            Ready,
            [EnumMember(Value = "uploaded")]
            [Description("uploaded")]
            Uploaded,
            [EnumMember(Value = "failed")]
            [Description("failed")]
            Failed,
            [EnumMember(Value = "incomplete")]
            [Description("incomplete")]
            Incomplete,
            [EnumMember(Value = "cancelled")]
            [Description("cancelled")]
            Cancelled,
            [EnumMember(Value = "expired")]
            [Description("expired")]
            Expired
        }

        [Theory]
        [InlineData(TestStatus.None, "None")]
        [InlineData(TestStatus.Ready, "Ready")]
        [InlineData(TestStatus.Uploaded, "Uploaded")]
        [InlineData(TestStatus.Failed, "Failed")]
        [InlineData(TestStatus.Incomplete, "Incomplete")]
        [InlineData(TestStatus.Cancelled, "Cancelled")]
        [InlineData(TestStatus.Expired, "Expired")]
        public void GetEnumName_Test(TestStatus status, string expected)
        {
            Assert.Equal(expected, status.GetName());
        }

        [Fact]
        public void GetEnumNames_Test()
        {
            var names = EnumExtensions.GetNames<TestStatus>();
            Assert.Equal(7, names.Length);

            var expectedNames = new string[]
            {
                "None",
                "Ready",
                "Uploaded",
                "Failed",
                "Incomplete",
                "Cancelled",
                "Expired"
            };

            Assert.True(expectedNames.SequenceEqual(names));
        }

        [Fact]
        public void GetEnumValues_Test()
        {
            var values = EnumExtensions.GetEnumValues<TestStatus>().ToArray();
            Assert.Equal(7, values.Length);

            var expectedValues = new TestStatus[]
            {
                TestStatus.None,
                TestStatus.Ready,
                TestStatus.Uploaded,
                TestStatus.Failed,
                TestStatus.Incomplete,
                TestStatus.Cancelled,
                TestStatus.Expired
            };

            Assert.True(expectedValues.SequenceEqual(values));
        }

        [Theory]
        [InlineData(TestStatus.None)]
        [InlineData(TestStatus.Ready)]
        [InlineData(TestStatus.Uploaded)]
        [InlineData(TestStatus.Failed)]
        [InlineData(TestStatus.Incomplete)]
        [InlineData(TestStatus.Cancelled)]
        [InlineData(TestStatus.Expired)]
        public void GetDescriptor_Test(TestStatus status)
        {
            Assert.Equal(status.GetName().ToLower(), status.GetDescriptor());
        }

        [Theory]
        [InlineData(TestStatus.None)]
        [InlineData(TestStatus.Ready)]
        [InlineData(TestStatus.Uploaded)]
        [InlineData(TestStatus.Failed)]
        [InlineData(TestStatus.Incomplete)]
        [InlineData(TestStatus.Cancelled)]
        [InlineData(TestStatus.Expired)]
        public void GetEnumMember_Test(TestStatus status)
        {
            Assert.Equal(status.GetName().ToLower(), status.GetEnumMemberValue());
        }
    }
}
