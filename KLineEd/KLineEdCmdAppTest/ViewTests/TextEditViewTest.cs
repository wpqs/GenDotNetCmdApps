using KLineEdCmdApp.Controller;
using KLineEdCmdAppTest.TestSupport;
using Xunit;

namespace KLineEdCmdAppTest.ViewTests
{
    public class TextEditViewTest : IClassFixture<ModelTextEditViewFixture>
    {
        private readonly ModelTextEditViewFixture _fixture;

        public TextEditViewTest(ModelTextEditViewFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void SetupTest()
        {
            Assert.True(_fixture.View.Ready);

            Assert.Equal(25, _fixture.View.WindowHeight);
            Assert.Equal(93, _fixture.View.WindowWidth);

        }

        [Fact]
        public void OnUpdateTest()
        {

            Assert.Equal(TestConst.UnitTestNone, _fixture.Error);
            Assert.True(_fixture.Model.Ready);
            Assert.True(_fixture.View.Ready);

           _fixture.Model.SetEditorHelpLine(TextEditingController.EditorHelpText);

            _fixture.Model.AppendChar('a');
            Assert.Equal("a", _fixture.View.LastTerminalOutput);

            _fixture.Model.AppendWord("hello");
            Assert.Equal("hello", _fixture.View.LastTerminalOutput);

            _fixture.Model.Refresh();
            Assert.Equal("a hello", _fixture.View.LastTerminalOutput);

            _fixture.Model.AppendWord("world");
            Assert.Equal("world", _fixture.View.LastTerminalOutput);

            _fixture.Model.Refresh();
            Assert.Equal("a hello world", _fixture.View.LastTerminalOutput);

            _fixture.Model.AppendChar('s');
            Assert.Equal("s", _fixture.View.LastTerminalOutput);

            _fixture.Model.Refresh();
            Assert.Equal("a hello worlds", _fixture.View.LastTerminalOutput);
            Assert.Equal(1, _fixture.Model.GetTextLineCount());

            _fixture.Model.AppendLine("hello 1234  byebye");
            Assert.Equal("hello 1234  byebye", _fixture.View.LastTerminalOutput);
            Assert.Equal(2, _fixture.Model.GetTextLineCount());

            _fixture.Model.Refresh();
            Assert.Equal("hello 1234  byebye", _fixture.View.LastTerminalOutput);
        }
    }
}
