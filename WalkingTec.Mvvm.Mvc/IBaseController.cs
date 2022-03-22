using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public interface IBaseController
    {
        WTMContext Wtm { get; set; }

        Configs ConfigInfo { get; }
        GlobalData GlobaInfo { get; }
        string CurrentCS { get; }

        DBTypeEnum? CurrentDbType { get; }

        IDataContext DC { get; }

        IDistributedCache Cache { get; }

        string BaseUrl { get; }

        ModelStateDictionary ModelState { get; }

        IStringLocalizer Localizer { get; }
    }
}