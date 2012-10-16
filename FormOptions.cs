using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PSO2Utility
{
	/// <summary>
    /// FormOptions の概要の説明です。
	/// </summary>
	public class FormOptions : System.Windows.Forms.Form
	{
		internal System.Windows.Forms.PropertyGrid PropertyGridOptions;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormOptions(PSO2UtilityOptions options)
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			PropertyGridOptions.SelectedObject = options;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
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

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
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
            this.Text = "オプション";
            this.ResumeLayout(false);

		}
		#endregion
	}
}
