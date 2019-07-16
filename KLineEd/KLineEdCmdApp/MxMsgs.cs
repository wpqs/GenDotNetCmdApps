using System.Diagnostics.CodeAnalysis;

namespace KLineEdCmdApp
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public static class MxMsgs
    {
        public const string SupportedCultures = "en;";      //must end with ;

        //Messages
        public const string MxMsgNotFound = "MxMsgNotFound"; //"Message not found. Coding defect. Please report this problem" 

        //Warnings - must start with text 'Warning'

        //Errors 
        public const string MxErrTest = "MxErrTest"; //"Error test. Test of error handling. Please take no action"
        public const string MxErrInvalidSettingsFile = "MxErrInvalidSettingsFile"; //"Error: your settings file is invalid. The file used for saving your program parameters and arguments is corrupt. Delete KLineEdApp.json and try again"
        public const string MxErrUnknownParam = "MxErrUnsupportedParam"; //"Error: parameter not supported. This is a coding defect which has been reported. Please try again with another parameter or install a later version of the application"
        public const string MxErrInvalidParamArg = "MxErrInvalidParamArg"; //"Error: parameter or argument is not correctly set. This is a coding defect which has been reported. Please try again with another parameter or install a later version of the application"
        public const string MxErrBadMethodParam = "MxErrBadMethodParam";  //Error: internal parameter is invalid. This is a coding defect which has been reported. Please use alternative functionality until fixed.
        public const string MxErrInvalidCondition = "MxErrInvalidCondition"; //Error: invalid condition. This is a coding defect which has been reported. Please use alternative functionality until fixed.
        public const string MxErrLineTooLong = "MxErrLineTooLong"; //Error: line is too long. You have reached the end of the line. Press the Enter key to continue
    }

    //Whilst you should try to avoid duplicating error code values in your code base, doing so doesn't break anything. The following conventions may help:
    //  each assembly starts with a new million value; ReturnCodeApp=1yyxxww, ReturnCodeTest=1yyxxww, Last=9yyxxww (>10 assemblies then add an extra digit to error code, but this shouldn't happen too often)
    //  each class starts with a new ten thousand value; Program=z01xxww, Detail=z02xxww,  Last=z99xxww (>100 classes in assembly then restart with next available assembly number, but this shouldn't happen in well structured code) 
    //  each method starts with a new hundred value; main=zyy01ww, Process=zyy02ww, Last=zyy99ww (>100 methods in class then restart with next available class number, but this shouldn't happen in well structured code)
    //  each error starts with a new unit value; error1=zyyxx01, error2=zyyxx02, Last=zyyxx99 (>100 errors in method then restart with next available method number, but this shouldn't happen in well structured code)


    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public static class ErrorCodeList
    {
        public const int ErrorNoLength = 7;
        public const int KLineEdCmdAppProgramFirst = 1010101; //first error code in class Program.cs
        public const int CmdLineParamsAppFirst = 1020101; //first error code in class CmdLineParamsApp.cs
        public const int KLineEditorFirst = 1030101; //first error code in class KLineEditor.cs
       // public const int EditFirst = 1040101; //first error code in class Edit.cs
        public const int ChapterModelFirst = 1050101; //first error code in class ChapterModel.cs
        public const int ModelHeaderChapterFirst = 1060101; //first error code in class HeaderChapter.cs
        public const int ModelHeaderSessionFirst = 1070101; //first error code in class HeaderSession.cs
        public const int ModelHeaderSessionPauseFirst = 1080101; //first error code in class HeaderSessionPause.cs
        public const int ModelHeaderFirst = 1090101; //first error code in class Header.cs
        public const int ModelBodyFirst = 1100101; //first error code in class Body.cs
        public const int KLineEdBaseViewFirst = 1110101; //first error code in class KLineEdView.cs
        public const int CmdsHelpViewFirst = 1120101; //first error code in class CmdsHelpView.cs
        public const int MsgLineViewFirst = 1130101; //first error code in class MsgLineView.cs
        public const int TextEditViewFirst = 1140101; //first error code in class TextEditView.cs
        public const int PropsEditViewFirst = 1150101; //first error code in class PropsEditViewView.cs
        public const int SpellEditViewFirst = 1160101; //first error code in class SpellEditViewView.cs

        public const int StatusLineViewFirst = 1200101; //first error code in class StatusLineView.cs
        public const int TerminalFirst = 1210101; //first error code in class Terminal.cs
        public const int ControllerFactoryFirst = 1220101; //first error code in class ControllerFactory.cs


    }
}
