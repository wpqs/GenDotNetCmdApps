using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Model.Base;
using Xunit;

namespace KLineEdCmdAppTest.ModelTests
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class HeaderPropsTest
    {
        [Fact]
        public void ToStringTest()
        {
            var data = $"Author: Wills Project: A23 Title: B23 File: C23";
            var info = new HeaderProps(45);

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
            var info = new HeaderProps(45);

            Assert.True(info.InitialiseFromString(data).GetResult());
            Assert.False(info.IsError());
            Assert.StartsWith($"{Environment.NewLine}{data}", info.GetReport());
        }

        [Fact]
        public void InitialiseFromStringTest()
        {
            var data = $"Author: Wills Project: A23 Title: B23 File: C23";
            var info = new HeaderProps(45);

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
            var info = new HeaderProps(45);

            Assert.False(info.InitialiseFromString(null).GetResult());
            Assert.True(info.IsError());
            Assert.Equal(HeaderElementBase.ValueNotSet, info.ToString());
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Project);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Title);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.PathFileName);
        }

        [Fact]
        public void MissingAuthorPropertyTest()
        {
            var data = $"Project: A23 Title: B23 File: C23";
            var info = new HeaderProps(45);

            Assert.False(info.InitialiseFromString(data).GetResult());
            Assert.True(info.IsError());
            Assert.Equal(HeaderElementBase.ValueNotSet, info.ToString());
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Author);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Project);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Title);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.PathFileName);
        }

        [Fact]
        public void MissingChapterPropertyTest()
        {
            var data = $"Author: Wills Project: A23 File: C23";
            var info = new HeaderProps(45);

            Assert.False(info.InitialiseFromString(data).GetResult());
            Assert.True(info.IsError());
            Assert.Equal(HeaderElementBase.ValueNotSet, info.ToString());
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Author);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Project);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Title);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.PathFileName);
        }


        [Fact]
        public void MissingFilePropertyTest()
        {
            var data = $"Author: Wills Project: A23 Title: B23";
            var info = new HeaderProps(45);

            Assert.False(info.InitialiseFromString(data).GetResult());
            Assert.True(info.IsError());
            Assert.Equal(HeaderElementBase.ValueNotSet, info.ToString());
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Author);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Project);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.Title);
            Assert.Equal(HeaderElementBase.PropertyNotSet, info.PathFileName);
        }


        [Fact]
        public void GetPropertyUpdateBadParamTest()
        {
            var property = "hello world";

            Assert.Null(Body.GetLineUpdateText(null, "X", 5, property.Length, false));
            Assert.Null(Body.GetLineUpdateText(property, null, 5, property.Length, false));
            Assert.Null(Body.GetLineUpdateText(property, "X", -1, property.Length, false));
            Assert.Null(Body.GetLineUpdateText(property, "X", property.Length + 1, property.Length, false));
            Assert.Null(Body.GetLineUpdateText(property, "X", 5, 0, false));
            Assert.Null(Body.GetLineUpdateText(property, "X", 5, -1, false));
        }

        [Fact]
        public void SetPropsEditViewCursorRowTest()
        {
            var props = new HeaderProps(45);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 0));
            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Project, 0));
            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(1, props.Cursor.RowIndex);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Title, 0));
            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(2, props.Cursor.RowIndex);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.PathFileName, 0));
            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(3, props.Cursor.RowIndex);
        }

        [Fact]
        public void SetPropsEditViewCursorColTest()
        {
            var data = $"Author: 012345 Project: B1 Title: C12 File: D123";
            var props = new HeaderProps(6);
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());


            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 0));
            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 5));
            Assert.Equal(5, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.False(props.SetCursor(HeaderProps.CursorRow.Author, 6));
            Assert.Equal(5, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.False(props.SetCursor(HeaderProps.CursorRow.Author, -1));
            Assert.Equal(5, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);
        }

        [Fact]
        public void SetMaxPropertyLengthTest()
        {
            var data = $"Author: 01234 Project: B1 Title: C12 File: D123";
            var props = new HeaderProps(5);
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 0));
            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 4));
            Assert.Equal(4, props.Cursor.ColIndex);

            Assert.False(props.SetCursor(HeaderProps.CursorRow.Author, 5));
            Assert.Equal(4, props.Cursor.ColIndex);
        }

        [Fact]
        public void GetPropertyLengthTest()
        {
            var data = $"Author: A Project: B1 Title: C12 File: D123";
            var props = new HeaderProps(4);
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());

            Assert.Equal(1, props.GetPropertyLength(HeaderProps.CursorRow.Author));
            Assert.Equal(2, props.GetPropertyLength(HeaderProps.CursorRow.Project));
            Assert.Equal(3, props.GetPropertyLength(HeaderProps.CursorRow.Title));
            Assert.Equal(4, props.GetPropertyLength(HeaderProps.CursorRow.PathFileName));
        }

        [Fact]
        public void GetPropsEditViewRowIndexTest()
        {
            var data = $"Author: A Project: B1 Title: C12 File: D123";
            var props = new HeaderProps(4);
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());

            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.Equal(HeaderProps.CursorRow.Author, props.GetPropsRowIndex(ChapterModel.RowState.Current));
            Assert.Equal(HeaderProps.CursorRow.Project, props.GetPropsRowIndex(ChapterModel.RowState.Next));
            Assert.Equal(HeaderProps.CursorRow.Title, props.GetPropsRowIndex(ChapterModel.RowState.Previous));

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Project, 0));
            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(1, props.Cursor.RowIndex);

            Assert.Equal(HeaderProps.CursorRow.Project, props.GetPropsRowIndex(ChapterModel.RowState.Current));
            Assert.Equal(HeaderProps.CursorRow.Title, props.GetPropsRowIndex(ChapterModel.RowState.Next));
            Assert.Equal(HeaderProps.CursorRow.Author, props.GetPropsRowIndex(ChapterModel.RowState.Previous));

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Title, 2));
            Assert.Equal(2, props.Cursor.ColIndex);
            Assert.Equal(2, props.Cursor.RowIndex);

            Assert.Equal(HeaderProps.CursorRow.Title, props.GetPropsRowIndex(ChapterModel.RowState.Current));
            Assert.Equal(HeaderProps.CursorRow.Author, props.GetPropsRowIndex(ChapterModel.RowState.Next));
            Assert.Equal(HeaderProps.CursorRow.Project, props.GetPropsRowIndex(ChapterModel.RowState.Previous));
        }

        [Fact]
        public void SetPropsWordInsertTest()
        {
            var data = $"Author: Z23 Project: A23 Title: B23 File: C23";
            var props = new HeaderProps(7);

        //    Assert.True(props.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName)); //[author not set]
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());

            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.Equal("Z23", props.Author);
            Assert.Equal("A23", props.Project);
            Assert.Equal("B23", props.Title);
            Assert.Equal("C23", props.PathFileName);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 2));
            Assert.True(props.SetWord("PQ", true, false, false));
            Assert.Equal("Z2PQ3", props.Author);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Project, 1));
            Assert.Equal(1, props.Cursor.ColIndex);
            Assert.Equal(1, props.Cursor.RowIndex);
            Assert.True(props.SetWord("01", true, true, false));
            Assert.Equal("A 0123", props.Project);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Title, 1));
            Assert.Equal(1, props.Cursor.ColIndex);
            Assert.Equal(2, props.Cursor.RowIndex);
            Assert.True(props.SetWord("01", true, false, true));
            Assert.Equal("B01 23", props.Title);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.PathFileName, 1));
            Assert.Equal(1, props.Cursor.ColIndex);
            Assert.Equal(3, props.Cursor.RowIndex);
            Assert.True(props.SetWord("01", true, true, true));
            Assert.Equal("C 01 23", props.PathFileName);
        }

        [Fact]
        public void SetPropsWordOverwriteTest()
        {
            var data = $"Author: Z0123 Project: A0123 Title: B0123 File: C0123";
            var props = new HeaderProps(5);

            //    Assert.True(props.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName)); //[author not set]
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());

            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.Equal("Z0123", props.Author);
            Assert.Equal("A0123", props.Project);
            Assert.Equal("B0123", props.Title);
            Assert.Equal("C0123", props.PathFileName);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 2));
            Assert.True(props.SetWord("PQ", false, false, false));
            Assert.Equal("Z0PQ3", props.Author);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Project, 1));
            Assert.Equal(1, props.Cursor.ColIndex);
            Assert.Equal(1, props.Cursor.RowIndex);
            Assert.True(props.SetWord("45", false, true, false));
            Assert.Equal("A 453", props.Project);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Title, 1));
            Assert.Equal(1, props.Cursor.ColIndex);
            Assert.Equal(2, props.Cursor.RowIndex);
            Assert.True(props.SetWord("45", false, false, true));
            Assert.Equal("B45 3", props.Title);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.PathFileName, 1));
            Assert.Equal(1, props.Cursor.ColIndex);
            Assert.Equal(3, props.Cursor.RowIndex);
            Assert.True(props.SetWord("45", false, true, true));
            Assert.Equal("C 45 ", props.PathFileName);
        }

        [Fact]
        public void SetPropsWordMaxLengthTest()
        {
            var data = $"Author: Z23 Project: A23 Title: B23 File: C23";
            var props = new HeaderProps(4);

            //    Assert.True(props.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName)); //[author not set]
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());

            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.Equal("Z23", props.Author);
            Assert.Equal("A23", props.Project);
            Assert.Equal("B23", props.Title);
            Assert.Equal("C23", props.PathFileName);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Project, 1));
            Assert.Equal(1, props.Cursor.ColIndex);
            Assert.Equal(1, props.Cursor.RowIndex);
            Assert.True(props.SetWord("0", true, false,false));
            Assert.Equal("A023", props.Project);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Project, 1));
            Assert.Equal(1, props.Cursor.ColIndex);
            Assert.Equal(1, props.Cursor.RowIndex);
            Assert.False(props.SetWord("01", true, false, false));
            Assert.Equal("A023", props.Project);
        }

        [Fact]
        public void SetPropsCharTest()
        {
            var data = $"Author: W. Stott Project: A23 Title: B23 File: C23";
            var props = new HeaderProps(15);

            //    Assert.True(props.SetDefaults(TestConst.UnitTestInstanceTestsPathFileName)); //[author not set]
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());

            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);
            Assert.Equal("W. Stott", props.Author);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 2));
            Assert.True(props.SetChar('P', true));
            Assert.Equal("W.P Stott", props.Author);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 9));
            Assert.True(props.SetChar('?', true));
            Assert.Equal("W.P Stott?", props.Author);
        }

        [Fact]
        public void SetPropsDelCharTest()
        {
            var data = $"Author: Z23 Project: A23 Title: B23 File: C234";
            var props = new HeaderProps(5);
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());

            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);
            Assert.Equal("Z23", props.Author);
            Assert.Equal("A23", props.Project);
            Assert.Equal("B23", props.Title);
            Assert.Equal("C234", props.PathFileName);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 1));
            Assert.True(props.SetDelChar());
            Assert.Equal("Z3", props.Author);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Title, 0));
            Assert.True(props.SetDelChar());
            Assert.Equal("23", props.Title);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Project, 2));
            Assert.True(props.SetDelChar());
            Assert.Equal("A2", props.Project);

            Assert.False(props.SetCursor(HeaderProps.CursorRow.PathFileName, -1));
            Assert.True(props.SetCursor(HeaderProps.CursorRow.PathFileName, 4)); //allow one char after end of prop
            Assert.False(props.SetCursor(HeaderProps.CursorRow.PathFileName, 5)); //... but no more

            Assert.True(props.SetCursor(HeaderProps.CursorRow.PathFileName, 0));
            Assert.True(props.SetDelChar());
            Assert.Equal("234", props.PathFileName);
            Assert.True(props.SetDelChar());
            Assert.Equal("34", props.PathFileName);
            Assert.True(props.SetDelChar());
            Assert.Equal("4", props.PathFileName);
            Assert.True(props.SetDelChar());
            Assert.Equal("", props.PathFileName);

            Assert.False(props.SetDelChar());
            Assert.Equal("", props.PathFileName);
        }

        [Fact]
        public void SetPropsDelCharBackspaceTest()
        {
            var data = $"Author: Z23 Project: A23 Title: B23 File: C234";
            var props = new HeaderProps(5);
            Assert.True(props.InitialiseFromString(data).GetResult());
            Assert.False(props.IsError());

            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);
            Assert.Equal("Z23", props.Author);
            Assert.Equal("A23", props.Project);
            Assert.Equal("B23", props.Title);
            Assert.Equal("C234", props.PathFileName);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Title, 2));
            Assert.True(props.SetDelChar(true));
            Assert.Equal("B3", props.Title);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 0));
            Assert.False(props.SetDelChar(true));
            Assert.Equal("Z23", props.Author);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 3));
            Assert.True(props.SetDelChar(true));
            Assert.Equal("Z2", props.Author);
            Assert.True(props.SetDelChar(true));
            Assert.Equal("Z", props.Author);
            Assert.True(props.SetDelChar(true));
            Assert.Equal("", props.Author);
            Assert.False(props.SetDelChar(true));
            Assert.Equal("", props.Author);
        }
    }
}
