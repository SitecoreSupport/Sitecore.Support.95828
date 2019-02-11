namespace Sitecore.Support.Xml.Xsl
{
    using Sitecore;
    using Sitecore.Collections;
    using Sitecore.Configuration;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Links;
    using Sitecore.Xml.Xsl;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Implements the Link Renderer.
    /// </summary>
    public class LinkRenderer : Sitecore.Xml.Xsl.LinkRenderer
    {
        private readonly char[] _delimiter = new char[2]
        {
        '=',
        '&'
        };
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Xml.Xsl.LinkRenderer" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public LinkRenderer(Item item) : base(item) { }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns>The render.</returns>
        public override RenderFieldResult Render()
        {
            SafeDictionary<string> safeDictionary = new SafeDictionary<string>();
            safeDictionary.AddRange(Parameters);
            if (MainUtil.GetBool(safeDictionary["endlink"], false))
            {
                return RenderFieldResult.EndLink;
            }
            Set<string> set = Set<string>.Create("field", "select", "text", "haschildren", "before", "after", "enclosingtag", "fieldname", "disable-web-editing");
            LinkField linkField = LinkField;
            if (linkField != null)
            {
                safeDictionary["title"] = HttpUtility.HtmlAttributeEncode(StringUtil.GetString(safeDictionary["title"], linkField.Title));
                safeDictionary["target"] = StringUtil.GetString(safeDictionary["target"], linkField.Target);
                safeDictionary["class"] = StringUtil.GetString(safeDictionary["class"], linkField.Class);
                SetRelAttribute(safeDictionary, linkField);
            }
            string text = string.Empty;
            string rawParameters = RawParameters;
            if (!string.IsNullOrEmpty(rawParameters) && rawParameters.IndexOfAny(_delimiter) < 0)
            {
                text = rawParameters;
            }
            if (string.IsNullOrEmpty(text))
            {
                Item targetItem = TargetItem;
                string text2 = (targetItem != null) ? targetItem.DisplayName : string.Empty;
                string s = (linkField != null) ? linkField.Text : string.Empty;
                s = HttpUtility.HtmlEncode(s);
                text = StringUtil.GetString(text, safeDictionary["text"], s, text2);
            }
            string url = GetUrl(linkField);
            string linkType = LinkType;
            if (linkType == "javascript")
            {
                safeDictionary["href"] = "#";
                safeDictionary["onclick"] = StringUtil.GetString(safeDictionary["onclick"], url);
            }
            else
            {
                safeDictionary["href"] = HttpUtility.HtmlEncode(StringUtil.GetString(safeDictionary["href"], url));
            }
            StringBuilder stringBuilder = new StringBuilder("<a", 47);
            foreach (KeyValuePair<string, string> item in safeDictionary)
            {
                string key = item.Key;
                string value = item.Value;
                if (!set.Contains(key.ToLowerInvariant()))
                {
                    FieldRendererBase.AddAttribute(stringBuilder, key, value);
                }
            }
            stringBuilder.Append('>');
            if (!MainUtil.GetBool(safeDictionary["haschildren"], false))
            {
                if (string.IsNullOrEmpty(text))
                {
                    string href = safeDictionary["href"];
                    if (string.IsNullOrEmpty(href))
                    {
                        return RenderFieldResult.Empty;
                    }
                    text = href;
                }
                stringBuilder.Append(text);
            }
            return new RenderFieldResult
            {
                FirstPart = stringBuilder.ToString(),
                LastPart = "</a>"
            };
        }
    }
}