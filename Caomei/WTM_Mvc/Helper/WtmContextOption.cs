using Caomei.Core;
using Caomei.Core.Support.FileHandlers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace Caomei.Mvc.Helper
{
    public class WtmContextOption
    {
        /// <summary>
        /// A Func that return ConnectionString key name, used for multi database program to select
        /// a db at run time
        /// </summary>
        public Func<ActionExecutingContext, string> CsSelector { get; set; }

        /// <summary>
        /// Set the data privileges that this system support
        /// </summary>
        public List<IDataPrivilege> DataPrivileges { get; set; }

        /// <summary>
        /// Set the sub directory of uploaded file, if you want to save file in different
        /// directories according to datetime or other properties, use this selector
        /// </summary>
        public Func<IWtmFileHandler, string> FileSubDirSelector { get; set; }

        public Func<WTMContext, string, LoginUserInfo> ReloadUserFunc { get; set; }
    }
}