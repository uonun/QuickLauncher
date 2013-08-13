using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using EnvDTE80;

namespace udnz.com.VSExtensions
{
    public class DTE2Helper
    {
        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        /// <summary>
        /// Get a snapshot of the running object table (ROT).
        /// </summary>
        /// <returns>A hashtable mapping the name of the object
        //     in the ROT to the corresponding object</returns>
        private static Hashtable GetRunningObjectTable()
        {
            Hashtable result = new Hashtable();

            IntPtr numFetched = Marshal.AllocHGlobal(sizeof(int));

            IRunningObjectTable runningObjectTable;
            IEnumMoniker monikerEnumerator;
            IMoniker[] monikers = new IMoniker[1];

            GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();

            while (monikerEnumerator.Next(1, monikers, numFetched) == 0)
            {
                IBindCtx ctx;
                CreateBindCtx(0, out ctx);

                string runningObjectName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectName);

                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);

                result[runningObjectName] = runningObjectVal;
            }

            return result;
        }

        public static DTE2 GetCurrentIDE()
        {
            //http://msdn.microsoft.com/zh-cn/library/68shb4dw(v=vs.100).aspx
            //Another way to get an instance of the FIRST running Visual Studio IDE.
            //EnvDTE80.DTE2 dte2;
            //dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.
            //GetActiveObject("VisualStudio.DTE.10.0");

            var process = System.Diagnostics.Process.GetCurrentProcess();
            if (process != null)
                return GetByProcessID(process.Id);
            else
                return null;
        }

        public static DTE2 GetByProcessID(int processID)
        {
            DTE2 ide = null;

            Hashtable runningIDEInstances = new Hashtable();
            Hashtable runningObjects = DTE2Helper.GetRunningObjectTable();

            IDictionaryEnumerator rotEnumerator = runningObjects.GetEnumerator();
            while (rotEnumerator.MoveNext())
            {
                string candidateName = (string)rotEnumerator.Key;
                if (!candidateName.StartsWith("!VisualStudio.DTE"))
                {
                    continue;
                }

                DTE2 tmp = rotEnumerator.Value as DTE2;
                if (tmp == null)
                    continue;

                runningIDEInstances[candidateName] = tmp;

                if (candidateName.EndsWith(processID.ToString()))
                {
                    ide = tmp;
                    ide.InitOutputWindowPane();
                }
            }

            return ide;
        }
    }
}
