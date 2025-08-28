using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class access_roleEntity
    {
        public string form_type { get; set; }
        public string user_type { get; set; }
        public string form_no { get; set; }
        public string role { get; set; }
        public string empno { get; set; }
        public string deptcode { get; set; }
        public string action { get; set; }
        public string ifnullskip { get; set; }
        public string updatetime { get; set; }
        public string update_empno { get; set; }
        public string approve_status { get; set; }
        public string priority { get; set; }
        public string root_no { get; set; }
        public string find_self { get; set; }
        public string approve { get; set; }
        public string user_group { get; set; }
        public string find_approve_next { get; set; }
        public string maxlevel { get; set; }
    }

    public class access_roleDTO : access_roleEntity
    {
        public int No { get; set; }
        public string STATUS_NAME { get; set; }
        public string SQL { get; set; }
    }


}
