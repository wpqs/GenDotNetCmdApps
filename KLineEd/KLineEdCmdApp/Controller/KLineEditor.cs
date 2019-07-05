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

        public TextLinesView Vdu { private set; get; }
        public  ChapterModel EditFile { private set; get; }

        public bool Ready { private set; get; }

        public KLineEditor()
        {
            Ready = false;
            Vdu = null;
            EditFile = null;
        }
        
        public MxReturnCode<bool> Start(ChapterModel model, TextLinesView view)
        {
            var rc = new MxReturnCode<bool>("Edit.Setup");

            if ((model == null) || (model.Ready == false) || (view == null) )
                rc.SetError(1040101, MxError.Source.Param, $"screen is null, model is null", "MxErrBadMethodParam");
            else
            {
                Vdu = view;
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
            rc += $"Report for editing session {EditFile?.Header?.GetLastSession()?.SessionNo ?? -1} of chapter {EditFile?.Header?.Chapter?.Title ?? "[null]"}:";
            rc += Environment.NewLine;
            rc += Environment.NewLine;
            rc += EditFile?.GetReport() ??HeaderBase.ValueNotSet;
            return rc;
        }

        private string GetCommandHints()
        {
            var rc = "[not initialised]";
            if (Ready)
            {
                rc = "Esc=refresh | Ctrl+X=exit | <- | -> | Del | BS";
            }
            return rc;
        }

        private string GetFooter()
        {
            var rc = "[not initialised]";
            if (Ready)
            {
                //start time, current edit duration, WPM, number of words written, number of pages written, total number of words in file, total number of pages in file
                rc = $" | {EditFile?.Folder ?? "[null]"}";
            }
            return rc;
        }
    }
}
