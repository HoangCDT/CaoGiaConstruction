<%@ WebHandler Language="C#" Class="RemoveHandler" %>

using System;
using System.Web;
using CodeUtility;

public class RemoveHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.Clear();

        string fileName = context.Request.Form["file"].ToSafetyString();

        if (fileName == string.Empty)
            context.Response.End();

        string fileUrl = "".StringFormat(fileName);

        Exception ex = null;
        FileUtility.Delete(fileUrl, ref ex);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}