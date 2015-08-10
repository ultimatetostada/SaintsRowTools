namespace HashTester
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
            this.InputBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.AudiokineticHashOut = new System.Windows.Forms.TextBox();
            this.CrcVolitionOut = new System.Windows.Forms.TextBox();
            this.HashVolitionOut = new System.Windows.Forms.TextBox();
            this.CrcVolitionDecimal = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // InputBox
            // 
            this.InputBox.Location = new System.Drawing.Point(111, 12);
            this.InputBox.Multiline = true;
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(420, 125);
            this.InputBox.TabIndex = 0;
            this.InputBox.TextChanged += new System.EventHandler(this.InputBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "HashVolition:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "CrcVolition:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "AudiokineticHash:";
            // 
            // AudiokineticHashOut
            // 
            this.AudiokineticHashOut.Location = new System.Drawing.Point(111, 195);
            this.AudiokineticHashOut.Name = "AudiokineticHashOut";
            this.AudiokineticHashOut.ReadOnly = true;
            this.AudiokineticHashOut.Size = new System.Drawing.Size(100, 20);
            this.AudiokineticHashOut.TabIndex = 5;
            // 
            // CrcVolitionOut
            // 
            this.CrcVolitionOut.Location = new System.Drawing.Point(111, 169);
            this.CrcVolitionOut.Name = "CrcVolitionOut";
            this.CrcVolitionOut.ReadOnly = true;
            this.CrcVolitionOut.Size = new System.Drawing.Size(100, 20);
            this.CrcVolitionOut.TabIndex = 6;
            // 
            // HashVolitionOut
            // 
            this.HashVolitionOut.Location = new System.Drawing.Point(111, 143);
            this.HashVolitionOut.Name = "HashVolitionOut";
            this.HashVolitionOut.ReadOnly = true;
            this.HashVolitionOut.Size = new System.Drawing.Size(100, 20);
            this.HashVolitionOut.TabIndex = 7;
            // 
            // CrcVolitionDecimal
            // 
            this.CrcVolitionDecimal.ForeColor = System.Drawing.SystemColors.WindowText;
            this.CrcVolitionDecimal.Location = new System.Drawing.Point(217, 169);
            this.CrcVolitionDecimal.Name = "CrcVolitionDecimal";
            this.CrcVolitionDecimal.ReadOnly = true;
            this.CrcVolitionDecimal.Size = new System.Drawing.Size(100, 20);
            this.CrcVolitionDecimal.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 227);
            this.Controls.Add(this.CrcVolitionDecimal);
            this.Controls.Add(this.HashVolitionOut);
            this.Controls.Add(this.CrcVolitionOut);
            this.Controls.Add(this.AudiokineticHashOut);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InputBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Hash Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox InputBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox AudiokineticHashOut;
        private System.Windows.Forms.TextBox CrcVolitionOut;
        private System.Windows.Forms.TextBox HashVolitionOut;
        private System.Windows.Forms.TextBox CrcVolitionDecimal;
    }
}

