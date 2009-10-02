<%@ Page Language="C#" Inherits="OpenRasta.Codecs.WebForms.ResourceView<Widget>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">    
    <link rel="stylesheet" type="text/css" href="Content/css/main.css" />
    <title></title>
</head>
<body>
    <div>
    <%= div[strong[Resource.Description]] %>
    
    <% using(scope(Xhtml.Form(Resource).Method("POST"))){ %>
        <%= Xhtml.TextBox(()=>Resource.Description) %>
        <input type="submit" />
    <%}%>
    
    </div>
</body>
</html>
