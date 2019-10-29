using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using KLineEdCmdApp.Model;
using MxReturnCode;

namespace KLineEdCmdApp.Utils
{
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public class ExportProc
    {
        public static MxReturnCode<string> CreateTxtFile(string exportInputFile, string exportOutputFile)
        {
            var rc = new MxReturnCode<string>("ExportProc. CreateTxtFile", Program.ValueNotSet);

            if (string.IsNullOrEmpty(exportInputFile) || string.IsNullOrEmpty(exportOutputFile))
                rc.SetError(1260101, MxError.Source.Param, $"{exportInputFile ?? "exportInputFile is null"} or {exportOutputFile ?? "exportOutputFile is null"}", MxMsgs.MxErrBadMethodParam);
            else
            {
                if (File.Exists(exportInputFile) == false)
                    rc.SetError(1260102, MxError.Source.User, $"{exportInputFile} doesn't exist", MxMsgs.MxErrExportInputFileNotFound);
                else
                {
                    if (File.Exists(exportOutputFile))
                        rc.SetError(1260103, MxError.Source.User, $"{exportOutputFile} already exists", MxMsgs.MxErrExportOutputFileExists);
                    else
                    {
                        try
                        {
                            var textLines = new List<string>();
                            bool bodyFound = false;
                            int rowIndex = 0;
                            using (var file = new StreamReader(exportInputFile)) //default StreamBuffer size is 1024
                            {

                                string line = null;
                                while ((line = file.ReadLine()) != null)
                                {
                                    if (line == Body.OpeningElement)
                                        bodyFound = true;
                                    if (line == Body.ClosingElement)
                                        bodyFound = false;
                                    if (bodyFound && (line != Body.OpeningElement))
                                    {
                                        if (line.EndsWith(Body.ParaBreakChar) == false)
                                            textLines.Add(line);
                                        else
                                        {
                                            textLines.Add(line.Remove(line.Length-1));
                                            textLines.Add(Environment.NewLine);
                                        }
                                        rowIndex++;
                                    }
                                }
                            }
                            if ((bodyFound == false) && (rowIndex == 0))
                                rc.SetError(1260104, MxError.Source.User, $"{exportOutputFile} is empty", MxMsgs.MxErrExportOutputFileEmpty);
                            else
                            {
                                var output = "";
                                var row = 0;
                                foreach (var line in textLines)
                                {
                                    if (line == Environment.NewLine)
                                        output += Environment.NewLine;
                                    else
                                    {
                                        if ((row + 1 >= textLines.Count) || (textLines[row + 1] == Environment.NewLine))
                                            output += line;
                                        else
                                            output += line + " ";
                                    }  
                                    row++;
                                }
                                using (var file = new StreamWriter(exportOutputFile)) //default StreamBuffer size is 1024
                                {
                                    file.Write(output);
                                }
                                if (bodyFound)
                                    rc.SetResult($"Succeeded: {exportInputFile} lacks closing {Body.ClosingElement}, but {rowIndex+1} lines still exported to {exportOutputFile}");
                                else
                                    rc.SetResult($"Succeeded: {exportInputFile} exported {rowIndex + 1} lines to {exportOutputFile}");
                            }
                        }
                        catch (Exception e)
                        {
                            rc.SetError(1260105, MxError.Source.Exception, e.Message, MxMsgs.MxErrException);
                        }
                    }
                }
            }
            return rc;
        }
    }
}
