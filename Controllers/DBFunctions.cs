using POMS.Controllers;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class DBFunctions
{
    string conString = System.Configuration.ConfigurationManager.AppSettings["connString"].ToString();
    public SqlConnection cvCon = new SqlConnection(ConfigurationManager.AppSettings["connString"].ToString());
    private SqlCommand lvCom;
    public SqlDataReader lvRed;
    public String VarHoldOption;
    private SqlCommand cmd;
    public SqlDataReader rdr;
    public DataSet ds = new DataSet();
    private SqlDataAdapter da;


    public SqlDataReader SysFetchData(string spName, ref Hashtable ParameterName)
    {
        SqlDataReader dr = null;
        IDictionaryEnumerator lvEnum = ParameterName.GetEnumerator();
        try
        {
            lvCom = new SqlCommand(spName, cvCon);
            lvCom.CommandTimeout = 0;
            cvCon.Close();
            if (cvCon.State == 0)
            {
                cvCon.Open();
            }
            lvCom.CommandType = CommandType.StoredProcedure;
            while (lvEnum.MoveNext())
            {
                lvCom.Parameters.AddWithValue(lvEnum.Key.ToString(), lvEnum.Value);
            }
            //dr.Close();
            dr = lvCom.ExecuteReader(CommandBehavior.CloseConnection);
        }
        catch (Exception ex)
        {

            LogWriter ll1 = new LogWriter("Login New DB:- " + ex.Message);
        }

        return dr;
    }

    public SqlDataAdapter SysFetchDataInDataAdapter(string spName, ref Hashtable ParameterName)
    {
        if (ds.Tables.Count > 0)
            ds.Tables[0].Clear();
        IDictionaryEnumerator iEnum = ParameterName.GetEnumerator();
        cmd = new SqlCommand(spName, cvCon);
        cmd.CommandTimeout = 0;
        if (cvCon.State == ConnectionState.Closed)
            cvCon.Open();
        cmd.CommandType = CommandType.StoredProcedure;
        while (iEnum.MoveNext())
        {
            cmd.Parameters.AddWithValue(iEnum.Key.ToString(), iEnum.Value.ToString());
        }
        da = new SqlDataAdapter(cmd);
        cvCon.Close();
        return da;
    }

    public void SysAddUpdateDelete(string spName, ref Hashtable ParameterName)
    {
        IDictionaryEnumerator lvEnum = ParameterName.GetEnumerator();
        lvCom = new SqlCommand(spName, cvCon);
        lvCom.CommandTimeout = 0;
        if (cvCon.State == 0)
        {
            cvCon.Open();
        }
        lvCom.CommandType = CommandType.StoredProcedure;
        while (lvEnum.MoveNext())
        {
            lvCom.Parameters.AddWithValue(lvEnum.Key.ToString(), lvEnum.Value);
        }
        lvRed = lvCom.ExecuteReader();
        lvRed.Close();
    }

    public DataSet GetDataSet(string query)
    {
        if (ds.Tables.Count > 0)
            ds.Tables[0].Clear();
        //DataSet dataset = new DataSet();
        cmd = new SqlCommand(query, cvCon);
        da = new SqlDataAdapter(cmd);
        try
        {
            da.Fill(ds);
        }
        catch (Exception)
        {
        }
        cvCon.Close();
        return ds;
    }

    public DataSet SysFetchDataInDataSet(string spName, Hashtable htParam)
    {
        int to = cvCon.ConnectionTimeout;
        IDictionaryEnumerator iEnum = htParam.GetEnumerator();
        cmd = new SqlCommand(spName, cvCon);
        cmd.CommandTimeout = 0;
        if (cvCon.State == ConnectionState.Closed)
            cvCon.Open();
        cmd.CommandType = CommandType.StoredProcedure;
        while (iEnum.MoveNext())
        {
            cmd.Parameters.AddWithValue(iEnum.Key.ToString(), iEnum.Value.ToString());
        }
        da = new SqlDataAdapter(cmd);
        DataSet ds = new DataSet();
        da.Fill(ds);
        cvCon.Close();
        return ds;

    }


    public DataSet SysFetchDataInDataSetNew(string spName, Hashtable htParam)
    {
        int to = cvCon.ConnectionTimeout;

        ds = new DataSet();
        if (ds.Tables.Count > 0)
            ds.Tables[0].Clear();
        IDictionaryEnumerator iEnum = htParam.GetEnumerator();
        cmd = new SqlCommand(spName, cvCon);
        cmd.CommandTimeout = 0;
        if (cvCon.State == ConnectionState.Closed)
            cvCon.Open();
        cmd.CommandType = CommandType.StoredProcedure;
        while (iEnum.MoveNext())
        {
            cmd.Parameters.AddWithValue(iEnum.Key.ToString(), iEnum.Value.ToString());
        }
        da = new SqlDataAdapter(cmd);
        da.Fill(ds);
        cvCon.Close();
        return ds;

    }

    public SqlDataReader RunQuery(string query)
    {
        SqlDataReader dr = null;
        SqlConnection cvCon = new SqlConnection(ConfigurationManager.AppSettings.Get(0));
        SqlCommand cmd = new SqlCommand(query, cvCon);
        cmd.CommandType = CommandType.Text;
        try
        {
            if (cvCon.State == ConnectionState.Closed)
                cvCon.Open();
            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        }
        catch (Exception)
        { }

        return dr;
    }

    public SqlDataReader RunSqlQuery1(string query)
    {
        SqlDataReader dr = null;
        SqlConnection cvCon = new SqlConnection(ConfigurationManager.AppSettings.Get(0));
        SqlCommand cmd = new SqlCommand(query, cvCon);
        cmd.CommandType = CommandType.Text;
        try
        {
            if (cvCon.State == ConnectionState.Closed)
                cvCon.Open();
            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        catch (Exception)
        { }

        return dr;
    }

    public static SqlDataReader RunSqlQuery(string query)
    {
        SqlDataReader dr = null;
        SqlConnection cvCon = new SqlConnection(ConfigurationManager.AppSettings.Get(0));
        SqlCommand cmd = new SqlCommand(query, cvCon);
        cmd.CommandType = CommandType.Text;
        try
        {
            if (cvCon.State == ConnectionState.Closed)
                cvCon.Open();
            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        }
        catch (Exception)
        { }
        return dr;
    }


}
