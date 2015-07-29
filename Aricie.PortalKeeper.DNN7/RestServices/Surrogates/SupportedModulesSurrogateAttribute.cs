using System;
using System.Collections.Generic;
using System.ComponentModel;
using Aricie.DNN.ComponentModel;
using Aricie.DNN.UI.Attributes;
using Aricie.DNN.UI.WebControls;
using Aricie.DNN.UI.WebControls.EditControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.Api;

namespace Aricie.PortalKeeper.DNN7
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SupportedModulesSurrogateAttribute : Attribute, IAttributeSurrogate
    {
        [Editor(typeof(MultiSelectorEditControl), typeof(EditControl))]
        [MultiSelectorType(typeof(DesktopModuleMultiSelector))]
        [TextField("FriendlyName")]
        [ValueField("ModuleName", TypeCode.String)]
        public List<string> SupportedModules{ get; set; }


        public SupportedModulesSurrogateAttribute()
        {
            SupportedModules = new List<string>();
        }

        public Attribute GetAttribute()
        {
            return new SupportedModulesAttribute(string.Join(",", SupportedModules));
        }
    }
}