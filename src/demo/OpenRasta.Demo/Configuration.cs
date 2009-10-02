#region License
/* Authors:
 *      Sebastien Lambla (seb@serialseb.com)
 * Copyright:
 *      (C) 2007-2009 Caffeine IT & naughtyProd Ltd (http://www.caffeine-it.com)
 * License:
 *      This file is distributed under the terms of the MIT License found at the end of this file.
 */
#endregion

using System.Collections.Generic;
using System.ServiceModel.Syndication;
using OpenRasta.Configuration;
using OpenRasta.Configuration.Fluent;
using OpenRasta.Configuration.WebForms;
using OpenRasta.Demo.Codecs;
using OpenRasta.Demo.Handlers;
using OpenRasta.Demo.Resources;

namespace OpenRasta.Demo
{
    public class Configuration : IConfigurationSource //, IDependencyResolverFactory
    {
        public void Configure()
        {
            using (OpenRastaConfiguration.Manual)
            {
                ResourceSpace.Uses.SharpView();

                ResourceSpace.Has.ResourcesOfType<Home>()
                    .AtUri("/home")
                    .And.AtUri("/")
                    .HandledBy<HomeHandler>()
                    .RenderedByAspx(new { index = "~/Views/HomeView.aspx" })
                    .And.AsJsonDataContract().ForMediaType("application/json;q=0.3")
                    .And.AsXmlDataContract().ForMediaType("application/xml;q=0.2");

                ResourceSpace.Has.TheUri("/toc").ForThePage("~/Views/toc.aspx");

                ResourceSpace.Has.ResourcesOfType<IList<Widget>>()
                    .AtUri("/widgets")
                    .HandledBy<WidgetListHandler>()
                    .RenderedByAspx("~/Views/WidgetList.aspx")
                    .And
                    .AsXmlDataContract();

                ResourceSpace.Has.ResourcesOfType<Widget>()
                    .AtUri("/widgets/{name}").Named("something")
                    .HandledBy<WidgetListHandler>()
                    .RenderedByAspx(new{ something = "~/Views/Widget.aspx"});

                ResourceSpace.Has.ResourcesOfType<IList<Product>>()
                    .AtUri("/products")
                    .HandledBy<ProductListHandler>()
                    .RenderedByAspx(new {index = "~/Views/ProductListView.aspx"});

                ResourceSpace.Has.ResourcesOfType<Product>()
                    .AtUri("/products/{name}")
                    .HandledBy<ProductHandler>()
                    .RenderedByAspx(new { index = "~/Views/ProductView.aspx" });

                ResourceSpace.Has.ResourcesOfType<NewsList>()
                    .AtUri("/newslist")
                    .HandledBy<NewsListHandler>()
                    .RenderedByUserControl("~/Views/NewsListPartialView.ascx");

                ResourceSpace.Has.ResourcesOfType<Customer>()
                    .AtUri("/customers")
                    .HandledBy<CustomerHandler>();

                ResourceSpace.Has.ResourcesOfType<SyndicationFeed>()
                    .AtUri("/news")
                    .HandledBy<ArticleHandler>()
                    .TranscodedBy<AtomFeedCodec>(null)
                    .And.TranscodedBy<RssFeedCodec>(null);

                ResourceSpace.Has.ResourcesOfType<SyndicationItem>()
                    .AtUri("/news/{id}")
                    .HandledBy<ArticleHandler>()
                    .TranscodedBy<AtomItemCodec>(null)
                    .And.TranscodedBy<RssItemCodec>(null);
            }
        }

        //public IDependencyResolver GetResolver() { return new WindsorDependencyResolver(new WindsorContainer()); }
    }

}

#region Full license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion