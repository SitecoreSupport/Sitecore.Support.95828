using System;
using System.Text.RegularExpressions;

namespace Sitecore.Support.Xml.Xsl
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Sitecore.Collections;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Xml.Xsl;

    public class CustomLinkRenderer : LinkRenderer
    {
        private readonly char[] _delimiter;

        public CustomLinkRenderer(Item item) : base(item)
        {
            this._delimiter = new char[] { '=', '&' };
        }

        public override RenderFieldResult Render()
        {
            string str;
            SafeDictionary<string> dictionary = new SafeDictionary<string>();
            dictionary.AddRange(base.Parameters);
            if (MainUtil.GetBool(dictionary["endlink"], false))
            {
                return RenderFieldResult.EndLink;
            }
            Set<string> set = Set<string>.Create(new string[] { "field", "select", "text", "haschildren", "before", "after", "enclosingtag", "fieldname", "disable-web-editing" });
            LinkField linkField = base.LinkField;
            if (linkField != null)
            {
                dictionary["title"] = HttpUtility.HtmlAttributeEncode(StringUtil.GetString(new string[] { dictionary["title"], linkField.Title }));
                dictionary["target"] = StringUtil.GetString(new string[] { dictionary["target"], linkField.Target });
                dictionary["class"] = StringUtil.GetString(new string[] { dictionary["class"], linkField.Class });
            }
            string str2 = string.Empty;
            string rawParameters = base.RawParameters;
            if (!(string.IsNullOrEmpty(rawParameters) || (rawParameters.IndexOfAny(this._delimiter) >= 0)))
            {
                str2 = rawParameters;
            }
            if (string.IsNullOrEmpty(str2))
            {
                Item targetItem = base.TargetItem;
                string str4 = (targetItem != null) ? targetItem.DisplayName : string.Empty;
                string str5 = (linkField != null) ? linkField.Text : string.Empty;
                str2 = StringUtil.GetString(new string[] { str2, dictionary["text"], str5, str4 });
            }
            string url = this.GetUrl(linkField);
            if (((str = base.LinkType) != null) && (str == "javascript"))
            {
                dictionary["href"] = "#";
                dictionary["onclick"] = StringUtil.GetString(new string[] { dictionary["onclick"], url });
            }
            else
            {
                dictionary["href"] = HttpUtility.HtmlEncode(StringUtil.GetString(new string[] { dictionary["href"], url }));
            }
            StringBuilder tag = new StringBuilder("<a", 0x2f);
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                string key = pair.Key;
                string str8 = pair.Value;
                if (!set.Contains(key.ToLowerInvariant()))
                {
                    FieldRendererBase.AddAttribute(tag, key, str8);
                }
            }
            tag.Append('>');
            if (!MainUtil.GetBool(dictionary["haschildren"], false))
            {
                // Check if Description field is empty if it so then use URL as Description Value
                if (string.IsNullOrEmpty(str2) || string.IsNullOrWhiteSpace(str2))
                {
                    string str9 = dictionary["href"];
                    if (string.IsNullOrEmpty(str9))
                    {
                        return RenderFieldResult.Empty;
                    }
                    str2 = str9;
                }
                tag.Append(str2);
            }
            return new RenderFieldResult
            {
                FirstPart = tag.ToString(),
                LastPart = "</a>"
            };
        }
    }
}