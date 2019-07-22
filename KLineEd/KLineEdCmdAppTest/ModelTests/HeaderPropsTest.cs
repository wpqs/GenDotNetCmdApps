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
        public void GetPropertyUpdateOverwriteTest()
        {
            var property = "hello world";
            Assert.Equal("helloXworld", HeaderProps.GetPropertyUpdate(property, "X", 5, property.Length, false));

            Assert.Equal("Xello world", HeaderProps.GetPropertyUpdate(property, "X", 0, property.Length, false));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", -1, property.Length, false));

            Assert.Equal("hello worlX", HeaderProps.GetPropertyUpdate(property, "X", property.Length - 1, property.Length, false));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", property.Length, property.Length, false));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", -1, property.Length - 1, false));

            Assert.Equal("hello moond", HeaderProps.GetPropertyUpdate(property, "moon", 6, property.Length, false));
            Assert.Equal("hello moons", HeaderProps.GetPropertyUpdate(property, "moons", 6, property.Length, false));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, "moonsx", 6, property.Length, false));
        }

        [Fact]
        public void GetPropertyUpdateInsertTest()
        {
            var property = "hello world";  //index 0-10  insert before index

            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", 5, property.Length, true));

            Assert.Equal("helloX world", HeaderProps.GetPropertyUpdate(property, "X", 5, property.Length + 1, true));

            Assert.Equal("Xhello world", HeaderProps.GetPropertyUpdate(property, "X", 0, property.Length + 1, true));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", -1, property.Length + 1, true));

            Assert.Equal("hello worXld", HeaderProps.GetPropertyUpdate(property, "X", 9, property.Length + 1, true));
            Assert.Equal("hello worlXd", HeaderProps.GetPropertyUpdate(property, "X", 10, property.Length + 1, true));
            Assert.Equal("hello worldX", HeaderProps.GetPropertyUpdate(property, "X", 11, property.Length + 1, true));

            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", 12, property.Length + 1, true));

            var insertText = " wonderful";
            Assert.Equal($"hello wonderful world", HeaderProps.GetPropertyUpdate(property, insertText, 5, property.Length + insertText.Length, true));
            Assert.Equal($" wonderfulhello world", HeaderProps.GetPropertyUpdate(property, insertText, 0, property.Length + insertText.Length, true));
            Assert.Equal($"hello world wonderful", HeaderProps.GetPropertyUpdate(property, insertText, property.Length, property.Length + insertText.Length, true));

        }

        [Fact]
        public void GetPropertyUpdateBadParamTest()
        {
            var property = "hello world";

            Assert.Null(HeaderProps.GetPropertyUpdate(null, "X", 5, property.Length, false));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, null, 5, property.Length, false));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", -1, property.Length, false));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", property.Length + 1, property.Length, false));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", 5, 0, false));
            Assert.Null(HeaderProps.GetPropertyUpdate(property, "X", 5, -1, false));
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
            var props = new HeaderProps(6);
  
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
            var props = new HeaderProps();
            props.SetMaxPropertyLength(5);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 0));
            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Author, 4));
            Assert.Equal(4, props.Cursor.ColIndex);

            Assert.False(props.SetCursor(HeaderProps.CursorRow.Author, 5));
            Assert.Equal(4, props.Cursor.ColIndex);
        }

        [Fact]
        public void GetPropsEditViewRowIndexTest()
        {
            var props = new HeaderProps(15);

            Assert.Equal(0, props.Cursor.ColIndex);
            Assert.Equal(0, props.Cursor.RowIndex);

            Assert.Equal(HeaderProps.CursorRow.Author, props.GetPropsRowIndex(ChapterModel.RowState.Current));
            Assert.Equal(HeaderProps.CursorRow.Project, props.GetPropsRowIndex(ChapterModel.RowState.Next));
            Assert.Equal(HeaderProps.CursorRow.Title, props.GetPropsRowIndex(ChapterModel.RowState.Previous));

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Project, 6));
            Assert.Equal(6, props.Cursor.ColIndex);
            Assert.Equal(1, props.Cursor.RowIndex);

            Assert.Equal(HeaderProps.CursorRow.Project, props.GetPropsRowIndex(ChapterModel.RowState.Current));
            Assert.Equal(HeaderProps.CursorRow.Title, props.GetPropsRowIndex(ChapterModel.RowState.Next));
            Assert.Equal(HeaderProps.CursorRow.Author, props.GetPropsRowIndex(ChapterModel.RowState.Previous));

            Assert.True(props.SetCursor(HeaderProps.CursorRow.Title, 10));
            Assert.Equal(10, props.Cursor.ColIndex);
            Assert.Equal(2, props.Cursor.RowIndex);

            Assert.Equal(HeaderProps.CursorRow.Title, props.GetPropsRowIndex(ChapterModel.RowState.Current));
            Assert.Equal(HeaderProps.CursorRow.Author, props.GetPropsRowIndex(ChapterModel.RowState.Next));
            Assert.Equal(HeaderProps.CursorRow.Project, props.GetPropsRowIndex(ChapterModel.RowState.Previous));
        }

        [Fact]
        public void SetPropsEditViewWordTest()
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
            Assert.True(props.SetPropsWord("P.Q.", true, false, false));
            Assert.Equal("W.P.Q. Stott", props.Author);

        }
    }
}
