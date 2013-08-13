using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.Security.Permissions;
using EnvDTE;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace udnz.com.VSExtensions.QuickLauncher
{
    /// <summary>
    /// GetClassControl.xaml 的交互逻辑
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand)]
    public partial class QuickLauncherControl : UserControl
    {
        internal QuickLauncherPackage Package { get; set; }
        internal ToolWindowPane Pane { get; set; }

        private System.Threading.SynchronizationContext _sc = System.Windows.Forms.WindowsFormsSynchronizationContext.Current;

        private DTE2 IDE
        {
            get
            {
                if (_ide == null)
                {
                    _ide = DTE2Helper.GetCurrentIDE();
                }

                if (_ide == null)
                {
                    throw new Exception("Can not get the instance of current IDE.");
                }

                return _ide;
            }
        }
        private DTE2 _ide = null;

        public QuickLauncherControl()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            tbKeyLabel.Text = Core.Resources.ResourceManager.GetString("KeyLabel");
            tbKey.ToolTip = Core.Resources.ResourceManager.GetString("KeyToolTip");
            ((GridView)lvList.View).Columns[0].Header = Core.Resources.ResourceManager.GetString("ColumnHead_Name");
            ((GridView)lvList.View).Columns[1].Header = Core.Resources.ResourceManager.GetString("ColumnHead_Path");
            cbAutoClose.ToolTip = Core.Resources.ResourceManager.GetString("CloseWindowAfterFileOpening");
            ShowInfo(Core.Resources.ResourceManager.GetString("Ready"));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            ReadSettings();
        }

        private void SeekAll(ProjectItem i, string key)
        {
            if (i.ProjectItems != null && i.ProjectItems.Count > 0)
            {
                foreach (ProjectItem item in i.ProjectItems)
                {
                    SeekAll(item, key);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(i.Name))
                {
                    bool isOk = false;
                    isOk = i.Name.ToLower().Contains(key);
                    if (i.Name.ToLower().Contains(key))
                    {
                        for (short n = 1;n <= i.FileCount;n++)
                        {
                            this.lvList.Items.Add(new ViewItem() { Item = i, Name = i.Name, FullName = i.FileNames[n] });
                        }
                    }
                }
            }
        }

        private void OpenFile()
        {
            if (this.lvList.SelectedIndex < 0)
                return;

            ViewItem item = (ViewItem)this.lvList.SelectedItem;
            if (item != null)
            {
                try
                {
                    var w = item.Item.Open(EnvDTE.Constants.vsViewKindPrimary);
                    w.Activate();
                    ShowInfo(string.Format(Core.Resources.ResourceManager.GetString("FileOpened"), item.Name), Colors.DarkGreen);

                    if (cbAutoClose.IsChecked ?? false)
                    {
                        this.Close();
                    }

                    IDE.ShowOutputMessage(string.Format(Core.Resources.ResourceManager.GetString("FileOpened"), item.FullName));
                }
                catch (Exception)
                {
                    ShowInfo(string.Format(Core.Resources.ResourceManager.GetString("CanNotOpen"), item.FullName), Colors.Red);
                    IDE.ShowOutputMessage(string.Format("Open file FAIL: {0}", item.FullName));
                }
            }
        }

        private void ShowInfo(string msg)
        {
            lbInfo.Content = msg;
        }

        private void ShowInfo(string msg, Color color)
        {
            ShowInfo(msg);

            // Create and animate a Brush to set the button's Background.
            SolidColorBrush myBrush = new SolidColorBrush();
            myBrush.Color = Colors.Black;
            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Colors.Black;
            myColorAnimation.To = color;
            myColorAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            myColorAnimation.AutoReverse = true;
            myColorAnimation.RepeatBehavior = new RepeatBehavior(3);

            // Apply the animation to the brush's Color property.
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);

            lbInfo.Foreground = myBrush;
        }

        private void Close()
        {
            ToolWindowPane window = this.Pane;
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            windowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
        }

        private void Control_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void tbKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lvList.HasItems) this.lvList.Items.Clear();

            if (IDE != null)
            {
                if (IDE.Solution.IsOpen)
                {
                    if (IDE.Solution.Projects != null && IDE.Solution.Projects.Count > 0)
                    {
                        _sc.Post((x) =>
                        {
                            string key = tbKey.Text;
                            if (!string.IsNullOrEmpty(key))
                            {
                                foreach (Project p in IDE.Solution.Projects)
                                {
                                    if (p.ProjectItems != null && p.ProjectItems.Count > 0)
                                    {
                                        foreach (ProjectItem item in p.ProjectItems)
                                        {
                                            SeekAll(item, key);
                                        }
                                    }
                                }
                            }

                            ShowInfo(string.Format(Core.Resources.ResourceManager.GetString("NItemsFound"), lvList.Items.Count));
                        }, e);
                    }
                    else
                    {
                        ShowInfo(Core.Resources.ResourceManager.GetString("NothingFound"));
                    }
                }
                else
                {
                    ShowInfo(Core.Resources.ResourceManager.GetString("PleaseOpenSolution"), Colors.Red);
                }
            }
        }

        private void tbKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbKey.Text))
            {
                if (e.Key == Key.Down || e.Key == Key.Tab || e.Key == Key.Enter || e.Key == Key.Return)
                {
                    this.lvList.Focus();
                    if (this.lvList.HasItems)
                    {
                        if (this.lvList.SelectedIndex == -1)
                        {
                            this.lvList.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        private void lvList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                this.tbKey.Focus();
            }
            else if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                OpenFile();
            }
        }

        private void lvList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFile();
        }

        private void cbAutoClose_Checked(object sender, RoutedEventArgs e)
        {
            WriteSettings();
        }

        #region [- settings -]

        private const string collectionPath = "udnz.com\\VSExtensions\\QuickLauncher";
        private const string propertyName = "CloseAfterOpen";
        private SettingsManager _settingsManager;
        private void ReadSettings()
        {
            if (_settingsManager == null)
                _settingsManager = new ShellSettingsManager(this.Package);

            var configurationSettingsStore = _settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
            bool result = configurationSettingsStore.GetBoolean(collectionPath, propertyName, false);
            cbAutoClose.IsChecked = result;
        }

        private void WriteSettings()
        {
            if (_settingsManager == null)
                _settingsManager = new ShellSettingsManager(this.Package);

            WritableSettingsStore configurationSettingsStore = _settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (!configurationSettingsStore.CollectionExists(collectionPath))
            {
                configurationSettingsStore.CreateCollection(collectionPath);
            }
            bool result = this.cbAutoClose.IsChecked ?? false;
            configurationSettingsStore.SetBoolean(collectionPath, propertyName, result);
        }

        #endregion
    }

    internal class ViewItem
    {
        public ProjectItem Item { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
