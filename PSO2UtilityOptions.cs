using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GlobalizedPropertyGrid;

namespace PSO2Utility
{
    /// <summary>
    /// アプリケーションの設定
    /// </summary>
    public class PSO2UtilityOptions : S2works.Application.Options
    {
        /// <summary>マイドキュメント内のPSO2フォルダ</summary>
        private string myDocumentsFolder;
        /// <summary>PSO2のウィンドウクラス名</summary>
        private string windowClassName;
        /// <summary>logフォルダ名</summary>
        private string logFolder;
        /// <summary>picturesフォルダ名</summary>
        private string picturesFolder;

        /// <summary>PSO2のフォルダ名</summary>
        private string gameFolder = @"C:\Program Files (x86)\SEGA\PHANTASYSTARONLINE2";
        /// <summary>システムボタンON/OFF</summary>
        private bool systemButtonsEnabled = false;
        /// <summary>自動的にウィンドウの位置を復元ON/OFF</summary>
        private bool windowAutoRestoreEnabled = false;

        /// <summary>ウィンドウの位置</summary>
        private Point windowPosition = new Point(50, 50);

        public PSO2UtilityOptions()
        {
            // マイドキュメント内のPSO2フォルダとPSO2のウィンドウクラス名
            myDocumentsFolder = @"SEGA\PHANTASYSTARONLINE2";
            windowClassName = "PHANTASY STAR ONLINE 2 NEW GENESIS";

            // マイドキュメントフォルダのパス
            string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // logフォルダ名
            logFolder = myDocuments + Path.DirectorySeparatorChar +
                myDocumentsFolder + Path.DirectorySeparatorChar + "log";

            // picturesフォルダ名
            picturesFolder = myDocuments + Path.DirectorySeparatorChar +
                myDocumentsFolder + Path.DirectorySeparatorChar + "pictures";
        }

        /// <summary>
        /// PSO2のフォルダ名
        /// </summary>
        [GlobalizedCategory("General")]
        [Editor(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string GameFolder
        {
            get { return gameFolder; }
            set {
                gameFolder = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// システムボタンON/OFF
        /// </summary>
        [GlobalizedCategory("General")]
        public bool SystemButtonsEnabled
        {
            get { return systemButtonsEnabled; }
            set
            {
                systemButtonsEnabled = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 自動的にウィンドウの位置を復元ON/OFF
        /// </summary>
        [GlobalizedCategory("RestoreWindowPosition")]
        public bool WindowAutoRestoreEnabled
        {
            get { return windowAutoRestoreEnabled; }
            set
            {
                windowAutoRestoreEnabled = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// ウィンドウの位置
        /// </summary>
        [GlobalizedCategory("RestoreWindowPosition")]
        public Point WindowPosition
        {
            get { return windowPosition; }
            set {
                windowPosition = value;
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// PSO2(pso2launcher.exe)のパス
        /// </summary>
        [Browsable(false)]
        public string GamePath
        {
            get
            {
                if (String.IsNullOrEmpty(gameFolder))
                {
                    return String.Empty;
                }
                else
                {
                    return gameFolder + @"\pso2_bin\pso2launcher.exe";
                }
            }
        }

        /// <summary>
        /// マイドキュメント内のPSO2フォルダ
        /// </summary>
        [Browsable(false)]
        public string MyDocumentsFolder
        {
            get { return myDocumentsFolder; }
        }

        /// <summary>
        /// PSO2のウィンドウクラス名
        /// </summary>
        [Browsable(false)]
        public string WindowClassName
        {
            get { return windowClassName; }
        }

        /// <summary>
        /// logフォルダ名
        /// </summary>
        [Browsable(false)]
        public string LogFolder
        {
            get { return logFolder; }
        }

        /// <summary>
        /// picturesフォルダ名
        /// </summary>
        [Browsable(false)]
        public string PicturesFolder
        {
            get { return picturesFolder; }
        }
    }
}