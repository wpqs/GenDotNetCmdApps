using KLineEdCmdApp.Model.Base;

namespace KLineEdCmdApp.Utils
{
    public class NotificationItem
    {
        public const int ChangeUnknown = -1;

        public int Change { get; set;  }    //change only makes sense in terms of the model so cannot define enum in NotifierModel and override in its derived class 
        public NotifierModel Data { get; set; }
        public string ErrorMsg { get; set; }
        public bool Unsubscribe { get; set; }

        public NotificationItem()
        {
            ErrorMsg = null;
            Data = null;
            Change = ChangeUnknown;
            Unsubscribe = false;
        }
    }
}
