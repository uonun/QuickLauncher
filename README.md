Overview
=============

QuickLauncher is a Visual Studio 2012/2010 extension that helps you find and open a specified file in solution/project/opened files easyly and quickly. Like "Ctrl+," or "Ctrl+;".

Details
=============

What does it looks like?

![Visual Studio 2012](http://work.udnz.com/VSExtension/QuickLauncher/Images/vs2012.jpg)

![Visual Studio 2010](http://work.udnz.com/VSExtension/QuickLauncher/Images/vs2010.jpg)


How to use it?
=============

After you install it, there will be a button on the menu: 
Main menu -> Tools, just click it and there will be a tool pane displayed.

![Main menu -> Tools](http://work.udnz.com/VSExtension/QuickLauncher/Images/how-to-use-it.jpg)

### It is very easy and convenient: 

* Input the keyword then press Enter/Return/Down/Tab to focus on the result list
* Press Up/Down to choose the file you want to open
* Press Enter or double-click 

So, you see, Input -> Down -> Down -> Enter, the file is open! It is amazing and very convenient!


### Tips: 
> - You can bind a shortcut key in Options of Visual Studio.
> - The popup window chould be docked on any side as same as other tool panes, have a nice try.

Download &amp; Install
=============

> - Link 1: http://work.udnz.com/Download/QuickLauncher.rar
> - Link 2: http://visualstudiogallery.msdn.microsoft.com/438f1a30-8572-4964-9d18-5bd862e2b094?SRC

### If you installed successful, you can find it here

Main menu -> Tools -> Extension Manager (Ok, actually, you can uninstall it here as well.) 

![ExtManager](http://work.udnz.com/VSExtension/QuickLauncher/Images/ExtManager.jpg)

Main menu -> Help -> About

![about-window](http://work.udnz.com/VSExtension/QuickLauncher/Images/about-window.jpg)

Other
==============

```C#
namespace udnz.com.VSExtensions
{
    public class Consts
    {
        public const string FrameworkName = "Visual Studio Extentions Framework";
        public const string Author = "uonun";
        public const string Homepage = "http://work.udnz.com/";
        public static readonly Version Version = new Version(1, 0, 0, 0);
    }
}
```

See also: [http://work.udnz.com](http://work.udnz.com/VSExtension/QuickLauncher/)

