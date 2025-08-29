
using Const;
using static Const.Enums;

namespace FTT_API.Common
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


        public string empno { get; set; } = "83272";
        public string empname { get; set; } = "丁x珊";
        public string engname { get; set; } = "Annie Dean";
        public string ext { get; set; } = "0912345678(78181)";
        public string username { get; set; } = "ydean";
        public string deptcode { get; set; } = "＊741571";
        public string usertype { get; set; } = "RETAIL";
        public string ivrcode { get; set; } = "1805318";

        public string userrole { get; set; } = "SUBMITTER";


        public IEnumerable<FuncID> Functions { get; set; } = Enumerable.Empty<FuncID>();

    }
}
