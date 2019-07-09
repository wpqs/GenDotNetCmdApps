using System.Runtime.Serialization;
using KLineEdCmdApp.Model;
// ReSharper disable All

namespace KLineEdCmdAppTest.TestSupport
{
    public class MockNotifierModel : NotifierModel
    {
        public enum ChangeHint
        {
            [EnumMember(Value = "Char")] Char = 0,
            [EnumMember(Value = "Word")] Word = 1,
            [EnumMember(Value = "Status")]Status = 2,
            [EnumMember(Value = "Cmd")] Cmd = 3,
            [EnumMember(Value = "All")] All = 4,
            [EnumMember(Value = "Unknown")] Unknown = NotificationItem.ChangeUnknown
        }
        public string Msg { private set; get; }

        public MockNotifierModel() : base()
        {
            Msg = "Hello world";
        }
        public void Close()
        {
            DisconnectAllViews();
        }
        public void SetMsg(string msg)
        {
            Msg = msg;
            UpdateAllViews((int)ChangeHint.Word);
        }
        public void SetError(string msg)
        {
            NotifyErrorAllViews(msg);
        }
    }
}
