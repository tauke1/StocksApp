using Razor.Templating.Core;
using SelectPdf;
using StocksApp.Models;
using StocksApp.StocksApiClients;
using StocksApp.StocksApiClients.Models;
using StocksApp.StocksApiClients.Models.Enums;
using StocksApp.StocksApiClients.Tiingo;
using StocksApp.StocksApiClients.YahooFinance;
using StocksApp.Templates.Models;
using StocksApp.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StocksApp
{
    public partial class MainForm : Form
    {
        private readonly IHtmlGenerator _htmlGenerator;
        private readonly IHtmlToPdfConverter _htmlToPdfConverter;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly IYahooFinanceApiClient _yahooFinanceApiClient;
        private readonly ITiingoApiClient _tiingoApiClient;
        private readonly IEnumerable<IStocksApiClient> _avaiableStocksApiClients;
        private IStocksApiClient _selectedStockApiClient;
        private IStocksApiClient _selectedStockApiClientProperty
        {
            get { return _selectedStockApiClient; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                intervalComboBox.DataSource = value.GetValidIntervals().Select(i => new DateIntervalWithShift
                {
                    DateInterval = i.Key,
                    Shift = i.Value
                }).ToList();
                _selectedStockApiClient = value;
            }
        }

        public MainForm(IYahooFinanceApiClient yahooFinanceApiClient, ITiingoApiClient tiingoApiClient, IHtmlGenerator htmlGenerator, IHtmlToPdfConverter htmlToPdfConverter,
            IDateTimeUtility dateTimeUtility)
        {
            _tiingoApiClient = tiingoApiClient ?? throw new ArgumentNullException(nameof(tiingoApiClient)); ;
            _yahooFinanceApiClient = yahooFinanceApiClient ?? throw new ArgumentNullException(nameof(yahooFinanceApiClient));
            _htmlGenerator = htmlGenerator ?? throw new ArgumentNullException(nameof(htmlGenerator));
            _htmlToPdfConverter = htmlToPdfConverter ?? throw new ArgumentNullException(nameof(htmlToPdfConverter));
            _dateTimeUtility = dateTimeUtility ?? throw new ArgumentNullException(nameof(dateTimeUtility));
            _avaiableStocksApiClients = new List<IStocksApiClient> { _yahooFinanceApiClient, _tiingoApiClient };
            InitializeComponent();
            stocksApiClientComboBox.DataSource = _avaiableStocksApiClients;
            stocksApiClientComboBox.SelectedItem = _yahooFinanceApiClient;
            _selectedStockApiClientProperty = _yahooFinanceApiClient;
        }

        private void stocksApiClientComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            _selectedStockApiClientProperty = (IStocksApiClient)comboBox.SelectedItem;
        }

        private void periodShiftTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private async void ReceiveStocksHistoryButton_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeEnabledStatusesOfControls(false);
                ReceiveStocksHistoryRequest inputValues = GetInputValues();
                DateRange dateRange = new DateRange(inputValues.StartDate, inputValues.EndDate);
                DateIntervalWithShift dateIntervalWithShift = inputValues.IntervalWithShift;
                SpecificDateInterval dateInterval = new SpecificDateInterval(dateIntervalWithShift.DateInterval, dateIntervalWithShift.Shift);
                IList<StockInfo> stocksHistory = await _selectedStockApiClientProperty.GetStocksHistoryAsync(inputValues.Ticker, dateRange, dateInterval);
                if (stocksHistory.Count == 0)
                {
                    MessageBox.Show("Stocks API returned an empty result");
                    return;
                }

                IList<StockHistoryItem> stockHistoryItemsForTemplate = stocksHistory.Select(h => new StockHistoryItem
                {
                    Date = h.Date,
                    Open = h.Open,
                    Close = h.Close,
                    High = h.High,
                    Low = h.Low

                }).ToList();
                StockHistoryInfo stockHistoryInfoForTemplate = new StockHistoryInfo
                {
                    EndDate = inputValues.EndDate,
                    StartDate = inputValues.StartDate,
                    StockHistoryItems = stockHistoryItemsForTemplate,
                    Ticker = inputValues.Ticker
                };
                string html = await _htmlGenerator.GenerateHtml($"~/Views/StocksHistoryTemplate.cshtml", stockHistoryInfoForTemplate);
                byte[] pdfContent = _htmlToPdfConverter.ConvertHtmlToPdf(html);
                using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Choose folder to save stock history data as .pdf file";
                folderBrowserDialog.UseDescriptionForTitle = true;
                DialogResult result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string selectedPath = folderBrowserDialog.SelectedPath;
                    long currentTimestamp = _dateTimeUtility.GetEpochTime(DateTime.Now);
                    string filePath = $"{selectedPath}{inputValues.Ticker}-{currentTimestamp}.pdf";
                    File.WriteAllBytes(filePath, pdfContent);
                    MessageBox.Show($"File successfully saved at path {filePath}");
                    string argument = "/select, \"" + filePath + "\"";
                    Process.Start("explorer.exe", argument);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ChangeEnabledStatusesOfControls(true);
            }
        }

        private ReceiveStocksHistoryRequest GetInputValues()
        {
            var errors = new List<string>();
            string ticker = tickerNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(ticker))
                errors.Add("Ticker is required");

            DateIntervalWithShift dateIntervalWithShift = intervalComboBox.SelectedItem as DateIntervalWithShift;
            if (dateIntervalWithShift == null)
                errors.Add("Date interval with shift is invalid");

            bool startDateEnabled = startDateEnabledCheckBox.Checked;
            bool endDateEnabled = endDateEnabledCheckBox.Checked;
            DateTime? startDate = startDateEnabled ? startDateDatetimePicker.Value.Date : default(DateTime?);
            DateTime? endDate = endDateEnabled ? endDateDatetimePicker.Value.Date : default(DateTime?);
            if (startDate > endDate)
                errors.Add("Start Date must not be later than End Date");

            if (errors.Count > 0)
            {
                string errorsString = string.Join('\n', errors);
                throw new Exception(errorsString);
            }

            return new ReceiveStocksHistoryRequest
            {
                IntervalWithShift = dateIntervalWithShift,
                Ticker = ticker,
                StartDate = startDate,
                EndDate = endDate
            };
        }

        private void ChangeEnabledStatusesOfControls(bool status)
        {
            intervalComboBox.Enabled = status;
            tickerNameTextBox.Enabled = status;
            receiveStocksHistoryButton.Enabled = status;
            endDateEnabledCheckBox.Enabled = status;
            startDateEnabledCheckBox.Enabled = status;
            if (endDateEnabledCheckBox.Checked)
                endDateDatetimePicker.Enabled = status;

            if (startDateEnabledCheckBox.Checked)
                startDateDatetimePicker.Enabled = status;

            receiveStocksHistoryButton.Enabled = status;
            stocksApiClientComboBox.Enabled = status;
        }

        private void startDateEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (startDateEnabledCheckBox.Checked)
            {
                startDateDatetimePicker.Value = DateTime.Now.Date;
                startDateDatetimePicker.Enabled = true;
            }
            else
            {
                startDateDatetimePicker.ResetText();
                startDateDatetimePicker.Enabled = false;
            }
        }

        private void endDateEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (endDateEnabledCheckBox.Checked)
            {
                endDateDatetimePicker.Value = DateTime.Now.Date;
                endDateDatetimePicker.Enabled = true;
            }
            else
            {
                endDateDatetimePicker.ResetText();
                endDateDatetimePicker.Enabled = false;
            }
        }
    }
}
