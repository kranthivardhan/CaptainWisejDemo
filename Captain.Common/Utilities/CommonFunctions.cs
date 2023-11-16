/****************************************************************************************************
 * Class Name    : CommonFunctions
 * Author        : Chitti
 * Created Date  : 
 * Version       : 1.0.0
 * Description   : This class has common functions,which used in across the application
 ****************************************************************************************************/

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Wisej.Web;
using Captain.Common.Interfaces;
using Captain.Common.Model.Data;
using Captain.Common.Model.Objects;
using Captain.Common.Model.Parameters;
using Captain.Common.Views.Forms.Base;
using Captain.Common.Views.UserControls;
using Captain.Common.Views.UserControls.Base;
using Captain.Common.Utilities;
using System.Runtime.InteropServices;
using Captain.Common.Exceptions;
using Captain.DatabaseLayer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

#endregion

namespace Captain.Common.Utilities
{
    public static class CommonFunctions
    {
        #region Variables

        private static object _serialization = new object();

        #endregion

        #region Private Methods

        /// <summary>
        /// GetUserNameFormatted
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="templateString"></param>
        /// <returns></returns>
        private static string GetUserNameFormatted(UserEntity userEntity, string templateString)
        {
            string userTemplate = templateString.ToLower();

            userTemplate = userTemplate.Replace(Consts.Parameters.FirstName, userEntity.FirstName);
            userTemplate = userTemplate.Replace(Consts.Parameters.LastName, userEntity.LastName);
            userTemplate = userTemplate.Replace(Consts.Parameters.MiddleName, userEntity.MI);
            //userTemplate = userTemplate.Replace(Consts.Parameters.Title, userEntity.);
            //userTemplate = userTemplate.Replace(Consts.Parameters.Email, userEntity.Email);
            userTemplate = userTemplate.Replace(Consts.Parameters.UserName, userEntity.UserName);
            userTemplate = userTemplate.Replace(Consts.Parameters.UserID, userEntity.UserID);

            return userTemplate.Trim();
        }

        private static string IncrementCharacter(string currentString)
        {
            char[] allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] givenCharacters = currentString.ToCharArray();
            int index = currentString.Length - 1;
            char lastChar = givenCharacters[index];
            for (int i = 0; i < allowedChars.Length; i++)
            {
                if (allowedChars[i] == lastChar)
                {
                    if (i == allowedChars.Length - 1)
                    {
                        givenCharacters[index] = allowedChars[0];
                        if (givenCharacters.Length > 1)
                        {
                            string stepUpPreceedingCharacters = IncrementCharacter(new string(givenCharacters.Take(givenCharacters.Length - 1).ToArray()));
                            givenCharacters = (stepUpPreceedingCharacters + givenCharacters[index]).ToCharArray();
                        }
                        else
                        {
                            givenCharacters = new char[] { allowedChars[0], givenCharacters[index] };
                        }
                    }
                    else
                    {
                        givenCharacters[index] = allowedChars[i + 1];
                    }
                    break;
                }
            }
            return new string(givenCharacters);
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates a timer and initializes it.
        /// </summary>
        /// <param name="interval">The interval of the timer.</param>
        /// <param name="eventHandler">The ticker event handler.</param>
        /// <param name="container">The container for the timer.</param>
        /// <returns>Returns the timer object.</returns>
        public static Timer AddTimedEvent(int interval, EventHandler eventHandler, IContainer container)
        {
            try
            {
                Timer timedEvent = new Timer(container);
                timedEvent.Interval = interval;
                timedEvent.Tick += eventHandler;
                timedEvent.Enabled = true;
                timedEvent.Start();

                return timedEvent;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a date value and formats using the specified format.
        /// </summary>
        /// <param name="dateValue">The date value to format.</param>
        /// <param name="format">The format to use.</param>
        /// <returns>Returns a formatted date value if it is a valid date. Otherwise, returns an empty string.</returns>
        public static string ChangeDateFormat(string dateValue, string currentFormat, string newFormat)
        {
            if (string.IsNullOrEmpty(dateValue)) { return string.Empty; }
            string value = string.Empty;
            DateTime dateTime;

            bool success = DateTime.TryParseExact(dateValue, currentFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
            if (!success) { success = DateTime.TryParse(dateValue, out dateTime); }
            if (success) { value = dateTime.ToString(newFormat); }

            return value;
        }

        /// <summary> By: Yeswanth 
        /// Gets a string and returns Datatable.
        /// </summary>
        /// <param XML_To_Table>XML string .</param>
        /// <returns>Returns Table after converting is the input string is valid XML data, else returns an empty string.</returns>
        public static DataTable Convert_XMLstring_To_Datatable(string XML_To_Table)
        {

            DataTable Return_Table = new DataTable();

            try
            {
                if (!string.IsNullOrEmpty(XML_To_Table))
                {
                    DataSet dataSet = new DataSet();
                    StringReader stringReader = new StringReader(XML_To_Table);
                    dataSet.ReadXml(stringReader);
                    Return_Table = dataSet.Tables[0];
                }
            }
            catch (Exception) { }

            return Return_Table;
        }


        /// <summary>
        /// Clears temporary files uploaded.
        /// </summary>
        /// <param name="globalGuid"></param>
        /// <returns></returns>
        public static bool ClearTempFolder(string globalGuid, string userID)
        {
            try
            {
                string directory = CommonFunctions.GetUserFolder(globalGuid, userID);
                Directory.Delete(directory, true);
            }
            catch (Exception)
            {
            }
            return true;
        }

        /// <summary>
        /// Clears temporary files uploaded.
        /// </summary>
        /// <param name="globalGuid"></param>
        /// <returns></returns>
        public static bool ClearTempFolder(string globalGuid, string userID, string fileName)
        {
            try
            {
                string directoryPath = CommonFunctions.GetUserFolder(globalGuid, userID);
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                if (directoryInfo.Exists)
                {
                    FileInfo[] fileInfoArray = directoryInfo.GetFiles(fileName, SearchOption.AllDirectories);
                    foreach (FileInfo fileInfo in fileInfoArray)
                    {
                        if (fileInfo != null)
                        {
                            fileInfo.Delete();
                            DirectoryInfo fileDirectoryInfo = fileInfo.Directory;
                            if (fileDirectoryInfo.GetFiles().Length + fileDirectoryInfo.GetDirectories().Length == 0)
                            {
                                fileDirectoryInfo.Delete();
                            }
                        }
                    }
                    if (Directory.Exists(directoryInfo.FullName) && directoryInfo.GetFiles().Length + directoryInfo.GetDirectories().Length == 0)
                    {
                        directoryInfo.Delete();
                    }
                }

            }
            catch (Exception)
            {
            }
            return true;
        }

        /// <summary>
        /// Clears the users cache.
        /// </summary>
        /// <param name="userID">The user id.</param>
        /// <param name="startsWith">The beginning of the cache key name.</param>
        public static void ClearUserCache(string userID, string startsWith)
        {
            //Loop through cached values
            foreach (DictionaryEntry dictionaryEntry in HttpRuntime.Cache)
            {
                if (dictionaryEntry.Key.ToString().StartsWith(startsWith) && dictionaryEntry.Key.ToString().EndsWith(Consts.Common.Underscore + userID))
                {
                    HttpRuntime.Cache.Remove(dictionaryEntry.Key.ToString());
                }
            }
        }

        /// <summary>
        /// Creates a folder struction from a file path.
        /// </summary>
        /// <param name="filePath">The full path to the file from which the structure will be created.</param>
        /// <returns>True if successfull.</returns>
        public static bool CreateFolderStructure(string filePath)
        {
            try
            {
                if (filePath != null)
                {
                    string path = Path.GetDirectoryName(filePath);
                    Directory.CreateDirectory(path);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeSerializeObject<T>(string data, bool isFileName)
        {
            if (string.IsNullOrEmpty(data)) { return default(T); }

            T objectOut = default(T);

            try
            {
                lock (_serialization)
                {
                    string attributeXml = string.Empty;
                    string xmlString = string.Empty;

                    if (isFileName)
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(data);
                        xmlString = xmlDocument.OuterXml;
                    }
                    else
                    {
                        xmlString = data;
                    }

                    using (StringReader read = new StringReader(xmlString))
                    {
                        Type outType = typeof(T);

                        XmlSerializer serializer = new XmlSerializer(outType);
                        using (XmlReader reader = new XmlTextReader(read))
                        {
                            objectOut = (T)serializer.Deserialize(reader);
                            reader.Close();
                        }

                        read.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //LogExceptionWithDisplay(new StackFrame(true), ex, Captain<string>.Session[Consts.SessionVariables.UserID], ExceptionSeverityLevel.Error, new string[0]);
            }

            return objectOut;
        }

        /// <summary>
        /// Formats date string to DD-MMM-YYYY format
        /// </summary>
        /// <param name="srcDate"></param>
        /// <returns></returns>
        public static string FormatDateString(string srcDate)
        {
            try
            {
                if (srcDate != string.Empty && srcDate.Trim().Length > 0)
                {
                    return Convert.ToDateTime(srcDate).ToString("");
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Get a path and makes sure that it is valid by removing invalid characters.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetValidPath(string path)
        {
            string regexSearch = string.Format(Consts.Parameters.Zero + Consts.Parameters.One, new string(Path.GetInvalidFileNameChars()), new string(Path.GetInvalidPathChars()));
            Regex r = new Regex(string.Format(Consts.Common.OpenBracket + Consts.Parameters.Zero + Consts.Common.CloseBracket, Regex.Escape(regexSearch)));

            return r.Replace(path, string.Empty);
        }

        /// <summary>
        /// Generates a folder path from a file path.
        /// </summary>
        /// <param name="filePath">The full qualified file path.</param>
        /// <returns>True if the folder structure is created. Otherwise, returns false.</returns>
        /// <example>bool success = GenerageFolderStructure("C:\Test\TestCases\testcase.doc")</example>
        public static bool GenerateFolderStructure(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    string path = Path.GetDirectoryName(filePath);
                    Directory.CreateDirectory(path);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnType"></param>
        /// <param name="isDropDown"></param>
        /// <returns></returns>
        public static CellType GetColumnType(string columnType, bool isDropDown)
        {
            CellType cellType = CellType.TextBox;

            switch (columnType.ToLower())
            {
                case Consts.DataTypes.Boolean:
                    cellType = CellType.CheckBox;
                    break;
                case Consts.DataTypes.UserType:
                    cellType = CellType.TextBox;
                    break;
                case Consts.DataTypes.String:
                    cellType = isDropDown ? CellType.ComboBox : CellType.TextBox;
                    break;
                case Consts.DataTypes.Percent:
                case Consts.DataTypes.Number:
                    cellType = CellType.TextBox;
                    break;
                case Consts.DataTypes.DateTime:
                    cellType = CellType.DateTimePicker;
                    break;
                case Consts.DataTypes.Date:
                    cellType = CellType.DatePicker;
                    break;
                case Consts.DataTypes.Time:
                    cellType = CellType.TextBox;
                    break;
            }

            return cellType;
        }

        /// <summary>
        /// Returns config value.
        /// </summary>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public static T GetConfigValue<T>(string valueName, T defaultValue)
        {
            try
            {
                object returnValue = null;

                bool isKeyExist = WebConfigurationManager.AppSettings.AllKeys.Cast<string>().ToList().Find(k => k.Equals(valueName)) != null;
                if (!isKeyExist) { return defaultValue; }

                switch (typeof(T).Name.ToLower())
                {
                    case "datetime":
                        returnValue = DateTime.Parse(WebConfigurationManager.AppSettings[valueName]);
                        break;
                    case "int32":
                        returnValue = int.Parse(WebConfigurationManager.AppSettings[valueName]);
                        break;
                    case "int64":
                        returnValue = long.Parse(WebConfigurationManager.AppSettings[valueName]);
                        break;
                    case "boolean":
                        returnValue = bool.Parse(WebConfigurationManager.AppSettings[valueName]);
                        break;
                    default:
                        returnValue = WebConfigurationManager.AppSettings[valueName];
                        break;
                }

                return null == returnValue ? defaultValue : (T)returnValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Used to get a confirmation message when deleting a node.
        /// </summary>
        /// <param name="isObjectHasActiveTask"></param>
        /// <param name="hasChildrenForAnyRow"></param>
        /// <param name="isDocument"></param>
        /// <returns></returns>
        public static string GetNodeDeleteConfirmationMessage(bool isObjectHasActiveTask, bool hasChildrenForAnyRow, bool isDocument)
        {
            string message = string.Empty;

            if (isObjectHasActiveTask)
            {
                if (hasChildrenForAnyRow && !isDocument)
                {
                    //message = Consts.Messages.ChildItemsAndActiveTasksCompleteOrCancelBeforeDeleting.GetMessage();
                }
                else if (isDocument && !hasChildrenForAnyRow)
                {
                    // message = Consts.Messages.ItemHasActiveTasksCompleteOrCancelBeforeDeleting.GetMessage();
                }
                else
                {
                    //message = Consts.Messages.AreYouSureYouWantToDeleteItemAndActiveTasks.GetMessage();
                }
            }
            else
            {
                if (hasChildrenForAnyRow)
                {
                    // message = Consts.Messages.ChildItemsAreYouSureYouWantToDelete.GetMessage();
                }
                else
                {
                    // message = Consts.Messages.AreYouSureYouWantToDelete.GetMessage();
                }
            }

            return message;
        }

        /// <summary>
        /// Gets the
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static XmlDocument GetEmbeddedXmlDocument(string embededXmlResource)
        {
            Assembly compounds = Assembly.GetExecutingAssembly();
            using (Stream readStream = compounds.GetManifestResourceStream(embededXmlResource))
            {
                XmlTextReader xmlTextReader = new XmlTextReader(readStream);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(xmlTextReader);

                return xmlDocument;
            }
        }

        /// <summary>
        /// Gets the description for an enumeration which has been assigned an DescriptionAttribute tag
        /// </summary>
        /// <param name="enumValue"></param>
        /// <param name="defaultDescription"></param>
        /// <returns></returns>
        public static string GetEnumDescription(object enumValue, string defaultDescription)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return defaultDescription;
        }

        /// <summary>
        /// Get the language from the browser.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static CultureInfo GetBrowserCulture(HttpContext httpContext)
        {
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(Consts.Common.DefaultLanguage);

            string[] languages = httpContext.Request.UserLanguages;

            if (languages != null && languages.Length != 0)
            {
                try
                {
                    string language = languages[0].ToLowerInvariant().Trim();
                    cultureInfo = CultureInfo.CreateSpecificCulture(language);
                }
                catch (ArgumentException)
                {
                }
            }

            return cultureInfo;
        }

        /// <summary>
        /// Gets the content of a file as base64
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileContent(string fileName)
        {
            string content = string.Empty;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, Convert.ToInt32(fileStream.Length));
                content = Convert.ToBase64String(fileBytes, Base64FormattingOptions.None);
            }

            return content;
        }

        /// <summary>
        /// Gets dates in the future.
        /// </summary>
        /// <param name="dates"></param>
        /// <returns></returns>
        public static string GetFutureDates(string dates)
        {
            try
            {
                string strFutureDates = string.Empty;
                if (dates == "")
                {
                    System.DateTime today = System.DateTime.Now;
                    strFutureDates = FormatDateString(today.AddDays(GetRandomUsers()).ToString());
                }
                else
                {
                    strFutureDates = FormatDateString(dates);
                }

                return strFutureDates;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshInterval"></param>
        /// <returns></returns>
        public static int GetIntervalInMilliseconds(int refreshInterval)
        {
            int interval = 0;

            switch (refreshInterval)
            {
                case 1: //10 Seconds
                    interval = 1000 * 10;
                    break;
                case 2: //20 Seconds
                    interval = 1000 * 20;
                    break;
                case 3: //30 Seconds
                    interval = 1000 * 30;
                    break;
                case 4: //40 Seconds
                    interval = 1000 * 40;
                    break;
                case 5: //50 Seconds
                    interval = 1000 * 50;
                    break;
                case 6: //1 Minute
                    interval = 1000 * 60;
                    break;
                case 7: //2 Minute
                    interval = 2000 * 60;
                    break;
                case 8: //3 Minute
                    interval = 3000 * 60;
                    break;
                case 9: //4 Minute
                    interval = 4000 * 60;
                    break;
                case 10: //5 Minute
                    interval = 5000 * 60;
                    break;
            }

            return interval;
        }

        /// <summary>
        /// Gets an open no content tab.
        /// </summary>
        /// <param name="ContentTabs"></param>
        /// <param name="objectID"></param>
        /// <returns></returns>
        public static TabPage GetNoContentTab(TabControl ContentTabs)
        {
            TabPage results = null;

            foreach (TabPage workspaceTab in ContentTabs.TabPages)
            {
                //if (workspaceTab.Controls[0] is NoContentControl)
                //{
                //    results = workspaceTab;
                //    break;
                //}
            }

            return results;
        }

        /// <summary>
        /// Gets an open tab.
        /// </summary>
        /// <param name="ContentTabs"></param>
        /// <param name="tagClass"></param>
        /// <returns></returns>
        public static TabPage GetOpenTab(TabControl ContentTabs, TagClass tagClass)
        {


            TabPage results = null;

            foreach (TabPage workspaceTab in ContentTabs.TabPages)
            {
                TagClass wsTagClass = workspaceTab.Tag as TagClass;

                //This will be added back when versions is implemented in the download/upload control - Alex
                //&& wsTagClass.MajorVersion == tagClass.MajorVersion && wsTagClass.MinorVersion == tagClass.MinorVersion
                if (wsTagClass.ObjectID == tagClass.ObjectID)
                {
                    results = workspaceTab;
                    break;
                }
            }

            return results;
        }

        /// <summary>
        /// Returns CTDPath by concatenating the name of the parent nodes.
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="ctdPath"></param>
        private static void GetParentNodes(XmlNode xmlNode, ref string ctdPath)
        {
            try
            {
                if (xmlNode.ParentNode != null)
                {
                    ctdPath = ctdPath + xmlNode.ParentNode.Name + Consts.Common.Slash;
                    GetParentNodes(xmlNode.ParentNode, ref ctdPath);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Geenrates a Random Number within the specified Range.
        /// </summary>
        /// <returns></returns>
        public static int GetRandomUsers()
        {
            try
            {
                // STUBS REMOVAL  :  Removed  Acced DB code from ?GetRandomUsers? method
                System.Random intRandomGenerator = new System.Random();
                return intRandomGenerator.Next(1, 9);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets a select one item for a combobox
        /// </summary>
        /// <returns></returns>
        public static ListItem GetSelectOneItem()
        {
            return new ListItem() { Text = Consts.Common.SelectOne, ID = string.Empty, Value = string.Empty };
        }

        /// <summary>
        /// Gets a Undefined item for a combobox
        /// </summary>
        /// <returns></returns>
        public static ListItem GetUndefinedItem()
        {
            return new ListItem() { Text = Consts.Common.UnDefined, ID = string.Empty, Value = string.Empty };
        }

        /// <summary>
        /// Gets a all item for a combobox
        /// </summary>
        /// <returns></returns>
        public static ListItem GetAllItem()
        {
            return new ListItem() { Text = Consts.Common.All, ID = string.Empty, Value = Consts.Common.Zero };
        }

        /// <summary>
        /// Gets a clear value item for a combobox
        /// </summary>
        /// <returns></returns>
        public static ListItem GetClearValueItem()
        {
            return new ListItem() { Text = Consts.Common.ClearValue, ID = string.Empty, Value = string.Empty };
        }

        /// <summary>
        /// Gets a list of selected taxonomy nodes.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="list"></param>
        /// <param name="parentNodeID"></param>
        public static void GetSelectedSpecificNodes(TreeNodeCollection nodes, List<TagClass> list, string parentNodeID)
        {
            foreach (TreeNode treeNode in nodes)
            {
                if (treeNode.Checked)
                {
                    TagClass tagClass = (TagClass)treeNode.Tag;
                    tagClass.ParentNodeID = parentNodeID;
                    list.Add(tagClass);
                    if (treeNode.Nodes.Count > 0) //Was treeNode.HasNodes
                    {
                        GetSelectedSpecificNodes(treeNode.Nodes, list, tagClass.NodeID);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a server path for a file
        /// </summary>
        /// <param name="globalGuid"></param>
        /// <param name="filesName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string GetServerFile(string globalGuid, string filesName, string userID)
        {
            string serverFile = CommonFunctions.GetUserFolder(globalGuid, userID) + filesName;

            string upDownRootPath = CommonFunctions.GetConfigValue(Consts.Common.UpDownRootPath, Consts.Common.DefaultUpDownRootPath);
            string upDownRootDrive = CommonFunctions.GetConfigValue(Consts.Common.UpDownRootDrive, Consts.Common.DefaultUpDownRootPath);

            serverFile = serverFile.Replace(upDownRootPath, upDownRootDrive);

            return serverFile;
        }

        /// <summary>
        /// Genareates a tooltip based on a TagClass
        /// </summary>
        /// <param name="tagClass">The TagClass object.</param>
        /// <returns></returns>
        public static string GetMenuToolTip(TagClass tagClass)
        {
            StringBuilder toolTip = new StringBuilder();

#if DEBUG
            toolTip.Append("Node Type: ");
#endif
            return toolTip.ToString();
        }

        /// <summary>
        /// Genareates a tooltip based on a TagClass
        /// </summary>
        /// <param name="tagClass">The TagClass object.</param>
        /// <returns></returns>
        public static string GetTagClassToolTip(TagClass tagClass)
        {
            StringBuilder toolTip = new StringBuilder();

#if DEBUG
            toolTip.Append("Name: ");

#endif

            return toolTip.ToString();
        }

        /// <summary>
        /// Gets a fully qualified file path.
        /// </summary>
        /// <param name="fileGuid">The file guid. This is used for uploading and will be appended to the path.</param>
        /// <param name="userID">This is the user's id. This allows to get the users file folder.</param>
        /// <returns>Retuns a folder path string.</returns>
        public static string GetUserFolder(string fileGuid, string userID)
        {
            string userFolder = string.Empty;
            string upDownRootPath = CommonFunctions.GetConfigValue(Consts.Common.UpDownRootPath, Consts.Common.DefaultUpDownRootPath);

            userFolder = userID.Equals(string.Empty) ? upDownRootPath : Path.Combine(upDownRootPath, userID);

            userFolder = fileGuid.Trim().Equals(string.Empty) ? userFolder : Path.Combine(userFolder, fileGuid);

            if (userFolder.LastIndexOf(Consts.Common.BackSlash) == userFolder.Length - 1)
            {
                return userFolder;
            }
            else
            {
                return userFolder + Consts.Common.BackSlash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_xmlDocument"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static XmlAttribute GetXmlNodeAttribute(XmlDocument _xmlDocument, string attributeName, string attributeValue)
        {
            XmlAttribute xmlAttr = null;
            try
            {
                xmlAttr = (XmlAttribute)_xmlDocument.CreateNode(XmlNodeType.Attribute, attributeName, string.Empty);
                xmlAttr.Value = attributeValue;
            }
            catch (Exception ex)
            {
                StackFrame stackframe = new StackFrame(1, true);
                //               ExceptionLogger.LogException(stackframe, ex, ExceptionSeverityLevel.Low);
            }
            return xmlAttr;
        }

        /// <summary>
        /// Gets a string value from an XmlNode given the path of the node.
        /// </summary>
        /// <param name="xmlNode">The XmlNode that contains the value.</param>
        /// <param name="xPath">The path to the single node.</param>
        /// <returns>A string value.</returns>
        public static string GetXmlValue(XmlNode xmlNode, string xPath)
        {
            string xmlString = string.Empty;
            try
            {
                if (xmlNode.SelectSingleNode(xPath) != null)
                {
                    xmlString = xmlNode.SelectSingleNode(xPath).InnerText;
                    return xmlString;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the value from a report option.
        /// </summary>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <param name="reportOptions"></param>
        /// <returns></returns>
        public static string GetReportOptions(string optionName, string reportOptions)
        {
            string reptOptionvalue = reportOptions.Replace(Consts.Common.LessThan + optionName + Consts.Common.GreaterThan, string.Empty);
            return reptOptionvalue.Replace(Consts.Common.LessThan + Consts.Common.Slash + optionName + Consts.Common.GreaterThan, string.Empty).Trim();
        }

        /// <summary>
        /// Returns true if the value is length of 4 and all are digits.
        /// </summary>
        /// <param name="theValue"></param>
        /// <returns></returns>
        public static bool Is4DigitInteger(string theValue)
        {
            bool returnValue;
            try
            {
                if (theValue.Length == 4)
                {
                    int dummyValue;
                    returnValue = Int32.TryParse(theValue, out dummyValue);
                }

                else
                {
                    returnValue = false;
                }
                return returnValue;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Gives whether the text is alphanumeric or not
        /// </summary>
        /// <param name="matchText"></param>   
        public static bool IsAlphaNumeric(string matchText)
        {
            try
            {
                System.Text.RegularExpressions.Match match = Regex.Match(matchText, "", RegexOptions.IgnoreCase);
                return match.Success;
            }
            catch (Exception ex)
            {
                // ExceptionLogger.LogAndDisplayMessageToUser(new StackFrame(true), ex, QuantumFaults.None, ExceptionSeverityLevel.High);
                return false;
            }
        }

        /// <summary>
        /// Checks for numbers only
        /// </summary>
        /// <param name="matchText"></param>
        /// <returns></returns>
        public static bool IsNumeric(string matchText)
        {
            try
            {
                System.Text.RegularExpressions.Match match = Regex.Match(matchText, Consts.StaticVars.DecimalString, RegexOptions.IgnoreCase);
                return match.Success;
            }
            catch (Exception ex)
            {
                //ExceptionLogger.LogAndDisplayMessageToUser(new StackFrame(true), ex, QuantumFaults.None, ExceptionSeverityLevel.High);
                return false;
            }
        }

        /// <summary>
        /// Gives whether the text is alphanumeric or not
        /// </summary>
        /// <param name="matchText"></param>   
        public static bool IsExtendedAlphaNumeric(string matchText)
        {
            try
            {
                System.Text.RegularExpressions.Match match = Regex.Match(matchText, Consts.StaticVars.AlphaNumericString, RegexOptions.IgnoreCase);
                return match.Success;
            }
            catch (Exception ex)
            {
                // ExceptionLogger.LogAndDisplayMessageToUser(new StackFrame(true), ex, QuantumFaults.None, ExceptionSeverityLevel.High);
                return false;
            }
        }

        /// <summary>
        /// Gives whether the text is alphanumeric or not
        /// </summary>
        /// <param name="matchText"></param>   
        public static bool IsAlpha(string matchText)
        {
            try
            {
                System.Text.RegularExpressions.Match match = Regex.Match(matchText, Consts.StaticVars.AlphaString, RegexOptions.IgnoreCase);
                return match.Success;
            }
            catch (Exception ex)
            {
                // ExceptionLogger.LogAndDisplayMessageToUser(new StackFrame(true), ex, QuantumFaults.None, ExceptionSeverityLevel.High);
                return false;
            }
        }

        public static bool IsDecimalValid(string matchText)
        {
            bool boolvalid = true;
            try
            {
                Convert.ToDecimal(matchText);
            }
            catch (Exception ex)
            {
                boolvalid = false;
            }
            return boolvalid;
        }
        /// <summary>
        /// Returns true if the selected treeview node is an application; returns false if the node is not initialized or is not a application
        /// </summary>
        /// <param name="theValue"></param>
        /// <returns></returns>
        public static bool IsApplicationNode(TreeNode node)
        {
            bool answer = false;
            try
            {
                if (!(node == null))
                {
                    // Get the tag class from the node
                    if (!(node.Tag == null))
                    {
                        TagClass tagClass = (TagClass)node.Tag;
                    }
                }

                return answer;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the mode is debug.
        /// </summary>
        /// <returns></returns>
        public static bool CheckDebugMode()
        {
            try
            {
                return GetConfigValue(Consts.Common.IsDebugMode, false);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns true if the value is integer.
        /// </summary>
        /// <param name="theValue"></param>
        /// <returns></returns>
        public static bool IsInteger(string theValue)
        {
            bool returnValue = false;
            try
            {
                int dummyValue;
                returnValue = Int32.TryParse(theValue, out dummyValue);
                return returnValue;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determins if a tab is open.
        /// </summary>
        /// <param name="ContentTabs"></param>
        /// <param name="tagClass"></param>
        /// <returns></returns>
        public static bool IsTabOpen(TabControl ContentTabs, TagClass tagClass)
        {
            return GetOpenTab(ContentTabs, tagClass) != null;
        }

        /// <summary>
        /// Gives whether the email is valid (or) not
        /// </summary>
        /// <param name="emailID"></param>      
        public static bool IsValidEmail(string emailID)
        {
            try
            {
                System.Text.RegularExpressions.Match match = Regex.Match(emailID.Trim(), Consts.StaticVars.EmailValidatingString, RegexOptions.IgnoreCase);
                return match.Success;
            }
            catch (Exception ex)
            {
                StackFrame stackFrame = new StackFrame();
                // ExceptionLogger.LogAndDisplayMessageToUser(stackFrame, ex, QuantumFaults.None, ExceptionSeverityLevel.High);
                return false;
            }
        }

        /// <summary>
        /// Function to get first 100 chars appended with ...
        /// </summary>
        /// <param name="originalValue">The original string</param>
        /// <returns>The modified string</returns>
        public static string LimitStringToHundredCharacters(string originalValue)
        {
            return LimitStringToNCharacters(originalValue, 100);
        }

        /// <summary>
        /// Function to get first N chars appended with ...
        /// </summary>
        /// <param name="originalValue">The original string</param>
        /// <param name="maxCharacters">The maximum characters to get from the string.</param>
        /// <returns>The modified string</returns>
        public static string LimitStringToNCharacters(string originalValue, int maxCharacters)
        {
            string resultValue = string.Empty;
            if (originalValue.Length <= maxCharacters)
            {
                resultValue = originalValue;
            }
            else
            {
                resultValue = originalValue.Substring(0, maxCharacters);
                resultValue = resultValue + Consts.Common.ThreeDots;
            }
            return resultValue;
        }

        /// <summary>
        /// Log errors to log file using log4net
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="parameters"></param>
        /// <param name="userID"></param>
        /// <param name="errorMessage"></param>
        public static void LogToFile(StackFrame frame, string[] parameters, string userID, string errorMessage)
        {
            ExceptionLoggingUtility logError = new ExceptionLoggingUtility();
            logError.WriteToLogFile(frame, parameters, errorMessage, userID, ExceptionSeverityLevel.Error);
            logError = null;
        }

        /// <summary>
        /// Logs the service calls to a trace.log file using log4net.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <param name="userID"></param>
        public static void LogToFile(StackFrame frame, string[] parameters, string userID, string[] response)
        {
            ExceptionLoggingUtility logMessage = new ExceptionLoggingUtility();
            logMessage.DebugToLogFile(frame, parameters, response, userID);
            logMessage = null;
        }

        /// <summary>
        /// Logs the service calls to a trace.log file using log4net.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="frame"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="userID"></param>
        public static void LogToFile<TRequest>(StackFrame frame, TRequest request, string response, string userID)
        {
            ExceptionLoggingUtility logMessage = new ExceptionLoggingUtility();

            logMessage.DebugToLogFile(frame, request, response, userID);
            logMessage = null;
        }

        /// <summary>
        /// Log errors to trace.txt file under viewpoint\logs
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logName"></param>
        public static void LogToFile(string message, string logName)
        {
            if (GetConfigValue(Consts.Common.IsDebugMode, false))
            {
                lock (Consts.CacheKeys.Locker)
                {
                    using (StreamWriter sw = File.AppendText(@"C:\Captain\Logs\" + logName + ".log"))
                    {
                        sw.WriteLine(message);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Reads from one stream, writes it to the other stream and closes both streams.
        /// </summary>
        /// <param name="readStream">The source stream.</param>
        /// <param name="writeStream">The target stream. </param>
        /// <remarks>This method is used to get resource stream and save it to a file stream. Once done it will close both streams.</remarks>
        public static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            try
            {
                int length = 256;
                Byte[] buffer = new Byte[length];
                int bytesRead = readStream.Read(buffer, 0, length);
                // write the required bytes
                while (bytesRead > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = readStream.Read(buffer, 0, length);
                }
                readStream.Close();
                writeStream.Close();
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        public static string SerializeObject<T>(T serializableObject)
        {
            string xmlString = string.Empty;

            if (serializableObject == null) { return xmlString; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlString = xmlDocument.OuterXml;
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                //LogExceptionWithDisplay(new StackFrame(true), ex, Captain<string>.Session[Consts.SessionVariables.UserID], ExceptionSeverityLevel.Error, new string[0]);
            }

            return xmlString;
        }

        /// <summary>
        /// Sets the cache user and system cache expiration timeout.
        /// </summary>
        public static void SetCacheExpiration()
        {
            GlobalVariables.CacheExpiration = GetConfigValue(Consts.Common.CacheExpiration, 1);
            GlobalVariables.SystemCacheExpiration = GetConfigValue(Consts.Common.SystemCacheExpiration, 24);
        }

        /// <summary>
        /// Set properties for a DataGridView column.
        /// </summary>
        /// <param name="column">DataGridView column.</param>
        /// <param name="headerText">Column header text.</param>
        /// <param name="padding">Column padding.</param>
        /// <param name="visible">Indicates if column should be displayed.</param>
        public static void SetGridColumn(DataGridViewColumn column, string headerText, Nullable<Padding> padding, bool visible)
        {
            if (padding.HasValue)
            {
                DataGridViewCellStyle dgvCellStyle = new DataGridViewCellStyle();
                dgvCellStyle.Padding = padding.Value;
                column.DefaultCellStyle = dgvCellStyle;
            }
            column.HeaderText = headerText;
            column.ToolTipText = headerText;
            column.Visible = visible;
        }

        /// <summary>
        /// Create a new node or sets the value of an existing payload node.
        /// </summary>
        /// <param name="payload">The existing payload.</param>
        /// <param name="payloadNodeType">The type of node to add to the payload.</param>
        /// <param name="nodeName">The name of the node.</param>
        /// <param name="nodeValue">The value of the node.</param>
        /// <returns></returns>
        public static List<XmlNode> SetPayloadNode(XmlNode[] payload, PayloadNodeTypes payloadNodeType, string nodeName, string nodeValue)
        {
            List<XmlNode> payloadList = new List<XmlNode>(payload);
            XmlNode xmlNode = payloadList.Find(n => n.Name.Equals(nodeName));

            if (xmlNode == null)
            {
                if (payloadNodeType == PayloadNodeTypes.Simple)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    string nodeXml = Consts.Common.TaskNodeTemplate;
                    nodeXml = string.Format(nodeXml, nodeName, nodeValue);
                    xmlDocument.LoadXml(nodeXml);

                    XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
                    namespaceManager.AddNamespace(Consts.Common.TaskPrefix, Consts.Common.TaskNamespace);
                    xmlNode = xmlDocument.SelectSingleNode(Consts.Common.TaskPrefix + Consts.Common.Colon + nodeName, namespaceManager);
                    xmlNode.InnerText = nodeValue;
                }
                payloadList.Add(xmlNode);
            }
            else
            {
                xmlNode.InnerText = nodeValue;
            }

            return payloadList;
        }

        /// <summary>
        /// Checks if a node can be expanded.
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        public static bool IsExpandable(TreeNode treeNode)
        {
            bool isExpandable = false;

            for (int nodeIndex = 0; nodeIndex < treeNode.Nodes.Count; nodeIndex++)
            {
                TreeNode node = treeNode.Nodes[nodeIndex];
                isExpandable = node.Nodes.Count > 0 && !node.IsExpanded;
                if (isExpandable)
                    break;
                else if (node.Nodes.Count > 0)
                    isExpandable = IsExpandable(node);
            }

            return isExpandable;
        }

        /// <summary>
        /// Grid view cell tool tip added.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="addOperator"></param>
        /// <param name="addDate"></param>
        /// <param name="updateOperator"></param>
        /// <param name="updateDate"></param>
        /// <param name="datagridview"></param>
        public static void setTooltip(int rowIndex, string addOperator, string addDate, string updateOperator, string updateDate, DataGridView datagridview)
        {
            string toolTipText = Consts.Common.AddedBy + addOperator.Trim() + Consts.Common.On + addDate.ToString() + Consts.Common.NewLine;
            string modifiedBy = string.Empty;
            if (!updateOperator.ToString().Trim().Equals(string.Empty))
                modifiedBy = updateOperator.ToString().Trim() + Consts.Common.On + updateDate.ToString();
            toolTipText += Consts.Common.ModifiedBy + modifiedBy;

            foreach (DataGridViewCell cell in datagridview.Rows[rowIndex].Cells)
            {
                cell.ToolTipText = toolTipText;
            }
        }



        /// <summary>
        /// Grid view cell tool tip added.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="addOperator"></param>
        /// <param name="addDate"></param>
        /// <param name="updateOperator"></param>
        /// <param name="updateDate"></param>
        /// <param name="datagridview"></param>
        public static void setTooltip(int rowIndex, string addOperator, string addDate, string updateOperator, string updateDate, DataGridView datagridview, string strApplicantNo)
        {
            string toolTipText = "Applicant No : " + strApplicantNo;
            string modifiedBy = string.Empty;
            string addedby = string.Empty;
            //+ Consts.Common.NewLine + Consts.Common.AddedBy + addOperator.Trim() + Consts.Common.On + addDate.ToString() + Consts.Common.NewLine
            if (!addOperator.ToString().Trim().Equals(string.Empty))
            {
                addedby = Consts.Common.NewLine + Consts.Common.AddedBy + addOperator.Trim() + Consts.Common.On + addDate.ToString();
                toolTipText += addedby;
            }
            if (!updateOperator.ToString().Trim().Equals(string.Empty))
            {
                modifiedBy = updateOperator.ToString().Trim() + Consts.Common.On + updateDate.ToString();
                toolTipText += Consts.Common.NewLine + Consts.Common.ModifiedBy + modifiedBy;
            }
            foreach (DataGridViewCell cell in datagridview.Rows[rowIndex].Cells)
            {
                cell.ToolTipText = toolTipText;
            }
        }

        public static void setTooltip(int rowIndex, string updateOperator, string updateDate, DataGridView datagridview)
        {
            string toolTipText = string.Empty;
            string modifiedBy = string.Empty;
            if (!updateOperator.ToString().Trim().Equals(string.Empty))
            {
                modifiedBy = updateOperator.ToString().Trim() + Consts.Common.On + updateDate.ToString();
                toolTipText += Consts.Common.NewLine + Consts.Common.ModifiedBy + modifiedBy;
            }
            foreach (DataGridViewCell cell in datagridview.Rows[rowIndex].Cells)
            {
                cell.ToolTipText = toolTipText;
            }
        }



        public static string GetHierachyFormat(string strAgency, string strDept, string strProgram, string strYear, string HIEREPRSNTN)
        {
            string strName = string.Empty;

            switch (HIEREPRSNTN)
            {
                case "01":
                    strName = strAgency;
                    break;
                case "02":
                    strName = strAgency + Consts.Common.TabSpace + strDept;
                    break;
                case "03":
                    strName = strAgency + Consts.Common.TabSpace + strProgram;
                    break;
                case "04":
                    strName = strAgency + Consts.Common.TabSpace + Consts.Common.TabSpace + Consts.Common.TabSpace + strDept + Consts.Common.TabSpace + Consts.Common.TabSpace + Consts.Common.TabSpace + strProgram;
                    break;
                case "05":
                    strName = strAgency + Consts.Common.TabSpace + strProgram + Consts.Common.TabSpace + strDept;
                    break;
                case "06":
                    strName = strDept;
                    break;
                case "07":
                    strName = strDept + Consts.Common.TabSpace + strAgency;
                    break;
                case "08":
                    strName = strDept + Consts.Common.TabSpace + strProgram;
                    break;
                case "09":
                    strName = strDept + Consts.Common.TabSpace + strProgram + Consts.Common.TabSpace + strAgency;
                    break;
                case "10":
                    strName = strDept + Consts.Common.TabSpace + strAgency + Consts.Common.TabSpace + strProgram;
                    break;
                case "11":
                    strName = strProgram;
                    break;
                case "12":
                    strName = strProgram + Consts.Common.TabSpace + strAgency;
                    break;
                case "13":
                    strName = strProgram + Consts.Common.TabSpace + strDept;
                    break;
                case "14":
                    strName = strProgram + Consts.Common.TabSpace + strAgency + Consts.Common.TabSpace + strDept;
                    break;
                case "15":
                    strName = strProgram + Consts.Common.TabSpace + strDept + Consts.Common.TabSpace + strAgency;
                    break;


            }
            return strName + Consts.Common.TabSpace + Consts.Common.TabSpace + Consts.Common.TabSpace + Consts.Common.TabSpace + Consts.Common.TabSpace + strYear;
        }

        public static string GetHTMLHierachyFormat(string strAgency, string strDept, string strProgram, string strYear, string HIEREPRSNTN)
        {
            string strName = string.Empty;

            switch (HIEREPRSNTN)
            {
                case "01":
                    strName = strAgency;
                    break;
                case "02":
                    strName = strAgency + Consts.Common.HTMLTabSpace + strDept;
                    break;
                case "03":
                    strName = strAgency + Consts.Common.HTMLTabSpace + strProgram;
                    break;
                case "04":
                    strName = strAgency + Consts.Common.HTMLTabSpace + strDept + Consts.Common.HTMLTabSpace + strProgram;
                    break;
                case "05":
                    strName = strAgency + Consts.Common.HTMLTabSpace + strProgram + Consts.Common.HTMLTabSpace + strDept;
                    break;
                case "06":
                    strName = strDept;
                    break;
                case "07":
                    strName = strDept + Consts.Common.HTMLTabSpace + strAgency;
                    break;
                case "08":
                    strName = strDept + Consts.Common.HTMLTabSpace + strProgram;
                    break;
                case "09":
                    strName = strDept + Consts.Common.HTMLTabSpace + strProgram + Consts.Common.HTMLTabSpace + strAgency;
                    break;
                case "10":
                    strName = strDept + Consts.Common.HTMLTabSpace + strAgency + Consts.Common.HTMLTabSpace + strProgram;
                    break;
                case "11":
                    strName = strProgram;
                    break;
                case "12":
                    strName = strProgram + Consts.Common.HTMLTabSpace + strAgency;
                    break;
                case "13":
                    strName = strProgram + Consts.Common.HTMLTabSpace + strDept;
                    break;
                case "14":
                    strName = strProgram + Consts.Common.HTMLTabSpace + strAgency + Consts.Common.HTMLTabSpace + strDept;
                    break;
                case "15":
                    strName = strProgram + Consts.Common.HTMLTabSpace + strDept + Consts.Common.HTMLTabSpace + strAgency;
                    break;


            }
            return strName + Consts.Common.HTMLTabSpace + Consts.Common.HTMLTabSpace + Consts.Common.HTMLTabSpace + strYear;
        }

        public static void SetComboBoxValue(ComboBox comboBox, string value)
        {
            if (comboBox != null && comboBox.Items.Count > 0)
            {
                foreach (ListItem li in comboBox.Items)
                {
                    if (Convert.ToString(li.Value).Trim().Equals(value.Trim()) || Convert.ToString(li.Text).Trim().Equals(value.Trim()))
                    {
                        comboBox.SelectedItem = li;
                        break;
                    }
                }
            }
        }
        public static string FollowupIndicatior(string strFollowupdate)
        {

            string strType = string.Empty;
            if (strFollowupdate != string.Empty)
            {
                if (Convert.ToDateTime(strFollowupdate) <= DateTime.Now.Date)
                {
                    strType = "R";
                }
                else
                {
                    if (Convert.ToDateTime(strFollowupdate) > DateTime.Now.Date && Convert.ToDateTime(strFollowupdate) <= DateTime.Now.Date.AddDays(7))
                    {
                        strType = "Y";
                    }
                    else
                    {
                        strType = "B";
                    }
                }
            }
            return strType;
        }
        public static HierarchyEntity GetHierachyNameFormat(string Agency, string Dept, string Program)
        {
            CaptainModel _model = new CaptainModel();
            return _model.HierarchyAndPrograms.GetCaseHierarchyName(Agency, Dept, Program);

        }

        public static void MessageBoxDisplay(string strMsg)
        {
            //MessageBox.Show(strMsg, Consts.Common.ApplicationCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            AlertBox.Show(strMsg, MessageBoxIcon.Warning);
        }

        public static string CalculationYear(DateTime dtstartDate, DateTime dtIntakeDate)
        {
            int intDiffInYears = 0;
            string strYears = "0";
            if (dtIntakeDate.Year > dtstartDate.Year)
            {
                if (dtIntakeDate.Month >= dtstartDate.Month)
                {
                    if (dtIntakeDate.Day >= dtstartDate.Day)
                    {   // this is a best case, easy subtraction situation
                        intDiffInYears = dtIntakeDate.Year - dtstartDate.Year;
                        strYears = intDiffInYears.ToString();
                    }
                    else
                    {
                        if (dtIntakeDate.Month == dtstartDate.Month)
                        {
                            intDiffInYears = dtIntakeDate.Year - dtstartDate.Year - 1;
                            strYears = intDiffInYears.ToString();

                        }
                        else
                        {
                            intDiffInYears = dtIntakeDate.Year - dtstartDate.Year;
                            strYears = intDiffInYears.ToString();
                        }
                    }
                }
                else
                {
                    intDiffInYears = dtIntakeDate.Year - dtstartDate.Year - 1;
                    strYears = intDiffInYears.ToString();
                }
            }
            //else
            //{
            //    if (dtIntakeDate.Year > dtstartDate.Year)
            //    {
            //        intDiffInYears = dtIntakeDate.Year - dtstartDate.Year - 1;
            //        strYears = intDiffInYears.ToString();
            //    }

            //}

            return strYears;

        }
        /// <summary>
        ///   Agytabs Table Filter AgyType and Hierchy wise
        /// </summary>
        /// <param name="commonAgyList"> BaseAgyTabsEntity</param>
        /// <param name="AgyCode">AgyCode</param>
        /// <param name="Agency">Agency</param>
        /// <param name="Dept">Dept</param>
        /// <param name="Prog">Program</param>
        /// <returns></returns>
        public static List<CommonEntity> AgyTabsFilterCode(List<CommonEntity> commonAgyList, string AgyCode, string Agency, string Dept, string Prog)
        {
            List<CommonEntity> _AgytabsFilter = new List<CommonEntity>();
            _AgytabsFilter = commonAgyList.FindAll(u => u.AgyCode == AgyCode);
            if (_AgytabsFilter.Count > 0)
            {
                _AgytabsFilter = _AgytabsFilter.FindAll(u => u.ListHierarchy.Contains(Agency + Dept + Prog) || u.ListHierarchy.Contains(Agency + Dept + "**") || u.ListHierarchy.Contains(Agency + "****") || u.ListHierarchy.Contains("******")).ToList();
                _AgytabsFilter = _AgytabsFilter.OrderByDescending(u => u.Active).ThenBy(u => u.Desc).ToList();
            }
            return _AgytabsFilter;
        }


        /// <summary>
        ///   Agytabs Table Filter AgyType and Hierchy wise
        /// </summary>
        /// <param name="commonAgyList"> BaseAgyTabsEntity</param>
        /// <param name="AgyCode">AgyCode</param>
        /// <param name="Agency">Agency</param>
        /// <param name="Dept">Dept</param>
        /// <param name="Prog">Program</param>
        ///  <param name="Mode">Mode</param>
        /// <returns></returns>
        public static List<CommonEntity> AgyTabsFilterCode(List<CommonEntity> commonAgyList, string AgyCode, string Agency, string Dept, string Prog, string Mode)
        {
            List<CommonEntity> _AgytabsFilter = new List<CommonEntity>();
            _AgytabsFilter = commonAgyList.FindAll(u => u.AgyCode == AgyCode);
            if (_AgytabsFilter.Count > 0)
            {

                if (Mode.ToUpper() == "ADD")
                {
                    _AgytabsFilter = _AgytabsFilter.FindAll(u => (u.ListHierarchy.Contains(Agency + Dept + Prog) || u.ListHierarchy.Contains(Agency + Dept + "**") || u.ListHierarchy.Contains(Agency + "****") || u.ListHierarchy.Contains("******")) && u.Active.ToString().ToUpper() == "Y").ToList();
                }
                else
                {
                    _AgytabsFilter = _AgytabsFilter.FindAll(u => u.ListHierarchy.Contains(Agency + Dept + Prog) || u.ListHierarchy.Contains(Agency + Dept + "**") || u.ListHierarchy.Contains(Agency + "****") || u.ListHierarchy.Contains("******")).ToList();
                }

                if (_AgytabsFilter.Count > 0)
                {
                    if (_AgytabsFilter[0].Sort == "C")
                        _AgytabsFilter = _AgytabsFilter.OrderByDescending(u => u.Active).ThenBy(u => u.Code).ToList();
                    else
                        _AgytabsFilter = _AgytabsFilter.OrderByDescending(u => u.Active).ThenBy(u => u.Desc).ToList();
                }
            }
            return _AgytabsFilter;
        }

        public static List<CommonEntity> AgyTabsFilterCodeDescription(List<CommonEntity> commonAgyList, string AgyCode, string Agency, string Dept, string Prog, string Mode)   // Brought update - Vikash 01032023 for Funding Source Report
        {
            List<CommonEntity> _AgytabsFilter = new List<CommonEntity>();
            _AgytabsFilter = commonAgyList.FindAll(u => u.AgyCode == AgyCode);
            if (_AgytabsFilter.Count > 0)
            {



                _AgytabsFilter = _AgytabsFilter.OrderBy(u => u.Desc).ToList();
            }
            return _AgytabsFilter;
        }


        /// <summary>
        ///   Agytabs Table Filter AgyType and Hierchy wise
        /// </summary>
        /// <param name="commonAgyList"> BaseAgyTabsEntity</param>
        /// <param name="AgyCode">AgyCode</param>
        /// <param name="Agency">Agency</param>
        /// <param name="Dept">Dept</param>
        /// <param name="Prog">Program</param>
        ///  <param name="Mode">Mode</param>
        /// <returns></returns>
        public static List<CommonEntity> AgyTabsFilterCodeStatus(List<CommonEntity> commonAgyList, string AgyCode, string Agency, string Dept, string Prog, string Mode)
        {
            List<CommonEntity> _AgytabsFilter = new List<CommonEntity>();
            _AgytabsFilter = commonAgyList.FindAll(u => u.AgyCode == AgyCode);
            if (_AgytabsFilter.Count > 0)
            {

                if (Mode.ToUpper() == "A")
                {
                    _AgytabsFilter = _AgytabsFilter.FindAll(u => u.Active.ToString().ToUpper() == "Y").ToList();//(u.ListHierarchy.Contains(Agency + Dept + Prog) || u.ListHierarchy.Contains(Agency + Dept + "**") || u.ListHierarchy.Contains(Agency + "****") || u.ListHierarchy.Contains("******"))
                }
                else if (Mode.ToUpper() == "I")
                {
                    _AgytabsFilter = _AgytabsFilter.FindAll(u => u.Active.ToString().ToUpper() == "N").ToList();
                }

                _AgytabsFilter = _AgytabsFilter.OrderByDescending(u => u.Active).ThenBy(u => u.Desc).ToList();
            }
            return _AgytabsFilter;
        }



        /// <summary>
        ///   Agytabs Table Filter AgyType and Hierchy wise
        /// </summary>
        /// <param name="commonAgyList"> BaseAgyTabsEntity</param>
        /// <param name="AgyCode">AgyCode</param>
        /// <param name="Agency">Agency</param>
        /// <param name="Dept">Dept</param>
        /// <param name="Prog">Program</param>
        ///  <param name="Mode">Mode</param>
        /// <returns></returns>
        public static List<CommonEntity> AgyTabsFilterOrderbyCode(List<CommonEntity> commonAgyList, string AgyCode, string Agency, string Dept, string Prog, string Mode)
        {
            List<CommonEntity> _AgytabsFilter = new List<CommonEntity>();
            _AgytabsFilter = commonAgyList.FindAll(u => u.AgyCode == AgyCode);
            if (_AgytabsFilter.Count > 0)
            {

                if (Mode.ToUpper() == "ADD")
                {
                    _AgytabsFilter = _AgytabsFilter.FindAll(u => (u.ListHierarchy.Contains(Agency + Dept + Prog) || u.ListHierarchy.Contains(Agency + Dept + "**") || u.ListHierarchy.Contains(Agency + "****") || u.ListHierarchy.Contains("******")) && u.Active.ToString().ToUpper() == "Y").ToList();
                }
                else
                {
                    _AgytabsFilter = _AgytabsFilter.FindAll(u => u.ListHierarchy.Contains(Agency + Dept + Prog) || u.ListHierarchy.Contains(Agency + Dept + "**") || u.ListHierarchy.Contains(Agency + "****") || u.ListHierarchy.Contains("******")).ToList();
                }

                _AgytabsFilter = _AgytabsFilter.OrderByDescending(u => u.Active).ThenBy(u => u.Code).ToList();
            }
            return _AgytabsFilter;
        }

        /// <summary>
        ///   Agytabs Table Filter AgyType and Hierchy wise
        /// </summary>
        /// <param name="commonAgyList"> BaseAgyTabsEntity</param>
        /// <param name="AgyCode">AgyCode</param>
        /// <param name="Agency">Agency</param>
        /// <param name="Dept">Dept</param>
        /// <param name="Prog">Program</param>
        ///  <param name="Mode">Mode</param>
        /// <returns></returns>
        public static List<CommonEntity> AgyTabsDecisionCodeFilters(List<CommonEntity> commonAgyList, string AgyCode, string Agency, string Dept, string Prog, string Mode)
        {
            List<CommonEntity> _AgytabsFilter = new List<CommonEntity>();
            _AgytabsFilter = commonAgyList;
            if (_AgytabsFilter.Count > 0)
            {

                if (Mode.ToUpper() == "ADD")
                {
                    _AgytabsFilter = _AgytabsFilter.FindAll(u => (u.ListHierarchy.Contains(Agency + Dept + Prog) || u.ListHierarchy.Contains(Agency + Dept + "**") || u.ListHierarchy.Contains(Agency + "****") || u.ListHierarchy.Contains("******")) && u.Active.ToString().ToUpper() == "Y").ToList();//u.Extension.ToString() != ""
                }
                else
                {
                    _AgytabsFilter = _AgytabsFilter.FindAll(u => u.ListHierarchy.Contains(Agency + Dept + Prog) || u.ListHierarchy.Contains(Agency + Dept + "**") || u.ListHierarchy.Contains(Agency + "****") || u.ListHierarchy.Contains("******")).ToList();
                }

                _AgytabsFilter = _AgytabsFilter.OrderByDescending(u => u.Active).ThenBy(u => u.Code).ToList();
            }
            return _AgytabsFilter;
        }


        public static string GetLIMPQuesResp(string strcode)
        {
            string strDesc = string.Empty;
            if (strcode == "Y")
                strDesc = "Yes";
            if (strcode == "N")
                strDesc = "No";
            if (strcode == "U")
                strDesc = "Not Applicable";

            return strDesc;
        }

        public static List<CommonEntity> filterByHIE(List<CommonEntity> LookupValues, string Mode, string Agency, string Depart, string Program)
        {
            string HIE = Agency + Depart + Program;
            List<CommonEntity> _AgytabsFilter = new List<CommonEntity>();
            //_AgytabsFilter = LookupValues;
            if (LookupValues.Count > 0)
            {
                int i = 0; bool Can_Continue = true;
                foreach (CommonEntity Entity in LookupValues)
                {
                    string Temp = Entity.Hierarchy.ToString().Trim();
                    Can_Continue = true; i = 0;
                    if (!string.IsNullOrEmpty(Temp.Trim()))
                    {
                        for (i = 0; Can_Continue;)
                        {
                            string TempCode = Temp.Substring(i, 6);

                            if (HIE == "******")
                                _AgytabsFilter.Add(Entity);
                            else if (Depart + Program == "****")
                            {
                                if (TempCode.Substring(0, 2).ToString().Trim() == Agency)
                                    _AgytabsFilter.Add(Entity);
                            }
                            else if (Program == "**")
                            {
                                if (TempCode.Substring(0, 4).ToString().Trim() == Agency + Depart)
                                    _AgytabsFilter.Add(Entity);
                            }
                            else
                            {
                                if (TempCode == HIE)
                                    _AgytabsFilter.Add(Entity);
                                else if (TempCode.Contains("**"))
                                {
                                    if (TempCode.Substring(4, 2).ToString() == "**")
                                    {
                                        if (TempCode.Substring(0, 4).ToString().Trim() == Agency + Depart)
                                            _AgytabsFilter.Add(Entity);
                                    }
                                    else if (TempCode.Substring(2, 4).ToString() == "****")
                                    {
                                        if (TempCode.Substring(0, 2).ToString().Trim() == Agency)
                                            _AgytabsFilter.Add(Entity);
                                    }
                                    else if (TempCode == "******")
                                        _AgytabsFilter.Add(Entity);
                                }
                            }


                            i += 6;
                            if (i >= Temp.Length)
                                Can_Continue = false;
                        }

                    }
                }
                _AgytabsFilter = _AgytabsFilter.OrderByDescending(u => u.Active).ThenBy(u => u.Desc).ToList();
            }

            return _AgytabsFilter;
        }

        public static string GetPhoneNo(string phnno)
        {
            try
            {
                if (phnno != string.Empty && phnno.Trim().Length > 0)
                {
                    MaskedTextBox mskPhn = new MaskedTextBox();
                    mskPhn.Mask = "000-000-0000";
                    mskPhn.Text = phnno;

                    return mskPhn.Text.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }



        #endregion

        #region Newly Implemented Common Functions

        /***********************************************************************/
        /*   JWT- Token for Gitbook URLs
         *   Kranthi on 11/23/2022 
        /***********************************************************************/
        private static string GenerateTokenKey(string _ModuleID)
        {
            string RestokenKey = "";
            string idtoken = "";
            if (_ModuleID == "01")
                idtoken = "acce26b9-b309-4d50-99a7-4f51455b535d";   // Admin Module
            if (_ModuleID == "03")
                idtoken = "50b01a0a-ad56-41e7-9065-cddcb9ed0f88";   // Case Management
            if (_ModuleID == "07")
                idtoken = "7da54d0f-f475-435f-bba3-970f8de7ea25";   // CEAP Module
            if (_ModuleID == "CMN")
                idtoken = "b8a212a4-e367-4b30-9393-f626fb5b692b";   // Common Screens in all Modules
            try
            {
                if (idtoken != "")
                {
                    //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(idtoken));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        null,
                      //_config["Jwt:Issuer"],
                      //_config["Jwt:Issuer"],
                      null,
                      null,
                      expires: DateTime.Now.AddMinutes(60),
                      signingCredentials: credentials);

                    RestokenKey = new JwtSecurityTokenHandler().WriteToken(token);
                }
            }
            catch
            {
            }


            return RestokenKey;
        }

        /*********************************************************************
         * Create GitBook URLs based on Form Name and Subpages Number
         * Kranthi on 11/23/2022
         ******************************************************************/
        public static string BuildHelpURLS(string FormName, int PageNumber, string _ModuleID)
        {
            string resURL = "";
            //string _moduleURL = "";
            //if (_ModuleID == "01")
            //    _moduleURL = "case - management-module";
            //if (_ModuleID == "01")
            //    _moduleURL = "case - management-module";
            //if (_ModuleID == "01")
            //    _moduleURL = "case - management-module";

            string gitbookSpaceURL = "https://capsystems-1.gitbook.io/";  //"https://capsystems-1.gitbook.io/captain-help-1/";
            string PageURL = "";

            try
            {
                switch (FormName)
                {
                    #region ADMINISTRATION
                    #region SCREENS
                    case "ADMN0005": //Assign User Privilages

                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/assign-user-accounts-and-privileges";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/assign-user-accounts-and-privileges/add-edit-new-use";
                        if (PageNumber == 2) PageURL = "system-administration-module/system-administration/screens/assign-user-accounts-and-privileges/assign-user-privileges";
                        if (PageNumber == 3) PageURL = "system-administration-module/system-administration/screens/assign-user-accounts-and-privileges/copy-a-template-user";

                        break;
                    case "ADMN0006": //Agency Table Codes
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/agency-table-codes";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/agency-table-codes/add-edit-table-code-entry";
                        break;
                    case "FIXSSNUM": //ClientID Auidt and Review
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/client-id-audit-and-review";
                        break;
                    case "ADMN0022": //Contacts and Service Custom Questions
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/contacts-and-services-custom-questions";
                        break;
                    case "ADMN0009": //Hierarchy Definition
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/hierarchy-definition";
                        break;
                    case "ADMN0010": //Master Poverty Guidelines 
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/master-poverty-guidelines";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/master-poverty-guidelines/federal-poverty-chart";
                        if (PageNumber == 2) PageURL = "system-administration-module/system-administration/screens/master-poverty-guidelines/hud";
                        if (PageNumber == 3) PageURL = "system-administration-module/system-administration/screens/master-poverty-guidelines/cmi";
                        if (PageNumber == 4) PageURL = "system-administration-module/system-administration/screens/master-poverty-guidelines/smi";
                        break;
                    case "ADMN0012": //Agency Control file Maintanence
                        if (PageNumber == 0) PageURL = "";
                        break;
                    case "ADMN0013": //Zipcode
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/zip-code-file-maintenace";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/zip-code-file-maintenace/add-edit-zip-code";
                        break;
                    case "ADMNCONT":    //Incomplete Intake Control
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/incomplete-intake-controls";
                        break;
                    case "CASE0008":    //Field Control Maintenance
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/fields-control-maintenance";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/fields-control-maintenance/add-edit-fields-control-in-a-specific-hierarchy";
                        if (PageNumber == 2) PageURL = "system-administration-module/system-administration/screens/fields-control-maintenance/custom-fields-definition";
                        break;
                    case "CASE0009": //Eligibilty Criteria
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/eligibility-criteria";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/eligibility-criteria/add-a-question-to-a-group";
                        if (PageNumber == 2) PageURL = "system-administration-module/system-administration/screens/eligibility-criteria/group-definitions";
                        break;
                    case "CASE0011": // Agency Referral Database Screen
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/agency-referral-database";
                        break;
                    case "ADMN0015":    //Site and room maintenance
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/site-and-room-maintenance";
                        break;
                    case "ADMN0020":    //SP Admin Screen
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/service-plan-admin-screen";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/service-plan-admin-screen/service-plan-admin";
                        if (PageNumber == 2) PageURL = "system-administration-module/system-administration/screens/service-plan-admin-screen/add-service-outcome-to-service-plan";
                        if (PageNumber == 3) PageURL = "system-administration-module/system-administration/screens/service-plan-admin-screen/validate-a-service-plan";
                        break;

                    case "TMS00009":   //Vendor Maintenance
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/vendor-maintenance";
                        break;
                    case "ADMN0016":    //Master Table Maintenance
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/master-table-maintenance";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/master-table-maintenance/service-table";
                        if (PageNumber == 2) PageURL = "system-administration-module/system-administration/screens/master-table-maintenance/outcome-table";
                        if (PageNumber == 3) PageURL = "system-administration-module/system-administration/screens/master-table-maintenance/outcome-indicators";
                        if (PageNumber == 4) PageURL = "system-administration-module/system-administration/screens/master-table-maintenance/individual-household-characteristics";
                        if (PageNumber == 5) PageURL = "system-administration-module/system-administration/screens/master-table-maintenance/services";
                        if (PageNumber == 6) PageURL = "system-administration-module/system-administration/screens/master-table-maintenance/ranking-categories";
                        break;
                    case "CASE0007":    //Program Definition
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/program-definition";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/program-definition/program-information";
                        if (PageNumber == 2) PageURL = "system-administration-module/system-administration/screens/program-definition/poverty-guidelines";
                        if (PageNumber == 3) PageURL = "system-administration-module/system-administration/screens/program-definition/program-switches";
                        if (PageNumber == 4) PageURL = "system-administration-module/system-administration/screens/program-definition/income-and-relation";
                        if (PageNumber == 5) PageURL = "system-administration-module/system-administration/screens/program-definition/hs-ar-controls";
                        break;
                    case "FIXFAMID":    //HouseholdID Audit and Review
                        if (PageNumber == 0) PageURL = "";
                        break;

                    case "MAT00001":    //Matrix/Scales Definitions
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/matrix-scales-definitions";
                        if (PageNumber == 1) PageURL = "system-administration-module/system-administration/screens/matrix-scales-definitions/add-edit-matrix";
                        if (PageNumber == 2) PageURL = "system-administration-module/system-administration/screens/matrix-scales-definitions/add-edit-benchmarks";
                        if (PageNumber == 3) PageURL = "system-administration-module/system-administration/screens/matrix-scales-definitions/add-edit-scales";
                        if (PageNumber == 4) PageURL = "system-administration-module/system-administration/screens/matrix-scales-definitions/add-edit-outcomes";
                        if (PageNumber == 5) PageURL = "system-administration-module/system-administration/screens/matrix-scales-definitions/add-edit-questions";
                        if (PageNumber == 6) PageURL = "system-administration-module/system-administration/screens/matrix-scales-definitions/add-edit-reasons";
                        break;
                    case "MAT00002":    //Matrix/Scales Score Sheets
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/screens/matrix-scales-score-sheet";
                        break;
                    #endregion
                    #region REPORTS
                    case "ADMNB001": // Report :: Master Tables List
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/reports/master-table-lists";
                        break;
                    case "ADMNB002":    // Report:: User tree structure
                        if (PageNumber == 0) PageURL = "system-administration-module/system-administration/reports/user-tree-structure";
                        break;
                    case "ADMNB005":    // Report:: Image Types Report
                        if (PageNumber == 0) PageURL = "";
                        break;
                    #endregion
                    #endregion

                    #region CASEMANAGEMENT
                    #region SCREENS
                    case "ADVMMSEARCH":
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-search-feature/search-using-advanced-search";
                        break;

                    case "APPT0001":    // Appointment Template
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/appointment-template";
                        break;
                    case "APPT0002": //Appointment Schedule
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/appointments-schedule"; 
                        break;
                    case "APPT0003":    // Appointment-Reserve Schedule
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/appointment-reserve-schedule";
                        break;
                    case "CASE0006":    // Service Posting
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/service-posting";
                        if (PageNumber == 1) PageURL = "captain-common-screens-and-reports/common-captain-screens/service-posting/add-edit-a-service-plan";
                        if (PageNumber == 2) PageURL = "captain-common-screens-and-reports/common-captain-screens/service-posting/add-edit-a-referral";
                        if (PageNumber == 3) PageURL = "captain-common-screens-and-reports/common-captain-screens/service-posting/add-edit-a-contact";
                        if (PageNumber == 4) PageURL = "captain-common-screens-and-reports/common-captain-screens/service-posting/add-edit-a-service-plan/add-edit-services-ceap";
                        if (PageNumber == 5) PageURL = "captain-common-screens-and-reports/common-captain-screens/service-posting/add-edit-a-service-plan/add-edit-services-non-ceap";
                        if (PageNumber == 6) PageURL = "captain-common-screens-and-reports/common-captain-screens/service-posting/add-edit-a-service-plan/add-edit-outcomes";
                        break;
                    case "CASE2001":        // Client intake
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake";
                        if (PageNumber == 1) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/add-edit-applicant/enter-applicant-details";
                        if (PageNumber == 2) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/add-edit-applicant/enter-address-and-intake-details";
                        if (PageNumber == 3) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/add-edit-applicant/complete-custom-questions";
                        if (PageNumber == 4) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/add-edit-applicant/housing-and-asset-inquiry";
                        if (PageNumber == 5) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/add-edit-applicant/employment-information";
                        if (PageNumber == 6) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/add-edit-applicant/complete-pre-assessment";
                        if (PageNumber == 7) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/add-edit-household-member";
                        if (PageNumber == 8) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/add-edit-household-member/complete-custom-questions";
                        if (PageNumber == 9) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/add-edit-household-member/enter-employment-information";
                        break;
                    case "CASE2003":        // Client intake
                        _ModuleID = "CMN";
                        if (PageNumber == 2) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/income-verification";
                        break;
                    case "CASE2004":        // SIM - Service Integration Matrix
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/sim-service-integration-matrix";
                        break;
                    case "CASINCOM":
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-intake/income-details";
                        break;

                    case "CASE0028":    // Client Followup
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-follow-up-solution/client-follow-up-on-tool";
                        break;
                    case "CASE0027":    // Client Followup Search tool
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-follow-up-solution";
                        if (PageNumber == 1) PageURL = "captain-common-screens-and-reports/common-captain-screens/client-follow-up-solution/client-follow-up-on-search-tool";
                        break;
                    case "MAT00003":    // Matrix/Scales Assessments
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/matrix-scales-assessments";
                        break;
                    case "PIP00000":    // Public Intake Portal Hub
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-screens/public-intake-portal-hub";
                        break;

                    #endregion
                    #region REPORTS
                    case "APPTB001": //Appointment Schedule Report
                        _ModuleID = "CMN";
                        if (PageNumber == 0) PageURL = "captain-common-screens-and-reports/common-captain-reports/appointment-schedule-report";
                        break;
                    case "CASB0007":    // Funnel Report
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/funnel-report";
                        break;
                    case "CASB0008":    // Customer Intake Quality Control
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/customer-intake-quality-control";
                        break;
                    case "CASB0012":    // Adhoc Report
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/ad-hoc-report";
                        break;
                    case "CASB0013":    // Agency Wide Activity Report
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/agency-wide-service-report";
                        break;
                    case "CASB0530":    // Ranking/Risk Assessment Report
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/ranking-risk-assessment";
                        break;
                    case "MATB1002":    // Matrix Scale Measures Report
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/matrix-scales-measures-report";
                        break;
                    case "PIPB0001":    //PIP Registration Report
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/pip-registration-report";
                        break;
                    case "PIPB0002":    //PIP Intake Report
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/pip-intake-report";
                        break;
                    case "RNGB0004":    //ROMA Individual/Household Characteristics
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/roma-individual-household-characteristics";
                        break;
                    case "RNGB0005":    // Program Service And Outcome Report
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/program-service-and-outcomes-report";
                        break;
                    case "RNGB0014":    //ROMA Outcome Indicators
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/roma-outcome-indicators";
                        break;
                    case "RNGS0014":    //ROMA Services
                        _ModuleID = "03";
                        if (PageNumber == 0) PageURL = "case-management-module/case-management-module/reports/roma-services-report";
                        break;
                    #endregion
                    #endregion

                    #region CEAP Program
                    #region Screens
                    case "CASE0016":    // Benifit Maintenance & Usage Posting
                        _ModuleID = "07";
                        if (PageNumber == 0) PageURL = "ceap-module/ceap-module/screens/benefit-maintenance-and-usage-posting";
                        break;
                    case "CASE0026":    // Budget Maintenance
                        _ModuleID = "07";
                        if (PageNumber == 0) PageURL = "ceap-module/ceap-module/screens/budget-maintenance";
                        break;
                    case "CASE0021":    // Service Payment Adjustment
                        _ModuleID = "07";
                        if (PageNumber == 0) PageURL = "ceap-module/ceap-module/screens/service-payment-adjustment";
                        break;
                    #endregion
                    #region Reports
                    case "TMSB0034":    // Funding Source Report
                                _ModuleID = "07";
                                if (PageNumber == 0) PageURL = "ceap-module/ceap-module/reports/funding-source-report";
                                break;
                            case "CEAPB002":    // Performance Measures Data
                                _ModuleID = "07";
                                if (PageNumber == 0) PageURL = "ceap-module/ceap-module/reports/performance-measures-data";
                                break;
                            case "CASB0019":    // Pledge Sheet/Bundling
                                _ModuleID = "07";
                                if (PageNumber == 0) PageURL = "ceap-module/ceap-module/reports/pledge-sheet-bundling";
                                break;
                            case "CASB0020":    // Request for Payment Process
                                _ModuleID = "07";
                                if (PageNumber == 0) PageURL = "ceap-module/ceap-module/reports/request-for-payment";
                                break;
                            case "CASB0021":    // Ser/Pay Adjustment Report
                                _ModuleID = "07";
                                if (PageNumber == 0) PageURL = "ceap-module/ceap-module/reports/ser-pay-adjustment-report";
                                break;
                            case "CASB0017":    // Usage Report
                                _ModuleID = "07";
                                if (PageNumber == 0) PageURL = "ceap-module/ceap-module/reports/usage-report";
                                break;
                        #endregion
                    #endregion


                    default:
                        if (PageNumber == 0) PageURL = "";
                        break;
                }

                if (PageURL != "")
                    resURL = gitbookSpaceURL + PageURL + "?jwt_token=" + GenerateTokenKey(_ModuleID);
            }
            catch
            {

            }

            return resURL;

        }

        /* Gridview Filter and Select the row on user key press*/

        public static void KeyPress(DataGridView dataGrid, int cellIndex, char keyChar)
        {
            #region Wisej Code
            //if (Char.IsLetterOrDigit(keyChar))
            //{
            //    var search = keyChar.ToString();
            //    var searchColumn = dataGrid.Columns[cellIndex];

            //    var row = dataGrid.Rows.FirstOrDefault(r =>
            //    {
            //        return r[searchColumn].Value.ToString().StartsWith(search, StringComparison.CurrentCultureIgnoreCase);
            //    });


            //    if (row != null)
            //    {
            //        dataGrid.ClearSelection();

            //        row.Selected = true;
            //        dataGrid.ScrollRowIntoView(row);
            //    }
            //}
            #endregion;

            #region Kranthi's Code

            if (Char.IsLetterOrDigit(keyChar))
            {

                List<DataGridViewRow> SelectedgvRows = (from c in dataGrid.Rows.Cast<DataGridViewRow>().ToList()
                                                        where ((c.Cells[cellIndex]).Value.ToString().ToUpper().StartsWith(keyChar.ToString().ToUpper()))
                                                        select c).ToList();
                if (SelectedgvRows.Count > 0)
                {
                    int i = 0;
                    foreach (DataGridViewRow dgvRow in SelectedgvRows)
                    {

                        if (i == 1)
                        {
                            dataGrid.ClearSelection();
                            dgvRow.Selected = true;
                            dataGrid.ScrollRowIntoView(dgvRow);
                            i = 0;
                            return;
                        }
                        if (dgvRow.Selected == true)
                            i = 1;
                    }

                    dataGrid.ClearSelection();
                    foreach (DataGridViewRow dgvRow in SelectedgvRows)
                    {
                        dgvRow.Selected = true;
                        dataGrid.ScrollRowIntoView(dgvRow);
                        break;
                    }
                }
                else
                {
                    AlertBox.Show("Key not found!", MessageBoxIcon.Warning, null, System.Drawing.ContentAlignment.BottomRight);
                }
            }

            #endregion
        }


        /* Search a string value in Datagridview by navigating Previous and Next */
        public static void NavigateStringSearch(DataGridView dataGrid, int cellIndex, string strSearchKey, string searchType)
        {
            if (strSearchKey != "")
            {

                List<DataGridViewRow> SelectedgvRows = (from c in dataGrid.Rows.Cast<DataGridViewRow>().ToList()
                                                        where ((c.Cells[cellIndex]).Value.ToString().ToUpper().Contains(strSearchKey.ToString().ToUpper()))
                                                        select c).ToList();
                if (SelectedgvRows.Count > 0)
                {
                    int i = 0; int currentIndex = 0;
                    //if (searchType == "N")
                    //{
                    foreach (DataGridViewRow dgvRow in SelectedgvRows)
                    {

                        if (i == 1)
                        {
                            dataGrid.ClearSelection();

                            if (searchType == "N")
                                dgvRow.Selected = true;
                            else if (searchType == "P")
                            {
                                currentIndex = dgvRow.ClientIndex;
                                if ((currentIndex - 1 - 1) >= 0)
                                    SelectedgvRows[currentIndex - 1 - 1].Selected = true;
                                else
                                    SelectedgvRows[0].Selected = true;
                            }

                            dataGrid.ScrollRowIntoView(dgvRow);
                            i = 0;
                            return;
                        }
                        if (dgvRow.Selected == true)
                            i = 1;
                    }
                    //}
                    //if (searchType == "P")
                    //{
                    //    dataGrid.ClearSelection();

                    //    currentIndex= dataGrid.SelectedRows[0]

                    //    if ((currentIndex - 1) >= 0)
                    //        SelectedgvRows[currentIndex - 1].Selected = true;
                    //    else
                    //        SelectedgvRows[0].Selected = true;
                    //    //for (int x = SelectedgvRows.Count; x > 0; x--)
                    //    //{
                    //    //    dataGrid.ClearSelection();
                    //    //}
                    //}

                    dataGrid.ClearSelection();
                    foreach (DataGridViewRow dgvRow in SelectedgvRows)
                    {
                        dgvRow.Selected = true;
                        dataGrid.ScrollRowIntoView(dgvRow);
                        break;
                    }
                }
                else
                {
                    AlertBox.Show("Key not found!", MessageBoxIcon.Warning, null, System.Drawing.ContentAlignment.BottomRight);
                }
            }

        }
        #endregion


        public static int BuildHIEGrid(List<HierarchyEntity> oSPHierachies, DataGridView grdViewControl)
        {
            int _rowIndex = 0;
            try
            {
                foreach (HierarchyEntity spHIE in oSPHierachies)
                {
                    string _spAgy = (spHIE.Agency == "" ? "**" : spHIE.Agency);
                    string _spDept = (spHIE.Dept == "" ? "**" : spHIE.Dept);
                    string _spProg = (spHIE.Prog == "" ? "**" : spHIE.Prog);

                    string _spcode = _spAgy + "-" + _spDept + "-" + _spProg;
                    string _spHIEName = spHIE.HirarchyName.ToString();

                    _rowIndex = grdViewControl.Rows.Add("", "", _spcode, _spHIEName);
                    grdViewControl.Rows[_rowIndex].Tag = spHIE;
                    if (spHIE.UsedFlag.Equals("Y"))
                    {
                        grdViewControl.Rows[_rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            catch (Exception ex) { }

            return _rowIndex;

        }

        public static int BuildSerHIEGrid(List<HierarchyEntity> oSPHierachies, DataGridView grdViewControl)
        {
            int _rowIndex = 0;
            try
            {
                foreach (HierarchyEntity spHIE in oSPHierachies)
                {
                    string _spAgy = (spHIE.Agency == "" ? "**" : spHIE.Agency);
                    string _spDept = (spHIE.Dept == "" ? "**" : spHIE.Dept);
                    string _spProg = (spHIE.Prog == "" ? "**" : spHIE.Prog);

                    string _spcode = _spAgy + "-" + _spDept + "-" + _spProg;
                    string _spHIEName = spHIE.HirarchyName.ToString();

                    _rowIndex = grdViewControl.Rows.Add("", "", _spcode, _spHIEName);
                    grdViewControl.Rows[_rowIndex].Tag = spHIE;
                    if (spHIE.UsedFlag.Equals("Y"))
                    {
                        grdViewControl.Rows[_rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            catch (Exception ex) { }

            return _rowIndex;

        }

        public static string GetAlphanumericCode()
        {
            string result = "";
            try
            {
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random random = new Random();
                result = new string(
                    Enumerable.Repeat(chars, 25)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray()
                );
            }
            catch { }

            return result;
        }

        public static string getDBName(string strcon)
        {
            string DBName = string.Empty;
            try 
            {
                //string strcon = System.Configuration.ConfigurationManager.ConnectionStrings["CMMSo"].ConnectionString.ToString();
                var connectionString = new System.Data.SqlClient.SqlConnectionStringBuilder(strcon);

                string dsrc = connectionString.DataSource;
                DBName = connectionString.InitialCatalog;
            }
            catch { }


            return DBName;
        }

    }
}

