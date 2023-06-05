﻿using Krypton.Toolkit;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Gaze.BusinessLogic.SQLManagement
{

    public class InfoSec
    {
        #region Declarations
        private string SQLConnectionString = ConfigurationManager.AppSettings["SQLConnection"];

        public static string GlobalUsername
        {
            get; set;
        }

        public static string GlobalCustomerID
        {
            get; set;
        }

        public static string GlobalSelectedPolicyID
        {
            get; set;
        }

        public static string GlobalTaskID
        {
            get; set;
        }

        #endregion


        #region Methods
        /// <summary>
        /// Method used to log a user in
        /// </summary>
        /// <param name="username">The username of user</param>
        /// <param name="password">Password for users account</param>
        /// <returns></returns>
        public bool UserLogin(KryptonTextBox username, string password)
        {
            SqlConnection scon = new SqlConnection(SQLConnectionString);

            try
            {
                scon.Open();
                SqlCommand sqlCommand = new SqlCommand("dbo.SELECT_USER_FOR_LOGIN", scon);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("username", username.Text);
                sqlCommand.Parameters.AddWithValue("password", password);
                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read())
                {
                    GlobalUsername = username.Text;
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            finally
            {
                scon.Close();
            }

        }

        /// <summary>
        /// Checks if the user is an Application Administrator
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool isUserAdmin(string username)
        {
            SqlConnection scon = new SqlConnection(SQLConnectionString);
            try
            {
                scon.Open();
                SqlCommand sqlCommand = new SqlCommand("dbo.SELECT_USER_ADMIN_SP", scon);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("username", username);
                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read())
                {

                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                scon.Close();
            }
        }

        /// <summary>
        /// Method to check the database status 
        /// </summary>
        /// <returns>True if online or False if Offline</returns>
        public bool CheckDBStatus()
        {
            SqlConnection scon = new SqlConnection(SQLConnectionString);
            try
            {
                scon.Open();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public void InsertFootPrint()
        {

            SqlConnection scon = new SqlConnection(SQLConnectionString);

            try
            {
                scon.Open();
                SqlCommand sqlCommand = new SqlCommand("[sec].[INSERT_AUDIT_FOOTPRINT_SP]", scon);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Agent", InfoSec.GlobalUsername);
                sqlCommand.Parameters.AddWithValue("@CustomerID", InfoSec.GlobalCustomerID);
                SqlDataReader reader = sqlCommand.ExecuteReader();
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                scon.Close();
            }

        }

        #endregion


    }
}
