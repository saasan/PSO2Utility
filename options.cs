using System;
using System.IO;
using System.ComponentModel;
using System.Xml.Serialization;     // XmlSerializer

namespace S2works.Application
{
	/// <summary>
	/// �A�v���P�[�V�����̐ݒ�
	/// </summary>
    public abstract class Options : GlobalizedPropertyGrid.GlobalizedObject
	{
        /// <summary>�ύX�C�x���g</summary>
        public event EventHandler Changed;

        /// <summary>�ݒ�t�@�C����</summary>
        private string file;
        /// <summary>�ݒ�t�@�C����ۑ�����t�H���_��</summary>
        private string folder;

        public Options()
        {
            // �ݒ�t�@�C����
            file = "options.xml";

            // Application Data�t�H���_�̃p�X
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // �ݒ�t�@�C����ۑ�����t�H���_��
            folder = appData + Path.DirectorySeparatorChar +
                System.Windows.Forms.Application.CompanyName + Path.DirectorySeparatorChar +
                System.Windows.Forms.Application.ProductName;
        }

        /// <summary>
        /// �ݒ�t�@�C����
        /// </summary>
        [XmlIgnore, Browsable(false)]
        public string File
        {
            get { return file; }
            set { file = value; }
        }

        /// <summary>
        /// �ݒ�t�@�C����ۑ�����t�H���_��
        /// </summary>
        [XmlIgnore, Browsable(false)]
        public string Folder
        {
            get { return folder; }
            set { folder = value; }
        }

        /// <summary>
        /// �ݒ�̓ǂݍ���
        /// </summary>
        public Options Load()
        {
            string filePath = folder + Path.DirectorySeparatorChar + file;

            // �C���X�^���X�Ɠ����^�̃I�u�W�F�N�g�����(�h���N���X�΍�)
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
        /// �ݒ�̕ۑ�
        /// </summary>
        public void Save()
        {
            string filePath = folder + Path.DirectorySeparatorChar + file;

            // �ۑ��t�H���_�쐬
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
        /// �ύX�C�x���g�𔭐�������
        /// </summary>
        /// <param name="e">�C�x���g�f�[�^</param>
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }
    }
}