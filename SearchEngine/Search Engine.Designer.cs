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
            this.searchQueryComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.progressLbl = new System.Windows.Forms.Label();
            this.crawlerLocationLbl = new System.Windows.Forms.Label();
            this.crawlLocationTxtbx = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.resetDatabaseChkBx = new System.Windows.Forms.CheckBox();
            this.divider = new System.Windows.Forms.Label();
            this.percentageLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.resultGV)).BeginInit();
            this.SuspendLayout();
            // 
            // searchTxtBx
            // 
            this.searchTxtBx.Location = new System.Drawing.Point(127, 291);
            this.searchTxtBx.Name = "searchTxtBx";
            this.searchTxtBx.Size = new System.Drawing.Size(679, 20);
            this.searchTxtBx.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(812, 282);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(94, 40);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // resultGV
            // 
            this.resultGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultGV.Location = new System.Drawing.Point(12, 343);
            this.resultGV.Name = "resultGV";
            this.resultGV.Size = new System.Drawing.Size(894, 265);
            this.resultGV.TabIndex = 3;
            // 
            // eventTxtBx
            // 
            this.eventTxtBx.Location = new System.Drawing.Point(127, 105);
            this.eventTxtBx.Multiline = true;
            this.eventTxtBx.Name = "eventTxtBx";
            this.eventTxtBx.ReadOnly = true;
            this.eventTxtBx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.eventTxtBx.Size = new System.Drawing.Size(679, 106);
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
            this.btnBrowse.Location = new System.Drawing.Point(127, 65);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(93, 36);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnCrawl
            // 
            this.btnCrawl.Location = new System.Drawing.Point(231, 65);
            this.btnCrawl.Name = "btnCrawl";
            this.btnCrawl.Size = new System.Drawing.Size(94, 35);
            this.btnCrawl.TabIndex = 7;
            this.btnCrawl.Text = "Crawl";
            this.btnCrawl.UseVisualStyleBackColor = true;
            this.btnCrawl.Click += new System.EventHandler(this.btnCrawl_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(127, 220);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(679, 26);
            this.progressBar.TabIndex = 8;
            // 
            // lblSearchEngine
            // 
            this.lblSearchEngine.AutoSize = true;
            this.lblSearchEngine.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchEngine.Location = new System.Drawing.Point(9, 262);
            this.lblSearchEngine.Name = "lblSearchEngine";
            this.lblSearchEngine.Size = new System.Drawing.Size(128, 22);
            this.lblSearchEngine.TabIndex = 9;
            this.lblSearchEngine.Text = "Search Engine";
            // 
            // searchQueryComboBox
            // 
            this.searchQueryComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.searchQueryComboBox.FormattingEnabled = true;
            this.searchQueryComboBox.Location = new System.Drawing.Point(127, 317);
            this.searchQueryComboBox.Name = "searchQueryComboBox";
            this.searchQueryComboBox.Size = new System.Drawing.Size(679, 21);
            this.searchQueryComboBox.TabIndex = 10;
            this.searchQueryComboBox.SelectedIndexChanged += new System.EventHandler(this.searchQueryComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 321);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Current Search Query";
            // 
            // progressLbl
            // 
            this.progressLbl.AutoSize = true;
            this.progressLbl.Location = new System.Drawing.Point(14, 225);
            this.progressLbl.Name = "progressLbl";
            this.progressLbl.Size = new System.Drawing.Size(86, 13);
            this.progressLbl.TabIndex = 12;
            this.progressLbl.Text = "Crawler Progress";
            // 
            // crawlerLocationLbl
            // 
            this.crawlerLocationLbl.AutoSize = true;
            this.crawlerLocationLbl.Location = new System.Drawing.Point(13, 42);
            this.crawlerLocationLbl.Name = "crawlerLocationLbl";
            this.crawlerLocationLbl.Size = new System.Drawing.Size(108, 13);
            this.crawlerLocationLbl.TabIndex = 13;
            this.crawlerLocationLbl.Text = "Set Location to Index";
            // 
            // crawlLocationTxtbx
            // 
            this.crawlLocationTxtbx.Location = new System.Drawing.Point(127, 39);
            this.crawlLocationTxtbx.Name = "crawlLocationTxtbx";
            this.crawlLocationTxtbx.ReadOnly = true;
            this.crawlLocationTxtbx.Size = new System.Drawing.Size(679, 20);
            this.crawlLocationTxtbx.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Crawler Log";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 296);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Search ";
            // 
            // resetDatabaseChkBx
            // 
            this.resetDatabaseChkBx.AutoSize = true;
            this.resetDatabaseChkBx.Location = new System.Drawing.Point(331, 82);
            this.resetDatabaseChkBx.Name = "resetDatabaseChkBx";
            this.resetDatabaseChkBx.Size = new System.Drawing.Size(103, 17);
            this.resetDatabaseChkBx.TabIndex = 18;
            this.resetDatabaseChkBx.Text = "Reset Database";
            this.resetDatabaseChkBx.UseVisualStyleBackColor = true;
            // 
            // divider
            // 
            this.divider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.divider.Location = new System.Drawing.Point(0, 258);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(915, 2);
            this.divider.TabIndex = 19;
            // 
            // percentageLbl
            // 
            this.percentageLbl.AutoSize = true;
            this.percentageLbl.Location = new System.Drawing.Point(819, 226);
            this.percentageLbl.Name = "percentageLbl";
            this.percentageLbl.Size = new System.Drawing.Size(0, 13);
            this.percentageLbl.TabIndex = 20;
            // 
            // Form1
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 623);
            this.Controls.Add(this.percentageLbl);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.resetDatabaseChkBx);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.crawlLocationTxtbx);
            this.Controls.Add(this.crawlerLocationLbl);
            this.Controls.Add(this.progressLbl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.searchQueryComboBox);
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
            this.Text = "Search Engine";
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
        private System.Windows.Forms.ComboBox searchQueryComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label progressLbl;
        private System.Windows.Forms.Label crawlerLocationLbl;
        private System.Windows.Forms.TextBox crawlLocationTxtbx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox resetDatabaseChkBx;
        private System.Windows.Forms.Label divider;
        private System.Windows.Forms.Label percentageLbl;
    }
}

