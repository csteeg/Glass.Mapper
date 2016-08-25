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

namespace Glass.Mapper.Umb.Configuration
{
    /// <summary>
    /// UmbracoInfoConfiguration
    /// </summary>
    public class UmbracoInfoConfiguration : InfoConfiguration
    {
        /// <summary>
        /// The type of information that should populate the property
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public UmbracoInfoType Type { get; set; }

        protected override AbstractPropertyConfiguration CreateCopy()
        {
            return new UmbracoInfoConfiguration();
        }

        protected override void Copy(AbstractPropertyConfiguration copy)
        {
            var config = copy as UmbracoInfoConfiguration;
            config.Type = Type;
            base.Copy(copy);
        }
    }
}

