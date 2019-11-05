using System;
using KLineEdCmdApp.Controller.Base;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View;
using MxReturnCode;

namespace KLineEdCmdApp.Controller
{
    public class TextEditingController : EditingBaseController
    {
        public static readonly string EditorHelpText = $"{TextEditView.TextEditorMode} Ctrl+Q=Quit Ctrl+S=Save F1=Help F2=Props F3=Spelling F4=Search F5=Thesaurus F6=Spell";
        public override EditingBaseController ProcessKey(ChapterModel model, ConsoleKeyInfo keyInfo)
        {
            var rc = new MxReturnCode<bool>($"TextEditingController.ProcessKey");

            EditingBaseController controller = this;
            if ((base.ProcessKey(model, keyInfo) != null) && (IsErrorState() == false))
            {
                var body = model?.ChapterBody;
                if (body == null)
                   rc.SetError(1240101, MxError.Source.Program, "model?.ChapterBody is null", MxMsgs.MxErrInvalidCondition);
                else
                {
                    //do stuff related to TextEditing, updating the model as needed
                    var ctrlKeyPressed = ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0);
                    if (keyInfo.Key == ConsoleKey.F2)
                        controller = ControllerFactory.Make(Chapter, ControllerFactory.PropsEditingController, BrowserCmd, HelpUrl, SearchUrl, ThesaurusUrl, SpellUrl);
                    else if (keyInfo.Key == ConsoleKey.F3)
                        controller = ControllerFactory.Make(Chapter, ControllerFactory.SpellEditingController, BrowserCmd, HelpUrl, SearchUrl, ThesaurusUrl, SpellUrl);
                    else if (keyInfo.Key == ConsoleKey.F4)
                        LaunchBrowser(body, SearchUrl, true);
                    else if (keyInfo.Key == ConsoleKey.F5)
                        LaunchBrowser(body, ThesaurusUrl, true);
                    else if (keyInfo.Key == ConsoleKey.F6)
                        LaunchBrowser(body, SpellUrl, true);
                    else if ((ctrlKeyPressed == false) && (keyInfo.Key == ConsoleKey.UpArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.PreviousRow);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if (ctrlKeyPressed && (keyInfo.Key == ConsoleKey.UpArrow))
                    {   
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.StartPara);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if ((ctrlKeyPressed == false) && (keyInfo.Key == ConsoleKey.DownArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.NextRow);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if (ctrlKeyPressed && (keyInfo.Key == ConsoleKey.DownArrow))
                    {   
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.EndPara);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if ((ctrlKeyPressed == false) && (keyInfo.Key == ConsoleKey.LeftArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.PreviousCol);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if (ctrlKeyPressed && (keyInfo.Key == ConsoleKey.LeftArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.StartLine);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if ((ctrlKeyPressed == false) && (keyInfo.Key == ConsoleKey.RightArrow))
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.NextCol);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if (ctrlKeyPressed && (keyInfo.Key == ConsoleKey.RightArrow))
                    {  
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.EndLine);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.PageUp)
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.PageUp);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.PageDown)
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.PageDown);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Home)
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.StartChapter);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.End)
                    {
                        var rcMove = model.BodyMoveCursor(Body.CursorMove.EndChapter);
                        if (rcMove.IsError(true))
                            rc += rcMove;
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Delete)
                    {
                        var rcDelChar = model.BodyDeleteCharacter();
                        if (rcDelChar.IsError(true))
                            rc += rcDelChar;
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        var rcBack = model.BodyBackSpace();
                        if (rcBack.IsError(true))
                            rc += rcBack;
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        var rcInsertLine = model.BodyInsertParaBreak(); //new string(Environment.NewLine), false);
                        if (rcInsertLine.IsError(true))
                            rc += rcInsertLine;
                        else
                            rc.SetResult(true);
                    }
                    else if (keyInfo.Key == ConsoleKey.Tab)
                    {
                        var rcInsertText = model.BodyInsertText(model.GetTabSpaces(), IsInsertMode());
                        if (rcInsertText.IsError(true))
                            rc += rcInsertText;
                        else
                            rc.SetResult(true);
                    }
                    else
                    {
                        var rcInsertText = model.BodyInsertText(keyInfo.KeyChar.ToString(), IsInsertMode());
                        if (rcInsertText.IsError(true))
                            rc += rcInsertText;
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
