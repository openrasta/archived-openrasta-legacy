<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductListView.aspx.cs" Inherits="OpenRasta.Demo.Views.ProductListView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD Xhtml 1.0 Transitional//EN" "http://www.w3.org/TR/Xhtml1/DTD/Xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/Xhtml" >
<head runat="server">
        <link rel="stylesheet" type="text/css" href="Content/css/main.css" />
        <title></title>
</head>
<body>
   
    <h1>List of products</h1>
    <ul>
    <%
        foreach (var product in Resource)
        {%>
       <li><a href="<%=product.CreateUri()%>"><%=product.Name%></a></li>
    <%
        }%>
    </ul>
    <fieldset>
        <legend>Add a product</legend>
        <div>
        <%
        using (scope(Xhtml.Form<List<Product>>().Method("POST")))
        {%>
            <label>New product name: <%=Xhtml.TextBox<Product>(c => c.Name)%></label>
            <input type="submit" value="Add product" />
        <%
        }%>
        </div>
    </fieldset>
</body>
</html>
