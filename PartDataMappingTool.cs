using SampleDataGridViewPaging;
using SampleDataGridViewPaging.Statement.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SampleDataMappingTool
{
    public partial class Form1 : Form
    {
        public DataTable MyTable { get; set; }
        public DataTable dt = new DataTable();

        // Sanitized directory path placeholder
        string directoryPath = @"C:\SampleDirectory";

        DataTable Category2Table = new DataTable();
        DataTable Category3Table = new DataTable();
        DataTable ManufacturerTable = new DataTable();

        public Form1()
        {
            InitializeComponent();
            progressBar1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindAllTables();
            BindManufacturers();
            BindCategories();
            BindCategory2Ids();
        }

        private void BindAllTables()
        {
            var dbHelper = new DatabaseHelper();
            DataTable dt = dbHelper.GetTablesToShow();

            this.cbRawFilesList.DataSource = dt;
            DataRow dr = dt.NewRow();
            dr[0] = "SELECT Table...";
            dt.Rows.InsertAt(dr, 0);

            cbRawFilesList.DisplayMember = "TableName";
            cbRawFilesList.SelectedIndex = 0;
        }

        private void BindManufacturers()
        {
            var dbHelper = new DatabaseHelper();
            ManufacturerTable = dbHelper.GetManufacturers();
            this.cbManuList.DataSource = ManufacturerTable;
            DataRow dr = ManufacturerTable.NewRow();
            dr[0] = "-1";
            dr[1] = "SELECT MANUFACTURER";
            ManufacturerTable.Rows.InsertAt(dr, 0);

            cbManuList.DisplayMember = "Manufacturer";
            cbManuList.ValueMember = "PK_ID";
            cbManuList.SelectedIndex = 0;
        }

        private void BindCategories()
        {
            var dbHelper = new DatabaseHelper();
            Category2Table = dbHelper.GetCategories();

            this.cbCatList.DataSource = Category2Table;
            DataRow dr = Category2Table.NewRow();
            dr[0] = "-1";
            dr[1] = "SELECT CATEGORY";
            Category2Table.Rows.InsertAt(dr, 0);

            cbCatList.DisplayMember = "Category";
            cbCatList.ValueMember = "PK_ID";
            cbCatList.SelectedIndex = 0;
        }

        private void BindCategory2Ids()
        {
            string cat2Id = cbCatList.SelectedValue.ToString();
            this.cbCat2Id.DataSource = Category2Table;
            cbCat2Id.DisplayMember = "Category";
            cbCat2Id.ValueMember = "PK_ID";
            cbCat2Id.SelectedValue = cat2Id;
        }

        private void BindCategory3Ids()
        {
            var dbHelper = new DatabaseHelper();
            string cat2Id = cbCat2Id.SelectedValue.ToString();
            Category3Table = dbHelper.GetCategory3Ids(cat2Id);
            this.cbCat3Id.DataSource = Category3Table;
            DataRow dr = Category3Table.NewRow();
            dr[0] = "0";
            dr[1] = "SELECT Cat3ID";
            Category3Table.Rows.InsertAt(dr, 0);

            cbCat3Id.DisplayMember = "Display";
            cbCat3Id.ValueMember = "Cat3ID";
            cbCat3Id.SelectedIndex = 0;
        }

        private void RemoveDuplicateRows(ref DataTable table)
        {
            DataView dv = table.DefaultView;
            table = dv.ToTable(true);
        }

        private void RemoveUncleanParts(ref DataTable table)
        {
            var dbHelper = new DatabaseHelper();
            Collection<DataRow> rowsToDelete = new Collection<DataRow>();
            DataTable tempDeletedRows = table.Clone();

            foreach (DataRow dr in table.Rows)
            {
                string partNumber = dr["PartNo"].ToString();

                // Example sanitization logic
                if (partNumber.Contains("SAMPLE")) { rowsToDelete.Add(dr); continue; }

                partNumber = Regex.Replace(partNumber, @"[^0-9a-zA-Z]+", "");

                bool partExists = dbHelper.CheckIfPartExists(partNumber);
                if (partExists)
                {
                    rowsToDelete.Add(dr);
                }
            }

            foreach (DataRow dr in rowsToDelete)
            {
                tempDeletedRows.ImportRow(dr);
                table.Rows.Remove(dr);
            }
        }

        private void btnGetParts_Click(object sender, EventArgs e)
        {
            this.tbLoadManual.Text = "";
            if (cbRawFilesList.SelectedIndex == 0)
            {
                MessageBox.Show("Select the input file first!");
                return;
            }

            if (cbManuList.SelectedIndex == 0)
            {
                MessageBox.Show("Select a Manufacturer to get parts");
                return;
            }

            CleanAllPreviousDetails();
            progressBar1.Visible = true;
            string manufName = cbManuList.GetItemText(cbManuList.SelectedItem);
            string cat2Id = cbCatList.SelectedValue.ToString();
            string dbTblName = cbRawFilesList.GetItemText(cbRawFilesList.SelectedItem);
            string subQuery = tbSubQuery.Text.Trim();
            string[] myArgs = { manufName, cat2Id, dbTblName, subQuery };
            bgWorker.RunWorkerAsync(myArgs);
        }

        private int RunGetPartsQuery_LongTask(BackgroundWorker bw, string[] myArgs)
        {
            string manufName = myArgs[0];
            string cat2Id = myArgs[1];
            string dbTblName = myArgs[2];
            string subQuery = myArgs[3];

            var dbHelper = new DatabaseHelper();
            if (Int32.Parse(cat2Id) == -1)
            {
                dt = dbHelper.GetPartsByManufacturer(manufName, dbTblName, subQuery);
                RemoveDuplicateRows(ref dt);
                RemoveUncleanParts(ref dt);
            }
            else
            {
                dt = dbHelper.GetPartsByManufacturerAndCategory(manufName, cat2Id, dbTblName, subQuery);
                RemoveDuplicateRows(ref dt);
                RemoveUncleanParts(ref dt);
            }
            return 1;
        }

        // --- Automation Section (Selenium) ---
        // NOTE: Selenium automation targets a sample AI-powered search UI.
        // Selectors and flow are representative only and not tied to any production system.

        private IWebDriver LaunchBrowserDriver(string urlstring)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string driverFolder = Path.Combine(exePath, @"Drivers\");

            IWebDriver driver = new ChromeDriver(driverFolder);
            driver.Url = urlstring;
            driver.Navigate().GoToUrl(driver.Url);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            return driver;
        }

        private void CopilotSearchProcess(string searchPart, IWebDriver driver)
        {
            IWebElement shadowHost = driver.FindElement(By.CssSelector("cib-serp.cib-serp-main"));
            IWebElement actionBar = GetNestedElementInShadowRoot(shadowHost, "cib-action-bar#cib-action-bar-main");
            IWebElement textInput = GetNestedElementInShadowRoot(actionBar, "cib-text-input[chat-type='consumer']");

            IWebElement txtarea = GetNestedElementInShadowRoot(textInput, "textarea#searchbox");
            txtarea.SendKeys(searchPart);

            IWebElement btnSubmit = GetNestedElementInShadowRoot(actionBar, "button[description='Submit']");
            btnSubmit.Click();
        }

        private void ProcessResponsePage(IWebDriver driver)
        {
            IWebElement shadowHost = driver.FindElement(By.TagName("cib-serp"));
            IWebElement conversation = GetNestedElementInShadowRoot(shadowHost, "cib-conversation#cib-conversation-main");
            IWebElement chatTurn = GetNestedElementInShadowRoot(conversation, "cib-chat-turn[mode='conversation']");
            IWebElement messageGroup = GetNestedElementInShadowRoot(chatTurn, "cib-message-group[class='response-message-group']");
            IWebElement message = GetNestedElementInShadowRoot(messageGroup, "cib-message[mode='conversation']");
            IWebElement textBlock = GetNestedElementInShadowRoot(message, "div[class='ac-textBlock']");

            string txt = textBlock.Text;
            string cleanTxt = Regex.Replace(txt, @"[^\u0000-\u007F]+", string.Empty);
            string cleanerTxt = cleanTxt.Replace("\r", "").Replace("\n", " ");
            rtbResult.Text = cleanerTxt;
            driver.Close();
            driver.Quit();
        }

        private IWebElement GetNestedElementInShadowRoot(IWebElement shadowHost, string selector)
        {
            ISearchContext shadowRootContext = shadowHost.GetShadowRoot();
            return shadowRootContext.FindElement(By.CssSelector(selector));
        }

        private void CleanAllPreviousDetails()
        {
            tbTitle.Text = "";
            tbDescription.Text = "";
        }

#region Helper Methods to get ShadowRoot Elements

    private static ISearchContext GetShadowRootContext(IWebElement shadowHost)
    {
        ISearchContext shadowRootContext = shadowHost.GetShadowRoot();
        return shadowRootContext;
    }

    private IWebElement GetNestedElementInShadowRoot(IWebElement shadowHost, string selector)
    {
        ISearchContext shadowRootContext = shadowHost.GetShadowRoot();
        return shadowRootContext.FindElement(By.CssSelector(selector));
    }

#endregion

private string GetOrCreateDirectory(string dirPath)
{
    try
    {
        if (Directory.Exists(dirPath))
        {
            return new DirectoryInfo(dirPath).FullName;
        }
        else
        {
            DirectoryInfo di = Directory.CreateDirectory(dirPath);
            return di.FullName;
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Directory creation failed: " + ex.Message);
        return null;
    }
}

private bool CheckIfPartExistsInFile(string partNumber, string fullFilePath)
{
    if (File.Exists(fullFilePath))
    {
        string contents = File.ReadAllText(fullFilePath);
        return contents.ToLower().Contains(partNumber.ToLower());
    }
    return false;
}

private void cbCategoryList_SelectionChangeCommitted(object sender, EventArgs e)
{
    BindCategory2Ids();
    BindCategory3Ids();
}

private void cbCategory2Id_SelectionChangeCommitted(object sender, EventArgs e)
{
    BindCategory3Ids();
}

private void cbManufacturerList_SelectionChangeCommitted(object sender, EventArgs e)
{
    tbManufacturerId.Text = cbManuList.SelectedValue.ToString();
}

private void ClearAllFieldsAfterSave()
{
    tbTitle.Text = "";
    tbDescription.Text = "";
    tbPartNumber.Text = "";
    tbOEMNumber.Text = "";
    tbCost.Text = "";
    tbQty.Text = "1";
    cbCategory1.SelectedIndex = 0;
    rtbResult.Text = "";
    tbPartNumberVariant.Text = "";
    lblPartMessage.Text = "";
    tbPartNumberVariant.BackColor = Color.White;
}

private void btnCopyToDescription_Click(object sender, EventArgs e)
{
    if (rtbResult.Text.Trim().Length > 0)
    {
        tbDescription.Text = rtbResult.Text.Trim();
    }
}

private void btnSearchPart_Click(object sender, EventArgs e)
{
    string partNumber = tbPartNumberVariant.Text.Trim();
    partNumber = Regex.Replace(partNumber, @"[^0-9a-zA-Z]+", "");

    var dbHelper = new DatabaseHelper();
    bool partExists = dbHelper.CheckIfPartExistsLike(partNumber);

    if (partExists)
    {
        lblPartMessage.Text = partNumber + " - Similar Part Exists - Please verify";
        lblPartMessage.ForeColor = Color.Red;
        tbPartNumberVariant.BackColor = Color.Yellow;
    }
    else
    {
        lblPartMessage.Text = partNumber + " - Part does not exist";
        lblPartMessage.ForeColor = Color.Green;
        tbPartNumberVariant.BackColor = Color.LightGreen;
    }
}

private void btnManualMapping_Click(object sender, EventArgs e)
{
    ManualMappingWindow mappingWindow = new ManualMappingWindow();
    mappingWindow.ShowDialog();
    dt = mappingWindow.PassedDataTable;
    GetPartsManually();
}

private void GetPartsManually()
{
    if (dt.Rows.Count > 0)
    {
        this.tbLoadManual.Text = "y";
        lblLoading.Text = "New Parts Found = " + dt.Rows.Count.ToString();
        lblLoading.Visible = true;
        lblPartsOf.Text = "0/" + dt.Rows.Count.ToString();
        tbRowIndex.Text = "0";
        if (dt.Rows.Count <= 1)
        {
            btnLeft.Enabled = false;
        }
    }
}

private void LoadPartManuallyFromFile(DataRow dr, int rowIndex)
{
    if (dr != null)
    {
        var dbHelper = new DatabaseHelper();
        string manufacturerName = dr["ManufacturerName"].ToString().Trim();
        tbManufacturerId.Text = dbHelper.GetManufacturerId(manufacturerName);

        tbTitle.Text = dr["Description"].ToString();
        tbDescription.Text = dr["Description"].ToString();
        tbPartNumber.Text = dr["PartNo"].ToString();
        tbOEMNumber.Text = dr["PartNo"].ToString();
        tbCost.Text = "0";
        tbQty.Text = "1";
        cbCategory1.SelectedIndex = 0;
        cbWeight.SelectedIndex = cbWeight.FindStringExact("2.0");
        tbRowIndex.Text = rowIndex.ToString();
        lblPartsOf.Text = (rowIndex + 1).ToString() + "/" + dt.Rows.Count.ToString();
    }
}
