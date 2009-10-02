<%@ Page Language="C#" Inherits="OpenRasta.Codecs.WebForms.ResourceView<IList<Widget>>" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link rel="stylesheet" type="text/css" href="Content/css/main.css" />
    <title></title>
</head>
<body>
    <div>
    <%= this.SharpView(()=>
    
        ul.Class("myWidgets")
            [li.ForEach(Resource)
                [a.Href(Resource.Current().CreateUri())[Resource.Current().Name]]
            ]
            
        )  %>
        
    <% foreach (var widget in Resource){ %>
       <%= widget.Name %>
    <%} %>
    <form action="/widgets" method="post">
        <%= Xhtml.TextBox<Widget>(w=>w.Name) %>
        <input type="submit" />
    </form>
    </div>
</body>
</html>
