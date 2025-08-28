using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class fet_user_profileEntity
    {
        public string EmpNo { get; set; }
        public string EmpName  { get; set; }
        public string EngName { get; set; }
        public string Ext { get; set; }

    }

    public class fet_user_profileDTO : fet_user_profileEntity
    {
        public int No { get; set; }
    }


}
