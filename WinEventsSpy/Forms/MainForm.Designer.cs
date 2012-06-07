using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace WinEventsSpy.Forms
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.btnToggleHook = new System.Windows.Forms.Button();
            this.tbxProcessId = new System.Windows.Forms.TextBox();
            this.lblProcessId = new System.Windows.Forms.Label();
            this.lblThreadId = new System.Windows.Forms.Label();
            this.tbxThreadId = new System.Windows.Forms.TextBox();
            this.tbxWindowId = new System.Windows.Forms.TextBox();
            this.lblWindowId = new System.Windows.Forms.Label();
            this.aslbxMessages = new WinEventsSpy.Controls.AutoscrollListBox();
            this.SuspendLayout();
            // 
            // btnToggleHook
            // 
            this.btnToggleHook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggleHook.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnToggleHook.Location = new System.Drawing.Point(392, 256);
            this.btnToggleHook.Name = "btnToggleHook";
            this.btnToggleHook.Size = new System.Drawing.Size(72, 24);
            this.btnToggleHook.TabIndex = 4;
            this.btnToggleHook.Text = "Hook";
            this.btnToggleHook.UseVisualStyleBackColor = true;
            this.btnToggleHook.Click += new System.EventHandler(this.btnToggleHook_Click);
            // 
            // tbxProcessId
            // 
            this.tbxProcessId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxProcessId.Location = new System.Drawing.Point(200, 258);
            this.tbxProcessId.Name = "tbxProcessId";
            this.tbxProcessId.ReadOnly = true;
            this.tbxProcessId.Size = new System.Drawing.Size(56, 20);
            this.tbxProcessId.TabIndex = 2;
            // 
            // lblProcessId
            // 
            this.lblProcessId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProcessId.AutoSize = true;
            this.lblProcessId.Location = new System.Drawing.Point(136, 262);
            this.lblProcessId.Name = "lblProcessId";
            this.lblProcessId.Size = new System.Drawing.Size(56, 13);
            this.lblProcessId.TabIndex = 4;
            this.lblProcessId.Text = "Process id";
            // 
            // lblThreadId
            // 
            this.lblThreadId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblThreadId.AutoSize = true;
            this.lblThreadId.Location = new System.Drawing.Point(264, 262);
            this.lblThreadId.Name = "lblThreadId";
            this.lblThreadId.Size = new System.Drawing.Size(52, 13);
            this.lblThreadId.TabIndex = 6;
            this.lblThreadId.Text = "Thread id";
            // 
            // tbxThreadId
            // 
            this.tbxThreadId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxThreadId.Location = new System.Drawing.Point(328, 258);
            this.tbxThreadId.Name = "tbxThreadId";
            this.tbxThreadId.ReadOnly = true;
            this.tbxThreadId.Size = new System.Drawing.Size(56, 20);
            this.tbxThreadId.TabIndex = 3;
            // 
            // tbxWindowId
            // 
            this.tbxWindowId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxWindowId.Location = new System.Drawing.Point(72, 258);
            this.tbxWindowId.Name = "tbxWindowId";
            this.tbxWindowId.ReadOnly = true;
            this.tbxWindowId.Size = new System.Drawing.Size(56, 20);
            this.tbxWindowId.TabIndex = 1;
            // 
            // lblWindowId
            // 
            this.lblWindowId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWindowId.AutoSize = true;
            this.lblWindowId.Location = new System.Drawing.Point(8, 262);
            this.lblWindowId.Name = "lblWindowId";
            this.lblWindowId.Size = new System.Drawing.Size(57, 13);
            this.lblWindowId.TabIndex = 8;
            this.lblWindowId.Text = "Window id";
            // 
            // aslbxMessages
            // 
            this.aslbxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.aslbxMessages.BackColor = System.Drawing.SystemColors.Control;
            this.aslbxMessages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.aslbxMessages.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.aslbxMessages.FormattingEnabled = true;
            this.aslbxMessages.Location = new System.Drawing.Point(8, 8);
            this.aslbxMessages.Name = "aslbxMessages";
            this.aslbxMessages.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.aslbxMessages.Size = new System.Drawing.Size(456, 236);
            this.aslbxMessages.TabIndex = 0;
            this.aslbxMessages.TabStop = false;
            // 
            // Main
            // 
            this.FormClosed += new FormClosedEventHandler(Main_FormClosed);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 288);
            this.Controls.Add(this.lblWindowId);
            this.Controls.Add(this.tbxWindowId);
            this.Controls.Add(this.lblThreadId);
            this.Controls.Add(this.tbxThreadId);
            this.Controls.Add(this.lblProcessId);
            this.Controls.Add(this.tbxProcessId);
            this.Controls.Add(this.btnToggleHook);
            this.Controls.Add(this.aslbxMessages);
            this.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            this.MinimumSize = new System.Drawing.Size(488, 326);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "WinEventsSpy";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private WinEventsSpy.Controls.AutoscrollListBox aslbxMessages;
        private System.Windows.Forms.Button btnToggleHook;
        private System.Windows.Forms.TextBox tbxProcessId;
        private System.Windows.Forms.Label lblProcessId;
        private Label lblThreadId;
        private TextBox tbxThreadId;
        private TextBox tbxWindowId;
        private Label lblWindowId;
    }
}