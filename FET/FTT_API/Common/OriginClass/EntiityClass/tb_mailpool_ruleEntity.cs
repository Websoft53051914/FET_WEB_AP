using Core.Utility.Helper.DB;
using Core.Utility.Helper.DB.Entity;
using DocumentFormat.OpenXml.Bibliography;
using FTT_API.Models.Handler;
using System.Data;

namespace FTT_API.Common.OriginClass.EntiityClass
{
    public class tb_mailpool_ruleEntity
    {
        /// <summary>
        /// 自動流水號 (Primary Key)
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 郵件類型
        /// </summary>
        public string mail_type { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string mail_reciver { get; set; }

        /// <summary>
        /// 副本收件人
        /// </summary>
        public string mail_reciver_cc { get; set; }

        /// <summary>
        /// 郵件主旨
        /// </summary>
        public string mailsubject { get; set; }

        /// <summary>
        /// 郵件開頭
        /// </summary>
        public string mailhead { get; set; }

        /// <summary>
        /// 郵件內容模板
        /// </summary>
        public string mailcontent { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 建立者 ID
        /// </summary>
        public int? creator { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime? createtime { get; set; }

        /// <summary>
        /// 更新者 ID
        /// </summary>
        public int? updater { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? updatetime { get; set; }

    }

    public class tb_mailpool_ruleDTO : tb_mailpool_ruleEntity
    {

    }


}
