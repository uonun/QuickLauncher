using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using EnvDTE;

namespace udnz.com.VSExtensions
{
    public static class DTE2Utils
    {
        /// <summary>
        /// 输出窗口
        /// </summary>
        private readonly static Dictionary<DTE2, OutputWindowPane> pans = new Dictionary<DTE2, OutputWindowPane>();
        public static void ShowOutputMessage(this DTE2 ide, string msg, string windowName = Consts.FrameworkName)
        {
            if (!pans.ContainsKey(ide) || pans[ide] == null)
            {
                InitOutputWindowPane(ide, windowName);
            }

            if (pans.ContainsKey(ide) && pans[ide] != null)
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    pans[ide].OutputString(string.Format("{0:yyyy-MM-dd HH:mm:ss fff} > {1}\r\n", DateTime.Now, msg));
                }
                else
                {
                    pans[ide].OutputString("\r\n");
                }
            }
        }

        public static void InitOutputWindowPane(this DTE2 ide, string windowName = Consts.FrameworkName)
        {
            if (!pans.ContainsKey(ide) || pans[ide] == null)
            {
                try
                {
                    bool found = false;
                    foreach (OutputWindowPane item in ide.ToolWindows.OutputWindow.OutputWindowPanes)
                    {
                        if (item.Name == windowName)
                        {
                            pans[ide] = item;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        pans[ide] = ide.ToolWindows.OutputWindow.OutputWindowPanes.Add(windowName);
                        pans[ide].OutputString("===============================================================\r\n");
                        pans[ide].OutputString(string.Format("=\t{0} v{1} loaded.\r\n", Consts.FrameworkName, Consts.Version));
                        pans[ide].OutputString("===============================================================\r\n");
                        pans[ide].OutputString(string.Format("=\tAuthor:\t{0}, {1}\r\n", Consts.AuthorMail, Consts.Homepage));
                        pans[ide].OutputString(string.Format("=\tSystem:\t{0}\r\n", Environment.OSVersion.VersionString));
                        pans[ide].OutputString(string.Format("=\t  .Net:\t{0}\r\n", Environment.Version));
                        pans[ide].OutputString(string.Format("=\t   IDE:\t{0}\r\n", ide.ToStringExt()));
                        pans[ide].OutputString("===============================================================\r\n");
                    }
                }
                catch (Exception ex)
                {
                    // do nothing
                    return;
                }
            }
        }

        public static string ToStringExt(this DTE2 ide)
        {
            return string.Format(string.Format("{0} {1} v{2}", ide.Name, ide.Edition, ide.Version));
        }
    }
}
