
using Const;
using static Const.Enums;

namespace FTT_VENDER_API.Common
{

    //原
    public partial class SessionVO
    {
        //public string empno { get; set; }
        //public string empname { get; set; }
        //public string engname { get; set; }
        //public string ext { get; set; }
        //public string username { get; set; }
        //public string deptcode { get; set; }
        //public string usertype { get; set; }
        //public string ivrcode { get; set; }

        //public string userrole { get; set; }

        public string empno { get; set; } = "boss456";
        public string empname { get; set; } = "鋐達室內裝修工程有限公司";
        public string engname { get; set; } = "鋐達室內裝修工程有限公司";
        public string ext { get; set; } = "0912345678(78181)";
        public string username { get; set; } = "boss456";
        public string deptcode { get; set; } = "鋐達室內裝修工程有限公司";
        public string usertype { get; set; } = "VENDOR";
        public string ivrcode { get; set; } = "29";

        public string userrole { get; set; } = "VENDOR";//= "SUBMITTER";
        public string shop_name { get; set; }

        public List<FuncID> Functions { get; set; } = new();

    }
}
