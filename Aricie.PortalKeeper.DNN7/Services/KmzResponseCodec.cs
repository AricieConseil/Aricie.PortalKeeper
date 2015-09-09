using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Ionic.Zip;
using SharpKml.Engine;

namespace Aricie.PortalKeeper.DNN7.Services
{
    public class KmzResponseCodec : Aricie.IO.HttpResponseCodecBase
    {
        public override bool CanProcess(HttpContext context)
        {
            return context.Response.ContentType.EndsWith("google-earth.kmz");
        }

        public override string Decode(HttpContext context, MemoryStream responseStream, ref object state)
        {
            var objKmz = SharpKml.Engine.KmzFile.Open(responseStream);
            state = objKmz;
            return objKmz.ReadKml();
        }

        public override Stream Encode(HttpContext context, string responseString, object state)
        {
            var objKmz = (KmzFile)state;

            var bytes = Encoding.UTF8.GetBytes(responseString);
            //using (var reader = new StringReader(responseString))
            //{
            //KmlFile objKml = KmlFile.Load(reader);
            //using (var ms = new MemoryStream())
            //{

            //objKml.Save(ms);
            var kmlFileName = objKmz.Files.First(strFileName => strFileName.ToLowerInvariant().EndsWith(".kml"));
            //objKmz.UpdateFile("doc.kml", bytes);// ms.ToArray());
            objKmz.UpdateFile(kmlFileName, bytes);// ms.ToArray());
            var toReturn = new MemoryStream();
            objKmz.Save(toReturn);
            return toReturn;
            //}
            //}
        }
    }
}


