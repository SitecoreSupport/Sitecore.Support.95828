namespace Sitecore.Support.Pipelines.RenderField
{
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines.RenderField;
    using Sitecore.Support.Xml.Xsl;
    using Sitecore.Xml.Xsl;
    using System;

    public class GetLinkFieldValue
    {
        protected virtual LinkRenderer CreateRenderer(Item item)
        {
            return new CustomLinkRenderer(item);
        }
            
        public void Process(RenderFieldArgs args)
        {
            if (!this.SkipProcessor(args))
            {
                SetWebEditParameters(args, new string[] { "class", "text", "target", "haschildren" });
                if (!string.IsNullOrEmpty(args.Parameters["text"]))
                {
                    args.WebEditParameters["text"] = args.Parameters["text"];
                }
                LinkRenderer renderer = this.CreateRenderer(args.Item);
                renderer.FieldName = args.FieldName;
                renderer.FieldValue = args.FieldValue;
                renderer.Parameters = args.Parameters;
                renderer.RawParameters = args.RawParameters;
                args.DisableWebEditFieldWrapping = true;
                args.DisableWebEditContentEditing = true;
                RenderFieldResult result = renderer.Render();
                args.Result.FirstPart = result.FirstPart;
                args.Result.LastPart = result.LastPart;
            }
        }

        private static void SetWebEditParameters(RenderFieldArgs args, params string[] parameterNames)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(parameterNames, "parameterNames");
            foreach (string str in parameterNames)
            {
                if (!string.IsNullOrEmpty(args.Parameters[str]))
                {
                    args.WebEditParameters[str] = args.Parameters[str];
                }
            }
        }

        protected virtual bool SkipProcessor(RenderFieldArgs args)
        {
            if (args == null)
            {
                return true;
            }
            string fieldTypeKey = args.FieldTypeKey;
            return ((fieldTypeKey != "link") && (fieldTypeKey != "general link"));
        }
    }
}