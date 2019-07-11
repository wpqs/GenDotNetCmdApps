using KLineEdCmdAppTest.TestSupport;
using Xunit;


namespace KLineEdCmdAppTest.ViewTests
{
    public class NotificationTest
    {

        [Fact]
        public void SetMsgTest()
        {
            var model = new MockNotifierModel();
            var view = new MockObserverView();

            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowTitle);
            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowFooter);
            Assert.Equal(0, model.GetSubscriberCount());

            using (model.Subscribe(view))
            {
                Assert.Equal(1, model.GetSubscriberCount());

                model.SetMsg("Hello Will...");          //calls UpdateAllViews();

                Assert.Equal("Hello Will...", view.WindowTitle);
                Assert.Equal("updated from model", view.WindowFooter);
            }

            Assert.Equal(0, model.GetSubscriberCount());
            Assert.Equal("view does not subscribe to model", view.WindowFooter);

            model.SetMsg("Bye Will...");                //calls NotifyChangeAllViews(), but View now disconnected so no update
            Assert.Equal("Hello Will...", view.WindowTitle);
        }

        [Fact] public void UpdateAllViewsTest()
        {
            var model = new MockNotifierModel();
            var view = new MockObserverView();

            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowTitle);
            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowFooter);
            Assert.Equal(0, model.GetSubscriberCount());

            using (model.Subscribe(view))
            {
                Assert.Equal(1, model.GetSubscriberCount());

                model.UpdateAllViews((int)MockNotifierModel.ChangeHint.All);          

                Assert.Equal("Hello world", view.WindowTitle);
                Assert.Equal("updated from model", view.WindowFooter);
            }

            Assert.Equal(0, model.GetSubscriberCount());
            Assert.Equal("view does not subscribe to model", view.WindowFooter);

            model.SetMsg("Bye Will...");                //calls NotifyChangeAllViews(), but View now disconnected so no update
            Assert.Equal("Hello world", view.WindowTitle);
        }

        [Fact]
        public void DisconnectAllViewsTest()
        {
            var model = new MockNotifierModel();
            var view = new MockObserverView();

            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowTitle);
            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowFooter);
            Assert.Equal(0, model.GetSubscriberCount());

            using (model.Subscribe(view))
            {
                Assert.Equal(1, model.GetSubscriberCount());

                model.SetMsg("Hello Will...");      //calls NotifyChangeAllViews();

                Assert.Equal("Hello Will...", view.WindowTitle);
                Assert.Equal("updated from model", view.WindowFooter);

                model.Close();                      //calls DisconnectAllViews();
                Assert.Equal("view does not subscribe to model", view.WindowFooter);
                Assert.Equal(0, model.GetSubscriberCount());
            }
            model.SetMsg("Bye Will...");
            Assert.Equal("Hello Will...", view.WindowTitle);
            Assert.Equal(0, model.GetSubscriberCount());
        }

        [Fact]
        public void NotifyErrorAllViewsTest()
        {
            var model = new MockNotifierModel();
            var view = new MockObserverView();

            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowTitle);
            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowFooter);
            Assert.Equal(0, model.GetSubscriberCount());

            using (model.Subscribe(view))
            {
                Assert.Equal(1, model.GetSubscriberCount());

                model.SetMsg("Hello Will...");      //calls NotifyChangeAllViews();

                Assert.Equal("Hello Will...", view.WindowTitle);
                Assert.Equal("updated from model", view.WindowFooter);

                model.SetError("problem found");
                Assert.Equal("problem found", view.WindowFooter);

                model.SetMsg("Bye Will...");        //calls NotifyChangeAllViews();

                Assert.Equal("Bye Will...", view.WindowTitle);
            }
            Assert.Equal(0, model.GetSubscriberCount());
        }

        [Fact]
        public void DoubleSubscribeTest()
        {
            var model = new MockNotifierModel();
            var view = new MockObserverView();

            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowTitle);
            Assert.Equal(KLineEdCmdApp.Program.ValueNotSet, view.WindowFooter);
            Assert.Equal(0, model.GetSubscriberCount());

            using (model.Subscribe(view))
            {
                Assert.Equal(1, model.GetSubscriberCount());

                using (model.Subscribe(view))
                {
                    Assert.Equal(1, model.GetSubscriberCount());

                    model.SetMsg("Hello Will..."); //calls NotifyChangeAllViews();

                    Assert.Equal("Hello Will...", view.WindowTitle);
                    Assert.Equal("updated from model", view.WindowFooter);
                }
            }
            Assert.Equal(0, model.GetSubscriberCount());
        }
    }
}
