using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace KLineEdCmdApp.Utils
{
    public static class MxReturnCodeUtils       //implement in MxReturnCode at next release - assume mxErrorMsg format is error 1100703-user: Warning: you cannot move beyond the end of the chapter
    {
        public static readonly string MxErrorMessageFirstWord = "error";
        public static readonly char MxErrorMessageTypeSeparator = '-';
        public static readonly string ErrorTextSeparator = ": ";

        public const string ErrorMsgPrecursor = "Error: ";       //error messages in resources MAY start with this text 
        public const string WarningMsgPrecursor = "Warning: ";   //All warning messages in resources start with this text 
        public const string InfoMsgPrecursor = "Info: ";         //All info messages in resources start with this text 

        public enum ErrorType
        {
            [EnumMember(Value = "exception")] exception = 0,
            [EnumMember(Value = "program")] program = 1,
            [EnumMember(Value = "data")] data = 2,
            [EnumMember(Value = "param")] param = 3,
            [EnumMember(Value = "user")] user = 4
        }

        public enum MsgClass
        {
            [EnumMember(Value = "Error")] Error = 0,
            [EnumMember(Value = "Warning")] Warning = 1,
            [EnumMember(Value = "Info")] Info = 2,
            [EnumMember(Value = "Unknown")] Unknown = 3
        }

        public static int GetErrorCode(string mxErrorMsg) //returns '1100703' from 'error 1100703-user: Warning: you cannot move beyond the end of the chapter' 
        {
            var rc = Program.PosIntegerNotSet;

            if (mxErrorMsg != null)
            {
                var errorPartStart = MxErrorMessageFirstWord;
                var start = mxErrorMsg.ToLower().IndexOf(errorPartStart, StringComparison.Ordinal);
                if (start >= 0)
                {
                    var end = mxErrorMsg.IndexOf(MxErrorMessageTypeSeparator);
                    if (end >= 0)
                    {
                        var errorCode = mxErrorMsg.Snip(start + errorPartStart.Length, end - 1);
                        if (int.TryParse(errorCode, out var num))
                            rc = num;
                    }
                }
            }
            return rc;
        }

        public static MsgClass GetErrorClass(string mxErrorMsg) //returns Error:, Warning: or Info: from 'error 1100703-user: Warning: you cannot move beyond the end of the chapter'
        {
            var rc = MsgClass.Unknown;

            if (mxErrorMsg != null)
            {
                var errorPartStart = MxErrorMessageFirstWord;
                var start = mxErrorMsg.ToLower().IndexOf(errorPartStart, StringComparison.Ordinal);
                if (start >= 0)
                {
                    var end = mxErrorMsg.IndexOf(MxErrorMessageTypeSeparator);
                    if (end >= 0)
                    {
                        var startTextIndex = mxErrorMsg.IndexOf(ErrorTextSeparator, StringComparison.Ordinal);
                        if (startTextIndex >= 0)
                        {
                            var msg = mxErrorMsg.Substring(startTextIndex + ErrorTextSeparator.Length);
                            if (msg.StartsWith(ErrorMsgPrecursor, StringComparison.CurrentCultureIgnoreCase))
                                rc = MsgClass.Error;
                            else if (msg.StartsWith(WarningMsgPrecursor, StringComparison.CurrentCultureIgnoreCase))
                                rc = MsgClass.Warning;
                            else if (msg.StartsWith(InfoMsgPrecursor, StringComparison.CurrentCultureIgnoreCase))
                                rc = MsgClass.Info;
                            else
                                rc = MsgClass.Error; //if no precursor, then assume error
                        }
                    }
                }
            }
            return rc;
        }

        public static string GetErrorText(string mxErrorMsg) //returns 'you cannot move beyond the end of the chapter' from 'error 1100703-user: Warning: you cannot move beyond the end of the chapter' 
        {
            var rc = Program.ValueNotSet;

            if (mxErrorMsg != null)
            {
                var errorPartStart = MxErrorMessageFirstWord;
                var start = mxErrorMsg.ToLower().IndexOf(errorPartStart, StringComparison.Ordinal);
                if (start < 0)
                    rc = mxErrorMsg;
                else
                {
                    var end = mxErrorMsg.IndexOf(MxErrorMessageTypeSeparator);
                    if (end < 0)
                        rc = mxErrorMsg;
                    else
                    {
                        var errorCode = mxErrorMsg.Snip(start + errorPartStart.Length, end - 1);
                        var startTextIndex = mxErrorMsg.IndexOf(ErrorTextSeparator, StringComparison.Ordinal); 
                        if ((startTextIndex < 0) || (errorCode == null))
                            rc = mxErrorMsg;
                        else
                        {
                            var msg = mxErrorMsg.Substring(startTextIndex + ErrorTextSeparator.Length);
                            if (msg.StartsWith(ErrorMsgPrecursor, StringComparison.CurrentCultureIgnoreCase))
                                rc = msg.Substring(ErrorMsgPrecursor.Length);
                            else if (msg.StartsWith(WarningMsgPrecursor, StringComparison.CurrentCultureIgnoreCase))
                                rc = msg.Substring(WarningMsgPrecursor.Length);
                            else if (msg.StartsWith(InfoMsgPrecursor, StringComparison.CurrentCultureIgnoreCase))
                                rc = msg.Substring(InfoMsgPrecursor.Length);
                            else
                                rc = msg; //if no precursor, then assume error
                        }
                    }
                }
            }
            return rc;
        }
    }
}
