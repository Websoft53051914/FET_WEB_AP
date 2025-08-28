using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility.Web.Base
{
    //public class JsonNetResult : JsonResult
    //{
    //    //public JsonSerializerSettings SerializerSettings { get; set; }
    //    //public Formatting Formatting { get; set; }
    //    //public JsonNetResult(object value)
    //    //{
    //    //    SerializerSettings = new JsonSerializerSettings();
    //    //}
    //    //public override void ExecuteResult(ControllerContext context)
    //    //{
    //    //    if (context == null)
    //    //        throw new ArgumentNullException("context");
    //    //    HttpResponse response = context.HttpContext.Response;
    //    //    response.ContentType =
    //    //        !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
    //    //    //if (ContentEncoding != null)
    //    //    //    response.ContentEncoding = ContentEncoding;
    //    //    if (Data != null)
    //    //    {
    //    //        JsonTextWriter writer = new JsonTextWriter(response.Output)
    //    //        {
    //    //            Formatting = Formatting
    //    //        };
    //    //        JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
    //    //        serializer.Serialize(writer, Data); writer.Flush();
    //    //    }
    //    //}
    //}
}
