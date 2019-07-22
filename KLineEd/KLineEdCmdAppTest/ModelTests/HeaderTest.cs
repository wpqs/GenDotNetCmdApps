using System;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Model.Base;
using KLineEdCmdAppTest.TestSupport;
using Xunit;
// ReSharper disable All

namespace KLineEdCmdAppTest.ModelTests
{
    public class HeaderTest
    {
        [Fact]
        public void SetDefaultsTest()
        {
            Header header = new Header();
            Assert.True(header.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName));
            Assert.StartsWith($"{Environment.NewLine}Author: [author not set]", header.GetChapterReport());
        }

        [Fact]
        public void GetChapterReportNotSetTest()
        {
            Header header = new Header();
            Assert.StartsWith($"{Environment.NewLine}{HeaderElementBase.ValueNotSet}", header.GetChapterReport());
        }

        [Fact]
        public void GetLastSessionNullTest()
        {
            Header header = new Header();
            Assert.Null(header.GetLastSession());
        }

        [Fact]
        public void CreateOneNewSessionTest()
        {
            Header header = new Header();
            Assert.True(header.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName));
            Assert.True(header.CreateNewSession(1).GetResult());

            Assert.True(header.Validate().GetResult());
            Assert.Equal(1, header.GetSessionCount());
            Assert.NotNull(header.GetLastSession());

            Assert.True(header.GetLastSession().SetDefaults(5,1));
            Assert.Equal(5, header.GetLastSession().SessionNo);
        }

        [Fact]
        public void CreateThreeNewSessionsTest()
        {
            Header header = new Header();
            Assert.True(header.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName));
            Assert.True(header.CreateNewSession(7).GetResult());
            Assert.True(header.CreateNewSession(8).GetResult());
            Assert.True(header.CreateNewSession(9).GetResult());

            Assert.True(header.Validate().GetResult());
            Assert.Equal(3, header.GetSessionCount());
            Assert.NotNull(header.GetLastSession());
            Assert.Equal(3, header.GetLastSession().SessionNo);
            Assert.Equal(9, header.GetLastSession().StartLine);

            Assert.True(header.GetLastSession().SetDefaults(5, 1));
            Assert.Equal(5, header.GetLastSession().SessionNo);
            Assert.Equal(1, header.GetLastSession().StartLine);
        }

        [Fact]
        public void CreateCreateNewSessionVariousLineNoTest()
        {
            Header header = new Header();
            Assert.True(header.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName));
            Assert.True(header.CreateNewSession(11).GetResult());
            Assert.True(header.Validate().GetResult());

            Assert.Equal(1, header.GetSessionCount());
            Assert.Equal(1, header.GetLastSession().SessionNo);
            Assert.Equal(11,header.GetLastSession().StartLine);

            Assert.True(header.CreateNewSession(77).GetResult());

            Assert.True(header.Validate().GetResult());
            Assert.Equal(2, header.GetSessionCount());
            Assert.NotNull(header.GetLastSession());
            Assert.Equal(2, header.GetLastSession().SessionNo);
            Assert.Equal(77, header.GetLastSession().StartLine);

            Assert.True(header.GetLastSession().SetDefaults(2, 1));

            Assert.True(header.Validate().GetResult());
            Assert.Equal(2, header.GetLastSession().SessionNo);
            Assert.Equal(1, header.GetLastSession().StartLine);
        }

        [Fact]
        public void CreateCreateNewSessionDupLineNoTest()
        {
            Header header = new Header();
            Assert.True(header.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName));
            Assert.True(header.CreateNewSession(11).GetResult());

            Assert.True(header.Validate().GetResult());
            Assert.Equal(1, header.GetSessionCount());
            Assert.Equal(1, header.GetLastSession().SessionNo);
            Assert.Equal(11, header.GetLastSession().StartLine);

            Assert.True(header.CreateNewSession(11).GetResult());       //duplicate line numbers are allowed as different sessions can start on same line
            Assert.True(header.Validate().GetResult());
            Assert.Equal(2, header.GetSessionCount());
            Assert.NotNull(header.GetLastSession());
            Assert.Equal(2, header.GetLastSession().SessionNo);
            Assert.Equal(11, header.GetLastSession().StartLine);
        }

        [Fact]
        public void CreateCreateNewSessionInvalidSessionNoTest()
        {
            Header header = new Header();
            Assert.True(header.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName));
            Assert.True(header.CreateNewSession(11).GetResult());
            Assert.True(header.CreateNewSession(11).GetResult());
            Assert.True(header.CreateNewSession(11).GetResult());

            Assert.True(header.Validate().GetResult());
            Assert.Equal(3, header.GetSessionCount());

            Assert.True(header.GetLastSession().SetDefaults(1, 1)); //change SessionNo from 3 to 1

            Assert.False(header.Validate().GetResult());
            Assert.Equal(3, header.GetSessionCount());
            Assert.Equal(1, header.GetLastSession().SessionNo);
 }
    }
}
