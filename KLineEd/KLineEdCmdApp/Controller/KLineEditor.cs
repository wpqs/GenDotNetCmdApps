using System;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Properties;
using MxReturnCode;

// ReSharper disable once CheckNamespace
namespace KLineEdCmdApp
{
    public class KLineEditor
    {
        public const int PosIntegerNotSet = -1; //used for default values of params so cannot be readonly
        public static readonly string ReportSectionDottedLine = $"{Environment.NewLine}....................................................{Environment.NewLine}";

        public  ChapterModel EditFile { private set; get; }

        public bool Ready { private set; get; }

        public KLineEditor()
        {
            Ready = false;
            EditFile = null;
        }
        
        public MxReturnCode<bool> Start(ChapterModel model)
        {
            var rc = new MxReturnCode<bool>("Edit.Setup");

            if ((model == null) || (model.Ready == false)  )
                rc.SetError(1040101, MxError.Source.Param, $"model is {((model == null) ? "[null]" : "[not ready]")}", "MxErrBadMethodParam");
            else
            {
                EditFile = model;

                var rcSession = model.CreateNewSession();
                rc += rcSession;
                if (rcSession.IsSuccess(true))
                {
                    Ready = true;
                    rc.SetResult(true);
                }
            }
            return rc;
        }

        public MxReturnCode<bool> Finish()
        {
            var rc = new MxReturnCode<bool>("Edit.Finish");

            try
            {
                if (Ready)
                {
                    EditFile.Close();
                    Ready = false;
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
                if (Ready)
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
            rc += $"Report for editing session {EditFile?.Header?.GetLastSession()?.SessionNo ?? KLineEditor.PosIntegerNotSet} of chapter {EditFile?.Header?.Chapter?.Title ?? "[null]"}:";
            rc += Environment.NewLine;
            rc += Environment.NewLine;
            rc += EditFile?.GetReport() ??HeaderBase.ValueNotSet;
            return rc;
        }

        private string GetCommandHints()
        {
            var rc = "[not initialized]";
            if (Ready)
            {
                rc = "Esc=refresh | Ctrl+X=exit | <- | -> | Del | BS";
            }
            return rc;
        }

        private string GetFooter()
        {
            var rc = "[not initialized]";
            if (Ready)
            {
                //start time, current edit duration, WPM, number of words written, number of pages written, total number of words in file, total number of pages in file
                rc = $" | {EditFile?.Folder ?? "[null]"}";
            }
            return rc;
        }
    }
}
