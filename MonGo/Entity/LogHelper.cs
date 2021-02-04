using log4net;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonGo.Entity
{
    /// <summary>
    /// log4net帮助类
    /// AdoNetAppender仅支持到.net framework4.5，不支持在.net core项目中持久化日志到数据库
    /// </summary>
    public class LogHelper
    {
        // 异常 // 注意：logger name不要写错
        private static readonly ILog logerror = LogManager.GetLogger(Log4NetRepository.loggerRepository.Name, "errLog");
        // 记录
        private static readonly ILog loginfo = LogManager.GetLogger(Log4NetRepository.loggerRepository.Name, "infoLog");

        public static void Error(string throwMsg, Exception ex)
        {
            string errorMsg = string.Format("【异常描述】：{0} <br>【异常类型】：{1} <br>【异常信息】：{2} <br>【堆栈调用】：{3}",
                new object[] {
                    throwMsg,
                    ex.GetType().Name,
                    ex.Message,
                    ex.StackTrace });
            errorMsg = errorMsg.Replace("\r\n", "<br>");
            logerror.Error(errorMsg);
        }

        public static void Info(string message)
        {
            loginfo.Info(string.Format("【日志信息】：{0}", message));
        }

    }
    public class LogServiceProvider
    {
        public static IServiceProvider ServiceProvider { get; set; }
    }
    public class Log4NetRepository
    {
        public static ILoggerRepository loggerRepository { get; set; }
    }
}
