using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using KLineEdCmdApp.Model;
using MxReturnCode;

namespace KLineEdCmdApp.Utils
{
    public class ImportProc
    {
        public static MxReturnCode<string> CreateKsxFile(string importInputFile, string importOutputFile, int displayCols)
        {
            var rc = new MxReturnCode<string>("ImportProc.CreateKsxFile", Program.ValueNotSet);

            if (string.IsNullOrEmpty(importInputFile) || string.IsNullOrEmpty(importOutputFile) || (displayCols < CmdLineParamsApp.ArgTextEditorDisplayColsMin) )
                rc.SetError(1270101, MxError.Source.Param, $"{importInputFile ?? "importInputFile is null"} or {importOutputFile ?? "importOutputFile is null"} or {displayCols} < {CmdLineParamsApp.ArgTextEditorDisplayColsMin}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (File.Exists(importInputFile) == false)
                    rc.SetError(1270102, MxError.Source.User, $"{importInputFile} doesn't exist", MxMsgs.MxErrImportInputFileNotFound);
                else
                {
                    if (File.Exists(importOutputFile))
                        rc.SetError(1270103, MxError.Source.User, $"{importOutputFile} already exists", MxMsgs.MxErrImportOutputFileExists);
                    else
                    {
                        try
                        {
                            var textLines = new List<string>();
                            int rowIndex = 0;
                            using (var file = new StreamReader(importInputFile)) //default StreamBuffer size is 1024
                            {
                                var text = file.ReadToEnd();
                                var lines = text.Split(Environment.NewLine );
                                foreach (var line in lines)
                                {
                                    if (string.IsNullOrEmpty(line))
                                        textLines.Add(Body.ParaBreak);
                                    else
                                    { 
                                        var lineEnd = line;
                                        var splitIndex = 0;
                                        while ((splitIndex != Program.PosIntegerNotSet) && (lineEnd.Length > 0))
                                        {
                                            splitIndex = Body.GetSplitIndexFromEnd2(lineEnd, displayCols);
                                            if ((splitIndex <= 0) || (splitIndex >= lineEnd.Length))
                                                textLines.Add(lineEnd+Body.ParaBreak);
                                            else
                                            {
                                                textLines.Add(lineEnd.Snip(0, splitIndex - 1) ?? Program.ValueNotSet);
                                                lineEnd = (lineEnd[splitIndex] == ' ') ? lineEnd.Substring(splitIndex+1) : lineEnd.Substring(splitIndex);
                                            }
                                        }
                                    }
                                    rowIndex++;
                                }
                            }
                            if (rowIndex == 0)
                                rc.SetError(1270104, MxError.Source.User, $"{importInputFile} is empty", MxMsgs.MxErrImportInputFileEmpty);
                            else
                            {
                                var manuscript = new ChapterModel();
                                var rcInit = manuscript.Initialise(CmdLineParamsApp.ArgTextEditorDisplayRowsDefault, displayCols, importOutputFile);
                                rc += rcInit;
                                if (rcInit.IsSuccess(true))
                                {
                                    var row = manuscript.ChapterBody.GetLineCount();
                                    foreach (var line in textLines)
                                    {
                                        var rcInsert = manuscript.ChapterBody.InsertLine(row, line);
                                        if (rcInsert.IsError(true))
                                        {
                                            rc += rcInsert;
                                            break;
                                        }
                                        row++;
                                    }
                                    if (rc.IsSuccess())
                                    {
                                        var rcSave = manuscript.Save();
                                        rc += rcSave;
                                        if (rcSave.IsSuccess(true))
                                        {
                                            rc.SetResult($"Succeeded: {rowIndex + 1} lines have been imported from {importInputFile} to {importOutputFile}");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            rc.SetError(1270105, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                        }
                    }
                }
            }
            return rc;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static int GetSplitIndex(string line, int maxColIndex)
        {
            var rc = Program.PosIntegerNotSet;
            if ((string.IsNullOrEmpty(line) == false) && (maxColIndex >= CmdLineParamsApp.ArgTextEditorDisplayColsMin))
            {
                var splitIndex = line.Length - 1;
                if (splitIndex >= maxColIndex)
                {
                    while (splitIndex > maxColIndex)
                        splitIndex--;
                    while ((splitIndex > 0) && (line[splitIndex] != ' '))
                        splitIndex--;
                    rc = (splitIndex > 0) ? splitIndex : maxColIndex;
                }
            }
            return rc;
        }
    }
}
