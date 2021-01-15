namespace Doko.UI.Forms
{
    partial class LoginForm
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
            this.GameNameTextBox = new System.Windows.Forms.TextBox();
            this.playerNoComboBox = new System.Windows.Forms.ComboBox();
            this.playerNameTextBox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GameNameTextBox
            // 
            this.GameNameTextBox.Location = new System.Drawing.Point(38, 36);
            this.GameNameTextBox.Name = "GameNameTextBox";
            this.GameNameTextBox.Size = new System.Drawing.Size(190, 20);
            this.GameNameTextBox.TabIndex = 0;
            this.GameNameTextBox.Text = "Doppelkopf";
            // 
            // playerNoComboBox
            // 
            this.playerNoComboBox.FormatString = "N0";
            this.playerNoComboBox.FormattingEnabled = true;
            this.playerNoComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.playerNoComboBox.Location = new System.Drawing.Point(38, 62);
            this.playerNoComboBox.Name = "playerNoComboBox";
            this.playerNoComboBox.Size = new System.Drawing.Size(190, 21);
            this.playerNoComboBox.TabIndex = 1;
            this.playerNoComboBox.Text = "1";
            // 
            // playerNameTextBox
            // 
            this.playerNameTextBox.Location = new System.Drawing.Point(38, 89);
            this.playerNameTextBox.Name = "playerNameTextBox";
            this.playerNameTextBox.Size = new System.Drawing.Size(190, 20);
            this.playerNameTextBox.TabIndex = 2;
            this.playerNameTextBox.Text = "Name";
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(88, 131);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(75, 23);
            this.loginButton.TabIndex = 3;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 166);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.playerNameTextBox);
            this.Controls.Add(this.playerNoComboBox);
            this.Controls.Add(this.GameNameTextBox);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ComboBox playerNoComboBox;
        public System.Windows.Forms.TextBox GameNameTextBox;
        public System.Windows.Forms.TextBox playerNameTextBox;
        private System.Windows.Forms.Button loginButton;
    }
}