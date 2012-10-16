using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PSO2Utility
{
	/// <summary>
    /// FormOptions �̊T�v�̐����ł��B
	/// </summary>
	public class FormOptions : System.Windows.Forms.Form
	{
		internal System.Windows.Forms.PropertyGrid PropertyGridOptions;
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormOptions(PSO2UtilityOptions options)
		{
			//
			// Windows �t�H�[�� �f�U�C�i �T�|�[�g�ɕK�v�ł��B
			//
			InitializeComponent();

			PropertyGridOptions.SelectedObject = options;
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h 
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOptions));
            this.PropertyGridOptions = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // PropertyGridOptions
            // 
            this.PropertyGridOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyGridOptions.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.PropertyGridOptions.Location = new System.Drawing.Point(0, 0);
            this.PropertyGridOptions.Name = "PropertyGridOptions";
            this.PropertyGridOptions.Size = new System.Drawing.Size(472, 454);
            this.PropertyGridOptions.TabIndex = 0;
            // 
            // FormOptions
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.ClientSize = new System.Drawing.Size(472, 454);
            this.Controls.Add(this.PropertyGridOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormOptions";
            this.Text = "�I�v�V����";
            this.ResumeLayout(false);

		}
		#endregion
	}
}
