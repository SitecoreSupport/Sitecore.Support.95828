namespace Sitecore.Support.XA.Foundation.Multisite.LinkManagers
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Configuration;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.DependencyInjection;
    using Sitecore.Links;
    using Sitecore.Sites;
    using Sitecore.Web;
    using Sitecore.XA.Foundation.Multisite;
    using Sitecore.XA.Foundation.Multisite.LinkManagers;
    using Sitecore.Xml.Xsl;

    public class SupportSxaLinkRenderer : Sitecore.Support.Xml.Xsl.LinkRenderer
    {
        protected ISiteInfoResolver SiteInfoResolver
        {
            get;
        } = ServiceLocator.ServiceProvider.GetService<ISiteInfoResolver>();


        public SupportSxaLinkRenderer(Item item)
            : base(item)
        {
        }

        protected override string GetUrl(XmlField field)
        {
            if (field != null)
            {
                return new LinkItem(field.Value).TargetUrl;
            }
            return LinkManager.GetItemUrl(base.Item, GetUrlOptions(base.Item));
        }

        protected UrlOptions GetUrlOptions(Item item)
        {
            UrlOptions defaultUrlOptions = LinkManager.GetDefaultUrlOptions();
            SiteInfo siteInfo = SiteInfoResolver.GetSiteInfo(item);
            defaultUrlOptions.SiteResolving = Settings.Rendering.SiteResolving;
            defaultUrlOptions.Site = new SiteContext(siteInfo);
            return defaultUrlOptions;
        }
    }
}