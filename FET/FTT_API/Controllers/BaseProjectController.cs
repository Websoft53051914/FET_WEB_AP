

using Core.Utility.Helper.Message;
using Core.Utility.Web.Base;
using Microsoft.AspNetCore.Mvc;
using FTT_API.Common;
using FTT_API.Models;
using static Const.Enums;
using NPOI.SS.Formula.Functions;
using System.Diagnostics;

namespace FTT_API.Controllers
{
    public class BaseProjectController : BaseController
    {

        #region -- Instance --

        private MessageHelper? _msgHelper = null;
        /// <summary>
        /// 錯誤訊息資訊
        /// </summary>
        /// <returns></returns>
        public MessageHelper GetMessage()
        {
            _msgHelper ??= new MessageHelper();
            return _msgHelper;
        }

        private SelectListHandler? _selectListHandler = null;
        /// <summary>
        /// SelectListHandler
        /// </summary>
        /// <returns></returns>
        public SelectListHandler GetSelectListHandler()
        {
            _selectListHandler ??= new SelectListHandler();
            return _selectListHandler;
        }

        #endregion  -- Instance --

        /// <summary>
        /// 轉址至指定位置並顯示訊息
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="message"></param>
        /// <param name="alertType"></param>
        /// <returns></returns>
        protected IActionResult RedirectToAlertMsg(string actionName, string controllerName, string message, string alertType = "success")
        {
            var paras = new AlertMsgRedirection()
            {
                ActionName = actionName,
                ControllerName = controllerName,
                Msgs = new List<string>() { message },
                AlertType = alertType
            };

            if (HttpContext.Request != null && HttpContext.Request.Query.ContainsKey("className"))
            {
                paras.ClassName = HttpContext.Request.Query["className"].FirstOrDefault() ?? string.Empty;
            }

            return RedirectToAction("Redirection", "AlertMsg", paras);
        }

        protected void LogError(Exception ex)
        {
            Trace.Write("<font color=red>Source:" + ex.Source + "</font>");
            Trace.Write("<font color=red>Msg:" + ex.Message + "</font>");
            Trace.Write(ex.ToString());
        }


        /// <summary>
        /// 紀錄例外於資料庫
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>ControlLog.Id</returns>
        //protected long LogError(Exception ex)
        //{
        //    var blLog = BLFactory.GetInstance<LogBL>();

        //    var logDM = new ControlLogDM()
        //    {
        //        IP = LoginSession.Current.IP ?? Method.GetClientIPAddress(),
        //        Status = ((int)LogStatusEnum.Failed).ToString(),
        //        ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
        //        ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
        //        Exception = ex.ToString()
        //    };

        //    return blLog.InsertLog(logDM);
        //}

        /// <summary>
        /// 紀錄失敗訊息於資料庫
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>ControlLog.Id</returns>
        //protected long LogError(string exception)
        //{
        //    var blLog = BLFactory.GetInstance<LogBL>();

        //    var logDM = new ControlLogDM()
        //    {
        //        IP = LoginSession.Current.IP ?? Method.GetClientIPAddress(),
        //        Status = ((int)LogStatusEnum.Failed).ToString(),
        //        ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
        //        ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
        //        Exception = exception
        //    };

        //    return blLog.InsertLog(logDM);
        //}

        /// <summary>
        /// 紀錄成功訊息於資料庫
        /// </summary>
        /// <param name="description"></param>
        //protected void LogSuccess(string description = null)
        //{
        //    var blLog = BLFactory.GetInstance<LogBL>();

        //    var logDM = new ControlLogDM()
        //    {
        //        IP = LoginSession.Current.IP ?? Method.GetClientIPAddress(),
        //        Status = ((int)LogStatusEnum.Success).ToString(),
        //        ControllerName = ControllerContext.ActionDescriptor?.ControllerName ?? string.Empty,
        //        ActionName = ControllerContext.ActionDescriptor?.ActionName ?? string.Empty,
        //        Exception = description,
        //    };

        //    blLog.InsertLog(logDM);
        //}

    }


}
