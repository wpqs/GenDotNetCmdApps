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

            Assert.Equal(27, _fixture.View.WindowHeight);
         //   Assert.Equal(90, _fixture.View.WindowWidth);

        }

        [Fact]
        public void OnUpdateTest()
        {

            Assert.Equal(TestConst.UnitTestNone, _fixture.Error);
            Assert.True(_fixture.Model.Ready);
            Assert.True(_fixture.View.Ready);

           _fixture.Model.SetEditorHelpLine(TextEditingController.EditorHelpText);

            _fixture.Model.BodyInsertText('a'.ToString());
            Assert.Equal("a>", _fixture.View.LastConsoleOutput);

            _fixture.Model.BodyInsertText(" hello");
            Assert.Equal("a hello>", _fixture.View.LastConsoleOutput);

            _fixture.Model.Refresh();
            Assert.False(_fixture.View.IsErrorState());
            Assert.Equal("a hello>", _fixture.View.LastConsoleOutput);

            _fixture.Model.BodyInsertText(" world");
            Assert.False(_fixture.View.IsErrorState());
            Assert.Equal("a hello world>", _fixture.View.LastConsoleOutput);

            _fixture.Model.Refresh();
            Assert.False(_fixture.View.IsErrorState());
            Assert.Equal("a hello world>", _fixture.View.LastConsoleOutput);

            _fixture.Model.BodyInsertText('s'.ToString());
            Assert.False(_fixture.View.IsErrorState());
            Assert.Equal("a hello worlds>", _fixture.View.LastConsoleOutput);

            _fixture.Model.Refresh();
            Assert.False(_fixture.View.IsErrorState());
            Assert.Equal("a hello worlds>", _fixture.View.LastConsoleOutput);
            Assert.Equal(1, _fixture.Model.ChapterBody.GetLineCount());

        }
    }
}
