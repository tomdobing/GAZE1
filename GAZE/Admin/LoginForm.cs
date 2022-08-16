﻿using Gaze.BusinessLogic.Security;
using Gaze.BusinessLogic.SQLManagement;
using Gaze.BusinessLogic.Startup;
using System;
using System.Windows.Forms;

namespace GAZE.Admin
{

    public partial class LoginForm : MetroFramework.Forms.MetroForm
    {
        #region Declarations
        PreLoginChecks PreLoginChecks = new PreLoginChecks();
        readonly InfoSec infoSec = new InfoSec();
        readonly LoginFormSettings formSettings = new LoginFormSettings();
        #endregion

        #region Methods
        public LoginForm()
        {
            InitializeComponent();
            formSettings.SetFormValue(this);
            //metroProgressSpinner1.Hide();

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            SQLError_lbl.Text = "";
            password_txt.UseSystemPasswordChar = true;
            //password_txt.PasswordChar = '*';
            metroLabel2.Text = Application.ProductVersion;
        }


        private void MetroButton1_Click(object sender, EventArgs e)
        {


            if (PreLoginChecks.CheckSQLServerIsOnline(SQLError_lbl) == true)
            {
                if (infoSec.UserLogin(username_txt, password_txt) == true)
                {
                    SwitchBoard master = new SwitchBoard();
                    master.Show();
                    this.Close();
                }
                else
                {
                    string message = "Unknown username/password. Please try again";
                    string caption = "Invalid Login Details!";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(this, message, caption, buttons,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    password_txt.Clear();
                    password_txt.Focus();
                }
            }
            else if (PreLoginChecks.CheckSQLServerIsOnline(SQLError_lbl) == false)
            {
                //Application.Exit();
            }



        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                metroButton1.PerformClick();
            }
        }
        #endregion

    }
}

