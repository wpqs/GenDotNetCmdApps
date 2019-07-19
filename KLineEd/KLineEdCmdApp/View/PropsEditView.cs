using System.Diagnostics.CodeAnalysis;
using MxReturnCode;

using KLineEdCmdApp.Model;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.View.Base;

namespace KLineEdCmdApp.View
{
    [SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    public class PropsEditView : EditAreaView
    {
        public static readonly string PropsEditorMode = "Properties Editing:";
        public static readonly int AuthorLineNo = 1;
        public static readonly int ProjectLineNo = 3;
        public static readonly int TitleLineNo = 5;
        public static readonly int FilenameLineLineNo = 8;

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
                        var authorLine = $"1.{model.GetTabSpaces()}{HeaderProps.AuthorLabel} {model.ChapterHeader?.Properties.Author ?? Program.ValueNotSet}";
                        var projectLine = $"2.{model.GetTabSpaces()}{HeaderProps.ProjectLabel} {model.ChapterHeader?.Properties.Project ?? Program.ValueNotSet}";
                        var titleLine = $"3.{model.GetTabSpaces()}{HeaderProps.TitleLabel} {model.ChapterHeader?.Properties.Title ?? Program.ValueNotSet}";
                        var filenameLine = $"{HeaderProps.PathFileNameLabel} {model.ChapterHeader?.Properties.PathFileName ?? Program.ValueNotSet}";

                        ChapterModel.ChangeHint change = (ChapterModel.ChangeHint) notificationItem.Change;
                        switch (change)
                        {
                            case ChapterModel.ChangeHint.Props:
                            case ChapterModel.ChangeHint.All:
                            {
                                rc += ClearEditAreaText();
                                if (rc.IsSuccess(true))
                                    rc += DisplayEditAreaLine(AuthorLineNo, authorLine, false);
                                if (rc.IsSuccess(true))
                                    rc += DisplayEditAreaLine(ProjectLineNo, projectLine, false);
                                if (rc.IsSuccess(true))
                                    rc += DisplayEditAreaLine(TitleLineNo, titleLine, false);
                                if (rc.IsSuccess(true))
                                    rc += DisplayEditAreaLine(FilenameLineLineNo, filenameLine, false);
                                if (rc.IsSuccess(true))
                                    rc += SetEditAreaCursor(AuthorLineNo, authorLine.Length); //Model.PropsEdit.Row, Model.PropsEdit.Col
                                if (rc.IsSuccess(true))
                                    rc.SetResult(true);
                                break;
                            }
                            case ChapterModel.ChangeHint.StatusLine: //reset the cursor after update to EditHelpView, MsgLineView, StatusLineView
                            case ChapterModel.ChangeHint.MsgLine:
                            case ChapterModel.ChangeHint.HelpLine:
                            { //get from Model ActivePropLine and ActivePropColumn and set line accordingly
                                rc += SetEditAreaCursor(AuthorLineNo, authorLine.Length); //Model.PropsEdit.Row, Model.PropsEdit.Col
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
            if (rc.IsError(true))
                DisplayErrorMsg(rc);
        }
    }
}
