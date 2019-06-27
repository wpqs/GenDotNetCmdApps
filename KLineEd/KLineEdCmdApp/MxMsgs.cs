namespace KLineEdCmdApp
{
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


    }

    //Whilst you should try to avoid duplicating error code values in your code base, doing so doesn't break anything. The following conventions may help:
    //  each assembly starts with a new million value; ReturnCodeApp=1yyxxww, ReturnCodeTest=1yyxxww, Last=9yyxxww (>10 assemblies then add an extra digit to error code, but this shouldn't happen too often)
    //  each class starts with a new ten thousand value; Program=z01xxww, Detail=z02xxww,  Last=z99xxww (>100 classes in assembly then restart with next available assembly number, but this shouldn't happen in well structured code) 
    //  each method starts with a new hundred value; main=zyy01ww, Process=zyy02ww, Last=zyy99ww (>100 methods in class then restart with next available class number, but this shouldn't happen in well structured code)
    //  each error starts with a new unit value; error1=zyyxx01, error2=zyyxx02, Last=zyyxx99 (>100 errors in method then restart with next available method number, but this shouldn't happen in well structured code)


    public static class ErrorCodeList
    {
        public const int KLineEdCmdAppProgramFirst = 1010101; //first error code in class Program.cs
    }
}
