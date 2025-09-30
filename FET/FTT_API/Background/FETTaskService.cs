using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using static Const.Enums;
using System.Net.Sockets;
using Core.Utility.Extensions;
using System.Net.NetworkInformation;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Linq;
using FTT_API.Background;

/// <summary>
///  檢查所有設備連線狀態
/// </summary>
public partial class FETTaskService : BackgroundService
{
    private readonly IConfiguration _config;

    public FETTaskService( IConfiguration config)
    {
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {  
            while (true)
            {
                //string logPath = $@".\logs\Dispatch_TT_{DateTime.Now:yyyyMMdd}.log";
                var _FETTaskHelper = new FETTaskHelper();
                _FETTaskHelper.Send_TT_No_RootCause("");
                //_FETTaskHelper.Close();

                await Task.Delay(5000, stoppingToken);
            }
        }
        catch (Exception e)
        {

        }
    }
}