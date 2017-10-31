namespace SearchEngine
{
    partial class Form1
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
            this.searchTxtBx = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.resultGV = new System.Windows.Forms.DataGridView();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.eventTxtBx = new System.Windows.Forms.TextBox();
            this.lblCrawler = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnCrawl = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblSearchEngine = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.resultGV)).BeginInit();
            this.SuspendLayout();
            // 
            // searchTxtBx
            // 
            this.searchTxtBx.Location = new System.Drawing.Point(12, 197);
            this.searchTxtBx.Name = "searchTxtBx";
            this.searchTxtBx.Size = new System.Drawing.Size(502, 20);
            this.searchTxtBx.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(520, 188);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(94, 37);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // resultGV
            // 
            this.resultGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultGV.Location = new System.Drawing.Point(12, 249);
            this.resultGV.Name = "resultGV";
            this.resultGV.Size = new System.Drawing.Size(602, 212);
            this.resultGV.TabIndex = 3;
            // 
            // eventTxtBx
            // 
            this.eventTxtBx.Location = new System.Drawing.Point(12, 34);
            this.eventTxtBx.Multiline = true;
            this.eventTxtBx.Name = "eventTxtBx";
            this.eventTxtBx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.eventTxtBx.Size = new System.Drawing.Size(502, 106);
            this.eventTxtBx.TabIndex = 4;
            // 
            // lblCrawler
            // 
            this.lblCrawler.AutoSize = true;
            this.lblCrawler.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCrawler.Location = new System.Drawing.Point(12, 9);
            this.lblCrawler.Name = "lblCrawler";
            this.lblCrawler.Size = new System.Drawing.Size(72, 22);
            this.lblCrawler.TabIndex = 5;
            this.lblCrawler.Text = "Crawler";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(521, 34);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(93, 36);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnCrawl
            // 
            this.btnCrawl.Location = new System.Drawing.Point(520, 105);
            this.btnCrawl.Name = "btnCrawl";
            this.btnCrawl.Size = new System.Drawing.Size(94, 35);
            this.btnCrawl.TabIndex = 7;
            this.btnCrawl.Text = "Crawl";
            this.btnCrawl.UseVisualStyleBackColor = true;
            this.btnCrawl.Click += new System.EventHandler(this.btnCrawl_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 146);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(498, 23);
            this.progressBar.TabIndex = 8;
            // 
            // lblSearchEngine
            // 
            this.lblSearchEngine.AutoSize = true;
            this.lblSearchEngine.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchEngine.Location = new System.Drawing.Point(13, 172);
            this.lblSearchEngine.Name = "lblSearchEngine";
            this.lblSearchEngine.Size = new System.Drawing.Size(128, 22);
            this.lblSearchEngine.TabIndex = 9;
            this.lblSearchEngine.Text = "Search Engine";
            // 
            // Form1
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 492);
            this.Controls.Add(this.lblSearchEngine);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnCrawl);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblCrawler);
            this.Controls.Add(this.eventTxtBx);
            this.Controls.Add(this.resultGV);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.searchTxtBx);
            this.Name = "Form1";
            this.Text = "Progress";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.resultGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox searchTxtBx;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView resultGV;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox eventTxtBx;
        private System.Windows.Forms.Label lblCrawler;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCrawl;
        private System.Windows.Forms.Label lblSearchEngine;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}

