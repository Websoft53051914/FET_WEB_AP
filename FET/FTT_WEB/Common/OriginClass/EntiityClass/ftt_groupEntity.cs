using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class ftt_groupEntity
    {
        public string EmpNo { get; set; }
        public string FTT_Group { get; set; }
        public string CName { get; set; }
        public string EName { get; set; }
        public string Ext { get; set; }
    }

    public class ftt_groupDTO : ftt_groupEntity
    {
        public int No { get; set; }
    }


}
