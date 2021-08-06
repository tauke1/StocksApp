
namespace StocksApp
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
            this.tickerNameTextBox = new System.Windows.Forms.TextBox();
            this.tickerLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.intervalComboBox = new System.Windows.Forms.ComboBox();
            this.receiveStocksHistoryButton = new System.Windows.Forms.Button();
            this.startDateDatetimePicker = new System.Windows.Forms.DateTimePicker();
            this.endDateDatetimePicker = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.stocksApiClientComboBox = new System.Windows.Forms.ComboBox();
            this.startDateEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.endDateEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tickerNameTextBox
            // 
            this.tickerNameTextBox.Location = new System.Drawing.Point(87, 21);
            this.tickerNameTextBox.Name = "tickerNameTextBox";
            this.tickerNameTextBox.Size = new System.Drawing.Size(313, 23);
            this.tickerNameTextBox.TabIndex = 0;
            // 
            // tickerLabel
            // 
            this.tickerLabel.AutoSize = true;
            this.tickerLabel.Location = new System.Drawing.Point(14, 29);
            this.tickerLabel.Name = "tickerLabel";
            this.tickerLabel.Size = new System.Drawing.Size(38, 15);
            this.tickerLabel.TabIndex = 1;
            this.tickerLabel.Text = "Ticker";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Interval";
            // 
            // intervalComboBox
            // 
            this.intervalComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.intervalComboBox.FormattingEnabled = true;
            this.intervalComboBox.Location = new System.Drawing.Point(87, 59);
            this.intervalComboBox.Name = "intervalComboBox";
            this.intervalComboBox.Size = new System.Drawing.Size(313, 23);
            this.intervalComboBox.TabIndex = 3;
            // 
            // receiveStocksHistoryButton
            // 
            this.receiveStocksHistoryButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.receiveStocksHistoryButton.Location = new System.Drawing.Point(87, 220);
            this.receiveStocksHistoryButton.Name = "receiveStocksHistoryButton";
            this.receiveStocksHistoryButton.Size = new System.Drawing.Size(313, 23);
            this.receiveStocksHistoryButton.TabIndex = 7;
            this.receiveStocksHistoryButton.Text = "Receive";
            this.receiveStocksHistoryButton.UseVisualStyleBackColor = false;
            this.receiveStocksHistoryButton.Click += new System.EventHandler(this.ReceiveStocksHistoryButton_Click);
            // 
            // startDateDatetimePicker
            // 
            this.startDateDatetimePicker.Location = new System.Drawing.Point(87, 96);
            this.startDateDatetimePicker.Name = "startDateDatetimePicker";
            this.startDateDatetimePicker.Size = new System.Drawing.Size(313, 23);
            this.startDateDatetimePicker.TabIndex = 9;
            // 
            // endDateDatetimePicker
            // 
            this.endDateDatetimePicker.Location = new System.Drawing.Point(87, 137);
            this.endDateDatetimePicker.Name = "endDateDatetimePicker";
            this.endDateDatetimePicker.Size = new System.Drawing.Size(313, 23);
            this.endDateDatetimePicker.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Start Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "End Date";

            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 180);
            this.label5.Name = "label4";
            this.label5.Size = new System.Drawing.Size(54, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "API Client";

            // 
            // stocksApiClientComboBox
            // 
            this.stocksApiClientComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stocksApiClientComboBox.FormattingEnabled = true;
            this.stocksApiClientComboBox.Location = new System.Drawing.Point(87, 180);
            this.stocksApiClientComboBox.Name = "intervalComboBox";
            this.stocksApiClientComboBox.Size = new System.Drawing.Size(313, 23);
            this.stocksApiClientComboBox.TabIndex = 3;
            this.stocksApiClientComboBox.SelectedIndexChanged += new System.EventHandler(this.stocksApiClientComboBox_SelectedIndexChanged);

            // 
            // startDateEnabledCheckBox
            // 
            this.startDateEnabledCheckBox.AutoSize = true;
            this.startDateEnabledCheckBox.Checked = true;
            this.startDateEnabledCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.startDateEnabledCheckBox.Location = new System.Drawing.Point(407, 99);
            this.startDateEnabledCheckBox.Name = "startDateEnabledCheckBox";
            this.startDateEnabledCheckBox.Size = new System.Drawing.Size(68, 19);
            this.startDateEnabledCheckBox.TabIndex = 13;
            this.startDateEnabledCheckBox.Text = "Enabled";
            this.startDateEnabledCheckBox.UseVisualStyleBackColor = true;
            this.startDateEnabledCheckBox.CheckedChanged += new System.EventHandler(this.startDateEnabledCheckBox_CheckedChanged);
            // 
            // endDateEnabledCheckBox
            // 
            this.endDateEnabledCheckBox.AutoSize = true;
            this.endDateEnabledCheckBox.Checked = true;
            this.endDateEnabledCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.endDateEnabledCheckBox.Location = new System.Drawing.Point(407, 142);
            this.endDateEnabledCheckBox.Name = "endDateEnabledCheckBox";
            this.endDateEnabledCheckBox.Size = new System.Drawing.Size(68, 19);
            this.endDateEnabledCheckBox.TabIndex = 14;
            this.endDateEnabledCheckBox.Text = "Enabled";
            this.endDateEnabledCheckBox.UseVisualStyleBackColor = true;
            this.endDateEnabledCheckBox.CheckedChanged += new System.EventHandler(this.endDateEnabledCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 250);
            this.Controls.Add(this.endDateEnabledCheckBox);
            this.Controls.Add(this.startDateEnabledCheckBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.endDateDatetimePicker);
            this.Controls.Add(this.startDateDatetimePicker);
            this.Controls.Add(this.receiveStocksHistoryButton);
            this.Controls.Add(this.intervalComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tickerLabel);
            this.Controls.Add(this.tickerNameTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.stocksApiClientComboBox);
            this.Name = "MainForm";
            this.Text = "Ticker Stocks History";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tickerNameTextBox;
        private System.Windows.Forms.Label tickerLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox intervalComboBox;
        private System.Windows.Forms.Button receiveStocksHistoryButton;
        private System.Windows.Forms.DateTimePicker startDateDatetimePicker;
        private System.Windows.Forms.DateTimePicker endDateDatetimePicker;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox stocksApiClientComboBox;
        private System.Windows.Forms.CheckBox startDateEnabledCheckBox;
        private System.Windows.Forms.CheckBox endDateEnabledCheckBox;
    }
}