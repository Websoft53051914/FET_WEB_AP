using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_WEB.Models.Handler;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace FTT_WEB.Common.OriginClass.EntiityClass
{
    public class fet_dept_profileEntity
    {

        public string deptcode { get; set; }

        public string deptengname { get; set; }

        public string deptchiname { get; set; }

        public string parent { get; set; }

        public string sdeptname { get; set; }

        public string costcenterflg { get; set; }

        public string costcenter { get; set; }

        public string compcode { get; set; }

        public string compname { get; set; }

        public DateTime? setdate { get; set; }

        public DateTime? offdate { get; set; }

        public string empno { get; set; }

        public string deptlevel { get; set; }

        public string deptlevelname { get; set; }

        public string depttype { get; set; }

        public string depttypename { get; set; }

        public string full_sname { get; set; }

        public string mgr_empno { get; set; }
    }

    public class fet_dept_profileDTO : fet_dept_profileEntity
    {
        public int No { get; set; }
    }


}
