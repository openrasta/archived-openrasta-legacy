<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductView.aspx.cs" Inherits="OpenRasta.Demo.Views.ProductView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD Xhtml 1.0 Transitional//EN" "http://www.w3.org/TR/Xhtml1/DTD/Xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/Xhtml" >
<head runat="server">
    <link rel="stylesheet" type="text/css" href="Content/css/main.css" />
    <title></title>
</head>
<body>
    <div>
        <h1><%=Resource.Name%></h1>
        <div><%= Resource.Description %></div>
        <fieldset>
            <legend>Query</legend>
        
        <% using (scope(Xhtml.Form(Resource).EncType(MediaType.MultipartFormData).Method("POST"))) { %>            
            <label><%= Xhtml.TextBox(()=>Resource.Description) %></label>
            <input type="submit" />
        <% } %>
        
        <% using (scope(Xhtml.Form(Resource).EncType(MediaType.MultipartFormData).Method("POST")))
          { %>                  
            <label>Image: <input type="file" name="image" accept="image/*" /></label>
            <input type="submit" />
        <%} %>
        
        </fieldset>
    </div>
</body>
</html>
