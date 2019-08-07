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
                        var rcMove = model.SetBodyMoveCursor(Body.CursorMove.PreviousRow);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        var rcMove = model.SetBodyMoveCursor(Body.CursorMove.NextRow);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        var rcMove = model.SetBodyMoveCursor(Body.CursorMove.PreviousCol);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.RightArrow)
                    {
                        var rcMove = model.SetBodyMoveCursor(Body.CursorMove.NextCol);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.Home)
                    {
                        var rcMove = model.SetBodyMoveCursor(Body.CursorMove.Home);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.End)
                    {
                        var rcMove = model.SetBodyMoveCursor(Body.CursorMove.End);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.Delete)
                    {
                        var rcDelChar = model.SetBodyDeleteCharacter();
                        if (rcDelChar.IsError(true))
                            SetupMxError(rcDelChar);
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        var rcBack = model.SetBodyBackSpace();
                        if (rcBack.IsError(true))
                            SetupMxError(rcBack);
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        var rcInsertLine = model.SetBodyInsertLine(new string(Environment.NewLine), false);
                        if (rcInsertLine.IsError(true))
                            SetupMxError(rcInsertLine);
                    }
                    else if (keyInfo.Key == ConsoleKey.Tab)
                    {
                        var rcInsertText = model.SetBodyInsertText(model.ChapterBody?.TabSpaces ?? "   ", IsInsertMode());
                        if (rcInsertText.IsError(true))
                            SetupMxError(rcInsertText);
                    }
                    else
                    {
                        var rcInsertText = model.SetBodyInsertText(keyInfo.KeyChar.ToString(), IsInsertMode());
                        if (rcInsertText.IsError(true))
                            SetupMxError(rcInsertText);
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
