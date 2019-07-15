﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KLineEdCmdApp.Utils.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KLineEdCmdApp.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: internal parameter is invalid. This is a coding defect which has been reported. Please use alternative functionality until fixed..
        /// </summary>
        internal static string MxErrBadMethodParam {
            get {
                return ResourceManager.GetString("MxErrBadMethodParam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: invalid condition. This is a coding defect which has been reported. Please use alternative functionality until fixed..
        /// </summary>
        internal static string MxErrInvalidCondition {
            get {
                return ResourceManager.GetString("MxErrInvalidCondition", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: parameter or argument is not correctly set. This is a coding defect which has been reported. Please try again with another parameter or install a later version of the application.
        /// </summary>
        internal static string MxErrInvalidParamArg {
            get {
                return ResourceManager.GetString("MxErrInvalidParamArg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: your settings file is invalid. The file used for saving your program parameters and arguments is corrupt. Please delete KLineEdApp.json and try again.
        /// </summary>
        internal static string MxErrInvalidSettingsFile {
            get {
                return ResourceManager.GetString("MxErrInvalidSettingsFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: line is too long. You have reached the end of the line. Press the Enter key to continue.
        /// </summary>
        internal static string MxErrLineTooLong {
            get {
                return ResourceManager.GetString("MxErrLineTooLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: test of error handling. Please take no action.
        /// </summary>
        internal static string MxErrTest {
            get {
                return ResourceManager.GetString("MxErrTest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error: parameter not supported. This is a coding defect which has been reported. Please try again with another parameter or install a later version of the application.
        /// </summary>
        internal static string MxErrUnsupportedParam {
            get {
                return ResourceManager.GetString("MxErrUnsupportedParam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Message not found. Coding defect. Please report this problem.
        /// </summary>
        internal static string MxMsgNotFound {
            get {
                return ResourceManager.GetString("MxMsgNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} v{1}.{3}Copyright {2}.{3}Use subject to standard MIT License - see https://github.com/wpqs/AwsDotNetCmdUtils {3}.
        /// </summary>
        internal static string WelcomeNotice {
            get {
                return ResourceManager.GetString("WelcomeNotice", resourceCulture);
            }
        }
    }
}