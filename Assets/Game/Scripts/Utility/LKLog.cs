using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace LostKingdom.Utility
{
    /// <summary> 日志对象 </summary>
    public class LKLog
    {
        /// <summary> 游戏信息源 </summary>
        public static readonly string Sender_GameName = LKLocalization.GetText("Logger.Sender.GameName");
        /// <summary> 默认信息源 </summary>
        public static readonly string Sender_System = "System";
        /// <summary> 警告信息源 </summary>
        public static readonly string Sender_Warning = LKLocalization.GetText("Logger.Sender.Warning");
        /// <summary> 异常信息源 </summary>
        public static readonly string Sender_Error = LKLocalization.GetText("Logger.Sender.Error");
        /// <summary> 全局默认日志对象 </summary>
        public static readonly LKLog Instance = new LKLog(null, "Logger.log");
        /// <summary> 日志文件存储路径 </summary>
        public string Path { get; private set; }
        /// <summary> 带后缀日志文件名 </summary>
        public string Name { get; private set; }

        /// <summary> 新建日志对象 </summary>
        /// <param Name="path"> 日志文件储存路径 </param>
        /// <param Name="name"> 日志文件名（带后缀，建议命名为*.log） </param>
        public LKLog(string path = null, string name = "Logger.log")
        {
            this.Path = System.IO.Path.GetFullPath(path ?? Environment.CurrentDirectory);
            this.Name = name;
            FileStream stream = new FileStream(System.IO.Path.Combine(this.Path, this.Name), FileMode.Create);
            stream.Close();
        }
        
        /// <summary> 记录常规信息到日志文件中 </summary>
        /// <param Name="log"> 信息 </param>
        /// <param Name="who"> 记录源 </param>
        public void LogMessage(object log, string who = null)
        {
            StreamWriter writer = new StreamWriter(System.IO.Path.Combine(this.Path, this.Name), true, Encoding.UTF8);
            writer.WriteLine(string.Format("[{0}] [{1}]: {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), who ?? LKLog.Sender_System, log));
            writer.Close();
            Debug.Log(log);
        }

        /// <summary> 记录警告信息到日志文件中 </summary>
        /// <param Name="log"> 警告信息 </param>
        /// <param Name="solution"> 对策方法 </param>
        public void LogWarning(object log, Action solution = null)
        {
            StreamWriter writer = new StreamWriter(System.IO.Path.Combine(this.Path, this.Name), true, Encoding.UTF8);
            writer.WriteLine(string.Format("[{0}] [{1}]: {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), LKLog.Sender_Warning, log));
            writer.Close();
            Debug.LogWarning(log);
            solution?.Invoke();
        }

        /// <summary> 记录异常信息到日志文件中 </summary>
        /// <param Name="log"> 异常信息 </param>
        /// <param Name="solution"> 对策方法 </param>
        public void LogError(object log, Action solution = null)
        {
            if (LK.IgnoreError) this.LogWarning(log, solution);
            else
            {
                StreamWriter writer = new StreamWriter(System.IO.Path.Combine(this.Path, this.Name), true, Encoding.UTF8);
                writer.WriteLine(string.Format("[{0}] [{1}]: {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), LKLog.Sender_Error, log));
                writer.Close();
                Debug.LogError(log);
                solution?.Invoke();
            }
        }

        /// <summary> 记录异常信息到日志文件中 </summary>
        /// <param Name="e"> 异常类 </param>
        /// <param Name="solution"> 对策方法 </param>
        public void LogError(Exception e, Action solution = null)
        {
            if (LK.IgnoreError) this.LogWarning(e.Message, solution);
            else
            {
                StreamWriter writer = new StreamWriter(System.IO.Path.Combine(this.Path, this.Name), true, Encoding.UTF8);
                writer.WriteLine(string.Format("[{0}] [{1}]: {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), LKLog.Sender_Error, e.Message));
                writer.Close();
                Debug.LogError(e.Message);
                if (solution == null) throw e;
                else solution.Invoke();
            }
        }

        /// <summary> 记录常规信息到日志文件中 </summary>
        /// <param Name="log"> 信息 </param>
        /// <param Name="who"> 信息源 </param>
        /// <param Name="logger"> 日志对象（为null则使用默认日志对象） </param>
        public static void LogMessage(object log, string who = null, LKLog logger = null) => (logger ?? LKLog.Instance).LogMessage(log, who);
        /// <summary> 记录警告信息到日志文件中 </summary>
        /// <param Name="log"> 警告信息 </param>
        /// <param Name="solution"> 对策方法 </param>
        /// <param Name="logger"> 日志对象（为null则使用默认日志对象） </param>
        public static void LogWarning(object log, Action solution = null, LKLog logger = null) => (logger ?? LKLog.Instance).LogWarning(log, solution);
        /// <summary> 记录异常信息到日志文件中 </summary>
        /// <param Name="log"> 异常信息 </param>
        /// <param Name="solution"> 对策方法 </param>
        /// <param Name="logger"> 日志对象（为null则使用默认日志对象） </param>
        public static void LogError(object log, Action solution = null, LKLog logger = null) => (logger ?? LKLog.Instance).LogError(log, solution);
        /// <summary> 记录异常信息到日志文件中 </summary>
        /// <param Name="e"> 异常类 </param>
        /// <param Name="solution"> 对策方法 </param>
        /// <param Name="logger"> 日志对象（为null则使用默认日志对象） </param>
        public static void LogError(Exception e, Action solution = null, LKLog logger = null) => (logger ?? LKLog.Instance).LogError(e, solution);
    }
}
