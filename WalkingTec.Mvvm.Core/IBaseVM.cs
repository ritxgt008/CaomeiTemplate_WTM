using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// IBaseVM
    /// </summary>
    public interface IBaseVM
    {
        #region Property

        WTMContext Wtm { get; set; }

        /// <summary>
        /// UniqueId
        /// </summary>
        string UniqueId { get; }

        /// <summary>
        /// WindowIds
        /// </summary>
        string WindowIds { get; }

        /// <summary>
        /// ViewDivId
        /// </summary>
        string ViewDivId { get; set; }

        /// <summary>
        /// DC
        /// </summary>
        IDataContext DC { get; set; }

        /// <summary>
        /// VMFullName
        /// </summary>
        string VMFullName { get; }

        /// <summary>
        /// CreatorAssembly
        /// </summary>
        string CreatorAssembly { get; set; }

        /// <summary>
        /// CurrentCS
        /// </summary>
        string CurrentCS { get; }

        /// <summary>
        /// FC
        /// </summary>
        Dictionary<string, object> FC { get; set; }

        /// <summary>
        /// Config
        /// </summary>
        Configs ConfigInfo { get; }

        ISessionService Session { get; }

        IDistributedCache Cache { get; }

        LoginUserInfo LoginUserInfo { get; }

        #endregion Property

        #region Event

        /// <summary>
        /// InitVM 完成后触发的事件
        /// </summary>
        event Action<IBaseVM> OnAfterInit;

        /// <summary>
        /// ReInitVM 完成后触发的事件
        /// </summary>
        event Action<IBaseVM> OnAfterReInit;

        #endregion Event

        #region Method

        /// <summary>
        /// 调用 InitVM 并触发 OnAfterInit 事件
        /// </summary>
        void DoInit();

        /// <summary>
        /// 调用 ReInitVM 并触发 OnAfterReInit 事件
        /// </summary>
        void DoReInit();

        #endregion Method
    }
}