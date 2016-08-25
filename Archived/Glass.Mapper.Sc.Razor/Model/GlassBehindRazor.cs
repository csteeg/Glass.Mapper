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
using Glass.Mapper.Sc.Configuration.Attributes;

namespace Glass.Mapper.Sc.Razor.Model
{
    /// <summary>
    /// Class GlassBehindRazor
    /// </summary>
    [SitecoreType(TemplateId="{9B162562-C999-45BF-B688-1C65D0EBCAAD}")]
    public class GlassBehindRazor
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [SitecoreInfo(Configuration.SitecoreInfoType.Name)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view.</value>
        [SitecoreField("Name")]
        public string View { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [SitecoreField]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the assembly.
        /// </summary>
        /// <value>The assembly.</value>
        [SitecoreField]
        public string Assembly { get; set; }
    }
}

