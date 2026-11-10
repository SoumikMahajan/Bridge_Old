using POMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace POMS.Controllers
{
    public class SystemAdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FAQAdmin()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult SystemAdminActivity()
        {
            string UserID = Convert.ToString(Session["UserID"]);
            ViewData["Country"] = getAllCountryName();
            ViewData["GetAllCountry"] = getAllCountry1();
            ViewData["GetAllCurency"] = GetAllCurrency1();
            return View();
        }

        public string BindAllSystemOperation()
        {
            string str = "";
            return str;
        }

        public ActionResult EditTermsConditions()
        {
            return View();
        }

        public ActionResult EditTermsCondition()
        {
            return View();
        }

        public string getAllTerms()
        {
            string jsnstr = "{\"data\":[";
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions DBFunctions = new DBFunctions();
            TermsConditions obj = new TermsConditions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "getAllTermsCondition");
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            while (dr.Read())
            {
                obj.TermsID = Convert.ToString(dr["TermID"]);
                obj.TermsCondition = Convert.ToString(dr["Terms"]);
                obj.Status = Convert.ToString(dr["Status"]);

                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string addTerms(string Terms)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "addTermsCondition");
            ht.Add("@UpdatedVal", Terms);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateTerms(string Terms, string TermsID)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "updateTermsCondition");
            ht.Add("@UpdatedVal", Terms);
            ht.Add("@ID", TermsID);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string ActivateDeActivateTerms(string ID, string Status)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Activate_Deactivate_Terms");
            ht.Add("@ID", ID);
            ht.Add("@Status", Status);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public ActionResult EditCurrency()
        {
            return View();
        }

        public string getAllCurrency()
        {
            string jsnstr = "{\"data\":[";
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions DBFunctions = new DBFunctions();
            CurrencyDetail obj = new CurrencyDetail();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "getAllCurrency");
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            while (dr.Read())
            {
                obj.CurrencyID = Convert.ToString(dr["CurrencyID"]);
                obj.Currency = Convert.ToString(dr["Currency"]);
                obj.CurrencyCode = Convert.ToString(dr["CurrencyCode"]);
                obj.SortOrder = Convert.ToString(dr["SortOrder"]);
                obj.Status = Convert.ToString(dr["Status"]);

                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string ActivateDeActivateCurrency(string ID, string Status)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Activate_Deactivate_Currency");
            ht.Add("@ID", ID);
            ht.Add("@Status", Status);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string addCurrency(string CurrencyName, string CurrencyCode)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "addCurrency");
            ht.Add("@Currency", CurrencyName);
            ht.Add("@CurrencyCode", CurrencyCode);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateCurrency(string Currency, string CurrencyCode, string CurrencyID, string Status)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "updateCurrency");
            ht.Add("@Currency", Currency);
            ht.Add("@CurrencyCode", CurrencyCode);
            ht.Add("@Status", Status);
            ht.Add("@ID", CurrencyID);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public ActionResult EditShipMethod()
        {
            return View();
        }

        public string getAllShipmentMethod()
        {
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions DBFunctions = new DBFunctions();
            ShipMethodDtl obj = new ShipMethodDtl();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "getAllShipmethod");
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            while (dr.Read())
            {
                obj.ShipID = Convert.ToString(dr["ShipID"]);
                obj.ShipMethod = Convert.ToString(dr["ShipMethod"]);
                obj.Status = Convert.ToString(dr["Status"]);

                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string addShipmentMethod(string ShipMethod)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "addShipmentMethodName");
            ht.Add("@UpdatedVal", ShipMethod);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateShipMethod(string ShipMethod, string ShipMethodID, string Status)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "updateShipMethod");
            ht.Add("@UpdatedVal", ShipMethod);
            ht.Add("@Status", Status);
            ht.Add("@ID", ShipMethodID);

            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string ActivateDeActivateShipMethod(string ID, string Status)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Activate_Deactivate_ShipMethod");
            ht.Add("@ID", ID);
            ht.Add("@Status", Status);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public ActionResult EditUOM()
        {
            return View();
        }

        public string getAllUOM()
        {
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions DBFunctions = new DBFunctions();
            UOMDtl obj = new UOMDtl();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "getAllUOM");
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            while (dr.Read())
            {
                obj.UOM_ID = Convert.ToString(dr["UOM_ID"]);
                obj.UOM = Convert.ToString(dr["UOM"]);
                obj.Status = Convert.ToString(dr["Status"]);
                obj.IsDefault = Convert.ToString(dr["IsDefault"]);

                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string addUOM(string UOM)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "addUOMName");
            ht.Add("@UpdatedVal", UOM);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateUOM(string UOM, string UOMID, string Dstatus, string Status)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "updateUOM");
            ht.Add("@UpdatedVal", UOM);
            ht.Add("@Status", Status);
            ht.Add("@State", Dstatus);
            ht.Add("@ID", UOMID);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string ActivateDeActivateUOM(string ID, string Status)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "Activate_Deactivate_UOM");
            ht.Add("@ID", ID);
            ht.Add("@Status", Status);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string ActivateDeActivateDefault(string WantToDefault, string UOM_ID)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Activate_Deactivate_Default");
            ht.Add("@ID", UOM_ID);
            ht.Add("@Status", WantToDefault);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public ActionResult EditItemApproval()
        {
            return View();
        }

        public string getAllItemApproval()
        {
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions DBFunctions = new DBFunctions();
            ItemApprovalDtl obj = new ItemApprovalDtl();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "getAllItemApproval");
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            while (dr.Read())
            {
                obj.ItemApprovalID = Convert.ToString(dr["ItemApprovalID"]);
                obj.ItemName = Convert.ToString(dr["ItemName"]);
                obj.Status = Convert.ToString(dr["Status"]);
                obj.ItemShortName = Convert.ToString(dr["ItemShortName"]);
                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }

            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string addItemApproval(string ItemApproval,string ItemShortName)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "addItemApprovalName");
            ht.Add("@UpdatedVal", ItemApproval);
            ht.Add("@CountryCode", ItemShortName);

            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateItemApproval(string ItemApproval, string ItemApprovalID, string Status,string ItemShortName)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "updateItemApproval");
            ht.Add("@UpdatedVal", ItemApproval);
            ht.Add("@Status", Status);
            ht.Add("@ID", ItemApprovalID);
            ht.Add("@CountryCode", ItemShortName);

            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string ActivateDeActivateItemApproval(string ID, string Status)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "Activate_Deactivate_ItemApproval");
            ht.Add("@ID", ID);
            ht.Add("@Status", Status);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public ActionResult EditClassification()
        {
            return View();
        }

        public string getAllClassification()
        {
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions DBFunctions = new DBFunctions();
            ClassificationDtl obj = new ClassificationDtl();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "getAllClassification");
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            while (dr.Read())
            {
                obj.ClassificationID = Convert.ToString(dr["Classification_ID"]);
                obj.Classification = Convert.ToString(dr["Classification"]);
                obj.Status = Convert.ToString(dr["Status"]);

                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string addClassification(string Classification)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "addClassificationName");
            ht.Add("@UpdatedVal", Classification);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateClassification(string Classification, string ClassificationID, string status)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "updateClassification");
            ht.Add("@Status", status);
            ht.Add("@UpdatedVal", Classification);
            ht.Add("@ID", ClassificationID);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string ActivateDeActivateClassification(string ID, string Status)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Activate_Deactivate_Classification");
            ht.Add("@ID", ID);
            ht.Add("@Status", Status);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public ActionResult EditCountry()
        {
            return View();
        }

        public string getAllCountry()
        {
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions DBFunctions = new DBFunctions();
            CountryDtl obj = new CountryDtl();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "getAllCountry");
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            while (dr.Read())
            {
                obj.CountryID = Convert.ToString(dr["CountryID"]);
                obj.CountryName = Convert.ToString(dr["Country"]);
                obj.CountryCode = Convert.ToString(dr["CountryCode"]);
                obj.Status = Convert.ToString(dr["Status"]);
                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string addCountry(string CountryName, string CountryCode)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "addCountryName");
            ht.Add("@Country", CountryName);
            ht.Add("@CountryCode", CountryCode);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateCountry(string CountryName, string CountryCode, string CountryID, string Status)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "updateCountry");
            ht.Add("@Country", CountryName);
            ht.Add("@CountryCode", CountryCode);
            ht.Add("@Status", Status);
            ht.Add("@ID", CountryID);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string ActivateDeActivateCountry(string ID, string Status)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Activate_Deactivate_Country");
            ht.Add("@ID", ID);
            ht.Add("@Status", Status);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);

            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public ActionResult EditState()
        {
            ViewData["Country"] = getAllCountryName();
            return View();
        }

        public string getAllCountryName()
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GetAllCountry");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            result = "<option value=0 name=ddlCrntry_0>--Select Country--</option>";
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["CountryID"]) + " data-name=ddlCrntry_" + Convert.ToString(dr["CountryID"]) + " name=ddlCrntry_" + Convert.ToString(dr["CountryID"]) + ">" + Convert.ToString(dr["Country"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string getAllStateDetail(string CountryID)
        {
            string jsnstr = "{\"data\":[";

            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions db = new DBFunctions();
            StateDtl obj = new StateDtl();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "GetAllStateByCountryID");
            ht.Add("@CountryCode", CountryID);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);

            while (dr.Read())
            {
                obj.StateID = Convert.ToString(dr["StateID"]);
                obj.StateName = Convert.ToString(dr["State"]);
                obj.StateCode = Convert.ToString(dr["StateCode"]);
                obj.Status = Convert.ToString(dr["Status"]);
                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }
            dr.Close();
            jsnstr += "]}";
            return jsnstr;

        }

        public string addState(string StateName, string StateCode, string CountryCode)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "addStateName");
            ht.Add("@State", StateName);
            ht.Add("@StateCode", StateCode);
            ht.Add("@CountryCode", CountryCode);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateState(string StateName, string StateCode, string StateID, string Status)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "updateState");
            ht.Add("@State", StateName);
            ht.Add("@StateCode", StateCode);
            ht.Add("@Status", Status);
            ht.Add("@ID", StateID);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string ActivateDeActivateState(string ID, string Status)
        {
            string str = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@mode", "Activate_Deactivate_State");
            ht.Add("@ID", ID);
            ht.Add("@Status", Status);
            dr = DBFunctions.SysFetchData("POMS_SysAdmin_Operation", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["ActivationToken"]);
            }
            dr.Close();
            return str;
        }

        public string getAllCommentByProjectID(string ProjectID)
        {
            string str = "";
            Hashtable ht = new Hashtable();
            ht.Clear();
            return str;
        }

        public string CheckSessionTimeExists()
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Exists");
            ht.Add("@session_time", 0);
            dr = DBFunctions.SysFetchData("POMS_SessionManagement", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;

        }

        public string InsertSessionTime(string time)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Insert");
            ht.Add("@session_time", Convert.ToInt32(time));
            dr = DBFunctions.SysFetchData("POMS_SessionManagement", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;

        }

        public string SelectSessionTime()
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Select");
            ht.Add("@session_time", 0);
            dr = DBFunctions.SysFetchData("POMS_SessionManagement", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["SessionTime"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateSessionTime(string time)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "Update");
            ht.Add("@session_time", Convert.ToInt32(time));
            dr = DBFunctions.SysFetchData("POMS_SessionManagement", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;

        }

        public string CheckInvoiveAddressExists()
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "CHECKINVOICEADDRESSEXISTS");

            dr = DBFunctions.SysFetchData("POMS_InvoiceAddress_Management", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;

        }

        public string InsertInvoiceAddress(string InvoiceAddress, string CountryID, string StateID, string City, string PostalCode)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "InsertInvoiceAddress");
            ht.Add("@InvoiceAddress", InvoiceAddress);
            ht.Add("@StateID", StateID);
            ht.Add("@CountryID", CountryID);
            ht.Add("@City", City);
            ht.Add("@PostalCode", PostalCode);

            dr = DBFunctions.SysFetchData("POMS_InvoiceAddress_Management", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;

        }

        public string SelectInvoiceAddress()
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GETINVOICEADDRESS");
            dr = DBFunctions.SysFetchData("POMS_InvoiceAddress_Management", ref ht);
            if (dr.Read())
            {
                str += Convert.ToString(dr["InvoiceAddress"]) + "^";
                str += Convert.ToString(dr["CountryID"]) + "^";
                str += Convert.ToString(dr["Country"]) + "^";
                str += Convert.ToString(dr["StateID"]) + "^";
                str += Convert.ToString(dr["State"]) + "^";
                str += Convert.ToString(dr["City"]) + "^";
                str += Convert.ToString(dr["PostalCode"]);
            }
            dr.Close();
            return str;
        }

        public string UpdateInvoiceAddress(string InvoiceAddress, string CountryID, string StateID, string City, string PostalCode)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "UpdateInvoiceAddress");
            ht.Add("@InvoiceAddress", InvoiceAddress);
            ht.Add("@StateID", StateID);
            ht.Add("@CountryID", CountryID);
            ht.Add("@City", City);
            ht.Add("@PostalCode", PostalCode);

            dr = DBFunctions.SysFetchData("POMS_InvoiceAddress_Management", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;
        }

        public string getAllCountry1()
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GetAllCountry");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            result = "<option value=0 name=ddlCrntry_0>--Select Country--</option>";
            while (dr.Read())
            {
                result += "<option value=" + Convert.ToString(dr["CountryID"]) + " data-name=ddlCrntry_" + Convert.ToString(dr["CountryID"]) + " name=ddlCrntry_" + Convert.ToString(dr["CountryID"]) + ">" + Convert.ToString(dr["Country"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        public string InsertBankAccount(string BankAccount, string IsActive, string Desc, string CurrCode)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "INSERTBANKACCOUNT");
            ht.Add("@BankAccount", BankAccount);
            ht.Add("@IsActive", IsActive);
            ht.Add("@Desc", Desc);
            ht.Add("@CurrCode", CurrCode);
            dr = DBFunctions.SysFetchData("POMS_BankAccount_Management", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;
        }

        public string SelectBankAccount()
        {
            string jsnstr = "{\"data\":[";
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions DBFunctions = new DBFunctions();
            BankAccountManagement obj = new BankAccountManagement();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "GETBANKACCOUNT");
            dr = DBFunctions.SysFetchData("POMS_BankAccount_Management", ref ht);
            while (dr.Read())
            {
                obj.BankAccountID = Convert.ToString(dr["Fld_ID"]);
                obj.Bank_Account = Convert.ToString(dr["Bank_Account"]);
                obj.IsActive = Convert.ToString(dr["IsActive"]);
                obj.Desc = Convert.ToString(dr["Desc"]);
                obj.Currency = Convert.ToString(dr["CurrencyCode"]);
                if (jsnstr == "{\"data\":[")
                {
                    jsnstr += js.Serialize(obj);
                }
                else
                {
                    jsnstr += "," + js.Serialize(obj);
                }
            }

            dr.Close();
            jsnstr += "]}";
            return jsnstr;
        }

        public string UpdateBankAccount(string BankAccount, string IsActive, string ID, string Desc, string CurrCode)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@mode", "UPDATEBANKACCOUNT");
            ht.Add("@BankAccount", BankAccount);
            ht.Add("@IsActive", IsActive);
            ht.Add("@Desc", Desc);
            ht.Add("@CurrCode", CurrCode);
            ht.Add("@ID", ID);

            dr = DBFunctions.SysFetchData("POMS_BankAccount_Management", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;

        }
        public async Task<string> UpdateQBSAccount()
        {
            /* Updating QBS Account */
            QuickBookController objQBC = new QuickBookController();
            string result = await objQBC.GetUpdateAccountsQBS();
            return result;
        }
        //public string UpdateQBSAccount()
        //{
        //    /* Updating QBS Account */
        //    QuickBookController objQBC = new QuickBookController();
        //    objQBC.GetUpdateAccountsQBS();
        //    return "success";
        //}

        [HttpPost]
        public string Upload()
        {
            string response = "SUCCESS";
            var fileName = "";
            string fname = "";
            string allfilename = "";
            string[] files = System.IO.Directory.GetFiles(Server.MapPath("~/Assets/SystemTermsFiles"));

            if (files.Length > 0)
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Assets/SystemTermsFiles"));
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            for (int i = 0; i < Request.Files.Count; i++)
            {
                string ext = "";
                var file = Request.Files[i];
                ext = Path.GetFileName(file.FileName).Split('.')[1];
                fname = Path.GetFileName(file.FileName).Split('.')[0].Replace(".", "");
                fileName = "SystemTermsFile^" + fname + "_" + i + "." + ext;
                if (allfilename.Trim() == "")
                {
                    allfilename = fileName;
                }
                else
                {
                    allfilename += "," + fileName;
                }
                var path = Path.Combine(Server.MapPath("~/Assets/SystemTermsFiles"), fileName);
                file.SaveAs(path);
            }
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();
            ht.Add("@filename", fileName);

            dr = DBFunctions.SysFetchData("POMS_UploadTermsCondition", ref ht);
            if (dr.Read())
            {

            }
            dr.Close();
            return response;
        }

        public string CheckTermsFileExists()
        {
            string result = "";
            string[] dirs = System.IO.Directory.GetDirectories(Server.MapPath("~/Assets/SystemTermsFiles"));
            string[] files = System.IO.Directory.GetFiles(Server.MapPath("~/Assets/SystemTermsFiles"));

            if (dirs.Length == 0 && files.Length == 0)
                result = "Not Exists";
            else
                result = "Exists^" + files[0].Split('^')[1];
            return result;
        }

        public void DeleteTermsFiles()
        {
            string[] files = System.IO.Directory.GetFiles(Server.MapPath("~/Assets/SystemTermsFiles"));
            if (files.Length > 0)
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Assets/SystemTermsFiles"));

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
        }

        public string getMailContent(string mode)
        {
            string result = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@Mode", mode);
            ht.Add("@Operation", "GET");

            dr = DBFunctions.SysFetchData("POMS_GetSetMailTemplate", ref ht);

            if (dr.Read())
            {
                result = Convert.ToString(dr["MailBody"]) + "!##!" + Convert.ToString(dr["Subject"]);
            }
            dr.Close();
            return result;
        }

        public string setMailContent(string mode, string BODY, string Subject)
        {
            string result = "";

            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@Mode", mode);
            ht.Add("@Operation", "SET");
            ht.Add("@Body", BODY);
            ht.Add("@Subject", Subject);

            dr = DBFunctions.SysFetchData("POMS_GetSetMailTemplate", ref ht);
            if (dr.Read())
            {
                result = Convert.ToString(dr["Result"]);
            }
            dr.Close();

            return result;
        }


        //public string setMailContent(string mode, string BODY, string Subject)
        //{
        //    string result = "";

        //    DBFunctions DBFunctions = new DBFunctions();
        //    SqlDataReader dr;
        //    Hashtable ht = new Hashtable();

        //    ht.Add("@Mode", mode);
        //    ht.Add("@Operation", "SET");
        //    ht.Add("@Body", BODY);
        //    ht.Add("@Subject", Subject);

        //    dr = DBFunctions.SysFetchData("POMS_GetSetMailTemplate", ref ht);
        //    if (dr.Read())
        //    {
        //        result = Convert.ToString(dr["Result"]);
        //    }
        //    dr.Close();

        //    return result;
        //}

        [UserAuthenticationFilter]
        public string sendfeedbackmail(string body, string page, string sub)
        {
            string mailbody = body;
            string Email_From = "";
            string Email_From_Name = "";
            string Name = "";
            string Address = "";
            string sHeader = "";
            string Email_Sendto = "";

            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@UserID", Convert.ToInt32(Session["UserID"]));
            dr = db.SysFetchData("[dbo].[POMS_FeedbackMail]", ref ht);

            if (dr.Read())
            {
                Email_From = Convert.ToString(dr["Email"]);
                Name = Convert.ToString(dr["Name"]);
                Address = Convert.ToString(dr["Address"]);
                Email_From_Name = Convert.ToString(dr["Email_From_Name"]);
                Email_Sendto = Convert.ToString(dr["tomail"]);
                sHeader = sub + " From " + Name;
            }
            dr.Close();
            mailbody = mailbody.Replace("../", "www.thebridgebytcg.com/");
            mailbody = Convert.ToString(HttpUtility.UrlDecode(mailbody));
            mailbody = "<p><span style='color: rgb(0, 0, 255);' data-mce-style='color: #0000ff;'>Hi " + Email_From_Name + ",</span></p>"
            + mailbody
            + "<h3><strong>Thanks and regards,</strong></h3>"
            + "<p><span style='color: rgb(0, 0, 255);' data-mce-style='color: #0000ff;'>" + Name + "</span></p>"
            + "<p><span style='color: rgb(51, 102, 255);' data-mce-style='color: #3366ff;'>" + Address + "</span></p>"
            + "<p><span style='color: rgb(51, 102, 255);' data-mce-style='color: #3366ff;'>" + Email_From + "</span></p>"
            + "<p><br data-mce-bogus='1'></p>"
            + "<h4><em>Sent From :<span style='text-decoration: underline;' data-mce-style='text-decoration: underline;'>" + page + "</span>&nbsp;</em></h4>";

            if (Email_Sendto.Trim() != "")
            {
                string Email_SendFrom = WebConfigurationManager.AppSettings["Mailfrom"].ToString();
                Email email = new Email();
                email.SendeMail("soumik.mahajan@esolzmail.com", "soumik.mahajan@esolzmail.com", "", "", sHeader, mailbody, "");
            }
            string newmailbody = "<p><span style='color: rgb(0, 0, 255);' data-mce-style='color: #0000ff;'>Hi " + Email_From_Name + ",</span></p>"
                + "<p>Thank you for your feedback.  The following message has been sent to the system administrator:</p>"
                + body
                + "<p><strong>Sincerely,</strong></p>"
                + "<p>The Bridge by TCG</p>";
            Email email2 = new Email();
            email2.SendeMail("", "soumik.mahajan@esolzmail.com", "", "", "Thank you for your feedback", newmailbody, "");
            return "SUCCESS";
        }

        [HttpPost]
        public string TermsManagement(Terms termsObj)
        {
            string str = "";
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@ID", termsObj.ID);
            ht.Add("@Terms", termsObj.Term);
            ht.Add("@Status", termsObj.Status);
            ht.Add("@Mode", termsObj.Mode);
            dr = DBFunctions.SysFetchData("POMS_TERMS_Management", ref ht);
            if (dr.Read())
            {
                str = Convert.ToString(dr["Result"]);
            }
            dr.Close();
            return str;
        }

        [HttpGet]
        public string GetTerms(string mode)
        {
            string str = "";
            List<Terms> TermsObj = new List<Terms>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            DBFunctions DBFunctions = new DBFunctions();
            SqlDataReader dr;
            Hashtable ht = new Hashtable();

            ht.Add("@Mode", mode);
            dr = DBFunctions.SysFetchData("POMS_TERMS_Management", ref ht);
            while (dr.Read())
            {
                string status = dr["Status"].ToString();
                TermsObj.Add(new Terms()
                {
                    ID = Convert.ToString(dr["TermID"]),
                    Term = Convert.ToString(dr["Terms"]),
                    Status = dr["Status"].ToString() == "1" ? true : false
                });
            }
            dr.Close();
            str = js.Serialize(TermsObj);
            return str;
        }

        public string GetAllCurrency1()
        {
            string result = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            ht.Clear();
            ht.Add("@mode", "GetAllCurrency");
            ht.Add("@CountryCode", 0);
            ht.Add("@Email", "");
            dr = db.SysFetchData("[dbo].[POMS_CountryStateDeatil]", ref ht);
            result = "<option value=0 name=ddlCurr_0>--Select Currency--</option>";
            while (dr.Read())
            {
                result += "<option data-currencycode='" + Convert.ToString(dr["CurrencyCode"]) + "' value=" + Convert.ToString(dr["CurrencyID"]) + " name=ddlCurr_" + Convert.ToString(dr["CurrencyID"]) + ">" + Convert.ToString(dr["Currency"]) + "</option>";
            }
            dr.Close();
            return result;
        }

        [HttpPost]
        public string fileupload()
        {
            string result = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                if (file.ContentType.ToLower() != "image/jpg" &&
                   file.ContentType.ToLower() != "image/jpeg" &&
                   file.ContentType.ToLower() != "image/pjpeg" &&
                   file.ContentType.ToLower() != "image/gif" &&
                   file.ContentType.ToLower() != "image/x-png" &&
                   file.ContentType.ToLower() != "image/png")
                {
                    return "INVALID";
                }

                //-------------------------------------------
                //  Check the image extension
                //-------------------------------------------
                if (Path.GetExtension(file.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(file.FileName).ToLower() != ".png"
                    && Path.GetExtension(file.FileName).ToLower() != ".gif"
                    && Path.GetExtension(file.FileName).ToLower() != ".jpeg")
                {
                    return "INVALID";
                }

                System.IO.Stream fs = file.InputStream;
                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                result = Convert.ToBase64String(bytes, 0, bytes.Length);

                string allfilename = "";
                string ext = Path.GetFileName(file.FileName).Split('.')[1];
                string fname = Path.GetFileName(file.FileName).Split('.')[0];
                string fileName = fname + "(" + DateTime.Now.Ticks.ToString() + "_" + "MCE" + ")" + "." + ext;
                if (allfilename.Trim() == "")
                {
                    allfilename = fileName;
                }
                else
                {
                    allfilename += "," + fileName;
                }
                var path = Path.Combine(Server.MapPath("~/Assets/TinyMceImges"), fileName);
                file.SaveAs(path);
                result = fileName.ToString();
            }
            return result;
        }

    }
}
