using System;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Properties;
using KLineEdCmdApp.View;
using MxReturnCode;

// ReSharper disable once CheckNamespace
namespace KLineEdCmdApp
{
    public class KLineEditor
    {
        public static readonly int PosIntegerNotSet = -1;
        public static readonly string ReportSectionDottedLine = $"{Environment.NewLine}....................................................{Environment.NewLine}";

        public Screen Vdu { private set; get; }
        public  EditFile EditFile { private set; get; }
        public string[] LastLines { private set; get; }

        public bool InitDone { private set; get; }

        public MxReturnCode<bool> Start(Screen screen, string editPathFilename)
        {
            var rc = new MxReturnCode<bool>("Edit.Setup");

            if ((String.IsNullOrEmpty(editPathFilename)) || (screen == null))
                rc.SetError(1040101, MxError.Source.Param, $"{editPathFilename ?? "[null]"} is invalid or screen is null", "MxErrBadMethodParam");
            else
            {
                if ((screen.AppParams?.EditFile != editPathFilename) || (screen.AppParams.DisplayLastLinesCnt < CmdLineParamsApp.ArgDisplayLastLinesCntMin) || (CmdLineParamsApp.ArgDisplayLastLinesCntMin < 0))
                    rc.SetError(1040102, MxError.Source.Program, $"editFile={editPathFilename} does not match screen.AppParams.EditFile={screen.AppParams?.EditFile ?? "[null]"} or DisplayLastLinesCnt={screen.AppParams?.DisplayLastLinesCnt} less than min={CmdLineParamsApp.ArgDisplayLastLinesCntMin} or 0", "MxErrInvalidCondition");
                else
                {
                    Vdu = screen;
                    EditFile = new EditFile();
                    var rcFile = EditFile.Initialise(screen.LineWidth, editPathFilename);
                    rc += rcFile;
                    if (rcFile.IsSuccess(true))
                    {
                        var rcSession = EditFile.CreateNewSession();
                        rc += rcSession;
                        if (rcSession.IsSuccess(true))
                        {
                            InitDone = true;
                            rc.SetResult(true);
                        }
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Finish()
        {
            var rc = new MxReturnCode<bool>("Edit.Finish");

            try
            {
                if (InitDone)
                {
                    EditFile.Close();
                    InitDone = false;
                    rc.SetResult(true);
                }
            }
            catch (Exception e)
            {
                rc.SetError(1040201, MxError.Source.Exception, e.Message);
            }
            return rc;
        }

        public MxReturnCode<bool> Process()
        {
            var rc = new MxReturnCode<bool>("Edit.Process");

            try
            {
                if (InitDone)
                {
                    //setup VDU commands, footer
                    //display previous lines
                    //while()
                    //  process input from user and add to line buffer or process cmds
                    //  at end of line append to file
                    //  setup VDU commands, footer
                    //  display previous lines


                    rc.SetResult(true);
                }
            }
            catch (Exception e)
            {
                rc.SetError(1040301, MxError.Source.Exception, e.Message);
            }
            return rc;
        }

        public string GetReport()
        {
            var rc = string.Format(Resources.WelcomeNotice, Program.CmdAppName, Program.CmdAppVersion, Program.CmdAppCopyright, Environment.NewLine);
            rc += Environment.NewLine;
            rc += "Run report:";
            rc += Environment.NewLine;
            rc += Environment.NewLine;
            rc += EditFile?.GetReport() ??HeaderBase.ValueNotSet;
            return rc;
        }

        private string GetCommandHints()
        {
            var rc = "[not initialised]";
            if (InitDone)
            {
                rc = "Esc=refresh | Ctrl+X=exit | <- | -> | Del | BS";
            }
            return rc;
        }

        private string GetFooter()
        {
            var rc = "[not initialised]";
            if (InitDone)
            {
                //start time, current edit duration, WPM, number of words written, number of pages written, total number of words in file, total number of pages in file
                rc = $" | {EditFile?.Folder ?? "[null]"}";
            }
            return rc;
        }
    }
}
