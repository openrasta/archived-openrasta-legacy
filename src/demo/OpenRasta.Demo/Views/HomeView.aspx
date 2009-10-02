<%@ Page Language="C#" Inherits="OpenRasta.Codecs.WebForms.ResourceView<Home>" MasterPageFile="~/Views/HomeView.Master" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="content" runat="server">   
    <div>
        <ul>
            <li><a href="/products">Products</a></li>
            <li><a href="/toc">Terms and Conditions</a></li>
            <li><a href="/news">Example syndicated feed</a></li>                        
            <li><a href="/news/2">Example syndicated feed item</a></li>                        
        </ul>                
    </div>
    
    <div>
        <% Xhtml.RenderResource(typeof(NewsList).CreateUri()); %>
    </div>
    
    <fieldset>
        <legend>Add a user</legend>
        <% using (scope(Xhtml.Form<Customer>().Method("POST"))) { %>
            User name: <%= Xhtml.TextBox<Customer>(c => c.Username) %>
            <input type="submit" value="Add user" />
        <% } %>
    </fieldset>
</asp:Content>
