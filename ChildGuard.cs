/*
 * ChildGuard - 儿童眼睛守护者
 * Copyright (C) 2026 Lawyer Xu 大许律师
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Reflection;

// 在代码文件的顶部写入程序集特性 (Assembly Attributes)，编译时将被写入编译后的 PE 文件（.exe 或 .dll）的元数据区中
[assembly: AssemblyTitle("my Debut works")]
[assembly: AssemblyFileVersion("1.2")]
[assembly: AssemblyInformationalVersion("1.2")]
[assembly: AssemblyCopyright("Copyright © 2026 Lawyer Xu")]
[assembly: AssemblyProduct("ChildGuard")]

namespace ChildGuard
{
    // ==========================================
    // 0. i18n 国际化本地管理
    // ==========================================
    public static class I18n
    {
        public enum Language { zh_CN, en_US }
        
        public static Language CurrentLang = Language.en_US; 

        private static readonly Dictionary<string, Dictionary<Language, string>> Strings = new Dictionary<string, Dictionary<Language, string>>
        {
            // 通用/提示
            { "PromptTitle", new Dictionary<Language, string> { { Language.zh_CN, "提示" }, { Language.en_US, "Prompt" } } },
            { "ErrorTitle", new Dictionary<Language, string> { { Language.zh_CN, "错误" }, { Language.en_US, "Error" } } },
            { "BtnConfirm", new Dictionary<Language, string> { { Language.zh_CN, "确 定" }, { Language.en_US, "OK" } } },
            { "AppAlreadyRunning", new Dictionary<Language, string> { { Language.zh_CN, "程序已经在后台运行中了，请直接使用快捷键呼出！" }, { Language.en_US, "The program is already running. Please use the hotkey to wake it up!" } } },
            
            // 默认文本
            { "DefaultLockText", new Dictionary<Language, string> { { Language.zh_CN, "眼睛需要休息啦！\n请离开座位走动一下" }, { Language.en_US, "Time to rest your eyes!\nPlease leave your seat and walk around." } } },
            { "DefaultWarnText", new Dictionary<Language, string> { { Language.zh_CN, "即将强制休息:" }, { Language.en_US, "Mandatory rest coming up:" } } },
            { "WarningTimeFormat", new Dictionary<Language, string> { { Language.zh_CN, "{0} {1:D2}分 {2:D2}秒" }, { Language.en_US, "{0} {1:D2}m {2:D2}s" } } },
            { "LockFormat", new Dictionary<Language, string> { { Language.zh_CN, "{0}\n\n距离解锁还有: {1:D2}分 {2:D2}秒" }, { Language.en_US, "{0}\n\nTime until unlock: {1:D2}m {2:D2}s" } } },

            // 设置面板
            { "SettingsTitle", new Dictionary<Language, string> { { Language.zh_CN, "家长控制台" }, { Language.en_US, "Parental Control Panel" } } },
            { "UsageMinutes", new Dictionary<Language, string> { { Language.zh_CN, "连续使用时间 (分):" }, { Language.en_US, "Continuous Usage (min):" } } },
            { "RestMinutes", new Dictionary<Language, string> { { Language.zh_CN, "强制休息时间 (分):" }, { Language.en_US, "Mandatory Rest (min):" } } },
            { "WarningMinutes", new Dictionary<Language, string> { { Language.zh_CN, "提前预警时间 (分):" }, { Language.en_US, "Early Warning (min):" } } },
            { "AudioPath", new Dictionary<Language, string> { { Language.zh_CN, "预警音频文件:" }, { Language.en_US, "Warning Audio File:" } } },
            { "BtnBrowse", new Dictionary<Language, string> { { Language.zh_CN, "浏览" }, { Language.en_US, "Browse" } } },
            { "AudioFormatError", new Dictionary<Language, string> { { Language.zh_CN, "请拖入支持的音频格式 (.wav, .mp3, .m4a, .wma)！" }, { Language.en_US, "Please drag in supported audio formats (.wav, .mp3, .m4a, .wma)!" } } },
            { "FormatNotSupported", new Dictionary<Language, string> { { Language.zh_CN, "格式不支持" }, { Language.en_US, "Format Not Supported" } } },
            { "WarnTextLabel", new Dictionary<Language, string> { { Language.zh_CN, "预警提示文字:" }, { Language.en_US, "Warning Text:" } } },
            { "LockTextLabel", new Dictionary<Language, string> { { Language.zh_CN, "锁屏提示文字:" }, { Language.en_US, "Lock Screen Text:" } } },
            { "HotkeyLabel", new Dictionary<Language, string> { { Language.zh_CN, "呼出快捷键:" }, { Language.en_US, "Call Hotkey:" } } },
            { "PwdLabel", new Dictionary<Language, string> { { Language.zh_CN, "管理密码:" }, { Language.en_US, "Admin Password:" } } },
            { "StatusFormat", new Dictionary<Language, string> { { Language.zh_CN, "📊 今日尝试重启/绕过次数: {0} 次\n⏳ 当前时段剩余可用时间: {1} 分 {2} 秒" }, { Language.en_US, "📊 Bypass attempts today: {0}\n⏳ Remaining time this session: {1}m {2}s" } } },
            { "BtnSave", new Dictionary<Language, string> { { Language.zh_CN, "保存并应用设置" }, { Language.en_US, "Save and Apply Settings" } } },
            { "BtnTest", new Dictionary<Language, string> { { Language.zh_CN, "运行模拟测试 (5秒锁屏)" }, { Language.en_US, "Run Simulation Test (5s Lock)" } } },
            { "BtnExit", new Dictionary<Language, string> { { Language.zh_CN, "完全退出程序" }, { Language.en_US, "Exit Program Completely" } } },
            
            // 设置保存提示
            { "WarnTimeError", new Dictionary<Language, string> { { Language.zh_CN, "预警时间必须大于等于 1 分钟，\n且必须小于连续使用时间！" }, { Language.en_US, "Warning time must be >= 1 minute\nand less than continuous usage time!" } } },
            { "SettingErrorTitle", new Dictionary<Language, string> { { Language.zh_CN, "设置错误" }, { Language.en_US, "Setting Error" } } },
            { "SaveSuccess", new Dictionary<Language, string> { { Language.zh_CN, "设置已保存，新热键及规则已生效！" }, { Language.en_US, "Settings saved, new hotkeys and rules applied!" } } },
            { "CompletedTitle", new Dictionary<Language, string> { { Language.zh_CN, "完成" }, { Language.en_US, "Completed" } } },
            { "TimeFormatError", new Dictionary<Language, string> { { Language.zh_CN, "时间设置请输入有效的纯数字！" }, { Language.en_US, "Please enter valid numbers for time settings!" } } },

            // 密码窗口
            { "PwdFormTitle", new Dictionary<Language, string> { { Language.zh_CN, "身份验证" }, { Language.en_US, "Authentication" } } },
            { "PwdPrompt", new Dictionary<Language, string> { { Language.zh_CN, "请输入家长密码:" }, { Language.en_US, "Enter parental password:" } } },
            { "PwdError", new Dictionary<Language, string> { { Language.zh_CN, "您输入的密码不正确！" }, { Language.en_US, "Incorrect password!" } } },
            { "SecurityIntercept", new Dictionary<Language, string> { { Language.zh_CN, "安全拦截" }, { Language.en_US, "Security Alert" } } },

            // 版权与关于
            { "AboutTitle", new Dictionary<Language, string> { { Language.zh_CN, "关于 ChildGuard" }, { Language.en_US, "About ChildGuard" } } },
            { "CopyrightInfo", new Dictionary<Language, string> { 
                { Language.zh_CN, "软件名称: 儿童眼睛守护者\n版本: v1.2\n作者: 大许律师\n版权所有 (C) 2026\n\n用心守护孩子的健康成长。" }, 
                { Language.en_US, "Software: ChildGuard\nVersion: v1.2\nAuthor: Lawyer Xu\nCopyright (C) 2026\n\nProtecting children's healthy growth." } 
            } }
        };

        public static string T(string key)
        {
            if (Strings.ContainsKey(key) && Strings[key].ContainsKey(CurrentLang))
                return Strings[key][CurrentLang];
            return key; 
        }

        public static void InitializeFromOS()
        {
            string osLang = CultureInfo.CurrentUICulture.Name.ToLower();
            if (osLang.StartsWith("zh")) CurrentLang = Language.zh_CN;
            else CurrentLang = Language.en_US;
        }
    }

    // ==========================================
    // 1. 程序入口
    // ==========================================
    static class Program
    {
        private static System.Threading.Mutex appMutex = new System.Threading.Mutex(true, "{ChildGuard-Single-Instance-Mutex}");

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
        private static extern int SetPreferredAppMode(int preferredAppMode);

        [STAThread]
        static void Main()
        {
            I18n.InitializeFromOS();

            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();
            try { SetPreferredAppMode(2); } catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!appMutex.WaitOne(TimeSpan.Zero, true))
            {
                DarkMessageBox.Show(I18n.T("AppAlreadyRunning"), I18n.T("PromptTitle"));
                return;
            }

            Application.Run(new HiddenContext());
        }
    }

    // ==========================================
    // 自定义：支持大箭头重绘的暗色无闪烁 ComboBox
    // ==========================================
    public class DarkComboBox : ComboBox
    {
        private const int WM_PAINT = 0x000F;
        private const int WM_ERASEBKGND = 0x0014;

        [DllImport("user32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        private static extern bool EndPaint(IntPtr hwnd, ref PAINTSTRUCT lpPaint);

        [StructLayout(LayoutKind.Sequential)]
        private struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int left, top, right, bottom; }

        public DarkComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            
            using (SolidBrush bgBrush = new SolidBrush(isSelected ? Color.FromArgb(70, 70, 70) : Color.FromArgb(45, 45, 45)))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            using (SolidBrush textBrush = new SolidBrush(Color.White))
            using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
            {
                Rectangle txtBounds = e.Bounds;
                txtBounds.X += 4;
                e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, textBrush, txtBounds, sf);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ERASEBKGND)
            {
                m.Result = (IntPtr)1;
                return;
            }

            if (m.Msg == WM_PAINT)
            {
                PAINTSTRUCT ps;
                IntPtr hdc = BeginPaint(m.HWnd, out ps);

                if (this.Width > 0 && this.Height > 0)
                {
                    using (Bitmap bmp = new Bitmap(this.Width, this.Height))
                    using (Graphics bg = Graphics.FromImage(bmp))
                    {
                        int btnWidth = 24; 
                        
                        Rectangle textRect = new Rectangle(0, 0, this.Width - btnWidth, this.Height);
                        using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(45, 45, 45)))
                        {
                            bg.FillRectangle(bgBrush, textRect);
                        }
                        
                        if (this.SelectedIndex >= 0 && this.Items.Count > 0)
                        {
                            using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near })
                            using (SolidBrush textBrush = new SolidBrush(Color.White))
                            {
                                Rectangle paddedTextRect = new Rectangle(4, 0, textRect.Width - 4, textRect.Height);
                                bg.DrawString(this.Items[this.SelectedIndex].ToString(), this.Font, textBrush, paddedTextRect, sf);
                            }
                        }

                        Rectangle btnRect = new Rectangle(this.Width - btnWidth, 0, btnWidth, this.Height);
                        using (SolidBrush btnBg = new SolidBrush(Color.FromArgb(45, 45, 45)))
                        {
                            bg.FillRectangle(btnBg, btnRect);
                        }
                        
                        int cx = btnRect.Left + btnRect.Width / 2;
                        int cy = btnRect.Top + btnRect.Height / 2;
                        int size = 7; 
                        
                        Point[] arrow = new Point[] {
                            new Point(cx - size, cy - size / 2 + 1),
                            new Point(cx + size, cy - size / 2 + 1),
                            new Point(cx, cy + size / 2 + 1)
                        };
                        
                        bg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        bg.FillPolygon(Brushes.White, arrow);
                        
                        using (Pen p = new Pen(Color.FromArgb(100, 100, 100))) {
                            bg.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                            bg.DrawLine(p, btnRect.Left, 0, btnRect.Left, this.Height);
                        }

                        using (Graphics g = Graphics.FromHdc(hdc))
                        {
                            g.DrawImage(bmp, 0, 0);
                        }
                    }
                }
                
                EndPaint(m.HWnd, ref ps);
                return; 
            }
            
            base.WndProc(ref m);
        }
    }

    // ==========================================
    // 自定义：圆角、内嵌眼睛图标的密码框
    // ==========================================
    public class DarkPasswordBox : Control
    {
        public TextBox TextBox { get; private set; }
        public Label LblEye { get; private set; }
        
        public DarkPasswordBox()
        {
            this.BackColor = Color.FromArgb(32, 32, 32); 
            
            this.Height = 42;
            this.MinimumSize = new Size(0, 42); 
            this.MaximumSize = new Size(9999, 42); 
            
            this.Cursor = Cursors.IBeam;

            TextBox = new TextBox();
            TextBox.BorderStyle = BorderStyle.None;
            TextBox.BackColor = Color.FromArgb(20, 20, 20); 
            TextBox.ForeColor = Color.White;
            TextBox.PasswordChar = '*';
            TextBox.Font = new Font("Microsoft YaHei UI", 12f);

            LblEye = new Label();
            LblEye.Text = "👁";
            LblEye.Font = new Font("Segoe UI Emoji", 10f);
            LblEye.ForeColor = Color.FromArgb(120, 120, 120);
            LblEye.BackColor = Color.FromArgb(20, 20, 20); 
            LblEye.AutoSize = false;
            LblEye.Size = new Size(30, 30);
            LblEye.TextAlign = ContentAlignment.MiddleCenter;
            LblEye.Cursor = Cursors.Hand;
            
            LblEye.Click += (s, e) => {
                TextBox.PasswordChar = TextBox.PasswordChar == '*' ? '\0' : '*';
                LblEye.ForeColor = TextBox.PasswordChar == '*' ? Color.FromArgb(120, 120, 120) : Color.White;
            };

            this.Controls.Add(LblEye);
            this.Controls.Add(TextBox);
            
            this.Resize += (s, e) => {
                LblEye.Location = new Point(this.Width - LblEye.Width - 10, (this.Height - LblEye.Height) / 2);
                TextBox.Location = new Point(15, (this.Height - TextBox.Height) / 2);
                TextBox.Width = LblEye.Left - 20;
            };
            
            this.Paint += (s, e) => {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                int r = 12; // 圆角半径
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath()) {
                    path.AddArc(rect.X, rect.Y, r, r, 180, 90);
                    path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
                    path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
                    path.CloseFigure();
                    
                    using (SolidBrush innerBrush = new SolidBrush(Color.FromArgb(20, 20, 20))) {
                        e.Graphics.FillPath(innerBrush, path);
                    }
                    using (Pen pen = new Pen(Color.FromArgb(80, 80, 80), 1f)) {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };
        }

        public string PasswordText 
        { 
            get { return TextBox.Text; } 
            set { TextBox.Text = value; } 
        }
        
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            TextBox.Focus();
        }
    }

    // ==========================================
    // 2. UI 渲染引擎与全局组件
    // ==========================================
    public static class UiArchitect
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        public static readonly Font LargeFont = new Font("Microsoft YaHei UI", 12f, FontStyle.Regular);

        public static void ApplyDarkTheme(Form form)
        {
            form.BackColor = Color.FromArgb(32, 32, 32);
            form.ForeColor = Color.White;
            form.ShowIcon = false;
            form.Font = LargeFont;

            int useImmersiveDarkMode = 1;
            DwmSetWindowAttribute(form.Handle, 20, ref useImmersiveDarkMode, sizeof(int));
            DwmSetWindowAttribute(form.Handle, 19, ref useImmersiveDarkMode, sizeof(int));

            ApplyControlColorsRecursive(form);
        }

        private static void ApplyControlColorsRecursive(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is DarkPasswordBox) continue; 

                try { SetWindowTheme(ctrl.Handle, "DarkMode_Explorer", null); } catch { }

                if (ctrl is TextBox) {
                    ((TextBox)ctrl).BackColor = Color.FromArgb(45, 45, 45);
                    ((TextBox)ctrl).ForeColor = Color.White;
                    ((TextBox)ctrl).BorderStyle = BorderStyle.FixedSingle;
                }
                else if (ctrl is ComboBox && !(ctrl is DarkComboBox)) { 
                    ((ComboBox)ctrl).BackColor = Color.FromArgb(45, 45, 45);
                    ((ComboBox)ctrl).ForeColor = Color.White;
                    ((ComboBox)ctrl).FlatStyle = FlatStyle.Flat;
                }
                else if (ctrl is Button) {
                    Button btn = (Button)ctrl;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
                    btn.BackColor = Color.FromArgb(50, 50, 50);
                    btn.ForeColor = Color.White;
                }
                else if (ctrl is Label || ctrl is CheckBox) {
                    ctrl.BackColor = Color.Transparent;
                    ctrl.ForeColor = Color.White;
                }
                
                if (ctrl.HasChildren) ApplyControlColorsRecursive(ctrl);
            }
        }
    }

    public static class DarkMessageBox
    {
        public static void Show(string message, string title)
        {
            using (Form msgForm = new Form())
            {
                msgForm.Text = title;
                
                int lineCount = message.Split('\n').Length;
                int dynamicHeight = Math.Max(200, 100 + lineCount * 28);
                
                msgForm.ClientSize = new Size(420, dynamicHeight);
                msgForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                msgForm.StartPosition = FormStartPosition.CenterScreen;
                msgForm.MaximizeBox = false;
                msgForm.MinimizeBox = false;
                msgForm.TopMost = true;

                Label lbl = new Label { Text = message, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
                Button btn = new Button { Text = I18n.T("BtnConfirm"), DialogResult = DialogResult.OK, Width = 120, Height = 45 };
                Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 75 };
                
                btn.Location = new Point((420 - 120) / 2, 15);
                pnlBottom.Controls.Add(btn);
                
                msgForm.Controls.Add(lbl);
                msgForm.Controls.Add(pnlBottom);
                msgForm.AcceptButton = btn;

                UiArchitect.ApplyDarkTheme(msgForm);
                msgForm.ShowDialog();
            }
        }
    }

    public class WarningForm : Form
    {
        private Label _lbl;

        // 【修改点 1 - 拖拽支持】：引入系统 API 允许无边框窗体拖动
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        public WarningForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            
            this.Size = new Size(800, 60); 
            
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point((screen.Width - this.Width) / 2, 20);
            
            _lbl = new Label { 
                Font = new Font("Microsoft YaHei UI", 18, FontStyle.Bold), 
                Dock = DockStyle.Fill, 
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.SizeAll // 改变鼠标指针以暗示可拖动
            };
            this.Controls.Add(_lbl);

            // 【修改点 1 - 拖拽支持】：绑定鼠标按下事件，触发底层拖拽
            _lbl.MouseDown += new MouseEventHandler(DragForm);
            this.MouseDown += new MouseEventHandler(DragForm);
        }

        private void DragForm(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            UiArchitect.ApplyDarkTheme(this);
            this.BackColor = Color.FromArgb(20, 20, 20); 
            
            _lbl.ForeColor = Color.Yellow;
        }

        public void UpdateTime(int remainSec, string warnText)
        {
            _lbl.Text = string.Format(I18n.T("WarningTimeFormat"), warnText, remainSec / 60, remainSec % 60);
        }
    }

    // ==========================================
    // 3. 核心后台上下文 
    // ==========================================
    public class HiddenContext : ApplicationContext
    {
        private Timer _monitorTimer;
        private DateTime _startTime;
        public DateTime StartTime { get { return _startTime; } }

        public int UsageMinutes, RestMinutes, WarningMinutes; 
        public string Password, AudioPath, LockScreenText, WarningText;
        public int HotkeyModifiers, HotkeyChar; 
        public int DailyStartupCount; 
        
        private bool _audioPlayed = false;
        private WarningForm _warningForm;
        private const int HOTKEY_ID = 1;
        
        private bool _isSettingsOpen = false;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO { public uint cbSize; public uint dwTime; }
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern uint mciSendString(string lpstrCommand, string lpstrReturnString, uint uReturnLength, IntPtr hwndCallback);

        public HiddenContext()
        {
            LoadSettings();
            UpdateHotKey();
            Application.AddMessageFilter(new HotKeyFilter(this));
            StartMonitoring();
        }

        public void LoadSettings()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\ChildGuard"))
            {
                UsageMinutes = (int)key.GetValue("UsageMinutes", 45);
                RestMinutes = (int)key.GetValue("RestMinutes", 10);
                WarningMinutes = (int)key.GetValue("WarningMinutes", 3); 
                Password = (string)key.GetValue("Password", "123456");
                AudioPath = (string)key.GetValue("AudioPath", "");
                HotkeyModifiers = (int)key.GetValue("HotkeyMod", 0x0001 | 0x0002); 
                HotkeyChar = (int)key.GetValue("HotkeyChar", 0x53); 
                LockScreenText = (string)key.GetValue("LockScreenText", I18n.T("DefaultLockText"));
                WarningText = (string)key.GetValue("WarningText", I18n.T("DefaultWarnText"));

                string lastDateStr = (string)key.GetValue("LastStartupDate", "");
                int count = (int)key.GetValue("StartupCount", 0);
                string todayStr = DateTime.Today.ToString("yyyy-MM-dd");

                if (lastDateStr == todayStr) {
                    DailyStartupCount = count + 1;
                } else {
                    DailyStartupCount = 1;
                }
                key.SetValue("LastStartupDate", todayStr);
                key.SetValue("StartupCount", DailyStartupCount);
            }
        }

        public void UpdateHotKey()
        {
            UnregisterHotKey(IntPtr.Zero, HOTKEY_ID);
            RegisterHotKey(IntPtr.Zero, HOTKEY_ID, (uint)HotkeyModifiers, (uint)HotkeyChar);
        }

        public void SaveSettings(int usage, int rest, int warning, string pwd, string audio, int mod, int keyChar, string lockText, string warnText)
        {
            UsageMinutes = usage; RestMinutes = rest; WarningMinutes = warning; Password = pwd; AudioPath = audio;
            HotkeyModifiers = mod; HotkeyChar = keyChar; LockScreenText = lockText; WarningText = warnText;

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\ChildGuard"))
            {
                key.SetValue("UsageMinutes", usage);
                key.SetValue("RestMinutes", rest);
                key.SetValue("WarningMinutes", warning);
                key.SetValue("Password", pwd);
                key.SetValue("AudioPath", audio);
                key.SetValue("HotkeyMod", mod);
                key.SetValue("HotkeyChar", keyChar);
                key.SetValue("LockScreenText", lockText);
                key.SetValue("WarningText", warnText);
            }
            UpdateHotKey();
            _startTime = DateTime.Now;
            _audioPlayed = false;
            CloseWarningForm();
        }

        private void StartMonitoring()
        {
            _startTime = DateTime.Now;
            _audioPlayed = false;
            CloseWarningForm();

            if (_monitorTimer == null)
            {
                _monitorTimer = new Timer { Interval = 1000 };
                _monitorTimer.Tick += new EventHandler(MonitorTimer_Tick);
            }
            _monitorTimer.Start();
        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            if (_isSettingsOpen) return;

            if (GetIdleTimeSeconds() > 300) { 
                _startTime = DateTime.Now; 
                _audioPlayed = false; 
                CloseWarningForm();
                return; 
            }

            double elapsedSec = (DateTime.Now - _startTime).TotalSeconds;
            double limitSec = UsageMinutes * 60;
            int remainingSec = (int)(limitSec - elapsedSec);

            if (remainingSec <= WarningMinutes * 60 && remainingSec > 0)
            {
                if (!_audioPlayed && !string.IsNullOrEmpty(AudioPath)) {
                    PlayWarningAudio();
                    _audioPlayed = true;
                }
                
                if (_warningForm == null || _warningForm.IsDisposed) {
                    _warningForm = new WarningForm();
                    _warningForm.Show(); 
                }
                _warningForm.UpdateTime(remainingSec, WarningText);
            }
            else
            {
                CloseWarningForm();
            }

            if (elapsedSec >= limitSec) TriggerLockScreen(false);
        }

        private void CloseWarningForm()
        {
            if (_warningForm != null && !_warningForm.IsDisposed) {
                _warningForm.Close();
                _warningForm = null;
            }
        }

        private void PlayWarningAudio()
        {
            if (string.IsNullOrEmpty(AudioPath)) return;
            try {
                mciSendString("close warnAudio", null, 0, IntPtr.Zero);
                mciSendString(string.Format("open \"{0}\" alias warnAudio", AudioPath), null, 0, IntPtr.Zero);
                mciSendString("play warnAudio", null, 0, IntPtr.Zero);
            } catch { }
        }

        public void StopWarningAudio()
        {
            try {
                mciSendString("stop warnAudio", null, 0, IntPtr.Zero);
                mciSendString("close warnAudio", null, 0, IntPtr.Zero);
            } catch { }
        }

        public void TriggerLockScreen(bool isTest)
        {
            _monitorTimer.Stop();
            CloseWarningForm(); 
            StopWarningAudio(); 

            using (var lockForm = new LockForm(isTest ? 0 : RestMinutes, isTest, LockScreenText))
            {
                lockForm.ShowDialog();
            }
            StartMonitoring();
        }

        private long GetIdleTimeSeconds()
        {
            LASTINPUTINFO lastInput = new LASTINPUTINFO();
            lastInput.cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO));
            GetLastInputInfo(ref lastInput);
            return (Environment.TickCount - lastInput.dwTime) / 1000;
        }

        public void ShowSettings()
        {
            if (_isSettingsOpen) return;
            _isSettingsOpen = true;

            using (var pwdForm = new PasswordForm(this))
            {
                if (pwdForm.ShowDialog() == DialogResult.OK)
                {
                    StopWarningAudio();
                    CloseWarningForm();

                    using (var settingForm = new SettingsForm(this)) { settingForm.ShowDialog(); }
                }
            }
            
            _isSettingsOpen = false;
        }
    }

    // ==========================================
    // 4. 设置面板 
    // ==========================================
    public class SettingsForm : Form
    {
        private HiddenContext _context;
        private TextBox txtUsage, txtRest, txtWarning, txtAudio, txtLockText, txtWarnText;
        private CheckBox cbCtrl, cbAlt, cbShift;
        private DarkComboBox comboKey; 
        private DarkPasswordBox pwdBox; 
        private bool _isTestAudioPlaying = false; 

        public SettingsForm(HiddenContext context)
        {
            _context = context;
            this.Text = I18n.T("SettingsTitle");
            
            this.ClientSize = new Size(780, 840); 
            this.MinimumSize = new Size(780, 840); 
            
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable; 
            this.TopMost = true;

            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), RowCount = 12, ColumnCount = 2 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); 
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); 

            AddRow(layout, 0, I18n.T("UsageMinutes"), txtUsage = new TextBox { Text = _context.UsageMinutes.ToString(), Margin = new Padding(0, 10, 0, 0) });
            AddRow(layout, 1, I18n.T("RestMinutes"), txtRest = new TextBox { Text = _context.RestMinutes.ToString(), Margin = new Padding(0, 10, 0, 0) });
            AddRow(layout, 2, I18n.T("WarningMinutes"), txtWarning = new TextBox { Text = _context.WarningMinutes.ToString(), Margin = new Padding(0, 10, 0, 0) });
            
            Label lblAudio = new Label { Text = I18n.T("AudioPath"), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Right, TextAlign = ContentAlignment.MiddleLeft };
            layout.Controls.Add(lblAudio, 0, 3);
            
            TableLayoutPanel audioGroup = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = new Padding(0, 5, 0, 0) };
            audioGroup.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); 
            audioGroup.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); 
            audioGroup.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); 
            audioGroup.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); 

            txtAudio = new TextBox { Dock = DockStyle.Fill, Text = _context.AudioPath, Margin = new Padding(0, 5, 10, 0) };
            
            txtAudio.AllowDrop = true;
            txtAudio.DragEnter += (s, e) => {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
                else e.Effect = DragDropEffects.None;
            };
            txtAudio.DragDrop += (s, e) => {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0) {
                    string ext = System.IO.Path.GetExtension(files[0]).ToLower();
                    if (ext == ".wav" || ext == ".mp3" || ext == ".m4a" || ext == ".wma") {
                        txtAudio.Text = files[0];
                    } else {
                        DarkMessageBox.Show(I18n.T("AudioFormatError"), I18n.T("FormatNotSupported"));
                    }
                }
            };

            audioGroup.Controls.Add(txtAudio, 0, 0);

            Button btnTestAudio = new Button { 
                Text = "▶", 
                AutoSize = true,
                Dock = DockStyle.Fill, 
                Margin = new Padding(10, 0, 0, 0),
                Padding = new Padding(10, 0, 10, 0) 
            };
            btnTestAudio.Click += (s, e) => {
                if (_isTestAudioPlaying) {
                    HiddenContext.mciSendString("stop testAudio", null, 0, IntPtr.Zero);
                    HiddenContext.mciSendString("close testAudio", null, 0, IntPtr.Zero);
                    btnTestAudio.Text = "▶";
                    _isTestAudioPlaying = false;
                } else {
                    if (!string.IsNullOrEmpty(txtAudio.Text) && System.IO.File.Exists(txtAudio.Text)) {
                        HiddenContext.mciSendString("close testAudio", null, 0, IntPtr.Zero);
                        HiddenContext.mciSendString(string.Format("open \"{0}\" alias testAudio", txtAudio.Text), null, 0, IntPtr.Zero);
                        HiddenContext.mciSendString("play testAudio", null, 0, IntPtr.Zero);
                        btnTestAudio.Text = "■"; 
                        _isTestAudioPlaying = true;
                    }
                }
            };
            audioGroup.Controls.Add(btnTestAudio, 1, 0);

            Button btnBrowse = new Button { 
                Text = I18n.T("BtnBrowse"), 
                AutoSize = true,
                Dock = DockStyle.Fill, 
                Margin = new Padding(10, 0, 0, 0),
                Padding = new Padding(15, 0, 15, 0) 
            };
            btnBrowse.Click += (s, e) => {
                OpenFileDialog ofd = new OpenFileDialog { Filter = "Audio Files|*.wav;*.mp3;*.m4a;*.wma|All Files|*.*" };
                if (ofd.ShowDialog() == DialogResult.OK) txtAudio.Text = ofd.FileName;
            };
            audioGroup.Controls.Add(btnBrowse, 2, 0);
            layout.Controls.Add(audioGroup, 1, 3);

            this.FormClosing += (s, e) => {
                if (_isTestAudioPlaying) {
                    HiddenContext.mciSendString("stop testAudio", null, 0, IntPtr.Zero);
                    HiddenContext.mciSendString("close testAudio", null, 0, IntPtr.Zero);
                }
            };

            AddRow(layout, 4, I18n.T("WarnTextLabel"), txtWarnText = new TextBox { Text = _context.WarningText, Multiline = true, ScrollBars = ScrollBars.None, Margin = new Padding(0, 10, 0, 0) });
            AddRow(layout, 5, I18n.T("LockTextLabel"), txtLockText = new TextBox { Text = _context.LockScreenText, Multiline = true, ScrollBars = ScrollBars.None, Margin = new Padding(0, 10, 0, 0) });

            Label lblHotkey = new Label { Text = I18n.T("HotkeyLabel"), AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Right, TextAlign = ContentAlignment.MiddleLeft };
            layout.Controls.Add(lblHotkey, 0, 6);

            FlowLayoutPanel hotkeyGroup = new FlowLayoutPanel { Dock = DockStyle.Fill, Margin = new Padding(0), WrapContents = false };
            cbCtrl = new CheckBox { Text = "Ctrl", Checked = (_context.HotkeyModifiers & 0x0002) != 0, AutoSize = true, Margin = new Padding(0, 10, 10, 0) };
            cbAlt = new CheckBox { Text = "Alt", Checked = (_context.HotkeyModifiers & 0x0001) != 0, AutoSize = true, Margin = new Padding(0, 10, 10, 0) };
            cbShift = new CheckBox { Text = "Shift", Checked = (_context.HotkeyModifiers & 0x0004) != 0, AutoSize = true, Margin = new Padding(0, 10, 10, 0) };
            
            comboKey = new DarkComboBox { 
                Width = 90, 
                Margin = new Padding(0, 6, 0, 0),
                Font = new Font("Microsoft YaHei UI", 14f, FontStyle.Bold)
            };
            for (char c = 'A'; c <= 'Z'; c++) comboKey.Items.Add(c.ToString());
            comboKey.SelectedItem = ((char)_context.HotkeyChar).ToString();
            
            hotkeyGroup.Controls.Add(cbCtrl);
            hotkeyGroup.Controls.Add(cbAlt);
            hotkeyGroup.Controls.Add(cbShift);
            hotkeyGroup.Controls.Add(comboKey);
            layout.Controls.Add(hotkeyGroup, 1, 6);

            pwdBox = new DarkPasswordBox { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 0) };
            pwdBox.PasswordText = _context.Password;
            AddRow(layout, 7, I18n.T("PwdLabel"), pwdBox);

            double elapsedSec = (DateTime.Now - _context.StartTime).TotalSeconds;
            int remainSec = Math.Max(0, (int)(_context.UsageMinutes * 60 - elapsedSec));
            
            Label lblStatus = new Label { 
                Text = string.Format(I18n.T("StatusFormat"), _context.DailyStartupCount, remainSec / 60, remainSec % 60), 
                ForeColor = Color.Yellow, 
                AutoSize = false, 
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter, 
                Margin = new Padding(0, 5, 0, 5) 
            };
            layout.Controls.Add(lblStatus, 0, 8); layout.SetColumnSpan(lblStatus, 2);

            Button btnSave = new Button { Text = I18n.T("BtnSave"), Dock = DockStyle.Fill, Margin = new Padding(0, 10, 0, 5) };
            btnSave.Click += BtnSave_Click;
            layout.Controls.Add(btnSave, 0, 9); layout.SetColumnSpan(btnSave, 2);

            Button btnTest = new Button { Text = I18n.T("BtnTest"), Dock = DockStyle.Fill, ForeColor = Color.LightGreen, Margin = new Padding(0, 5, 0, 5) };
            btnTest.Click += (s, e) => { this.Close(); _context.TriggerLockScreen(true); };
            layout.Controls.Add(btnTest, 0, 10); layout.SetColumnSpan(btnTest, 2);

            Button btnExit = new Button { Text = I18n.T("BtnExit"), Dock = DockStyle.Bottom, Height = 50, ForeColor = Color.IndianRed, Margin = new Padding(0, 5, 0, 0) };
            btnExit.Click += (s, e) => Application.Exit();
            layout.Controls.Add(btnExit, 0, 11); layout.SetColumnSpan(btnExit, 2);

            this.Controls.Add(layout);

            MouseEventHandler rightClickAction = new MouseEventHandler((s, e) => {
                if (e.Button == MouseButtons.Right) {
                    DarkMessageBox.Show(I18n.T("CopyrightInfo"), I18n.T("AboutTitle"));
                }
            });

            this.MouseClick += rightClickAction;
            layout.MouseClick += rightClickAction;
            lblAudio.MouseClick += rightClickAction;
            audioGroup.MouseClick += rightClickAction;
            lblHotkey.MouseClick += rightClickAction;
            hotkeyGroup.MouseClick += rightClickAction;
            cbCtrl.MouseClick += rightClickAction;
            cbAlt.MouseClick += rightClickAction;
            cbShift.MouseClick += rightClickAction;
            lblStatus.MouseClick += rightClickAction; 
            
            pwdBox.MouseClick += rightClickAction;
            pwdBox.TextBox.MouseClick += rightClickAction;
            pwdBox.LblEye.MouseClick += rightClickAction;
            btnTestAudio.MouseClick += rightClickAction;
        }

        private void AddRow(TableLayoutPanel lp, int row, string label, Control ctrl)
        {
            Label lbl = new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Right, TextAlign = ContentAlignment.MiddleLeft };
            lbl.MouseClick += (s, e) => {
                if (e.Button == MouseButtons.Right) DarkMessageBox.Show(I18n.T("CopyrightInfo"), I18n.T("AboutTitle"));
            };
            lp.Controls.Add(lbl, 0, row);
            
            ctrl.Dock = DockStyle.Fill;
            lp.Controls.Add(ctrl, 1, row);
        }

        protected override void OnHandleCreated(EventArgs e) { base.OnHandleCreated(e); UiArchitect.ApplyDarkTheme(this); }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            int usage, rest, warning, mod = 0;
            if (int.TryParse(txtUsage.Text, out usage) && int.TryParse(txtRest.Text, out rest) && int.TryParse(txtWarning.Text, out warning)) {
                
                if (warning < 1 || warning >= usage) {
                    DarkMessageBox.Show(I18n.T("WarnTimeError"), I18n.T("SettingErrorTitle"));
                    return; 
                }

                if (cbAlt.Checked) mod |= 0x0001;
                if (cbCtrl.Checked) mod |= 0x0002;
                if (cbShift.Checked) mod |= 0x0004;
                int keyChar = (int)comboKey.SelectedItem.ToString()[0];

                _context.SaveSettings(usage, rest, warning, pwdBox.PasswordText, txtAudio.Text, mod, keyChar, txtLockText.Text, txtWarnText.Text);
                DarkMessageBox.Show(I18n.T("SaveSuccess"), I18n.T("CompletedTitle"));
                this.Close();
            }
            else {
                DarkMessageBox.Show(I18n.T("TimeFormatError"), I18n.T("ErrorTitle"));
            }
        }
    }

    // ==========================================
    // 5. 密码窗体
    // ==========================================
    public class PasswordForm : Form
    {
        private HiddenContext _context;
        private DarkPasswordBox pwdBox;

        public PasswordForm(HiddenContext context) {
            _context = context;
            this.Text = I18n.T("PwdFormTitle"); 
            this.ClientSize = new Size(400, 220); 
            this.StartPosition = FormStartPosition.CenterScreen; 
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.TopMost = true;

            TableLayoutPanel lp = new TableLayoutPanel { 
                Dock = DockStyle.Fill, 
                Padding = new Padding(20), 
                RowCount = 3, 
                ColumnCount = 1 
            };
            lp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            lp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            lp.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            lp.Controls.Add(new Label { Text = I18n.T("PwdPrompt"), AutoSize = true }, 0, 0);
            
            pwdBox = new DarkPasswordBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Margin = new Padding(0, 10, 0, 0) };
            pwdBox.PasswordText = "";
            lp.Controls.Add(pwdBox, 0, 1);
            
            Button btn = new Button { Text = I18n.T("BtnConfirm"), Dock = DockStyle.Right, Width = 100, Height = 45, Margin = new Padding(0, 10, 0, 0) };
            btn.Click += (s, e) => {
                if (pwdBox.PasswordText == _context.Password) this.DialogResult = DialogResult.OK;
                else DarkMessageBox.Show(I18n.T("PwdError"), I18n.T("SecurityIntercept"));
            };
            lp.Controls.Add(btn, 0, 2);
            this.Controls.Add(lp); this.AcceptButton = btn;
        }

        protected override void OnHandleCreated(EventArgs e) { base.OnHandleCreated(e); UiArchitect.ApplyDarkTheme(this); }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Activate(); 
            this.ActiveControl = pwdBox.TextBox;
            pwdBox.TextBox.Focus(); 
        }

        public string InputPassword { get { return pwdBox.PasswordText; } }
    }

    // ==========================================
    // 6. 屏保级动画锁屏窗体
    // ==========================================
    public class LockForm : Form
    {
        private int _sec;
        private Label _lbl;
        private Timer _t;         
        private Timer _animTimer; 
        
        // 【修改点 2 - 减速】：将锁屏文字的步长由 6 降低到了 2，让运动更加平缓
        private int _dx = 2, _dy = 2; 
        private string _lockText; 

        public LockForm(int minutes, bool isTest, string lockText)
        {
            _sec = isTest ? 5 : minutes * 60;
            _lockText = string.IsNullOrEmpty(lockText) ? I18n.T("DefaultLockText") : lockText;

            this.FormBorderStyle = FormBorderStyle.None; 
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true; 
            this.BackColor = Color.Black; 
            this.Cursor = Cursors.Default; 

            // 【修改点 2 - 灰色文字】：将原来的 Color.White 改为了 Color.Gray
            _lbl = new Label { 
                ForeColor = Color.Gray, 
                Font = new Font("Microsoft YaHei", 22, FontStyle.Bold), 
                AutoSize = true, 
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            UpdateTxt(); 
            this.Controls.Add(_lbl);

            Random rnd = new Random();
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            _lbl.Location = new Point(rnd.Next(0, bounds.Width - 400), rnd.Next(0, bounds.Height - 100));

            _t = new Timer { Interval = 1000 };
            _t.Tick += (s, e) => {
                _sec--; UpdateTxt();
                if (_sec <= 0) { 
                    _t.Stop(); 
                    if (_animTimer != null) _animTimer.Stop();
                    this.Close(); 
                }
            };
            _t.Start();

            _animTimer = new Timer { Interval = 30 };
            _animTimer.Tick += new EventHandler(AnimTimer_Tick);
            _animTimer.Start();
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            int nx = _lbl.Left + _dx;
            int ny = _lbl.Top + _dy;

            if (nx <= 0 || nx >= this.ClientSize.Width - _lbl.Width) {
                _dx = -_dx; 
            }
            if (ny <= 0 || ny >= this.ClientSize.Height - _lbl.Height) {
                _dy = -_dy; 
            }
            
            _lbl.Location = new Point(_lbl.Left + _dx, _lbl.Top + _dy);
        }

        private void UpdateTxt() {
            _lbl.Text = string.Format(I18n.T("LockFormat"), _lockText, _sec / 60, _sec % 60);
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (_sec > 0 && e.CloseReason == CloseReason.UserClosing) e.Cancel = true;
            base.OnFormClosing(e);
        }
    }

    // ==========================================
    // 7. 热键过滤
    // ==========================================
    public class HotKeyFilter : IMessageFilter
    {
        private HiddenContext _ctx;
        public HotKeyFilter(HiddenContext c) { _ctx = c; }
        public bool PreFilterMessage(ref Message m) {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1) { _ctx.ShowSettings(); return true; }
            return false;
        }
    }
}