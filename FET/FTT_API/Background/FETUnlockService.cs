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
public partial class FETUnlockService : BackgroundService
{
    private readonly IConfiguration _config;

    public FETUnlockService(IConfiguration config)
    {
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var _FETUnlockHelper = new FETUnlockHelper();

            while (true)
            {
                _FETUnlockHelper.Unlock("");
                await Task.Delay(1000 * 60, stoppingToken);
            }
        }
        catch (Exception e)
        {

        }
    }
}