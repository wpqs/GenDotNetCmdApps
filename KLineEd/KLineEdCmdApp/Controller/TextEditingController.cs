using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.Properties;
using KLineEdCmdApp.View;
using MxReturnCode;

namespace KLineEdCmdApp.Controller
{
    public class TextEditingController : EditingBaseController
    {
        public static readonly string EditorHelpText = $"{TextEditView.TextEditorMode} Ctrl+Q=Quit Esc=Refresh F1=Help F2=Props F3=Spelling";
        public override EditingBaseController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            EditingBaseController controller = this;
            if ((base.ProcessKey(model, keyInfo) != null) && (IsError() == false))
            {
                var body = model?.ChapterBody;
                if (body == null)
                    SetMxError(1240101, MxError.Source.Program, "model?.ChapterBody is null", MxMsgs.MxErrInvalidCondition);
                else
                {
                    //do stuff related to TextEditing, updating the model as needed
                    if (keyInfo.Key == ConsoleKey.F2)
                        controller = ControllerFactory.Make(Chapter, ControllerFactory.PropsEditingController, BrowserExe);
                    else if (keyInfo.Key == ConsoleKey.F3)
                        controller = ControllerFactory.Make(Chapter, ControllerFactory.SpellEditingController, BrowserExe);
                    else if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        if (model.MoveBodyCursor(Body.CursorMove.PreviousRow) == false)
                            SetMxError(1230102, MxError.Source.User, Resources.MxWarnStartOfChapter, MxMsgs.MxWarnStartOfChapter); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        if (model.MoveBodyCursor(Body.CursorMove.NextRow) == false)
                            SetMxError(1230103, MxError.Source.User, Resources.MxWarnEndOfChapter, MxMsgs.MxWarnEndOfChapter); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        if (model.MoveBodyCursor(Body.CursorMove.PreviousCol) == false)
                            SetMxError(1240104, MxError.Source.User, Resources.MxWarnStartOfChapter, MxMsgs.MxWarnStartOfChapter); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.RightArrow)
                    {
                        if (model.MoveBodyCursor(Body.CursorMove.NextCol) == false)
                            SetMxError(1240105, MxError.Source.User, Resources.MxWarnEndOfChapter, MxMsgs.MxWarnEndOfChapter); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.Home)
                    {
                        if (model.MoveBodyCursor(Body.CursorMove.Home) == false)
                            SetMxError(1240106, MxError.Source.User, Resources.MxWarnStartOfChapter, MxMsgs.MxWarnStartOfChapter); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.End)
                    {
                        if (model.MoveBodyCursor(Body.CursorMove.End) == false)
                            SetMxError(1240107, MxError.Source.User, Resources.MxWarnEndOfChapter, MxMsgs.MxWarnEndOfChapter); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.Delete)
                    {
                        if (model.SetBodyDelChar(false) == false)
                            SetMxError(1240108, MxError.Source.User, Resources.MxWarnNoCharToDelete, MxMsgs.MxWarnNoCharToDelete); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        if (model.SetBodyDelChar(true) == false)
                            SetMxError(1240109, MxError.Source.User, Resources.MxWarnStartOfChapter, MxMsgs.MxWarnStartOfChapter); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        if (model.SetBodyInsertLine(new string(Environment.NewLine), false) == false)
                            SetMxError(1240110, MxError.Source.User, Resources.MxWarnTooManyLines, MxMsgs.MxWarnTooManyLines); //todo update when next release of MxReturnCode is available
                    }
                    else if (keyInfo.Key == ConsoleKey.Tab)
                    {
                        var insert = IsInsertMode() ? true : false;
                        if (model.SetBodyText(model.ChapterBody?.TabSpaces ?? "   ", insert) == false)
                            SetMxError(1240111, MxError.Source.User, Resources.MxWarnInvalidChar, MxMsgs.MxWarnInvalidChar); //todo update when next release of MxReturnCode is available
                    }
                    else
                    {
                        var insert = IsInsertMode() ? true : false;
                        if (model.SetBodyChar(keyInfo.KeyChar, insert) == false)
                            SetMxError(1240112, MxError.Source.User, Resources.MxWarnInvalidChar, MxMsgs.MxWarnInvalidChar); //todo update when next release of MxReturnCode is available
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
