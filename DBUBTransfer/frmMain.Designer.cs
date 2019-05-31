namespace DBUBTransfer
{
    partial class frmMain
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.Selected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DBName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SourceTableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtMsg = new System.Windows.Forms.RichTextBox();
            this.btnAllStart = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Selected,
            this.DBName,
            this.SourceTableName});
            this.dgvData.Location = new System.Drawing.Point(12, 12);
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            this.dgvData.RowTemplate.Height = 24;
            this.dgvData.Size = new System.Drawing.Size(800, 450);
            this.dgvData.TabIndex = 3;
            this.dgvData.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_CellContentClick);
            // 
            // Selected
            // 
            this.Selected.HeaderText = "選取";
            this.Selected.Name = "Selected";
            this.Selected.ReadOnly = true;
            // 
            // DBName
            // 
            this.DBName.HeaderText = "資料庫名";
            this.DBName.Name = "DBName";
            this.DBName.ReadOnly = true;
            // 
            // SourceTableName
            // 
            this.SourceTableName.HeaderText = "原始資料表";
            this.SourceTableName.Name = "SourceTableName";
            this.SourceTableName.ReadOnly = true;
            // 
            // txtMsg
            // 
            this.txtMsg.Location = new System.Drawing.Point(12, 497);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(800, 311);
            this.txtMsg.TabIndex = 5;
            this.txtMsg.Text = "";
            // 
            // btnAllStart
            // 
            this.btnAllStart.Location = new System.Drawing.Point(24, 468);
            this.btnAllStart.Name = "btnAllStart";
            this.btnAllStart.Size = new System.Drawing.Size(75, 23);
            this.btnAllStart.TabIndex = 4;
            this.btnAllStart.Text = "全部轉碼";
            this.btnAllStart.UseVisualStyleBackColor = true;
            this.btnAllStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(105, 468);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(96, 23);
            this.btnTransfer.TabIndex = 6;
            this.btnTransfer.Text = "不備份只轉碼";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnBackup
            // 
            this.btnBackup.Location = new System.Drawing.Point(207, 468);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(87, 23);
            this.btnBackup.TabIndex = 7;
            this.btnBackup.Text = "只備份不轉碼";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 819);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.dgvData);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.btnAllStart);
            this.Name = "frmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Selected;
        private System.Windows.Forms.DataGridViewTextBoxColumn DBName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceTableName;
        private System.Windows.Forms.RichTextBox txtMsg;
        private System.Windows.Forms.Button btnAllStart;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.Button btnBackup;
    }
}

