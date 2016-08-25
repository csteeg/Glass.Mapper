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

namespace Glass.Mapper.Sc.Razor.Web.Ui
{
    /// <summary>
    /// PartialControl
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PartialControl : TypedControl
    {
        object _tempModel;

        /// <summary>
        /// Sets the model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void SetModel(object model)
        {
           _tempModel = model;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <returns></returns>
        public override object GetModel()
        {
            return _tempModel;
        }
    }

}

