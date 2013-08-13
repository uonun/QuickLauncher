// Guids.cs
// MUST match guids.h
using System;

namespace udnz.com.VSExtensions.QuickLauncher
{
    static class GuidList
    {
        public const string guidQuickLauncherPkgString = "85384cd7-23bd-4ac9-8eca-473d5303b089";
        public const string guidQuickLauncherCmdSetString = "cb906bda-c57a-4aea-8789-66e12ca6e742";

        public static readonly Guid guidQuickLauncherCmdSet = new Guid(guidQuickLauncherCmdSetString);
    };
}