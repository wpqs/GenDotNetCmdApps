﻿using System;
using System.IO;
using KLineEdCmdApp.Utils;
using KLineEdCmdApp.Model;
using KLineEdCmdApp.View;
using KLineEdCmdAppTest.TestSupport.Base;
using MxDotNetUtilsLib;

namespace KLineEdCmdAppTest.TestSupport
{
    public class ModelTextEditViewFixture :  ModelViewBaseFixture
    {
        public TextEditView View { get; }

        public ModelTextEditViewFixture()
        {
            if (Error == TestConst.UnitTestNone)
            {
                Error = TestConst.UnitTestNotSet;
                View = new TextEditView(new MockTerminal());
                var rcTerm = View.Setup(AppCmdLineParams);
                if (rcTerm.IsError())
                    Error = rcTerm.GetErrorTechMsg();
                else
                {
                    if ((Unsubscribe = Model.Subscribe(View)) != null)
                    {
                        Error = TestConst.UnitTestNone;
                    }
                }
            }
        }
    }
}
