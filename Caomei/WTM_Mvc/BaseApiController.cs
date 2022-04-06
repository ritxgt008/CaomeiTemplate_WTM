using Caomei.Core;
using Caomei.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Caomei.Mvc
{
    public abstract class BaseApiController : ControllerBase, IBaseController
    {
        [JsonIgnore]
        [BindNever]
        public WTMContext Wtm { get; set; }

        [JsonIgnore]
        [BindNever]
        public Configs ConfigInfo { get => Wtm?.ConfigInfo; }

        [JsonIgnore]
        [BindNever]
        public GlobalData GlobaInfo { get => Wtm?.GlobaInfo; }

        [JsonIgnore]
        [BindNever]
        public IDistributedCache Cache { get => Wtm?.Cache; }

        [JsonIgnore]
        [BindNever]
        public string CurrentCS { get => Wtm?.CurrentCS; }

        [JsonIgnore]
        [BindNever]
        public DBTypeEnum? CurrentDbType { get => Wtm?.CurrentDbType; }

        [JsonIgnore]
        [BindNever]
        public IDataContext DC { get => Wtm?.DC; }

        [JsonIgnore]
        [BindNever]
        public string BaseUrl { get => Wtm?.BaseUrl; }

        [JsonIgnore]
        [BindNever]
        public IStringLocalizer Localizer { get => Wtm?.Localizer; }

        //-------------------------------------------方法------------------------------------//

        //#region CreateVM
        ///// <summary>
        ///// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        ///// </summary>
        ///// <param name="VMType">The type of the viewmodel</param>
        ///// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        ///// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        ///// <param name="values">properties of the viewmodel that you want to assign values</param>
        ///// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        ///// <returns>ViewModel</returns>
        //[NonAction]
        //private BaseVM CreateVM(Type VMType, object Id = null, object[] Ids = null, Dictionary<string, object> values = null, bool passInit = false)
        //{
        //    //通过反射创建ViewModel并赋值
        //    var ctor = VMType.GetConstructor(Type.EmptyTypes);
        //    BaseVM rv = ctor.Invoke(null) as BaseVM;
        //    rv.Wtm = Wtm;
        //    rv.FC = new Dictionary<string, object>();
        //    rv.CreatorAssembly = this.GetType().AssemblyQualifiedName;
        //    rv.ControllerName = this.GetType().FullName;
        //    if (HttpContext != null && HttpContext.Request != null)
        //    {
        //        try
        //        {
        //            if (Request.QueryString != QueryString.Empty)
        //            {
        //                foreach (var key in Request.Query.Keys)
        //                {
        //                    if (rv.FC.Keys.Contains(key) == false)
        //                    {
        //                        rv.FC.Add(key, Request.Query[key]);
        //                    }
        //                }
        //            }
        //            var f = HttpContext.Request.Form;
        //            foreach (var key in f.Keys)
        //            {
        //                if (rv.FC.Keys.Contains(key) == false)
        //                {
        //                    rv.FC.Add(key, f[key]);
        //                }
        //            }
        //        }
        //        catch { }
        //    }
        //    //如果传递了默认值，则给vm赋值
        //    if (values != null)
        //    {
        //        foreach (var v in values)
        //        {
        //            PropertyHelper.SetPropertyValue(rv, v.Key, v.Value, null, false);
        //        }
        //    }
        //    //如果ViewModel T继承自BaseCRUDVM<>且Id有值，那么自动调用ViewModel的GetById方法
        //    if (Id != null && rv is IBaseCRUDVM<TopBasePoco> cvm)
        //    {
        //        cvm.SetEntityById(Id);
        //    }
        //    //如果ViewModel T继承自IBaseBatchVM<BaseVM>，则自动为其中的ListVM和EditModel初始化数据
        //    if (rv is IBaseBatchVM<BaseVM> temp)
        //    {
        //        temp.Ids = new string[] { };
        //        if (Ids != null)
        //        {
        //            var tempids = new List<string>();
        //            foreach (var iid in Ids)
        //            {
        //                tempids.Add(iid.ToString());
        //            }
        //            temp.Ids = tempids.ToArray();
        //        }
        //        if (temp.ListVM != null)
        //        {
        //            temp.ListVM.CopyContext(rv);
        //            temp.ListVM.Ids = Ids == null ? new List<string>() : temp.Ids.ToList();
        //            temp.ListVM.SearcherMode = ListVMSearchModeEnum.Batch;
        //            temp.ListVM.NeedPage = false;
        //        }
        //        if (temp.LinkedVM != null)
        //        {
        //            temp.LinkedVM.CopyContext(rv);
        //        }
        //        if (temp.ListVM != null)
        //        {
        //            //绑定ListVM的OnAfterInitList事件，当ListVM的InitList完成时，自动将操作列移除
        //            temp.ListVM.OnAfterInitList += (self) =>
        //            {
        //                self.RemoveActionColumn();
        //                self.RemoveAction();
        //                if (temp.ErrorMessage.Count > 0)
        //                {
        //                    self.AddErrorColumn();
        //                }
        //            };
        //            if (temp.ListVM.Searcher != null)
        //            {
        //                var searcher = temp.ListVM.Searcher;
        //                searcher.CopyContext(rv);
        //                if (passInit == false)
        //                {
        //                    searcher.DoInit();
        //                }
        //            }
        //        }
        //        temp.LinkedVM?.DoInit();
        //        //temp.ListVM?.DoSearch();
        //    }
        //    //如果ViewModel是ListVM，则初始化Searcher并调用Searcher的InitVM方法
        //    if (rv is IBasePagedListVM<TopBasePoco, ISearcher> lvm)
        //    {
        //        var searcher = lvm.Searcher;
        //        searcher.CopyContext(rv);
        //        if (passInit == false)
        //        {
        //            //获取保存在Cookie中的搜索条件的值，并自动给Searcher中的对应字段赋值
        //            string namePre = ConfigInfo.CookiePre + "`Searcher" + "`" + rv.VMFullName + "`";
        //            Type searcherType = searcher.GetType();
        //            var pros = searcherType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).ToList();
        //            pros.Add(searcherType.GetProperty("IsValid"));

        // searcher.DoInit(); } } if (rv is IBaseImport<BaseTemplateVM> tvm) { var template =
        // tvm.Template; template.CopyContext(rv); template.DoInit(); }

        //    //自动调用ViewMode的InitVM方法
        //    if (passInit == false)
        //    {
        //        rv.DoInit();
        //    }
        //    return rv;
        //}

        ///// <summary>
        ///// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        ///// </summary>
        ///// <typeparam name="T">The type of the viewmodel</typeparam>
        ///// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        ///// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        ///// <param name="values">use Lambda to set viewmodel's properties,use && for multiply properties, for example Wtm.CreateVM<Test>(values: x=>x.Field1=='a' && x.Field2 == 'b'); will set viewmodel's Field1 to 'a' and Field2 to 'b'</param>
        ///// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        ///// <returns>ViewModel</returns>
        //[NonAction]
        //public T Wtm.CreateVM<T>(object Id = null, object[] Ids = null, Expression<Func<T, object>> values = null, bool passInit = false) where T : BaseVM
        //{
        //    SetValuesParser p = new SetValuesParser();
        //    var dir = p.Parse(values);
        //    return CreateVM(typeof(T), Id, Ids, dir, passInit) as T;
        //}

        ///// <summary>
        ///// Create a ViewModel, and pass Session,cache,dc...etc to the viewmodel
        ///// </summary>
        ///// <param name="VmFullName">the fullname of the viewmodel's type</param>
        ///// <param name="Id">If the viewmodel is a BaseCRUDVM, the data having this id will be fetched</param>
        ///// <param name="Ids">If the viewmodel is a BatchVM, the BatchVM's Ids property will be assigned</param>
        ///// <param name="passInit">if true, the viewmodel will not call InitVM internally</param>
        ///// <returns>ViewModel</returns>
        //[NonAction]
        //public BaseVM CreateVM(string VmFullName, object Id = null, object[] Ids = null, bool passInit = false)
        //{
        //    return CreateVM(Type.GetType(VmFullName), Id, Ids, null, passInit);
        //}
        //#endregion

        #region ReInit model

        [NonAction]
        private void SetReInit(ModelStateDictionary msd, BaseVM model)
        {
            var reinit = model.GetType().GetTypeInfo().GetCustomAttributes(typeof(ReInitAttribute), false).Cast<ReInitAttribute>().SingleOrDefault();

            if (ModelState.IsValid)
            {
                if (reinit != null && (reinit.ReInitMode == ReInitModes.SUCCESSONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                {
                    model.DoReInit();
                }
            }
            else
            {
                if (reinit == null || (reinit.ReInitMode == ReInitModes.FAILEDONLY || reinit.ReInitMode == ReInitModes.ALWAYS))
                {
                    model.DoReInit();
                }
            }
        }

        #endregion ReInit model

        #region Validate model

        [NonAction]
        public Dictionary<string, string> RedoValidation(object item)
        {
            Dictionary<string, string> rv = new Dictionary<string, string>();
            TryValidateModel(item);

            foreach (var e in ControllerContext.ModelState)
            {
                if (e.Value.ValidationState == ModelValidationState.Invalid)
                {
                    rv.Add(e.Key, e.Value.Errors.Select(x => x.ErrorMessage).ToSepratedString());
                }
            }

            return rv;
        }

        #endregion Validate model

        #region update viewmodel

        /// <summary>
        /// Set viewmodel's properties to the matching items posted by user
        /// </summary>
        /// <param name="vm">     ViewModel </param>
        /// <param name="prefix"> prefix </param>
        /// <returns> true if success </returns>
        [NonAction]
        public bool RedoUpdateModel(object vm, string prefix = null)
        {
            try
            {
                BaseVM bvm = vm as BaseVM;
                foreach (var item in bvm.FC.Keys)
                {
                    PropertyHelper.SetPropertyValue(vm, item, bvm.FC[item], prefix, true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion update viewmodel

        protected JsonResult JsonMore(object data, int statusCode = StatusCodes.Status200OK, string msg = "success")
        {
            return new JsonResult(new JsonResultT<object> { Msg = msg, Code = statusCode, Data = data });
        }
    }
}