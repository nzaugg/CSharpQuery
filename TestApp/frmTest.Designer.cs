namespace TestApp {
	partial class frmTest {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.btnRetreiveFromIndex = new System.Windows.Forms.Button();
			this.txtCriteria = new System.Windows.Forms.TextBox();
			this.lbResults = new System.Windows.Forms.ListBox();
			this.lblStopwatch = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.btnCreateIndex = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnRetreiveFromIndex
			// 
			this.btnRetreiveFromIndex.Location = new System.Drawing.Point(17, 65);
			this.btnRetreiveFromIndex.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnRetreiveFromIndex.Name = "btnRetreiveFromIndex";
			this.btnRetreiveFromIndex.Size = new System.Drawing.Size(176, 28);
			this.btnRetreiveFromIndex.TabIndex = 1;
			this.btnRetreiveFromIndex.Text = "Quick Search";
			this.btnRetreiveFromIndex.UseVisualStyleBackColor = true;
			this.btnRetreiveFromIndex.Click += new System.EventHandler(this.btnRetreiveFromIndex_Click);
			// 
			// txtCriteria
			// 
			this.txtCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtCriteria.Location = new System.Drawing.Point(17, 33);
			this.txtCriteria.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtCriteria.Name = "txtCriteria";
			this.txtCriteria.Size = new System.Drawing.Size(413, 22);
			this.txtCriteria.TabIndex = 2;
			// 
			// lbResults
			// 
			this.lbResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lbResults.FormattingEnabled = true;
			this.lbResults.ItemHeight = 16;
			this.lbResults.Location = new System.Drawing.Point(17, 119);
			this.lbResults.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.lbResults.Name = "lbResults";
			this.lbResults.Size = new System.Drawing.Size(790, 196);
			this.lbResults.TabIndex = 3;
			this.lbResults.DoubleClick += new System.EventHandler(this.lbResults_DoubleClick);
			// 
			// lblStopwatch
			// 
			this.lblStopwatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStopwatch.AutoSize = true;
			this.lblStopwatch.Location = new System.Drawing.Point(438, 36);
			this.lblStopwatch.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblStopwatch.Name = "lblStopwatch";
			this.lblStopwatch.Size = new System.Drawing.Size(82, 17);
			this.lblStopwatch.TabIndex = 4;
			this.lblStopwatch.Text = "Query took:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 97);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(650, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "MO: Multiple Occurance;   LPI: Low Phrase Index;   STP: Search Term Proximity;   " +
				"WM: Word Matching";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(17, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "Search Query";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(478, 65);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(187, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "Save Text To Database";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// btnCreateIndex
			// 
			this.btnCreateIndex.Location = new System.Drawing.Point(201, 65);
			this.btnCreateIndex.Name = "btnCreateIndex";
			this.btnCreateIndex.Size = new System.Drawing.Size(165, 28);
			this.btnCreateIndex.TabIndex = 7;
			this.btnCreateIndex.Text = "Build Index";
			this.btnCreateIndex.UseVisualStyleBackColor = true;
			this.btnCreateIndex.Click += new System.EventHandler(this.btnCreateIndex_Click);
			// 
			// frmTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(824, 325);
			this.Controls.Add(this.btnCreateIndex);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblStopwatch);
			this.Controls.Add(this.lbResults);
			this.Controls.Add(this.txtCriteria);
			this.Controls.Add(this.btnRetreiveFromIndex);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "frmTest";
			this.Text = "CSharpQuery Test Form";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnRetreiveFromIndex;
		private System.Windows.Forms.TextBox txtCriteria;
		private System.Windows.Forms.ListBox lbResults;
		private System.Windows.Forms.Label lblStopwatch;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button btnCreateIndex;
	}
}

