﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DotNetCampus.Logger.Properties {
    using System;


    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Localizations {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Localizations() {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DotNetCampus.Logger.Properties.Localizations", typeof(Localizations).Assembly);
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
        ///   Looks up a localized string similar to Unknown Error.
        /// </summary>
        internal static string DL0000 {
            get {
                return ResourceManager.GetString("DL0000", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to An unknown error occurred..
        /// </summary>
        internal static string DL0000_Message {
            get {
                return ResourceManager.GetString("DL0000_Message", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change the entry class to a partial class.
        /// </summary>
        internal static string DL1001 {
            get {
                return ResourceManager.GetString("DL1001", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to After changing the entry class to a partial class, the source generator can generate auxiliary log code according to the subsequent log initialization requirements; this way, you can even start using logs in the first sentence of the Main method without worrying about initialization issues..
        /// </summary>
        internal static string DL1001_Description {
            get {
                return ResourceManager.GetString("DL1001_Description", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change &apos;{0}&apos; to a partial class.
        /// </summary>
        internal static string DL1001_Fix {
            get {
                return ResourceManager.GetString("DL1001_Fix", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Change &apos;{0}&apos; to a partial class to allow it to use the logging system before the logging module is initialized..
        /// </summary>
        internal static string DL1001_Message {
            get {
                return ResourceManager.GetString("DL1001_Message", resourceCulture);
            }
        }
    }
}
