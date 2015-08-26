﻿using System.Collections.Generic;
using Glass.Mapper.Pipelines.ObjectConstruction;
using Glass.Mapper.Sc.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Glass.Mapper.Sc.Pipelines.ObjectConstruction
{
    public class EnforcedTemplateCheck : IObjectConstructionTask
    {
        private static Dictionary<string, bool> _cache;

        static EnforcedTemplateCheck()
        {
            _cache= new Dictionary<string, bool>();
        }

        public void Execute(ObjectConstructionArgs args)
        {

            if (args.Result == null)
            {
                var scConfig = args.Configuration as SitecoreTypeConfiguration;

                if (scConfig != null && scConfig.EnforceTemplate != SitecoreEnforceTemplate.No)
                {
                    var scArgs = args.AbstractTypeCreationContext as SitecoreTypeCreationContext;

                    var key = "{0} {1} {2}".Formatted(scConfig.TemplateId, scArgs.Item.TemplateID, scConfig.EnforceTemplate);
                    var result = false;

                    if (_cache.ContainsKey(key))
                    {
                        result = _cache[key];
                    }
                    else
                    {
                        var item = scArgs.Item;

                        if (scConfig.EnforceTemplate == SitecoreEnforceTemplate.TemplateAndBase)
                        {
                            result = TemplateAndBaseCheck(item.Template, scConfig.TemplateId);
                        }
                        else if(scConfig.EnforceTemplate == SitecoreEnforceTemplate.Template)
                        {
                            result = item.TemplateID == scConfig.TemplateId;
                        }

                        _cache[key] = result;
                    }

                    if (!result)
                    {
                        args.AbortPipeline();
                    }

                }

            }
        }

        protected virtual bool TemplateAndBaseCheck(TemplateItem template, ID templateId)
        {
            if (template.ID == templateId)
            {
                return true;
            }


            foreach (var baseTemplate in template.BaseTemplates)
            {
                if (TemplateAndBaseCheck(baseTemplate, templateId))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
