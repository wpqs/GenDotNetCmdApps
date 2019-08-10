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
    public class PropsEditingController : EditingBaseController
    {
        public static readonly string EditorHelpText = $"{PropsEditView.PropsEditorMode} Ctrl+Q=Quit Esc=Refresh F1=Help F2=Text editing";

        public override EditingBaseController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            EditingBaseController controller = this;
            if ((base.ProcessKey(model, keyInfo) != null) && (IsError() == false))
            {
                //do stuff related to TextEditing, updating the model as needed
                var  props = model?.ChapterHeader?.Properties;
                if (props == null)
                    SetMxError(1230101, MxError.Source.Program, "model?.ChapterHeader?.Properties is null", MxMsgs.MxErrInvalidCondition);
                else
                {
                    if (keyInfo.Key == ConsoleKey.F2)
                    {
                        controller = ControllerFactory.Make(Chapter, ControllerFactory.TextEditingController, BrowserExe);
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        var row = props.GetRowIndex(ChapterModel.CursorState.Next);
                        if (model.PropsSetCursor(row, 0) == false)
                            SetMxError(1230102, MxError.Source.Program, $"SetCursor({row}, {0}) failed", MxMsgs.MxErrInvalidCondition); 
                    }
                    else if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        var row = props.GetRowIndex(ChapterModel.CursorState.Previous);
                        if (model.PropsSetCursor(row, 0) == false)
                            SetMxError(1230103, MxError.Source.Program, $"SetCursor({row}, {0}) failed", MxMsgs.MxErrInvalidCondition); 
                    }
                    else if (keyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        var row = props.GetRowIndex();
                        var col = props.Cursor.ColIndex;
                        if (model.PropsSetCursor(row, --col) == false)
                            SetMxError(1230104, MxError.Source.User, Resources.MxWarnStartOfLine, MxMsgs.MxWarnStartOfLine); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.RightArrow)
                    {
                        var row = props.GetRowIndex();
                        var col = props.Cursor.ColIndex;
                        if (model.PropsSetCursor(row, ++col) == false)
                            SetMxError(1230105, MxError.Source.User, Resources.MxWarnEndOfLine, MxMsgs.MxWarnEndOfLine); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.Delete)
                    {
                        if (model.PropsDelChar(false) == false)
                            SetMxError(1230106, MxError.Source.User, Resources.MxWarnNoCharToDelete, MxMsgs.MxWarnNoCharToDelete); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (model.PropsDelChar(true) == false)
                            SetMxError(1230107, MxError.Source.User, Resources.MxWarnBackspaceAtStartOfLine, MxMsgs.MxWarnBackspaceAtStartOfLine); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.Tab)
                    {
                        var insert = IsInsertMode() ? true : props.IsCursorBeyondEndOfLine(props.GetRowIndex());
                        if (model.PropsSetText(model.ChapterBody?.TabSpaces ?? "   ", insert) == false)
                            SetMxError(1230108, MxError.Source.User, Resources.MxWarnBackspaceAtStartOfLine, MxMsgs.MxWarnBackspaceAtStartOfLine); //todo update when next release of MxReturnCode is available
                    }
                    else
                    {
                        var insert = IsInsertMode() ? true : props.IsCursorBeyondEndOfLine(props.GetRowIndex());
                        if (model.PropsSetChar(keyInfo.KeyChar, insert ) == false)
                            SetMxError(1230109, MxError.Source.User, Resources.MxWarnInvalidChar, MxMsgs.MxWarnInvalidChar); //todo update when next release of MxReturnCode is available
                    }
                }
            }
            return controller;
        }

        public override string GetEditorHelpLine()
        {
            return EditorHelpText;
        }
    }
}
