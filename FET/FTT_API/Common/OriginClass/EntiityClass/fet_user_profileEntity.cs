using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class fet_user_profileEntity
    {
        public string aliasname { get; set; }
        public string empno { get; set; }

        public string deptcode { get; set; }

        public string empname { get; set; }

        public string engname { get; set; }

        public string sex { get; set; }
        public string email { get; set; }

        public string mobile { get; set; }

        public string ext { get; set; }

        public string titlename { get; set; }

        public string region { get; set; }

        public string regionname { get; set; }

        public string costcenter { get; set; }

        public string locationcode { get; set; }

        public string locationname { get; set; }

        public DateTime? entdate { get; set; }

        public DateTime? offdate { get; set; }

        public DateTime? finaldate { get; set; }

        public string emptype { get; set; }

        public string opid { get; set; }

        public string loginid { get; set; }

        public string repflg { get; set; }

        public string rocid { get; set; }

        public string agent { get; set; }

        public string titleengname { get; set; }

        public string sr_agent { get; set; }

        public string compname { get; set; }

        public string rigion { get; set; }

        public string rigionname { get; set; }

        public string deptengname { get; set; }

        public string deptchiname { get; set; }

        public string parent { get; set; }

        public string sdeptname { get; set; }

        public string costcenterflg { get; set; }

        public int? compcode { get; set; }

        public string setdate { get; set; }

        public int? deptlevel { get; set; }

        public string deptlevelname { get; set; }

        public string depttype { get; set; }

        public string depttypename { get; set; }
        public string mgr_empno { get; set; }
    }

    public class fet_user_profileDTO : fet_user_profileEntity
    {
        public int No { get; set; }
        public string deptname { get; set; }
    }
}
