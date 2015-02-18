﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Mapper.Pipelines.ObjectConstruction;
using Glass.Mapper.Sc.Configuration;
using Sitecore.Data;

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

                if (scConfig.EnforceTemplate != SitecoreEnforceTemplate.No)
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
                            result = item.TemplateID == scConfig.TemplateId ||
                                     item.Template.BaseTemplates.Any(x => x.ID == scConfig.TemplateId);
                        }
                        else if(scConfig.EnforceTemplate == SitecoreEnforceTemplate.Template)
                        {
                            result = item.TemplateID == scConfig.TemplateId;
                        }

                        _cache[key] = result;
                    }

                    if (result)
                    {
                        return;
                    }
                    else
                    {
                        args.AbortPipeline();
                    }

                }

            }
        }
    }
}
