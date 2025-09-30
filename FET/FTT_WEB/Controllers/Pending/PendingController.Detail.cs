
using Core.Utility.Enums;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FTT_WEB.Common;
using FTT_WEB.Common.OriginClass;
using FTT_WEB.Common.OriginClass.EntiityClass;
using FTT_WEB.Models;
using FTT_WEB.Models.Handler;
using FTT_WEB.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph.Models;
using Npgsql;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.POIFS.Crypt.Agile;
using System.Data;
using System.Data.Common;

namespace FTT_WEB.Controllers.Pending
{
    public partial class PendingController : BaseProjectController
    {
        public IActionResult Detail(string formNo)
        {
            ViewData["form_no"] = formNo; ;

            return View();
        }
    }
}