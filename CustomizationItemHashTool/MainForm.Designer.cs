namespace CustomizationItemHashTool
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
            this.ItemName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.MaleMeshFilename = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.VariantID = new System.Windows.Forms.NumericUpDown();
            this.MaleStr2PCName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.FemaleStr2PCName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.VariantID)).BeginInit();
            this.SuspendLayout();
            // 
            // ItemName
            // 
            this.ItemName.Location = new System.Drawing.Point(121, 12);
            this.ItemName.Name = "ItemName";
            this.ItemName.Size = new System.Drawing.Size(191, 20);
            this.ItemName.TabIndex = 0;
            this.ItemName.TextChanged += new System.EventHandler(this.ItemName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Item name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Male mesh filename:";
            // 
            // MaleMeshFilename
            // 
            this.MaleMeshFilename.Location = new System.Drawing.Point(121, 38);
            this.MaleMeshFilename.Name = "MaleMeshFilename";
            this.MaleMeshFilename.Size = new System.Drawing.Size(191, 20);
            this.MaleMeshFilename.TabIndex = 3;
            this.MaleMeshFilename.TextChanged += new System.EventHandler(this.MaleMeshFilename_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Variant ID:";
            // 
            // VariantID
            // 
            this.VariantID.Location = new System.Drawing.Point(121, 64);
            this.VariantID.Maximum = new decimal(new int[] {
            1215752191,
            23,
            0,
            0});
            this.VariantID.Name = "VariantID";
            this.VariantID.Size = new System.Drawing.Size(191, 20);
            this.VariantID.TabIndex = 5;
            this.VariantID.ValueChanged += new System.EventHandler(this.VariantID_ValueChanged);
            // 
            // MaleStr2PCName
            // 
            this.MaleStr2PCName.Location = new System.Drawing.Point(121, 104);
            this.MaleStr2PCName.Name = "MaleStr2PCName";
            this.MaleStr2PCName.ReadOnly = true;
            this.MaleStr2PCName.Size = new System.Drawing.Size(191, 20);
            this.MaleStr2PCName.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Male str2_pc:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Female str2_pc:";
            // 
            // FemaleStr2PCName
            // 
            this.FemaleStr2PCName.Location = new System.Drawing.Point(121, 130);
            this.FemaleStr2PCName.Name = "FemaleStr2PCName";
            this.FemaleStr2PCName.ReadOnly = true;
            this.FemaleStr2PCName.Size = new System.Drawing.Size(191, 20);
            this.FemaleStr2PCName.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 162);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.FemaleStr2PCName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.MaleStr2PCName);
            this.Controls.Add(this.VariantID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MaleMeshFilename);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ItemName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Customization Item Hash Tool";
            ((System.ComponentModel.ISupportInitialize)(this.VariantID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ItemName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox MaleMeshFilename;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown VariantID;
        private System.Windows.Forms.TextBox MaleStr2PCName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox FemaleStr2PCName;
    }
}

