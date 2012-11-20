using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace PSO2Utility
{
    public class StartUp
    {
        // http://www.atmarkit.co.jp/fdotnet/dotnettips/145winmutex/winmutex.html
        // アプリケーション固定名
        private static string mutexName = Application.ProductName;
        // 多重起動を防止するミューテックス
        private static System.Threading.Mutex mutexObject;

        [STAThread]
        public static void Main()
        {
            // Windows 2000（NT 5.0）以降のみグローバル・ミューテックス利用可
            OperatingSystem os = Environment.OSVersion;

            if ((os.Platform == PlatformID.Win32NT) && (os.Version.Major >= 5))
            {
                mutexName = @"Global\" + mutexName;
            }

            try
            {
                // ミューテックスを生成する
                mutexObject = new System.Threading.Mutex(false, mutexName);
            }
            catch (ApplicationException)
            {
                return;
            }

            // ミューテックスを取得する
            if (mutexObject.WaitOne(0, false))
            {
                // アプリケーションを実行
                using (PSO2Utility main = new PSO2Utility())
                {
                    Application.Run();
                }

                // ミューテックスを解放する
                mutexObject.ReleaseMutex();
            }

            // ミューテックスを破棄する
            mutexObject.Close();
        }
    }

    public class PSO2Utility : IDisposable
    {
        // 設定
        private PSO2UtilityOptions options = new PSO2UtilityOptions();

        // メニュー
        private MenuItem menuExecuteGame = new MenuItem();
        private MenuItem menuLine1 = new MenuItem();
        private MenuItem menuSystemButtons = new MenuItem();
        private MenuItem menuWindowAutoRestore = new MenuItem();
        private MenuItem menuLine2 = new MenuItem();
        private MenuItem menuWindowSave = new MenuItem();
        private MenuItem menuWindowRestore = new MenuItem();
        private MenuItem menuLine3 = new MenuItem();
        private MenuItem menuOpenFolderLog = new MenuItem();
        private MenuItem menuOpenFolderScreenshot = new MenuItem();
        private MenuItem menuLine4 = new MenuItem();
        private MenuItem menuOptions = new MenuItem();
        private MenuItem menuLine5 = new MenuItem();
        private MenuItem menuExit = new MenuItem();
        private ContextMenu contextMenu = new ContextMenu();
        // タスクトレイのアイコン
        private NotifyIcon notifyIcon = new NotifyIcon();
        // ウィンドウ関連(システムボタン、位置)用タイマ
        private Timer windowTimer = new Timer();

        // 設定ウィンドウ
        private Form formOptions;

        // PSO2のウィンドウ検出が初回
        private bool firstTime = true;

        public PSO2Utility()
        {
            // 初期化
            InitializeComponent();

            // 設定読み込み
            options = (PSO2UtilityOptions)options.Load();

            // タスクトレイのアイコンを表示
            notifyIcon.Visible = true;
        }

        public void Dispose()
        {
            // タスクトレイのアイコンを非表示
            notifyIcon.Visible = false;
            
            // 設定保存
            options.Save();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void InitializeComponent()
        {
            // コンテキストメニュー作成
            CreateMenu();

            // タスクトレイのアイコン作成
            CreateNotifyIcon();

            // タイマー設定
            windowTimer.Interval = 1000;
            windowTimer.Tick += new EventHandler(windowTimer_Tick);
            windowTimer.Enabled = true;
        }

        /// <summary>
        /// コンテキストメニュー作成
        /// </summary>
        private void CreateMenu()
        {
            menuExecuteGame.Text = Properties.Resources.ExecuteGame;
            menuExecuteGame.DefaultItem = true;
            menuExecuteGame.Click += new EventHandler(this.menuExecuteGame_Click);
            contextMenu.MenuItems.Add(menuExecuteGame);

            menuLine1.Text = "-";
            contextMenu.MenuItems.Add(menuLine1);

            menuSystemButtons.Text = Properties.Resources.ShowSystemButtons;
            menuSystemButtons.Click += new EventHandler(this.menuSystemButtons_Click);
            contextMenu.MenuItems.Add(menuSystemButtons);

            menuWindowAutoRestore.Text = Properties.Resources.AutoRestoreWindowPosition;
            menuWindowAutoRestore.Click += new EventHandler(this.menuWindowAutoRestore_Click);
            contextMenu.MenuItems.Add(menuWindowAutoRestore);

            menuLine2.Text = "-";
            contextMenu.MenuItems.Add(menuLine2);

            menuWindowSave.Text = Properties.Resources.SaveWindowPosition;
            menuWindowSave.Click += new EventHandler(this.menuWindowSave_Click);
            contextMenu.MenuItems.Add(menuWindowSave);

            menuWindowRestore.Text = Properties.Resources.RestoreWindowPosition;
            menuWindowRestore.Click += new EventHandler(this.menuWindowRestore_Click);
            contextMenu.MenuItems.Add(menuWindowRestore);

            menuLine3.Text = "-";
            contextMenu.MenuItems.Add(menuLine3);

            menuOpenFolderLog.Text = Properties.Resources.OpenLogFolder;
            menuOpenFolderLog.Click += new EventHandler(this.menuOpenFolderLog_Click);
            contextMenu.MenuItems.Add(menuOpenFolderLog);

            menuOpenFolderScreenshot.Text = Properties.Resources.OpenScreenshotFolder;
            menuOpenFolderScreenshot.Click += new EventHandler(this.menuOpenFolderBmp_Click);
            contextMenu.MenuItems.Add(menuOpenFolderScreenshot);

            menuLine4.Text = "-";
            contextMenu.MenuItems.Add(menuLine4);

            menuOptions.Text = Properties.Resources.Options;
            menuOptions.Click += new EventHandler(this.menuOptions_Click);
            contextMenu.MenuItems.Add(menuOptions);

            menuLine5.Text = "-";
            contextMenu.MenuItems.Add(menuLine5);

            menuExit.Text = Properties.Resources.Exit;
            menuExit.Click += new EventHandler(this.menuExit_Click);
            contextMenu.MenuItems.Add(menuExit);

            contextMenu.Popup += new EventHandler(contextMenu_Popup);
        }

        /// <summary>
        /// タスクトレイのアイコン作成
        /// </summary>
        private void CreateNotifyIcon()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            notifyIcon.Icon = new System.Drawing.Icon(assembly.GetManifestResourceStream("PSO2Utility.tray.ico"), 16, 16);
            notifyIcon.Text = Application.ProductName;
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
        }

        /// <summary>
        /// アイコンのダブルクリック
        /// </summary>
        private void notifyIcon_DoubleClick(object sender, System.EventArgs e)
        {
            if (!String.IsNullOrEmpty(options.GamePath) && File.Exists(options.GamePath) && !Window.Exists(options.WindowClassName))
            {
                ExecuteGame();
            }
        }

        /// <summary>
        /// コンテキストメニューのポップアップ
        /// </summary>
        private void contextMenu_Popup(object sender, System.EventArgs e)
        {
            menuExecuteGame.Enabled = (!String.IsNullOrEmpty(options.GamePath) && File.Exists(options.GamePath) && !Window.Exists(options.WindowClassName));
            menuSystemButtons.Checked = options.SystemButtonsEnabled;
            menuWindowAutoRestore.Checked = options.WindowAutoRestoreEnabled;
            menuWindowSave.Enabled = menuWindowRestore.Enabled = Window.Exists(options.WindowClassName);
            menuOpenFolderLog.Enabled = Directory.Exists(options.LogFolder);
            menuOpenFolderScreenshot.Enabled = Directory.Exists(options.PicturesFolder);
        }

        /// <summary>
        /// ウィンドウの位置とサイズを保存
        /// </summary>
        private void menuWindowSave_Click(object sender, System.EventArgs e)
        {
            SaveWindow();
        }

        /// <summary>
        /// ウィンドウの位置を復元
        /// </summary>
        private void menuWindowRestore_Click(object sender, System.EventArgs e)
        {
            try
            {
                Window window = new Window(options.WindowClassName);

                if (options.WindowSize.Width == 0 && options.WindowSize.Height == 0)
                {
                    // ウィンドウの位置を復元
                    window.Position = options.WindowPosition;
                }
                else
                {
                    // ウィンドウのサイズが保存されていたらサイズも復元(最小化状態から戻らなくなる現象対策)
                    window.SetPositionAndSize(options.WindowPosition, options.WindowSize);
                }

                window.SetForeground();
            }
            catch (WindowNotFoundException)
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.WindowNotFound, ToolTipIcon.Error, 3);
            }
            catch (WindowOperationException)
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.WindowOperationFailed, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// ゲームを起動
        /// </summary>
        private void menuExecuteGame_Click(object sender, System.EventArgs e)
        {
            ExecuteGame();
        }

        /// <summary>
        /// システムボタンの表示
        /// </summary>
        private void menuSystemButtons_Click(object sender, System.EventArgs e)
        {
            options.SystemButtonsEnabled = !options.SystemButtonsEnabled;
        }

        /// <summary>
        /// 自動的にウィンドウの位置を復元
        /// </summary>
        private void menuWindowAutoRestore_Click(object sender, System.EventArgs e)
        {
            options.WindowAutoRestoreEnabled = !options.WindowAutoRestoreEnabled;
            firstTime = true;
        }

        /// <summary>
        /// logフォルダを開く
        /// </summary>
        private void menuOpenFolderLog_Click(object sender, System.EventArgs e)
        {
            OpenFolder(options.LogFolder);
        }

        /// <summary>
        /// bmpフォルダを開く
        /// </summary>
        private void menuOpenFolderBmp_Click(object sender, System.EventArgs e)
        {
            OpenFolder(options.PicturesFolder);
        }

        /// <summary>
        /// 設定
        /// </summary>
        private void menuOptions_Click(object sender, System.EventArgs e)
        {
            ShowOptions();
        }

        /// <summary>
        /// 終了
        /// </summary>
        private void menuExit_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void windowTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Window window = new Window(options.WindowClassName);

                if (!window.Visible) return;

                // システムボタンの表示(初検出時のみだとちゃんと表示されないので毎回やる)
                if (options.SystemButtonsEnabled) window.AddSystemButton();

                // 初検出の場合
                if (firstTime)
                {
                    // ウィンドウサイズを保存(最小化状態から戻らなくなる現象対策)
                    options.WindowSize = window.Size;

                    // 自動的にウィンドウの位置を復元
                    if (options.WindowAutoRestoreEnabled) window.Position = options.WindowPosition;

                    firstTime = false;
                }
            }
            catch (WindowNotFoundException)
            {
                // ウィンドウが無くなっていたら、次回起動時のためにtrueに戻す
                firstTime = true;
            }
            catch (WindowOperationException)
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.WindowOperationFailed, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// ウィンドウの位置を保存
        /// </summary>
        private void SaveWindow()
        {
            try
            {
                Point position = Window.GetPosition(options.WindowClassName);

                options.WindowPosition = position;
            }
            catch (WindowNotFoundException)
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.WindowNotFound, ToolTipIcon.Error, 3);
            }
            catch (WindowOperationException)
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.WindowOperationFailed, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// 設定ウィンドウ表示
        /// </summary>
        private void ShowOptions()
        {
            // すでに表示されているか？
            if (formOptions == null || formOptions.IsDisposed)
            {
                // メニューの無効化
                foreach (MenuItem menu in contextMenu.MenuItems)
                {
                    if (!menu.Equals(menuExit)) menu.Enabled = false;
                }

                // 設定ウィンドウ表示
                using (formOptions = new FormOptions(options))
                {
                    formOptions.ShowDialog();
                }

                // メニューの有効化
                foreach (MenuItem menu in contextMenu.MenuItems)
                {
                    if (!menu.Equals(menuExit)) menu.Enabled = true;
                }
            }
            else
            {
                // アクティブにする
                formOptions.Activate();
            }
        }

        /// <summary>
        /// フォルダを開く
        /// </summary>
        private void OpenFolder(string path)
        {
            if (Directory.Exists(path))
            {
                if (System.Diagnostics.Process.Start("explorer.exe", "/n," + path) == null)
                {
                    ShowBalloonTip(Properties.Resources.Error, Properties.Resources.ExecutionFailed + "\n" + "explorer.exe", ToolTipIcon.Error, 3);
                }
            }
            else
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.DirectoryNotFound + "\n" + path, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// ゲームを実行する
        /// </summary>
        private void ExecuteGame()
        {
            if (String.IsNullOrEmpty(options.GamePath))
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.RequireOptionGameFolder, ToolTipIcon.Error, 3);
                return;
            }

            if (!File.Exists(options.GamePath))
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.FileNotFound, ToolTipIcon.Error, 3);
                return;
            }

            if (Window.Exists(options.WindowClassName))
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.AlreadyStarted, ToolTipIcon.Error, 3);
                return;
            }

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = options.GamePath;
            psi.Verb = "RunAs";
            try
            {
                System.Diagnostics.Process.Start(psi);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.ExecutionFailed, ToolTipIcon.Error, 3);
            }
            catch (FileNotFoundException)
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.FileNotFound, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// バルーンチップの表示
        /// </summary>
        private void ShowBalloonTip(string title, string text, ToolTipIcon icon, int timeout)
        {
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = text;
            notifyIcon.BalloonTipIcon = icon;
            notifyIcon.ShowBalloonTip(timeout * 1000);
        }
    }
}
