<%@ Control Language="C#" AutoEventWireup="true" Inherits="OpenRasta.Codecs.WebForms.ResourceSubView<NewsList>" %>
<!-- Enumerating stuff using SharpView -->
<%=(s)
    ul[li.ForEach(Resource.NewsEntries)[Resource.NewsEntries.Current()]]
%>
<!-- Enumerating stuff using standard asp.net webforms and c# -->
<ul>
<%foreach (var news in Resource.NewsEntries){%>
    <li><%=news%></li>
<%}%>
</ul>