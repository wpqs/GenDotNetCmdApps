using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
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
                    var CtrlKeyPressed = ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0);
                    if (keyInfo.Key == ConsoleKey.F2)
                        controller = ControllerFactory.Make(Chapter, ControllerFactory.PropsEditingController, BrowserExe);
                    else if (keyInfo.Key == ConsoleKey.F3)
                        controller = ControllerFactory.Make(Chapter, ControllerFactory.SpellEditingController, BrowserExe);
                    else if ((CtrlKeyPressed == false) && (keyInfo.Key == ConsoleKey.UpArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.PreviousRow);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (CtrlKeyPressed && (keyInfo.Key == ConsoleKey.UpArrow))
                    {   
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.StartPara);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                   }
                    else if ((CtrlKeyPressed == false) && (keyInfo.Key == ConsoleKey.DownArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.NextRow);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (CtrlKeyPressed && (keyInfo.Key == ConsoleKey.DownArrow))
                    {   
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.EndPara);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if ((CtrlKeyPressed == false) && (keyInfo.Key == ConsoleKey.LeftArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.PreviousCol);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (CtrlKeyPressed && (keyInfo.Key == ConsoleKey.LeftArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.StartLine);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if ((CtrlKeyPressed == false) && (keyInfo.Key == ConsoleKey.RightArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.NextCol);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (CtrlKeyPressed && (keyInfo.Key == ConsoleKey.RightArrow))
                    {  
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.EndLine);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.PageUp)
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.PageUp);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.PageDown)
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.PageDown);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.Home)
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.StartChapter);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.End)
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.EndChapter);
                        if (rcMove.IsError(true))
                            SetupMxError(rcMove);
                    }
                    else if (keyInfo.Key == ConsoleKey.Delete)
                    {
                        var rcDelChar = model.BodyDeleteCharacter();
                        if (rcDelChar.IsError(true))
                            SetupMxError(rcDelChar);
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        var rcBack = model.BodyBackSpace();
                        if (rcBack.IsError(true))
                            SetupMxError(rcBack);
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        var rcInsertLine = model.BodyInsertParaBreak(); //new string(Environment.NewLine), false);
                        if (rcInsertLine.IsError(true))
                            SetupMxError(rcInsertLine);
                    }
                    else if (keyInfo.Key == ConsoleKey.Tab)
                    {
                        var rcInsertText = model.BodyInsertText(model.GetTabSpaces(), IsInsertMode());
                        if (rcInsertText.IsError(true))
                            SetupMxError(rcInsertText);
                    }
                    else
                    {
                        var rcInsertText = model.BodyInsertText(keyInfo.KeyChar.ToString(), IsInsertMode());
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
