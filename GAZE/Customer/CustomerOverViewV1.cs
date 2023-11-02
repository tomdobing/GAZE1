﻿using Gaze.BusinessLogic.BillingManagement;
using Gaze.BusinessLogic.Config;
using Gaze.BusinessLogic.CustomerManagement;
using Gaze.BusinessLogic.PolicyManagement;
using Gaze.BusinessLogic.Security;
using Gaze.BusinessLogic.SQLManagement;
using Gaze.Security.Management;
using GAZE.Customer.Details;
using GAZE.Customer.Documents;
using GAZE.Customer.DPA;
using GAZE.Customer.Notes;
using GAZE.Customer.Policy;
using GAZE.Help;
using GAZE.Policy;
using Krypton.Toolkit;
using MetroFramework.Controls;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace GAZE.Customer
{
    public partial class CustomerOverViewV1 : KryptonForm
    {
        #region Declarations
        FormSettings FormSettings = new FormSettings();
        InfoSec InfoSec = new InfoSec();
        CustomerManagement CustomerManagement = new CustomerManagement();
        SQLManagement PolicySQLManagement = new SQLManagement();
        HomePage HomePage = new HomePage();
        SQLBilling SQLBilling = new SQLBilling();
        CustomerLogic CustomerLogic = new CustomerLogic();
        SQLDataAdmin SQLDataAdmin = new SQLDataAdmin();
        ControlAccessHelper ControlAccessHelper = new ControlAccessHelper();
        DataProtection DataProtection = new DataProtection();
        PolicySecurity PolicySecurity = new PolicySecurity();
        #endregion

        #region Methods
        public CustomerOverViewV1()
        {

            InitializeComponent();
            FormSettings.SetFormSettings(this);
            FormSettings.ChangeableFormSettings(this, "INDEV - Customer Overview - CustomerID:" + InfoSec.GlobalCustomerID);
            this.Palette = HomePage.kryptonManager1.GlobalPalette;
            ControlAccessHelper.DisableItemsMenuItems(PolMenStr);
           
            foreach (MetroTabPage tab in metroTabControl1.TabPages)
            {
                foreach (KryptonTextBox control1 in tab.Controls.OfType<KryptonTextBox>())
                {
                    control1.PaletteMode = PaletteMode.Global;
                    control1.ReadOnly = true;
                }
                foreach (KryptonMaskedTextBox item in tab.Controls.OfType<KryptonMaskedTextBox>())
                {
                    item.PaletteMode = PaletteMode.Global;
                    item.ReadOnly = true;
                }
            }
            if (string.IsNullOrEmpty(kryptonTextBox1.Text))
            {
                updateOverviewNoteToolStripMenuItem.Enabled = false;
                deleteNoteToolStripMenuItem.Enabled = false;
            }

        }

        private void CustomerOverViewV1_Load(object sender, EventArgs e)
        {

            ExecuteCustomerLoad();
            InfoSec.InsertFootPrint();
            if (string.IsNullOrEmpty(dpaPass_txt.Text))
            {

                KryptonMessageBox.Show("Warning - This customer has no DPA Password set \n\nPlease discuss with the customer regarding setting a DPA Password",
                                        "Security Warning",
                                        MessageBoxButtons.OK,
                                        KryptonMessageBoxIcon.Warning);
                createPasswordToolStripMenuItem.Enabled = true;
                updatePasswordToolStripMenuItem.Enabled = false;
                removePasswordToolStripMenuItem.Enabled = false;
            }
            else
            {
                createPasswordToolStripMenuItem.Enabled = false;
                updatePasswordToolStripMenuItem.Enabled = true;
                removePasswordToolStripMenuItem.Enabled = true;
            }
        }

        public void ExecuteCustomerLoad()
        {
            ///Load all customer policies
            CustomerManagement.GetCustomerPoliciesForOverview(metroGrid1);

            ///Wait for 1 second while Database finishes
            Thread.Sleep(1000);

            ///Get All Customer Details
            CustomerManagement.GetCustomerOverViewV1(CustName_txt, CustTitle_txt, FName_txt, CSurname_txt, CDOB_txt, ContactNum_txt, AltCont_txt, EmailAddress_txt,
                addrL1_txt, AddrL2_txt, Town_txt, postalcode_txt, country_txt, PolicyID_txt, PolStatus_lbl, DeactReas_txt, PolEffStart_txt, PolEndDate_txt, discount_txt, ProdName_txt, ProdDesc_txt
                , ProdActDate_txt, CustID_txt, ProdEndDate_txt, PolID_Txt, StatID_txt);
            
            ///Get Customers OverviewNote
            CustomerManagement.GetCustomerOverviewNote(kryptonTextBox1);

            ///Get the customers billing overview
            SQLBilling.GetOverviewBillingDetails(BillingID_txt, bilRef_txt, BillingType_txt, BillingFreq_txt, Yearly_txt,
                                                MTotal_txt, billingstatus_txt, accountNum_txt,
                                                sortcode_txt, NextBillDay_txt);

            ///Get the customers password if present !!!TODO!!!
            DataProtection.GetCustomersPassword(dpaPass_txt);


            ///select tab 1
            metroTabControl1.SelectedTab = metroTabPage1;

        }
         
        private void metroGrid1_SelectionChanged(object sender, EventArgs e)
        {
            string strMessage;
            //string Department = PolicySecurity.GetPolicyRedirectDepartment().ToString();
            if (metroGrid1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = metroGrid1.SelectedRows[0];
                string PolicyID = selectedRow.Cells["PolicyID"].Value.ToString();
                InfoSec.GlobalSelectedPolicyID = PolicyID;
                PolicySQLManagement.GetPolicyDataViaPolicyID(CustName_txt, CustTitle_txt, FName_txt, CSurname_txt, CDOB_txt, ContactNum_txt, AltCont_txt, EmailAddress_txt,
                addrL1_txt, AddrL2_txt, Town_txt, postalcode_txt, country_txt, PolicyID_txt, PolStatus_lbl, DeactReas_txt, PolEffStart_txt, PolEndDate_txt, ProdName_txt, ProdDesc_txt
                , ProdPrice_txt, ProdActDate_txt, CustID_txt, ProdEndDate_txt, PolID_Txt, StatID_txt);
                if (PolicySecurity.ReDirectExists() == true)
                {
                    strMessage = "Please direct all queries and amendments on this policy ";
                    strMessage = strMessage + "through to the " + PolicySecurity.GetPolicyRedirectDepartment();
                    strMessage = strMessage + " Department. Thank you.";
                    strMessage = strMessage + "\n\n" + "Additional Information: \n";
                    strMessage = strMessage + PolicySecurity.GetPolicyReDirectMessage();
                    KryptonMessageBox.Show(strMessage,
                                           "Policy ReDirect Alert",
                                           MessageBoxButtons.OK,
                                           KryptonMessageBoxIcon.Exclamation);


                }
            }
            

        }


        private void deleteNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string message = "Are you sure you wish to delete this Customers overview note?";
            string caption = "Are you sure?";
            DialogResult result;

            result = KryptonMessageBox.Show(message, caption, MessageBoxButtons.YesNo, KryptonMessageBoxIcon.Question, KryptonMessageBoxDefaultButton.Button3);
            if (result == DialogResult.Yes)
            {
                CustomerManagement.RemoveOverviewNote(); 
                ExecuteCustomerLoad();
                //Application.Exit();
            }
            if (result == DialogResult.No)
            {
                return;
            };
        }


        private void cancelBillingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "Are you sure you wish to cancel this customers Billing Account? \n\nThe Customer will need to provide their banking information" +
                " again in order to reactivate and continue with their product!";
            string caption = "Are you sure?";

            DialogResult result;

            result = KryptonMessageBox.Show(message, caption, MessageBoxButtons.YesNo, KryptonMessageBoxIcon.Question, KryptonMessageBoxDefaultButton.Button3);
            if (result == DialogResult.Yes)
            {
                SQLBilling.CancelCustomerBilling();
                ExecuteCustomerLoad();
            }
            if (result == DialogResult.No)
            {
                return;
            };
        }

        private void editBillingDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Billing.UpdateBankingDetails updateBankingDetails = new Billing.UpdateBankingDetails();
            updateBankingDetails.Show();
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustHistory custHistory = new CustHistory();
            custHistory.Show();
        }

        private void addNewPolicyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewCustomerPolicy newCustomerPolicy = new NewCustomerPolicy();
            newCustomerPolicy.Show();
        }

        private void updatePolicyStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PolicyStatus policyStatus = new PolicyStatus();
            policyStatus.Show();
        }

        private void createNewNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewNote newNote = new NewNote();
            newNote.Show();
        }

        private void notesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustNotes custNotes = new CustNotes();
            custNotes.Show();
        }

        private void lockCustomerPolicyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "You are about to restrict this customers policy. Are you sure you wish to continue?" + "\n\nRestricting this customers" +
                " Policy will prevent any access to this policy including reading of notes or billing. By Restricting a customers Policy this will also " +
                "cancel the billing and any associated policies. \n\nPlease advise the customer that if they require the account unrestricting they will need to" +
                "register for a new policy.";
            string caption = "Are you sure?";
            DialogResult result;

            result = KryptonMessageBox.Show(message, caption, MessageBoxButtons.YesNo, KryptonMessageBoxIcon.Warning, KryptonMessageBoxDefaultButton.Button3);
            if (result == DialogResult.Yes)
            {
                CustomerLogic.RestrictCustomerAccount(this);

            }
        }

        private void documentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomerDocuments customerDocuments = new CustomerDocuments();
            customerDocuments.Show();
        }
        
        
        private void acceptedFileTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AcceptedFileTypes acceptedFileTypes = new AcceptedFileTypes();
            acceptedFileTypes.Show();
        }

        private void tasksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer.Tasks.TaskOverview taskOverview = new Customer.Tasks.TaskOverview();
            taskOverview.Show();
        }





        #endregion

        private void createPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = KryptonMessageBox.Show("You are about to set a DPA Password for this customer - Would you like to Continue?", "Continue?",
                                        MessageBoxButtons.YesNo, KryptonMessageBoxIcon.Question, 0, 0,
                                        false, false, false, false, null);
            if (dialogResult == DialogResult.Yes)
            {
                string Password = KryptonInputBox.Show("Please enter the customer password exactly as told over the phone", "New Customer Password", default, "Password.........", default, default, default);
                DataProtection.CreateNewCustomerDPAPassword(Password);
            }
            else
            {
                return;
            }
                
        }

        private void updatePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {

            UpdateDPAPassword updateDPAPassword = new UpdateDPAPassword();
            updateDPAPassword.ShowDialog();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            UpdateAddressDetails updateAddressDetails = new UpdateAddressDetails();
            updateAddressDetails.ShowDialog();
        }

        private void redirectPolicyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PolicyRedirect policyRedirect = new PolicyRedirect();
            policyRedirect.ShowDialog();
        }
    }
}
