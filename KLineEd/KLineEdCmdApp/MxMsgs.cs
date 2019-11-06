using System.Diagnostics.CodeAnalysis;

namespace KLineEdCmdApp
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public static class MxMsgs
    {
        public const string SupportedCultures = "en;";      //must end with ;

        //Messages
        public const string MxMsgNotFound = "MxMsgNotFound"; //"Message not found. Coding defect. Please report this problem" 

        //Errors 
        public const string MxErrTest = "MxErrTest"; //"Error test. Test of error handling. Please take no action"
        public const string MxErrInvalidSettingsFile = "MxErrInvalidSettingsFile"; //"Your settings file is invalid. The file used for saving your program parameters and arguments is corrupt. Delete KLineEdApp.json and try again"
        public const string MxErrUnknownParam = "MxErrUnsupportedParam"; //"Parameter not supported. This is a coding defect which has been reported. Please try again with another parameter or install a later version of the application"
        public const string MxErrInvalidParamArg = "MxErrInvalidParamArg"; //"Parameter or argument is not correctly set. This is a coding defect which has been reported. Please try again with another parameter or install a later version of the application"
        public const string MxErrParamArgValueNotFound = "MxErrParamArgValueNotFound"; //"Value of argument not found. Command line parameters are incorrect. Please try again with correct parameters and arguments"
        public const string MxErrParamArgNameNotFound = "MxErrParamArgNameNotFound"; //"Argument name not found. Command line parameters are incorrect. Please try again with correct parameters and arguments"
        public const string MxErrParamArgNameDuplicateFound = "MxErrParamArgNameDuplicateFound"; //"Duplicate argument name found. Command line parameters are incorrect. Please try again with correct parameters and arguments"

        public const string MxErrBadMethodParam = "MxErrBadMethodParam";  //Internal parameter is invalid. This is a coding defect which has been reported. Please use alternative functionality until fixed.
        public const string MxErrInvalidCondition = "MxErrInvalidCondition"; //Invalid condition. This is a coding defect which has been reported. Please use alternative functionality until fixed.
        public const string MxErrLineTooLong = "MxErrLineTooLong"; //Line is too long. You have reached the end of the line. Press the Enter key to continue
        public const string MxErrInvalidChapterFile = "MxErrInvalidChapterFile"; //Chapter file cannot be opened. It has been corrupted. Run using --fix parameter and then try again 
        public const string MxErrException = "MxErrException"; //Unexpected exception. This is a coding defect which has been reported. Please use alternative functionality until fixed.
        public const string MxErrSystemFailure = "MxErrSystemFailure"; //Unexpected system problem. There may be something wrong with your system. Please reboot your computer and try again.
        public const string MxErrBrowserFailed = "MxErrBrowserFailed"; //Cannot start browser. Your web browser may be defective. Please reboot your computer and try again.
        public const string MxErrNoWordSelected = "MxErrNoWordSelected"; //No word found at current cursor position. Move cursor and try again"
        public const string MxErrInvalidWordSelected = "MxErrInvalidWordSelected"; //Word found at current cursor position is invalid. Move cursor and try again"
        public const string MxErrExportInputFileNotFound = "MxErrExportInputFileNotFound"; //Input file does not exist. Change the '--export' parameter argument 'from' and try again
        public const string MxErrExportOutputFileExists = "MxErrExportOutputFileExists"; //Output file already exists. Change the '--export' parameter argument 'to' and try again
        public const string MxErrExportOutputFileEmpty = "MxErrExportOutputFileEmpty"; //Input file is empty. Select another file and try again
        public const string MxErrImportInputFileNotFound = "MxErrImportInputFileNotFound"; //Input file does not exist. Change the '--import' parameter argument 'from' and try again
        public const string MxErrImportOutputFileExists = "MxErrImportOutputFileExists"; //Output file already exists. Change the '--import' parameter argument 'to' and try again
        public const string MxErrImportInputFileEmpty = "MxErrImportInputFileEmpty"; //Input file is empty. Select another file and try again

        //Warnings - must start with text 'Warning: '

        public const string MxWarnEndOfLine = "MxErrEndOfLine";     //Warning: you are at the end of the line. Press the left arrow key and try again.
        public const string MxWarnStartOfLine = "MxErrStartOfLine"; //Warning: you are at the start of the line. Press the right arrow key or enter a character.
        public const string MxWarnNoCharToDelete = "MxWarnNoCharToDelete"; //Warning: no character to delete.
        public const string MxWarnBackspaceAtStartOfLine = "MxWarnBackspaceAtStartOfLine"; //Warning: you cannot backspace at the start of a line.
        public const string MxWarnInvalidChar = "MxWarnInvalidChar"; //Warning: invalid character. Please press another key.
        public const string MxWarnStartOfChapter = "MxWarnStartOfChapter"; //Warning: you cannot move beyond the start of the chapter
        public const string MxWarnEndOfChapter = "MxWarnEndOfChapter"; //Warning: you cannot move beyond the end of the chapter
        public const string MxWarnTooManyLines = "MxWarnTooManyLines"; //Warning: Line limit reached. Please start a new chapter
        public const string MxWarnChapterEmpty = "MxWarnChapterEmpty"; //Warning: Chapter is empty. Please type a character and try again.
        public const string MxWarnScrollLimit = "MxWarnScrollLimit";   //Warning: you cannot scroll back any further. Change --limits parameter and try again.

        //Info - must start with text 'Info: '

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
        public const int KLineEdCmdAppProgramFirst =    1010101; //first error code in class Program.cs
        public const int CmdLineParamsAppFirst =        1020101; //first error code in class CmdLineParamsApp.cs
        public const int KLineEditorFirst =             1030101; //first error code in class KLineEditor.cs
       // public const int EditFirst = 1040101; //first error code in class Edit.cs
        public const int ChapterModelFirst =            1050101; //first error code in class ChapterModel.cs
        public const int ModelHeaderChapterFirst =      1060101; //first error code in class HeaderChapter.cs
        public const int ModelHeaderSessionFirst =      1070101; //first error code in class HeaderSession.cs
        public const int ModelHeaderSessionPauseFirst = 1080101; //first error code in class HeaderSessionPause.cs
        public const int ModelHeaderFirst =             1090101; //first error code in class Header.cs
        public const int ModelBodyFirst =               1100101; //first error code in class Body.cs
        public const int BaseViewFirst =                1110101; //first error code in class BaseView.cs
        public const int CmdsHelpViewFirst =            1120101; //first error code in class CmdsHelpView.cs
        public const int MsgLineViewFirst =             1130101; //first error code in class MsgLineView.cs
        public const int TextEditViewFirst =            1140101; //first error code in class TextEditView.cs
        public const int PropsEditViewFirst =           1150101; //first error code in class PropsEditViewView.cs
        public const int SpellEditViewFirst =           1160101; //first error code in class SpellEditViewView.cs
        public const int EditAreaViewFirst =            1170101; //first error code in class EditAreaViewView.cs


        public const int StatusLineViewFirst =          1200101; //first error code in class StatusLineView.cs
        public const int MxConsoleFirst =               1210101; //first error code in class MxConsole.cs
        public const int ControllerFactoryFirst =       1220101; //first error code in class ControllerFactory.cs
        public const int PropsEditingControllerFirst =  1230101; //first error code in class PropsEditingController.cs
        public const int TextEditingControllerFirst =   1240101; //first error code in class TextEditingController.cs
        public const int BaseEditingControllerFirst =   1250101; //first error code in class BaseEditingController.cs

        public const int ExportProcFirst =              1260101; //first error code in class ExportProc.cs
        public const int ImportProcFirst =              1270101; //first error code in class ImportProc.cs
    }
}
