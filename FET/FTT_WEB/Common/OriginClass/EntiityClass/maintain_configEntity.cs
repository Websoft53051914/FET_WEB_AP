using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class maintain_configEntity
    {
        public string config_name { get; set; }
        public string config_value { get; set; }
        public string config_desc { get; set; }
        public string update_date { get; set; }
        public string configi_desc { get; set; }
    }

    public class maintain_configDTO : maintain_configEntity
    {
        public int No { get; set; }
    }


}
