<%@ Page Language="C#" %><%
string originalPath = Request.Path;
HttpContext.Current.RewritePath(Request.ApplicationPath, false);
IHttpHandler httpHandler = new OpenRasta.Hosting.AspNet.OpenRastaIntegratedHandler();
httpHandler.ProcessRequest(HttpContext.Current);
HttpContext.Current.RewritePath(originalPath, false);
%>