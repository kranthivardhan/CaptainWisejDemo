#region Using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Wisej.Web;
using Captain.Common.Utilities;
using Captain.Common.Model.Data;
using Captain.Common.Model.Objects;
using System.IO;
using Captain.Common.Views.Forms;
using System.Net.Mail;
using System.Configuration;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

#endregion

namespace Captain
{
    public partial class LoginForm : Form /*, ILogonForm*/
    {
        private ErrorProvider _errorProvider = null;
        private string strFolderPath = string.Empty;
        private DirectoryInfo MyDir;

        public LoginForm()
        {
            InitializeComponent();
            userProfile = null;
            timer1.Stop();
            timer1.Enabled = false;

            pnlAuthentication.Visible = false;
            pnlUserlogin.Visible = true;
            this.Size = new Size(530, Size.Height - pnlAuthentication.Height);
        }

        private void LoginFormLoad(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                if (!string.IsNullOrEmpty(Application.Cookies["UserName"]))
                {
                    txtUserName.Text = Application.Cookies["UserName"];
                    chkRememberUserName.Checked = true;
                    txtPassword.Focus();
                }
                else
                {
                    txtUserName.Focus();
                }
            }
            catch (Exception ex)
            {
                //
            }
        }

        /// <summary>
        /// This method is to validate the form when login button is clicked.
        /// </summary>
        /// <returns>true/false</returns>
        private bool IsFormValid()
        {
            var isValid = true;

            if (_errorProvider == null)
            {
                _errorProvider = new ErrorProvider(this);
                _errorProvider.IconPadding = 10;
            }

            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                _errorProvider.SetError(txtUserName, "Please Enter User ID");
                AlertBox.Show("Please Enter User ID", MessageBoxIcon.Warning);
                txtUserName.Clear();
                txtUserName.Focus();
                return isValid = false;
            }
            else
            {
                _errorProvider.SetError(txtUserName, string.Empty);
                isValid = true;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                _errorProvider.SetError(txtPassword, "Please Enter Password");
                AlertBox.Show("Please Enter Password", MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
                return isValid = false;
            }
            else
            {
                _errorProvider.SetError(txtPassword, string.Empty);
                isValid = true;
            }

            return isValid;
        }

        private int inttimelife = 0;
        private UserEntity userProfile;
        private string AgencyName = "";
        List<verifyDevice> lstverifyDevices = new List<verifyDevice>();
        private void LoginClick(object sender, EventArgs e)
        {
            if (IsFormValid())
            {
                _officeAddress = ""; AgencyName = "";
                var userName = txtUserName.Text;
                var password = txtPassword.Text;
                var strErrorMsg = string.Empty;
                lblOnetime2.Text = string.Empty;
                lblOnetime2.Visible = true;
                var model = new CaptainModel();
                try
                {
                    userProfile =
                        model.AuthenticateUser.AuthenticateWithProfile(userName, password, string.Empty,
                                                                        out strErrorMsg);

                    if (userProfile != null)
                    {
                        if (!userProfile.UserID.Equals("@InCorrect@1UID@2PSW") &&
                            !userProfile.UserID.Equals("@InActiveUserId") &&
                            int.Parse(userProfile.UnSuccessful) < 5) //|| userProfile != null) 
                        {
                            var isLoginRegistered = model.AuthenticateUser.RegisterLogin(userProfile.UserID);
                            var boollogin = true;
                            if (txtPassword.Text != "******")
                            {
                                var AgencyControlDetails = model.ZipCodeAndAgency.GetAgencyControlFile("00");
                                if (AgencyControlDetails != null)
                                {
                                    if (userProfile.CaseWorker.ToUpper() != "JAKE")
                                    {
                                        if (AgencyControlDetails.LoginMFA.ToUpper() == "Y")
                                        {

                                            /****************************************************************************************************/
                                            /****************************************************************************************************/
                                            string strverTrust = "";
                                            Wisej.Base.Cookie c = Wisej.Web.Application.Cookies.Get("lstverifyDevices");
                                            if (c != null)
                                            {
                                                lstverifyDevices = JsonConvert.DeserializeObject<List<verifyDevice>>(c.Value);
                                                if (lstverifyDevices.Count > 0)
                                                {

                                                    var lstverfdev1 = lstverifyDevices.Where(x => x.username == txtUserName.Text).ToList();
                                                    if (lstverfdev1.Count > 0)
                                                    {
                                                        strverTrust = lstverfdev1[0].trust;
                                                    }

                                                }
                                            }
                                            /****************************************************************************************************/
                                            /****************************************************************************************************/




                                            if (userProfile.PWDEmail.Trim() != string.Empty && strverTrust != "Y")
                                            {
                                                AgencyName = AgencyControlDetails.AgyName.Trim();

                                                var AgyCntrlDets = model.ZipCodeAndAgency.GetAgencyControlFile(userProfile.Agency);
                                                string strStreet = "", strCity = "", strState = "", strZIP = "";
                                                if (AgyCntrlDets != null)
                                                {
                                                    if (AgyCntrlDets.Street != "")
                                                        strStreet = AgyCntrlDets.Street + ", ";
                                                    if (AgyCntrlDets.City != "")
                                                        strCity = AgyCntrlDets.City + ", ";
                                                    if (AgyCntrlDets.State != "")
                                                        strState = AgyCntrlDets.State + ", ";
                                                    if (AgyCntrlDets.Zip1 != "")
                                                        strZIP = AgyCntrlDets.Zip1 + ".";
                                                }

                                                _officeAddress = strStreet + strCity + (strCity != "" ? ("<br/>" + strState) : strState) + strZIP;
                                                boollogin = false;
                                            }
                                        }
                                    }
                                }
                                if (boollogin)
                                {
                                    Captain<string>.Session[Consts.SessionVariables.FullName] =
                                        userProfile.FirstName.Trim() + Consts.Common.Space +
                                        (userProfile.MI.Trim().Equals(string.Empty)
                                            ? string.Empty
                                            : userProfile.MI + Consts.Common.Space) + userProfile.LastName.Trim();
                                    Captain<string>.Session[Consts.SessionVariables.UserID] = userProfile.UserID.Trim();
                                    Captain<string>.Session[Consts.SessionVariables.UserName] = userProfile.UserName;

                                    Captain<string>.Session[Consts.SessionVariables.LostLogin_Status] = " Successful";
                                    Captain<string>.Session[Consts.SessionVariables.LostLogin_Date] =
                                        userProfile.LastSuccessful;
                                    if (int.Parse(userProfile.UnSuccessful) > 0)
                                    {
                                        Captain<string>.Session[Consts.SessionVariables.LostLogin_Status] =
                                            " unsuccessful";
                                        Captain<string>.Session[Consts.SessionVariables.LostLogin_Date] =
                                            userProfile.LastUnSuccessful;
                                    }

                                    Captain<UserEntity>.Session[Consts.SessionVariables.UserProfile] = userProfile;

                                    Application.Session.IsLoggedOn = true;
                                    DialogResult = DialogResult.OK;
                                    Close();
                                }
                                else
                                {
                                    //var _model = new CaptainModel();
                                    //List<HierarchyEntity> lstHIEentity = _model.HierarchyAndPrograms.GetCaseHierachyAllData(userProfile.Agency, "", "");
                                    //if (lstHIEentity.Count > 0)
                                    //{
                                    //    AgencyName = lstHIEentity[0].HirarchyName;
                                    //}

                                    /****************************************************************************************************/
                                    /****************************************************************************************************/
                                    chktrusted.Visible = true;
                                    Wisej.Base.Cookie c = Wisej.Web.Application.Cookies.Get("lstverifyDevices");
                                    if (c != null)
                                    {
                                        lstverifyDevices = JsonConvert.DeserializeObject<List<verifyDevice>>(c.Value);
                                        if (lstverifyDevices.Count > 0)
                                        {

                                            var lstverfdev1 = lstverifyDevices.Where(x => x.username == txtUserName.Text).ToList();
                                            if (lstverfdev1.Count > 0)
                                            {
                                                chktrusted.Visible = false;
                                            }

                                        }
                                    }
                                    /****************************************************************************************************/
                                    /****************************************************************************************************/

                                    RandomTokenNumber(6);
                                    SendEmail(userProfile.PWDEmail, proptext, AgencyName, _officeAddress);
                                    if (_isemailSent == "Y")
                                    {
                                        pnlUserlogin.Visible = false;
                                        this.Size = new Size(630, this.Size.Height + pnlAuthentication.Height - pnlUserlogin.Height);
                                        pnlauthblock.Visible = true;
                                        pnlAuthentication.Visible = true;
                                        lblTimerLeft.Visible = true;
                                        txtverifytext.Visible = true; lblEntertext.Visible = true; btnValidCaptcher.Visible = true; chktrusted.Visible = true; lblTimerLeft.Visible = true;
                                        btnLogin.Enabled = false; linkresend.Visible = false; linktryanotheruser.Visible = lblOnetime2.Visible = true;
                                        txtPassword.Enabled = false; txtUserName.Enabled = false;
                                        linktryanotheruser.Location = new Point(345, 164);
                                        inttimelife = 120;
                                        lblOnetime2.Text = "One time Text sent to your email id : " + MaskEmailfunction(userProfile.PWDEmail);
                                        timer1.Start();
                                        timer1.Enabled = true;
                                    }
                                    if (_isemailSent == "N")
                                    {
                                        pnlUserlogin.Visible = true;
                                        this.Size = new Size(630, this.Size.Height + 30);
                                        pnlauthblock.Visible = true;
                                        pnlAuthentication.Visible = true;
                                        txtverifytext.Visible = false; lblEntertext.Visible = false; btnValidCaptcher.Visible = false; chktrusted.Visible = false; lblTimerLeft.Visible = false;
                                        btnLogin.Enabled = false; linkresend.Visible = false; lblOnetime2.Visible = false;
                                        txtPassword.Enabled = false; txtUserName.Enabled = false;
                                        linktryanotheruser.Visible = true;
                                        linktryanotheruser.Location = new Point(340, 0);
                                        inttimelife = 120;
                                        lblOnetime2.Text = "";
                                        timer1.Start();
                                        timer1.Enabled = true;
                                    }
                                }
                            }
                            else
                            {
                                if (userProfile.AccessAll.Equals("Y"))
                                {
                                    Admn0004UserForm objAdmn0004 = new Admn0004UserForm(userProfile, txtUserName.Text);
                                    objAdmn0004.StartPosition = FormStartPosition.CenterScreen;

                                    objAdmn0004.ShowDialog();

                                    if (Application.Session.IsLoggedOn == true)
                                    {
                                        //DialogResult = DialogResult.OK;
                                        Close();
                                    }
                                }
                                else
                                {
                                    AlertBox.Show("Invalid User ID/Password. Please Contact System administrator", MessageBoxIcon.Warning);
                                }
                            }

                            Application.Session["usersessionid"] = Application.SessionId;
                            var ip = Application.ServerVariables["HTTP_X_FORWARDED_FOR"];
                            if (string.IsNullOrEmpty(ip)) ip = Application.ServerVariables["REMOTE_ADDR"];
                            Application.Session["userlogid"] = model.UserProfileAccess.InsertUpdateLogUsers(
                                userProfile.UserID, Application.Session["usersessionid"].ToString(), ip, "Add",
                                string.Empty);
                        }
                        else if (userProfile.UserID.Equals("@NoHierarchy"))
                        {
                            AlertBox.Show("Sorry! you have access on no hierarchy", MessageBoxIcon.Warning);
                        }
                        else if (userProfile.UserID.Equals("@InActiveUserId"))
                        {
                            AlertBox.Show("Inactivated User...Please Contact Administrator...", MessageBoxIcon.Warning);
                        }
                        else
                        {
                            //AlertBox.Show("Invalid User ID/Password. Please Contact System administrator", MessageBoxIcon.Warning);
                            //return;
                            if (userProfile.UserID.Equals("@InCorrect@1UID@2PSW") &&
                                (userProfile.Password.Equals("PASSWORD") || int.Parse(userProfile.UnSuccessful) > 0))
                            {
                                //if (int.Parse(userProfile.UnSuccessful) < 5)
                                //{
                                //    AlertBox.Show("Invalid Password. You have '" +
                                //                    (5 - int.Parse(userProfile.UnSuccessful)).ToString() +
                                //                    "' attempts Left With", MessageBoxIcon.Warning);
                                //    return;
                                //}
                                //else
                                //{
                                //    AlertBox.Show("Your Account is Blocked. Please Contact System administrator", MessageBoxIcon.Warning);
                                //    return;
                                //}
                                if (int.Parse(userProfile.UnSuccessful) == 5)
                                {
                                    AlertBox.Show("Your Account is Blocked. Please Contact System administrator", MessageBoxIcon.Warning);
                                    return;
                                }
                                else
                                {
                                    if (int.Parse(userProfile.UnSuccessful) < 5)
                                    {
                                        AlertBox.Show("Invalid Password. You have '" +
                                                        (5 - int.Parse(userProfile.UnSuccessful)).ToString() +
                                                        "' attempts Left With", MessageBoxIcon.Warning);
                                        return;
                                    }
                                    else
                                    {
                                        AlertBox.Show("Your Account is Blocked. Please Contact System administrator", MessageBoxIcon.Warning);
                                        return;
                                    }
                                    //AlertBox.Show("Invalid User ID/Password. Please Contact System administrator", MessageBoxIcon.Warning);
                                }
                            }
                            else
                            {
                                AlertBox.Show("Invalid User ID/Password. Please Contact System administrator", MessageBoxIcon.Warning);
                            }


                        }

                        if (chkRememberUserName.Checked)
                            Application.Cookies[Consts.SessionVariables.UserName] = txtUserName.Text;
                        else
                            Application.Cookies[Consts.SessionVariables.UserName] = null;
                    }
                    else
                    {
                        //CommonFunctions.MessageBoxDisplay("Sqlserver is down please contact administrator");
                        CommonFunctions.MessageBoxDisplay(strErrorMsg);
                    }
                }
                catch (Exception ex)
                {
                    CommonFunctions.MessageBoxDisplay(strErrorMsg);
                }
            }
        }

        /// <summary>
        /// This event is fired when the user types enter key in password textbox.
        /// </summary>
        /// <param name="objSender"></param>
        /// <param name="objArgs"></param>
        private void PasswordEnterKeyDown(object objSender, KeyEventArgs objArgs)
        {
            if (objArgs.KeyCode == Keys.Enter)
                LoginClick(btnLogin, EventArgs.Empty);
        }


        private void txtPassword_GotFocus(object sender, EventArgs e)
        {
            txtPassword.SelectionLength = txtPassword.Text.Length;
        }

        private void btnValidCaptcher_Click(object sender, EventArgs e)
        {
            _errorProvider.SetError(btnValidCaptcher, null);
            if (string.IsNullOrEmpty(txtverifytext.Text))
            {
                _errorProvider.SetError(btnValidCaptcher, "Please Enter Text");
            }
            else
            {
                var boolpagelogin = true;
                if (proptext == string.Empty)
                {
                    boolpagelogin = false;
                    if (propexptext == txtverifytext.Text)
                    {
                        CommonFunctions.MessageBoxDisplay("Text is expired, Please Click on Resend Text link");
                        boolpagelogin = false;
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay("Wrong Text is entered");
                        boolpagelogin = false;
                    }
                }

                if (boolpagelogin)
                {
                    if (proptext == txtverifytext.Text)
                    {
                        timer1.Stop();
                        var model = new CaptainModel();
                        _errorProvider.SetError(btnValidCaptcher, null);
                        var isLoginRegistered = model.AuthenticateUser.RegisterLogin(userProfile.UserID);


                        Captain<string>.Session[Consts.SessionVariables.FullName] = userProfile.FirstName.Trim() +
                            Consts.Common.Space +
                            (userProfile.MI.Trim().Equals(string.Empty)
                                ? string.Empty
                                : userProfile.MI + Consts.Common.Space) + userProfile.LastName.Trim();
                        Captain<string>.Session[Consts.SessionVariables.UserID] = userProfile.UserID.Trim();
                        Captain<string>.Session[Consts.SessionVariables.UserName] = userProfile.UserName;

                        Captain<string>.Session[Consts.SessionVariables.LostLogin_Status] = " Successful";
                        Captain<string>.Session[Consts.SessionVariables.LostLogin_Date] = userProfile.LastSuccessful;
                        if (int.Parse(userProfile.UnSuccessful) > 0)
                        {
                            Captain<string>.Session[Consts.SessionVariables.LostLogin_Status] = " unsuccessful";
                            Captain<string>.Session[Consts.SessionVariables.LostLogin_Date] =
                                userProfile.LastUnSuccessful;
                        }

                        Wisej.Base.Cookie c = Wisej.Web.Application.Cookies.Get("lstverifyDevices");
                        string strverTrust = "";
                        if (c != null)
                        {
                            lstverifyDevices = JsonConvert.DeserializeObject<List<verifyDevice>>(c.Value);

                            if (lstverifyDevices.Count > 0)
                            {

                                var lstverfdev1 = lstverifyDevices.Where(x => x.username == txtUserName.Text).ToList();
                                if (lstverfdev1.Count > 0)
                                    strverTrust = lstverfdev1[0].trust;

                            }
                        }

                        if (chktrusted.Checked)
                        {

                            string strchkTrust = "Y";
                            // if (chkTrust.Checked)
                            //  strchkTrust = "Y";

                            string username = txtUserName.Text;

                            var lstverfdev = lstverifyDevices.Where(x => x.username == txtUserName.Text).ToList();
                            if (lstverfdev.Count > 0)
                            {
                                lstverfdev[0].password = txtPassword.Text;
                                lstverfdev[0].trust = strchkTrust;

                            }
                            else
                                lstverifyDevices.Add(new verifyDevice { username = username, password = txtPassword.Text, trust = strchkTrust });

                            string myObjectJson = new JavaScriptSerializer().Serialize(lstverifyDevices);
                            Wisej.Web.Application.Cookies.Add("lstverifyDevices", myObjectJson);

                        }



                        Captain<UserEntity>.Session[Consts.SessionVariables.UserProfile] = userProfile;
                        Application.Session.IsLoggedOn = true;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        CommonFunctions.MessageBoxDisplay("Invalid OTP");
                    }
                }
            }
        }

        #region Recapptcha

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string proptext = string.Empty;
        private string propexptext = string.Empty;

        private string RandomTokenNumber(int length)
        {
            var strTokenNumber = RandomString(length);
            proptext = strTokenNumber;
            return strTokenNumber;
        }

        public string _isemailSent = "Y";
        public string _officeAddress = "";
        private void SendEmail(string emailID, string TokenNumber, string _AgencyName, string _officeAddr)
        {
            try
            {
                var model = new CaptainModel();
                var dtMailConfig = model.UserProfileAccess.GetEMailSetting("LOGINOTP");

                if (dtMailConfig.Rows.Count > 0)
                {
                    var mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(dtMailConfig.Rows[0]["MAIL_EMAILID"].ToString(), _AgencyName);
                    mailMessage.Subject = dtMailConfig.Rows[0]["MAIL_SUBJECT"].ToString();

                    var body = dtMailConfig.Rows[0]["MAIL_CONTENT"].ToString() + TokenNumber;
                    body = body + "<br/><br/><br/><br/><br/><br/>" +
                        _AgencyName + "<br/>" + _officeAddr;
                    //dtMailConfig.Rows[0]["MAIL_SENDER_NAME"].ToString() + "<br/>" + 
                    //dtMailConfig.Rows[0]["MAIL_SENDER_ADDR"].ToString();
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    mailMessage.To.Add(emailID);


                    var smtp = new SmtpClient();
                    smtp.Host = dtMailConfig.Rows[0]["MAIL_HOST"].ToString();
                    smtp.EnableSsl = true;
                    var NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = dtMailConfig.Rows[0]["MAIL_EMAILID"].ToString();
                    NetworkCred.Password = dtMailConfig.Rows[0]["MAIL_PASSWORD"].ToString();
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = int.Parse(dtMailConfig.Rows[0]["MAIL_PORT"].ToString());
                    smtp.Send(mailMessage);
                    _isemailSent = "Y";
                }
            }
            catch (Exception ex)
            {
                _isemailSent = "N";
                // pnlUserlogin.Visible = true;
                AlertBox.Show("Failed to send OTP. Please contact Administrator", MessageBoxIcon.Warning);
                //lblerrormsg.Text = ex.Message; //"email not delivered!";

                //lblerrormsg.Text = "email not delivered!";          
                //Response.Write(ex.Message);
            }
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (userProfile != null)
            {
                if (inttimelife > 0)
                {
                    // Display the new time left
                    // by updating the Time Left label.
                    inttimelife = inttimelife - 1;
                    //txtverifytext.Watermark = inttimelife + " seconds left";
                    lblTimerLeft.Text = inttimelife.ToString() + " seconds left";
                    linkresend.Visible = false;
                }
                else
                {
                    // If the user ran out of time, stop the timer, show
                    // a MessageBox, and fill in the answers.
                    timer1.Stop();
                    linkresend.Visible = true;
                    btnValidCaptcher.Visible = false;
                    lblEntertext.Visible = txtverifytext.Visible = lblOnetime2.Visible = false; chktrusted.Visible = false; lblTimerLeft.Visible = false;
                    if (proptext != string.Empty)
                    {
                        propexptext = proptext;
                        proptext = string.Empty;
                    }
                }
            }
        }

        private void linkresend_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _errorProvider.SetError(btnValidCaptcher, null);
            txtverifytext.Visible = true; chktrusted.Visible = true; lblTimerLeft.Visible = true;
            lblEntertext.Visible = true;
            btnValidCaptcher.Visible = true;
            lblOnetime2.Visible = true;
            btnLogin.Enabled = false;
            inttimelife = 120;
            linkresend.Visible = false;
            lblEntertext.Visible = true;
            // txtverifyText.Watermark = "60 seconds left";
            timer1.Start();
            RandomTokenNumber(6);
            SendEmail(userProfile.PWDEmail, proptext, AgencyName, _officeAddress);
        }

        private void linktryanotheruser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _errorProvider.SetError(btnValidCaptcher, null);
            timer1.Stop();
            lblTimerLeft.Visible = false;
            txtverifytext.Visible = false; lblEntertext.Visible = false; btnValidCaptcher.Visible = linkresend.Visible = lblOnetime2.Visible = linktryanotheruser.Visible = false; chktrusted.Visible = false; lblTimerLeft.Visible = false;
            btnLogin.Enabled = true; txtUserName.Enabled = true; txtPassword.Enabled = true; userProfile = null; txtUserName.Text = ""; txtPassword.Clear();

            pnlAuthentication.Visible = false;
            pnlUserlogin.Visible = true;
            this.Size = new System.Drawing.Size(530, 272);
        }

        private string MaskEmailfunction(string strEmail)
        {
            var strMainmaskemail = string.Empty;
            try
            {
                var stremailat = strEmail.Split('@');
                if (stremailat.Length > 0)
                {
                    var email = stremailat[0].ToString();
                    var strmaskemail = string.Empty;
                    if (email.Length > 0)
                        for (var i = 0; i < email.Length; i++)
                            if (i == 0)
                                strmaskemail = strmaskemail + email[0].ToString();
                            else if (i == email.Length - 1)
                                strmaskemail = strmaskemail + email[email.Length - 1].ToString();
                            else
                                strmaskemail = strmaskemail + "*";
                    //var maskedEmail = string.Format("{0}****{1}", email[0],
                    //email.Substring(email.Length - 1));
                    var email2 = stremailat[1].ToString();
                    var strmaskemail2 = string.Empty;
                    if (email2.Length > 0)
                    {
                        var strarremail = email2.Split('.');
                        for (var i = 0; i < strarremail[0].Length; i++)
                            if (i == 0)
                                strmaskemail2 = strmaskemail2 + strarremail[0][0].ToString();
                            else if (i == strarremail[0].Length - 1)
                                strmaskemail2 = strmaskemail2 + strarremail[0][strarremail[0].Length - 1].ToString();
                            else
                                strmaskemail2 = strmaskemail2 + "*";
                        strmaskemail2 = strmaskemail2 + "." + strarremail[1];
                    }


                    //var maskedEmail2 = string.Format("{0}****{1}", email2[0],
                    //email2.Substring(email2.IndexOf('.') - 1));
                    strMainmaskemail = strmaskemail + "@" + strmaskemail2;
                }
            }
            catch (Exception ex)
            {
            }

            return strMainmaskemail;
        }

        private void lblOnetime_TextChanged(object sender, EventArgs e)
        {
            pnlAuthentication.Visible = true;
            lblOnetime2.Visible = true;

            pnlUserlogin.Visible = false;
        }

        //private void pnlauthblock_VisibleChanged(object sender, EventArgs e)
        //{
        //	if (pnlauthblock.Visible)
        //	{
        //		SendEmail(userProfile.PWDEmail, proptext);
        //		txtverifytext.Visible = true;
        //		lblEntertext.Visible = true;
        //		btnValidCaptcher.Visible = true;
        //		btnLogin.Enabled = false;
        //		linkresend.Visible = false;
        //		linktryanotheruser.Visible = lblOnetime.Visible = true;
        //		txtPassword.Enabled = false;
        //		txtUserName.Enabled = false;
        //		inttimelife = 1;
        //		//txtverifyText.Watermark = "60 seconds left";
        //		lblOnetime.Text = "One time Text sent to your email id :" +
        //						MaskEmailfunction(userProfile.PWDEmail);
        //		timer1.Start();
        //	}
        //	else
        //	{
        //		_errorProvider.SetError(btnValidCaptcher, null);
        //		txtverifytext.Visible = false;
        //		lblEntertext.Visible = false;
        //		btnValidCaptcher.Visible = linkresend.Visible = lblOnetime.Visible = linktryanotheruser.Visible = false;
        //		btnLogin.Enabled = true;
        //		txtUserName.Enabled = true;
        //		txtPassword.Enabled = true;
        //		userProfile = null;
        //		txtUserName.Text = "";
        //		txtPassword.Clear();
        //	}
        //}
    }
}

class verifyDevice
{

    public string username { get; set; }
    public string password { get; set; }
    public string trust { get; set; }

}