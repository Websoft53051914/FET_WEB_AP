using Const.DTO;
using Core.Utility.Web.EX;

namespace Const.VO
{
    public class DispatchingIndexVO
    {

    }

    public class DispatchingGridVO
    {
        /// <summary>
        /// 工單號碼
        /// </summary>
        [SortColumn(nameof(VFttForm2DTO.form_no))]
        public int? FormNo { get; set; }
        /// <summary>
        /// 報修型態
        /// </summary>
        [SortColumn(nameof(VFttForm2DTO.tt_category))]
        public string? TtCategory { get; set; }
        /// <summary>
        /// 報修類別
        /// </summary>
        [SortColumn(nameof(VFttForm2DTO.l2_desc))]
        public string? L2Desc { get; set; }
        /// <summary>
        /// 報修品項
        /// </summary>
        [SortColumn(nameof(VFttForm2DTO.ciname))]
        public string? CiName { get; set; }
        /// <summary>
        /// 報修日期
        /// </summary>
        [SortColumn(nameof(VFttForm2DTO.createtime_text))]
        public string? CreateTimeText { get; set; }
        /// <summary>
        /// 工單狀態
        /// </summary>
        [SortColumn(nameof(VFttForm2DTO.statusname))]
        public string? StatusName { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        [SortColumn(nameof(VFttForm2DTO.updatetime), DefaultSortOrder = "DESC", IsDefault = true)]
        public string? UpdateTimeText { get; set; }
    }
}
