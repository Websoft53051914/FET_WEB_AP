using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class v_ftt_form2Entity
    {
        public string form_no { get; set; }
        public string tt_category { get; set; }
        public string l2_desc { get; set; }
        public string ciname { get; set; }
        public string createtime { get; set; }
        public string shop_name { get; set; }
        public string statusname { get; set; }
        public string updatetime { get; set; }
        public string StatusId { get; set; }


    }

    public class v_ftt_form2DTO : v_ftt_form2Entity
    {
        public int No { get; set; }


        public string USERROLE { get; set; }
        public string EMPNO { get; set; }
        public string IVRCODE { get; set; }


        public bool IsTicket { get; set; }

        public string CurrentInchargeName { get; set; }

    }


}
