using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Aricie.DNN7.Web.UI
{
    class AricieVersionEditControl: DotNetNuke.UI.WebControls.VersionEditControl
    {

        protected override void RenderViewMode(System.Web.UI.HtmlTextWriter writer)
        {
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            if (Version != null)
            {
                writer.Write(Version.ToString());
            }
            writer.RenderEndTag();
        }

    }
}
