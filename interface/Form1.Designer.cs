namespace @interface
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.DataGridView processDataGridView;
        private System.Windows.Forms.DataVisualization.Charting.Chart usageChart;
        private System.Windows.Forms.GroupBox categoryGroupBox;
        private System.Windows.Forms.RadioButton allRadioButton;
        private System.Windows.Forms.RadioButton gamesRadioButton;
        private System.Windows.Forms.RadioButton learningRadioButton;
        private System.Windows.Forms.RadioButton codingRadioButton;
        private System.Windows.Forms.RadioButton entertainmentRadioButton;
        private System.Windows.Forms.Panel statsPanel;
        private System.Windows.Forms.Label allTimeLabel;
        private System.Windows.Forms.Label totalTodayLabel;
        private System.Windows.Forms.Label idleLabel;
        private System.Windows.Forms.Label activeLabel;

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
            this.components = new System.ComponentModel.Container();
            this.processDataGridView = new System.Windows.Forms.DataGridView();
            this.usageChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.categoryGroupBox = new System.Windows.Forms.GroupBox();
            this.allRadioButton = new System.Windows.Forms.RadioButton();
            this.gamesRadioButton = new System.Windows.Forms.RadioButton();
            this.learningRadioButton = new System.Windows.Forms.RadioButton();
            this.codingRadioButton = new System.Windows.Forms.RadioButton();
            this.entertainmentRadioButton = new System.Windows.Forms.RadioButton();
            this.statsPanel = new System.Windows.Forms.Panel();
            this.allTimeLabel = new System.Windows.Forms.Label();
            this.totalTodayLabel = new System.Windows.Forms.Label();
            this.idleLabel = new System.Windows.Forms.Label();
            this.activeLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.processDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usageChart)).BeginInit();
            this.categoryGroupBox.SuspendLayout();
            this.statsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // processDataGridView
            // 
            this.processDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.processDataGridView.Location = new System.Drawing.Point(20, 250);
            this.processDataGridView.Name = "processDataGridView";
            this.processDataGridView.Size = new System.Drawing.Size(400, 180);
            this.processDataGridView.TabIndex = 0;
            // 
            // usageChart
            // 
            this.usageChart.Location = new System.Drawing.Point(20, 20);
            this.usageChart.Name = "usageChart";
            this.usageChart.Size = new System.Drawing.Size(400, 200);
            this.usageChart.TabIndex = 1;
            // 
            // categoryGroupBox
            // 
            this.categoryGroupBox.Controls.Add(this.allRadioButton);
            this.categoryGroupBox.Controls.Add(this.gamesRadioButton);
            this.categoryGroupBox.Controls.Add(this.learningRadioButton);
            this.categoryGroupBox.Controls.Add(this.codingRadioButton);
            this.categoryGroupBox.Controls.Add(this.entertainmentRadioButton);
            this.categoryGroupBox.Location = new System.Drawing.Point(450, 20);
            this.categoryGroupBox.Name = "categoryGroupBox";
            this.categoryGroupBox.Size = new System.Drawing.Size(320, 120);
            this.categoryGroupBox.TabIndex = 2;
            this.categoryGroupBox.TabStop = false;
            this.categoryGroupBox.Text = "Categories";
            // 
            // allRadioButton
            // 
            this.allRadioButton.AutoSize = true;
            this.allRadioButton.Location = new System.Drawing.Point(20, 20);
            this.allRadioButton.Name = "allRadioButton";
            this.allRadioButton.Size = new System.Drawing.Size(36, 17);
            this.allRadioButton.TabIndex = 0;
            this.allRadioButton.TabStop = true;
            this.allRadioButton.Text = "All";
            this.allRadioButton.UseVisualStyleBackColor = true;
            // 
            // gamesRadioButton
            // 
            this.gamesRadioButton.AutoSize = true;
            this.gamesRadioButton.Location = new System.Drawing.Point(20, 43);
            this.gamesRadioButton.Name = "gamesRadioButton";
            this.gamesRadioButton.Size = new System.Drawing.Size(57, 17);
            this.gamesRadioButton.TabIndex = 1;
            this.gamesRadioButton.Text = "Games";
            this.gamesRadioButton.UseVisualStyleBackColor = true;
            // 
            // learningRadioButton
            // 
            this.learningRadioButton.AutoSize = true;
            this.learningRadioButton.Location = new System.Drawing.Point(100, 20);
            this.learningRadioButton.Name = "learningRadioButton";
            this.learningRadioButton.Size = new System.Drawing.Size(66, 17);
            this.learningRadioButton.TabIndex = 2;
            this.learningRadioButton.Text = "Learning";
            this.learningRadioButton.UseVisualStyleBackColor = true;
            // 
            // codingRadioButton
            // 
            this.codingRadioButton.AutoSize = true;
            this.codingRadioButton.Location = new System.Drawing.Point(100, 43);
            this.codingRadioButton.Name = "codingRadioButton";
            this.codingRadioButton.Size = new System.Drawing.Size(58, 17);
            this.codingRadioButton.TabIndex = 3;
            this.codingRadioButton.Text = "Coding";
            this.codingRadioButton.UseVisualStyleBackColor = true;
            // 
            // entertainmentRadioButton
            // 
            this.entertainmentRadioButton.AutoSize = true;
            this.entertainmentRadioButton.Location = new System.Drawing.Point(200, 20);
            this.entertainmentRadioButton.Name = "entertainmentRadioButton";
            this.entertainmentRadioButton.Size = new System.Drawing.Size(89, 17);
            this.entertainmentRadioButton.TabIndex = 4;
            this.entertainmentRadioButton.Text = "Entertainment";
            this.entertainmentRadioButton.UseVisualStyleBackColor = true;
            // 
            // statsPanel
            // 
            this.statsPanel.Controls.Add(this.allTimeLabel);
            this.statsPanel.Controls.Add(this.totalTodayLabel);
            this.statsPanel.Controls.Add(this.idleLabel);
            this.statsPanel.Controls.Add(this.activeLabel);
            this.statsPanel.Location = new System.Drawing.Point(450, 160);
            this.statsPanel.Name = "statsPanel";
            this.statsPanel.Size = new System.Drawing.Size(320, 120);
            this.statsPanel.TabIndex = 3;
            // 
            // allTimeLabel
            // 
            this.allTimeLabel.AutoSize = true;
            this.allTimeLabel.Location = new System.Drawing.Point(20, 20);
            this.allTimeLabel.Name = "allTimeLabel";
            this.allTimeLabel.Size = new System.Drawing.Size(60, 13);
            this.allTimeLabel.TabIndex = 0;
            this.allTimeLabel.Text = "All time: 0h";
            // 
            // totalTodayLabel
            // 
            this.totalTodayLabel.AutoSize = true;
            this.totalTodayLabel.Location = new System.Drawing.Point(20, 43);
            this.totalTodayLabel.Name = "totalTodayLabel";
            this.totalTodayLabel.Size = new System.Drawing.Size(85, 13);
            this.totalTodayLabel.TabIndex = 1;
            this.totalTodayLabel.Text = "Total today: 0m";
            // 
            // idleLabel
            // 
            this.idleLabel.AutoSize = true;
            this.idleLabel.Location = new System.Drawing.Point(20, 66);
            this.idleLabel.Name = "idleLabel";
            this.idleLabel.Size = new System.Drawing.Size(54, 13);
            this.idleLabel.TabIndex = 2;
            this.idleLabel.Text = "Idle: 0m";
            // 
            // activeLabel
            // 
            this.activeLabel.AutoSize = true;
            this.activeLabel.Location = new System.Drawing.Point(20, 89);
            this.activeLabel.Name = "activeLabel";
            this.activeLabel.Size = new System.Drawing.Size(65, 13);
            this.activeLabel.TabIndex = 3;
            this.activeLabel.Text = "Active: 0m";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 450);
            this.Controls.Add(this.processDataGridView);
            this.Controls.Add(this.usageChart);
            this.Controls.Add(this.categoryGroupBox);
            this.Controls.Add(this.statsPanel);
            this.Name = "Form1";
            this.Text = "TimeThread";
            ((System.ComponentModel.ISupportInitialize)(this.processDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usageChart)).EndInit();
            this.categoryGroupBox.ResumeLayout(false);
            this.categoryGroupBox.PerformLayout();
            this.statsPanel.ResumeLayout(false);
            this.statsPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}

