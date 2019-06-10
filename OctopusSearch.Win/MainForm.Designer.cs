namespace OctopusSearch.Win
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
            this.tabsMain = new System.Windows.Forms.TabControl();
            this.tpSettings = new System.Windows.Forms.TabPage();
            this.lblAPIKey = new System.Windows.Forms.Label();
            this.txtAPIKey = new System.Windows.Forms.TextBox();
            this.lblAPIUrl = new System.Windows.Forms.Label();
            this.txtAPIUrl = new System.Windows.Forms.TextBox();
            this.tpSearchVariables = new System.Windows.Forms.TabPage();
            this.tpSearchProjects = new System.Windows.Forms.TabPage();
            this.tpSearchTargets = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tabsMain.SuspendLayout();
            this.tpSettings.SuspendLayout();
            this.tpSearchVariables.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabsMain
            // 
            this.tabsMain.Controls.Add(this.tpSettings);
            this.tabsMain.Controls.Add(this.tpSearchVariables);
            this.tabsMain.Controls.Add(this.tpSearchProjects);
            this.tabsMain.Controls.Add(this.tpSearchTargets);
            this.tabsMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsMain.Location = new System.Drawing.Point(0, 0);
            this.tabsMain.Margin = new System.Windows.Forms.Padding(6);
            this.tabsMain.Name = "tabsMain";
            this.tabsMain.SelectedIndex = 0;
            this.tabsMain.Size = new System.Drawing.Size(792, 307);
            this.tabsMain.TabIndex = 0;
            // 
            // tpSettings
            // 
            this.tpSettings.Controls.Add(this.lblAPIKey);
            this.tpSettings.Controls.Add(this.txtAPIKey);
            this.tpSettings.Controls.Add(this.lblAPIUrl);
            this.tpSettings.Controls.Add(this.txtAPIUrl);
            this.tpSettings.Location = new System.Drawing.Point(4, 34);
            this.tpSettings.Margin = new System.Windows.Forms.Padding(6);
            this.tpSettings.Name = "tpSettings";
            this.tpSettings.Padding = new System.Windows.Forms.Padding(6);
            this.tpSettings.Size = new System.Drawing.Size(784, 269);
            this.tpSettings.TabIndex = 3;
            this.tpSettings.Text = "Settings";
            this.tpSettings.UseVisualStyleBackColor = true;
            // 
            // lblAPIKey
            // 
            this.lblAPIKey.Location = new System.Drawing.Point(15, 73);
            this.lblAPIKey.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblAPIKey.Name = "lblAPIKey";
            this.lblAPIKey.Size = new System.Drawing.Size(117, 33);
            this.lblAPIKey.TabIndex = 4;
            this.lblAPIKey.Text = "API Key:";
            this.lblAPIKey.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAPIKey
            // 
            this.txtAPIKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAPIKey.Location = new System.Drawing.Point(144, 73);
            this.txtAPIKey.Margin = new System.Windows.Forms.Padding(6);
            this.txtAPIKey.Name = "txtAPIKey";
            this.txtAPIKey.Size = new System.Drawing.Size(554, 33);
            this.txtAPIKey.TabIndex = 3;
            // 
            // lblAPIUrl
            // 
            this.lblAPIUrl.Location = new System.Drawing.Point(15, 28);
            this.lblAPIUrl.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblAPIUrl.Name = "lblAPIUrl";
            this.lblAPIUrl.Size = new System.Drawing.Size(117, 33);
            this.lblAPIUrl.TabIndex = 2;
            this.lblAPIUrl.Text = "API Url:";
            this.lblAPIUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAPIUrl
            // 
            this.txtAPIUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAPIUrl.Location = new System.Drawing.Point(144, 28);
            this.txtAPIUrl.Margin = new System.Windows.Forms.Padding(6);
            this.txtAPIUrl.Name = "txtAPIUrl";
            this.txtAPIUrl.Size = new System.Drawing.Size(554, 33);
            this.txtAPIUrl.TabIndex = 1;
            // 
            // tpSearchVariables
            // 
            this.tpSearchVariables.Controls.Add(this.label1);
            this.tpSearchVariables.Controls.Add(this.textBox2);
            this.tpSearchVariables.Location = new System.Drawing.Point(4, 34);
            this.tpSearchVariables.Margin = new System.Windows.Forms.Padding(6);
            this.tpSearchVariables.Name = "tpSearchVariables";
            this.tpSearchVariables.Padding = new System.Windows.Forms.Padding(6);
            this.tpSearchVariables.Size = new System.Drawing.Size(784, 269);
            this.tpSearchVariables.TabIndex = 0;
            this.tpSearchVariables.Text = "Search Variables";
            this.tpSearchVariables.UseVisualStyleBackColor = true;
            // 
            // tpSearchProjects
            // 
            this.tpSearchProjects.Location = new System.Drawing.Point(4, 34);
            this.tpSearchProjects.Margin = new System.Windows.Forms.Padding(6);
            this.tpSearchProjects.Name = "tpSearchProjects";
            this.tpSearchProjects.Padding = new System.Windows.Forms.Padding(6);
            this.tpSearchProjects.Size = new System.Drawing.Size(784, 269);
            this.tpSearchProjects.TabIndex = 1;
            this.tpSearchProjects.Text = "Search Projects";
            this.tpSearchProjects.UseVisualStyleBackColor = true;
            // 
            // tpSearchTargets
            // 
            this.tpSearchTargets.Location = new System.Drawing.Point(4, 34);
            this.tpSearchTargets.Margin = new System.Windows.Forms.Padding(6);
            this.tpSearchTargets.Name = "tpSearchTargets";
            this.tpSearchTargets.Padding = new System.Windows.Forms.Padding(6);
            this.tpSearchTargets.Size = new System.Drawing.Size(784, 269);
            this.tpSearchTargets.TabIndex = 2;
            this.tpSearchTargets.Text = "Search Deployment Targets";
            this.tpSearchTargets.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 33);
            this.label1.TabIndex = 4;
            this.label1.Text = "API Url:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(141, 34);
            this.textBox2.Margin = new System.Windows.Forms.Padding(6);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(554, 33);
            this.textBox2.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 307);
            this.Controls.Add(this.tabsMain);
            this.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Octopus";
            this.tabsMain.ResumeLayout(false);
            this.tpSettings.ResumeLayout(false);
            this.tpSettings.PerformLayout();
            this.tpSearchVariables.ResumeLayout(false);
            this.tpSearchVariables.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabsMain;
        private System.Windows.Forms.TabPage tpSearchVariables;
        private System.Windows.Forms.TabPage tpSearchProjects;
        private System.Windows.Forms.TabPage tpSearchTargets;
        private System.Windows.Forms.TabPage tpSettings;
        private System.Windows.Forms.Label lblAPIUrl;
        private System.Windows.Forms.TextBox txtAPIUrl;
        private System.Windows.Forms.Label lblAPIKey;
        private System.Windows.Forms.TextBox txtAPIKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
    }
}

