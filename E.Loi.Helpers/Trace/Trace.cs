namespace E.Loi.Helpers.Trace;

public static class Trace
{

    public static void Logging(Exception exception, string methodeName, string repository)
    {
        StreamWriter sw = File.AppendText("C:\\logs\\log.txt");
        sw.WriteLine("------------------- Trace du " + DateTime.Now.ToString() + " -------------------");
        sw.WriteLine("[repository] :{0}", repository);
        sw.WriteLine("[methodeName] :{0}", methodeName);
        sw.WriteLine("[TargetSite] :{0}", exception.TargetSite);
        sw.WriteLine("[Message] :{0}", exception.Message);
        if (exception.InnerException != null) sw.WriteLine("[InnerException] :{0}", exception.InnerException);
        sw.WriteLine("[Exception] :{0}", exception.ToString());
        sw.WriteLine("[Line] : {0}", new System.Diagnostics.StackTrace(exception, true).GetFrame(0).GetFileColumnNumber());
        sw.WriteLine();
        sw.Close();
    }
}
