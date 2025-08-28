
namespace FTT_VENDER_WEB.Models.ViewModel
{
    public partial class MemberVM : BaseVM
    {
        public string? MEMBERACCOUNT { get; set; }
        public string? ACCOUNTNAME { get; set; }
        public int? PERMISSIONID { get; set; }
        public string? ACCOUNTSTATUS { get; set; }
        public DateTime LASTLOGINTIME { get; set; }
        public string? MEMBERPWD { get; set; }
        public DateTime? LASTMEMBERPWDTIME { get; set; }
        public string? ACCOUNTEMAIL { get; set; }
        public string? RESETPWDCODE { get; set; }
        public DateTime LASTFORGETPWDTIME { get; set; }
        public int LOGINS { get; set; }
        public DateTime? LOCKTIME { get; set; }
        public DateTime? LOGOUTTIME { get; set; }
        public string? TESTA { get; set; }
        public string? TESTAC { get; set; }
    }

    public partial class MemberVM
    {
        public long No { get; set; }
        public List<string> PermissionIDs { get; set; }
        public bool IsLock { get; set; }
        public string? RoleName { get; set; }
        public long RoleId { get; set; }
    }
}
