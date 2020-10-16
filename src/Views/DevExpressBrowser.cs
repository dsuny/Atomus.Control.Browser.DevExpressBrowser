using Atomus.Attribute;
using Atomus.Control.Browser.Controllers;
using Atomus.Control.Browser.Models;
using Atomus.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Deployment.Application;
using System.Web;

namespace Atomus.Control.Browser
{
    public partial class DevExpressBrowser : DevExpress.XtraBars.Ribbon.RibbonForm, IAction
    {
        private IAction loginControl;
        private IAction joinControl;
        private IAction menuControl;

        private AtomusControlEventHandler beforeActionEventHandler;
        private AtomusControlEventHandler afterActionEventHandler;

        private DevExpress.XtraBars.Docking2010.DocumentManager documentManager1;
        private DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView tabbedView1;

        string[] items;
        private List<BarButtonItem> buttonList;

        //BarButtonItem barButtonItem8;

        #region Init
        public DevExpressBrowser()
        {
            string skinName;
            Color color;

            //DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.Skins.SkinManager.EnableMdiFormSkins();

            InitializeComponent();

            ControlLocalizer.InitLocalize();//DevExpress 언어 설정

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.IsMdiContainer = false;
            this.ShowIcon = false;
            this.ControlBox = false;
            this.ShowInTaskbar = false;
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            this.dockManager1.DockingOptions.HideImmediatelyOnAutoHide = true;
            this.dockManager1.AutoHideSpeed = 1000;

            try
            {
                this.ribbon.RibbonStyle = (DevExpress.XtraBars.Ribbon.RibbonControlStyle)Enum.Parse(typeof(DevExpress.XtraBars.Ribbon.RibbonControlStyle), this.GetAttribute("RibbonControlStyle"));
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
                this.ribbon.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Default;
            }

            skinName = this.GetAttribute("SkinName");

            if (skinName != null)
            {
                Config.Client.SetAttribute("SkinName", skinName);

                color = this.GetAttributeColor(skinName + ".BackColor");
                if (color != null)
                    this.BackColor = color;

                color = this.GetAttributeColor(skinName + ".ForeColor");
                if (color != null)
                    this.ForeColor = color;
            }

            DevExpress.LookAndFeel.UserLookAndFeel.Default.StyleChanged += Default_StyleChanged;

            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;

            this.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Hidden;
            this.ribbonStatusBar.Visible = false;
            this.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Hidden;

            this.FormClosing += new FormClosingEventHandler(this.DefaultBrowser_FormClosing);

            this.buttonList = new List<BarButtonItem>();

            try
            {
                //This set the style to use skin technology
                DevExpress.LookAndFeel.UserLookAndFeel.Default.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;

                if (Properties.Settings.Default.SkinName.IsNullOrEmpty())
                {
                    //Here we specify the skin to use by its name           
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(this.GetAttribute("DevExpressSkinName"));
                }
                else
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(Properties.Settings.Default.SkinName);
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
            }

            try
            {
                if (!this.GetAttribute("Font.FamilyName").IsNullOrEmpty())
                {
                    if (this.IsFontInstalled(this.GetAttribute("Font.FamilyName")))
                        if (!this.GetAttribute("Font.EmSize").IsNullOrEmpty())
                            DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = new Font(this.GetAttribute("Font.FamilyName"), (float)this.GetAttributeDecimal("Font.EmSize"));
                        else
                            DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = new Font(this.GetAttribute("Font.FamilyName"), DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont.Size);
                }
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
            }

            this.GetActivationUri();
        }

        private void Default_StyleChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SkinName = DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName;
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Dictionary
        #endregion

        #region Spread 
        #endregion

        #region IO 
        object IAction.ControlAction(ICore sender, AtomusControlArgs e)
        {
            System.Windows.Forms.Control[] controls;
            List<BarButtonItem> listRemoveButton;
            string[] toolbarItems;

            try
            {
                if (e.Action != "UserToolbarButton.Add" & e.Action != "UserToolbarButton.Remove")
                    this.beforeActionEventHandler?.Invoke(this, e);

                //this.ribbon.Pages["System"].Groups["Action"]

                switch (e.Action)
                {
                    case "UserToolbarButton.Remove":
                        listRemoveButton = new List<BarButtonItem>();

                        try
                        {
                            //기본 버튼 다음부터; 마지막 버튼까지
                            for (int i = items.Length; i < this.buttonList.Count; i++)
                            {
                                //Global 버튼이 아니면 제거
                                if ((this.buttonList[i].Tag as ICore).GetAttribute(string.Format("{0}.Global", this.buttonList[i].Name)) == null
                                    || (this.buttonList[i].Tag as ICore).GetAttribute(string.Format("{0}.Global", this.buttonList[i].Name)) == "N")
                                {
                                    this.ribbon.Pages["System"].Groups["Action"].ItemLinks.Remove(this.buttonList[i]);
                                    this.RemoveButton(this.buttonList[i]);
                                    listRemoveButton.Add(this.buttonList[i]);
                                }

                                //this.RemoveImageList(sender, this._ButtonList[i].Name);
                            }

                            foreach (BarButtonItem button in listRemoveButton)
                            {
                                this.buttonList.Remove(button);
                            }
                            return true;
                        }
                        catch (Exception ex)
                        {
                            DiagnosticsTool.MyTrace(ex);
                        }

                        return true;

                    case "UserToolbarButton.Add":
                        listRemoveButton = (List<BarButtonItem>)Config.Client.GetAttribute(sender, "ToolbarButtons");

                        if (listRemoveButton != null)//기존에 등록되어 있으면
                        {
                            foreach (BarButtonItem button in listRemoveButton)
                            {
                                //Global 버튼이 아니면
                                if (sender.GetAttribute(string.Format("{0}.Global", button.Name)) == null || sender.GetAttribute(string.Format("{0}.Global", button.Name)) == "N")
                                {
                                    this.ribbon.Pages["System"].Groups["Action"].ItemLinks.Add(button);
                                    this.AddButton(sender, button);
                                }
                            }
                        }
                        else
                        {
                            //toolbarItems = ((string)Config.Client.GetAttribute(sender, "ToolbarButtonItems"))?.Split(',');
                            toolbarItems = sender.GetAttribute("ToolbarButtonItems")?.Split(',');

                            if (toolbarItems != null)
                            {
                                for (int i = 0; i < toolbarItems.Length; i++)
                                {
                                    if (toolbarItems[i] != null && toolbarItems[i] != "")
                                        this.AddButton(sender, toolbarItems[i]);
                                }
                            }
                        }

                        return true;

                    default:
                        controls = this.Controls.Find(e.Action.Split('.')[1], true);

                        if (controls.Length == 1)
                        {
                            controls[0].Enabled = e.Value.Equals("Y");
                            return true;
                        }
                        else
                            return false;
                }
            }
            finally
            {
                if (e.Action != "UserToolbarButton.Add" & e.Action != "UserToolbarButton.Remove")
                    this.afterActionEventHandler?.Invoke(this, e);
            }
        }
        private void RemoveButton(BarButtonItem button)
        {
            button.Tag = null;
            button.ItemClick -= this.BarButtonItem_ItemClick;
        }

        private void AddButton(ICore core, string name)
        {
            List<BarButtonItem> listRemoveButton;
            BarButtonItem button;

            button = new BarButtonItem();

            try
            {
                listRemoveButton = ((List<BarButtonItem>)Config.Client.GetAttribute(core, "ToolbarButtons"));

                if (listRemoveButton == null)//기존에 등록되어 있으면
                {
                    listRemoveButton = new List<BarButtonItem>();

                    Config.Client.SetAttribute(core, "ToolbarButtons", listRemoveButton);
                }

                listRemoveButton.Add(button);

                button.Name = name;
                button.Tag = name;
                if (this.GetAttributeBool("TextVisible"))
                    button.Caption = core.GetAttribute(string.Format("{0}.{1}", name, "Text"));

                try
                {
                    button.Alignment = (BarItemLinkAlignment)Enum.Parse(typeof(BarItemLinkAlignment), this.GetAttribute("TextAlign"));
                }
                catch (Exception exception)
                {
                    DiagnosticsTool.MyTrace(exception);
                }

                button.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
                button.Caption = core.GetAttribute(string.Format("{0}.Text", name));
                this.GetAttributeWebImageAsync(core, button.ImageOptions, name);
                //barButtonItem.ImageOptions.Image = this.GetAttributeWebImageAsync(sender, string.Format("{0}.Image", toolbarItems[i])).Result;
                //barButtonItem.ImageOptions.LargeImage = this.GetAttributeWebImageAsync(sender, string.Format("{0}.ImageOn", toolbarItems[i])).Result;
                button.Name = name;
                //button.Tag = name;
                //button.Tag = core;

                this.ribbon.Items.Add(button);
                this.ribbon.Pages["System"].Groups["Action"].ItemLinks.Add(button, true);

                //this.AddImageList(core, button);
                this.AddButton(core, button);
            }
            catch (Exception exception)
            {
                DiagnosticsTool.MyTrace(exception);
            }
        }
        private void AddButton(ICore core, BarButtonItem button)
        {
            if (!this.buttonList.Contains(button))
                this.buttonList.Add(button);

            button.Tag = core;
            button.ItemClick -= this.BarButtonItem_ItemClick;
            button.ItemClick += this.BarButtonItem_ItemClick;
        }
        private async void GetAttributeWebImageAsync(ICore core, BarItemImageOptions barItemImageOptions, string attributeName)
        {
            barItemImageOptions.Image = await core.GetAttributeWebImage(string.Format("{0}.Image", attributeName));
            barItemImageOptions.LargeImage = await core.GetAttributeWebImage(string.Format("{0}.ImageOn", attributeName));
        }

        private void LoginControl_BeforeActionEventHandler(ICore sender, AtomusControlEventArgs e) { }
        private async void LoginControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            UserControl userControl;
            string noticeString;

            try
            {
                switch (e.Action)
                {
                    case "Form.Size":
                        this.Size = (Size)e.Value;
                        break;

                    case "Login.Ok":
                        this.Opacity = 0;
                        this.Controls.Remove((System.Windows.Forms.Control)this.loginControl);

                        this.ControlBox = true;
                        this.ShowInTaskbar = true;
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                        this.StartPosition = FormStartPosition.CenterScreen;
                        this.TopMost = false;
                        this.MinimizeBox = true;
                        this.MaximizeBox = true;

                        this.SetMdiContainer();

                        this.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Auto;
                        this.ribbonStatusBar.Visible = true;
                        this.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Visible;

                        try
                        {
                            this.Icon = await this.GetAttributeWebIcon("Icon");
                            if (this.Icon != null)
                                this.ShowIcon = true;
                        }
                        catch (Exception _Exception)
                        {
                            DiagnosticsTool.MyTrace(_Exception);
                        }

                        this.WindowState = FormWindowState.Maximized;

                        if (await this.SetRibbonPages())
                        {
                            this.SetRibbonStatusBar();
                            this.SetBrowserViewer();
                        }

                        try
                        {
                            noticeString = this.GetAttribute("NoticeString");

                            if (noticeString != null && noticeString != "")
                                this.AddNotice(this, noticeString);
                        }
                        catch (Exception exception)
                        {
                            DiagnosticsTool.MyTrace(exception);
                        }

                        this.dockManager1.ActivePanelChanged += DockManager1_ActivePanelChanged;

                        this.Opacity = 1;
                        break;

                    case "Login.Fail":
                        break;

                    case "Login.Cancel":
                        this.Close();
                        break;

                    case "Login.JoinNew":
                        if (this.joinControl == null)
                        {
                            this.joinControl = (IAction)this.CreateInstance("JoinControl");
                            //this.joinControl = new Join.DevExpressJoin();

                            this.joinControl.BeforeActionEventHandler += JoinControl_BeforeActionEventHandler;
                            this.joinControl.AfterActionEventHandler += JoinControl_AfterActionEventHandler;

                            userControl = (UserControl)this.joinControl;
                            userControl.Dock = DockStyle.Fill;

                            this.Controls.Add((UserControl)this.joinControl);
                        }

                        userControl = (UserControl)this.joinControl;
                        userControl.BringToFront();
                        break;

                    default:
                        throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }
        private void JoinControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e) { }
        private void JoinControl_BeforeActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            try
            {
                switch (e.Action)
                {
                    case "Form.Size":
                        this.Size = (Size)e.Value;
                        break;

                    case "Join.Start":
                        break;

                    case "Join.Ok":
                        break;

                    case "Join.Cancel":
                        this.SetLoginControl();
                        break;

                    case "PasswordChange.Start":
                        break;

                    case "PasswordChange.Ok":

                    default:
                        throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }

        private void MenuControl_BeforeActionEventHandler(ICore sender, AtomusControlEventArgs e) { }
        private void MenuControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            object[] objects;
            ICore core;
            string MENU_ID;

            try
            {
                switch (e.Action)
                {
                    case "Menu.OpenControl":
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, _VisibleOne

                        if ((bool)objects[2])//_VisibleOne
                        {
                            foreach (DevExpress.XtraBars.Docking.DockPanel _TabPage in this.dockManager1.Panels)
                            {
                                if (_TabPage.Tag == null)
                                    continue;

                                core = (ICore)_TabPage.Tag;


                                MENU_ID = core.GetAttribute("MENU_ID");

                                if (MENU_ID != null)
                                    if (MENU_ID.Equals(objects[0].ToString()))
                                    {
                                        this.Tag = _TabPage.Tag;

                                        if (_TabPage.DockedAsTabbedDocument)
                                            this.tabbedView1.ActivateDocument(_TabPage);
                                        else
                                            this.dockManager1.ActivePanel = _TabPage;

                                        return;//기존 화면이 있으니 바로 빠져 나감
                                    }
                            }
                        }

                        e.Value = this.OpenControl((decimal)objects[0], (decimal)objects[1], sender, null, true);

                        break;

                    case "Menu.GetControl":
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, AtomusControlEventArgs, addTabControl

                        e.Value = this.OpenControl((decimal)objects[0], (decimal)objects[1], sender, (AtomusControlEventArgs)objects[2], (bool)objects[3]);

                        break;

                    //case "Menu.Atomus.Control.AtomusManagement.ComposeServiceAssemblies":
                    //    _Core = Factory.CreateInstance(System.IO.File.ReadAllBytes(@"C:\Work\Project\Atomus\개발\Control\AtomusManagement\ComposeServiceAssemblies\bin\Debug\Atomus.Control.AtomusManagement.ComposeServiceAssemblies.V1.0.0.0.dll"), "Atomus.Control.AtomusManagement.ComposeServiceAssemblies", false, false);
                    //    this.OpenControl("ComposeServiceAssemblies", "ComposeServiceAssemblies", (UserControl)_Core);
                    //    break;

                    case "ApplicationExit":
                        this.ApplicationExit();
                        break;

                    //default:
                    //    throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }

        private void HomeControl_BeforeActionEventHandler(ICore sender, AtomusControlEventArgs e) { }
        private void HomeControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e) { }

        private void UserControl_BeforeActionEventHandler(ICore sender, AtomusControlEventArgs e) { }
        private void UserControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            object[] objects;
            Service.IResponse response;
            string tmp;

            try
            {
                switch (e.Action)
                {
                    case "UserControl.OpenControl":
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, sender, AtomusControlArgs

                        e.Value = this.OpenControl((decimal)objects[0], (decimal)objects[1], sender, (AtomusControlEventArgs)objects[2], true);

                        break;
                    case "UserControl.GetControl":
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, sender, AtomusControlArgs

                        e.Value = this.OpenControl((decimal)objects[0], (decimal)objects[1], sender, (AtomusControlEventArgs)objects[2], false);

                        break;

                    case "UserControl.AssemblyVersionCheck":
                        tmp = this.GetAttribute("ProcedureAssemblyVersionCheck");

                        if (tmp != null && tmp.Trim() != "")
                        {
                            response = this.AssemblyVersionCheck(sender);

                            if (response.Status != Service.Status.OK)
                            {
                                this.MessageBoxShow(this, response.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                e.Value = false;
                            }
                            else
                                e.Value = true;
                        }
                        else
                            e.Value = true;

                        break;


                    case "UserControl.Status":
                        objects = (object[])e.Value;//StatusBarInfomation1  Text

                        this.ribbon.Items[string.Format("RibbonStatusBar_{0}", objects[0])].Caption = (string)objects[1];

                        break;

                        //default:
                        //    throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }


        private async Task<ICore> OpenControlAsync(decimal _MENU_ID, decimal _ASSEMBLY_ID, ICore sender, AtomusControlEventArgs _AtomusControlEventArgs, bool addTabControl)
        {
            Service.IResponse _Result;
            IAction _Core;

            try
            {
                _Result = await this.SearchOpenControlAsync(new DevExpressBrowserSearchModel()
                {
                    MENU_ID = _MENU_ID,
                    ASSEMBLY_ID = _ASSEMBLY_ID
                });

                if (_Result.Status == Service.Status.OK)
                {
                    if (_Result.DataSet.Tables.Count == 2)
                        if (_Result.DataSet.Tables[0].Rows.Count == 1)
                        {
                            if (_Result.DataSet.Tables[0].Columns.Contains("FILE_TEXT") && _Result.DataSet.Tables[0].Rows[0]["FILE_TEXT"] != DBNull.Value)
                                _Core = (IAction)Factory.CreateInstance(Convert.FromBase64String((string)_Result.DataSet.Tables[0].Rows[0]["FILE_TEXT"]), _Result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);
                            else
                                _Core = (IAction)Factory.CreateInstance((byte[])_Result.DataSet.Tables[0].Rows[0]["FILE"], _Result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);
                            //_Core = new Atomus.Management.Control.Assemblies();

                            _Core.BeforeActionEventHandler += UserControl_BeforeActionEventHandler;
                            _Core.AfterActionEventHandler += UserControl_AfterActionEventHandler;

                            _Core.SetAttribute("MENU_ID", _MENU_ID.ToString());
                            _Core.SetAttribute("ASSEMBLY_ID", _ASSEMBLY_ID.ToString());

                            foreach (DataRow _DataRow in _Result.DataSet.Tables[1].Rows)
                            {
                                _Core.SetAttribute(_DataRow["ATTRIBUTE_NAME"].ToString(), _DataRow["ATTRIBUTE_VALUE"].ToString());
                            }

                            if (addTabControl)
                                this.OpenControl((_Result.DataSet.Tables[0].Rows[0]["NAME"] as string).Translate(), string.Format("{0} {1}", (_Result.DataSet.Tables[0].Rows[0]["DESCRIPTION"] as string).Translate(), _Core.GetType().Assembly.GetName().Version.ToString()), (UserControl)_Core);

                            if (_AtomusControlEventArgs != null)
                                _Core.ControlAction(sender, _AtomusControlEventArgs.Action, _AtomusControlEventArgs.Value);

                            return _Core;
                        }
                }
                else
                {
                    this.MessageBoxShow(this, _Result.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                return null;
            }
            catch (Exception _Exception)
            {
                this.MessageBoxShow(this, _Exception);
                return null;
            }
            finally
            {
            }
        }
        private ICore OpenControl(decimal _MENU_ID, decimal _ASSEMBLY_ID, ICore sender, AtomusControlEventArgs _AtomusControlEventArgs, bool addTabControl)
        {
            Service.IResponse _Result;
            IAction _Core;

            try
            {
                _Result = this.SearchOpenControl(new DevExpressBrowserSearchModel()
                {
                    MENU_ID = _MENU_ID,
                    ASSEMBLY_ID = _ASSEMBLY_ID
                });

                if (_Result.Status == Service.Status.OK)
                {
                    if (_Result.DataSet.Tables.Count == 2)
                        if (_Result.DataSet.Tables[0].Rows.Count == 1)
                        {
                            if (_Result.DataSet.Tables[0].Columns.Contains("FILE_TEXT") && _Result.DataSet.Tables[0].Rows[0]["FILE_TEXT"] != DBNull.Value)
                                _Core = (IAction)Factory.CreateInstance(Convert.FromBase64String((string)_Result.DataSet.Tables[0].Rows[0]["FILE_TEXT"]), _Result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);
                            else
                                _Core = (IAction)Factory.CreateInstance((byte[])_Result.DataSet.Tables[0].Rows[0]["FILE"], _Result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);

                            //_Core = (IAction)Factory.CreateInstance((byte[])_Result.DataSet.Tables[0].Rows[0]["FILE"], _Result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);
                            _Core.BeforeActionEventHandler += UserControl_BeforeActionEventHandler;
                            _Core.AfterActionEventHandler += UserControl_AfterActionEventHandler;

                            _Core.SetAttribute("MENU_ID", _MENU_ID.ToString());
                            _Core.SetAttribute("ASSEMBLY_ID", _ASSEMBLY_ID.ToString());

                            foreach (DataRow _DataRow in _Result.DataSet.Tables[1].Rows)
                            {
                                _Core.SetAttribute(_DataRow["ATTRIBUTE_NAME"].ToString(), _DataRow["ATTRIBUTE_VALUE"].ToString());
                            }

                            if (addTabControl)
                                this.OpenControl((_Result.DataSet.Tables[0].Rows[0]["NAME"] as string).Translate(), string.Format("{0} {1}", (_Result.DataSet.Tables[0].Rows[0]["DESCRIPTION"] as string).Translate(), _Core.GetType().Assembly.GetName().Version.ToString()), (UserControl)_Core);

                            if (_AtomusControlEventArgs != null)
                                _Core.ControlAction(sender, _AtomusControlEventArgs.Action, _AtomusControlEventArgs.Value);

                            return _Core;
                        }
                }
                else
                {
                    this.MessageBoxShow(this, _Result.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                return null;
            }
            catch (Exception _Exception)
            {
                this.MessageBoxShow(this, _Exception);
                return null;
            }
            finally
            {
            }
        }
        private void OpenControl(string name, string description, UserControl userControl)
        {
            DevExpress.XtraBars.Docking.DockPanel temp;

            try
            {
                userControl.Dock = DockStyle.Fill;

                temp = this.dockManager1.AddPanel(new Point(0,0));
                temp.DockedAsTabbedDocument = true;

                temp.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;

                temp.BackColor = Color.Transparent;
                temp.Text = name;
                temp.Hint = description;
                temp.Tag = userControl;

                temp.ClosingPanel += ClosingPanel;

                this.Tag = userControl;

                temp.Controls.Add(userControl);

                this.dockManager1.ActivePanel = temp;
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }

        private void DockManager1_ActivePanelChanged(object sender, DevExpress.XtraBars.Docking.ActivePanelChangedEventArgs e)
        {
            ICore core;
            string value;

            try
            {
                if (e.Panel != null)
                {
                    if (e.Panel.Controls[0].Controls.Count > 0)
                    {
                        this.Tag = e.Panel.Controls[0].Controls[0];

                        this.ControlAction((ICore)this.Tag, "UserToolbarButton.Remove", null);

                        core = (ICore)this.Tag;

                        value = core.GetAttribute("Action.New");
                        this.ribbon.Items["New"].Enabled = (value != null && value == "Y");
                        value = core.GetAttribute("Action.Search");
                        this.ribbon.Items["Search"].Enabled = (value != null && value == "Y");
                        value = core.GetAttribute("Action.Save");
                        this.ribbon.Items["Save"].Enabled = (value != null && value == "Y");
                        value = core.GetAttribute("Action.Delete");
                        this.ribbon.Items["Delete"].Enabled = (value != null && value == "Y");
                        value = core.GetAttribute("Action.Print");
                        this.ribbon.Items["Print"].Enabled = (value != null && value == "Y");

                        if (e.Panel.Hint != "")
                        {
                            this.ribbon.SelectedPage = this.ribbon.Pages["System"];
                            this.ControlAction(core, "UserToolbarButton.Add", null);//각 화면에서만 사용하는 버튼 활성화
                        }
                    }
                }
                else
                    if (e.OldPanel != null) this.TabbedView1_DocumentActivated(sender, new DevExpress.XtraBars.Docking2010.Views.DocumentEventArgs(this.tabbedView1.ActiveDocument));
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
            }
        }
        private void TabbedView1_DocumentActivated(object sender, DevExpress.XtraBars.Docking2010.Views.DocumentEventArgs e)
        {
            ICore core;
            string value;

            if (e.Document == null) return;

            if (e.Document.Control.Controls[0].Controls.Count > 0)
            {
                this.Tag = e.Document.Control.Controls[0].Controls[0];

                this.ControlAction((ICore)this.Tag, "UserToolbarButton.Remove", null);

                core = (ICore)this.Tag;

                try
                {
                    value = core.GetAttribute("Action.New");
                    this.ribbon.Items["New"].Enabled = (value != null && value == "Y");
                    value = core.GetAttribute("Action.Search");
                    this.ribbon.Items["Search"].Enabled = (value != null && value == "Y");
                    value = core.GetAttribute("Action.Save");
                    this.ribbon.Items["Save"].Enabled = (value != null && value == "Y");
                    value = core.GetAttribute("Action.Delete");
                    this.ribbon.Items["Delete"].Enabled = (value != null && value == "Y");
                    value = core.GetAttribute("Action.Print");
                    this.ribbon.Items["Print"].Enabled = (value != null && value == "Y");

                    if (((DevExpress.XtraBars.Docking.DockPanel)e.Document.Control).Hint != "")
                    {
                        this.ribbon.SelectedPage = this.ribbon.Pages["System"];
                        this.ControlAction(core, "UserToolbarButton.Add", null);//각 화면에서만 사용하는 버튼 활성화
                    }
                }
                catch (Exception ex)
                {
                    DiagnosticsTool.MyTrace(ex);
                }
            }
            else
                this.DockManager1_ActivePanelChanged(sender, new DevExpress.XtraBars.Docking.ActivePanelChangedEventArgs(this.dockManager1.ActivePanel, null));
        }

        private void ClosingPanel(object sender, DevExpress.XtraBars.Docking.DockPanelCancelEventArgs e)
        {
            IAction action;

            if (((DevExpress.XtraBars.Docking.DockPanel)sender).Controls.Count > 0 && ((DevExpress.XtraBars.Docking.DockPanel)sender).Controls[0].Controls.Count > 0 && ((DevExpress.XtraBars.Docking.DockPanel)sender).Controls[0].Controls[0] is IAction)
            {
                action = (IAction)((DevExpress.XtraBars.Docking.DockPanel)sender).Controls[0].Controls[0];

                if (action.GetAttribute("AllowCloseAction") != null && action.GetAttribute("AllowCloseAction") == "Y")
                    try
                    {
                        if (!(bool)action.ControlAction(this, new AtomusControlArgs("Close", null)))
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    catch (Exception exception)
                    {
                        this.MessageBoxShow(this, exception);
                        e.Cancel = true;
                        return;
                    }

                if (((DevExpress.XtraBars.Docking.DockPanel)sender).Controls[0].Controls[0] is UserControl)
                    ((DevExpress.XtraBars.Docking.DockPanel)sender).Controls[0].Controls[0].Dispose();
            }


            this.dockManager1.RemovePanel((DevExpress.XtraBars.Docking.DockPanel)sender);
        }
        #endregion

        #region Event
        event AtomusControlEventHandler IAction.BeforeActionEventHandler
        {
            add
            {
                this.beforeActionEventHandler += value;
            }
            remove
            {
                this.beforeActionEventHandler -= value;
            }
        }
        event AtomusControlEventHandler IAction.AfterActionEventHandler
        {
            add
            {
                this.afterActionEventHandler += value;
            }
            remove
            {
                this.afterActionEventHandler -= value;
            }
        }

        private void DevExpressBrowser_Load(object sender, EventArgs e)
        {
            try
            {
#if DEBUG
                DiagnosticsTool.MyDebug(string.Format("DefaultBrowser_Load(object sender = {0}, EventArgs e = {1})", (sender != null) ? sender.ToString() : "null", (e != null) ? e.ToString() : "null"));
#endif
                this.Size = new Size(0, 0);
                this.Text = Factory.FactoryConfig.GetAttribute("Atomus", "ServiceName");
            }
            //catch (AtomusException _Exception)
            //{
            //    this.MessageBoxShow(this, _Exception);
            //    Application.Exit();
            //}
            //catch (TypeInitializationException _Exception)
            //{
            //    this.MessageBoxShow(this, _Exception);
            //    Application.Exit();
            //}
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
                Application.Exit();
            }

            try
            {
                this.SetLoginControl();
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
                Application.Exit();
            }
        }

        private string NoticeString;
        private Font NoticeFont;
        private Color NoticeColor;
        private void RibbonControl_Paint(object sender, PaintEventArgs e)
        {
            if (this.NoticeString != null && this.NoticeString != "" && this.NoticeFont != null)
                e.Graphics.DrawString(this.NoticeString, this.NoticeFont, new SolidBrush(Color.Red), this.Width - (NoticeString.Length * this.NoticeFont.Size) - 20, 80F);
        }

        private void DevExpressBrowser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F4)
            {
                try
                {
                    if (this.dockManager1.ActivePanel != null)
                    {
                        this.dockManager1.ActivePanel.Close();
                    }
                    else
                    {
                        if (this.tabbedView1.ActiveDocument != null)
                            ((DevExpress.XtraBars.Docking.DockPanel)this.tabbedView1.ActiveDocument.Control).Close();
                    }

                    //    if (this.dockManager1.ActivePanel.IsMdiDocument)
                    //{
                    //    if (this.dockManager1.ActivePanel != null)
                    //    {
                    //        this.dockManager1.RemovePanel(this.dockManager1.ActivePanel);
                    //    }
                    //    else if (this.tabbedView1.ActiveDocument != null)
                    //        this.dockManager1.RemovePanel((DevExpress.XtraBars.Docking.DockPanel)this.tabbedView1.ActiveDocument.Control);
                    //}
                }
                catch (Exception exception)
                {
                    this.MessageBoxShow(this, exception);
                }
            }

            //if (e.Control && e.KeyCode == Keys.Tab && ActiveControl != this.tabControl)
            //{
            //    if (this.tabControl.SelectedIndex + 1 == this.tabControl.TabCount)
            //        this.tabControl.SelectedIndex = 0;
            //    else
            //        this.tabControl.SelectedIndex += 1;
            //}

#if DEBUG
            if (e.Control && e.Shift && e.KeyCode == Keys.D)
            {
                DiagnosticsTool.ShowForm();
            }
#endif

            if (e.Control && e.Shift && e.KeyCode == Keys.T)
            {
                DiagnosticsTool.ShowForm();
            }
        }

        private void DefaultBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.IsApplicationExitIn)
                if (!this.ApplicationExit())
                    e.Cancel = true;
        }

        private void Temp_ClosingPanel(object sender, DevExpress.XtraBars.Docking.DockPanelCancelEventArgs e)
        {
            this.FindForm().Close();

            e.Cancel = true;
        }
        #endregion

        #region ETC
        private void SetLoginControl()
        {
            UserControl userControl;

            try
            {
                if (this.loginControl == null)
                {
                    //this.loginControl = new Login.DevExpressLogin();
                    //this.loginControl = new Login.DefaultLogin();
                    this.loginControl = (IAction)this.CreateInstance("LoginControl", false);

                    this.loginControl.BeforeActionEventHandler += LoginControl_BeforeActionEventHandler;
                    this.loginControl.AfterActionEventHandler += LoginControl_AfterActionEventHandler;

                    userControl = (UserControl)this.loginControl;
                    userControl.Dock = DockStyle.Fill;

                    this.Controls.Add((UserControl)this.loginControl);
                }
                else
                    userControl = (UserControl)this.loginControl;

                userControl.Visible = true;
                userControl.BringToFront();
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }

        private void SetMdiContainer()
        {
            this.IsMdiContainer = true;

            this.tabbedView1 = new DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView();
            this.tabbedView1.DocumentActivated += this.TabbedView1_DocumentActivated;

            this.documentManager1 = new DevExpress.XtraBars.Docking2010.DocumentManager
            {
                ContainerControl = this,
                View = this.tabbedView1
            };
            this.documentManager1.ViewCollection.AddRange(new DevExpress.XtraBars.Docking2010.Views.BaseView[] { this.tabbedView1 });
        }

        private void SetBrowserViewer()
        {
            this.CreateDockPanel();
        }

        private void AddNotice(ICore core, string noticeString)
        {
            this.ribbon.Paint += this.RibbonControl_Paint;

            this.NoticeString = noticeString;

            this.NoticeFont = core.GetAttributeFont(new Font(this.Font.FontFamily, this.Font.Size * 2), "NoticeString.Font");

            if (this.NoticeFont == null)
                this.NoticeFont = new Font(this.Font.FontFamily, this.Font.Size * 2, FontStyle.Bold);

            this.NoticeColor = core.GetAttributeColor("NoticeString.ForeColor");
        }

        private void DockPanelsVisibility(DevExpress.XtraBars.Docking.DockVisibility dockVisibility)
        {
            foreach (DevExpress.XtraBars.Docking.DockPanel dockPanel in dockManager1.Panels)
            {
                if (dockPanel.ParentPanel == null)
                {
                    if (!dockPanel.DockedAsTabbedDocument)
                        dockPanel.Visibility = dockVisibility;
                }
                else
                {
                    //dockPanel.Visibility = dockVisibility;
                }
            }

        }
        private void CreateDockPanel()
        {
            string[] temps;
            DevExpress.XtraBars.Docking.DockPanel parent;
            DevExpress.XtraBars.Docking.DockPanel temp;
            string visibility;

            parent = dockManager1.ActivePanel;
            temps = this.GetAttribute("DockPanel").Split(',');

            if (temps == null || temps.Count() < 1)
                return;

            foreach (string tmp in temps)
            {
                if (parent != null)
                    temp = parent.AddPanel();
                else
                    temp = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Left);

                temp.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;

                SetDock(temp, this.GetAttribute(string.Format("DockPanel.{0}.DockingStyle", tmp)));

                this.CreateDockPanel(string.Format("DockPanel.{0}", tmp), temp);

                temp.Text = this.GetAttribute(string.Format("DockPanel.{0}.Text", tmp));
                temp.Size = this.GetAttributeSize(string.Format("DockPanel.{0}.Size", tmp));

                temp.DockedAsTabbedDocument = this.GetAttributeBool(string.Format("DockPanel.{0}.DockedAsTabbedDocument", tmp));

                if (tmp.Contains("Home"))
                {
                    temp.ClosingPanel += Temp_ClosingPanel;
                    temp.Hint = tmp;
                }
                else
                {
                    temp.Options.ShowCloseButton = false;

                    visibility = this.GetAttribute(string.Format("DockPanel.{0}.Visibility", tmp));

                    if (visibility != null && visibility != "")
                        temp.Visibility = (DevExpress.XtraBars.Docking.DockVisibility)Enum.Parse(typeof(DevExpress.XtraBars.Docking.DockVisibility), visibility);
                }

                AddAtomusUserControl(temp, string.Format("DockPanel.{0}", tmp));
            }
        }
        private void CreateDockPanel(string baseAttributeName, DevExpress.XtraBars.Docking.DockPanel parent)
        {
            DevExpress.XtraBars.Docking.DockPanel temp;
            string temp1;
            string[] temps;
            string visibility;


            temp1 = this.GetAttribute(string.Format("{0}.Child", baseAttributeName));

            if (temp1 == null || temp1.Equals(string.Empty))
                return;

            temps = temp1.Split(',');

            foreach (string tmp in temps)
            {
                if (tmp.Equals(string.Empty))
                    continue;

                if (parent != null)
                    temp = parent.AddPanel();
                else
                    temp = dockManager1.AddPanel(DevExpress.XtraBars.Docking.DockingStyle.Left);


                SetDock(temp, this.GetAttribute(string.Format("{0}.{1}.DockingStyle", baseAttributeName, tmp)));

                this.CreateDockPanel(string.Format("{0}.{1}", baseAttributeName, tmp), temp);

                temp.Text = this.GetAttribute(string.Format("{0}.{1}.Text", baseAttributeName, tmp));
                temp.Size = this.GetAttributeSize(string.Format("{0}.{1}.Size", baseAttributeName, tmp));
                temp.Options.ShowCloseButton = false;

                visibility = this.GetAttribute(string.Format("{0}.{1}.Visibility", baseAttributeName, tmp));

                if (visibility != null && visibility != "")
                    temp.Visibility = (DevExpress.XtraBars.Docking.DockVisibility)Enum.Parse(typeof(DevExpress.XtraBars.Docking.DockVisibility), visibility);

                temp.DockedAsTabbedDocument = this.GetAttributeBool(string.Format("DockPanel.{0}.DockedAsTabbedDocument", tmp));

                AddAtomusUserControl(temp, string.Format("{0}.{1}", baseAttributeName, tmp));
            }
        }

        private void AddAtomusUserControl(DevExpress.XtraBars.Docking.DockPanel dockPanel, string baseAttributeName)
        {
            System.Windows.Forms.Control control;
            IAction action;
            string Namespace;

            control = null;

            Namespace = string.Format("{0}.Namespace", baseAttributeName);

            if (Namespace == null || this.GetAttribute(Namespace) == null)
                return;

            if (this.GetAttribute(Namespace).Equals("Menu.GetControl"))
            {
                AtomusControlEventArgs e;

                //Action, new object[] { _MENU_ID, _ASSEMBLY_ID, AtomusControlEventArgs, addTabControl }
                e = new AtomusControlEventArgs("Menu.GetControl", new object[] { this.GetAttributeDecimal(string.Format(".{0}.MenuID", baseAttributeName))
                                                                                , this.GetAttributeDecimal(string.Format(".{0}.AssemblyID", baseAttributeName))
                                                                                , null, false });

                this.afterActionEventHandler?.Invoke(this, e);

                if (e.Value is System.Windows.Forms.Control)
                    control = (System.Windows.Forms.Control)e.Value;
            }
            else
            {
                control = (System.Windows.Forms.Control)this.CreateInstance(Namespace);
                //control = new Home.DevExpressHome();
            }
            //control = new Menu.DefaultMenu();
            //control = new Menu.DevExpressMenu();
            //if (Namespace.Contains("Home"))
            //{
            //    control = new Home.DevExpressHome();
            //}
            //else
            //{
            //    control = (System.Windows.Forms.Control)this.CreateInstance(Namespace);
            //}

            control.Name = baseAttributeName;

            if (baseAttributeName.Contains("Menu"))
            {
                action = (IAction)control;
                action.BeforeActionEventHandler += MenuControl_BeforeActionEventHandler;
                action.AfterActionEventHandler += MenuControl_AfterActionEventHandler;

                this.menuControl = action;
            }
            else//if (controlName.Contains("Home"))
            {
                action = (IAction)control;
                action.BeforeActionEventHandler += MenuControl_AfterActionEventHandler;
                action.AfterActionEventHandler += MenuControl_AfterActionEventHandler;
                //action.BeforeActionEventHandler += HomeControl_BeforeActionEventHandler;
                //action.AfterActionEventHandler += HomeControl_AfterActionEventHandler;
            }

            action.SetAttribute("MENU_ID", Namespace);

            control.Dock = DockStyle.Fill;

            dockPanel.Controls.Add(control);
            control.BringToFront();

            dockPanel.Tag = control;

            this.dockManager1.ActivePanel = dockPanel;

            this.DockManager1_ActivePanelChanged(null, new DevExpress.XtraBars.Docking.ActivePanelChangedEventArgs(this.dockManager1.ActivePanel, null));
        }

        private void SetDock(DevExpress.XtraBars.Docking.DockPanel control, string dock)
        {
            if (dock == null)
                return;

            control.Dock = (DevExpress.XtraBars.Docking.DockingStyle)Enum.Parse(typeof(DevExpress.XtraBars.Docking.DockingStyle), dock);
        }


        private async Task<bool> SetRibbonPages()
        {
            string tmp;
            string[] temps;
            string[] temps1;
            string[] temps2;
            DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage;
            DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup;
            DevExpress.XtraBars.BarButtonItem barButtonItem;
            //DevExpress.XtraBars.Ribbon.ApplicationMenu applicationMenu1;
            Service.IResponse result;
            decimal START_MENU_ID;


            temps = this.GetAttribute("RibbonPages").Split(',');

            if (temps == null || temps.Count() < 1)
                return true;


            foreach (string pageName in temps)
            {
                var pageIndex = (from item in this.ribbon.Pages
                                 where item.Name == pageName
                                 //&& item.GetType() == Type.GetType("DevExpress.XtraBars.Ribbon.RibbonPage")
                                 select item.PageIndex
                          ).ToList();

                if (pageIndex.Count() == 0)
                {
                    ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage
                    {
                        Name = pageName,
                        Text = this.GetAttribute(string.Format("RibbonPages.{0}.Caption", pageName))
                    };
                    pageIndex.Add(this.ribbon.Pages.Add(ribbonPage));
                }

                ribbonPage = this.ribbon.Pages[pageIndex.ToList()[0]];


                tmp = this.GetAttribute(string.Format("RibbonPages.{0}", pageName));

                if (tmp != null)
                {
                    temps1 = tmp.Split(',');

                    foreach (string pageGroupName in temps1)
                    {
                        var groups = (from item in ribbonPage.Groups
                                      where item.Name == pageGroupName
                                      select item.Name).ToList();

                        if (groups.Count() == 0)
                        {
                            ribbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup
                            {
                                ShowCaptionButton = false,
                                Name = pageGroupName,
                                Text = this.GetAttribute(string.Format("RibbonPages.{0}.{1}.Caption", pageName, pageGroupName))
                            };
                            ribbonPage.Groups.Add(ribbonPageGroup);
                            groups.Add(pageGroupName);

                            if (pageGroupName == "Skin")
                                this.AddSkinPageGroup(ribbonPageGroup);
                        }

                        ribbonPageGroup = ribbonPage.Groups[groups.ToList()[0]];

                        tmp = this.GetAttribute(string.Format("RibbonPages.{0}.{1}", pageName, pageGroupName));

                        if (tmp != null)
                        {
                            temps2 = tmp.Split(',');

                            if (pageGroupName == "Action")
                                this.items = temps2;

                            foreach (string name in temps2)
                            {
                                if ((from aa in ribbonPageGroup.ItemLinks
                                     where aa.Caption == name
                                     select aa).Count() == 0)
                                {
                                    barButtonItem = new BarButtonItem
                                    {
                                        RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large,
                                        Caption = this.GetAttribute(string.Format("RibbonPages.{0}.{1}.{2}.Caption", pageName, pageGroupName, name)),
                                        Name = name,
                                        Tag = name,
                                    };
                                    barButtonItem.ImageOptions.Image = await this.GetAttributeWebImage(string.Format("RibbonPages.{0}.{1}.{2}.ImageOptions.Image", pageName, pageGroupName, name));
                                    barButtonItem.ImageOptions.LargeImage = await this.GetAttributeWebImage(string.Format("RibbonPages.{0}.{1}.{2}.ImageOptions.LargeImage", pageName, pageGroupName, name));


                                    if (pageGroupName == "Action")
                                    {
                                        barButtonItem.ItemClick += BarButtonItem_ItemClick;

                                        barButtonItem.ItemShortcut = new BarShortcut((Keys)Enum.Parse(typeof(Keys), this.GetAttribute(string.Format("RibbonPages.{0}.{1}.{2}.ItemShortcut", pageName, pageGroupName, name))));

                                        this.ribbon.QuickToolbarItemLinks.Add(barButtonItem);

                                        this.buttonList.Add(barButtonItem);

                                        //if (this.ribbon.ApplicationButtonDropDownControl == null)
                                        //{
                                        //    applicationMenu1 = new DevExpress.XtraBars.Ribbon.ApplicationMenu();
                                        //    applicationMenu1.Name = "applicationMenu1";
                                        //    applicationMenu1.Ribbon = this.ribbon;
                                        //    this.ribbon.ApplicationButtonDropDownControl = applicationMenu1;
                                        //}

                                        //((DevExpress.XtraBars.Ribbon.ApplicationMenu)this.ribbon.ApplicationButtonDropDownControl).ItemLinks.Add(barButtonItem);
                                    }

                                    if (pageGroupName == "View")
                                    {
                                        barButtonItem.ItemClick += BarButtonItem_ItemClick;

                                        barButtonItem.ItemShortcut = new BarShortcut((Keys)Enum.Parse(typeof(Keys), this.GetAttribute(string.Format("RibbonPages.{0}.{1}.{2}.ItemShortcut", pageName, pageGroupName, name))));

                                    }

                                    this.ribbon.Items.Add(barButtonItem);
                                    ribbonPageGroup.ItemLinks.Add(barButtonItem, true);
                                }
                            }
                        }

                        tmp = this.GetAttribute(string.Format("RibbonPages.{0}.{1}.Procedure", pageName, pageGroupName));

                        if (tmp != null && tmp != "")
                        {
                            START_MENU_ID = this.GetAttributeDecimal(string.Format("RibbonPages.{0}.{1}.Procedure.PARENT_MENU_ID", pageName, pageGroupName));
                            result = await this.SearchParentMenuAsync(new DevExpressSearchParentMenuModel()
                            {
                                Procedure = tmp,
                                START_MENU_ID = START_MENU_ID,
                                ONLY_PARENT_MENU_ID = START_MENU_ID
                            });

                            if (result.Status == Service.Status.OK)
                            {
                                foreach (DataRow dataRow in result.DataSet.Tables[1].Rows)
                                {
                                    barButtonItem = new BarButtonItem
                                    {
                                        RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large,
                                        Caption = dataRow["NAME"].ToString(), //this.GetAttribute(string.Format("RibbonPages.{0}.{1}.{2}.Caption", pageName, pageGroupName, name));
                                        Name = string.Format("MENU_ID_{0}", dataRow["MENU_ID"].ToString()),
                                        Tag = new decimal[] { (decimal)dataRow["MENU_ID"], -1 }
                                    };

                                    tmp = dataRow["IMAGE_URL1"].ToString();
                                    if (tmp != "")
                                        barButtonItem.ImageOptions.Image = await this.GetAttributeWebImage(new Uri(tmp));

                                    tmp = dataRow["IMAGE_URL2"].ToString();
                                    if (tmp != "")
                                        barButtonItem.ImageOptions.LargeImage = await this.GetAttributeWebImage(new Uri(tmp));

                                    this.ribbon.Items.Add(barButtonItem);
                                    ribbonPageGroup.ItemLinks.Add(barButtonItem, true);

                                    barButtonItem.ItemClick += this.BarButtonItemParentMenu_ItemClick;
                                }
                            }

                            tmp = this.GetAttribute(string.Format("RibbonPages.{0}.{1}.All.Caption", pageName, pageGroupName));

                            if (tmp != null && tmp != "")
                            {
                                barButtonItem = new BarButtonItem
                                {
                                    RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large,
                                    Caption = tmp,
                                    Name = "MENU_ID_All",
                                    Tag = new decimal[] { START_MENU_ID, -1 }
                                };

                                barButtonItem.ImageOptions.Image = await this.GetAttributeWebImage(string.Format("RibbonPages.{0}.{1}.All.ImageOptions.Image", pageName, pageGroupName));
                                barButtonItem.ImageOptions.LargeImage = await this.GetAttributeWebImage(string.Format("RibbonPages.{0}.{1}.All.ImageOptions.LargeImage", pageName, pageGroupName));

                                this.ribbon.Items.Add(barButtonItem);
                                ribbonPageGroup.ItemLinks.Add(barButtonItem, true);

                                barButtonItem.ItemClick += BarButtonItemParentMenu_ItemClick;
                            }
                        }
                    }
                }
            }

            return true;

            //DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
            //BarEditItem beScheme;

            //repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();

            //repositoryItemComboBox1.AutoHeight = false;
            ////repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            //repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            //repositoryItemComboBox1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //repositoryItemComboBox1.Items.AddRange(new string[] { "KAMS", "FOBIS", "RMS" });

            //this.ribbon.RepositoryItems.Add(repositoryItemComboBox1);

            //beScheme = new DevExpress.XtraBars.BarEditItem();

            //beScheme.Caption = "Connection: ";
            //beScheme.Edit = repositoryItemComboBox1;
            //beScheme.EditWidth = 100;
            //beScheme.Id = 188;
            //beScheme.Name = "beScheme";
            //beScheme.EditValueChanged += BeScheme_EditValueChanged;

            //this.ribbon.Items.Add(beScheme);
            //this.ribbon.PageHeaderItemLinks.Add(beScheme);

            //beScheme.EditValue = "FOBIS";
        }

        private void BeScheme_EditValueChanged(object sender, EventArgs e)
        {
            BarEditItem barEditItem;

            barEditItem = (BarEditItem)sender;

            this.MessageBoxShow(this, string.Format("Value : {0}", barEditItem.EditValue), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SetRibbonStatusBar()
        {
            string type;
            string kind;
            string[] temps;
            BarStaticItem barStaticItem;

            temps = this.GetAttribute("RibbonStatusBar").Split(',');

            if (temps == null || temps.Count() < 1)
                return;

            foreach (string name in temps)
            {
                type = this.GetAttribute(string.Format("RibbonStatusBar.{0}.Type", name));

                switch (type)
                {
                    case "BarStaticItem" :
                        barStaticItem = new BarStaticItem
                        {
                            Alignment = (BarItemLinkAlignment)Enum.Parse(typeof(BarItemLinkAlignment), this.GetAttribute(string.Format("RibbonStatusBar.{0}.Alignment", name))),
                            Name = string.Format("RibbonStatusBar_{0}", name)
                        };

                        this.ribbon.Items.Add(barStaticItem);
                        this.ribbonStatusBar.ItemLinks.Add(barStaticItem);

                        kind = this.GetAttribute(string.Format("RibbonStatusBar.{0}.Kind", name));

                        switch (kind)
                        {
                            case "Email":
                                barStaticItem.Caption = (string)Config.Client.GetAttribute("Account.EMAIL");
                                break;
                            case "NickName":
                                barStaticItem.Caption = string.Format("{0} ({1})", Config.Client.GetAttribute("Account.NICKNAME"), Config.Client.GetAttribute("Account.USER_ID"));
                                break;
                            case "Responsibility":
                                barStaticItem.Caption = (string)Config.Client.GetAttribute("Account.RESPONSIBILITY_NAME");
                                break;
                            case "Server":
                                try
                                {
                                    if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                                        //barStaticItem.Caption = Config.Client.GetAttribute("DeployUriHost").ToString();
                                        barStaticItem.Caption = string.Format("{0}:{1}"
                                                                            , System.Deployment.Application.ApplicationDeployment.CurrentDeployment.ActivationUri.Host
                                                                            , System.Deployment.Application.ApplicationDeployment.CurrentDeployment.ActivationUri.Port);
                                    else
                                        barStaticItem.Caption = "";
                                }
                                catch (Exception ex)
                                {
                                    DiagnosticsTool.MyTrace(ex);
                                }

                                break;
                            case "Timer":
                                DateTime dateTime;
                                Timer timer;

                                timer = new Timer() { Interval = 1000 };
                                timer.Tick += (o, e) => {
                                    dateTime = ((DateTime)Config.Client.GetAttribute("Account.DATETIME"));

                                    if (Config.Client.GetAttribute("Account.DiffNowServer") == null)
                                    {
                                        Config.Client.SetAttribute("Account.DiffNowServer", dateTime - DateTime.Now);
                                    }

                                    dateTime = DateTime.Now.Add((TimeSpan)Config.Client.GetAttribute("Account.DiffNowServer"));

                                    Config.Client.SetAttribute("Account.DATETIME", dateTime);

                                    barStaticItem.Caption = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                                };

                                timer.Start();

                                break;

                            default:
                                barStaticItem.Caption = "";
                                break;
                        }
                        break;
                    //case "":
                    //    break;
                    //case "":
                    //    break;
                }


            }
        }

        DevExpress.XtraBars.Docking.DockVisibility dockVisibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
        private void BarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            IAction action;
            DevExpress.XtraBars.Docking.DockPanel dockPanel;
            string actionString;
            string message;
            AtomusControlEventArgs atomusControlEventArgs;

            try
            {
                if (e.Item.Name == "FullWindowsMode")
                {
                    if (dockVisibility == DevExpress.XtraBars.Docking.DockVisibility.Visible)
                        dockVisibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    else
                        dockVisibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;

                    this.DockPanelsVisibility(dockVisibility);
                }
                else if (this.Tag is IAction)
                {
                    if (e.Item.Tag != null && e.Item.Tag is IAction)
                        action = (IAction)e.Item.Tag;
                    else
                        action = (IAction)this.Tag;

                    if ((string)e.Item.Name == "Close")
                    {
                        if (this.dockManager1.ActivePanel != null)
                        {
                            if (this.dockManager1.ActivePanel.Hint != "")
                            {
                                dockPanel = this.dockManager1.ActivePanel;

                                dockPanel.Close();
                                //if (dockPanel.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Hidden)
                                //    this.dockManager1.RemovePanel(dockPanel);
                            }
                        }
                        else if (this.tabbedView1.ActiveDocument != null)
                            if (((DevExpress.XtraBars.Docking.DockPanel)this.tabbedView1.ActiveDocument.Control).Hint != "")
                            {
                                dockPanel = ((DevExpress.XtraBars.Docking.DockPanel)this.tabbedView1.ActiveDocument.Control);

                                dockPanel.Close();
                                //if (dockPanel.Visibility == DevExpress.XtraBars.Docking.DockVisibility.Hidden)
                                //    this.dockManager1.RemovePanel(dockPanel);
                            }
                    }
                    else
                    {
                        actionString = (string)e.Item.Name;

                        if (!actionString.StartsWith("Action."))
                        {
                            message = this.GetAttribute(string.Format("RibbonPages.System.Action.{0}.ClickMessage", actionString));

                            if (message != null && message != "")
                                if (this.MessageBoxShow(this, message, actionString, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                    return;

                            if (action.GetAttribute("ASSEMBLY_ID") != null)
                            {
                                atomusControlEventArgs = new AtomusControlEventArgs("UserControl.AssemblyVersionCheck", null);
                                this.UserControl_AfterActionEventHandler(action, atomusControlEventArgs);

                                if ((bool)atomusControlEventArgs.Value)
                                {
                                    action.ControlAction(this, (string)e.Item.Name, null);
                                    return;
                                }
                            }
                            else
                                action.ControlAction(this, (string)e.Item.Name, null);
                        }
                    }
                }
                //control = this.tabbedView1.ActiveDocument.Control;

                //if (control == null)
                //    control = dockManager1.ActivePanel;

                //this.afterActionEventHandler?.Invoke(this, control.Tag.ToString());
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }
        private void BarButtonItemParentMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarButtonItem barButtonItem;

            try
            {
                barButtonItem = (BarButtonItem)e.Item;

                if (barButtonItem.Name.StartsWith("MENU_ID"))
                {
                    this.menuControl.ControlAction(this, new AtomusControlArgs()
                    {
                        Action = "Search",
                        Value = (decimal[])barButtonItem.Tag//MENU_ID, PARENT_MENU_ID
                    });
                }
            }
            catch (Exception exception)
            {
                this.MessageBoxShow(this, exception);
            }
        }

        private void AddSkinPageGroup(DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup)
        {
            this.ribbon.BeginInit();

            SkinRibbonGalleryBarItem skinRibbonGalleryBarItem;

            skinRibbonGalleryBarItem = new SkinRibbonGalleryBarItem
            {
                Caption = "skinRibbonGalleryBarItem1",
                Id = 1,
                Name = "skinRibbonGalleryBarItem1"
            };

            this.ribbon.Items.Add(skinRibbonGalleryBarItem);

            ribbonPageGroup.ItemLinks.Add(skinRibbonGalleryBarItem);

            this.ribbon.EndInit();
        }


        private bool IsApplicationExitIn = false;
        private bool ApplicationExit()
        {
            DevExpress.XtraBars.Docking.DockPanel[] dockPanels;

            try
            {
                this.IsApplicationExitIn = true;

                if (this.MessageBoxShow(this, "종료하시겠습니까?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        dockPanels = this.dockManager1.Panels.ToArray();

                        for (int i = dockPanels.Count() - 1; i >= 0; i--)
                        {
                            if (dockPanels[i].Controls.Count > 0 && dockPanels[i].Controls[0].Controls.Count > 0 && dockPanels[i].Controls[0].Controls[0] is IAction)
                                if (!dockPanels[i].Controls[0].Controls[0].Equals(menuControl))
                                {
                                    dockPanels[i].Close();

                                    if (this.dockManager1.Panels.Contains(dockPanels[i]))
                                        return false;
                                }
                                //else
                                //    ;
                        }
                    }
                    catch (Exception exception)
                    {
                        this.MessageBoxShow(this, exception);
                    }

                    Application.Exit();
                    return true;
                }
                else
                    return false;

            }
            finally
            {
                this.IsApplicationExitIn = false;
            }
        }

        private void DebugStart()
        {
            if (!DiagnosticsTool.IsStart)
            {
                DiagnosticsTool.Mode = Mode.DebugToTextBox | Mode.DebugToFile;
                DiagnosticsTool.TextBoxBase = new RichTextBox();
                DiagnosticsTool.Start();
            }
        }

        private void TraceStart()
        {
            if (!DiagnosticsTool.IsStart)
            {
                DiagnosticsTool.Mode = Mode.TraceToTextBox | Mode.TraceToFile;
                DiagnosticsTool.TextBoxBase = new RichTextBox();
                DiagnosticsTool.Start();
            }
        }


        private void GetActivationUri()
        {
            string tmp;
            string[] tmps;
            string[] tmps1;

            try
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    tmp = ApplicationDeployment.CurrentDeployment.ActivationUri.Query;

                    if (!tmp.IsNullOrEmpty() && tmp.Contains("?"))
                    {
                        tmps = tmp.Substring(tmp.IndexOf('?') + 1).Split('&');

                        foreach (string value in tmps)
                            if (value.Contains("="))
                            {
                                tmps1 = value.Split('=');

                                if (tmps1.Length > 1)
                                    Config.Client.SetAttribute(string.Format("UriParameter.{0}", tmps1[0]), tmps1[1]);
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
            }
        }
        private void GetActivationUri_20200106()
        {
            try
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    System.Collections.Specialized.NameValueCollection nameValueTable = new System.Collections.Specialized.NameValueCollection();

                    if (ApplicationDeployment.IsNetworkDeployed)
                    {
                        DiagnosticsTool.MyTrace(new Exception(ApplicationDeployment.CurrentDeployment.ActivationUri.Query));
                        nameValueTable = HttpUtility.ParseQueryString(ApplicationDeployment.CurrentDeployment.ActivationUri.Query);
                    }

                    foreach (string key in nameValueTable.AllKeys)
                        Config.Client.SetAttribute(string.Format("UriParameter.{0}", key), nameValueTable[key]);
                }
            }
            catch (Exception ex)
            {
                DiagnosticsTool.MyTrace(ex);
            }
        }

        private bool IsFontInstalled(string name)
        {
            using (System.Drawing.Text.InstalledFontCollection fontsCollection = new System.Drawing.Text.InstalledFontCollection())
            {
                return fontsCollection.Families.Any(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            }
        }
        #endregion
    }
}