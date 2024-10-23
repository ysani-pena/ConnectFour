namespace ConnectFour
{
    partial class JoinForm
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
            txtName = new TextBox();
            txtIp = new TextBox();
            txtPort = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            btnJoin = new Button();
            txtPNumber = new TextBox();
            label4 = new Label();
            SuspendLayout();
            // 
            // txtName
            // 
            txtName.Location = new Point(36, 60);
            txtName.Name = "txtName";
            txtName.Size = new Size(128, 23);
            txtName.TabIndex = 0;
            // 
            // txtIp
            // 
            txtIp.Location = new Point(170, 60);
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(128, 23);
            txtIp.TabIndex = 1;
            // 
            // txtPort
            // 
            txtPort.Location = new Point(304, 60);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(128, 23);
            txtPort.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(59, 42);
            label1.Name = "label1";
            label1.Size = new Size(74, 15);
            label1.TabIndex = 3;
            label1.Text = "Player Name";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(204, 42);
            label2.Name = "label2";
            label2.Size = new Size(62, 15);
            label2.TabIndex = 4;
            label2.Text = "IP Address";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(347, 42);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 5;
            label3.Text = "Port #";
            //label3.Click += label3_Click;
            // 
            // btnJoin
            // 
            btnJoin.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnJoin.Font = new Font("Gadugi", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnJoin.Location = new Point(253, 98);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(86, 33);
            btnJoin.TabIndex = 6;
            btnJoin.Text = "Join";
            btnJoin.UseVisualStyleBackColor = true;
            btnJoin.Click += btnJoin_Click;
            // 
            // txtPNumber
            // 
            txtPNumber.Location = new Point(438, 60);
            txtPNumber.Name = "txtPNumber";
            txtPNumber.Size = new Size(128, 23);
            txtPNumber.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(480, 42);
            label4.Name = "label4";
            label4.Size = new Size(49, 15);
            label4.TabIndex = 8;
            label4.Text = "Player #";
            //label4.Click += label4_Click;
            // 
            // JoinForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(602, 152);
            Controls.Add(label4);
            Controls.Add(txtPNumber);
            Controls.Add(btnJoin);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtPort);
            Controls.Add(txtIp);
            Controls.Add(txtName);
            Name = "JoinForm";
            Text = "JoinConnect4";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtName;
        private TextBox txtIp;
        private TextBox txtPort;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btnJoin;
        private TextBox txtPNumber;
        private Label label4;
    }
}