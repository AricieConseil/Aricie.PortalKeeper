using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Xml.Serialization;
using Aricie.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aricie.PortalKeeper.DNN7.WebAPI
{
    public class BrowserJsonFormatter : JsonMediaTypeFormatter
    {
        public BrowserJsonFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            //this.SerializerSettings.Formatting = Formatting.Indented;
            //this.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            //this.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            //this.SerializerSettings.ContractResolver = new XmlAwareContractResolver();
            this.SerializerSettings.SetDefaultSettings();
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            if (mediaType == null || mediaType.MediaType == "text/html")
            {
                headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
        }
    }


    //public class XmlAwareContractResolver : DefaultContractResolver
    //{
    //    private readonly JsonMediaTypeFormatter formatter;

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="MyContractResolver " /> class.
    //    /// </summary>
    //    public XmlAwareContractResolver(JsonMediaTypeFormatter formatter)
    //    {
    //        this.formatter = formatter;
    //    }

    //    /// <summary>
    //    /// Gets the formatter.
    //    /// </summary>
    //    public JsonMediaTypeFormatter Formatter
    //    {
    //        [DebuggerStepThrough]
    //        get { return this.formatter; }
    //    }

    //    #region Overrides of DefaultContractResolver

    //    /// <summary>
    //    /// Creates a <see cref="JsonProperty" /> for the given <see cref="MemberInfo" />.
    //    /// </summary>
    //    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    //    {
    //        JsonProperty property = base.CreateProperty(member, memberSerialization);
    //        this.ConfigureProperty(member, property);
    //        return property;
    //    }

    //    #endregion

    //    #region Private Methods

    //    // Determines whether a member is required or not and sets the appropriate JsonProperty settings
    //    private void ConfigureProperty(MemberInfo member, JsonProperty property)
    //    {
    //        // Check for NonSerialized attributes
    //        if (Attribute.IsDefined(member, typeof(NonSerializedAttribute), true) || Attribute.IsDefined(member, typeof(XmlIgnoreAttribute), true))
    //        {
    //            property.Ignored = true;
    //        }
    //        //if (typeof(ICollection).IsAssignableFrom(property.PropertyType)  )
    //        //{
    //        //    var formerExp = property.GetIsSpecified;
    //        //    property.GetIsSpecified = delegate(object o)
    //        //    {
    //        //        return ((formerExp == null) || formerExp(o)) && (!IsEmptyCollection(o));
    //        //    };
    //        //}
    //    }

    //    //private bool IsEmptyCollection(object o)
    //    //{
    //    //    if (o==null)
    //    //    {
    //    //        return false;
    //    //    }
    //    //    if  (!(o is ICollection))
    //    //    {
    //    //        return false;
    //    //    }
    //    //    if (((ICollection)o).Count > 0)
    //    //    {
    //    //        return false;
    //    //    }
    //    //    return true;

    //    //}

    //    #endregion
    //}

}