
using Const;
using static Const.Enums;

namespace FTT_VENDER_WEB.Common
{

    //原
    public partial class SessionVO
    {
        public string empno { get; set; }
        public string empname { get; set; }
        public string engname { get; set; }
        public string ext { get; set; }
        public string username { get; set; }
        public string deptcode { get; set; }
        public string usertype { get; set; }
        public string ivrcode { get; set; }

        public string userrole { get; set; }


        public List<FuncID> Functions { get; set; } = new();

    }
}
