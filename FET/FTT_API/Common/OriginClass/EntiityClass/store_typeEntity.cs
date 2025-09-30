using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class store_typeEntity
    {
        public string type_name { get; set; }
        public string type_value { get; set; }
        public int order_id { get; set; }
        public int type_id { get; set; }
    }

    public class store_typeDTO : store_typeEntity
    {
        public int No { get; set; }

        public string USERROLE { get; set; }
        public string EMPNO { get; set; }
        public string IVRCODE { get; set; }

        public string STORE_TYPE { get; set; }
    }
}
