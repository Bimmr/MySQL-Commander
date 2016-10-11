/**
 * 10/7/16
 * Randy Bimm
 * 
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MySQLCommander
{
    /// <summary>
    /// MySQL Commander's main class
    /// </summary>
    public partial class MySQLCommander : Form
    {
        /// <summary>
        /// Create a new instance of MySQLCommander
        /// </summary>
        public MySQLCommander()
        {
            InitializeComponent();
            getMySQL();
        }
        /// <summary>
        /// Get the latest version of MySQL Server installed on the computer
        /// </summary>
        /// <returns></returns>
        private bool getLatestMySQLServer()
        {
            //Check if the folder path exists
            string path = @"C:\Program Files\MySQL";
            if (!Directory.Exists(path))
                return false;

            //Get the latest version of MySQL Server
            double highest = 0.0;
            foreach (string directory in Directory.GetDirectories(path))
            {
                Console.WriteLine(directory);
                if (directory.Contains("MySQL Server"))
                {
                    double t = Double.Parse(directory.Replace(path+"\\MySQL Server", ""));
                    if (t > highest)
                        highest = t;
                }
            }
            //If the highest is less than 1, then we didn't find one
            if (highest < 1)
                return false;

            //Otherwise update the mysql path with the latest found
            else
                txtMySQL.Text = path + "\\MySQL Server " + highest +"\\bin";

            return true;
        }

        /// <summary>
        /// Get the MySQL path
        /// </summary>
        private void getMySQL()
        {
            if (!getLatestMySQLServer() && Directory.Exists(@"C:\xampp\mysql\bin"))
                txtMySQL.Text = @"C:\xampp\mysql\bin";
        }

        /// <summary>
        /// Call the function that creates the out file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOut_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text;
            string output = txtOutput.Text;
            string database = txtDatabase.Text;
            string mysql = txtMySQL.Text;
            string user = txtUser.Text;
            string password = txtPassword.Text;

            btnOut.Enabled = false;
            createOutFile(input, output, database, mysql, user, password);
            btnOut.Enabled = true;

        }

        /// <summary>
        /// Open a command prompt and create the out file
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="database"></param>
        /// <param name="mysql"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        private void createOutFile(string input, string output, string database, string mysql, string user, string password)
        {

            //Create the command prompt
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            //cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.WorkingDirectory = @mysql;
            cmd.Start();

            //Enter the inputs
            cmd.StandardInput.AutoFlush = true;
            cmd.StandardInput.WriteLine("mysql -uroot  " + (password != "" ? "-p" + password : "") + " < " + database);
            //MessageBox.Show("");
            cmd.StandardInput.WriteLine("mysql -uroot " + (password != "" ? "-p" + password : "") + " < " + input + " > " + output);
            //MessageBox.Show("");
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            MessageBox.Show("The file has successfully been created: \n" +
                "File Location: " + output,
                "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>
        /// Open a file explorer to find the .sql file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectInput_Click(object sender, EventArgs e)
        {
            DialogResult result = ofdInput.ShowDialog();

            string file = ofdInput.FileName;
            if (validateSQLFile(file))
                txtInput.Text = file;

        }

        /// <summary>
        /// Set the output text to equal the input text
        /// Replaces .sql with .out
        /// </summary>
        private void updateOutputText()
        {
            txtOutput.Text = txtInput.Text.Replace(".sql", ".out");
        }

        /// <summary>
        /// Open a file explorer to find the database .sql file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectDatabase_Click(object sender, EventArgs e)
        {
            DialogResult result = ofdInput.ShowDialog();

            string file = ofdInput.FileName;
            if (validateSQLFile(file))
                txtDatabase.Text = file;

        }

        /// <summary>
        /// Toggles the password char on/off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = (txtPassword.PasswordChar == '\0' ? '•' : '\0');
        }

        /// <summary>
        /// Checks if the selected file fits the requirements
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool validateSQLFile(string file)
        {
            bool valid = false;
            if (file == "openFileDialog1")
                file = null;
            if (file != null && !file.EndsWith(".sql"))
                MessageBox.Show("Invalid .SQL file.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (file != null && file.Contains(" "))
                MessageBox.Show("File path shouldn't contain a space.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (file == null)
                valid = false;
            else
                valid = true;
            return valid;
        }

        /// <summary>
        /// Update the output text when the input text gets changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            updateOutputText();
        }
    }
}
