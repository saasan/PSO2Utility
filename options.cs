using System;
using System.IO;
using System.ComponentModel;
using System.Xml.Serialization;     // XmlSerializer

namespace S2works.Application
{
	/// <summary>
	/// アプリケーションの設定
	/// </summary>
    public abstract class Options : GlobalizedPropertyGrid.GlobalizedObject
	{
        /// <summary>変更イベント</summary>
        public event EventHandler Changed;

        /// <summary>設定ファイル名</summary>
        private string file;
        /// <summary>設定ファイルを保存するフォルダ名</summary>
        private string folder;

        public Options()
        {
            // 設定ファイル名
            file = "options.xml";

            // Application Dataフォルダのパス
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // 設定ファイルを保存するフォルダ名
            folder = appData + Path.DirectorySeparatorChar +
                System.Windows.Forms.Application.CompanyName + Path.DirectorySeparatorChar +
                System.Windows.Forms.Application.ProductName;
        }

        /// <summary>
        /// 設定ファイル名
        /// </summary>
        [XmlIgnore, Browsable(false)]
        public string File
        {
            get { return file; }
            set { file = value; }
        }

        /// <summary>
        /// 設定ファイルを保存するフォルダ名
        /// </summary>
        [XmlIgnore, Browsable(false)]
        public string Folder
        {
            get { return folder; }
            set { folder = value; }
        }

        /// <summary>
        /// 設定の読み込み
        /// </summary>
        public Options Load()
        {
            string filePath = folder + Path.DirectorySeparatorChar + file;

            // インスタンスと同じ型のオブジェクトを作る(派生クラス対策)
            Type type = this.GetType();
            Options options = (Options)Activator.CreateInstance(type);

            if (System.IO.File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(type);

                    options = (Options)serializer.Deserialize(fs);
                }
            }

            return options;
        }

        /// <summary>
        /// 設定の保存
        /// </summary>
        public void Save()
        {
            string filePath = folder + Path.DirectorySeparatorChar + file;

            // 保存フォルダ作成
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(fs, this);
            }
        }

        /// <summary>
        /// 変更イベントを発生させる
        /// </summary>
        /// <param name="e">イベントデータ</param>
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }
    }
}