/*
   Copyright 2012 Michael Edwards
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 
*/ 
//-CRE-

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Mapper.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Glass.Mapper.Sc.Configuration
{
    public class SitecoreTypeConfiguration : AbstractTypeConfiguration
    {
        public ID TemplateId { get; set; }

        public ID BranchId { get; set; }

        public SitecoreIdConfiguration IdConfig { get; set; }
        public SitecoreInfoConfiguration LanguageConfig { get; set; }
        public SitecoreInfoConfiguration VersionConfig { get; set; }
        /// <summary>
        /// Indicates that the class is used in a code first scenario.
        /// </summary>
        public bool CodeFirst { get; set; }

        /// <summary>
        /// Overrides the default template name when using code first
        /// </summary>
        public string TemplateName { get; set; }

        public Item ResolveItem(object target, Database database)
        {
            ID id;
            Language language = null;
            int versionNumber = -1;

            if (IdConfig == null)
                throw new NotSupportedException(
                    "You can not save a class that does not contain a property that represents the item ID. Ensure that at least one property has the SitecoreIdAttribute");

            if (IdConfig.PropertyInfo.PropertyType == typeof (Guid))
            {
                var guidId = (Guid) IdConfig.PropertyInfo.GetValue(target, null);
                id = new ID(guidId);
            }
            else if (IdConfig.PropertyInfo.PropertyType == typeof (ID))
            {
                id = IdConfig.PropertyInfo.GetValue(target, null) as ID;
            }
            else
            {
                throw new NotSupportedException("Cannot get ID for item");
            }

            if (LanguageConfig != null)
            {
                language = LanguageConfig.PropertyInfo.GetValue(target, null) as Language;
                if (language == null)
                    language = Language.Current;
            }

            if (VersionConfig != null)
            {
                versionNumber = (int) VersionConfig.PropertyInfo.GetValue(target, null);
            }

            if (language != null && versionNumber > 0)
            {
                return database.GetItem(id, language, new global::Sitecore.Data.Version(versionNumber));
            }
            else if (language != null)
            {
                return database.GetItem(id, language);
            }
            else
            {
                return database.GetItem(id);
            }
        }
    }
}



