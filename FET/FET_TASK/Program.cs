//using System;

//class Program
//{
//    static void Main()
//    {
//        string logPath = $@".\logs\Dispatch_TT_{DateTime.Now:yyyyMMdd}.log";
//        var db = new FETTaskHelper(logPath);

//        try
//        {
//            db.Open();

//            db.InsertLog("--------***  Start process IT Trouble-Tickets ***--------");
//            db.Send_TT_No_RootCause("");
//            db.InsertLog("--------***  Program end normally ***--------");
//        }
//        catch (Exception ex)
//        {
//            db.ErrorLog("Main Program Error", ex);
//        }
//        finally
//        {
//            db.Close();
//        }
//    }
//}