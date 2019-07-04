﻿using System;
using MxReturnCode;

namespace KLineEdCmdApp.Model
{
    public abstract class HeaderBase
    {
        public static readonly string PropertyNotSet = "";
        public static readonly string ValueNotSet = "[not set]";

        public abstract override string ToString();
        public  abstract string GetReport();
        public abstract bool Validate();
        public abstract bool IsLabelFound(string name);
        public abstract MxReturnCode<bool> InitialiseFromString(string toString);
        public abstract void Reset();

        protected bool Error;

        protected HeaderBase()
        {
            Error = true;
        }
        public bool IsError()
        {
            return Error;
        }

        protected string GetString(string value, string existingValue, out bool doneOk)
        {
            var rc = existingValue;
            doneOk = false;
            if ((string.IsNullOrEmpty(value) == false) && (IsLabelFound(value) == false))
            {
                rc= value.Trim();
                doneOk = true;
            }
            return rc;
        }
        protected int GetPosInteger(string value, int existingValue, out bool doneOk)
        {
            var rc = existingValue;
            doneOk = false;
            if ((string.IsNullOrEmpty(value) == false) && (IsLabelFound(value) == false))
            {
                if (int.TryParse(value, out var result))
                {
                    if (result >= 0)
                    {
                        rc = result;
                        doneOk = true;
                    }
                }
            }
            return rc;
        }
        protected DateTime? GetDateTime(string value, DateTime? existingValue, out bool doneOk)
        {
            var rc = existingValue;
            doneOk = false;
            if ((string.IsNullOrEmpty(value) == false) && (IsLabelFound(value) == false))
            {
                if (DateTime.TryParse(value, out var result))
                {
                    rc = result;
                    doneOk = true;
                }
            }
            return rc;
        }
        protected TimeSpan? GetTimeSpan(string value, TimeSpan? existingValue, out bool doneOk)
        {
            var rc = existingValue;
            doneOk = false;
            if ((string.IsNullOrEmpty(value) == false) && (IsLabelFound(value) == false))
            {
                if (TimeSpan.TryParse(value, out var result))
                {
                    rc = result;
                    doneOk = true;
                }
            }
            return rc;
        }
    }
}
