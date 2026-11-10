using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Web.Mvc;

namespace POMS.Controllers
{
    public class UploadFileController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void Upload(string projectid, string mode)
        {
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            var fileName = "";
            string fname = "";
            string allfilename = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                string ext = "";
                if (mode == "TC") 
                {
                    var file = Request.Files[i];
                    ext = Path.GetFileName(file.FileName).Split('.')[1];
                    fname = Path.GetFileName(file.FileName).Split('.')[0];
                    fname = fname.Replace("'", "`");
                    fileName = fname + "(" + mode + "_" + projectid + "_" + i + ")" + "." + ext;
                    if (allfilename.Trim() == "")
                    {
                        allfilename = fileName;
                    }
                    else
                    {
                        allfilename += "," + fileName;
                    }
                    var path = Path.Combine(Server.MapPath("~/Assets/TermsConditionFiles"), fileName);
                    file.SaveAs(path);
                }
                else if (mode == "PF")
                {
                    var filePF = Request.Files[i];
                    ext = Path.GetFileName(filePF.FileName).Split('.')[1];
                    fname = Path.GetFileName(filePF.FileName).Split('.')[0];
                    fname = fname.Replace("'", "`");
                    fileName = fname + "(" + mode + "_" + projectid + "_" + i + ")" + "." + ext;
                    if (allfilename.Trim() == "")
                    {
                        allfilename = fileName;
                    }
                    else
                    {
                        allfilename += "," + fileName;
                    }
                    var path = Path.Combine(Server.MapPath("~/Assets/ProjectFiles"), fileName);
                    filePF.SaveAs(path);
                }
                else if (mode == "ISF")
                {
                    var fileISF = Request.Files[i];
                    ext = Path.GetFileName(fileISF.FileName).Split('.')[1];
                    fname = Path.GetFileName(fileISF.FileName).Split('.')[0];
                    fname = fname.Replace("'", "`");
                    fileName = fname + "(" + mode + "_" + projectid + "_" + i + ")" + "." + ext;
                    if (allfilename.Trim() == "")
                    {
                        allfilename = fileName;
                    }
                    else
                    {
                        allfilename += "," + fileName;
                    }
                    var path = Path.Combine(Server.MapPath("~/Assets/InstallationScheduleFiles"), fileName);
                    fileISF.SaveAs(path);
                }                
            }

            if (mode == "TC")
            {
                string[] files = System.IO.Directory.GetFiles(Server.MapPath("~/Assets/SystemTermsFiles"));
                if (files.Length > 0)
                {
                    string ext = files[0].Split('.')[1];
                    fname = files[0].Split('.')[0];
                    fname = fname.Replace("'", "`");
                    fileName = fname.Split('^')[1] + "(" + mode + "_" + projectid + "_" + 0 + ")" + "." + ext;
                    if (allfilename.Trim() == "")
                    {
                        allfilename = fileName;
                    }
                    else
                    {
                        allfilename += "," + fileName;
                    }
                    var path = Path.Combine(Server.MapPath("~/Assets/TermsConditionFiles"), fileName);
                    foreach (var file in Directory.GetFiles(Server.MapPath("~/Assets/SystemTermsFiles")))
                    {
                        System.IO.File.Copy(file, Path.Combine(Server.MapPath("~/Assets/TermsConditionFiles"), fileName), true);
                    }
                }

                ht.Clear();
                ht.Add("@mode", "TC");
                ht.Add("@FilePaths", allfilename);
                ht.Add("@FldId", "0");
                ht.Add("@ProjectNmbr", projectid);
                dr = db.SysFetchData("[dbo].[POMS_UpdatefilePaths]", ref ht);
                if (dr.Read())
                {
                }
                dr.Close();
            }
            else if (mode == "PF")
            {
                ht.Clear();
                ht.Add("@mode", "PF");
                ht.Add("@FilePaths", allfilename);
                ht.Add("@FldId", "0");
                ht.Add("@ProjectNmbr", projectid);
                dr = db.SysFetchData("[dbo].[POMS_UpdatefilePaths]", ref ht);
                if (dr.Read())
                {
                }
                dr.Close();
            }
            else if (mode == "ISF")
            {
                ht.Clear();
                ht.Add("@mode", "ISF");
                ht.Add("@FilePaths", allfilename);
                ht.Add("@FldId", "0");
                ht.Add("@ProjectNmbr", projectid);
                dr = db.SysFetchData("[dbo].[POMS_UpdatefilePaths]", ref ht);
                if (dr.Read())
                {
                }
                dr.Close();
            }

            Response.ContentType = "text/plain";
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result = new { name = fileName };
            Response.Write(serializer.Serialize(result));
            Response.End();
        }

        [HttpPost]
        public void UploadWhenEdit(string projectid, string mode, string InstallationScheduleFilecount, string TermsFilecount, string ProjectFilecount)
        {
            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;

            var fileName = "";
            string allfilename = "";
            string fname = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                string ext = "";
                if (mode == "TC")
                {
                    var file = Request.Files[i];
                    string fn = Path.GetFileName(file.FileName);
                    int indx = Path.GetFileName(file.FileName).LastIndexOf('.');
                    int ln = fn.Length;
                    fname = fn.Substring(0, indx);
                    ext = fn.Substring(indx + 1, ln - indx - 1);

                    //ext = Path.GetFileName(file.FileName).Split('.')[1];
                    //fname  = Path.GetFileName(file.FileName).Split('.')[0];
                    fname = fname.Replace(',', '_');
                    fname = fname.Replace("'", "`");
                    fileName = fname + "(" +  mode + "_" + projectid + "_" + (Convert.ToInt32(TermsFilecount) + 1) +  ")" + "." + ext;

                    if (allfilename.Trim() == "")
                    {
                        allfilename = fileName;
                    }
                    else
                    {
                        allfilename += "," + fileName;
                    }
                    var path = Path.Combine(Server.MapPath("~/Assets/TermsConditionFiles"), fileName);
                    file.SaveAs(path);
                    TermsFilecount = Convert.ToString(Convert.ToInt32(TermsFilecount) + 1);
                }
                else if (mode == "PF")
                {
                    var filePF = Request.Files[i];
                    string fn = Path.GetFileName(filePF.FileName);
                    int indx = Path.GetFileName(filePF.FileName).LastIndexOf('.');
                    int ln = fn.Length;
                    fname = fn.Substring(0, indx);
                    ext = fn.Substring(indx + 1, ln - indx - 1);

                    
                    //ext = Path.GetFileName(filePF.FileName).Split('.')[1];
                    //fname = Path.GetFileName(filePF.FileName).Split('.')[0];
                    fname = fname.Replace(',', '_');
                    fname = fname.Replace("'", "`");
                    fileName = fname + "(" + mode + "_" + projectid + "_" + (Convert.ToInt32(ProjectFilecount) + 1) + ")" + "." + ext;
                    ProjectController pc = new ProjectController();
                    pc.ProjectFilesToInclude(Convert.ToString(Convert.ToInt16(projectid) - 1000), fileName + "|||" + "1");
                    if (allfilename.Trim() == "")
                    {
                        allfilename = fileName;
                    }
                    else
                    {
                        allfilename += "," + fileName;
                    }
                    var path = Path.Combine(Server.MapPath("~/Assets/ProjectFiles"), fileName);
                    filePF.SaveAs(path);
                    ProjectFilecount = Convert.ToString(Convert.ToInt32(ProjectFilecount) + 1);
                }
                else if (mode == "ISF")
                {
                    var fileISF = Request.Files[i];
                    string fn = Path.GetFileName(fileISF.FileName);
                    int indx = Path.GetFileName(fileISF.FileName).LastIndexOf('.');
                    int ln = fn.Length;
                    fname = fn.Substring(0, indx);
                    ext = fn.Substring(indx + 1, ln - indx - 1);

                    //var fileISF = Request.Files[i];
                    //ext = Path.GetFileName(fileISF.FileName).Split('.')[1];
                    //fname = Path.GetFileName(fileISF.FileName).Split('.')[0];
                    fname = fname.Replace(',', '_');
                    fname = fname.Replace("'", "`");
                    fileName = fname + "(" + mode + "_" + projectid + "_" + (Convert.ToInt32(InstallationScheduleFilecount) + 1) + ")" + "." + ext;
                    if (allfilename.Trim() == "")
                    {
                        allfilename = fileName;
                    }
                    else
                    {
                        allfilename += "," + fileName;
                    }
                    var path = Path.Combine(Server.MapPath("~/Assets/InstallationScheduleFiles"), fileName);
                    fileISF.SaveAs(path);

                    InstallationScheduleFilecount = Convert.ToString(Convert.ToInt32(InstallationScheduleFilecount) + 1);
                }
            }

            if (mode == "TC")
            {
                ht.Clear();
                ht.Add("@mode", "TC");
                ht.Add("@FilePaths", allfilename);
                ht.Add("@FldId", "0");
                ht.Add("@ProjectNmbr", projectid);
                dr = db.SysFetchData("[dbo].[POMS_UpdatefilePaths]", ref ht);
                if (dr.Read())
                {
                }
                dr.Close();
            }
            else if (mode == "PF")
            {
                ht.Clear();
                ht.Add("@mode", "PF");
                ht.Add("@FilePaths", allfilename);
                ht.Add("@FldId", "0");
                ht.Add("@ProjectNmbr", projectid);
                dr = db.SysFetchData("[dbo].[POMS_UpdatefilePaths]", ref ht);
                if (dr.Read())
                {
                }
                dr.Close();
            }
            else if (mode == "ISF")
            {
                ht.Clear();
                ht.Add("@mode", "ISF");
                ht.Add("@FilePaths", allfilename);
                ht.Add("@FldId", "0");
                ht.Add("@ProjectNmbr", projectid);
                dr = db.SysFetchData("[dbo].[POMS_UpdatefilePaths]", ref ht);
                if (dr.Read())
                {
                }
                dr.Close();
            }

            Response.ContentType = "text/plain";
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var result = new { name = fileName };
            Response.Write(serializer.Serialize(result));
            Response.End();
        }

        public string DeleteFile(string FileName, string ProjectID,string mode)
        {
            string result = "";
            var photoName = "";
            var fullPath = "";

            DBFunctions db = new DBFunctions();
            Hashtable ht = new Hashtable();
            SqlDataReader dr;
            photoName = FileName;

            if (mode == "PF")
            {
                fullPath = Server.MapPath("~/Assets/ProjectFiles/" + photoName);
            }
            else if (mode == "TC")
            {
                fullPath = Server.MapPath("~/Assets/TermsConditionFiles/" + photoName);
            }
            else if (mode == "ISF")
            {
                fullPath = Server.MapPath("~/Assets/InstallationScheduleFiles/" + photoName);
            }
            
            if (System.IO.File.Exists(fullPath))
            {
                ht.Clear();
                ht.Add("@Filename", FileName.Substring(FileName.LastIndexOf('/') + 1));
                ht.Add("@ProjectID", ProjectID);
                ht.Add("@Mode", mode);
               
                dr = db.SysFetchData("[dbo].[POMS_DeleteFileForProject]", ref ht);
                if (dr.Read())
                {
                    result = Convert.ToString(dr["result"]);
                }
                dr.Close();
                System.IO.File.Delete(fullPath);
            }
            return result;
        }

        [HttpPost]
        public void UploadForApprovals(string fileid)
        {
            var fileName = "";
            string fname = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
              

                string ext = "";
                var file = Request.Files[i];
                string fnm = Path.GetFileName(file.FileName);
                int lastindx = fnm.LastIndexOf(".");
                int namelen = fnm.Length;
                int extlen = namelen - lastindx + 1;
                ext = fnm.Substring(lastindx+1);
                fname = fnm.Substring(0, lastindx);
                fname = fname.Replace("'", "`");
                //ext = Path.GetFileName(file.FileName).Split('.')[Path.GetFileName(file.FileName).Split('.').Length - 1];
                //fname = Path.GetFileName(file.FileName).Split('.')[Path.GetFileName(file.FileName).Split('.').Length - 2];
                fileName = fileid + "_" + fname + "." + ext;
                var path = Path.Combine(Server.MapPath("~/Assets/Approvals"), fileName);
                file.SaveAs(path);
                Response.ContentType = "text/plain";
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var result = new { name = fileName };
                Response.Write(serializer.Serialize(result));
                Response.End();
            }
        }

    }
}
