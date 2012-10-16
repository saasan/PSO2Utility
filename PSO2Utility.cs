using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace PSO2Utility
{
    public class StartUp
    {
        // http://www.atmarkit.co.jp/fdotnet/dotnettips/145winmutex/winmutex.html
        // �A�v���P�[�V�����Œ薼
        private static string mutexName = Application.ProductName;
        // ���d�N����h�~����~���[�e�b�N�X
        private static System.Threading.Mutex mutexObject;

        [STAThread]
        public static void Main()
        {
            // Windows 2000�iNT 5.0�j�ȍ~�̂݃O���[�o���E�~���[�e�b�N�X���p��
            OperatingSystem os = Environment.OSVersion;

            if ((os.Platform == PlatformID.Win32NT) && (os.Version.Major >= 5))
            {
                mutexName = @"Global\" + mutexName;
            }

            try
            {
                // �~���[�e�b�N�X�𐶐�����
                mutexObject = new System.Threading.Mutex(false, mutexName);
            }
            catch (ApplicationException)
            {
                return;
            }

            // �~���[�e�b�N�X���擾����
            if (mutexObject.WaitOne(0, false))
            {
                // �A�v���P�[�V���������s
                using (PSO2Utility main = new PSO2Utility())
                {
                    Application.Run();
                }

                // �~���[�e�b�N�X���������
                mutexObject.ReleaseMutex();
            }

            // �~���[�e�b�N�X��j������
            mutexObject.Close();
        }
    }

    public class PSO2Utility : IDisposable
    {
        // �ݒ�
        private PSO2UtilityOptions options = new PSO2UtilityOptions();

        // ���j���[
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
        // �^�X�N�g���C�̃A�C�R��
        private NotifyIcon notifyIcon = new NotifyIcon();
        // �E�B���h�E�֘A(�V�X�e���{�^���A�ʒu)�p�^�C�}
        private Timer windowTimer = new Timer();

        // �ݒ�E�B���h�E
        private Form formOptions;

        // PSO2�̃E�B���h�E���o������
        private bool firstTime = true;

        public PSO2Utility()
        {
            // ������
            InitializeComponent();

            // �ݒ�ǂݍ���
            options = (PSO2UtilityOptions)options.Load();

            // �ݒ�̓K�p
            ApplyOptions();

            // �C�x���g�n���h����ǉ�
            options.Changed += new EventHandler(options_Changed);

            // �^�X�N�g���C�̃A�C�R����\��
            notifyIcon.Visible = true;
        }

        public void Dispose()
        {
            // �^�X�N�g���C�̃A�C�R�����\��
            notifyIcon.Visible = false;

            // �C�x���g�n���h�����폜
            options.Changed -= new EventHandler(options_Changed);
            
            // �ݒ�ۑ�
            options.Save();
        }

        /// <summary>
        /// ������
        /// </summary>
        private void InitializeComponent()
        {
            // �R���e�L�X�g���j���[�쐬
            CreateMenu();

            // �^�X�N�g���C�̃A�C�R���쐬
            CreateNotifyIcon();

            // �^�C�}�[�ݒ�
            windowTimer.Interval = 1000;
            windowTimer.Tick += new EventHandler(windowTimer_Tick);
        }

        /// <summary>
        /// �R���e�L�X�g���j���[�쐬
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
        /// �^�X�N�g���C�̃A�C�R���쐬
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
        /// �A�C�R���̃_�u���N���b�N
        /// </summary>
        private void notifyIcon_DoubleClick(object sender, System.EventArgs e)
        {
            if (!String.IsNullOrEmpty(options.GamePath) && File.Exists(options.GamePath) && !Window.Exists(options.WindowClassName))
            {
                ExecuteGame();
            }
        }

        /// <summary>
        /// �R���e�L�X�g���j���[�̃|�b�v�A�b�v
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
        /// �E�B���h�E�̈ʒu�ƃT�C�Y��ۑ�
        /// </summary>
        private void menuWindowSave_Click(object sender, System.EventArgs e)
        {
            SaveWindow();
        }

        /// <summary>
        /// �E�B���h�E�̈ʒu�ƃT�C�Y�𕜌�
        /// </summary>
        private void menuWindowRestore_Click(object sender, System.EventArgs e)
        {
            try
            {
                Window.SetPosition(options.WindowClassName, options.WindowPosition);
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
        /// �Q�[�����N��
        /// </summary>
        private void menuExecuteGame_Click(object sender, System.EventArgs e)
        {
            ExecuteGame();
        }

        /// <summary>
        /// �V�X�e���{�^���̕\��
        /// </summary>
        private void menuSystemButtons_Click(object sender, System.EventArgs e)
        {
            options.SystemButtonsEnabled = !options.SystemButtonsEnabled;
        }

        /// <summary>
        /// �����I�ɃE�B���h�E�̈ʒu�ƃT�C�Y�𕜌�
        /// </summary>
        private void menuWindowAutoRestore_Click(object sender, System.EventArgs e)
        {
            options.WindowAutoRestoreEnabled = !options.WindowAutoRestoreEnabled;
            firstTime = true;
        }

        /// <summary>
        /// log�t�H���_���J��
        /// </summary>
        private void menuOpenFolderLog_Click(object sender, System.EventArgs e)
        {
            OpenFolder(options.LogFolder);
        }

        /// <summary>
        /// bmp�t�H���_���J��
        /// </summary>
        private void menuOpenFolderBmp_Click(object sender, System.EventArgs e)
        {
            OpenFolder(options.PicturesFolder);
        }

        /// <summary>
        /// �ݒ�
        /// </summary>
        private void menuOptions_Click(object sender, System.EventArgs e)
        {
            ShowOptions();
        }

        /// <summary>
        /// �I��
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
                if (options.SystemButtonsEnabled) window.AddSystemButton();

                if (firstTime && options.WindowAutoRestoreEnabled)
                {
                    window.SetPosition(options.WindowPosition);
                    firstTime = false;
                }
            }
            catch (WindowNotFoundException)
            {
                // �E�B���h�E�������Ȃ��Ă�����A����N�����̂��߂�true�ɖ߂�
                firstTime = true;
            }
            catch (WindowOperationException)
            {
                ShowBalloonTip(Properties.Resources.Error, Properties.Resources.WindowOperationFailed, ToolTipIcon.Error, 3);
            }
        }

        /// <summary>
        /// �E�B���h�E�̈ʒu��ۑ�
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
        /// �ݒ�E�B���h�E�\��
        /// </summary>
        private void ShowOptions()
        {
            // ���łɕ\������Ă��邩�H
            if (formOptions == null || formOptions.IsDisposed)
            {
                // ���j���[�̖�����
                foreach (MenuItem menu in contextMenu.MenuItems)
                {
                    if (!menu.Equals(menuExit)) menu.Enabled = false;
                }

                // �ݒ�E�B���h�E�\��
                using (formOptions = new FormOptions(options))
                {
                    formOptions.ShowDialog();
                }

                // ���j���[�̗L����
                foreach (MenuItem menu in contextMenu.MenuItems)
                {
                    if (!menu.Equals(menuExit)) menu.Enabled = true;
                }
            }
            else
            {
                // �A�N�e�B�u�ɂ���
                formOptions.Activate();
            }
        }

        /// <summary>
        /// �t�H���_���J��
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
        /// �Q�[�������s����
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
        /// �ݒ肪�ύX���ꂽ�C�x���g
        /// </summary>
        private void options_Changed(object sender, EventArgs e)
        {
            ApplyOptions();
        }

        /// <summary>
        /// �ݒ�̓K�p
        /// </summary>
        private void ApplyOptions()
        {
            windowTimer.Enabled = (options.SystemButtonsEnabled || options.WindowAutoRestoreEnabled);
        }

        /// <summary>
        /// �o���[���`�b�v�̕\��
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
