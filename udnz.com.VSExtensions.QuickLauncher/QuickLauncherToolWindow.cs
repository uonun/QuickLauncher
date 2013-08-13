using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using udnz.com.VSExtensions.QuickLauncher.Core;

namespace udnz.com.VSExtensions.QuickLauncher
{
    public class QuickLauncherToolWindow : ToolWindowPane
    {
        public QuickLauncherToolWindow()
        {
            //// Set the window title reading it from the resources.
            this.Caption = Resources.ResourceManager.GetString("ToolWindowTitle");
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 0; // 0-based.

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            QuickLauncherControl c = new QuickLauncherControl();
            base.Content = c;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            QuickLauncherControl c = (QuickLauncherControl)base.Content;
            c.Package = (QuickLauncherPackage)this.Package;
            c.Pane = this;            
        }
    }
}
