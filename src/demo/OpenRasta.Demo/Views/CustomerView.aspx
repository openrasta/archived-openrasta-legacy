<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerView.aspx.cs" Inherits="OpenRasta.Demo.Views.CustomerView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD Xhtml 1.0 Transitional//EN" "http://www.w3.org/TR/Xhtml1/DTD/Xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/Xhtml" >
<head runat="server">
    <link rel="stylesheet" type="text/css" href="Content/css/main.css" />
    <title></title>
</head>
<body>
    <div>
    Customer: <%=this.Resource.Name%>
    </div>
</body>
</html>
