﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace backend.Translations {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class prijava {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal prijava() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("backend.Translations.prijava", typeof(prijava).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string prekoGoogla {
            get {
                return ResourceManager.GetString("prekoGoogla", resourceCulture);
            }
        }
        
        internal static string ili {
            get {
                return ResourceManager.GetString("ili", resourceCulture);
            }
        }
        
        internal static string email {
            get {
                return ResourceManager.GetString("email", resourceCulture);
            }
        }
        
        internal static string lozinka {
            get {
                return ResourceManager.GetString("lozinka", resourceCulture);
            }
        }
        
        internal static string nemasNalog {
            get {
                return ResourceManager.GetString("nemasNalog", resourceCulture);
            }
        }
        
        internal static string btnPrijava {
            get {
                return ResourceManager.GetString("btnPrijava", resourceCulture);
            }
        }
        
        internal static string val_required {
            get {
                return ResourceManager.GetString("val-required", resourceCulture);
            }
        }
        
        internal static string val_invalidlog {
            get {
                return ResourceManager.GetString("val-invalidlog", resourceCulture);
            }
        }
    }
}