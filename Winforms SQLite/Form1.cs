// (c) Rou1997
// E-mail: rou1997<<bow-bow!>>bk<<point>>ru
// License: Public Domain | MIT
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// ------------------------------
using System.Data.SQLite;

namespace Winforms_SQLite
{
    public partial class Form1 : Form
    {
        string mDbPath = Application.StartupPath + "/getstarted.db";

        SQLiteConnection mConn;
        SQLiteDataAdapter mAdapter;
        DataTable mTable;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // -------------------- Connecting To DB --------------------
            // ----------------------------------------------------------
            // If DB Not Exists, it will be created.
            mConn = new SQLiteConnection("Data Source=" + mDbPath);

            // ----------------- Opening The Connection -----------------
            // ----------------------------------------------------------
            // I.e. Opening DB's File for Reading And Writing.
            // SQLiteDataAdapter cans do it automatically.
            // But, if you would also use SQLiteCommand, or GetSchema(),
            // you should Open DB Manually.
            mConn.Open();

            // ---------- Creating A Test Table, If Not Exists ----------
            // ----------------------------------------------------------
            // id        - Unique Counter - Key Field (Required in any table)
            // FirstName - Text
            // Age       - Integer
            using (SQLiteCommand mCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS [Test Table] (id INTEGER PRIMARY KEY AUTOINCREMENT, 'FirstName' TEXT, 'Age' INTEGER);", mConn))
            {
                mCmd.ExecuteNonQuery();
            }

            // ---------- Get All Tables From DB to ComboBox -----------
            // ---------------------------------------------------------
            // There "Tables" is a system table which contains info
            // about tables in DB.
            // "TABLE_NAME" field in "Tables" contains table names.
            using (DataTable mTables = mConn.GetSchema("Tables"))
            {
                for (int i = 0; i < mTables.Rows.Count; i++)
                {
                    CmbTables.Items.Add(mTables.Rows[i].ItemArray[mTables.Columns.IndexOf("TABLE_NAME")].ToString());
                }

                if (CmbTables.Items.Count > 0)
                {
                    CmbTables.SelectedIndex = 0; // Default selected index.
                }
            }
        }

        private void BtnSelectTable_Click(object sender, EventArgs e)
        {
            // --- Putting All Data From Selected Table To DataTable ---
            // ---------------------------------------------------------
            // In simply put, DataTable is just matrix (2-dimensional array)
            // which stores data of the table.
            mAdapter = new SQLiteDataAdapter("SELECT * FROM [" + CmbTables.Text + "]", mConn);
            mTable = new DataTable(); // Don't forget initialize!
            mAdapter.Fill(mTable);

            // ---------- Disabling Counter Field For Edition ----------
            // ---------------------------------------------------------
            // Because it can throw exception.
            if (mTable.Columns.Contains("id"))
            {
                mTable.Columns["id"].ReadOnly = true;
            }

            // ------------ Making DataBase Saving Changes -------------
            // ---------------------------------------------------------
            // SQLiteCommandBuilder authomatically generates
            // neccessary INSERT, UPDATE, DELETE SQL queries.
            // Next we just have to run the
            // mAdapter.Update(mTable);
            // and all changes in the table will be saved to DataBase.
            new SQLiteCommandBuilder(mAdapter);

            // ----------- Binding DataTable To DataGridView -----------
            // ---------------------------------------------------------
            // DataGridView visualizes DataTable's data in the window.
            dataGridView1.DataSource = mTable;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // -------- Saving Modified Data To Selected Table ---------
            // -------------------- On Form Closed ---------------------
            // ---------------------------------------------------------

            if (mAdapter == null) // If No Table Selected.
                return;

            mAdapter.Update(mTable);
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // - Preventing PEEESKY DataGridView Default Error Dialog --
            // ---- When we trying set a non-int value to int field ----
            // ---------------------------------------------------------
            // Just handle DataError event. It's enough.
            // But if you want, you can to show your own error message.
            // Not so peeeeesky, please!!! :)
        }
    }
}
