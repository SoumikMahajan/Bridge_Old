using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
//using Antlr.Runtime;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Exception;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace POMS.Controllers
{
    
    public class QuickBookController : Controller
    {
        public class Payment
        {
            public string PaymentID { get; set; }
            public string QBOID { get; set; }
            public string CheckNumber { get; set; }
            public string Status { get; set; }

        }
        public class QueryRespPurchase
        {
            public QueryResp<List<Purchase>> QueryResponse { get; set; }
            public DateTime time { get; set; }
        }
        public class QueryRespVendor
        {
            public QueryResp<List<Vendor>> QueryResponse { get; set; }
            public DateTime time { get; set; }
        }
        public class QueryRespACInfo
        {
            public QueryResp<List<Account>> QueryResponse { get; set; }
            public DateTime time { get; set; }
        }

        public class QueryResp<T>
        {
            public T CompanyInfo { get; set; }
            public string maxResult { get; set; }
        }
        public class QryResCompanyInfo
        {
            public QueryResp<List<CompanyInfo>> QueryResponse { get; set; }
            public DateTime time { get; set; }
        }
        /* Getting all set up data that are required to use APP */
        public static String REQUEST_TOKEN_URL = WebConfigurationManager.AppSettings["GET_REQUEST_TOKEN"];
        public static String ACCESS_TOKEN_URL = WebConfigurationManager.AppSettings["GET_ACCESS_TOKEN"];
        public static String AUTHORIZE_URL = WebConfigurationManager.AppSettings["AuthorizeUrl"];
        public static String OAUTH_URL = WebConfigurationManager.AppSettings["OauthLink"];
        public static String consumerKey = WebConfigurationManager.AppSettings["ConsumerKey"];
        public static String consumerSecret = WebConfigurationManager.AppSettings["ConsumerSecret"];
        public string strrequestToken = string.Empty;
        public string tokenSecret = string.Empty;
        public static string oauth_callback_url = WebConfigurationManager.AppSettings["oauth_callback_url"];
        public static string appEnvironment = WebConfigurationManager.AppSettings["env"];
        public string GrantUrl = WebConfigurationManager.AppSettings["GrantUrl"];
        public string BaseUrlQBO = WebConfigurationManager.AppSettings["BaseUrlQBO"]; //this will determine between production/sandboxQBO

        static OAuth2Client oauthClient = new OAuth2Client(consumerKey, consumerSecret, oauth_callback_url, appEnvironment);

        //Oauth 2.0 30-11-2019
        public static string mod;
        public static string expo;
        public static string stateCSRFToken = "";

        public static string authorizeUrl = "";
        public static string tokenEndpoint = "";
        public static string revocationEndpoint = "";
        public static string userinfoEndpoint = "";
        public static string issuerEndpoint = "";
        public static string code = "";

        public static string access_token = "";
        public static string refresh_token = "";
        public static string identity_token = "";
        public static IList<JsonWebKey> keys;

        string sub = "";
        string email = "";
        string emailVerified = "";
        string givenName = "";
        string familyName = "";
        string phoneNumber = "";
        string phoneNumberVerified = "";
        string streetAddress = "";
        string locality = "";
        string region = "";
        string postalCode = "";
        string country = "";

        IdTokenJWTClaimTypes payloadData;

        //QBO
        DiscoveryClient discoveryClient;
        DiscoveryResponse doc;
        AuthorizeRequest request;
        public static string scope;

        public string env = WebConfigurationManager.AppSettings["env"];

        public async Task<string> SaveToken(string accessToken, string accessTokenSecret, string key, string RefreshToken, string TokenType, DateTime AccessTokenDatetime, DateTime RefreshTokenDatetime, string TokenEndPoint)
        {
            string res = "Failed";
            try
            {
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@Key", key);
                ht.Add("@accessToken", accessToken);
                ht.Add("@accessTokenSecret", accessTokenSecret);
                ht.Add("@RefreshToken", RefreshToken);
                ht.Add("@TokenType", TokenType);
                ht.Add("@AccessTokenExpire", AccessTokenDatetime);
                ht.Add("@RefreshTokenExpire", RefreshTokenDatetime);
                ht.Add("@TokenEndpoint", TokenEndPoint);
                ht.Add("@Mode", "INSERT");
                dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
                while (dr.Read())
                {
                    res = "success";
                }
                dr.Close();
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        public async Task<string> RefreshToken(string AccessTokenSecrete, string key, string refreshToken)
        {
            try
            {
                //Refresh Token call
                var tokenClient = new TokenClient(tokenEndpoint, consumerKey, consumerSecret);
                //var principal = User as ClaimsPrincipal;

                TokenResponse response = await tokenClient.RequestRefreshTokenAsync(refreshToken);
                //UpdateCookie(response);
                if (response.AccessToken != null)
                {
                    await SaveToken(response.AccessToken, AccessTokenSecrete, key, response.RefreshToken, response.TokenType, DateTime.Now.AddSeconds(response.AccessTokenExpiresIn), DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn), tokenEndpoint);
                }
                else
                {

                    response = await oauthClient.RefreshTokenAsync(refreshToken);
                    if (response.AccessToken != null && response.RefreshToken != null)
                    {
                        await SaveToken(response.AccessToken, AccessTokenSecrete, key, response.RefreshToken, response.TokenType, DateTime.Now.AddSeconds(response.AccessTokenExpiresIn), DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn), tokenEndpoint);
                    }
                }
                return response.AccessToken;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<ActionResult> AuthenticationAsync()
        {
            List<string> queryKeys = new List<string>(Request.QueryString.AllKeys);
            if (queryKeys.Contains("connect"))
            {//Intialize DiscoverPolicy Oauth 2.0
                DiscoveryPolicy dpolicy = new DiscoveryPolicy();
                dpolicy.RequireHttps = true;
                dpolicy.ValidateIssuerName = true;
                string discoveryUrl = ConfigurationManager.AppSettings["DiscoveryUrl"];

                if (discoveryUrl != null && consumerKey != null && consumerSecret != null)
                {
                    discoveryClient = new DiscoveryClient(discoveryUrl);
                }
                else
                {
                    Exception ex = new Exception("Discovery Url missing!");
                    throw ex;
                }
                doc = await discoveryClient.GetAsync();
                if (doc.StatusCode == HttpStatusCode.OK)
                {
                    //Authorize endpoint
                    authorizeUrl = doc.AuthorizeEndpoint;

                    //Token endpoint
                    tokenEndpoint = doc.TokenEndpoint;

                    //Token Revocation enpoint
                    revocationEndpoint = doc.RevocationEndpoint;

                    //UserInfo endpoint
                    userinfoEndpoint = doc.UserInfoEndpoint;

                    //Issuer endpoint
                    issuerEndpoint = doc.Issuer;

                    //JWKS Keys
                    keys = doc.KeySet.Keys;
                }
                //Get mod and exponent value
                foreach (var key in keys)
                {
                    if (key.N != null)
                    {
                        //Mod
                        mod = key.N;
                    }
                    if (key.E != null)
                    {
                        //Exponent
                        expo = key.E;
                    }
                }
                scope = OidcScopes.OpenId.GetStringValue() + " " + OidcScopes.Address.GetStringValue()
                     + " " + OidcScopes.Email.GetStringValue() + " " + OidcScopes.Phone.GetStringValue()
                     + " " + OidcScopes.Profile.GetStringValue() + " " + OidcScopes.Accounting.GetStringValue();
                //+" "+ OidcScopes.Payment.GetStringValue();
                authorizeUrl = GetAuthorizeUrl(scope);
                // perform the redirect here.
                return Redirect(authorizeUrl);

                FireAuthAsync();
            }
            //if (queryKeys.Contains("oauth_token"))
            if (queryKeys.Contains("code"))
            {
                //Request Oauth2 tokens
                var tokenClient = new TokenClient(tokenEndpoint, consumerKey, consumerSecret);


                var code = Request.QueryString["code"];
                var realmId = Request.QueryString["realmId"];
                if (realmId != null)
                {
                    Session["realmId"] = realmId;
                }
                //var tempState = await GetTempStateAsync();
                //Request.GetOwinContext().Authentication.SignOut("TempState");

                TokenResponse response = await tokenClient.RequestTokenFromCodeAsync(code, oauth_callback_url);
                LogWriter ll = new LogWriter("RefreshToken :- " + response.RefreshToken + " realmId : -"+ Session["realmId"]);
                await ValidateResponseAndSignInAsync(response);

                if (!string.IsNullOrEmpty(response.IdentityToken))
                {
                    Session["accessTokenSecret"] = response.IdentityToken;
                }
                if (!string.IsNullOrEmpty(response.AccessToken))
                {
                    Session["accessToken"] = response.AccessToken;
                }
                //Oauth 1.0 
                //ReadTokenAsync();

                string key = Session["realmId"].ToString();
                string accessToken = Session["accessToken"].ToString();
                string accessTokenSecret = Session["accessTokenSecret"].ToString();

                await SaveToken(accessToken, accessTokenSecret, key, response.RefreshToken, response.TokenType, DateTime.Now.AddSeconds(response.AccessTokenExpiresIn), DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn), tokenEndpoint);


            }
            return View();
        }
        //Oauth 2.0 
        private async Task<IEnumerable<Claim>> GetUserInfoClaimsAsync(string accessToken)
        {
            IEnumerable<Claim> userData = new List<Claim>();
            //Get UserInfo data when correct scope is set for SIWI and Get App now flows
            var userInfoClient = new UserInfoClient(userinfoEndpoint);

            UserInfoResponse userInfoResponse = await userInfoClient.GetAsync(accessToken);
            if (userInfoResponse.HttpStatusCode == HttpStatusCode.OK)
            {
                //Read UserInfo Details
                userData = userInfoResponse.Json.ToClaims();

                foreach (Claim item in userData)
                {
                    if (item.Type == "sub" && item.Value != null)
                        sub = item.Value;
                    if (item.Type == "email" && item.Value != null)
                        email = item.Value;
                    if (item.Type == "emailVerified" && item.Value != null)
                        emailVerified = item.Value;
                    if (item.Type == "givenName" && item.Value != null)
                        givenName = item.Value;
                    if (item.Type == "familyName" && item.Value != null)
                        familyName = item.Value;
                    if (item.Type == "phoneNumber" && item.Value != null)
                        phoneNumber = item.Value;
                    if (item.Type == "phoneNumberVerified" && item.Value != null)
                        phoneNumberVerified = item.Value;

                    if (item.Type == "address" && item.Value != null)
                    {

                        Address jsonObject = JsonConvert.DeserializeObject<Address>(item.Value);

                        if (jsonObject.StreetAddress != null)
                            streetAddress = jsonObject.StreetAddress;
                        if (jsonObject.Locality != null)
                            locality = jsonObject.Locality;
                        if (jsonObject.Region != null)
                            region = jsonObject.Region;
                        if (jsonObject.PostalCode != null)
                            postalCode = jsonObject.PostalCode;
                        if (jsonObject.Country != null)
                            country = jsonObject.Country;
                    }

                }

            }

            return userData;
        }

        private bool ValidateToken(string identityToken)
        {
            if (keys != null)
            {

                //IdentityToken
                if (identityToken != null)
                {
                    //Split the identityToken to get Header and Payload
                    string[] splitValues = identityToken.Split('.');
                    if (splitValues[0] != null)
                    {

                        //Decode header 
                        var headerJson = Encoding.UTF8.GetString(Base64Url.Decode(splitValues[0].ToString()));

                        //Deserilaize headerData
                        IdTokenHeader headerData = JsonConvert.DeserializeObject<IdTokenHeader>(headerJson);

                        //Verify if the key id of the key used to sign the payload is not null
                        if (headerData.Kid == null)
                        {
                            return false;
                        }

                        //Verify if the hashing alg used to sign the payload is not null
                        if (headerData.Alg == null)
                        {
                            return false;
                        }

                    }
                    if (splitValues[1] != null)
                    {
                        //Decode payload
                        var payloadJson = Encoding.UTF8.GetString(Base64Url.Decode(splitValues[1].ToString()));


                        payloadData = JsonConvert.DeserializeObject<IdTokenJWTClaimTypes>(payloadJson);



                        //Verify Aud matches ClientId
                        if (payloadData.Aud != null)
                        {
                            if (payloadData.Aud[0].ToString() != consumerKey)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }


                        //Verify Authtime matches the time the ID token was authorized.                
                        if (payloadData.Auth_time == null)
                        {
                            return false;
                        }



                        //Verify exp matches the time the ID token expires, represented in Unix time (integer seconds).                
                        if (payloadData.Exp != null)
                        {
                            long expiration = Convert.ToInt64(payloadData.Exp);
                            long currentEpochTime = EpochTimeExtensions.ToEpochTime(DateTime.UtcNow);
                            //Verify the ID expiration time with what expiry time you have calculated and saved in your application
                            //If they are equal then it means IdToken has expired 

                            if ((expiration - currentEpochTime) <= 0)
                            {
                                return false;

                            }

                        }

                        //Verify Iat matches the time the ID token was issued, represented in Unix time (integer seconds).            
                        if (payloadData.Iat == null)
                        {
                            return false;
                        }


                        //verify Iss matches the  issuer identifier for the issuer of the response.     
                        if (payloadData.Iss != null)
                        {
                            if (payloadData.Iss.ToString() != issuerEndpoint)
                            {

                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }



                        //Verify sub. Sub is an identifier for the user, unique among all Intuit accounts and never reused. 
                        //An Intuit account can have multiple emails at different points in time, but the sub value is never changed.
                        //Use sub within your application as the unique-identifier key for the user.
                        if (payloadData.Sub == null)
                        {

                            return false;
                        }



                    }

                    //Use external lib to decode mod and expo value and generte hashes
                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                    //Read values of n and e from discovery document.
                    rsa.ImportParameters(
                      new RSAParameters()
                      {
                          //Read values from discovery document
                          Modulus = Base64Url.Decode(mod),
                          Exponent = Base64Url.Decode(expo)
                      });

                    //Verify Siganture hash matches the signed concatenation of the encoded header and the encoded payload with the specified algorithm
                    SHA256 sha256 = SHA256.Create();

                    byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(splitValues[0] + '.' + splitValues[1]));

                    RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                    rsaDeformatter.SetHashAlgorithm("SHA256");
                    if (rsaDeformatter.VerifySignature(hash, Base64Url.Decode(splitValues[2])))
                    {
                        //identityToken is valid
                        return true;
                    }
                    else
                    {
                        //identityToken is not valid
                        return false;

                    }
                }
                else
                {
                    //identityToken is not valid
                    return false;
                }
            }
            else
            {
                //Missing mod and expo values
                return false;
            }
        }

        private async System.Threading.Tasks.Task ValidateResponseAndSignInAsync(TokenResponse response)
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrWhiteSpace(response.IdentityToken))
            {

                bool isIdTokenValid = ValidateToken(response.IdentityToken);//throw error is not valid

                if (isIdTokenValid == true)
                {
                    claims.Add(new Claim("id_token", response.IdentityToken));

                }
            }
            if (Session["realmId"] != null)
            {
                claims.Add(new Claim("realmId", Session["realmId"].ToString()));
            }


            if (!string.IsNullOrWhiteSpace(response.AccessToken))
            {
                var userClaims = await GetUserInfoClaimsAsync(response.AccessToken);
                claims.AddRange(userClaims);

                claims.Add(new Claim("access_token", response.AccessToken));
                claims.Add(new Claim("access_token_expires_at", (DateTime.Now.AddSeconds(response.AccessTokenExpiresIn)).ToString()));
            }

            if (!string.IsNullOrWhiteSpace(response.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", response.RefreshToken));
                claims.Add(new Claim("refresh_token_expires_at", (DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn)).ToString()));

            }

            var id = new ClaimsIdentity(claims, "Cookies");
            Request.GetOwinContext().Authentication.SignIn(id);

        }
        private async Task<Tuple<string>> GetTempStateAsync()
        {
            var data = await Request.GetOwinContext().Authentication.AuthenticateAsync("TempState");

            var state = data.Identity.FindFirst("state").Value;


            return Tuple.Create(state);
        }

        public async Task<string> checkValidityAsync()
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string WriteDate = "";
            string Refresh_Token = "";

            try
            {
                string key = "";
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();

                ht.Add("@Mode", "CHECK");
                ht.Add("@Key", key);
                dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
                while (dr.Read())
                {
                    accessToken = Convert.ToString(dr["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                    WriteDate = Convert.ToString(dr["WriteDate"]);
                    key = Convert.ToString(dr["Key"]);
                    Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                    tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
                }
                dr.Close();

                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                //Oauth 2.0
                var principal = User as ClaimsPrincipal;


                string query = "select * from CompanyInfo";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);
                if (key != "")
                {
                    string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                    //add qbobase url and query
                    string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                    string res = "";
                    try
                    {
                        var client = new HttpClient();

                        client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                        client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                        res = await client.GetStringAsync(uri);
                        QryResCompanyInfo str = new QryResCompanyInfo();
                        str = JsonConvert.DeserializeObject<QryResCompanyInfo>(res);

                        return str.QueryResponse.CompanyInfo.FirstOrDefault().CompanyName + "***" + WriteDate;
                    }
                    catch (Exception ex)
                    {
                        return "INVALID";
                    }
                    if (res == "")
                    {
                        return "INVALID";
                    }
                    else
                    {
                        return res; //= //companyInfo.CompanyName + "***" + WriteDate;
                    }
                }
                /* QBO object setup for calling APP */
                //OAuthRequestValidator oauthValidator = new OAuthRequestValidator(
                //        accessToken, accessTokenSecret, consumerKey, consumerSecret);
                //    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                //            accessToken);
                //    string appToken = WebConfigurationManager.AppSettings["appToken"];
                //    string companyID = key;
                //    ServiceContext context2 = new ServiceContext(companyID, IntuitServicesType.QBO, oauthValidator);
                //    context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                //    context2.IppConfiguration.MinorVersion.Qbo = "23";

                //    ServiceContext serviceContext = new ServiceContext(key, IntuitServicesType.QBO, oauthValidator);



                //    context2.IppConfiguration.Message.Request.SerializationFormat = SerializationFormat.Xml;
                //    context2.IppConfiguration.Message.Response.SerializationFormat = SerializationFormat.Json;


                //    QueryService<CompanyInfo> service = new QueryService<CompanyInfo>(context2);

                //    /*company data will be returned if the data are valid*/
                //    var companyInfo = service.ExecuteIdsQuery("Select * from CompanyInfo").FirstOrDefault();
                //    if (companyInfo == null)
                //    {
                //        using (EventLog eventLog = new EventLog("Application"))
                //        {
                //            eventLog.Source = "Application";

                //            eventLog.WriteEntry(" Auth not valid ", EventLogEntryType.Information, 102, 1);

                //        }
                //        result = "INVALID";
                //    }
                //    else
                //    {
                //        result = companyInfo.CompanyName + "***" + WriteDate;
                //    }
            }
            catch (Exception e)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(" Severe error ", EventLogEntryType.Information, 102, 1);

                }
                result = "INVALID";
            }
            return result;
        }
        public string RemoveSpace(string type)
        {
            type = type.Replace(" ", "");
            return type;
        }

        /*Get all the accounts of vendor from quick book system*/
        public async Task<string> GetUpdateAccountsQBS()
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
            }
            dr.Close();
            accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                    accessToken);
            string companyID = key;
            ServiceContext context2 = new ServiceContext(companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service = new QueryService<Account>(context2);
            try
            {
                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                //Oauth 2.0
                //var principal = User as ClaimsPrincipal;
                //QBOAccntID = "35";

                string query = "Select * from Account";
                // build the  request
                //string encodedQuery = WebUtility.UrlEncode(query);
                if (key != "")
                {

                    string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                    ////add qbobase url and query
                    //string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                    //string res = "";

                    //var client = new HttpClient();

                    //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                    //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    //res = await client.GetStringAsync(uri);
                    ////Account accounts = JsonConvert.DeserializeObject<QryRes<QueryRespACInfo>>(res).QueryRes.accounts.FirstOrDefault();

                    ////QueryRespACInfo str = new QueryRespACInfo();
                    //dynamic dyn;
                    ////str = JsonConvert.DeserializeObject<QueryRespACInfo>(res);
                    //dyn = JsonConvert.DeserializeObject(res);
                    //var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                    //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                    //string jstr = JsonConvert.SerializeObject(Acc);
                    //var json = JArray.Parse(jstr);
                    //for (int i = 0; i < json.Count; i++)
                    //{
                    //    string ret = RemoveSpace(json[i]["AccountType"].ToString());
                    //    json[i]["AccountType"] = ret;
                    //}
                    //jstr = JsonConvert.SerializeObject(json);
                    ////jstr = jstr.Replace("AccountType", "Account1SubType");
                    ////jstr = jstr.Replace("AccountSubType", "AccountType");
                    ////jstr = jstr.Replace("Account1SubType", "AccountSubType");
                    var jstr = service.ExecuteIdsQuery(query).ToList<Account>();
                    List<Account> accounts3 = jstr;
                    if (accounts3 == null)
                    {
                        result = "INVALID";
                    }
                    else
                    {
                        int accounts2 = accounts3.Count;
                        if (accounts2 < 1000)
                        {
                            query = "Select * from Account WHERE Active IN ( True, False ) MaxResults 1000";
                            // build the  request
                            //encodedQuery = WebUtility.UrlEncode(query);

                            //uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                            //res = "";

                            //client = new HttpClient();

                            //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                            //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                            //res = await client.GetStringAsync(uri);
                            //dyn = JsonConvert.DeserializeObject(res);
                            //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                            //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                            //jstr = JsonConvert.SerializeObject(Acc);
                            //json = JArray.Parse(jstr);
                            //for (int i = 0; i < json.Count; i++)
                            //{
                            //    string ret = RemoveSpace(json[i]["AccountType"].ToString());
                            //    json[i]["AccountType"] = ret;
                            //}
                            //jstr = JsonConvert.SerializeObject(json);
                            List<Account> accounts = service.ExecuteIdsQuery(query).ToList<Account>();

                            string strDelete = "1";
                            if (accounts != null && accounts.Count() > 0)
                            {
                                LogWriter logWriter = new LogWriter("accounts : " + accounts.Count().ToString());
                                foreach (var objAccount in accounts)
                                {
                                    Hashtable htValue = new Hashtable();
                                    SqlDataReader objdr;
                                    DBFunctions objDBFunctions = new DBFunctions();
                                    ReferenceType objCurrencyreference = objAccount.CurrencyRef;

                                    htValue.Clear();
                                    htValue.Add("@Mode", "ADDQBSAcoount");
                                    htValue.Add("@QBid", objAccount.Id);
                                    htValue.Add("@AccountName", objAccount.Name);
                                    htValue.Add("@AccountNo", objAccount.AcctNum == null ? "" : objAccount.AcctNum);
                                    htValue.Add("@CurrencyValue", objCurrencyreference.Value);
                                    htValue.Add("@Balance", objAccount.CurrentBalance);
                                    htValue.Add("@Delete", strDelete);
                                    DataSet ds=objDBFunctions.SysFetchDataInDataSet("[dbo].[POMS_AddAccountQBS]", htValue);
                                    LogWriter logWriter1 = new LogWriter(ds.Tables[0].Rows[0]["res"].ToString() + " - ID " + objAccount.Id + " - Name " + objAccount.Name + " - Delete " + strDelete);
                                    strDelete = "0";
                                    objCurrencyreference = null;
                                    htValue = null;
                                    objdr = null;
                                    objDBFunctions = null;
                                }
                                return "success";
                            }


                        }
                        else
                        {
                            string strDelete = "1";

                            for (int a = 1; a <= accounts2; a = a + 1000)
                            {
                                query = "Select * from Account STARTPOSITION " + a + " MAXRESULTS 1000";
                                // build the  request
                                //encodedQuery = WebUtility.UrlEncode(query);

                                //uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                                //res = "";

                                //client = new HttpClient();

                                //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                                //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                                //res = await client.GetStringAsync(uri);
                                //dyn = JsonConvert.DeserializeObject(res);
                                //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                                //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                                //jstr = JsonConvert.SerializeObject(Acc);
                                //json = JArray.Parse(jstr);
                                //for (int i = 0; i < json.Count; i++)
                                //{
                                //    string ret = RemoveSpace(json[i]["AccountType"].ToString());
                                //    json[i]["AccountType"] = ret;
                                //}
                                //jstr = JsonConvert.SerializeObject(json);
                                List<Account> accounts = service.ExecuteIdsQuery(query).ToList<Account>();
                                //

                                if (accounts != null && accounts.Count() > 0)
                                {
                                    LogWriter logWriter = new LogWriter("accounts : " + accounts.Count().ToString());
                                    foreach (var objAccount in accounts)
                                    {
                                        Hashtable htValue = new Hashtable();
                                        SqlDataReader objdr;
                                        DBFunctions objDBFunctions = new DBFunctions();
                                        ReferenceType objCurrencyreference = objAccount.CurrencyRef;

                                        htValue.Clear();
                                        htValue.Add("@Mode", "ADDQBSAcoount");
                                        htValue.Add("@QBid", objAccount.Id);
                                        htValue.Add("@AccountName", objAccount.Name);
                                        htValue.Add("@AccountNo", objAccount.AcctNum == null ? "" : objAccount.AcctNum);
                                        htValue.Add("@CurrencyValue", objCurrencyreference.Value);
                                        htValue.Add("@Balance", objAccount.CurrentBalance);
                                        htValue.Add("@Delete", strDelete);
                                        DataSet ds= objDBFunctions.SysFetchDataInDataSet("[dbo].[POMS_AddAccountQBS]", htValue);

                                        LogWriter logWriter11 = new LogWriter(ds.Tables[0].Rows[0]["res"].ToString() + " - ID " + objAccount.Id + " - Name " + objAccount.Name + " - Delete " + strDelete);   

                                        strDelete = "0";
                                        objCurrencyreference = null;
                                        htValue = null;
                                        objdr = null;
                                        objDBFunctions = null;
                                    }
                                    return "success";

                                }
                            }
                        }

                    }
                    



                }



            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;
        }


        [HttpPost]
        public async Task<string> Allpaymentnumbersync()
        {
            string result = "";
            try
            {
                List<string> qboid = new List<string>();
                List<string> synctoken = new List<string>();
                string jsnstr = "{\"data\":[";

                string accessToken = "";
                string accessTokenSecret = "";
                string Refresh_Token = "";
                string key = WebConfigurationManager.AppSettings["companyID"];
                SqlDataReader dr2;
                DBFunctions db2 = new DBFunctions();
                Hashtable ht2 = new Hashtable();
                ht2.Clear();
                ht2.Add("@Mode", "GET");
                ht2.Add("@Key", key);
                dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
                while (dr2.Read())
                {
                    accessToken = Convert.ToString(dr2["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                    key = Convert.ToString(dr2["Key"]);
                    Refresh_Token = Convert.ToString(dr2["RefreshToken"]);
                    tokenEndpoint = Convert.ToString(dr2["TokenEndpoint"]);
                }
                dr2.Close();
                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(accessToken);

                string appToken = WebConfigurationManager.AppSettings["appToken"];
                string companyID = key;
                ServiceContext context2 = new ServiceContext(companyID, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                QueryService<Purchase> qp = new QueryService<Purchase>(context2);
                QueryService<Deposit> qp1 = new QueryService<Deposit>(context2);
                string json = "";
                int indx = 0;

                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                //var principal = User as ClaimsPrincipal;
                //QBOAccntID = "35";

                string query = "Select count(*) from Purchase";
                // build the  request
                //string encodedQuery = WebUtility.UrlEncode(query);
                if (key != "")
                {
                    string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                    //add qbobase url and query

                    var client = new HttpClient();


                    var pcounto = qp.ExecuteIdsQuery(query).ToList<Purchase>();

                    //string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                    string res = "";


                    //var pcounto = JsonConvert.DeserializeObject<List<Purchase>>(jstr).ToList();
                    if (pcounto == null)
                    {
                        result = "INVALID";
                    }
                    else
                    {
                        List<Purchase> Allp = new List<Purchase>();

                        int pcount = pcounto.Count;
                        if (pcount < 1000)
                        {

                            query = "Select * from Purchase MaxResults 1000";
                            var prs = qp.ExecuteIdsQuery(query).ToList<Purchase>();
                            // build the  request
                            //encodedQuery = WebUtility.UrlEncode(query);
                            //add qbobase url and query
                            //uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                            //res = "";

                            //    client = new HttpClient();

                            //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                            //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                            //res = await client.GetStringAsync(uri);
                            //dyn = JsonConvert.DeserializeObject(res);
                            //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                            //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                            //jstr = JsonConvert.SerializeObject(Acc);
                            //var prs = JsonConvert.DeserializeObject<List<Purchase>>(jstr).ToList();



                            string strDelete = "1";
                            if (prs != null && prs.Count() > 0)
                            {
                                Allp.AddRange(prs);
                            }


                        }
                        else
                        {
                            string strDelete = "1";

                            for (int a = 1; a <= pcount; a = a + 1000)
                            {

                                query = "Select * from Purchase STARTPOSITION " + a + " MAXRESULTS 1000";
                                var prs = qp.ExecuteIdsQuery(query).ToList<Purchase>();
                                // build the  request
                                //encodedQuery = WebUtility.UrlEncode(query);
                                //add qbobase url and query
                                //uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                                //res = "";

                                //client = new HttpClient();

                                //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                                //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                                //res = await client.GetStringAsync(uri);
                                //dyn = JsonConvert.DeserializeObject(res);
                                //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                                //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                                //jstr = JsonConvert.SerializeObject(Acc);
                                //var prs = JsonConvert.DeserializeObject<List<Purchase>>(jstr).ToList();

                                if (prs != null && prs.Count() > 0)
                                {
                                    Allp.AddRange(prs);

                                }
                            }
                        }


                        query = "Select count(*) from Deposit";
                        var jcounto = qp1.ExecuteIdsQuery(query).ToList<Deposit>();
                        // build the  request
                        //encodedQuery = WebUtility.UrlEncode(query);
                        ////add qbobase url and query
                        //uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                        //res = "";

                        //client = new HttpClient();

                        //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                        //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                        //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                        //res = await client.GetStringAsync(uri);
                        //dyn = JsonConvert.DeserializeObject(res);
                        //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                        //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                        //jstr = JsonConvert.SerializeObject(Acc);
                        //var jcounto = JsonConvert.DeserializeObject<List<Deposit>>(jstr).ToList();




                        List<Deposit> Allj = new List<Deposit>();

                        int jcount = jcounto.Count;
                        if (jcount < 1000)
                        {

                            query = "Select * from Deposit MaxResults 1000";
                            var jrs = qp1.ExecuteIdsQuery(query).ToList<Deposit>();
                            // build the  request
                            //encodedQuery = WebUtility.UrlEncode(query);
                            ////add qbobase url and query
                            //uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                            //res = "";

                            //client = new HttpClient();

                            //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                            //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                            //res = await client.GetStringAsync(uri);
                            //dyn = JsonConvert.DeserializeObject(res);
                            //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                            //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                            //jstr = JsonConvert.SerializeObject(Acc);
                            //var jrs = JsonConvert.DeserializeObject<List<Deposit>>(jstr).ToList();



                            string strDelete = "1";
                            if (jrs != null && jrs.Count() > 0)
                            {
                                Allj.AddRange(jrs);
                            }


                        }
                        else
                        {
                            string strDelete = "1";

                            for (int a = 1; a <= jcount; a = a + 1000)
                            {

                                query = "Select * from Deposit STARTPOSITION " + a + " MAXRESULTS 1000";
                                var jrs = qp1.ExecuteIdsQuery(query).ToList<Deposit>();
                                // build the  request
                                //encodedQuery = WebUtility.UrlEncode(query);
                                ////add qbobase url and query
                                //uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                                //res = "";

                                //client = new HttpClient();

                                //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                                //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                                //res = await client.GetStringAsync(uri);
                                //dyn = JsonConvert.DeserializeObject(res);
                                //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                                //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                                //jstr = JsonConvert.SerializeObject(Acc);
                                //var jrs = JsonConvert.DeserializeObject<List<Deposit>>(jstr).ToList();

                                if (jrs != null && jrs.Count() > 0)
                                {
                                    Allj.AddRange(jrs);

                                }
                            }
                        }




                        List<Payment> Paymentlist = new List<QuickBookController.Payment>();

                        SqlDataReader dr3;
                        DBFunctions db3 = new DBFunctions();
                        Hashtable ht3 = new Hashtable();

                        ht3.Clear();
                        ht3.Add("@Mode", "GET");

                        dr3 = db3.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht3);
                        while (dr3.Read())
                        {
                            Payment p = new Payment();
                            p.PaymentID = Convert.ToString(dr3["PaymentID"]);
                            p.QBOID = Convert.ToString(dr3["QBOID"]);
                            p.CheckNumber = Convert.ToString(dr3["CheckNumber"]);
                            Paymentlist.Add(p);

                        }
                        dr2.Close();
                        string time = DateTime.UtcNow.ToString("yyyy.MM.dd",
                                               CultureInfo.InvariantCulture);

                        string serverpath = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/");
                        serverpath += "QBO_" + time + ".txt";
                        if (!System.IO.File.Exists(serverpath))
                        {
                            System.IO.File.Create(serverpath).Dispose();

                        }
                        using (StreamWriter sw = System.IO.File.AppendText(serverpath))
                        {
                            sw.WriteLine("Started syncing..." + DateTime.Now.ToString());
                            sw.WriteLine("...................................................................................");
                            sw.WriteLine("...................................................................................");
                            sw.WriteLine("...................................................................................");

                        }



                        foreach (var a in Paymentlist)
                        {
                            Purchase p = Allp.Where(P => P.Id == a.QBOID).FirstOrDefault();


                            string message = "";
                            if (p != null)
                            {

                                if ((Convert.ToString(p.DocNumber) != Convert.ToString(a.CheckNumber)) && p.DocNumber != null)
                                {
                                    message = "Changing payment " + a.CheckNumber + " of bridge to " + Convert.ToString(p.DocNumber);
                                    updateqbosentdata(a.PaymentID, Convert.ToString(p.DocNumber), Convert.ToString(p.PrintStatus));
                                }
                                message = "Payment " + Convert.ToString(p.DocNumber) + " is not changed";

                            }
                            else
                            {



                                Deposit j = Allj.Where(J => J.Id == a.QBOID).FirstOrDefault();



                                if (j != null)
                                {

                                    if ((Convert.ToString(j.DocNumber) != Convert.ToString(a.CheckNumber)) && j.DocNumber != null)
                                    {
                                        message = "Changing Refund " + a.CheckNumber + " of bridge to " + Convert.ToString(j.DocNumber);
                                        //updateqbosentdata(a.PaymentID, Convert.ToString(j.DocNumber),"");
                                    }
                                    message = "Refund " + Convert.ToString(j.DocNumber) + " is not changed";

                                }




                                else
                                {

                                    message = "No payment for " + a.CheckNumber + " of bridge in QBO.";
                                    //updateqbosentdata(a.PaymentID, "", "");
                                }



                            }





                            using (StreamWriter sw = System.IO.File.AppendText(serverpath))
                            {
                                sw.WriteLine(message);

                            }


                        }
                        using (StreamWriter sw = System.IO.File.AppendText(serverpath))
                        {
                            sw.WriteLine("Syncing End... ");
                            sw.WriteLine("...................................................................................");
                            sw.WriteLine("...................................................................................");
                            sw.WriteLine("...................................................................................");

                        }


                    }


                }


            }

            catch (Exception e)
            {
            }
            return result;
        }



        //public async void Allpaymentnumbersync()
        //{
        //    try
        //    {
        //        string result = "";
        //        List<string> qboid = new List<string>();
        //        List<string> synctoken = new List<string>();
        //        string jsnstr = "{\"data\":[";

        //        string accessToken = "";
        //        string accessTokenSecret = "";
        //        string Refresh_Token = "";
        //        string key = WebConfigurationManager.AppSettings["companyID"];
        //        SqlDataReader dr2;
        //        DBFunctions db2 = new DBFunctions();
        //        Hashtable ht2 = new Hashtable();
        //        ht2.Clear();
        //        ht2.Add("@Mode", "GET");
        //        ht2.Add("@Key", key);
        //        dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
        //        while (dr2.Read())
        //        {
        //            accessToken = Convert.ToString(dr2["accessToken"]);
        //            accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
        //            key = Convert.ToString(dr2["Key"]);
        //            Refresh_Token = Convert.ToString(dr2["RefreshToken"]);
        //            tokenEndpoint = Convert.ToString(dr2["TokenEndpoint"]);
        //        }
        //        dr2.Close();
        //        accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
        //        OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(accessToken);

        //        string appToken = WebConfigurationManager.AppSettings["appToken"];
        //        string companyID = key;
        //        ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
        //        context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
        //        QueryService<Purchase> qp = new QueryService<Purchase>(context2);
        //        string json = "";
        //        int indx = 0;

        //        accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
        //        var principal = User as ClaimsPrincipal;
        //        //QBOAccntID = "35";

        //        string query = "Select count(*) from Purchase";
        //        // build the  request
        //        string encodedQuery = WebUtility.UrlEncode(query);
        //        if (key != "")
        //        {
        //            string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
        //            //add qbobase url and query
        //            string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

        //            string res = "";

        //            var client = new HttpClient();

        //            client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
        //            client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
        //            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        //            res = await client.GetStringAsync(uri);
        //            //Account accounts = JsonConvert.DeserializeObject<QryRes<QueryRespACInfo>>(res).QueryRes.accounts.FirstOrDefault();

        //            //QueryRespACInfo str = new QueryRespACInfo();
        //            dynamic dyn;
        //            //str = JsonConvert.DeserializeObject<QueryRespACInfo>(res);
        //            dyn = JsonConvert.DeserializeObject(res);
        //            var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
        //            Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
        //            string jstr = JsonConvert.SerializeObject(Acc);
        //            var pcounto = JsonConvert.DeserializeObject<List<Purchase>>(jstr).ToList();
        //            if (pcounto == null)
        //            {
        //                result = "INVALID";
        //            }
        //            else
        //            {
        //                List<Purchase> Allp = new List<Purchase>();

        //                int pcount = pcounto.Count;
        //                if (pcount < 1000)
        //                {

        //                    query = "Select * from Purchase MaxResults 1000";
        //                    // build the  request
        //                    encodedQuery = WebUtility.UrlEncode(query);
        //                    //add qbobase url and query
        //                    uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

        //                    res = "";

        //                    client = new HttpClient();

        //                    client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
        //                    client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
        //                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        //                    res = await client.GetStringAsync(uri);
        //                    dyn = JsonConvert.DeserializeObject(res);
        //                    Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
        //                    Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
        //                    jstr = JsonConvert.SerializeObject(Acc);
        //                    var prs = JsonConvert.DeserializeObject<List<Purchase>>(jstr).ToList();



        //                    string strDelete = "1";
        //                    if (prs != null && prs.Count() > 0)
        //                    {
        //                        Allp.AddRange(prs);
        //                    }


        //                }
        //                else
        //                {
        //                    string strDelete = "1";

        //                    for (int a = 1; a <= pcount; a = a + 1000)
        //                    {

        //                        query = "Select * from Purchase STARTPOSITION " + a + " MAXRESULTS 1000";
        //                        // build the  request
        //                        encodedQuery = WebUtility.UrlEncode(query);
        //                        //add qbobase url and query
        //                        uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

        //                        res = "";

        //                        client = new HttpClient();

        //                        client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
        //                        client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
        //                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        //                        res = await client.GetStringAsync(uri);
        //                        dyn = JsonConvert.DeserializeObject(res);
        //                        Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
        //                        Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
        //                        jstr = JsonConvert.SerializeObject(Acc);
        //                        var prs = JsonConvert.DeserializeObject<List<Purchase>>(jstr).ToList();

        //                        if (prs != null && prs.Count() > 0)
        //                        {
        //                            Allp.AddRange(prs);

        //                        }
        //                    }
        //                }


        //                query = "Select count(*) from Deposit";
        //                // build the  request
        //                encodedQuery = WebUtility.UrlEncode(query);
        //                //add qbobase url and query
        //                uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

        //                res = "";

        //                client = new HttpClient();

        //                client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
        //                client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
        //                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        //                res = await client.GetStringAsync(uri);
        //                dyn = JsonConvert.DeserializeObject(res);
        //                Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
        //                Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
        //                jstr = JsonConvert.SerializeObject(Acc);
        //                var jcounto = JsonConvert.DeserializeObject<List<Deposit>>(jstr).ToList();




        //                List<Deposit> Allj = new List<Deposit>();

        //                int jcount = jcounto.Count;
        //                if (jcount < 1000)
        //                {

        //                    query = "Select * from Deposit MaxResults 1000";
        //                    // build the  request
        //                    encodedQuery = WebUtility.UrlEncode(query);
        //                    //add qbobase url and query
        //                    uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

        //                    res = "";

        //                    client = new HttpClient();

        //                    client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
        //                    client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
        //                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        //                    res = await client.GetStringAsync(uri);
        //                    dyn = JsonConvert.DeserializeObject(res);
        //                    Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
        //                    Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
        //                    jstr = JsonConvert.SerializeObject(Acc);
        //                    var jrs = JsonConvert.DeserializeObject<List<Deposit>>(jstr).ToList();



        //                    string strDelete = "1";
        //                    if (jrs != null && jrs.Count() > 0)
        //                    {
        //                        Allj.AddRange(jrs);
        //                    }


        //                }
        //                else
        //                {
        //                    string strDelete = "1";

        //                    for (int a = 1; a <= jcount; a = a + 1000)
        //                    {

        //                        query = "Select * from Deposit STARTPOSITION " + a + " MAXRESULTS 1000";
        //                        // build the  request
        //                        encodedQuery = WebUtility.UrlEncode(query);
        //                        //add qbobase url and query
        //                        uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

        //                        res = "";

        //                        client = new HttpClient();

        //                        client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
        //                        client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
        //                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        //                        res = await client.GetStringAsync(uri);
        //                        dyn = JsonConvert.DeserializeObject(res);
        //                        Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
        //                        Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
        //                        jstr = JsonConvert.SerializeObject(Acc);
        //                        var jrs = JsonConvert.DeserializeObject<List<Deposit>>(jstr).ToList();

        //                        if (jrs != null && jrs.Count() > 0)
        //                        {
        //                            Allj.AddRange(jrs);

        //                        }
        //                    }
        //                }




        //                List<Payment> Paymentlist = new List<QuickBookController.Payment>();

        //                SqlDataReader dr3;
        //                DBFunctions db3 = new DBFunctions();
        //                Hashtable ht3 = new Hashtable();

        //                ht3.Clear();
        //                ht3.Add("@Mode", "GET");

        //                dr3 = db3.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht3);
        //                while (dr3.Read())
        //                {
        //                    Payment p = new Payment();
        //                    p.PaymentID = Convert.ToString(dr3["PaymentID"]);
        //                    p.QBOID = Convert.ToString(dr3["QBOID"]);
        //                    p.CheckNumber = Convert.ToString(dr3["CheckNumber"]);
        //                    Paymentlist.Add(p);

        //                }
        //                dr2.Close();
        //                string time = DateTime.UtcNow.ToString("yyyy.MM.dd",
        //                                       CultureInfo.InvariantCulture);

        //                string serverpath = System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/");
        //                serverpath += "QBO_" + time + ".txt";
        //                if (!System.IO.File.Exists(serverpath))
        //                {
        //                    System.IO.File.Create(serverpath).Dispose();

        //                }
        //                using (StreamWriter sw = System.IO.File.AppendText(serverpath))
        //                {
        //                    sw.WriteLine("Started syncing..." + DateTime.Now.ToString());
        //                    sw.WriteLine("...................................................................................");
        //                    sw.WriteLine("...................................................................................");
        //                    sw.WriteLine("...................................................................................");

        //                }



        //                foreach (var a in Paymentlist)
        //                {
        //                    Purchase p = Allp.Where(P => P.Id == a.QBOID).FirstOrDefault();


        //                    string message = "";
        //                    if (p != null)
        //                    {

        //                        if ((Convert.ToString(p.DocNumber) != Convert.ToString(a.CheckNumber)) && p.DocNumber != null)
        //                        {
        //                            message = "Changing payment " + a.CheckNumber + " of bridge to " + Convert.ToString(p.DocNumber);
        //                            updateqbosentdata(a.PaymentID, Convert.ToString(p.DocNumber), Convert.ToString(p.PrintStatus));
        //                        }
        //                        message = "Payment " + Convert.ToString(p.DocNumber) + " is not changed";

        //                    }
        //                    else
        //                    {



        //                        Deposit j = Allj.Where(J => J.Id == a.QBOID).FirstOrDefault();



        //                        if (j != null)
        //                        {

        //                            if ((Convert.ToString(j.DocNumber) != Convert.ToString(a.CheckNumber)) && j.DocNumber != null)
        //                            {
        //                                message = "Changing Refund " + a.CheckNumber + " of bridge to " + Convert.ToString(j.DocNumber);
        //                                //updateqbosentdata(a.PaymentID, Convert.ToString(j.DocNumber),"");
        //                            }
        //                            message = "Refund " + Convert.ToString(j.DocNumber) + " is not changed";

        //                        }




        //                        else
        //                        {

        //                            message = "No payment for " + a.CheckNumber + " of bridge in QBO.";
        //                            //updateqbosentdata(a.PaymentID, "", "");
        //                        }



        //                    }





        //                    using (StreamWriter sw = System.IO.File.AppendText(serverpath))
        //                    {
        //                        sw.WriteLine(message);

        //                    }


        //                }
        //                using (StreamWriter sw = System.IO.File.AppendText(serverpath))
        //                {
        //                    sw.WriteLine("Syncing End... ");
        //                    sw.WriteLine("...................................................................................");
        //                    sw.WriteLine("...................................................................................");
        //                    sw.WriteLine("...................................................................................");

        //                }


        //            }


        //        }


        //    }

        //    catch (Exception e)
        //    {
        //    }

        //}

        /*verify the account against account name and return the balance*/
        public async Task<string> CheckBankAccount(string name)
        {


            string QBOAccntID = "";
            //string QBOAccntID = "";

            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            ht.Add("@AccntID", name);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProcForBankAccnt]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                QBOAccntID = Convert.ToString(dr["QBid"]);
                Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                if (tokenEndpoint == "")
                    tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);

            }
            dr.Close();
            if (QBOAccntID != "")
            {
                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                //Oauth 2.0
                var principal = User as ClaimsPrincipal;
                //QBOAccntID = "35";

                string query = "Select * from Account where ID ='" + QBOAccntID + "'";
                // build the  request
                string encodedQuery = WebUtility.UrlEncode(query);
                if (key != "")
                {
                    string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                    //add qbobase url and query
                    string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                    string res = "";
                    try
                    {
                        var client = new HttpClient();

                        client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                        client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                        res = await client.GetStringAsync(uri);
                        //Account accounts = JsonConvert.DeserializeObject<QryRes<QueryRespACInfo>>(res).QueryRes.accounts.FirstOrDefault();

                        //QueryRespACInfo str = new QueryRespACInfo();
                        dynamic dyn;
                        //str = JsonConvert.DeserializeObject<QueryRespACInfo>(res);
                        dyn = JsonConvert.DeserializeObject(res);
                        var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                        Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                        string jstr = JsonConvert.SerializeObject(Acc);
                        var json = JArray.Parse(jstr);
                        for (int i = 0; i < json.Count; i++)
                        {
                            string ret = RemoveSpace(json[i]["AccountType"].ToString());
                            json[i]["AccountType"] = ret;
                        }
                        jstr = JsonConvert.SerializeObject(json);
                        Account str = JsonConvert.DeserializeObject<List<Account>>(jstr).FirstOrDefault();
                        if (str == null)
                        {
                            result = "INVALID";
                        }
                        else
                        {
                            string sblnc = string.Empty;
                            Decimal blnc = str.CurrentBalance;
                            if (blnc > 0)
                            {
                                sblnc = string.Format("{0:#.00}", Convert.ToDecimal(str.CurrentBalance * 100) / 100);
                            }
                            else
                            {
                                sblnc = "-" + string.Format("{0:#.00}", 0 - Convert.ToDecimal(str.CurrentBalance * 100) / 100) + "";

                            }
                            result = sblnc + " (" + str.CurrencyRef.Value.ToString() + ")" + "^^^" + str.Id.ToString();
                        }
                        return result;
                    }
                    catch (Exception ex)
                    {
                        return "INVALID";
                    }

                }
            }
            else
            {
                return "INVALID";
            }
            //Oauth 1.0
            //OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
            //        accessToken);
            //string appToken = WebConfigurationManager.AppSettings["appToken"];
            //string companyID = key;
            //ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
            //context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            //var service = new QueryService<Account>(context2);
            //try
            //{
            //    if (QBOAccntID == "")
            //    {
            //        result = "INVALID";
            //    }
            //    else
            //    {
            //        var accounts = service.ExecuteIdsQuery("Select * from Account where ID ='" + QBOAccntID + "'").FirstOrDefault();
            //        if (accounts == null)
            //        {
            //            result = "INVALID";
            //        }
            //        else
            //        {
            //            string sblnc = string.Empty;
            //            Decimal blnc = accounts.CurrentBalance;
            //            if (blnc > 0)
            //            {
            //                sblnc = string.Format("{0:#.00}", Convert.ToDecimal(accounts.CurrentBalance * 100) / 100);
            //            }
            //            else
            //            {
            //                sblnc = "-" + string.Format("{0:#.00}", 0 - Convert.ToDecimal(accounts.CurrentBalance * 100) / 100) + "";

            //            }
            //            result = sblnc + " (" + accounts.CurrencyRef.Value.ToString() + ")" + "^^^" + accounts.Id.ToString();
            //        }
            //    }
            //}

            return result;
        }





        /* Get the invoice level accounts and the id from QBO */
        public string getAllInvoiceAccounts(string invoices)
        {
            string name = "";
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@InvoiceID", invoices);
            dr2 = db2.SysFetchData("[dbo].[poms_Getinvaccountforqbo]", ref ht2);
            while (dr2.Read())
            {
                string r = Convert.ToString(dr2["AccountNumber"]) + "|||" + Convert.ToString(dr2["Description"]);
                name = name == "" ? r : "^^^" + r;
            }
            dr2.Close();
            return name;
        }

        /* Checking the invoice account and balance */
        public async Task<string> CheckInvoiceAccount(string name)
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            string QBOAccntID = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            ht.Add("@AccntID", name);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProcForBankAccnt]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                QBOAccntID = Convert.ToString(dr["QBid"]);
                Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
            }
            dr.Close();
            accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
            //Oauth 2.0

            // build the  request
            if (key != "")
            {
                OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                        accessToken);
                string appToken = WebConfigurationManager.AppSettings["appToken"];
                string realmId = key;
                ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                var service = new QueryService<Account>(context2);


                if (QBOAccntID == "")
                {
                    result = "INVALID";
                }
                else
                {
                    var accounts = service.ExecuteIdsQuery("Select * from Account where ID ='" + QBOAccntID + "'").FirstOrDefault();


                    if (accounts == null)
                    {
                        result = "INVALID";
                    }
                    else
                    {
                        result = accounts.CurrentBalance + " (" + accounts.CurrencyRef.Value.ToString() + ")" + "^^^" + accounts.Id.ToString();
                    }
                }
            }



            return result;
        }

        public async Task<string> senddatatoqbo_journal(POMS.Models.QBOCheck qboc)
        {
            try
            {

                string accessToken = "";
                string accessTokenSecret = "";
                string Refresh_Token = "";
                string key = "";//WebConfigurationManager.AppSettings["companyID"];
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@Mode", "GET");
                ht.Add("@Key", key);
                dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);

                while (dr.Read())
                {
                    accessToken = Convert.ToString(dr["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                    key = Convert.ToString(dr["Key"]);
                    Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                    tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
                }
                dr.Close();

                Deposit de = new Deposit();

                de.DocNumber = qboc.Checknumber;
                de.PrivateNote = qboc.Memo;

                Line[] ln = new Line[2];

                int tmp = 0;

                decimal amounttotal = 0;
                string invoicenumbers = "";
                foreach (var l in qboc.items)
                {
                    amounttotal += Convert.ToDecimal(l.total);
                    invoicenumbers += invoicenumbers == "" ? l.description : " , " + l.description;
                }

                ln[tmp] = new Line();
                ln[tmp].Amount = 0 - Convert.ToDecimal(qboc.Total);
                ln[tmp].AmountSpecified = true;

                DepositLineDetail jd = new DepositLineDetail();

                jd.AccountRef = new ReferenceType();
                jd.AccountRef.Value = qboc.items[0].accountid;
                jd.Entity = new ReferenceType();
                jd.Entity.Value = qboc.VendorID;

                jd.PaymentMethodRef = new ReferenceType();
                jd.PaymentMethodRef.Value = PaymentMethodEnum.Check.ToString();

                ln[tmp].Description = invoicenumbers;
                ln[tmp].AnyIntuitObject = jd;
                ln[tmp].DetailType = LineDetailTypeEnum.DepositLineDetail;
                ln[tmp].DetailTypeSpecified = true;
                tmp++;


                ln[tmp] = new Line();
                ln[tmp].Amount = 0 - Convert.ToDecimal(amounttotal);
                ln[tmp].AmountSpecified = true;



                DepositLineDetail jd1 = new DepositLineDetail();

                jd1.AccountRef = new ReferenceType();
                jd1.AccountRef.Value = qboc.Bank;



                //jd1.Entity.Value = qboc.VendorID;
                //jd1.PaymentMethodRef.Value = PaymentMethodEnum.Check.ToString();

                //jd1.Entity = new EntityTypeRef();
                jd1.Entity = new ReferenceType();
                jd1.Entity.Value = qboc.VendorID;



                ln[tmp].AnyIntuitObject = jd1;
                ln[tmp].DetailTypeSpecified = true;

                //if (qboc.VendorCurrency != qboc.BankCurr)
                //{
                //    je.ExchangeRate = Convert.ToDecimal(qboc.ExchangeRate);
                //    je.ExchangeRateSpecified = true;
                //}
                de.CurrencyRef = new ReferenceType();
                de.CurrencyRef.Value = qboc.BankCurr.ToString();
                de.HomeTotalAmt = 0 - Convert.ToDecimal(qboc.items[0].total);
                de.HomeTotalAmtSpecified = true;
                de.Line = ln;
                de.TxnDate = Convert.ToDateTime(qboc.Paymentdate);
                de.TxnDateSpecified = true;
                de.DepositToAccountRef = new ReferenceType();
                de.DepositToAccountRef.Value = qboc.Bank;

                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);

                OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                        accessToken);
                string realmId = key;
                ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                var service = new DataService(context2);

                Deposit d = service.Add<Deposit>(de) as Deposit;
                de.Id = d.Id;
                de.SyncToken = d.SyncToken;

                string uid = Convert.ToString(Session["UserID"]);
                SqlDataReader dr2;
                DBFunctions db2 = new DBFunctions();
                Hashtable ht2 = new Hashtable();

                ht2.Clear();
                ht2.Add("@Mode", "INSERT");
                ht2.Add("@PaymentID", qboc.BridgePaymentID);
                ht2.Add("@VendorID", qboc.BridgeVendorID);
                ht2.Add("@Checknumber", qboc.Checknumber);
                ht2.Add("@Amount", qboc.Total);
                ht2.Add("@UserID", uid);
                ht2.Add("@QBOID", de.Id.ToString());
                ht2.Add("@SyncToken", de.SyncToken.ToString());
                dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
                while (dr2.Read())
                {
                }
                dr2.Close();
                return de.Id;
            }
            catch (Exception ex)
            {
                return "INVALID";
            }

        }

        public string senddatatoqbo_journal_old(POMS.Models.QBOCheck qboc)
        {
            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);

            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
            }
            dr.Close();
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                    accessToken);
            string appToken = WebConfigurationManager.AppSettings["appToken"];
            string realmId = key;
            ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service = new DataService(context2);
            JournalEntry je = new JournalEntry();
            je.DocNumber = qboc.Checknumber;
            je.PrivateNote = qboc.Memo;
            Line[] ln = new Line[2];
            int tmp = 0;
            ln[tmp] = new Line();
            ln[tmp].Amount = 0 - Convert.ToDecimal(qboc.Total);
            ln[tmp].AmountSpecified = true;
            ln[tmp].DetailType = LineDetailTypeEnum.JournalEntryLineDetail;
            JournalEntryLineDetail jd = new JournalEntryLineDetail();
            jd.PostingType = PostingTypeEnum.Debit;
            jd.PostingTypeSpecified = true;
            jd.AccountRef = new ReferenceType();
            jd.AccountRef.Value = qboc.Bank;
            ln[tmp].AnyIntuitObject = jd;
            ln[tmp].DetailTypeSpecified = true;
            tmp++;
            decimal amounttotal = 0;
            string invoicenumbers = "";
            foreach (var l in qboc.items)
            {
                amounttotal += Convert.ToDecimal(l.total);
                invoicenumbers += invoicenumbers == "" ? l.description : " , " + l.description;
            }

            ln[tmp] = new Line();
            ln[tmp].Amount = 0 - Convert.ToDecimal(amounttotal);
            ln[tmp].AmountSpecified = true;
            ln[tmp].DetailType = LineDetailTypeEnum.JournalEntryLineDetail;
            ln[tmp].Description = invoicenumbers;
            JournalEntryLineDetail jd1 = new JournalEntryLineDetail();
            jd1.PostingType = PostingTypeEnum.Credit;
            jd1.PostingTypeSpecified = true;
            jd1.AccountRef = new ReferenceType();
            jd1.AccountRef.Value = qboc.items[0].accountid;
            jd1.Entity = new EntityTypeRef();
            jd1.Entity.EntityRef = new ReferenceType();
            jd1.Entity.EntityRef.Value = qboc.VendorID;
            ln[tmp].AnyIntuitObject = jd1;
            ln[tmp].DetailTypeSpecified = true;

            //if (qboc.VendorCurrency != qboc.BankCurr)
            //{
            //    je.ExchangeRate = Convert.ToDecimal(qboc.ExchangeRate);
            //    je.ExchangeRateSpecified = true;
            //}
            je.CurrencyRef = new ReferenceType();
            je.CurrencyRef.Value = "USD";
            je.HomeTotalAmt = 0 - Convert.ToDecimal(qboc.items[0].total);
            je.HomeTotalAmtSpecified = true;
            je.Line = ln;
            je.TxnDate = Convert.ToDateTime(qboc.Paymentdate);
            je.TxnDateSpecified = true;
            je = service.Add<JournalEntry>(je) as JournalEntry;

            string uid = Convert.ToString(Session["UserID"]);
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();

            ht2.Clear();
            ht2.Add("@Mode", "INSERT");
            ht2.Add("@PaymentID", qboc.BridgePaymentID);
            ht2.Add("@VendorID", qboc.BridgeVendorID);
            ht2.Add("@Checknumber", qboc.Checknumber);
            ht2.Add("@Amount", qboc.Total);
            ht2.Add("@UserID", uid);
            ht2.Add("@QBOID", je.Id.ToString());
            ht2.Add("@SyncToken", je.SyncToken.ToString());
            dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
            while (dr2.Read())
            {
            }
            dr2.Close();
            return "";
        }

        /* final method to send the data to QBO*/
        public async Task<string> senddatatoqbo(POMS.Models.QBOCheck qboc)
        {
            //return "";
            if (Convert.ToDecimal(qboc.Total) < 0)
            {
                //senddatatoqbo_journal(qboc);
                string accessToken = "";
                string accessTokenSecret = "";
                string Refresh_Token = "";
                string key = "";//WebConfigurationManager.AppSettings["companyID"];
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@Mode", "GET");
                ht.Add("@Key", key);
                dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);

                while (dr.Read())
                {
                    accessToken = Convert.ToString(dr["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                    key = Convert.ToString(dr["Key"]);
                    Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                    tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
                }
                dr.Close();

                Deposit de = new Deposit();
                // RefundReceipt de = new RefundReceipt();

                string updateid = Convert.ToString(Session["UpdateQBOID"]);
                Session["UpdateQBOID"] = null;
                if (updateid != "" && updateid != null)
                {
                    try
                    {
                        string query = "select * from deposit where id = '" + updateid + "'";
                        string realmId = key;
                        accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                        OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                            accessToken);
                        ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                        context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                        var service1 = new QueryService<Deposit>(context2);
                        Deposit str = service1.ExecuteIdsQuery(query).FirstOrDefault();

                        de.Id = str.Id;
                        de.SyncToken = str.SyncToken;
                        de.MetaData = str.MetaData;
                        de.sparse = false;
                        de.DocNumber = qboc.Checknumber;
                        if (qboc.Memo != null)
                        {
                            de.PrivateNote = qboc.Checknumber + " | " + qboc.Memo;
                        }
                        else
                        {
                            de.PrivateNote = qboc.Checknumber;
                        }
                        de.TotalAmt = 0 - Convert.ToDecimal(qboc.Total);

                        for (int i = 0; i < str.Line.Count(); i++)
                        {
                            str.Line[i].Amount = 0 - Convert.ToDecimal(qboc.items[i].total);
                            str.Line[i].Description = qboc.items[i].description;
                            DepositLineDetail jd = new DepositLineDetail();
                            jd.AccountRef = new ReferenceType();
                            jd.AccountRef.Value = qboc.items[0].accountid;
                            jd.Entity = new ReferenceType();
                            jd.Entity.Value = qboc.VendorID;
                            jd.CheckNum = qboc.Checknumber;

                            jd.PaymentMethodRef = new ReferenceType();
                            jd.PaymentMethodRef.Value = PaymentMethodEnum.Check.ToString();
                            str.Line[i].AnyIntuitObject = jd;
                        }
                        de.CurrencyRef = new ReferenceType();
                        de.CurrencyRef.Value = qboc.BankCurr.ToString();
                        //de.HomeTotalAmt = 0 - Convert.ToDecimal(qboc.items[0].total);
                        de.HomeTotalAmtSpecified = true;
                        de.Line = str.Line;
                        de.TxnDate = Convert.ToDateTime(qboc.Paymentdate);
                        de.TxnDateSpecified = true;
                        de.DepositToAccountRef = new ReferenceType();
                        de.DepositToAccountRef.Value = qboc.Bank;
                        de.domain = "QBO";
                        //lnn[0] = ln[0];
                        //de.Line = lnn;

                        accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                        oauthValidator = new OAuth2RequestValidator(
                            accessToken);
                        realmId = key;
                        ServiceContext context1 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                        context1.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                        var service = new DataService(context1);
                        Deposit d = service.Update<Deposit>(de) as Deposit;
                    }
                    catch (Exception e)
                    {

                    }
                    string uid = Convert.ToString(Session["UserID"]);
                    SqlDataReader dr2;
                    DBFunctions db2 = new DBFunctions();
                    Hashtable ht2 = new Hashtable();

                    if (qboc.Checknumber == null)
                    {
                        qboc.Checknumber = "N/A";
                    }

                    ht2.Clear();
                    ht2.Add("@Mode", "UPDATE");
                    ht2.Add("@PaymentID", qboc.BridgePaymentID);
                    ht2.Add("@VendorID", qboc.BridgeVendorID);
                    ht2.Add("@Checknumber", qboc.Checknumber);
                    ht2.Add("@Amount", qboc.Total);
                    ht2.Add("@UserID", uid);
                    ht2.Add("@QBOID", de.Id.ToString());
                    ht2.Add("@SyncToken", de.SyncToken.ToString());
                    dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
                    while (dr2.Read())
                    {
                    }
                    dr2.Close();
                }
                else
                {
                    de.DocNumber = qboc.Checknumber;
                    if (qboc.Memo != null)
                    {
                        de.PrivateNote = qboc.Checknumber + " | " + qboc.Memo;
                    }
                    else
                    {
                        de.PrivateNote = qboc.Checknumber;
                    }

                    Line[] ln = new Line[2];

                    int tmp = 0;

                    decimal amounttotal = 0;
                    string invoicenumbers = "";
                    foreach (var l in qboc.items)
                    {
                        amounttotal += Convert.ToDecimal(l.total);
                        invoicenumbers += invoicenumbers == "" ? l.description : " , " + l.description;
                    }

                    ln[tmp] = new Line();
                    ln[tmp].Amount = 0 - Convert.ToDecimal(qboc.Total);
                    ln[tmp].AmountSpecified = true;

                    DepositLineDetail jd = new DepositLineDetail();

                    jd.AccountRef = new ReferenceType();
                    jd.AccountRef.Value = qboc.items[0].accountid;
                    jd.Entity = new ReferenceType();
                    jd.Entity.Value = qboc.VendorID;
                    jd.CheckNum = qboc.Checknumber;

                    jd.PaymentMethodRef = new ReferenceType();
                    jd.PaymentMethodRef.Value = PaymentMethodEnum.Check.ToString();

                    ln[tmp].Description = invoicenumbers;
                    ln[tmp].AnyIntuitObject = jd;
                    ln[tmp].DetailType = LineDetailTypeEnum.DepositLineDetail;
                    ln[tmp].DetailTypeSpecified = true;
                    tmp++;


                    ln[tmp] = new Line();
                    ln[tmp].Amount = 0 - Convert.ToDecimal(amounttotal);
                    ln[tmp].AmountSpecified = true;



                    DepositLineDetail jd1 = new DepositLineDetail();

                    jd1.AccountRef = new ReferenceType();
                    jd1.AccountRef.Value = qboc.Bank;



                    //jd1.Entity.Value = qboc.VendorID;
                    //jd1.PaymentMethodRef.Value = PaymentMethodEnum.Check.ToString();

                    //jd1.Entity = new EntityTypeRef();
                    jd1.Entity = new ReferenceType();
                    jd1.Entity.Value = qboc.VendorID;



                    ln[tmp].AnyIntuitObject = jd1;
                    ln[tmp].DetailTypeSpecified = true;

                    //if (qboc.VendorCurrency != qboc.BankCurr)
                    //{
                    //    je.ExchangeRate = Convert.ToDecimal(qboc.ExchangeRate);
                    //    je.ExchangeRateSpecified = true;
                    //}
                    de.CurrencyRef = new ReferenceType();
                    de.CurrencyRef.Value = qboc.BankCurr.ToString();
                    de.HomeTotalAmt = 0 - Convert.ToDecimal(qboc.items[0].total);
                    de.HomeTotalAmtSpecified = true;
                    de.Line = ln;
                    de.TxnDate = Convert.ToDateTime(qboc.Paymentdate);
                    de.TxnDateSpecified = true;
                    de.DepositToAccountRef = new ReferenceType();
                    de.DepositToAccountRef.Value = qboc.Bank;

                    accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);

                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                            accessToken);
                    string realmId = key;
                    ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                    context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                    var service = new DataService(context2);

                    Deposit d = service.Add<Deposit>(de) as Deposit;
                    de.Id = d.Id;
                    de.SyncToken = d.SyncToken;

                    string uid = Convert.ToString(Session["UserID"]);
                    SqlDataReader dr2;
                    DBFunctions db2 = new DBFunctions();
                    Hashtable ht2 = new Hashtable();

                    ht2.Clear();
                    ht2.Add("@Mode", "INSERT");
                    ht2.Add("@PaymentID", qboc.BridgePaymentID);
                    ht2.Add("@VendorID", qboc.BridgeVendorID);
                    ht2.Add("@Checknumber", qboc.Checknumber);
                    ht2.Add("@Amount", qboc.Total);
                    ht2.Add("@UserID", uid);
                    ht2.Add("@QBOID", de.Id.ToString());
                    ht2.Add("@SyncToken", de.SyncToken.ToString());
                    dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
                    while (dr2.Read())
                    {
                    }
                    dr2.Close();

                }
                return de.Id;
            }
            else
            {
                string accessToken = "";
                string accessTokenSecret = "";
                string key = "";
                string Refresh_Token = "";
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();
                ht.Clear();
                ht.Add("@Mode", "GET");
                ht.Add("@Key", key);
                dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
                while (dr.Read())
                {
                    accessToken = Convert.ToString(dr["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                    key = Convert.ToString(dr["Key"]);
                    Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                    tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
                }
                dr.Close();
                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                        accessToken);
                string appToken = WebConfigurationManager.AppSettings["appToken"];
                string realmId = key;
                ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                var service = new DataService(context2);

                /* Purchase class will be used to write checks to QBO */
                Purchase pur = new Purchase();
                pur.PaymentType = PaymentTypeEnum.Check;
                pur.PaymentTypeSpecified = true;

                /*Payee info*/
                pur.EntityRef = new ReferenceType();
                pur.EntityRef.Value = qboc.VendorID;

                /*check number*/
                if (qboc.Type == "Cheque" && (qboc.Checknumber == "N/A" || qboc.Checknumber == "To be printed"))
                {
                    pur.PrintStatus = PrintStatusEnum.NeedToPrint;
                    pur.PrintStatusSpecified = true;
                }
                else
                {
                    pur.DocNumber = qboc.Checknumber;
                }

                /*Memo for check*/
                pur.PrivateNote = qboc.Memo;
                pur.TotalAmt = Convert.ToDecimal(qboc.Total);
                pur.TotalAmtSpecified = true;

                /*Payment account(Bank account)*/
                pur.AccountRef = new ReferenceType();
                pur.AccountRef.Value = qboc.Bank;

                /*Assign exchange rate if the payee currency differs from bank account*/
                //if (qboc.VendorCurrency != qboc.BankCurr)
                //{
                //    pur.ExchangeRate = Convert.ToDecimal(qboc.ExchangeRate);
                //    pur.ExchangeRateSpecified = true;
                //}
                pur.TxnDate = Convert.ToDateTime(qboc.Paymentdate);
                pur.TxnDateSpecified = true;
                PhysicalAddress pa = new PhysicalAddress();
                pa.Line1 = qboc.MailingAddress;
                pur.RemitToAddr = pa;

                /*Line items assignment. At least one item is mandatory*/
                Line[] ln = new Line[qboc.items.Count];
                int tmp = 0;
                foreach (var l in qboc.items)
                {
                    ln[tmp] = new Line();
                    ln[tmp].Amount = Convert.ToDecimal(l.total);
                    ln[tmp].AmountSpecified = true;

                    ln[tmp].DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                    ln[tmp].DetailTypeSpecified = true;

                    ln[tmp].Description = l.description;
                    AccountBasedExpenseLineDetail aaa = new AccountBasedExpenseLineDetail();
                    aaa.AccountRef = new ReferenceType();
                    aaa.AccountRef.Value = l.accountid;



                    //QueryService<TaxCode> Tc = new QueryService<TaxCode>(context2);
                    //List<TaxCode> Tcl = Tc.ExecuteIdsQuery("select * from Taxcode").ToList();

                    //foreach (var abc in Tcl){
                    //using (EventLog eventLog = new EventLog("Application"))
                    //{
                    //    eventLog.Source = "Application";

                    //    eventLog.WriteEntry(abc.Id + " is ID " + abc.Name + " >>>" + "", EventLogEntryType.Information, 102, 1);
                    //}
                    //}

                    //aaa.TaxCodeRef = new ReferenceType();
                    //aaa.TaxCodeRef.Value = "4";  //At server


                    //aaa.TaxCodeRef.Value = "2";  //At local
                    ////aaa.TaxAmountSpecified = false;

                    //aaa.TaxAmountSpecified = false;

                    ln[tmp].Id = Convert.ToString(tmp + 1);
                    ln[tmp].AnyIntuitObject = aaa;

                    tmp++;
                }

                pur.GlobalTaxCalculation = GlobalTaxCalculationEnum.NotApplicable;
                pur.GlobalTaxCalculationSpecified = true;
                pur.Line = ln;

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(pur.Line.Length.ToString() + " items are being sent and total is " + pur.TotalAmt.ToString(), EventLogEntryType.Information, 102, 1);
                }

                // return "";

                try
                {
                    /*check whether it will be edited  or created*/
                    string updateid = Convert.ToString(Session["UpdateQBOID"]);
                    Session["UpdateQBOID"] = null;

                    /*update existing check*/
                    if (updateid != "" && updateid != null)
                    {
                        //Oauth 2.0
                        var principal = User as ClaimsPrincipal;

                        string query = "select * from purchase where id = '" + updateid + "'";
                        // build the  request
                        //string encodedQuery = WebUtility.UrlEncode(query);
                        if (key != "")
                        {
                            string res = "";
                            try
                            {
                                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                                oauthValidator = new OAuth2RequestValidator(
                    accessToken);
                                realmId = key;
                                context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                                var service1 = new QueryService<Purchase>(context2);
                                Purchase str = service1.ExecuteIdsQuery(query).FirstOrDefault();

                                if (str != null)
                                {
                                    pur.SyncToken = str.SyncToken;
                                    pur.Id = str.Id;
                                    //dont send
                                    Purchase p = service.Update<Purchase>(pur) as Purchase;
                                    pur.Id = p.Id;
                                    res = pur.Id;
                                    updateqbosentdata(qboc.BridgePaymentID, pur.DocNumber.ToString(), pur.PrintStatus.ToString());
                                }
                                else
                                {
                                    res = "INVALID";
                                }
                            }
                            catch (Exception ex)
                            {
                                return "INVALID";
                            }
                            if (res == "")
                            {
                                return "null";
                            }
                            else
                            {
                                return res; //= //companyInfo.CompanyName + "***" + WriteDate;
                            }
                        }

                        //Oauth 1.0
                        //QueryService<Purchase> qr = new QueryService<Purchase>(context2);
                        //Purchase pu = qr.ExecuteIdsQuery("select * from purchase where id = '" + updateid + "'").FirstOrDefault();
                        //if (pu != null)
                        //{
                        //    pur.SyncToken = pu.SyncToken;
                        //    pur.Id = pu.Id;
                        //    //dont send
                        //    pur = service.Update(pur);
                        //}
                    }
                    /*Add new check*/
                    else
                    {
                        Purchase p = service.Add<Purchase>(pur) as Purchase;
                        pur.Id = p.Id;
                        pur.SyncToken = p.SyncToken;
                    }

                    /*Update or insert the QBO data to System*/
                    string uid = Convert.ToString(Session["UserID"]);
                    SqlDataReader dr2;
                    DBFunctions db2 = new DBFunctions();
                    Hashtable ht2 = new Hashtable();
                    ht2.Clear();
                    ht2.Add("@Mode", "INSERT");
                    ht2.Add("@PaymentID", qboc.BridgePaymentID);
                    ht2.Add("@VendorID", qboc.BridgeVendorID);
                    ht2.Add("@Checknumber", qboc.Type == "Cheque" ? "N/A" : qboc.Checknumber);
                    ht2.Add("@Amount", qboc.Total);
                    ht2.Add("@UserID", uid);
                    ht2.Add("@QBOID", pur.Id.ToString());
                    ht2.Add("@SyncToken", pur.SyncToken.ToString());
                    dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
                    while (dr2.Read())
                    {
                    }
                    dr2.Close();
                    return pur.Id;
                }
                catch (Exception e)
                {
                    return "INVALID^^^" + e.InnerException.Message.ToString();
                }
            }
            return "";
        }

        public async Task<string> CheckVendor(string name, string vname, string curr)
        {
            name = HttpUtility.HtmlDecode(name);
            vname = HttpUtility.HtmlDecode(vname);
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
            }
            dr.Close();

            accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                    accessToken);
            string realmId = key;
            ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            var service = new QueryService<Vendor>(context2);
            vname = vname.Replace("'", @"\'");
            //Oauth 2.0
            var principal = User as ClaimsPrincipal;

            string query = "Select * from vendor where DisplayName='" + vname + "'";
            // build the  request
            string encodedQuery = WebUtility.UrlEncode(query);
            if (key != "")
            {
                string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                //add qbobase url and query
                string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                string res = "";
                try
                {
                    var client = new HttpClient();

                    client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                    client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    res = await client.GetStringAsync(uri);
                    //Vendor Vendor = JsonConvert.DeserializeObject<QryRes<QueryRespVendor>>(res).QueryRes.vendors.FirstOrDefault();
                    dynamic dyn;
                    //str = JsonConvert.DeserializeObject<QueryRespACInfo>(res);
                    dyn = JsonConvert.DeserializeObject(res);
                    var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                    Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                    string jstr = JsonConvert.SerializeObject(Acc);
                    Vendor str = JsonConvert.DeserializeObject<List<Vendor>>(jstr).FirstOrDefault();

                    if (str == null)
                    {
                        result = "INVALID";
                    }
                    else
                    {
                        var vvc = new Vendor();
                        vvc.Id = str.Id;
                        vvc.SyncToken = str.SyncToken;
                        vvc.DisplayName = str.DisplayName;
                        vvc.PrintOnCheckName = vname;


                        SqlDataReader dr2;
                        DBFunctions db2 = new DBFunctions();
                        Hashtable ht2 = new Hashtable();
                        ht2.Clear();
                        ht2.Add("@Name", name);
                        ht2.Add("@Curr", curr);
                        dr2 = db2.SysFetchData("[dbo].[POMS_Getparentvendordetails]", ref ht2);
                        var service2 = new DataService(context2);
                        string vsname = "";

                        while (dr2.Read())
                        {
                            vsname = Convert.ToString(dr2["LegalName"]);
                            PhysicalAddress pa = new PhysicalAddress();
                            pa.Country = Convert.ToString(dr2["Country"]);
                            pa.City = Convert.ToString(dr2["R_City"]);
                            pa.PostalCode = Convert.ToString(dr2["R_ZipCode"]);
                            pa.CountrySubDivisionCode = Convert.ToString(dr2["State"]);
                            pa.Line1 = Convert.ToString(dr2["R_address"]);
                            vvc.BillAddr = pa;
                            TelephoneNumber tl = new TelephoneNumber();
                            tl.Default = true;
                            tl.DefaultSpecified = true;
                            tl.DeviceType = TelephoneDeviceTypeEnum.Fax.ToString();
                            tl.FreeFormNumber = Convert.ToString(dr2["Fax"]);
                            vvc.Fax = tl;
                            TelephoneNumber tl2 = new TelephoneNumber();
                            tl2.Default = true;
                            tl2.DefaultSpecified = true;
                            tl2.FreeFormNumber = Convert.ToString(dr2["Phone"]);
                            tl2.DeviceType = TelephoneDeviceTypeEnum.LandLine.ToString();
                            vvc.PrimaryPhone = tl2;
                            vvc.Notes = Convert.ToString(dr2["Note"]);
                            TelephoneNumber tl3 = new TelephoneNumber();
                            tl3.Default = true;
                            tl3.DefaultSpecified = true;
                            tl3.DeviceType = TelephoneDeviceTypeEnum.Mobile.ToString();
                            tl3.FreeFormNumber = Convert.ToString(dr2["Phone2"]);
                            vvc.AlternatePhone = tl3;
                            EmailAddress ea = new EmailAddress();
                            ea.Address = Convert.ToString(dr2["Email"]);
                            vvc.PrimaryEmailAddr = ea;

                            string weburi = Convert.ToString(dr2["Website"]);
                            if (!weburi.Contains("http://") && !weburi.Contains("https://"))
                            {
                                weburi = "http://" + Convert.ToString(dr2["Website"]);
                            }
                            if (Uri.IsWellFormedUriString(weburi, UriKind.Absolute))
                            {
                                WebSiteAddress ws = new WebSiteAddress();
                                ws.URI = weburi;
                                vvc.WebAddr = ws;
                            }
                            var service3 = new QueryService<Term>(context2);
                            //Oauth 2.0
                            query = "Select * from term where Name = '" + Convert.ToString(dr2["Terms"]) + "'";
                            // build the  request
                            encodedQuery = WebUtility.UrlEncode(query);
                            //add qbobase url and query
                            uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);
                            client = new HttpClient();

                            client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                            client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                            res = await client.GetStringAsync(uri);
                            //str = JsonConvert.DeserializeObject<QueryRespACInfo>(res);
                            dyn = JsonConvert.DeserializeObject(res);
                            Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                            Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                            jstr = JsonConvert.SerializeObject(Acc);
                            Term trms = JsonConvert.DeserializeObject<List<Term>>(jstr).FirstOrDefault();

                            if (trms == null && Convert.ToString(dr2["Terms"]) != null && Convert.ToString(dr2["Terms"]) != "")
                            {
                                Term ntrm = new Term();
                                ntrm.Name = Convert.ToString(dr2["Terms"]);
                                var obj = new object();
                                ntrm.AnyIntuitObjects = new Object[] { 10 };
                                ntrm.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DueDays };
                                ntrm = service2.Add(ntrm);
                                ReferenceType rf = new ReferenceType();
                                rf.Value = ntrm.Id;
                                vvc.TermRef = rf;
                            }
                            else if (Convert.ToString(dr2["Terms"]) != null && Convert.ToString(dr2["Terms"]) != "")
                            {
                                ReferenceType rf = new ReferenceType();
                                rf.Value = trms.Id;
                                vvc.TermRef = rf;
                            }
                        }
                        dr2.Close();
                        vvc.CompanyName = vsname;

                        vvc.PrintOnCheckName = vsname;
                        vname = vname.Replace("\\'", "'");

                        vvc.DisplayName = vname;
                        if (Convert.ToString(str.CurrencyRef.Value) == curr)
                        {
                            Vendor v = service2.Update<Vendor>(vvc) as Vendor;
                            result = v.Id.ToString();
                        }
                        else
                        {
                            //QueryService<Purchase> qr = new QueryService<Purchase>(context2);
                            //Purchase pu = qr.ExecuteIdsQuery("select * from purchase where EntityRef.Value = '" + vvc.Id + "'").FirstOrDefault();

                            TempData["TempVend"] = vvc;
                            result = "0";

                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    return "INVALID";
                }

            }
            return result;
        }

        public async Task<string> getvendorsall(string id, string name)
        {
            string Json = "";
            var vvc = new Vendor();
            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
            }
            dr.Close();
            accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
            //Oauth 2.0
            var principal = User as ClaimsPrincipal;
            //QBOAccntID = "35";

            string query = "Select * from vendor where Id='" + id + "'";
            // build the  request
            string encodedQuery = WebUtility.UrlEncode(query);
            if (key != "")
            {
                //string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                ////add qbobase url and query
                //string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                string res = "";
                try
                {
                    //var client = new HttpClient();

                    //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                    //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    //res = await client.GetStringAsync(uri);
                    ////Vendor vendors = JsonConvert.DeserializeObject<QryRes<QueryRespVendor>>(res).QueryRes.vendors.FirstOrDefault();
                    //dynamic dyn;
                    ////str = JsonConvert.DeserializeObject<QueryRespACInfo>(res);
                    //dyn = JsonConvert.DeserializeObject(res);
                    //var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                    //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                    //string jstr = JsonConvert.SerializeObject(Acc);
                    //Vendor str = JsonConvert.DeserializeObject<List<Vendor>>(jstr).FirstOrDefault();

                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                    accessToken);
                    string companyID = key;
                    ServiceContext context2 = new ServiceContext(companyID, IntuitServicesType.QBO, oauthValidator);
                    context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                    var service2 = new DataService(context2);
                    var service = new QueryService<Vendor>(context2);
                    var vendors = service.ExecuteIdsQuery(query).FirstOrDefault();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    vendors.DisplayName = name;
                    Vendor v = service2.Update<Vendor>(vendors) as Vendor;
                    return js.Serialize(v);
                }
                catch (Exception ex)
                {
                    return "INVALID";
                }
                return "INVALID";
            }
            return "INVALID";
        }


        //OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
        //        accessToken);
        //string companyID = key;
        //ServiceContext context2 = new ServiceContext(companyID, IntuitServicesType.QBO, oauthValidator);
        //context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
        //var service2 = new DataService(context2);
        //var service = new QueryService<Vendor>(context2);
        //var vendors = service.ExecuteIdsQuery("Select * from vendor where Id='" + id + "'").FirstOrDefault();
        //JavaScriptSerializer js = new JavaScriptSerializer();
        //vendors.DisplayName = name;
        //vendors = service2.Update(vendors);

        //foreach (var a in vendors)
        //{
        //    if (Json == "")
        //    {
        //        Json += js.Serialize(a);
        //    }
        //    else
        //    {
        //        Json += "," + js.Serialize(a);
        //    }
        //}
        //return js.Serialize(vendors);
        //}

        public async Task<string> getvendorsall2(string name)
        {
            string Json = "";
            var vvc = new Vendor();
            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
            }
            dr.Close();
            accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
            //Oauth 2.0
            var principal = User as ClaimsPrincipal;

            string query = "Select * from vendor where DisplayName like '%" + name + "%'";
            // build the  request
            //string encodedQuery = WebUtility.UrlEncode(query);
            if (key != "")
            {
                //string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                //add qbobase url and query
                //string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                string res = "";
                try
                {
                    //var client = new HttpClient();

                    //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                    //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    //res = await client.GetStringAsync(uri);
                    ////IList<Vendor> vendors = JsonConvert.DeserializeObject<QryRes<QueryRespVendor>>(res).QueryRes.vendors;
                    //dynamic dyn;
                    ////str = JsonConvert.DeserializeObject<QueryRespACInfo>(res);
                    //dyn = JsonConvert.DeserializeObject(res);
                    //var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                    //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                    //string jstr = JsonConvert.SerializeObject(Acc);
                    //List<Vendor> str = JsonConvert.DeserializeObject<List<Vendor>>(jstr);


                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                    accessToken);
                    string companyID = key;
                    ServiceContext context2 = new ServiceContext(companyID, IntuitServicesType.QBO, oauthValidator);
                    context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                    var service2 = new DataService(context2);
                    var service = new QueryService<Vendor>(context2);

                    var vendors = service.ExecuteIdsQuery(query).ToList();

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    //str[0].DisplayName = name;
                    //service2.UpdateAsync(str.FirstOrDefault());
                    foreach (var a in vendors)
                    {
                        if (Json == "")
                        {
                            Json += js.Serialize(a);
                        }
                        else
                        {
                            Json += "," + js.Serialize(a);
                        }
                    }
                    return Json;
                }
                catch (Exception ex)
                {
                    return "INVALID";
                }
                return "INVALID";
            }
            return "INVALID";
        }

        //OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
        //        accessToken);
        //string appToken = WebConfigurationManager.AppSettings["appToken"];
        //string companyID = key;
        //ServiceContext context2 = new ServiceContext(appToken, companyID, IntuitServicesType.QBO, oauthValidator);
        //context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
        //var service2 = new DataService(context2);
        //var service = new QueryService<Vendor>(context2);
        //var vendors = service.ExecuteIdsQuery("Select * from vendor where DisplayName like '%" + name + "%'").ToList();
        //JavaScriptSerializer js = new JavaScriptSerializer();
        //vendors.DisplayName = name;
        //vendors = service2.Update(vendors);

        //foreach (var a in vendors)
        //{
        //    if (Json == "")
        //    {
        //        Json += js.Serialize(a);
        //    }
        //    else
        //    {
        //        Json += "," + js.Serialize(a);
        //    }
        //}
        //return Json;
        //}


        public async Task<string> checkandsavevendor(string curr)
        {
            var vvc = new Vendor();
            vvc = (Vendor)TempData["TempVend"];


            string result = "";

            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
            }
            dr.Close();
            accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
            var principal = User as ClaimsPrincipal;

            string query = "Select count(*) from Purchase";
            string query1 = "Select * from Purchase MaxResults 1000";
            // build the  request
            string encodedQuery = WebUtility.UrlEncode(query);
            string encodedQuery1 = WebUtility.UrlEncode(query1);
            if (key != "")
            {
                string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                //add qbobase url and query
                string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);
                string uri1 = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery1);

                string res = ""; string res1 = "";
                try
                {
                    var client = new HttpClient();

                    client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                    client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                    //Purchase
                    res = await client.GetStringAsync(uri);
                    //Account accounts = JsonConvert.DeserializeObject<QryRes<QueryRespACInfo>>(res).QueryRes.accounts.FirstOrDefault();

                    //QueryRespACInfo str = new QueryRespACInfo();
                    dynamic dyn;
                    //str = JsonConvert.DeserializeObject<QueryRespACInfo>(res);
                    dyn = JsonConvert.DeserializeObject(res);
                    var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                    Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                    string jstr = JsonConvert.SerializeObject(Acc);
                    List<Purchase> purchases = JsonConvert.DeserializeObject<List<Purchase>>(jstr);


                    //Purchase 2
                    res1 = await client.GetStringAsync(uri1);
                    dyn = JsonConvert.DeserializeObject(res);
                    Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                    Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                    string jstr1 = JsonConvert.SerializeObject(Acc);
                    List<Purchase> purchases2 = JsonConvert.DeserializeObject<List<Purchase>>(jstr);
                    int a = 0;

                    int pcount = purchases.Count;
                    if (pcount < 1000)
                    {
                        for (a = 0; a < pcount; a++)
                        {
                            if (purchases2[a].EntityRef != null)
                            {
                                if (vvc.Id.ToString() == purchases2[a].EntityRef.Value)
                                {
                                    result = "0";
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {

                        for (a = 1; a <= pcount; a = a + 1000)
                        {
                            string p2 = "Select * from Purchase STARTPOSITION " + a + " MAXRESULTS 1000";
                            encodedQuery = WebUtility.UrlEncode(p2);
                            //add qbobase url and query
                            uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);
                            res = await client.GetStringAsync(uri);
                            res1 = await client.GetStringAsync(uri1);
                            dyn = JsonConvert.DeserializeObject(res);
                            Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                            Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                            jstr1 = JsonConvert.SerializeObject(Acc);
                            purchases2 = JsonConvert.DeserializeObject<List<Purchase>>(jstr);

                            if (p2 != null && p2.Count() > 0)
                            {
                                foreach (var objAccount in purchases2)
                                {
                                    if (objAccount.EntityRef != null)
                                    {
                                        if (vvc.Id.ToString() == objAccount.EntityRef.Value)
                                        {
                                            result = "0";
                                            break;
                                        }
                                    }
                                }
                            }
                        }


                    }

                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                    accessToken);
                    string appToken = WebConfigurationManager.AppSettings["appToken"];
                    string realmId = key;
                    ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                    context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                    var service2 = new DataService(context2);
                    var service = new QueryService<Purchase>(context2);

                    if (result == "0")
                        return "0";
                    //var purchase =            
                    Vendor vvc2 = new Vendor();
                    vvc2 = vvc;
                    //vvc = service2.Update(vvc);
                    vvc.Active = false;
                    vvc.ActiveSpecified = true;
                    Vendor vnd = service2.Update<Vendor>(vvc) as Vendor;
                    vvc = vnd;

                    vvc2.CurrencyRef = new ReferenceType();
                    vvc2.CurrencyRef.Value = curr;

                    vvc2.Active = true;
                    vvc2.ActiveSpecified = true;

                    vvc2.Id = null;

                    Vendor v = service2.Add<Vendor>(vvc2) as Vendor;
                    // vvc2 = service2.Update(vvc2);
                    vvc2.Id = v.Id.ToString();
                    return result == "" ? vvc2.Id.ToString() : result;
                }
                catch (Exception ex)
                {
                    return "INVALID";
                }

            }
            return result;

        }

        public async Task<string> saveVendor(string name, string curr, string vname)
        {
            name = HttpUtility.HtmlDecode(name);
            vname = HttpUtility.HtmlDecode(vname);
            string accessToken = "";
            string accessTokenSecret = "";
            string key = "";
            string Refresh_Token = "";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@Mode", "GET");
            ht.Add("@Key", key);
            dr = db.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht);
            while (dr.Read())
            {
                accessToken = Convert.ToString(dr["accessToken"]);
                accessTokenSecret = Convert.ToString(dr["accessTokenSecret"]);
                key = Convert.ToString(dr["Key"]);
                Refresh_Token = Convert.ToString(dr["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr["TokenEndpoint"]);
            }
            dr.Close();
            accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                    accessToken);
            //string appToken = WebConfigurationManager.AppSettings["appToken"];
            string realmId = key;
            ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            DataService service2 = new DataService(context2);
            string address = "";
            Vendor vvc = new Vendor();
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Name", name);
            ht2.Add("@Curr", curr);
            dr2 = db2.SysFetchData("[dbo].[POMS_Getparentvendordetails]", ref ht2);
            string vsname = "";
            while (dr2.Read())
            {
                vsname = Convert.ToString(dr2["LegalName"]);
                PhysicalAddress pa = new PhysicalAddress();
                pa.Country = Convert.ToString(dr2["Country"]);
                pa.City = Convert.ToString(dr2["R_City"]);
                pa.PostalCode = Convert.ToString(dr2["R_ZipCode"]);
                pa.CountrySubDivisionCode = Convert.ToString(dr2["State"]);
                pa.Line1 = Convert.ToString(dr2["R_address"]);
                vvc.BillAddr = pa;
                TelephoneNumber tl = new TelephoneNumber();
                tl.Default = true;
                tl.DefaultSpecified = true;
                tl.DeviceType = TelephoneDeviceTypeEnum.Fax.ToString();
                tl.FreeFormNumber = Convert.ToString(dr2["Fax"]);
                vvc.Fax = tl;
                TelephoneNumber tl2 = new TelephoneNumber();
                tl2.Default = true;
                tl2.DefaultSpecified = true;
                tl2.FreeFormNumber = Convert.ToString(dr2["Phone"]);
                tl2.DeviceType = TelephoneDeviceTypeEnum.LandLine.ToString();
                vvc.PrimaryPhone = tl2;
                EmailAddress ea = new EmailAddress();
                ea.Address = Convert.ToString(dr2["Email"]);
                vvc.PrimaryEmailAddr = ea;
                vvc.Notes = Convert.ToString(dr2["Note"]);
                TelephoneNumber tl3 = new TelephoneNumber();
                tl3.Default = true;
                tl3.DefaultSpecified = true;
                tl3.DeviceType = TelephoneDeviceTypeEnum.Mobile.ToString();
                tl3.FreeFormNumber = Convert.ToString(dr2["Phone2"]);
                vvc.AlternatePhone = tl3;

                string weburi = Convert.ToString(dr2["Website"]);
                if (!weburi.Contains("http://") && !weburi.Contains("https://"))
                {
                    weburi = "http://" + Convert.ToString(dr2["Website"]);
                }
                if (Uri.IsWellFormedUriString(weburi, UriKind.Absolute))
                {
                    WebSiteAddress ws = new WebSiteAddress();
                    ws.URI = weburi;
                    vvc.WebAddr = ws;
                }
                //Oauth 2.0
                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                var principal = User as ClaimsPrincipal;

                string query = "Select * from term where Name='" + Convert.ToString(dr2["Terms"]) + "'";
                string encodedQuery = WebUtility.UrlEncode(query);
                if (key != "")
                {
                    //string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                    ////add qbobase url and query
                    //string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                    string res = "";
                    try
                    {
                        //var client = new HttpClient();

                        //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                        //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                        //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                        ////Purchase
                        //res = await client.GetStringAsync(uri);
                        //dynamic dyn;
                        //dyn = JsonConvert.DeserializeObject(res);
                        //var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                        //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                        //string jstr = JsonConvert.SerializeObject(Acc);
                        //Term trms = JsonConvert.DeserializeObject<List<Term>>(jstr).FirstOrDefault();

                        QueryService<Term> qp = new QueryService<Term>(context2);
                        Term trms = qp.ExecuteIdsQuery(query).FirstOrDefault<Term>();


                        if (trms == null && Convert.ToString(dr2["Terms"]) != "")
                        {
                            Term ntrm = new Term();
                            ntrm.Name = Convert.ToString(dr2["Terms"]);
                            var obj = new object();
                            ntrm.AnyIntuitObjects = new Object[] { 10 };
                            ntrm.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DueDays };
                            Term t = service2.Add<Term>(ntrm) as Term;
                            ReferenceType rf = new ReferenceType();
                            rf.Value = t.Id;
                            vvc.TermRef = rf;
                        }
                        else if (Convert.ToString(dr2["Terms"]) != "")
                        {
                            ReferenceType rf = new ReferenceType();
                            rf.Value = trms.Id;
                            vvc.TermRef = rf;
                        }




                    }
                    catch (Exception ex)
                    {
                        return "INVALID";
                    }

                }


            }
            dr2.Close();
            vvc.CompanyName = vsname;
            vvc.PrintOnCheckName = vsname;
            vvc.DisplayName = vname;
            vvc.CurrencyRef = new ReferenceType();
            vvc.CurrencyRef.Value = curr;
            try
            {
                //context2.IppConfiguration.MinorVersion.Qbo = "23";
                //service2 = new DataService(context2);
                Vendor v = service2.Add<Vendor>(vvc) as Vendor;
                vvc.Id = v.Id;
            }
            catch (IdsException e)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(e.InnerException.Message.ToString() + " is the error ", EventLogEntryType.Information, 107, 1);
                }
            }
            return vvc.Id;
        }

        public async Task<ActionResult> Index()
        {
            if (Request.QueryString.Count > 0)
            {
                List<string> queryKeys = new List<string>(Request.QueryString.AllKeys);
                if (queryKeys.Contains("connect"))
                {//Intialize DiscoverPolicy Oauth 2.0
                    DiscoveryPolicy dpolicy = new DiscoveryPolicy();
                    dpolicy.RequireHttps = true;
                    dpolicy.ValidateIssuerName = true;
                    string discoveryUrl = ConfigurationManager.AppSettings["DiscoveryUrl"];

                    if (discoveryUrl != null && consumerKey != null && consumerSecret != null)
                    {
                        discoveryClient = new DiscoveryClient(discoveryUrl);
                    }
                    else
                    {
                        Exception ex = new Exception("Discovery Url missing!");
                        throw ex;
                    }
                    doc = await discoveryClient.GetAsync();
                    if (doc.StatusCode == HttpStatusCode.OK)
                    {
                        //Authorize endpoint
                        authorizeUrl = doc.AuthorizeEndpoint;

                        //Token endpoint
                        tokenEndpoint = doc.TokenEndpoint;

                        //Token Revocation enpoint
                        revocationEndpoint = doc.RevocationEndpoint;

                        //UserInfo endpoint
                        userinfoEndpoint = doc.UserInfoEndpoint;

                        //Issuer endpoint
                        issuerEndpoint = doc.Issuer;

                        //JWKS Keys
                        keys = doc.KeySet.Keys;
                    }
                    //Get mod and exponent value
                    foreach (var key in keys)
                    {
                        if (key.N != null)
                        {
                            //Mod
                            mod = key.N;
                        }
                        if (key.E != null)
                        {
                            //Exponent
                            expo = key.E;
                        }
                    }
                    scope = OidcScopes.OpenId.GetStringValue() + " " + OidcScopes.Address.GetStringValue()
                         + " " + OidcScopes.Email.GetStringValue() + " " + OidcScopes.Phone.GetStringValue()
                         + " " + OidcScopes.Profile.GetStringValue() + " " + OidcScopes.Accounting.GetStringValue();
                    //+" "+ OidcScopes.Payment.GetStringValue();
                    authorizeUrl = GetAuthorizeUrl(scope);
                    // perform the redirect here.
                    return Redirect(authorizeUrl);
                    FireAuthAsync();
                }
                //if (queryKeys.Contains("oauth_token"))
                if (queryKeys.Contains("code"))
                {
                    var tokenClient = new TokenClient(tokenEndpoint, consumerKey, consumerSecret);


                    var code = Request.QueryString["code"];
                    var realmId = Request.QueryString["realmId"];
                    if (realmId != null)
                    {
                        Session["realmId"] = realmId;
                    }
                    //var tempState = await GetTempStateAsync();
                    //Request.GetOwinContext().Authentication.SignOut("TempState");

                    TokenResponse response = await tokenClient.RequestTokenFromCodeAsync(code, oauth_callback_url);

                    await ValidateResponseAndSignInAsync(response);

                    if (!string.IsNullOrEmpty(response.IdentityToken))
                    {
                        Session["accessTokenSecret"] = response.IdentityToken;
                    }
                    if (!string.IsNullOrEmpty(response.AccessToken))
                    {
                        Session["accessToken"] = response.AccessToken;
                    }
                    //Oauth 1.0 
                    //ReadTokenAsync();

                    string key = Session["realmId"].ToString();
                    string accessToken = Session["accessToken"].ToString();
                    string accessTokenSecret = Session["accessTokenSecret"].ToString();
                }
            }
            else
            {
                if (Session["accessToken"] == null && Session["accessTokenSecret"] == null)
                {
                }
                else
                {
                    //Disconnect();
                }
            }
            return View();
        }

        private void Disconnect()
        {
            try
            {
                Session.Clear();
                Session.Abandon();
                Session["accessToken"] = null;
                Session["accessTokenSecret"] = null;
                Session["realm"] = null;
                Session["dataSource"] = null;
            }
            catch (Exception ex)
            {
                Response.Write(ex.InnerException);
            }
        }

        protected void CreateAuthorization()
        {
            Session["consumerKey"] = consumerKey;
            Session["consumerSecret"] = consumerSecret;
            Session["oauthLink"] = OAUTH_URL;
            OAuth2Client auth2Client = new OAuth2Client(consumerKey, consumerSecret, oauth_callback_url, env);
            List<OidcScopes> scopes = new List<OidcScopes>();
            scopes.Add(OidcScopes.Accounting);
            string authorizeUrl = auth2Client.GetAuthorizationURL(scopes);

            //IOAuthSession session = CreateSession();
            //IToken requestToken = session.GetRequestToken();
            //Session["requestToken"] = requestToken;
            //tokenSecret = requestToken.TokenSecret;
            //var authUrl = string.Format("{0}?oauth_token={1}&oauth_callback={2}", AUTHORIZE_URL, requestToken.Token, UriUtility.UrlEncode(oauth_callback_url));
            //OAUTH_URL = string.Format("{0}?oauth_token={1}&oauth_callback={2}", AUTHORIZE_URL, requestToken.Token, UriUtility.UrlEncode(oauth_callback_url));
            Session["oauthLink"] = authorizeUrl;
            Response.Redirect(authorizeUrl);
        }


        private void getAccessToken()
        {
            IOAuthSession clientSession = CreateSession();
            IToken accessToken = clientSession.ExchangeRequestTokenForAccessToken((IToken)Session["requestToken"], Session["oauthVerifyer"].ToString());
            Session["accessToken"] = accessToken.Token;
            Session["accessTokenSecret"] = accessToken.TokenSecret;
        }

        protected IOAuthSession CreateSession()
        {
            var consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1
            };
            return new OAuthSession(consumerContext,
                                    REQUEST_TOKEN_URL,
                                    OAUTH_URL.ToString(),
                                    ACCESS_TOKEN_URL);
        }

        private async System.Threading.Tasks.Task ReadTokenAsync()
        {
            //Session["oauthToken"] = Request.QueryString["oauth_token"].ToString(); ;
            //Session["oauthVerifyer"] = Request.QueryString["oauth_verifier"].ToString();
            Session["realm"] = Request.QueryString["realmId"].ToString();
            //Session["dataSource"] = Request.QueryString["dataSource"].ToString();
            Session["realmId"] = Request.QueryString["realmId"].ToString();
            Session["code"] = Request.QueryString["code"].ToString();
            /*Stored in a session for demo purposes.*/
            /*Production applications should securely store the Access Token*/
            getAccessToken();
            //await GetAuthTokensAsync(Request.QueryString["code"].ToString(), Request.QueryString["realmId"].ToString());
        }

        private void FireAuthAsync()
        {

            //Oauth 1.0
            Session["consumerKey"] = consumerKey;
            Session["consumerSecret"] = consumerSecret;
            CreateAuthorization();
            //IToken token = (IToken)Session["requestToken"];
            //tokenSecret = token.TokenSecret;
            //strrequestToken = token.Token;
        }
        //Oauth 2.0 02-12-2019
        private string GetAuthorizeUrl(string scope)
        {
            var state = Guid.NewGuid().ToString("N");

            SetTempState(state);

            //Make Authorization request
            var request = new AuthorizeRequest(authorizeUrl);

            string url = request.CreateAuthorizeUrl(
               clientId: consumerKey,
               responseType: OidcConstants.AuthorizeResponse.Code,
               scope: scope,
               redirectUri: oauth_callback_url,
               state: state);

            return url;
        }
        private void SetTempState(string state)
        {
            var tempId = new ClaimsIdentity("TempState");
            tempId.AddClaim(new Claim("state", state));

            Request.GetOwinContext().Authentication.SignIn(tempId);
        }

        static string GetDevDefinedOAuthHeader(HttpWebRequest webRequest, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            OAuthConsumerContext consumerContext = new OAuthConsumerContext
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
                SignatureMethod = SignatureMethod.HmacSha1,
                UseHeaderForOAuthParameters = true
            };
            consumerContext.UseHeaderForOAuthParameters = true;
            /*URIs not used - we already have Oauth tokens*/
            OAuthSession oSession = new OAuthSession(consumerContext, "https://www.example.com",
                                    "https://www.example.com",
                                    "https://www.example.com");
            oSession.AccessToken = new TokenBase
            {
                Token = accessToken,
                ConsumerKey = consumerKey,
                TokenSecret = accessTokenSecret
            };
            IConsumerRequest consumerRequest = oSession.Request();
            consumerRequest = ConsumerRequestExtensions.ForMethod(consumerRequest, webRequest.Method);
            consumerRequest = ConsumerRequestExtensions.ForUri(consumerRequest, webRequest.RequestUri);
            consumerRequest = consumerRequest.SignWithToken();
            return consumerRequest.Context.GenerateOAuthParametersForHeader();
        }

        public string updateqbosentdata(string payid, string cnumber, string printstatus)
        {
            string res = "";
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "UPDATE");
            ht2.Add("@PaymentID", payid);
            ht2.Add("@Checknumber", cnumber);
            ht2.Add("@PrintStatus", printstatus);

            dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
            while (dr2.Read())
            {
            }
            dr2.Close();

            return res;
        }

        public string deleteqbosentdata(string payid, string cnumber, string printstatus)
        {
            string res = "";
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "DELETE");
            ht2.Add("@PaymentID", payid);
            ht2.Add("@Checknumber", cnumber);
            ht2.Add("@PrintStatus", printstatus);

            dr2 = db2.SysFetchData("[dbo].[POMS_Updatepaymentforqbosent]", ref ht2);
            while (dr2.Read())
            {
            }
            dr2.Close();

            return res;
        }

        public void getCheckDetailsall()
        {
            Allpaymentnumbersync();
            return;
            string res = "";
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@PaymentID", "");
            dr2 = db2.SysFetchData("[dbo].[POMS_GetQBODataAll]", ref ht2);
            while (dr2.Read())
            {
                decimal amt = Convert.ToDecimal(dr2["Amount"]);
                if (amt > 0)
                {
                    getCheckDetails(Convert.ToString(dr2["PaymentID"]));
                }
                else
                {
                    getCheckDetailsJ(Convert.ToString(dr2["PaymentID"]));
                }
            }
            dr2.Close();
        }

        public async Task<string> getCheckDetailsJ(string paymentid)
        {
            string result = "";
            List<string> qboid = new List<string>();
            List<string> synctoken = new List<string>();
            string jsnstr = "{\"data\":[";
            SqlDataReader dr;
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            ht.Clear();
            ht.Add("@PaymentID", paymentid);
            dr = db.SysFetchData("[dbo].[POMS_GetQBOData]", ref ht);
            int i = 0;
            while (dr.Read())
            {
                qboid.Add(Convert.ToString(dr["QBOID"]));
                synctoken.Add(Convert.ToString(dr["SyncToken"]));
            }
            dr.Close();
            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "GET");
            ht2.Add("@Key", key);
            dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
            while (dr2.Read())
            {
                accessToken = Convert.ToString(dr2["accessToken"]);
                accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                key = Convert.ToString(dr2["Key"]);
                Refresh_Token = Convert.ToString(dr2["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr2["TokenEndpoint"]);
            }
            dr2.Close();
            //Oauth 2.0
            var principal = User as ClaimsPrincipal;

            string json = "";
            int indx = 0;
            foreach (string a in qboid)
            {
                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                string query = "select * from Deposit where id='" + a + "'";
                // build the  request
                //string encodedQuery = WebUtility.UrlEncode(query);
                //string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                ////add qbobase url and query
                //string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                string res = "";
                //var client = new HttpClient();

                //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                //res = await client.GetStringAsync(uri);
                //dynamic dyn;
                //dyn = JsonConvert.DeserializeObject(res);
                //var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                //string jstr = JsonConvert.SerializeObject(Acc);
                //Deposit prc = JsonConvert.DeserializeObject<List<Deposit>>(jstr).FirstOrDefault();
                OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                        accessToken);
                string appToken = WebConfigurationManager.AppSettings["appToken"];
                string companyID = key;
                ServiceContext context2 = new ServiceContext(companyID, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                QueryService<Deposit> qp = new QueryService<Deposit>(context2);
                Deposit prc = qp.ExecuteIdsQuery(query).FirstOrDefault<Deposit>();
                //Deposit prc = qp.ExecuteIdsQuery("select * from Deposit where id='" + a + "'").FirstOrDefault();
                if (prc == null)
                {
                    deleteqbosentdata(paymentid, "", "");
                    json = "{\"QBOID\" : \""
                     + ""
                     + "\","
                     + "\"Total\" : \""
                     + ""
                     + "\","
                     + "\"SyncToken\" : \""
                     + ""
                     + "\","
                     + "\"Status\" : \""
                     + ""
                     + "\","
                     + "\"SystemToken\" : \""
                     + ""
                     + "\","
                     + "\"QboToken\" : \""
                     + ""
                     + "\","
                     + "\"Vendor\" : \""
                     + ""
                     + "\","
                     + "\"Checknumber\" : \""
                     + ""
                     + "\""
                     + "}";
                    if (jsnstr == "{\"data\":[")
                    { jsnstr += json; }
                    else
                    { jsnstr += "," + json; }

                    updateqbosentdata(paymentid, "", "");

                    continue;
                }
                string status = "Active";
                if (prc.statusSpecified)
                {
                    status = prc.status.ToString();
                }
                prc.status.ToString();
                string synctokecn = prc.SyncToken;
                string Checknumber = "";
                //if (prc.DocNumber == null)
                if (!prc.PrivateNote.ToString().Contains('|'))
                {
                    if (prc.PrivateNote.ToString().Trim() == "")
                    {
                        Checknumber = "N/A";
                    }
                    else
                    {
                        Checknumber = prc.PrivateNote.ToString();
                    }
                }
                else
                {
                    //Checknumber = prc.DocNumber.ToString();
                    Checknumber = prc.PrivateNote.ToString().Split('|')[0];
                }
                updateqbosentdata(paymentid, Checknumber, Convert.ToString(""));
                json = "{\"QBOID\" : \""
                      + prc.Id.ToString()
                      + "\","
                      + "\"Total\" : \""
                      + prc.TotalAmt.ToString()
                      + "\","
                      + "\"SyncToken\" : \""
                      + prc.SyncToken.ToString()
                      + "\","
                      + "\"Status\" : \""
                      + status
                      + "\","
                      + "\"SystemToken\" : \""
                      + synctoken[indx++]
                      + "\","
                      + "\"QboToken\" : \""
                      + prc.SyncToken.ToString()
                      + "\","
                      + "\"Vendor\" : \""
                      + ""
                      + "\","
                      + "\"Checknumber\" : \""
                      + Checknumber
                      + "\""
                      + "}";
                if (jsnstr == "{\"data\":[")
                { jsnstr += json; }
                else
                { jsnstr += "," + json; }
            }
            return jsnstr + "]}";
        }

        public async Task<string> getCheckDetails(string paymentid)
        {
            // Allpaymentnumbersync();

            try
            {
                string result = "";
                List<string> qboid = new List<string>();
                List<string> synctoken = new List<string>();
                string jsnstr = "{\"data\":[";
                SqlDataReader dr;
                DBFunctions db = new DBFunctions();
                Hashtable ht = new Hashtable();

                ht.Clear();
                ht.Add("@PaymentID", paymentid);
                dr = db.SysFetchData("[dbo].[POMS_GetQBOData]", ref ht);
                int i = 0;
                while (dr.Read())
                {
                    qboid.Add(Convert.ToString(dr["QBOID"]));
                    synctoken.Add(Convert.ToString(dr["SyncToken"]));
                }
                dr.Close();
                string accessToken = "";
                string accessTokenSecret = "";
                string Refresh_Token = "";
                string key = WebConfigurationManager.AppSettings["companyID"];
                SqlDataReader dr2;
                DBFunctions db2 = new DBFunctions();
                Hashtable ht2 = new Hashtable();
                ht2.Clear();
                ht2.Add("@Mode", "GET");
                ht2.Add("@Key", key);
                dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
                while (dr2.Read())
                {
                    accessToken = Convert.ToString(dr2["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                    key = Convert.ToString(dr2["Key"]);
                    Refresh_Token = Convert.ToString(dr2["RefreshToken"]);
                    tokenEndpoint = Convert.ToString(dr2["TokenEndpoint"]);
                }
                dr2.Close();
                //Oauth 2.0
                var principal = User as ClaimsPrincipal;

                string json = "";
                int indx = 0;
                foreach (string a in qboid)
                {
                    accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
                    string query = "select * from Purchase where id='" + a + "'";
                    // build the  request
                    //string encodedQuery = WebUtility.UrlEncode(query);
                    //string qboBaseUrl = ConfigurationManager.AppSettings["QBOBaseUrl"];
                    ////add qbobase url and query
                    //string uri = string.Format("{0}/v3/company/{1}/query?query={2}", qboBaseUrl, key, encodedQuery);

                    string res = "";
                    try
                    {
                        var client = new HttpClient();


                        OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                        accessToken);
                        string appToken = WebConfigurationManager.AppSettings["appToken"];
                        string companyID = key;
                        ServiceContext context2 = new ServiceContext(companyID, IntuitServicesType.QBO, oauthValidator);
                        context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                        QueryService<Purchase> qp = new QueryService<Purchase>(context2);
                        Purchase prc = qp.ExecuteIdsQuery(query).FirstOrDefault<Purchase>();

                        if (prc == null)
                        {
                            //deleteqbosentdata(paymentid, "", "");
                            json = "{\"QBOID\" : \""
                             + ""
                             + "\","
                             + "\"Total\" : \""
                             + ""
                             + "\","
                             + "\"SyncToken\" : \""
                             + ""
                             + "\","
                             + "\"Status\" : \""
                             + ""
                             + "\","
                             + "\"SystemToken\" : \""
                             + ""
                             + "\","
                             + "\"QboToken\" : \""
                             + ""
                             + "\","
                             + "\"Vendor\" : \""
                             + ""
                             + "\","
                             + "\"Checknumber\" : \""
                             + ""
                             + "\""
                             + "}";
                            if (jsnstr == "{\"data\":[")
                            { jsnstr += json; }
                            else
                            { jsnstr += "," + json; }
                            updateqbosentdata(paymentid, "", "");

                            continue;
                        }

                        string status = "Active";
                        if (prc.statusSpecified)
                        {
                            status = prc.status.ToString();
                        }
                        prc.status.ToString();
                        string synctokecn = prc.SyncToken;
                        query = "select * from Vendor where id='" + prc.EntityRef.Value.ToString() + "'";
                        // build the  request
                        QueryService<Vendor> qp1 = new QueryService<Vendor>(context2);
                        Vendor vnd = qp1.ExecuteIdsQuery(query).FirstOrDefault<Vendor>();

                        string Checknumber = "";
                        if (prc.DocNumber == null)
                        {
                            Checknumber = "N/A";
                        }
                        else
                        {
                            Checknumber = prc.DocNumber.ToString();
                        }
                        updateqbosentdata(paymentid, Checknumber, Convert.ToString(prc.PrintStatus));
                        json = "{\"QBOID\" : \""
                              + prc.Id.ToString()
                              + "\","
                              + "\"Total\" : \""
                              + prc.TotalAmt.ToString()
                              + "\","
                              + "\"SyncToken\" : \""
                              + prc.SyncToken.ToString()
                              + "\","
                              + "\"Status\" : \""
                              + status
                              + "\","
                              + "\"SystemToken\" : \""
                              + synctoken[indx++]
                              + "\","
                              + "\"QboToken\" : \""
                              + prc.SyncToken.ToString()
                              + "\","
                              + "\"Vendor\" : \""
                              + vnd.DisplayName.ToString()
                              + "\","
                              + "\"Checknumber\" : \""
                              + Checknumber
                              + "\""
                              + "}";
                        if (jsnstr == "{\"data\":[")
                        { jsnstr += json; }
                        else
                        { jsnstr += "," + json; }
                    }
                    catch (Exception ex)
                    {
                        return "INVALID";
                    }

                    //Purchase prc = qp.ExecuteIdsQuery("select * from Purchase where id='" + a + "'").FirstOrDefault();


                }
                return jsnstr + "]}";
            }
            catch (Exception e)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";

                    eventLog.WriteEntry(e.Message.ToString() + " is the error ", EventLogEntryType.Information, 105, 1);
                }
                return "";
            }

            //await Task.Delay(10000);
        }

        public async Task<string> changepayment(string paymentid, string status)
        {
            string result = "";
            try
            {
                string accessToken = "";
                string accessTokenSecret = "";
                string Refresh_Token = "";
                string key = WebConfigurationManager.AppSettings["companyID"];
                SqlDataReader dr2;
                DBFunctions db2 = new DBFunctions();
                Hashtable ht2 = new Hashtable();
                ht2.Clear();
                ht2.Add("@Mode", "GET");
                ht2.Add("@Key", "");
                dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
                while (dr2.Read())
                {
                    accessToken = Convert.ToString(dr2["accessToken"]);
                    accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                    key = Convert.ToString(dr2["Key"]);
                    Refresh_Token = Convert.ToString(dr2["RefreshToken"]);
                    tokenEndpoint = Convert.ToString(dr2["TokenEndpoint"]);
                }
                dr2.Close();

                accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);

                OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                        accessToken);
                string realmId = key;
                ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
                context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
                QueryService<Purchase> qp = new QueryService<Purchase>(context2);


                var principal = User as ClaimsPrincipal;
                string query = "select * from Purchase where id='" + paymentid + "'";
                string res = "";
                try
                {
                    Purchase prc = qp.ExecuteIdsQuery(query).FirstOrDefault();
                    if (prc != null)
                    {
                        prc.TotalAmt = 0;
                        prc.TotalAmtSpecified = true;
                        for (int a = 0; a < prc.Line.Length; a++)
                        {
                            prc.Line[a].Amount = 0;
                            prc.Line[a].AmountSpecified = true;
                        }
                        DataService ds = new DataService(context2);
                        Purchase p = ds.Update<Purchase>(prc) as Purchase;
                        result = "Success";

                    }
                    return result;
                }
                catch (Exception ex)
                {
                    return "INVALID";
                }


            }
            catch (Exception ex)
            {
                return "INVALID";
            }
        }

        public async Task<string> deletepayment(string paymentid, string mode)
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string Refresh_Token = "";
            string key = WebConfigurationManager.AppSettings["companyID"];
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "GET");
            ht2.Add("@Key", key);
            dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
            while (dr2.Read())
            {
                accessToken = Convert.ToString(dr2["accessToken"]);
                accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                key = Convert.ToString(dr2["Key"]);
                Refresh_Token = Convert.ToString(dr2["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr2["TokenEndpoint"]);
            }
            dr2.Close();
            accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                    accessToken);
            string realmId = key;
            ServiceContext context2 = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            QueryService<Purchase> qp = new QueryService<Purchase>(context2);
            string json = "";
            int indx = 0;

            var principal = User as ClaimsPrincipal;
            string query = "select * from Purchase where id='" + paymentid + "'";
            string res = "";
            try
            {
                Purchase prc = qp.ExecuteIdsQuery(query).FirstOrDefault();
                if (prc != null)
                {
                    prc.TotalAmt = 0;
                    prc.TotalAmtSpecified = true;
                    for (int a = 0; a < prc.Line.Length; a++)
                    {
                        prc.Line[a].Amount = 0;
                        prc.Line[a].AmountSpecified = true;
                    }
                    DataService ds = new DataService(context2);
                    if (mode == "GET")
                    {
                        if (prc.PrintStatus == PrintStatusEnum.NotSet)
                        {
                            result = "Success";
                        }
                        else
                        {
                            result = "Failed";
                        }
                    }
                    else
                    {
                        if (prc.PrintStatus == PrintStatusEnum.NotSet)
                        {
                            ds.Delete<Purchase>(prc);
                            result = "Success";
                        }
                        else
                        {
                            result = "Failed";
                        }
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                return "INVALID";
            }


            return result;
        }

        public async Task<string> updatepayment(string paymentid)
        {
            string result = "";
            string accessToken = "";
            string accessTokenSecret = "";
            string key = "";
            string Refresh_Token = "";
            SqlDataReader dr2;
            DBFunctions db2 = new DBFunctions();
            Hashtable ht2 = new Hashtable();
            ht2.Clear();
            ht2.Add("@Mode", "GET");
            ht2.Add("@Key", key);
            dr2 = db2.SysFetchData("[dbo].[POMS_QBSecurityProc]", ref ht2);
            while (dr2.Read())
            {
                accessToken = Convert.ToString(dr2["accessToken"]);
                accessTokenSecret = Convert.ToString(dr2["accessTokenSecret"]);
                key = Convert.ToString(dr2["Key"]);
                Refresh_Token = Convert.ToString(dr2["RefreshToken"]);
                tokenEndpoint = Convert.ToString(dr2["TokenEndpoint"]);
            }
            dr2.Close();
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(
                    accessToken);
            string companyID = key;
            ServiceContext context2 = new ServiceContext(companyID, IntuitServicesType.QBO, oauthValidator);
            context2.IppConfiguration.BaseUrl.Qbo = BaseUrlQBO;
            QueryService<Purchase> qp = new QueryService<Purchase>(context2);
            string json = "";
            int indx = 0;

            accessToken = await RefreshToken(accessTokenSecret, key, Refresh_Token);
            var principal = User as ClaimsPrincipal;
            string query = "select * from Purchase where id='" + paymentid + "'";
            // build the  request

            string res = "";
            try
            {
                //var client = new HttpClient();

                //client.DefaultRequestHeaders.Add("Accept", "application/json;charset=UTF-8");
                //client.DefaultRequestHeaders.Add("ContentType", "application/json;charset=UTF-8");
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                //res = await client.GetStringAsync(uri);
                //dynamic dyn;
                ////str = JsonConvert.DeserializeObject<QueryRespACInfo>(res);
                //dyn = JsonConvert.DeserializeObject(res);
                //var Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)dyn).First).First;
                //Acc = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)Acc).First).First;
                //string jstr = JsonConvert.SerializeObject(Acc);
                //Purchase prc = JsonConvert.DeserializeObject<List<Purchase>>(jstr).FirstOrDefault();

                Purchase prc = qp.ExecuteIdsQuery(query).FirstOrDefault<Purchase>();
                if (prc == null)
                {
                    prc.TotalAmt = 0;
                    prc.TotalAmtSpecified = true;
                    for (int a = 0; a < prc.Line.Length; a++)
                    {
                        prc.Line[a].Amount = 0;
                        prc.Line[a].AmountSpecified = true;
                    }
                    DataService ds = new DataService(context2);
                    Purchase p = ds.Update<Purchase>(prc) as Purchase;
                    result = "Success";

                }
                return result;
            }
            catch (Exception ex)
            {
                return "INVALID";
            }



            return result;
        }

        public string setupdateid(string id)
        {
            Session["UpdateQBOID"] = id;
            return "";
        }

    }
}
