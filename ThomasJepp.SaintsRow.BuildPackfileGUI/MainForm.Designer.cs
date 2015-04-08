namespace ThomasJepp.SaintsRow.BuildPackfileGUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProgressMessage = new System.Windows.Forms.Label();
            this.BuildPackfileButton = new System.Windows.Forms.Button();
            this.ProgressIndicator = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.UpdateASM = new System.Windows.Forms.CheckBox();
            this.SourceFolderPath = new System.Windows.Forms.TextBox();
            this.SourceBrowse = new System.Windows.Forms.Button();
            this.AsmBrowse = new System.Windows.Forms.Button();
            this.AsmPath = new System.Windows.Forms.TextBox();
            this.SourceBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.AsmFilePicker = new System.Windows.Forms.OpenFileDialog();
            this.PackfileSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.GameComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProgressMessage
            // 
            this.ProgressMessage.BackColor = System.Drawing.Color.Transparent;
            this.ProgressMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ProgressMessage.Location = new System.Drawing.Point(12, 125);
            this.ProgressMessage.Name = "ProgressMessage";
            this.ProgressMessage.Size = new System.Drawing.Size(400, 23);
            this.ProgressMessage.TabIndex = 5;
            // 
            // BuildPackfileButton
            // 
            this.BuildPackfileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.BuildPackfileButton.Location = new System.Drawing.Point(418, 125);
            this.BuildPackfileButton.Name = "BuildPackfileButton";
            this.BuildPackfileButton.Size = new System.Drawing.Size(75, 23);
            this.BuildPackfileButton.TabIndex = 4;
            this.BuildPackfileButton.Text = "Build";
            this.BuildPackfileButton.UseVisualStyleBackColor = true;
            this.BuildPackfileButton.Click += new System.EventHandler(this.BuildPackfileButton_Click);
            // 
            // ProgressIndicator
            // 
            this.ProgressIndicator.Location = new System.Drawing.Point(12, 96);
            this.ProgressIndicator.Name = "ProgressIndicator";
            this.ProgressIndicator.Size = new System.Drawing.Size(481, 23);
            this.ProgressIndicator.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Source files:";
            // 
            // UpdateASM
            // 
            this.UpdateASM.AutoSize = true;
            this.UpdateASM.Checked = true;
            this.UpdateASM.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UpdateASM.Location = new System.Drawing.Point(12, 45);
            this.UpdateASM.Name = "UpdateASM";
            this.UpdateASM.Size = new System.Drawing.Size(123, 17);
            this.UpdateASM.TabIndex = 8;
            this.UpdateASM.Text = "Update asm_pc file?";
            this.UpdateASM.UseVisualStyleBackColor = true;
            this.UpdateASM.CheckedChanged += new System.EventHandler(this.UpdateASM_CheckedChanged);
            // 
            // SourceFolderPath
            // 
            this.SourceFolderPath.Location = new System.Drawing.Point(141, 14);
            this.SourceFolderPath.Name = "SourceFolderPath";
            this.SourceFolderPath.Size = new System.Drawing.Size(271, 20);
            this.SourceFolderPath.TabIndex = 9;
            // 
            // SourceBrowse
            // 
            this.SourceBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.SourceBrowse.Location = new System.Drawing.Point(418, 12);
            this.SourceBrowse.Name = "SourceBrowse";
            this.SourceBrowse.Size = new System.Drawing.Size(75, 23);
            this.SourceBrowse.TabIndex = 10;
            this.SourceBrowse.Text = "Browse...";
            this.SourceBrowse.UseVisualStyleBackColor = true;
            this.SourceBrowse.Click += new System.EventHandler(this.SourceBrowse_Click);
            // 
            // AsmBrowse
            // 
            this.AsmBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AsmBrowse.Location = new System.Drawing.Point(418, 41);
            this.AsmBrowse.Name = "AsmBrowse";
            this.AsmBrowse.Size = new System.Drawing.Size(75, 23);
            this.AsmBrowse.TabIndex = 12;
            this.AsmBrowse.Text = "Browse...";
            this.AsmBrowse.UseVisualStyleBackColor = true;
            this.AsmBrowse.Click += new System.EventHandler(this.AsmBrowse_Click);
            // 
            // AsmPath
            // 
            this.AsmPath.Location = new System.Drawing.Point(141, 43);
            this.AsmPath.Name = "AsmPath";
            this.AsmPath.Size = new System.Drawing.Size(271, 20);
            this.AsmPath.TabIndex = 11;
            // 
            // SourceBrowser
            // 
            this.SourceBrowser.Description = "Select the folder containing your source files:";
            this.SourceBrowser.ShowNewFolderButton = false;
            // 
            // AsmFilePicker
            // 
            this.AsmFilePicker.Filter = "asm_pc files|*.asm_pc|All files|*.*";
            this.AsmFilePicker.Title = "Choose the ASM to update:";
            // 
            // PackfileSaveDialog
            // 
            this.PackfileSaveDialog.DefaultExt = "str2_pc";
            this.PackfileSaveDialog.Filter = "Streamed Packfile|*.str2_pc|Normal Packfile|*.vpp_pc|All files|*.*";
            this.PackfileSaveDialog.Title = "Enter the filename of the packfile you wish to create:";
            // 
            // GameComboBox
            // 
            this.GameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GameComboBox.FormattingEnabled = true;
            this.GameComboBox.Items.AddRange(new object[] {
            "Saints Row 2",
            "Saints Row IV",
            "Saints Row: Gat out of Hell"});
            this.GameComboBox.Location = new System.Drawing.Point(141, 69);
            this.GameComboBox.Name = "GameComboBox";
            this.GameComboBox.Size = new System.Drawing.Size(271, 21);
            this.GameComboBox.TabIndex = 13;
            this.GameComboBox.SelectedIndexChanged += new System.EventHandler(this.GameComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Game:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 155);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.GameComboBox);
            this.Controls.Add(this.AsmBrowse);
            this.Controls.Add(this.AsmPath);
            this.Controls.Add(this.SourceBrowse);
            this.Controls.Add(this.SourceFolderPath);
            this.Controls.Add(this.UpdateASM);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProgressMessage);
            this.Controls.Add(this.BuildPackfileButton);
            this.Controls.Add(this.ProgressIndicator);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Saints Row IV Packfile Builder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ProgressMessage;
        private System.Windows.Forms.Button BuildPackfileButton;
        private System.Windows.Forms.ProgressBar ProgressIndicator;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox UpdateASM;
        private System.Windows.Forms.TextBox SourceFolderPath;
        private System.Windows.Forms.Button SourceBrowse;
        private System.Windows.Forms.Button AsmBrowse;
        private System.Windows.Forms.TextBox AsmPath;
        private System.Windows.Forms.FolderBrowserDialog SourceBrowser;
        private System.Windows.Forms.OpenFileDialog AsmFilePicker;
        private System.Windows.Forms.SaveFileDialog PackfileSaveDialog;
        private System.Windows.Forms.ComboBox GameComboBox;
        private System.Windows.Forms.Label label2;


    }
}

