using System;
using MxDotNetUtilsLib;
using MxReturnCode;
// ReSharper disable All

namespace KLineEdCmdApp.Model
{
    public class HeaderSessionPause : HeaderBase
    {
        public static readonly char Separator = ',';
        public static readonly char Terminator = ';';
        public static readonly int  MinRecordLength = 10; //00:00:00,0;
        public static readonly int  TimeFieldLength = 8;  //00:00:00;

        public DateTime? PauseTime { get; private set; }
        public int Duration { get; private set; }   //seconds

        public HeaderSessionPause()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Reset();    
        }

        public override bool Validate()
        {
            // ReSharper disable once ReplaceWithSingleAssignment.False
            var rc = false;
            if ((PauseTime != null) && (Duration >= 0))
                rc = true;
            Error = !rc;  //set Error = false if all properties are now valid, else Error = true;
            return rc;
        }

        public override void Reset()
        {
            PauseTime = null;
            Duration = KLineEditor.PosIntegerNotSet;
            Error = true;
        }
        public override string GetReport()
        {
            return ToString();
        }

        public override bool IsLabelFound(string name)
        {
            return false;   //labels not used in this class
        }

        public override string ToString()
        {
            var rc = HeaderBase.ValueNotSet;
            if (IsError() == false)     //order must be same as InitialiseFromString()
                rc = $"{PauseTime?.ToString(MxStdFrmt.Time) ?? "[null]"},{Duration.ToString()};";
            return rc;

        }
        public override MxReturnCode<bool> InitialiseFromString(string toString)
        {
            var rc = new MxReturnCode<bool>("HeaderSessionPause.InitialiseFromString");

            Reset();
  
            if (toString == null)
                rc.SetError(1080101, MxError.Source.Param, $"toString is null", "MxErrBadMethodParam");
            else
            {       //order must be same as ToString()
                var rcPause = SetPauseTime(toString);
                rc += rcPause;
                if (rcPause.IsSuccess(true))
                {
                    var rcDuration = SetDuration(toString);
                    rc += rcDuration;
                    if (rcDuration.IsSuccess(true))
                    {
                        rc.SetResult(true);
                    }
                }
                if (rc.IsError(true))
                    Reset();
            }
            return rc;
        }
        public MxReturnCode<bool> SetPauseTime(string record)
        {
            var rc = new MxReturnCode<bool>("HeaderSessionPause.SetPauseTime");

            if (string.IsNullOrEmpty(record) == true)
                rc.SetError(1080201, MxError.Source.Param, $"field is null or empty", "MxErrBadMethodParam");
            else
            {
                var value = record.Trim();
                if (value.Length < HeaderSessionPause.MinRecordLength)
                    rc.SetError(1080202, MxError.Source.Data, $"value={value} is invalid; too short", "MxErrInvalidCondition");
                else
                {
                    var end = value.IndexOf(Separator);
                    if ((end == -1) || (value[value.Length - 1] != Terminator) || (end != TimeFieldLength) || (value.Length < TimeFieldLength + 3))
                        rc.SetError(1080203, MxError.Source.Data, $"value={value} is invalid; no separator '{Separator}', no terminator '{Terminator}', or missing field", "MxErrInvalidCondition");
                    else
                    {
                        var tim = value.Snip(0, end - 1)?.Trim();
                        if (tim == null)
                            rc.SetError(1080204, MxError.Source.Data, $"Snip({0}, {end-1}) failed for value={value}", "MxErrInvalidCondition");
                        else
                        {
                            PauseTime = GetDateTime(tim, PauseTime, out var result);
                            if (result == false)
                                rc.SetError(1080205, MxError.Source.Data, $"GetDateTime({tim}) failed", "MxErrInvalidCondition");
                            else
                            {
                                rc.SetResult(true);
                                Validate();
                            }
                        }
                    }
                }
            }
            return rc;
        }

        public MxReturnCode<bool> SetDuration(string record)
        {
            var rc = new MxReturnCode<bool>("HeaderSessionPause.SetDuration");

            if (string.IsNullOrEmpty(record) == true)
                rc.SetError(1080301, MxError.Source.Param, $"record is null or empty", "MxErrBadMethodParam");
            else
            {
                var value = record.Trim();
                if (value.Length < HeaderSessionPause.MinRecordLength)
                    rc.SetError(1080302, MxError.Source.Data, $"value={value} is invalid; too short", "MxErrInvalidCondition");
                else
                {
                    var start = value.IndexOf(Separator);
                    if ((start == -1) || (value[value.Length-1] != Terminator) ||(start != TimeFieldLength) || (value.Length < TimeFieldLength+3))
                        rc.SetError(1080303, MxError.Source.Data, $"value={value} is invalid; no separator '{Separator}', no terminator '{Terminator}', or missing field", "MxErrInvalidCondition");
                    else
                    {
                        var number = value.Snip(start + 1, value.Length - 2)?.Trim();
                        if (number == null)
                            rc.SetError(1080304, MxError.Source.Data, $"Snip({start + 1},{value.Length - 2}) failed for value={value}", "MxErrInvalidCondition");
                        else
                        {
                            Duration = GetPosInteger(number, Duration, out var result);
                            if (result == false)
                                rc.SetError(1080305, MxError.Source.Data, $"GetPosInteger({number}) failed", "MxErrInvalidCondition");
                            else
                            {
                                rc.SetResult(true);
                                Validate();
                            }
                        }
                    }
                }
            }
            return rc;
        }
    }
}
