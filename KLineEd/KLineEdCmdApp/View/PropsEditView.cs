using System.Diagnostics.CodeAnalysis;
using MxReturnCode;

using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public class PropsEditView : EditAreaView
    {
        public static readonly string PropsEditorMode = "Properties Editing:";

        public static readonly string AuthorLabel =       $"Author:  ";     //labels for view - {HeaderProps.AuthorLabel}
        public static readonly string ProjectLabel =      $"Project: ";     //labels for view - {HeaderProps.ProjectLabel
        public static readonly string TitleLabel =        $"Title:   ";     //labels for view - {HeaderProps.TitleLabel}
        public static readonly string PathFileNameLabel = $"File:    ";      //labels for view - {HeaderProps.PathFileNameLabel}

        public static readonly int LongestLabelLength = ProjectLabel.Length;
        public static readonly int AuthorLineNo = 1;
        public static readonly int ProjectLineNo = 3;
        public static readonly int TitleLineNo = 5;
        public static readonly int FilenameLineLineNo = 8;

        public enum PropsEditViewCursorRow
        {
            Author = 1,
            Project = 3,
            Title = 5,
            PathFileName = 8
        }

        public PropsEditView(ITerminal terminal) : base(terminal) { }

        public override void OnUpdate(NotificationItem notificationItem)
        {
            var rc = new MxReturnCode<bool>("PropsEditView.OnUpdate");

            base.OnUpdate(notificationItem);
            if (IsError())
                rc.SetError(GetErrorNo(), GetErrorSource(), GetErrorTechMsg(), GetErrorUserMsg());
            else
            {
                ChapterModel model = notificationItem.Data as ChapterModel;
                if (model == null)
                    rc.SetError(1150101, MxError.Source.Param, $"model is null", MxMsgs.MxErrBadMethodParam);
                else
                {
                    if ((model.EditorHelpLine?.StartsWith(PropsEditView.PropsEditorMode) ?? false) == false)
                        rc.SetResult(true);
                    else
                    {
                        var authorLine = $"{AuthorLabel} {model.ChapterHeader?.Properties.Author ?? Program.ValueNotSet}";
                        var projectLine = $"{ProjectLabel} {model.ChapterHeader?.Properties.Project ?? Program.ValueNotSet}";
                        var titleLine = $"{TitleLabel} {model.ChapterHeader?.Properties.Title ?? Program.ValueNotSet}";
                        var filenameLine = $"{PathFileNameLabel} {model.ChapterHeader?.Properties.PathFileName ?? Program.ValueNotSet}";

                        var currentCursorRow = XlatHeaderPropsRow((HeaderProps.CursorRow)(model.ChapterHeader?.Properties?.Cursor?.RowIndex ?? 0));
                        var currentCursorCol = GetLabelLength(currentCursorRow) + 1 + model.ChapterHeader?.Properties?.Cursor?.ColIndex ?? 0;

                        ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;

                        switch (change)
                        {
                            case ChapterModel.ChangeHint.Props:
                            {
                                    if (currentCursorRow == PropsEditViewCursorRow.Author)
                                        rc += DisplayEditAreaLine((int)PropsEditViewCursorRow.Author, authorLine, true);
                                    else if (currentCursorRow == PropsEditViewCursorRow.Project)
                                        rc += DisplayEditAreaLine((int)PropsEditViewCursorRow.Project, projectLine, true);
                                    else if (currentCursorRow == PropsEditViewCursorRow.Title)
                                        rc += DisplayEditAreaLine((int)PropsEditViewCursorRow.Title, titleLine, true);
                                    else
                                        rc.SetError(1150102, MxError.Source.Program, $"currentCursorRow={currentCursorRow} not supported", MxMsgs.MxErrInvalidCondition);

                                    if (rc.IsSuccess(true))
                                        rc += SetEditAreaCursor((int)currentCursorRow, currentCursorCol);
                                    if (rc.IsSuccess(true))
                                        rc.SetResult(true);
                                    break;
                            }
                            case ChapterModel.ChangeHint.All:
                            {
                                rc += ClearEditAreaText();
                                if (rc.IsSuccess(true))
                                    rc += DisplayEditAreaLine((int)PropsEditViewCursorRow.Author, authorLine, false);
                                if (rc.IsSuccess(true))
                                    rc += DisplayEditAreaLine((int)PropsEditViewCursorRow.Project, projectLine, false);
                                if (rc.IsSuccess(true))
                                    rc += DisplayEditAreaLine((int)PropsEditViewCursorRow.Title, titleLine, false);
                                if (rc.IsSuccess(true))
                                    rc += DisplayEditAreaLine((int)PropsEditViewCursorRow.PathFileName, filenameLine, false);
                                if (rc.IsSuccess(true))
                                    rc += SetEditAreaCursor((int)currentCursorRow, currentCursorCol); 
                                if (rc.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            case ChapterModel.ChangeHint.StatusLine: //reset the cursor after update to EditHelpView, MsgLineView, StatusLineView
                            case ChapterModel.ChangeHint.MsgLine:
                            case ChapterModel.ChangeHint.HelpLine:
                            { //get from Model ActivePropLine and ActivePropColumn and set line accordingly
                                rc += SetEditAreaCursor((int)currentCursorRow, currentCursorCol); 
                                if (rc.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            // ReSharper disable once RedundantEmptySwitchSection
                            default:
                            {
                                rc.SetError(1150101, MxError.Source.Program, $"hint={MxDotNetUtilsLib.EnumOps.XlatToString(change)} not handled", MxMsgs.MxErrInvalidCondition);
                                break;
                            }
                        }
                    }
                }
            }
            OnUpdateDone(rc, true);
        }

        private int GetLabelLength(PropsEditViewCursorRow row)
        {
            var rc = Program.PosIntegerNotSet; 

            if (row == PropsEditViewCursorRow.Author)
                rc = AuthorLabel.Length;
            else if (row == PropsEditViewCursorRow.Project)
                rc = ProjectLabel.Length;
            else if (row == PropsEditViewCursorRow.Title)
                rc = TitleLabel.Length;
            else if (row == PropsEditViewCursorRow.PathFileName)
                rc = PathFileNameLabel.Length;
            else
            {
                rc = 0;
            }
            return rc;
        }

        private PropsEditViewCursorRow XlatHeaderPropsRow(HeaderProps.CursorRow row)
        {
            PropsEditViewCursorRow rc = PropsEditViewCursorRow.Author;

            if (row == HeaderProps.CursorRow.Author)
                rc = PropsEditViewCursorRow.Author;
            else if (row == HeaderProps.CursorRow.Project)
                rc = PropsEditViewCursorRow.Project;
            else if (row == HeaderProps.CursorRow.Title)
                rc = PropsEditViewCursorRow.Title;
            else if (row == HeaderProps.CursorRow.PathFileName)
                rc = PropsEditViewCursorRow.PathFileName;
            else
            {
                rc = PropsEditViewCursorRow.Author;
            }
            return rc;
        }
    }
}
