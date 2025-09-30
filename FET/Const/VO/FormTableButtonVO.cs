using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Const.VO
{
    /// <summary>
    /// _FormTable 按鈕
    /// </summary>
    public class FormTableButtonVO
    {
        /// <summary>
        /// 來自 form_access_control.allow_wording
        /// </summary>
        public string? StatusWording { get; set; }
        /// <summary>
        /// 來自 form_access_control.require_field
        /// </summary>
        public string? RequireField { get; set; }
        /// <summary>
        /// 來自 form_access_control.approve
        /// </summary>
        public string? Approve { get; set; }
        /// <summary>
        /// 來自 form_access_control.allow_status
        /// </summary>
        public string? Status { get; set; }
        /// <summary>
        /// 來自 form_access_control.form_type
        /// </summary>
        public string? FormType { get; set; }
        /// <summary>
        /// 來自 form_access_control.user_type
        /// </summary>
        public string? UserType { get; set; }
        /// <summary>
        /// 是否為建議／說明
        /// </summary>
        public bool IsApproveCommon { get; set; } = false;
    }
}
