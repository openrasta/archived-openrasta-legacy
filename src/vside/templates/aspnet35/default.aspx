<%@ Page Language="C#" %>
<!-- Nothing to see here. -->
<html>
<head>
    <title>Redirection page</title>
</head>
<body>
<%
string originalPath = Request.Path;
HttpContext.Current.RewritePath(Request.ApplicationPath, false);
IHttpHandler httpHandler = new OpenRasta.Hosting.AspNet.OpenRastaIntegratedHandler();
httpHandler.ProcessRequest(HttpContext.Current);
HttpContext.Current.RewritePath(originalPath, false);
%>
</body>
</html>