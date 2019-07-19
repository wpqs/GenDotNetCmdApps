using System;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Model.Base;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    public class HeaderPropsTest
    {
        [Fact]
        public void ToStringTest()
        {
            var data = $"Author: Wills Project: A23 Title: B23 File: C23";
            var info = new HeaderProps();
            Assert.True(info.InitialiseFromString(data).GetResult());
            Assert.False(info.IsError());
            Assert.Equal(data, info.ToString());
            Assert.Equal("Wills", info.Author);
            Assert.Equal("A23", info.Project);
            Assert.Equal("B23", info.Title);
            Assert.Equal("C23", info.PathFileName);
        }

        [Fact]
        public void GetReportTest()
        {
            var data = $"Author: Wills{Environment.NewLine}Project: A23{Environment.NewLine}Title: B23{Environment.NewLine}File: C23";
            var info = new HeaderProps();
            Assert.True(info.InitialiseFromString(data).GetResult());
            Assert.False(info.IsError());
            Assert.StartsWith(data, info.GetReport());
        }

        [Fact]
        public void InitialiseFromStringTest()
        {
            var data = $"Author: Wills Project: A23 Title: B23 File: C23";
            var info = new HeaderProps();
            Assert.True(info.InitialiseFromString(data).GetResult());
            Assert.False(info.IsError());
            Assert.Equal(data, info.ToString());

            Assert.True(info.InitialiseFromString($"Author: WillsProject: A23Title: B23File: C23").GetResult());
            Assert.False(info.IsError());
            Assert.Equal(data, info.ToString());
            Assert.Equal("Wills", info.Author);
            Assert.Equal("A23", info.Project);
            Assert.Equal("B23", info.Title);
            Assert.Equal("C23", info.PathFileName);
        }

        [Fact]
        public void NullTest()
        {
            var info = new HeaderProps();
            Assert.False(info.InitialiseFromString(null).GetResult());
            Assert.True(info.IsError());
            Assert.Equal(HeaderBase.ValueNotSet, info.ToString());
            Assert.Equal(HeaderBase.PropertyNotSet, info.Project);
            Assert.Equal(HeaderBase.PropertyNotSet, info.Title);
            Assert.Equal(HeaderBase.PropertyNotSet, info.PathFileName);
        }

        [Fact]
        public void MissingAuthorPropertyTest()
        {
            var data = $"Project: A23 Title: B23 File: C23";
            var info = new HeaderProps();
            Assert.False(info.InitialiseFromString(data).GetResult());
            Assert.True(info.IsError());
            Assert.Equal(HeaderBase.ValueNotSet, info.ToString());
            Assert.Equal(HeaderBase.PropertyNotSet, info.Author);
            Assert.Equal(HeaderBase.PropertyNotSet, info.Project);
            Assert.Equal(HeaderBase.PropertyNotSet, info.Title);
            Assert.Equal(HeaderBase.PropertyNotSet, info.PathFileName);
        }

        [Fact]
        public void MissingChapterPropertyTest()
        {
            var data = $"Author: Wills Project: A23 File: C23";
            var info = new HeaderProps();
            Assert.False(info.InitialiseFromString(data).GetResult());
            Assert.True(info.IsError());
            Assert.Equal(HeaderBase.ValueNotSet, info.ToString());
            Assert.Equal(HeaderBase.PropertyNotSet, info.Author);
            Assert.Equal(HeaderBase.PropertyNotSet, info.Project);
            Assert.Equal(HeaderBase.PropertyNotSet, info.Title);
            Assert.Equal(HeaderBase.PropertyNotSet, info.PathFileName);
        }

        [Fact]
        public void MissingFilePropertyTest()
        {
            var data = $"Author: Wills Project: A23 Title: B23";
            var info = new HeaderProps();
            Assert.False(info.InitialiseFromString(data).GetResult());
            Assert.True(info.IsError());
            Assert.Equal(HeaderBase.ValueNotSet, info.ToString());
            Assert.Equal(HeaderBase.PropertyNotSet, info.Author);
            Assert.Equal(HeaderBase.PropertyNotSet, info.Project);
            Assert.Equal(HeaderBase.PropertyNotSet, info.Title);
            Assert.Equal(HeaderBase.PropertyNotSet, info.PathFileName);
        }
    }
}
