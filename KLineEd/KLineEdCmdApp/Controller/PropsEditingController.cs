using System;
using System.Diagnostics.CodeAnalysis;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Properties;
using KLineEdCmdApp.View;
using MxReturnCode;

namespace KLineEdCmdApp.Controller
{
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class PropsEditingController : BaseEditingController
    {
        public static readonly string EditorHelpText = $"{PropsEditView.PropsEditorMode} Ctrl+Q=Quit Ctrl+S=Save Esc=Refresh F1=Help F12=Text editing";

        public override BaseEditingController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            var rc = new MxReturnCode<bool>("PropsEditingController. ProcessKey");

            BaseEditingController controller = this;
            if ((base.ProcessKey(model, keyInfo) == null)) // && (IsErrorState() == false))
                rc.SetResult(true);     //key handled by base class - no further action needed
            else
            {
                //do stuff related to TextEditing, updating the model as needed
                var  props = model?.ChapterHeader?.Properties;
                if (props == null)
                    rc.SetError(1230101, MxError.Source.Program, "model?.ChapterHeader?.Properties is null", MxMsgs.MxErrInvalidCondition);
                else
                {
                    if (keyInfo.Key == ConsoleKey.F12)
                    {
                        controller = ControllerFactory.Make(Chapter, ControllerFactory.TextEditingController, BrowserCmd, HelpUrl, SearchUrl, ThesaurusUrl, SpellUrl);
                        rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        var row = props.GetRowIndex(ChapterModel.CursorState.Next);
                        if (model.PropsSetCursor(row, 0) == false)
                            rc.SetError(1230102, MxError.Source.Program, $"SetCursor({row}, {0}) failed", MxMsgs.MxErrInvalidCondition);
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        var row = props.GetRowIndex(ChapterModel.CursorState.Previous);
                        if (model.PropsSetCursor(row, 0) == false)
                            rc.SetError(1230103, MxError.Source.Program, $"SetCursor({row}, {0}) failed", MxMsgs.MxErrInvalidCondition);
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        var row = props.GetRowIndex();
                        var col = props.Cursor.ColIndex;
                        if (model.PropsSetCursor(row, --col) == false)
                            rc.SetError(1230104, MxError.Source.User, Resources.MxWarnStartOfLine, MxMsgs.MxWarnStartOfLine); //todo update when next release of MxReturnCode is available
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.RightArrow)
                    {
                        var row = props.GetRowIndex();
                        var col = props.Cursor.ColIndex;
                        if (model.PropsSetCursor(row, ++col) == false)
                            rc.SetError(1230105, MxError.Source.User, Resources.MxWarnEndOfLine, MxMsgs.MxWarnEndOfLine); //todo update when next release of MxReturnCode is available
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Delete)
                    {
                        if (model.PropsDelChar(false) == false)
                            rc.SetError(1230106, MxError.Source.User, Resources.MxWarnChapterEmpty, MxMsgs.MxWarnNoCharToDelete); //todo update when next release of MxReturnCode is available
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (model.PropsDelChar(true) == false)
                            rc.SetError(1230107, MxError.Source.User, Resources.MxWarnBackspaceAtStartOfLine, MxMsgs.MxWarnBackspaceAtStartOfLine); //todo update when next release of MxReturnCode is available
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Tab)
                    {
                        var insert = IsInsertMode() ? true : props.IsCursorBeyondEndOfLine(props.GetRowIndex());
                        if (model.PropsSetText(model.ChapterBody?.TabSpaces ?? "   ", insert) == false)
                            rc.SetError(1230108, MxError.Source.User, Resources.MxWarnBackspaceAtStartOfLine, MxMsgs.MxWarnBackspaceAtStartOfLine); //todo update when next release of MxReturnCode is available
                        else
                            rc.SetResult(true);
                    }
                    else
                    {
                        var insert = IsInsertMode() ? true : props.IsCursorBeyondEndOfLine(props.GetRowIndex());
                        if (model.PropsSetChar(keyInfo.KeyChar, insert ) == false)
                            rc.SetError(1230109, MxError.Source.User, Resources.MxWarnInvalidChar, MxMsgs.MxWarnInvalidChar); //todo update when next release of MxReturnCode is available
                        else
                            rc.SetResult(true);
                    }
                }
            }

            if (rc.IsError(true))
                SetErrorState(rc);

            return controller;
        }

        public override string GetEditorHelpLine()
        {
            return EditorHelpText;
        }
    }
}
