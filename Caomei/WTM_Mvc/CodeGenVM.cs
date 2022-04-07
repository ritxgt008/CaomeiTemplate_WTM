using Caomei.Core;
using Caomei.Core.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Caomei.Mvc
{
    public enum ApiAuthMode
    {
        [Display(Name = "Both Jwt and Cookie")]
        Both,

        [Display(Name = "Jwt")]
        Jwt,

        [Display(Name = "Cookie")]
        Cookie
    }

    [ReInit(ReInitModes.ALWAYS)]
    public class CodeGenVM : BaseVM
    {
        public CodeGenListVM FieldList { get; set; }
        public List<CodeGenListView> CodeGenFieldList { get; set; }

        public List<FieldInfo> FieldInfos { get; set; }

        public string PreviewFile { get; set; }

        public UIEnum UI { get; set; }

        [Display(Name = "Codegen.GenApi")]
        public bool IsApi { get; set; }

        [Display(Name = "Codegen.AuthMode")]
        public ApiAuthMode AuthMode { get; set; }

        public string ModelName
        {
            get
            {
                return SelectedModel?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault() ?? "";
            }
        }

        [Display(Name = "Codegen.ModelNS")]
        [ValidateNever()]
        public string ModelNS => SelectedModel?.Split(',').FirstOrDefault()?.Split('.').SkipLast(1).ToSepratedString(seperator: ".");

        [Display(Name = "Codegen.ModuleName")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string ModuleName { get; set; }

        [RegularExpression("^[A-Za-z_]+", ErrorMessage = "Codegen.EnglishOnly")]
        public string Area { get; set; }

        [ValidateNever()]
        [BindNever()]
        public List<ComboSelectListItem> AllModels { get; set; }

        [Required(ErrorMessage = "Validate.{0}required")]
        [Display(Name = "Sys.SelectedModel")]
        public string SelectedModel { get; set; }

        [ValidateNever()]
        public string EntryDir { get; set; }

        public string _mainDir;

        [ValidateNever()]
        public string MainDir
        {
            get
            {
                if (_mainDir == null)
                {
                    int? index = EntryDir?.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}Debug{Path.DirectorySeparatorChar}");
                    if (index == null || index < 0)
                    {
                        index = EntryDir?.IndexOf($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}") ?? 0;
                    }

                    _mainDir = EntryDir?.Substring(0, index.Value);
                }
                return _mainDir;
            }
            set
            {
                _mainDir = value;
            }
        }

        public string _vmdir;

        [ValidateNever()]
        public string VmDir
        {
            get
            {
                if (_vmdir == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".viewmodel")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            vmdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}ViewModels{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Areas{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}ViewModels{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                        else
                        {
                            vmdir = Directory.CreateDirectory(vmdir.FullName + $"{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}{ModelName}VMs");
                        }
                    }
                    _vmdir = vmdir.FullName;
                }
                return _vmdir;
            }
        }

        public string _sharedir;

        [ValidateNever()]
        public string ShareDir
        {
            get
            {
                if (_sharedir == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var sharedir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".shared")).FirstOrDefault();
                    if (string.IsNullOrEmpty(Area))
                    {
                        sharedir = Directory.CreateDirectory(sharedir.FullName + $"{Path.DirectorySeparatorChar}Pages{Path.DirectorySeparatorChar}{ModelName}");
                    }
                    else
                    {
                        sharedir = Directory.CreateDirectory(sharedir.FullName + $"{Path.DirectorySeparatorChar}Pages{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}{ModelName}");
                    }

                    _sharedir = sharedir.FullName;
                }
                return _sharedir;
            }
        }

        public string _vuedir;

        [ValidateNever()]
        public string VueDir
        {
            get
            {
                if (_vuedir == null)
                {
                    var aera = string.IsNullOrEmpty(Area) ? "" : "_"+Area.ToLower();
                    var path = $"{MainDir}{Path.DirectorySeparatorChar}ClientApp{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}pages{Path.DirectorySeparatorChar}{aera}{Path.DirectorySeparatorChar}{ModelName.ToLower()}{Path.DirectorySeparatorChar}";
                    if (Directory.Exists($"{path}") == false)
                    {
                        Directory.CreateDirectory($"{path}");
                    }
                    if (Directory.Exists($"{path}views") == false)
                    {
                        Directory.CreateDirectory($"{path}views");
                    }
                    if (Directory.Exists($"{path}controller") == false)
                    {
                        Directory.CreateDirectory($"{path}controller");
                    }
                    if (Directory.Exists($"{path}views{Path.DirectorySeparatorChar}widgets") == false)
                    {
                        Directory.CreateDirectory($"{path}views{Path.DirectorySeparatorChar}widgets");
                    }

                    _vuedir = path;
                }
                return _vuedir;
            }
        }

        public string _testdir;

        [ValidateNever()]
        public string TestDir
        {
            get
            {
                if (_testdir == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var testdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".test")).FirstOrDefault();
                    _testdir = testdir?.FullName;
                }
                return _testdir;
            }
        }

        public string _controllerdir;

        [ValidateNever()]
        public string ControllerDir
        {
            get
            {
                if (_controllerdir == null)
                {
                    if (string.IsNullOrEmpty(Area))
                    {
                        _controllerdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Controllers").FullName;
                    }
                    else
                    {
                        _controllerdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Areas{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}Controllers").FullName;
                    }
                }
                return _controllerdir;
            }
        }

        public string _viewdir;

        [ValidateNever()]
        public string ViewDir
        {
            get
            {
                if (_viewdir == null)
                {
                    if (string.IsNullOrEmpty(Area))
                    {
                        _viewdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}{ModelName}").FullName;
                    }
                    else
                    {
                        _viewdir = Directory.CreateDirectory(MainDir + $"{Path.DirectorySeparatorChar}Areas{Path.DirectorySeparatorChar}{Area}{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}{ModelName}").FullName;
                    }
                }
                return _viewdir;
            }
        }

        private string _mainNs;

        public string MainNS
        {
            get
            {
                int index = MainDir.LastIndexOf(Path.DirectorySeparatorChar);
                if (index > 0)
                {
                    _mainNs = MainDir[(index + 1)..];
                }
                else
                {
                    _mainNs = MainDir;
                }
                return _mainNs;
            }
            set
            {
                _mainNs = value;
            }
        }

        private string _controllerNs;

        [Display(Name = "Codegen.ControllerNs")]
        [ValidateNever()]
        public string ControllerNs
        {
            get
            {
                var area = string.IsNullOrWhiteSpace(Area) ? "" : "."+Area;

                if (_controllerNs == null)
                {
                    _controllerNs = MainNS +area+ ".Controllers";
                }
                return _controllerNs;
            }
            set
            {
                _controllerNs = value;
            }
        }

        private string _testNs;

        [Display(Name = "Codegen.TestNs")]
        [ValidateNever()]
        public string TestNs
        {
            get
            {
                if (_testNs == null)
                {
                    _testNs = MainNS + ".Test";
                }
                return _testNs;
            }
            set
            {
                _testNs = value;
            }
        }

        private string _dataNs;

        [Display(Name = "Codegen.DataNs")]
        [ValidateNever()]
        public string DataNs
        {
            get
            {
                if (_dataNs == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".dataaccess")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        _dataNs = MainNS;
                    }
                    else
                    {
                        _dataNs = MainNS + ".DataAccess"; ;
                    }
                }
                return _dataNs;
            }
            set
            {
                _dataNs = value;
            }
        }

        private string _vmNs;

        [Display(Name = "Codegen.VMNs")]
        [ValidateNever()]
        public string VMNs
        {
            get
            {
                if (_vmNs == null)
                {
                    var up = Directory.GetParent(MainDir);
                    var vmdir = up.GetDirectories().Where(x => x.Name.ToLower().EndsWith(".viewmodel")).FirstOrDefault();
                    if (vmdir == null)
                    {
                        if (string.IsNullOrEmpty(Area))
                        {
                            _vmNs = MainNS + $".ViewModels.{ModelName}VMs";
                        }
                        else
                        {
                            _vmNs = MainNS + $".{Area}.ViewModels.{ModelName}VMs";
                        }
                    }
                    else
                    {
                        int index = vmdir.FullName.LastIndexOf(Path.DirectorySeparatorChar);
                        if (index > 0)
                        {
                            _vmNs = vmdir.FullName[(index + 1)..];
                        }
                        else
                        {
                            _vmNs = vmdir.FullName;
                        }
                        if (string.IsNullOrEmpty(Area))
                        {
                            _vmNs += $".{ModelName}VMs";
                        }
                        else
                        {
                            _vmNs += $".{Area}.{ModelName}VMs";
                        }
                    }
                }
                return _vmNs;
            }
            set
            {
                _vmNs = value;
            }
        }

        [ValidateNever()]
        public BlazorAndVueEnum BlazorAndVue { get; set; }

        public enum BlazorAndVueEnum
        { VUE, Blazor }

        protected override void InitVM()
        {
            //if (string.IsNullOrEmpty(SelectedModel) == false)
            //{
            //    foreach (var item in ConfigInfo.Connections)
            //    {
            //        var dc = item.CreateDC();
            //        Type t = typeof(DbSet<>).MakeGenericType(Type.GetType(SelectedModel));
            //        var exist = dc.GetType().GetSingleProperty(x => x.PropertyType == t);
            //        if (exist != null)
            //        {
            //            this.DC = dc;
            //        }
            //    }
            //}

            //FieldList = new CodeGenListVM { DC=this.DC };
            //FieldList.CopyContext(this);
        }

        public void SetDC()
        {
            if (string.IsNullOrEmpty(SelectedModel) == false)
            {
                foreach (var item in ConfigInfo.Connections)
                {
                    var dc = item.CreateDC();
                    Type t = typeof(DbSet<>).MakeGenericType(Type.GetType(SelectedModel));
                    var exist = dc.GetType().GetSingleProperty(x => x.PropertyType == t);
                    if (exist != null)
                    {
                        this.DC = dc;
                    }
                }
            }
        }

        public void DoGen()
        {
            SetDC();
            File.WriteAllText($"{ControllerDir}{Path.DirectorySeparatorChar}_{ModelName}{(IsApi == true ? "Api" : "")}Controller.cs", GenerateController(), Encoding.UTF8);

            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}VM.cs", GenerateVM("CrudVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}ListVM.cs", GenerateVM("ListVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}BatchVM.cs", GenerateVM("BatchVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}ImportVM.cs", GenerateVM("ImportVM"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}Searcher.cs", GenerateVM("Searcher"), Encoding.UTF8);
            File.WriteAllText($"{VmDir}{Path.DirectorySeparatorChar}{ModelName}{(IsApi == true ? "Api" : "")}BatchVM.cs", GenerateVM("BatchVM"), Encoding.UTF8);

            if (IsApi == false)
            {
                if (UI == UIEnum.LayUI)
                {
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Index.cshtml", GenerateView("ListView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Create.cshtml", GenerateView("CreateView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Edit.cshtml", GenerateView("EditView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Delete.cshtml", GenerateView("DeleteView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Details.cshtml", GenerateView("DetailsView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}Import.cshtml", GenerateView("ImportView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}BatchEdit.cshtml", GenerateView("BatchEditView"), Encoding.UTF8);
                    File.WriteAllText($"{ViewDir}{Path.DirectorySeparatorChar}BatchDelete.cshtml", GenerateView("BatchDeleteView"), Encoding.UTF8);
                }
                if (UI == UIEnum.VUE||UI==UIEnum.BlazorAndVue)
                {
                    List<string> apipneeded = new List<string>();
                    File.WriteAllText(contents: GenerateVUEView("locales", apipneeded),
                       path: $"{VueDir}locales.ts", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("index", apipneeded),
                       path: $"{VueDir}index.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("controller.entity", apipneeded),
                       path: $"{VueDir}controller{Path.DirectorySeparatorChar}entity.ts", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("controller.index", apipneeded),
                       path: $"{VueDir}controller{Path.DirectorySeparatorChar}index.ts", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.batchEdit", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}batchEdit.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.create", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}create.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.details", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}details.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.edit", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}edit.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.import", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}import.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.widgets.actionViewIndex", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}widgets{Path.DirectorySeparatorChar}actionViewIndex.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.widgets.batchFormViewBatchEdit", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}widgets{Path.DirectorySeparatorChar}batchFormViewBatchEdit.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.widgets.filterViewIndexSearcher", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}widgets{Path.DirectorySeparatorChar}filterViewIndexSearcher.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.widgets.formViewCreateAdd", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}widgets{Path.DirectorySeparatorChar}formViewCreateAdd.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.widgets.formViewDetailsDetail", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}widgets{Path.DirectorySeparatorChar}formViewDetailsDetail.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.widgets.formViewEditEdit", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}widgets{Path.DirectorySeparatorChar}formViewEditEdit.vue", encoding: Encoding.UTF8);
                    File.WriteAllText(contents: GenerateVUEView("views.widgets.gridViewIndex", apipneeded),
                       path: $"{VueDir}views{Path.DirectorySeparatorChar}widgets{Path.DirectorySeparatorChar}gridViewIndex.vue", encoding: Encoding.UTF8);
                }

                if (UI == UIEnum.Blazor||UI==UIEnum.BlazorAndVue)
                {
                    File.WriteAllText($"{ShareDir}{Path.DirectorySeparatorChar}Index.razor", GenerateBlazorView("Index"), Encoding.UTF8);
                    File.WriteAllText($"{ShareDir}{Path.DirectorySeparatorChar}Create.razor", GenerateBlazorView("Create"), Encoding.UTF8);
                    File.WriteAllText($"{ShareDir}{Path.DirectorySeparatorChar}Edit.razor", GenerateBlazorView("Edit"), Encoding.UTF8);
                    File.WriteAllText($"{ShareDir}{Path.DirectorySeparatorChar}Details.razor", GenerateBlazorView("Details"), Encoding.UTF8);
                    File.WriteAllText($"{ShareDir}{Path.DirectorySeparatorChar}Import.razor", GenerateBlazorView("Import"), Encoding.UTF8);
                    File.WriteAllText($"{ShareDir}{Path.DirectorySeparatorChar}BatchEdit.razor", GenerateBlazorView("BatchEdit"), Encoding.UTF8);
                }
            }
            var test = GenerateTest();
            if (test != "")
            {
                if (UI == UIEnum.LayUI && IsApi == false)
                {
                    File.WriteAllText($"{TestDir}{Path.DirectorySeparatorChar}{ModelName}ControllerTest.cs", test, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllText($"{TestDir}{Path.DirectorySeparatorChar}{ModelName}ApiTest.cs", test, Encoding.UTF8);
                }
            }
        }

        public string GenerateController()
        {
            var sqlkey = ConfigInfo.Connections.Where(x => x.Value==DC.CSName).First().Key;
            var FixConnection = sqlkey=="defalut" ? "" : $"[FixConnection(DBOperationEnum.Read, CsName = \"{sqlkey}\")]";//$FixConnection$
            string dir = "";
            string jwt = "";
            if (UI == UIEnum.LayUI && IsApi == false)
            {
                dir = "Mvc";
            }
            else
            {
                dir = "Spa";
                if (UI == UIEnum.Blazor)
                {
                    dir = "Spa.Blazor";
                }
                switch (AuthMode)
                {
                    case ApiAuthMode.Both:
                        jwt = "[AuthorizeJwtWithCookie]";
                        break;

                    case ApiAuthMode.Jwt:
                        jwt = "[AuthorizeJwt]";
                        break;

                    case ApiAuthMode.Cookie:
                        jwt = "[AuthorizeCookie]";
                        break;

                    default:
                        break;
                }
            }
            var rv = GetResource("Controller.txt", dir).Replace("$jwt$", jwt).Replace("$vmnamespace$", VMNs).Replace("$namespace$", ControllerNs).Replace("$des$", ModuleName).Replace("$modelname$", ModelName).Replace("$modelnamespace$", ModelNS).Replace("$controllername$", $"{ModelName}{(IsApi == true ? "Api" : "")}").Replace("$FixConnection$", FixConnection);
            if (string.IsNullOrEmpty(Area))
            {
                rv = rv.Replace("$area$", "/");
            }
            else
            {
                rv = rv.Replace("$area$", $"/_{Area}/");
            }
            //生成api中获取下拉菜单数据的api
            //如果一个一对多关联其他类的字段是搜索条件或者表单字段，则生成对应的获取关联表数据的api
            if (UI != UIEnum.LayUI || IsApi == true)
            {
                StringBuilder other = new StringBuilder();
                List<FieldInfo> pros = FieldInfos.Where(x => x.IsSearcherField == true || x.IsFormField == true).ToList();
                List<string> existSubPro = new List<string>();
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    if ((item.InfoType == FieldInfoType.One2Many || item.InfoType == FieldInfoType.Many2Many) && item.SubField != "`file")
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        var subpro = subtype.GetSingleProperty(item.SubField);
                        var key = subtype.FullName + ":" + subpro.Name;
                        existSubPro.Add(key);
                        int count = existSubPro.Where(x => x == key).Count();
                        if (count == 1)
                        {
                            other.AppendLine($@"
        [HttpGet(""Get{subtype.Name}s"")]
        public ActionResult Get{subtype.Name}s()
        {{
            return Ok(DC.Set<{subtype.Name}>().GetSelectListItems(Wtm, x => x.{item.SubField}));
        }}");
                        }
                    }
                }
                rv = rv.Replace("$other$", other.ToString());
                rv = GetRelatedNamespace(pros, rv);
            }
            return rv;
        }

        public string GenerateVM(string name)
        {
            var rv = GetResource($"{name}.txt").Replace("$modelnamespace$", ModelNS).Replace("$vmnamespace$", VMNs).Replace("$modelname$", ModelName).Replace("$area$", $"{Area ?? ""}").Replace("$classname$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
            if (name == "Searcher" || name == "BatchVM")
            {
                string prostring = "";
                string initstr = "";
                Type modelType = Type.GetType(SelectedModel);
                List<FieldInfo> pros = null;
                if (name == "Searcher")
                {
                    pros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                }
                if (name == "BatchVM")
                {
                    pros = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                }
                foreach (var pro in pros)
                {
                    //对于一对一或者一对多的搜索和批量修改字段，需要在vm中生成对应的变量来获取关联表的数据
                    if (pro.InfoType != FieldInfoType.Normal)
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                        if (UI == UIEnum.LayUI && IsApi == false)
                        {
                            var fname = "All" + pro.FieldName + "s";
                            prostring += $@"
        public List<ComboSelectListItem> {fname} {{ get; set; }}";
                            initstr += $@"
            {fname} = DC.Set<{subtype.Name}>().GetSelectListItems(Wtm, y => y.{pro.SubField});";
                        }
                    }

                    //生成普通字段定义
                    var proType = modelType.GetSingleProperty(pro.FieldName);
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    string typename = proType.PropertyType.Name;
                    string proname = pro.GetField(DC, modelType);

                    switch (pro.InfoType)
                    {
                        case FieldInfoType.Normal:
                            if (proType.PropertyType.IsNullable())
                            {
                                typename = proType.PropertyType.GetGenericArguments()[0].Name + "?";
                            }
                            else if (proType.PropertyType.IsBool())
                            {
                            }
                            else if (proType.PropertyType != typeof(string))
                            {
                                typename = proType.PropertyType.Name + "?";
                            }
                            break;

                        case FieldInfoType.One2Many:
                            typename = pro.GetFKType(DC, modelType);
                            if (typename != "string")
                            {
                                typename += "?";
                            }
                            break;

                        case FieldInfoType.Many2Many:
                            proname = $@"Selected{pro.FieldName}IDs";
                            typename = $"List<{pro.GetFKType(DC, modelType)}>";
                            break;

                        default:
                            break;
                    }
                    if ((typename == "DateTime" || typename == "DateTime?") && name == "Searcher")
                    {
                        typename = "DateRange";
                    }
                    prostring += $@"
        public {typename} {proname} {{ get; set; }}";
                }
                rv = rv.Replace("$pros$", prostring).Replace("$init$", initstr);
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "ListVM")
            {
                string headerstring = "";
                string selectstring = "";
                string wherestring = "";
                string subprostring = "";
                string formatstring = "";
                string actionstring = "";

                if (UI == UIEnum.LayUI && IsApi == false)
                {
                    actionstring = $@"
        protected override List<GridAction> InitGridAction()
        {{
            return new List<GridAction>
            {{
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Create, Localizer[""Sys.Create""],""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Edit, Localizer[""Sys.Edit""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Delete, Localizer[""Sys.Delete""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Details, Localizer[""Sys.Details""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.BatchEdit, Localizer[""Sys.BatchEdit""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.BatchDelete, Localizer[""Sys.BatchDelete""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.Import, Localizer[""Sys.Import""], ""{Area ?? ""}"", dialogWidth: 800),
                this.MakeStandardAction(""{ModelName}"", GridActionStandardTypesEnum.ExportExcel, Localizer[""Sys.Export""], ""{Area ?? ""}""),
            }};
        }}
";
                }

                var pros = FieldInfos.Where(x => x.IsListField == true).ToList();
                Type modelType = Type.GetType(SelectedModel);
                List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                foreach (var pro in pros)
                {
                    if (pro.InfoType == FieldInfoType.Normal)
                    {
                        headerstring += $@"
                this.MakeGridHeader(x => x.{pro.FieldName}),";
                        if (pro.FieldName.ToLower() != "id")
                        {
                            selectstring += $@"
                    {pro.FieldName} = x.{pro.FieldName},";
                        }
                    }
                    else
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (subtype == typeof(FileAttachment))
                        {
                            var filefk = DC.GetFKName2(modelType, pro.FieldName);
                            headerstring += $@"
                this.MakeGridHeader(x => x.{filefk}).SetFormat({filefk}Format),";
                            selectstring += $@"
                    {filefk} = x.{filefk},";
                            formatstring += GetResource("HeaderFormat.txt").Replace("$modelname$", ModelName).Replace("$field$", filefk).Replace("$classname$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
                        }
                        else
                        {
                            var subpro = subtype.GetSingleProperty(pro.SubField);
                            existSubPro.Add(subpro);
                            string prefix = "";
                            int count = existSubPro.Where(x => x.Name == subpro.Name).Count();
                            if (count > 1)
                            {
                                prefix = count + "";
                            }
                            string subtypename = subpro.PropertyType.Name;
                            if (subpro.PropertyType.IsNullable())
                            {
                                subtypename = subpro.PropertyType.GetGenericArguments()[0].Name + "?";
                            }

                            var subdisplay = subpro.GetCustomAttribute<DisplayAttribute>();
                            headerstring += $@"
                this.MakeGridHeader(x => x.{pro.SubField + "_view" + prefix}),";
                            if (pro.InfoType == FieldInfoType.One2Many)
                            {
                                selectstring += $@"
                    {pro.SubField + "_view" + prefix} = x.{pro.FieldName}.{pro.SubField},";
                            }
                            else
                            {
                                var middleType = modelType.GetSingleProperty(pro.FieldName).PropertyType.GenericTypeArguments[0];
                                var middlename = DC.GetPropertyNameByFk(middleType, pro.SubIdField);
                                if (typeof(IPersistPoco).IsAssignableFrom(Type.GetType(pro.RelatedField)))
                                {
                                    selectstring += $@"
                    {pro.SubField + "_view" + prefix} = x.{pro.FieldName}.Where(y=>y.{middlename}.IsValid==true).Select(y=>y.{middlename}.{pro.SubField}).ToSepratedString(null,"",""), ";
                                }
                                else
                                {
                                    selectstring += $@"
                    {pro.SubField + "_view" + prefix} = x.{pro.FieldName}.Select(y=>y.{middlename}.{pro.SubField}).ToSepratedString(null,"",""), ";
                                }
                            }
                            if (subdisplay?.Name != null)
                            {
                                subprostring += $@"
        [Display(Name = ""{subdisplay.Name}"")]";
                            }
                            subprostring += $@"
        public {subtypename} {pro.SubField + "_view" + prefix} {{ get; set; }}";
                        }
                    }
                }
                var wherepros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                foreach (var pro in wherepros)
                {
                    if (pro.SubField == "`file")
                    {
                        continue;
                    }
                    var proType = modelType.GetSingleProperty(pro.FieldName)?.PropertyType;

                    switch (pro.InfoType)
                    {
                        case FieldInfoType.Normal:
                            if (proType == typeof(string))
                            {
                                wherestring += $@"
                .CheckContain(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";
                            }
                            else if (proType == typeof(DateTime) || proType == typeof(DateTime?))
                            {
                                wherestring += $@"
                .CheckBetween(Searcher.{pro.FieldName}?.GetStartTime(), Searcher.{pro.FieldName}?.GetEndTime(), x => x.{pro.FieldName}, includeMax: false)";
                            }
                            else
                            {
                                wherestring += $@"
                .CheckEqual(Searcher.{pro.FieldName}, x=>x.{pro.FieldName})";
                            }
                            break;

                        case FieldInfoType.One2Many:
                            var fk = DC.GetFKName2(modelType, pro.FieldName);
                            wherestring += $@"
                .CheckEqual(Searcher.{fk}, x=>x.{fk})";
                            break;

                        case FieldInfoType.Many2Many:
                            var subtype = Type.GetType(pro.RelatedField);
                            var fk2 = DC.GetFKName(modelType, pro.FieldName);
                            wherestring += $@"
                .CheckWhere(Searcher.Selected{pro.FieldName}IDs,x=>DC.Set<{proType.GetGenericArguments()[0].Name}>().Where(y=>Searcher.Selected{pro.FieldName}IDs.Contains(y.{pro.SubIdField})).Select(z=>z.{fk2}).Contains(x.ID))";
                            break;

                        default:
                            break;
                    }
                }
                rv = rv.Replace("$headers$", headerstring).Replace("$where$", wherestring).Replace("$select$", selectstring).Replace("$subpros$", subprostring).Replace("$format$", formatstring).Replace("$actions$", actionstring);
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "CrudVM")
            {
                string prostr = "";
                string initstr = "";
                string includestr = "";
                string addstr = "";
                string editstr = "";
                var pros = FieldInfos.Where(x => x.IsFormField == true && string.IsNullOrEmpty(x.RelatedField) == false).ToList();
                foreach (var pro in pros)
                {
                    var subtype = Type.GetType(pro.RelatedField);
                    if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                    {
                        continue;
                    }
                    var fname = "All" + pro.FieldName + "s";
                    if (UI == UIEnum.LayUI)
                    {
                        prostr += $@"
        public List<ComboSelectListItem> {fname} {{ get; set; }}";
                        initstr += $@"
            {fname} = DC.Set<{subtype.Name}>().GetSelectListItems(Wtm, y => y.{pro.SubField});";
                    }
                    includestr += $@"
            SetInclude(x => x.{pro.FieldName});";

                    if (pro.InfoType == FieldInfoType.Many2Many)
                    {
                        Type modelType = Type.GetType(SelectedModel);
                        var protype = modelType.GetSingleProperty(pro.FieldName);
                        prostr += $@"
        [Display(Name = ""{protype.GetPropertyDisplayName()}"")]
        public List<string> Selected{pro.FieldName}IDs {{ get; set; }}";
                        initstr += $@"
            Selected{pro.FieldName}IDs = Entity.{pro.FieldName}?.Select(x => x.{pro.SubIdField}.ToString()).ToList();";
                        addstr += $@"
            Entity.{pro.FieldName} = new List<{protype.PropertyType.GetGenericArguments()[0].Name}>();
            if (Selected{pro.FieldName}IDs != null)
            {{
                foreach (var id in Selected{pro.FieldName}IDs)
                {{
                     {protype.PropertyType.GetGenericArguments()[0].Name} middle = new {protype.PropertyType.GetGenericArguments()[0].Name}();
                    middle.SetPropertyValue(""{pro.SubIdField}"", id);
                    Entity.{pro.FieldName}.Add(middle);
                }}
            }}
";
                        editstr += $@"
            Entity.{pro.FieldName} = new List<{protype.PropertyType.GetGenericArguments()[0].Name}>();
            if(Selected{pro.FieldName}IDs != null )
            {{
                 foreach (var item in Selected{pro.FieldName}IDs)
                {{
                    {protype.PropertyType.GetGenericArguments()[0].Name} middle = new {protype.PropertyType.GetGenericArguments()[0].Name}();
                    middle.SetPropertyValue(""{pro.SubIdField}"", item);
                    Entity.{pro.FieldName}.Add(middle);
                }}
            }}
";
                    }
                }
                if ((UI == UIEnum.LayUI && IsApi == false) || UI == UIEnum.Blazor)
                {
                    rv = rv.Replace("$pros$", prostr).Replace("$init$", initstr).Replace("$include$", includestr).Replace("$add$", addstr).Replace("$edit$", editstr);
                }
                else
                {
                    rv = rv.Replace("$pros$", "").Replace("$init$", "").Replace("$include$", includestr).Replace("$add$", "").Replace("$edit$", "");
                }
                rv = GetRelatedNamespace(pros, rv);
            }
            if (name == "ImportVM")
            {
                string prostring = "";
                string initstr = "";
                Type modelType = Type.GetType(SelectedModel);
                List<FieldInfo> pros = FieldInfos.Where(x => x.IsImportField == true).ToList();
                foreach (var pro in pros)
                {
                    if (pro.InfoType == FieldInfoType.Many2Many)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(pro.RelatedField) == false)
                    {
                        var subtype = Type.GetType(pro.RelatedField);
                        if (typeof(TopBasePoco).IsAssignableFrom(subtype) == false || subtype == typeof(FileAttachment))
                        {
                            continue;
                        }
                        initstr += $@"
            {pro.FieldName + "_Excel"}.DataType = ColumnDataType.ComboBox;
            {pro.FieldName + "_Excel"}.ListItems = DC.Set<{subtype.Name}>().GetSelectListItems(Wtm, y => y.{pro.SubField});";
                    }
                    var proType = modelType.GetSingleProperty(pro.FieldName);
                    var display = proType.GetCustomAttribute<DisplayAttribute>();
                    var filefk = DC.GetFKName2(modelType, pro.FieldName);
                    if (display != null)
                    {
                        prostring += $@"
        [Display(Name = ""{display.Name}"")]";
                    }
                    if (string.IsNullOrEmpty(pro.RelatedField) == false)
                    {
                        prostring += $@"
        public ExcelPropety {pro.FieldName + "_Excel"} = ExcelPropety.CreateProperty<{ModelName}>(x => x.{filefk});";
                    }
                    else
                    {
                        prostring += $@"
        public ExcelPropety {pro.FieldName + "_Excel"} = ExcelPropety.CreateProperty<{ModelName}>(x => x.{pro.FieldName});";
                    }
                }
                rv = rv.Replace("$pros$", prostring).Replace("$init$", initstr);
                rv = GetRelatedNamespace(pros, rv);
            }
            return rv;
        }

        public string GenerateView(string name)
        {
            var rv = GetResource($"{name}.txt", "Mvc").Replace("$vmnamespace$", VMNs).Replace("$modelname$", ModelName);
            if (name == "CreateView" || name == "EditView" || name == "DeleteView" || name == "DetailsView" || name == "BatchEditView")
            {
                StringBuilder fieldstr = new StringBuilder();
                string pre = "";
                List<FieldInfo> pros = null;
                if (name == "BatchEditView")
                {
                    pros = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                    pre = "LinkedVM";
                }
                else
                {
                    pros = FieldInfos.Where(x => x.IsFormField == true).ToList();
                    pre = "Entity";
                }
                Type modelType = Type.GetType(SelectedModel);
                fieldstr.Append(Environment.NewLine);
                fieldstr.Append(@"<wt:row items-per-row=""ItemsPerRowEnum.Two"">");
                fieldstr.Append(Environment.NewLine);
                foreach (var item in pros)
                {
                    if (name == "DeleteView" || name == "DetailsView")
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            if (string.IsNullOrEmpty(item.RelatedField) == false && item.SubField != "`file")
                            {
                                fieldstr.Append($@"<wt:display field=""{pre}.{item.FieldName}.{item.SubField}"" />");
                            }
                            else
                            {
                                string idname = item.FieldName;
                                if (string.IsNullOrEmpty(item.RelatedField) == false && item.SubField == "`file")
                                {
                                    var filefk = DC.GetFKName2(modelType, item.FieldName);
                                    idname = filefk;
                                }
                                fieldstr.Append($@"<wt:display field=""{pre}.{idname}"" />");
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.RelatedField) == false)
                        {
                            var filefk = DC.GetFKName2(modelType, item.FieldName);
                            if (item.SubField == "`file")
                            {
                                if (name != "BatchEditView")
                                {
                                    fieldstr.Append($@"<wt:upload field=""{pre}.{filefk}"" />");
                                }
                            }
                            else
                            {
                                var fname = "All" + item.FieldName + "s";
                                if (name == "BatchEditView")
                                {
                                    fname = "LinkedVM." + fname;
                                }
                                if (string.IsNullOrEmpty(item.SubIdField))
                                {
                                    fieldstr.Append($@"<wt:combobox field=""{pre}.{filefk}"" items=""{fname}""/>");
                                }
                                else
                                {
                                    if (name == "BatchEditView")
                                    {
                                        fieldstr.Append($@"<wt:checkbox field=""LinkedVM.Selected{item.FieldName}IDs"" items=""{fname}""/>");
                                    }
                                    else
                                    {
                                        fieldstr.Append($@"<wt:checkbox field=""Selected{item.FieldName}IDs"" items=""{fname}""/>");
                                    }
                                }
                            }
                        }
                        else
                        {
                            var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                            Type checktype = proType;
                            if (proType.IsNullable())
                            {
                                checktype = proType.GetGenericArguments()[0];
                            }
                            if (checktype == typeof(bool) && proType.IsNullable() == false)
                            {
                                fieldstr.Append($@"<wt:switch field=""{pre}.{item.FieldName}"" />");
                            }
                            else if (checktype == typeof(bool) || checktype.IsEnum())
                            {
                                fieldstr.Append($@"<wt:combobox field=""{pre}.{item.FieldName}"" />");
                            }
                            else if (checktype.IsPrimitive || checktype == typeof(string) || checktype == typeof(decimal))
                            {
                                fieldstr.Append($@"<wt:textbox field=""{pre}.{item.FieldName}"" />");
                            }
                            else if (checktype == typeof(DateTime))
                            {
                                fieldstr.Append($@"<wt:datetime field=""{pre}.{item.FieldName}"" />");
                            }
                        }
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                fieldstr.Append("</wt:row>");
                rv = rv.Replace("$fields$", fieldstr.ToString());
            }
            if (name == "ListView")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                Type modelType = Type.GetType(SelectedModel);
                fieldstr.Append(Environment.NewLine);
                fieldstr.Append(@"<wt:row items-per-row=""ItemsPerRowEnum.Three"">");
                fieldstr.Append(Environment.NewLine);
                foreach (var item in pros)
                {
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (item.SubField == "`file")
                        {
                            continue;
                        }
                        var fname = "All" + item.FieldName + "s";
                        var fk = "";
                        if (string.IsNullOrEmpty(item.SubIdField))
                        {
                            fk = DC.GetFKName2(modelType, item.FieldName); ;
                        }
                        else
                        {
                            fk = $@"Selected{item.FieldName}IDs";
                        }
                        fieldstr.Append($@"<wt:combobox field=""Searcher.{fk}"" items=""Searcher.{fname}"" empty-text=""@Localizer[""Sys.All""]"" />");
                    }
                    else
                    {
                        var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if ((checktype.IsPrimitive && checktype != typeof(bool)) || checktype == typeof(string) || checktype == typeof(decimal))
                        {
                            fieldstr.Append($@"<wt:textbox field=""Searcher.{item.FieldName}"" />");
                        }
                        if (checktype == typeof(DateTime))
                        {
                            fieldstr.Append($@"<wt:datetime field=""Searcher.{item.FieldName}"" range=""true"" />");
                        }
                        if (checktype.IsEnum() || checktype.IsBool())
                        {
                            fieldstr.Append($@"<wt:combobox field=""Searcher.{item.FieldName}"" empty-text=""@Localizer[""Sys.All""]"" />");
                        }
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                fieldstr.Append($@"</wt:row>");
                string url = "";
                if (string.IsNullOrEmpty(Area))
                {
                    url = $"/{ModelName}/Search";
                }
                else
                {
                    url = $"/{Area}/{ModelName}/Search";
                }
                rv = rv.Replace("$fields$", fieldstr.ToString()).Replace("$searchurl$", url);
            }
            return rv;
        }

        public string GenerateTest()
        {
            var rv = "";
            if (TestDir != null)
            {
                Type modelType = Type.GetType(SelectedModel);
                if (UI == UIEnum.LayUI && IsApi == false)
                {
                    if (typeof(IBasePoco).IsAssignableFrom(modelType) || typeof(IPersistPoco).IsAssignableFrom(modelType))
                    {
                        rv = GetResource($"ControllerTest.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs);
                    }
                    else
                    {
                        rv = GetResource($"ControllerTestTopPoco.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs);
                    }
                }
                else
                {
                    if (typeof(IBasePoco).IsAssignableFrom(modelType) || typeof(IPersistPoco).IsAssignableFrom(modelType))
                    {
                        rv = GetResource($"ApiTest.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs).Replace("$classnamel$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
                    }
                    else
                    {
                        rv = GetResource($"ApiTestTopPoco.txt").Replace("$cns$", ControllerNs).Replace("$tns$", TestNs).Replace("$vns$", VMNs).Replace("$model$", ModelName).Replace("$mns$", ModelNS).Replace("$dns$", DataNs).Replace("$classnamel$", $"{ModelName}{(IsApi == true ? "Api" : "")}");
                    }
                }
                var modelprops = modelType.GetRandomValues();
                var batchpros = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                string cpros = "";
                string epros = "";
                string pros = "";
                string mpros = "";
                string assert = "";
                string eassert = "";
                string fc = "";
                string add = "";
                string linkedpros = "";
                string linkedfc = "";
                string meassert = "";
                List<Type> addexist = new List<Type>();
                foreach (var pro in modelprops)
                {
                    if (pro.Value == "$fk$")
                    {
                        var fktype = modelType.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                        add += GenerateAddFKModel(pro.Key[0..^2], fktype, addexist);
                    }
                }

                foreach (var pro in modelprops)
                {
                    if (pro.Value == "$fk$")
                    {
                        var fktype = modelType.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                        cpros += $@"
            v.{pro.Key} = Add{fktype.Name}();";
                        pros += $@"
                v.{pro.Key} = Add{fktype.Name}();";
                        mpros += $@"
                v1.{pro.Key} = Add{fktype.Name}();";
                    }
                    else
                    {
                        cpros += $@"
            v.{pro.Key} = {pro.Value};";
                        pros += $@"
                v.{pro.Key} = {pro.Value};";
                        mpros += $@"
                v1.{pro.Key} = {pro.Value};";
                        assert += $@"
                Assert.AreEqual(data.{pro.Key}, {pro.Value});";
                    }
                    fc += $@"
            vm.FC.Add(""Entity.{pro.Key}"", """");";
                }

                var modelpros2 = modelType.GetRandomValues();
                foreach (var pro in modelpros2)
                {
                    //if (pro.Key.ToLower() == "id")
                    //{
                    //    continue;
                    //}

                    if (pro.Value == "$fk$")
                    {
                        mpros += $@"
                v2.{ pro.Key} = v1.{pro.Key}; ";
                    }
                    else
                    {
                        mpros += $@"
                v2.{pro.Key} = {pro.Value};";
                        if (pro.Key.ToLower() != "id")
                        {
                            epros += $@"
            v.{pro.Key} = {pro.Value};";
                            eassert += $@"
                Assert.AreEqual(data.{pro.Key}, {pro.Value});";
                        }
                    }
                }

                var modelpros3 = modelType.GetRandomValues();
                foreach (var pro in modelpros3)
                {
                    if (batchpros.Any(x => x.FieldName == pro.Key) && pro.Key.ToLower() != "id")
                    {
                        linkedpros += $@"
            vm.LinkedVM.{pro.Key} = {pro.Value};";
                        linkedfc += $@"
            vm.FC.Add(""LinkedVM.{pro.Key}"", """");";
                        meassert += $@"
                Assert.AreEqual(data1.{pro.Key}, {pro.Value});";
                        meassert += $@"
                Assert.AreEqual(data2.{pro.Key}, {pro.Value});";
                    }
                }

                string del = $"Assert.AreEqual(data, null);";
                string mdel = @"Assert.AreEqual(data1, null);
            Assert.AreEqual(data2, null);";
                if (typeof(IPersistPoco).IsAssignableFrom(modelType))
                {
                    del = $"Assert.AreEqual(data.IsValid, false);";
                    mdel = @"Assert.AreEqual(data1.IsValid, false);
            Assert.AreEqual(data2.IsValid, false);";
                }

                rv = rv.Replace("$cpros$", cpros).Replace("$epros$", epros).Replace("$pros$", pros).Replace("$mpros$", mpros)
                    .Replace("$assert$", assert).Replace("$eassert$", eassert).Replace("$fc$", fc).Replace("$add$", add).Replace("$del$", del).Replace("$mdel$", mdel)
                    .Replace("$linkedpros$", linkedpros).Replace("$linkedfc$", linkedfc).Replace("$meassert$", meassert);

                rv = GetRelatedNamespace(FieldInfos.Where(x => string.IsNullOrEmpty(x.RelatedField) == false).ToList(), rv);
            }
            return rv;
        }

        private string GenerateAddFKModel(string keyname, Type t, List<Type> exist)
        {
            if (exist == null)
            {
                exist = new List<Type>();
            }
            if (exist.Contains(t) == true)
            {
                return "";
            }
            exist.Add(t);
            var modelprops = t.GetRandomValues();
            var mname = t.Name?.Split(',').FirstOrDefault()?.Split('.').LastOrDefault() ?? "";
            string cpros = "";
            string rv = "";
            foreach (var pro in modelprops)
            {
                if (pro.Value == "$fk$")
                {
                    var fktype = t.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                    if (fktype != t)
                    {
                        rv += GenerateAddFKModel(pro.Key[0..^2], fktype, exist);
                    }
                }
            }

            foreach (var pro in modelprops)
            {
                if (pro.Value == "$fk$")
                {
                    var fktype = t.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                    if (fktype != t)
                    {
                        cpros += $@"
                v.{pro.Key} = Add{fktype.Name}();";
                    }
                }
                else
                {
                    cpros += $@"
                v.{pro.Key} = {pro.Value};";
                }
            }
            var idpro = t.GetSingleProperty("ID");
            rv += $@"
        private {idpro.PropertyType.Name} Add{t.Name}()
        {{
            {mname} v = new {mname}();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {{
                try{{
{cpros}
                context.Set<{mname}>().Add(v);
                context.SaveChanges();
                }}
                catch{{}}
            }}
            return v.ID;
        }}
";
            return rv;
        }

        public string GenerateReactView(string name)
        {
            var rv = GetResource($"{name}.txt", "Spa.React.views")
                .Replace("$modelname$", ModelName.ToLower());
            Type modelType = Type.GetType(SelectedModel);
            if (name == "table")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsListField == true).ToList();
                fieldstr.Append(Environment.NewLine);
                List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                int rowheight = 30;
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    var mpro = modelType.GetSingleProperty(item.FieldName);
                    string label = mpro.GetPropertyDisplayName();
                    string render = "";
                    string newname = item.FieldName;
                    if (mpro.PropertyType.IsBoolOrNullableBool())
                    {
                        render = "columnsRenderBoolean";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        string prefix = "";
                        if (subtype == typeof(FileAttachment))
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon"))
                            {
                                render = "columnsRenderImg";
                                rowheight = 110;
                            }
                            else
                            {
                                render = "columnsRenderDownload";
                            }
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            newname = fk;
                        }
                        else
                        {
                            var subpro = subtype.GetSingleProperty(item.SubField);
                            existSubPro.Add(subpro);
                            int count = existSubPro.Where(x => x.Name == subpro.Name).Count();
                            if (count > 1)
                            {
                                prefix = count + "";
                            }
                            newname = item.SubField + "_view" + prefix;
                        }
                    }
                    fieldstr.Append($@"
    {{
        field: ""{newname}"",
        headerName: ""{label}""");

                    if (render != "")
                    {
                        fieldstr.Append($@",
        cellRenderer: ""{render}"" ");
                    }
                    fieldstr.Append($@"
    }}");
                    if (i < pros.Count - 1)
                    {
                        fieldstr.Append(',');
                    }
                    fieldstr.Append(Environment.NewLine);
                }
                return rv.Replace("$columns$", fieldstr.ToString()).Replace("$rowheight$", rowheight.ToString());
            }
            if (name == "models")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder fieldstr2 = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsFormField == true).ToList();
                var pros2 = FieldInfos.Where(x => x.IsSearcherField == true).ToList();

                //生成表单model
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    var property = modelType.GetSingleProperty(item.FieldName);
                    string label = property.GetPropertyDisplayName();
                    bool isrequired = property.IsPropertyRequired();
                    var fktest = DC.GetFKName2(modelType, item.FieldName);
                    if (string.IsNullOrEmpty(fktest) == false)
                    {
                        isrequired = modelType.GetSingleProperty(fktest).IsPropertyRequired();
                    }
                    string rules = "rules: []";
                    if (isrequired == true)
                    {
                        rules = $@"rules: [{{ ""required"": true, ""message"": <FormattedMessage id='tips.error.required' values={{{{ txt: getLocalesValue('{label}') }}}} /> }}]";
                    }
                    fieldstr.AppendLine($@"            /** {label} */");
                    if (string.IsNullOrEmpty(item.RelatedField) == false && string.IsNullOrEmpty(item.SubIdField) == true)
                    {
                        var fk = DC.GetFKName2(modelType, item.FieldName);
                        fieldstr.AppendLine($@"            ""Entity.{fk}"":{{");
                    }
                    else
                    {
                        fieldstr.AppendLine($@"            ""Entity.{item.FieldName}"":{{");
                    }
                    fieldstr.AppendLine($@"                label: ""{label}"",");
                    fieldstr.AppendLine($@"                {rules},");
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (item.SubField == "`file")
                        {
                            fieldstr.AppendLine($@"                formItem: <WtmUploadImg />");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.SubIdField) == true)
                            {
                                fieldstr.AppendLine($@"                formItem: <WtmSelect placeholder=""{label}""
                    dataSource ={{ Request.cache({{ url: ""/api/{ModelName}/Get{subtype.Name}s"" }})}}
                /> ");
                            }
                            else
                            {
                                fieldstr.AppendLine($@"                formItem: <WtmTransfer
                    listStyle={{undefined}}
                    dataSource={{Request.cache({{ url: ""/api/{ModelName}/Get{subtype.Name}s"" }})}}
                    mapKey=""{item.SubIdField}""
                /> ");
                            }
                        }
                    }
                    else
                    {
                        var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            fieldstr.AppendLine($@"                formItem: <Switch checkedChildren={{<Icon type=""check"" />}} unCheckedChildren={{<Icon type=""close"" />}} />");
                        }
                        else if (checktype.IsEnum())
                        {
                            var es = checktype.ToListItems();
                            fieldstr.AppendLine($@"                formItem: <WtmSelect placeholder=""{label}"" dataSource={{[  ");
                            for (int a = 0; a < es.Count; a++)
                            {
                                var e = es[a];
                                fieldstr.Append($@"                    {{ Text: ""{e.Text}"", Value: ""{e.Value}"" }}");
                                if (a < es.Count - 1)
                                {
                                    fieldstr.Append(',');
                                }
                                fieldstr.AppendLine();
                            }
                            fieldstr.AppendLine($@"                ]}}/>");
                        }
                        else if (checktype.IsNumber())
                        {
                            fieldstr.AppendLine($@"                formItem: <InputNumber placeholder={{getLocalesTemplate('tips.placeholder.input', {{ txt: getLocalesValue('{label}') }})}} />");
                        }
                        else if (checktype == typeof(string))
                        {
                            fieldstr.AppendLine($@"                formItem: <Input placeholder={{getLocalesTemplate('tips.placeholder.input', {{ txt: getLocalesValue('{label}') }})}} />");
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            fieldstr.AppendLine($@"                formItem: <WtmDatePicker placeholder={{getLocalesTemplate('tips.placeholder.input', {{ txt: getLocalesValue('{label}') }})}} />");
                        }
                    }
                    fieldstr.Append("            }");
                    if (i < pros.Count - 1)
                    {
                        fieldstr.Append(',');
                    }
                    fieldstr.Append(Environment.NewLine);
                }

                //生成searchmodel
                for (int i = 0; i < pros2.Count; i++)
                {
                    var item = pros2[i];
                    if (item.SubField == "`file")
                    {
                        continue;
                    }
                    var property = modelType.GetSingleProperty(item.FieldName);
                    string label = property.GetPropertyDisplayName();
                    string rules = "rules: []";

                    fieldstr2.AppendLine($@"            /** {label} */");
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            fieldstr2.AppendLine($@"            ""{fk}"":{{");
                        }
                        else
                        {
                            fieldstr2.AppendLine($@"            ""Selected{item.FieldName}IDs"":{{");
                        }
                    }
                    else
                    {
                        fieldstr2.AppendLine($@"            ""{item.FieldName}"":{{");
                    }
                    fieldstr2.AppendLine($@"                label: ""{label}"",");
                    fieldstr2.AppendLine($@"                {rules},");
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            fieldstr2.AppendLine($@"                formItem: <WtmSelect placeholder={{getLocalesValue('tips.all')}}
                    dataSource ={{ Request.cache({{ url: ""/api/{ModelName}/Get{subtype.Name}s"" }})}}
                /> ");
                        }
                        else
                        {
                            fieldstr2.AppendLine($@"                formItem: <WtmSelect placeholder={{getLocalesValue('tips.all')}}  multiple
                    dataSource ={{ Request.cache({{ url: ""/api/{ModelName}/Get{subtype.Name}s"" }})}}
                /> ");
                        }
                    }
                    else
                    {
                        var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            fieldstr2.AppendLine($@"                formItem: <WtmSelect placeholder={{getLocalesValue('tips.all')}}  dataSource={{[
                    {{ Text: <FormattedMessage id='tips.bool.true' />, Value: true }},{{ Text: <FormattedMessage id='tips.bool.false' />, Value: false }}
                ]}}/>");
                        }
                        else if (checktype.IsEnum())
                        {
                            var es = checktype.ToListItems();
                            fieldstr2.AppendLine($@"                formItem: <WtmSelect placeholder={{getLocalesValue('tips.all')}} dataSource={{[  ");
                            for (int a = 0; a < es.Count; a++)
                            {
                                var e = es[a];
                                fieldstr2.Append($@"                    {{ Text: ""{e.Text}"", Value: ""{e.Value}"" }}");
                                if (a < es.Count - 1)
                                {
                                    fieldstr2.Append(',');
                                }
                                fieldstr2.AppendLine();
                            }
                            fieldstr2.AppendLine($@"                ]}}/>");
                        }
                        else if (checktype.IsNumber())
                        {
                            fieldstr2.AppendLine($@"                formItem: <InputNumber placeholder="""" />");
                        }
                        else if (checktype == typeof(string))
                        {
                            fieldstr2.AppendLine($@"                formItem: <Input placeholder="""" />");
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            fieldstr2.AppendLine($@"                formItem: <WtmRangePicker placeholder="""" />");
                        }
                    }
                    fieldstr2.Append("            }");
                    if (i < pros.Count - 1)
                    {
                        fieldstr2.Append(',');
                    }
                    fieldstr2.Append(Environment.NewLine);
                }

                return rv.Replace("$fields$", fieldstr.ToString()).Replace("$fields2$", fieldstr2.ToString());
            }

            if (name == "search")
            {
                return rv;
            }
            if (name == "forms")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsFormField == true).ToList();

                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    if (string.IsNullOrEmpty(item.SubIdField))
                    {
                        if (string.IsNullOrEmpty(item.RelatedField) == false)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            fieldstr.AppendLine($@"                <FormItem {{...props}} fieId=""Entity.{fk}"" />");
                        }
                        else
                        {
                            var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                            Type checktype = proType;
                            if (proType.IsNullable())
                            {
                                checktype = proType.GetGenericArguments()[0];
                            }
                            if (checktype == typeof(bool))
                            {
                                fieldstr.AppendLine($@"                <FormItem {{...props}} fieId=""Entity.{item.FieldName}""  $switchdefaultvalue$ />");
                            }
                            else
                            {
                                fieldstr.AppendLine($@"                <FormItem {{...props}} fieId=""Entity.{item.FieldName}"" />");
                            }
                        }
                    }
                    else
                    {
                        fieldstr.AppendLine($@"                <Col span={{24}}>
                    <FormItem {{...props}} fieId=""Entity.{item.FieldName}"" layout=""row"" />
                </Col>");
                    }
                }
                return rv.Replace("$fields$", fieldstr.Replace("$switchdefaultvalue$", "value={false}").ToString()).Replace("$efields$", fieldstr.Replace("$switchdefaultvalue$", "").ToString());
            }

            return rv;
        }

        public string GenerateVUEView(string name, List<string> apineeded)
        {
            var area_toLower = Area.ToLower();//$area$
            var area = Area;//$area$
            var modelname_toLower = ModelName.ToLower();//$modelname_toLower$
            var rv = GetResource($"{name}.txt", "Spa.Vue")
                .Replace("$modelname$", ModelName).Replace("$area_toLower$", area_toLower).Replace("$modelname_toLower$", modelname_toLower).Replace("$area$", area);
            if (apineeded == null)
            {
                apineeded = new List<string>();
            }
            Type modelType = Type.GetType(SelectedModel);
            if (name == "controller.entity")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder enumstr = new StringBuilder();
                var pros_IsSearcherField = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                var pros_IsFormField = FieldInfos.Where(x => x.IsFormField == true).ToList();
                var pros_IsBatchField = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                fieldstr.Append(Environment.NewLine);
                for (int i = 0; i < pros_IsSearcherField.Count; i++)
                {
                    var str = string.Empty;
                    var item = pros_IsSearcherField[i];
                    string valueType = "";
                    var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                    var display = modelType.GetSingleProperty(item.FieldName).GetCustomAttribute<DisplayAttribute>();
                    var displayname = display is null ? $"{ModelName}_{item.FieldName}" : display.Name.Replace(".", "_");
                    Type checktype = proType;
                    if (proType.IsNullable())
                    {
                        checktype = proType.GetGenericArguments()[0];
                    }
                    if (checktype.IsBoolOrNullableBool())
                    {
                        valueType="switch";
                    }
                    else if (checktype.IsEnum())
                    {
                        valueType="select";
                    }
                    else if (checktype == typeof(DateTime))
                    {
                        valueType="dateTimeRange";
                    }
                    else if (checktype== typeof(FileAttachment))
                    {
                        if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon"))
                        {
                            valueType = "image";
                        }
                        else
                        {
                            valueType = "upload";
                        }
                    }
                    else
                    {
                        valueType="text";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        var fk = DC.GetFKName2(modelType, item.FieldName);
                        var url = $"/api/_{area}/{ModelName}/Get{subtype.Name}s";

                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            str=
                                $"    readonly {item.FieldName}_Filter: WTM_EntitiesField = {{"+
                                Environment.NewLine+
                                $"        name: '{fk}',"+
                                Environment.NewLine+
                                $"        label: '{displayname}',"+
                                Environment.NewLine+
                                $"        request: async () => FieldRequest('{url}'),"+
                                Environment.NewLine+
                                $"        valueType: WTM_ValueType.{valueType},"+
                                Environment.NewLine+
                                "    }";
                        }
                        else
                        {
                            str=
                                $"    readonly Selected{item.FieldName}IDs_Filter: WTM_EntitiesField = {{"+
                                Environment.NewLine+
                                $"        name: 'Selected{item.FieldName}IDs',"+
                                Environment.NewLine+
                                $"        label: '{displayname}',"+
                                Environment.NewLine+
                                $"        fieldProps: {{ mode: 'tags' }},"+
                                Environment.NewLine+
                                $"        request: async () => FieldRequest('{url}'),"+
                                Environment.NewLine+
                                $"        valueType: WTM_ValueType.{valueType},"+
                                Environment.NewLine+
                                "    }";
                        }
                    }
                    else
                    {
                        str=
                            $"    readonly {item.FieldName}_Filter: WTM_EntitiesField = {{"+
                            Environment.NewLine+
                            $"        name: '{item.FieldName}',"+
                            Environment.NewLine+
                            $"        label: '{displayname}',"+
                            Environment.NewLine+
                            $"        valueType: WTM_ValueType.{valueType},"+
                            Environment.NewLine+
                            "    }";
                    }

                    fieldstr.Append(str);
                    fieldstr.Append(Environment.NewLine);
                }
                for (int i = 0; i < pros_IsFormField.Count; i++)
                {
                    var str = string.Empty;
                    var item = pros_IsFormField[i];
                    string valueType = "";
                    var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                    var display = modelType.GetSingleProperty(item.FieldName).GetCustomAttribute<DisplayAttribute>();
                    var displayname = display is null ? $"{ModelName}_{item.FieldName}" : display.Name.Replace(".", "_");
                    Type checktype = proType;
                    if (proType.IsNullable())
                    {
                        checktype = proType.GetGenericArguments()[0];
                    }
                    if (checktype.IsBoolOrNullableBool())
                    {
                        valueType="switch";
                    }
                    else if (checktype.IsEnum())
                    {
                        valueType="select";
                    }
                    else if (checktype == typeof(DateTime))
                    {
                        valueType="dateTime";
                    }
                    else if (checktype== typeof(FileAttachment))
                    {
                        if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon"))
                        {
                            valueType = "image";
                        }
                        else
                        {
                            valueType = "upload";
                        }
                    }
                    else
                    {
                        valueType="text";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        var fk = DC.GetFKName2(modelType, item.FieldName);
                        var url = $"/api/_{area}/{ModelName}/Get{subtype.Name}s";

                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            str=
                                $"    readonly {item.FieldName}_Form: WTM_EntitiesField = {{"+
                                Environment.NewLine+
                                $"        name: ['Entity', '{item.FieldName}'],"+
                                Environment.NewLine+
                                $"        label: '{displayname}',"+
                                Environment.NewLine+
                                $"        fieldProps: {{ mode: 'tags' }},"+
                                Environment.NewLine+
                                $"        request: async () => FieldRequest('{url}'),"+
                                Environment.NewLine+
                                $"        valueType: WTM_ValueType.{valueType},"+
                                Environment.NewLine+
                                "    }";
                        }
                        else
                        {
                            str=
                                $"    readonly Selected{item.FieldName}IDs_Form: WTM_EntitiesField = {{"+
                                Environment.NewLine+
                                $"        name: ['Entity', 'Selected{item.FieldName}IDs'],"+
                                Environment.NewLine+
                                $"        label: '{displayname}',"+
                                Environment.NewLine+
                                $"        fieldProps: {{ mode: 'tags' }},"+
                                Environment.NewLine+
                                $"        request: async () => FieldRequest('{url}'),"+
                                Environment.NewLine+
                                $"        valueType: WTM_ValueType.{valueType},"+
                                Environment.NewLine+
                                "    }"+
                                $"    readonly {item.FieldName}_Detail: WTM_EntitiesField = {{"+
                                Environment.NewLine+
                                $"        name: ['Entity', '{item.FieldName}'],"+
                                Environment.NewLine+
                                $"        label: '{displayname}',"+
                                Environment.NewLine+
                                $"        fieldProps: {{ mode: 'tags' }},"+
                                Environment.NewLine+
                                $"        request: async () => FieldRequest('{url}'),"+
                                Environment.NewLine+
                                $"        valueType: WTM_ValueType.checkbox,"+
                                Environment.NewLine+
                                "    }";
                        }
                    }
                    else
                    {
                        str=
                            $"    readonly {item.FieldName}_Form: WTM_EntitiesField = {{"+
                            Environment.NewLine+
                            $"        name: ['Entity', '{item.FieldName}'],"+
                            Environment.NewLine+
                            $"        label: '{displayname}',"+
                            Environment.NewLine+
                            $"        valueType: WTM_ValueType.{valueType},"+
                            Environment.NewLine+
                            "    }";
                    }
                    fieldstr.Append(str);
                    fieldstr.Append(Environment.NewLine);
                }
                for (int i = 0; i < pros_IsBatchField.Count; i++)
                {
                    var str = string.Empty;
                    var item = pros_IsBatchField[i];
                    string valueType = "";
                    var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                    var display = modelType.GetSingleProperty(item.FieldName).GetCustomAttribute<DisplayAttribute>();
                    var displayname = display is null ? $"{ModelName}_{item.FieldName}" : display.Name.Replace(".", "_");
                    Type checktype = proType;
                    if (proType.IsNullable())
                    {
                        checktype = proType.GetGenericArguments()[0];
                    }
                    if (checktype.IsBoolOrNullableBool())
                    {
                        valueType="switch";
                    }
                    else if (checktype.IsEnum())
                    {
                        valueType="select";
                    }
                    else if (checktype == typeof(DateTime))
                    {
                        valueType="dateTime";
                    }
                    else if (checktype== typeof(FileAttachment))
                    {
                        if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon"))
                        {
                            valueType = "image";
                        }
                        else
                        {
                            valueType = "upload";
                        }
                    }
                    else
                    {
                        valueType="text";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        var fk = DC.GetFKName2(modelType, item.FieldName);
                        var url = $"/api/_{area}/{ModelName}/Get{subtype.Name}s";

                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            str=
                                $"    readonly {item.FieldName}_LinkedVM: WTM_EntitiesField = {{"+
                                Environment.NewLine+
                                $"        name: ['LinkedVM', '{fk}'],"+
                                Environment.NewLine+
                                $"        label: '{displayname}',"+
                                Environment.NewLine+
                                $"        request: async () => FieldRequest('{url}'),"+
                                Environment.NewLine+
                                $"        valueType: WTM_ValueType.{valueType},"+
                                Environment.NewLine+
                                "    }";
                        }
                        else
                        {
                            str=
                                $"    readonly Selected{item.FieldName}IDs_LinkedVM: WTM_EntitiesField = {{"+
                                Environment.NewLine+
                                $"        name: ['LinkedVM', 'Selected{item.FieldName}IDs'],"+
                                Environment.NewLine+
                                $"        label: '{displayname}',"+
                                Environment.NewLine+
                                $"        fieldProps: {{ mode: 'tags' }},"+
                                Environment.NewLine+
                                $"        request: async () => FieldRequest('{url}'),"+
                                Environment.NewLine+
                                $"        valueType: WTM_ValueType.{valueType},"+
                                Environment.NewLine+
                                "    }";
                        }
                    }
                    else
                    {
                        str=
                            $"    readonly {item.FieldName}_LinkedVM: WTM_EntitiesField = {{"+
                            Environment.NewLine+
                            $"        name: ['LinkedVM', '{item.FieldName}'],"+
                            Environment.NewLine+
                            $"        label: '{displayname}',"+
                            Environment.NewLine+
                            $"        valueType: WTM_ValueType.{valueType},"+
                            Environment.NewLine+
                            "    }";
                    }

                    fieldstr.Append(str);
                    fieldstr.Append(Environment.NewLine);
                }
                return rv.Replace("$fields$", fieldstr.ToString());
            }
            if (name == "views.widgets.batchFormViewBatchEdit")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder fieldstr1 = new StringBuilder();
                var pros_IsSearcherField = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                var pros_IsListField = FieldInfos.Where(x => x.IsListField == true).ToList();
                var pros_IsFormField = FieldInfos.Where(x => x.IsFormField == true).ToList();
                var pros_IsBatchField = FieldInfos.Where(x => x.IsBatchField == true).ToList();
                fieldstr.Append(Environment.NewLine);
                for (int i = 0; i < pros_IsBatchField.Count; i++)
                {
                    var str = string.Empty;
                    var str1 = string.Empty;
                    var item = pros_IsBatchField[i];
                    List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                    string newname = item.FieldName;
                    var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                    if (proType== typeof(FileAttachment))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        var fk = DC.GetFKName2(modelType, item.FieldName);

                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            str=
                                $"        <a-col :span=\"12\">"+
                                Environment.NewLine+
                                //$"           <WtmField entityKey=\"{item.FieldName}Id_LinkedVM\"/>"+
                                $"           <WtmField entityKey=\"{fk}_LinkedVM\"/>"+
                                Environment.NewLine+
                                $"        </a-col>";

                            str1=
                                //$"        {item.FieldName}Id: undefined,";
                                $"        {fk}: undefined,";
                        }
                        else
                        {
                            str=
                                $"        <a-col :span=\"12\">"+
                                Environment.NewLine+
                                $"           <WtmField entityKey=\"Selected{item.FieldName}IDs_LinkedVM\"/>"+
                                Environment.NewLine+
                                $"        </a-col>";

                            str1=
                                $"        Selected{item.FieldName}IDs: undefined,";
                        }
                    }
                    else
                    {
                        str=
                            $"        <a-col :span=\"12\">"+
                            Environment.NewLine+
                            $"           <WtmField entityKey=\"{item.FieldName}_LinkedVM\"/>"+
                            Environment.NewLine+
                            $"        </a-col>";

                        str1=
                            $"        {item.FieldName}: undefined,";
                    }

                    fieldstr.Append(str);
                    fieldstr.Append(Environment.NewLine);
                    fieldstr1.Append(str1);
                    fieldstr1.Append(Environment.NewLine);
                }
                return rv.Replace("$fields$", fieldstr.ToString()).Replace("$fields1$ ", fieldstr1.ToString());
            }
            if (name == "views.widgets.filterViewIndexSearcher")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder fieldstr1 = new StringBuilder();
                var pros_IsSearcherField = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                fieldstr.Append(Environment.NewLine);
                for (int i = 0; i < pros_IsSearcherField.Count; i++)
                {
                    var str = string.Empty;
                    var str1 = string.Empty;
                    var item = pros_IsSearcherField[i];
                    List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                    string newname = item.FieldName;
                    var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                    if (proType== typeof(FileAttachment))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        var fk = DC.GetFKName2(modelType, item.FieldName);
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            str=
                                //$"    <WtmField entityKey=\"{item.FieldName}Id_Filter\" />";
                                $"    <WtmField entityKey=\"{fk}_Filter\" />";
                            str1=
                                //$"        {item.FieldName}Id: undefined,";
                                $"        {fk}: undefined,";
                        }
                        else
                        {
                            str=
                            $"    <WtmField entityKey=\"Selected{item.FieldName}IDs_Filter\" />";
                            str1=
                                $"        Selected{item.FieldName}IDs: [],";
                        }
                    }
                    else
                    {
                        str=
                            $"    <WtmField entityKey=\"{item.FieldName}_Filter\" />";
                        str1=
                            $"        {item.FieldName}: undefined,";
                    }

                    fieldstr.Append(str);
                    fieldstr.Append(Environment.NewLine);
                    fieldstr1.Append(str1);
                    fieldstr1.Append(Environment.NewLine);
                }
                return rv.Replace("$fields$", fieldstr.ToString()).Replace("$fields1$ ", fieldstr1.ToString());
            }

            if (name == "views.widgets.formViewCreateAdd"||name == "views.widgets.formViewDetailsDetail"||name == "views.widgets.formViewEditEdit")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder fieldstr1 = new StringBuilder();
                StringBuilder fieldstr2 = new StringBuilder();
                var pros_IsFormField = FieldInfos.Where(x => x.IsFormField == true).ToList();
                fieldstr.Append(Environment.NewLine);
                for (int i = 0; i < pros_IsFormField.Count; i++)
                {
                    var str = string.Empty;
                    var str1 = string.Empty;
                    var str2 = string.Empty;
                    var item = pros_IsFormField[i];
                    List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                    string newname = item.FieldName;
                    var houzhui = "Form";
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        var fk = DC.GetFKName2(modelType, item.FieldName);
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            str=
                                $"      <a-col :span=\"12\">"+
                                Environment.NewLine+
                                //$"         <WtmField entityKey=\"{item.FieldName}Id_{houzhui}\"/>"+
                                $"         <WtmField entityKey=\"{fk}_{houzhui}\"/>"+
                                Environment.NewLine+
                                $"      </a-col>";
                            str1=
                                //$"        {item.FieldName}Id: undefined,";
                                $"        {fk}: undefined,";
                        }
                        else
                        {
                            if (name == "views.widgets.formViewDetailsDetail")
                            {
                                houzhui="Detail";
                            }
                            str=
                            $"      <a-col :span=\"12\">"+
                            Environment.NewLine+
                            $"         <WtmField entityKey=\"Selected{item.FieldName}IDs_{houzhui}\"/>"+
                            Environment.NewLine+
                            $"      </a-col>";
                            str2=
                                $"    Selected{item.FieldName}IDs: [],";
                        }
                    }
                    else
                    {
                        str=
                            $"      <a-col :span=\"12\">"+
                            Environment.NewLine+
                            $"         <WtmField entityKey=\"{item.FieldName}_{houzhui}\"/>"+
                            Environment.NewLine+
                            $"      </a-col>";

                        str1=
                            $"        {item.FieldName}: undefined,";
                    }
                    fieldstr.Append(str);
                    fieldstr.Append(Environment.NewLine);
                    fieldstr1.Append(str1);
                    fieldstr1.Append(Environment.NewLine);
                    fieldstr2.Append(str2);
                    fieldstr2.Append(Environment.NewLine);
                }
                return rv.Replace("$fields$", fieldstr.ToString()).Replace("$fields1$ ", fieldstr1.ToString()).Replace("$fields2$ ", fieldstr2.ToString());
            }
            if (name == "views.widgets.gridViewIndex")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder fieldstr1 = new StringBuilder();
                var pros_IsListField = FieldInfos.Where(x => x.IsListField == true).ToList();
                fieldstr.Append(Environment.NewLine);
                for (int i = 0; i < pros_IsListField.Count; i++)
                {
                    var str = string.Empty;
                    var item = pros_IsListField[i];
                    var display = modelType.GetSingleProperty(item.FieldName).GetCustomAttribute<DisplayAttribute>();
                    var displayname = display is null ? $"{ModelName}_{item.FieldName}" : display.Name.Replace(".", "_");
                    List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                    string newname = item.FieldName;
                    var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                    if (proType== typeof(FileAttachment))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        string prefix = "";

                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            var subpro = subtype.GetSingleProperty(item.SubField);
                            existSubPro.Add(subpro);
                            int count = existSubPro.Where(x => x.Name == subpro.Name).Count();
                            if (count > 1)
                            {
                                prefix = count + "";
                            }
                            newname = item.SubField + "_view" + prefix;
                            str=
                            $"      {{"+
                            Environment.NewLine+
                            $"        headerName:'{displayname}',"+
                            Environment.NewLine+
                            $"        field: \"{newname}\","+
                            Environment.NewLine+
                            $"      }},";
                        }
                        else
                        {
                            //////////这里list的多对多没时间弄
                        }
                    }
                    else
                    {
                        str=
                        $"      {{"+
                        Environment.NewLine+
                        $"        headerName:'{displayname}',"+
                        Environment.NewLine+
                        $"        field: \"{item.FieldName}\","+
                        Environment.NewLine+
                        $"      }},";
                    }

                    fieldstr.Append(str);
                    fieldstr.Append(Environment.NewLine);
                }
                return rv.Replace("$fields$", fieldstr.ToString());
            }
            return rv;
        }

        public string GenerateBlazorView(string name)
        {
            string pagepath = string.IsNullOrEmpty(Area) ? $"/{ModelName}" : $"/{Area}/{ModelName}";
            string area = string.IsNullOrEmpty(Area) ? "/" : $"/_{Area}/";
            if (name != "Index")
            {
                pagepath += $"/{name}";
            }
            if (name == "Edit" || name == "Details")
            {
                pagepath += "/{id}";
            }
            var rv = GetResource($"{name}.txt", "Spa.Blazor")
                .Replace("$modelname$", ModelName)
                .Replace("$vmnamespace$", VMNs)
                .Replace("$des$", ModuleName)
                .Replace("$controllername$", $"{ControllerNs},{ModelName}")
                .Replace("$pagepath$", pagepath);
            Type modelType = Type.GetType(SelectedModel);
            if (name == "Index")
            {
                StringBuilder fieldstr = new StringBuilder();
                StringBuilder fieldstr2 = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsListField == true).ToList();
                var pros2 = FieldInfos.Where(x => x.IsSearcherField == true).ToList();
                List<PropertyInfo> existSubPro = new List<PropertyInfo>();
                Dictionary<string, string> apis = new Dictionary<string, string>();
                Dictionary<string, string> multiapis = new Dictionary<string, string>();
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    var mpro = modelType.GetSingleProperty(item.FieldName);
                    string render = "";
                    string render1 = "Sortable=\"true\"";
                    string template = "";
                    string newname = item.FieldName;
                    if (mpro.PropertyType.IsBoolOrNullableBool())
                    {
                        if (mpro.PropertyType.IsNullable())
                        {
                            render = "ComponentType=\"@typeof(NullSwitch)\"";
                        }
                        else
                        {
                            render = "ComponentType=\"@typeof(Switch)\"";
                        }
                    }
                    if (mpro.PropertyType == typeof(DateTime) || mpro.PropertyType == typeof(DateTime?))
                    {
                        render = "FormatString=\"yyyy-MM-dd HH: mm: ss\"";
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        string prefix = "";
                        if (subtype == typeof(FileAttachment))
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon") || item.FieldName.ToLower().Contains("zhaopian") || item.FieldName.ToLower().Contains("tupian"))
                            {
                                template = @"
            <Template Context=""data"">
                <Avatar @key=""data.Value"" Size=""Size.ExtraSmall"" GetUrlAsync=""()=>WtmBlazor.GetBase64Image(data.Value.ToString(),150,150)"" />
            </Template>";
                            }
                            else
                            {
                                template = @"
            <Template Context=""data"">
                @if (data.Value.HasValue){
                    <Button Size=""Size.ExtraSmall"" Text=""@WtmBlazor.Localizer[""Sys.Download""]"" OnClick=""@(async x => await Download($""/api/_file/DownloadFile/{data.Value}"",null, HttpMethodEnum.GET))"" />
                }
            </Template>";
                            }
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            newname = fk;
                        }
                        else
                        {
                            var subpro = subtype.GetSingleProperty(item.SubField);
                            existSubPro.Add(subpro);
                            int count = existSubPro.Where(x => x.Name == subpro.Name).Count();
                            if (count > 1)
                            {
                                prefix = count + "";
                            }
                            newname = item.SubField + "_view" + prefix;
                        }
                    }
                    if (template == "")
                    {
                        fieldstr.Append($@"
        <TableColumn @bind-Field=""@context.{newname}"" {render} {render1} />");
                    }
                    else
                    {
                        fieldstr.Append($@"
        <TableColumn @bind-Field=""@context.{newname}"" {render}  >
{template}
        </TableColumn>");
                    }
                }

                for (int i = 0; i < pros2.Count; i++)
                {
                    string controltype = "BootstrapInput";
                    string sitems = "";
                    string bindfield = "";
                    string ph = "";
                    string render = "";

                    var item = pros2[i];
                    if (item.SubField == "`file")
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            bindfield = fk;
                        }
                        else
                        {
                            bindfield = $"Selected{item.FieldName}IDs";
                        }
                    }
                    else
                    {
                        bindfield = item.FieldName;
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            controltype = "Select";
                        }
                        else
                        {
                            controltype = "MultiSelect";
                        }
                        var tempname = $"All{subtype.Name}s";
                        sitems = $"Items=\"@{tempname}\"";
                        if (apis.ContainsKey(tempname) == false && multiapis.ContainsKey(tempname) == false)
                        {
                            if (controltype == "Select")
                            {
                                apis.Add(tempname, $"/api{area}{ModelName}/Get{subtype.Name}s");
                            }
                            else
                            {
                                multiapis.Add(tempname, $"/api{area}{ModelName}/Get{subtype.Name}s");
                            }
                        }
                    }
                    else
                    {
                        var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            controltype = "Select";
                            sitems = "Items=\"@WtmBlazor.GlobalSelectItems.SearcherBoolItems\"";
                        }
                        else if (checktype.IsEnum())
                        {
                            controltype = "Select";
                        }
                        else if (checktype.IsNumber())
                        {
                            controltype = "BootstrapInputNumber";
                        }
                        else if (checktype == typeof(string))
                        {
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            controltype = "WTDateRange";
                        }
                    }
                    if (controltype == "Select" || controltype == "MultiSelect")
                    {
                        ph = "PlaceHolder=\"@WtmBlazor.Localizer[\"Sys.All\"]\"";
                        render = "ShowSearch=\"true\"";
                    }
                    fieldstr2.Append($@"
            <{controltype} @bind-Value=""@SearchModel.{bindfield}"" {sitems} {ph} {render}/>");
                }

                StringBuilder apiinit = new StringBuilder();
                StringBuilder fieldinit = new StringBuilder();
                foreach (var item in apis)
                {
                    apiinit.Append(@$"
        {item.Key} = await WtmBlazor.Api.CallItemsApi(""{item.Value}"", placeholder: WtmBlazor.Localizer[""Sys.All""]);
");
                    fieldinit.Append($@"
    private List<SelectedItem> {item.Key} = new List<SelectedItem>();
");
                }
                foreach (var item in multiapis)
                {
                    apiinit.Append(@$"
        {item.Key} = await WtmBlazor.Api.CallItemsApi(""{item.Value}"");
");
                    fieldinit.Append($@"
    private List<SelectedItem> {item.Key} = new List<SelectedItem>();
");
                }
                return rv.Replace("$columns$", fieldstr.ToString()).Replace("$searchfields$", fieldstr2.ToString()).Replace("$init$", apiinit.ToString()).Replace("$fieldinit$", fieldinit.ToString()).Replace("$area$", area);
            }

            if (name == "Create" || name == "Edit")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsFormField == true).ToList();

                //生成表单model
                Dictionary<string, string> apis = new Dictionary<string, string>();
                Dictionary<string, string> multiapis = new Dictionary<string, string>();
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    string controltype = "BootstrapInput";
                    string sitems = "";
                    string bindfield = "";
                    string ph = "";
                    string render = "";
                    var property = modelType.GetSingleProperty(item.FieldName);

                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            bindfield = "Entity." + fk;
                        }
                        else
                        {
                            bindfield = $"Selected{item.FieldName}IDs";
                        }
                    }
                    else
                    {
                        bindfield = "Entity." + item.FieldName;
                    }

                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (item.SubField == "`file")
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon") || item.FieldName.ToLower().Contains("zhaopian") || item.FieldName.ToLower().Contains("tupian"))
                            {
                                controltype = "WTUploadImage";
                            }
                            else
                            {
                                controltype = "WTUploadFile";
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.SubIdField) == true)
                            {
                                controltype = "Select";
                            }
                            else
                            {
                                controltype = "Transfer";
                            }
                            var tempname = $"All{subtype.Name}s";
                            sitems = $"Items=\"@{tempname}\"";
                            if (apis.ContainsKey(tempname) == false && multiapis.ContainsKey(tempname) == false)
                            {
                                if (controltype == "Select")
                                {
                                    apis.Add(tempname, $"/api{area}{ModelName}/Get{subtype.Name}s");
                                }
                                else
                                {
                                    multiapis.Add(tempname, $"/api{area}{ModelName}/Get{subtype.Name}s");
                                }
                            }
                        }
                    }
                    else
                    {
                        var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            if (proType.IsNullable())
                            {
                                controltype = "NullSwitch";
                            }
                            else
                            {
                                controltype = "Switch";
                            }
                        }
                        else if (checktype.IsEnum())
                        {
                            controltype = "Select";
                        }
                        else if (checktype.IsNumber())
                        {
                            controltype = "BootstrapInputNumber";
                        }
                        else if (checktype == typeof(string))
                        {
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            controltype = "DateTimePicker";
                        }
                    }
                    if (controltype == "Select" || controltype == "MultiSelect")
                    {
                        ph = "PlaceHolder=\"@WtmBlazor.Localizer[\"Validate.PleaseSelect\"]\"";
                        render = "ShowSearch=\"true\"";
                    }
                    if (controltype == "DateTimePicker")
                    {
                        render = "ViewModel=\"DatePickerViewModel.DateTime\" ShowSidebar=\"true\"";
                    }

                    if (controltype == "Transfer")
                    {
                        fieldstr.Append($@"
    <Row ColSpan=""2"">
            <{controltype} @bind-Value=""@Model.{bindfield}"" {sitems} {ph}/>
    </Row>");
                    }
                    else
                    {
                        fieldstr.Append($@"
            <{controltype} @bind-Value=""@Model.{bindfield}"" {sitems} {ph} {render}/>");
                    }
                }

                StringBuilder apiinit = new StringBuilder();
                StringBuilder fieldinit = new StringBuilder();
                foreach (var item in apis)
                {
                    apiinit.Append(@$"
        {item.Key} = await WtmBlazor.Api.CallItemsApi(""{item.Value}"", placeholder: WtmBlazor.Localizer[""Validate.PleaseSelect""]);
");
                    fieldinit.Append($@"
    private List<SelectedItem> {item.Key} = new List<SelectedItem>();
");
                }
                foreach (var item in multiapis)
                {
                    apiinit.Append(@$"
        {item.Key} = await WtmBlazor.Api.CallItemsApi(""{item.Value}"");
");
                    fieldinit.Append($@"
    private List<SelectedItem> {item.Key} = new List<SelectedItem>();
");
                }
                return rv.Replace("$formfields$", fieldstr.ToString()).Replace("$fieldinit$", fieldinit.ToString()).Replace("$init$", apiinit.ToString()).Replace("$area$", area);
            }
            if (name == "Details")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsFormField == true).ToList();

                //生成表单model
                Dictionary<string, string> apis = new Dictionary<string, string>();
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    string controltype = "Display";
                    string sitems = "";
                    string bindfield = "";
                    string disabled = "";

                    var property = modelType.GetSingleProperty(item.FieldName);

                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            bindfield = "Entity." + fk;
                        }
                        else
                        {
                            bindfield = $"Selected{item.FieldName}IDs";
                        }
                    }
                    else
                    {
                        bindfield = "Entity." + item.FieldName;
                    }
                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (item.SubField == "`file")
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon") || item.FieldName.ToLower().Contains("zhaopian") || item.FieldName.ToLower().Contains("tupian"))
                            {
                                controltype = "WTUploadImage";
                            }
                            else
                            {
                                controltype = "WTUploadFile";
                            }
                            disabled = "IsDisabled=\"true\"";
                        }
                        else
                        {
                            var tempname = $"All{subtype.Name}s";
                            sitems = $"Lookup=\"@{tempname}\"";
                            if (apis.ContainsKey(tempname) == false)
                            {
                                apis.Add(tempname, $"/api{area}{ModelName}/Get{subtype.Name}s");
                            }
                        }
                    }
                    else
                    {
                        var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                        Type checktype = proType;
                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }
                        if (checktype == typeof(bool))
                        {
                            if (proType.IsNullable())
                            {
                                controltype = "NullSwitch";
                            }
                            else
                            {
                                controltype = "Switch";
                            }
                            disabled = "IsDisabled=\"true\"";
                        }
                        if (checktype == typeof(DateTime))
                        {
                            controltype = "DateTimePicker";
                            disabled = "IsDisabled=\"true\"";
                        }
                    }
                    if (controltype == "WTUploadFile")
                    {
                        string label = property.GetPropertyDisplayName();
                        fieldstr.Append($@"
                @if (Model.{bindfield}.HasValue){{
                    <div>
                          <label class=""control-label is-display"">{label}</label>
                          <div><Button Size=""Size.Small"" Text=""@WtmBlazor.Localizer[""Sys.Download""]"" OnClick=""@(async x => await Download($""/api/_file/DownloadFile/{{Model.{bindfield}}}"",null, HttpMethodEnum.GET))"" /></div>
                    </div>
                }}
");
                    }
                    else if (controltype == "DateTimePicker")
                    {
                        fieldstr.Append($@"
            <{controltype} @bind-Value=""@Model.{bindfield}"" {sitems} {disabled} ShowLabel=""true"" ViewModel=""DatePickerViewModel.DateTime"" />");
                    }
                    else
                    {
                        fieldstr.Append($@"
            <{controltype} @bind-Value=""@Model.{bindfield}"" {sitems} {disabled} ShowLabel=""true""/>");
                    }
                }

                StringBuilder apiinit = new StringBuilder();
                StringBuilder fieldinit = new StringBuilder();
                foreach (var item in apis)
                {
                    apiinit.Append(@$"
        {item.Key} = await WtmBlazor.Api.CallItemsApi(""{item.Value}"", placeholder: WtmBlazor.Localizer[""Sys.All""]);
");
                    fieldinit.Append($@"
    private List<SelectedItem> {item.Key} = new List<SelectedItem>();
");
                }

                return rv.Replace("$formfields$", fieldstr.ToString()).Replace("$fieldinit$", fieldinit.ToString()).Replace("$init$", apiinit.ToString()).Replace("$area$", area);
            }
            if (name == "BatchEdit")
            {
                StringBuilder fieldstr = new StringBuilder();
                var pros = FieldInfos.Where(x => x.IsBatchField == true).ToList();

                //生成表单model
                Dictionary<string, string> apis = new Dictionary<string, string>();
                Dictionary<string, string> multiapis = new Dictionary<string, string>();
                for (int i = 0; i < pros.Count; i++)
                {
                    var item = pros[i];
                    string controltype = "BootstrapInput";
                    string sitems = "";
                    string bindfield = "";
                    string ph = "";
                    string render = "";
                    var property = modelType.GetSingleProperty(item.FieldName);

                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        if (string.IsNullOrEmpty(item.SubIdField) == true)
                        {
                            var fk = DC.GetFKName2(modelType, item.FieldName);
                            bindfield = "LinkedVM." + fk;
                        }
                        else
                        {
                            bindfield = $"Selected{item.FieldName}IDs";
                        }
                    }
                    else
                    {
                        bindfield = "LinkedVM." + item.FieldName;
                    }

                    if (string.IsNullOrEmpty(item.RelatedField) == false)
                    {
                        var subtype = Type.GetType(item.RelatedField);
                        if (item.SubField == "`file")
                        {
                            if (item.FieldName.ToLower().Contains("photo") || item.FieldName.ToLower().Contains("pic") || item.FieldName.ToLower().Contains("icon") || item.FieldName.ToLower().Contains("zhaopian") || item.FieldName.ToLower().Contains("tupian"))
                            {
                                controltype = "WTUploadImage";
                            }
                            else
                            {
                                controltype = "WTUploadFile";
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.SubIdField) == true)
                            {
                                controltype = "Select";
                            }
                            else
                            {
                                controltype = "Transfer";
                            }
                            var tempname = $"All{subtype.Name}s";
                            sitems = $"Items=\"@{tempname}\"";
                            if (apis.ContainsKey(tempname) == false && multiapis.ContainsKey(tempname) == false)
                            {
                                if (controltype == "Select")
                                {
                                    apis.Add(tempname, $"/api{area}{ModelName}/Get{subtype.Name}s");
                                }
                                else
                                {
                                    multiapis.Add(tempname, $"/api{area}{ModelName}/Get{subtype.Name}s");
                                }
                            }
                        }
                    }
                    else
                    {
                        var proType = modelType.GetSingleProperty(item.FieldName)?.PropertyType;
                        Type checktype = proType;

                        if (proType.IsNullable())
                        {
                            checktype = proType.GetGenericArguments()[0];
                        }

                        if (checktype == typeof(bool))
                        {
                            if (proType.IsNullable())
                            {
                                controltype = "NullSwitch";
                            }
                            else
                            {
                                controltype = "Switch";
                            }
                        }
                        else if (checktype.IsEnum())
                        {
                            controltype = "Select";
                        }
                        else if (checktype.IsNumber())
                        {
                            controltype = "BootstrapInputNumber";
                        }
                        else if (checktype == typeof(string))
                        {
                        }
                        else if (checktype == typeof(DateTime))
                        {
                            controltype = "DateTimePicker";
                        }
                    }
                    if (controltype == "Select" || controltype == "MultiSelect")
                    {
                        ph = "PlaceHolder=\"@WtmBlazor.Localizer[\"Sys.PleaseSelect\"]\"";
                        render = "ShowSearch=\"true\"";
                    }
                    if (controltype == "DateTimePicker")
                    {
                        render = "ViewModel=\"DatePickerViewModel.DateTime\" ShowSidebar=\"true\"";
                    }

                    if (controltype == "Transfer")
                    {
                        fieldstr.Append($@"
    <Row ColSpan=""2"">
            <{controltype} @bind-Value=""@Model.{bindfield}"" {sitems} {ph}/>
    </Row>");
                    }
                    else
                    {
                        fieldstr.Append($@"
            <{controltype} @bind-Value=""@Model.{bindfield}"" {sitems} {render} {ph}/>");
                    }
                }

                StringBuilder apiinit = new StringBuilder();
                StringBuilder fieldinit = new StringBuilder();
                foreach (var item in apis)
                {
                    apiinit.Append(@$"
        {item.Key} = await WtmBlazor.Api.CallItemsApi(""{item.Value}"", placeholder: WtmBlazor.Localizer[""Sys.PleaseSelect""]);
");
                    fieldinit.Append($@"
    private List<SelectedItem> {item.Key} = new List<SelectedItem>();
");
                }
                foreach (var item in multiapis)
                {
                    apiinit.Append(@$"
        {item.Key} = await WtmBlazor.Api.CallItemsApi(""{item.Value}"");
");
                    fieldinit.Append($@"
    private List<SelectedItem> {item.Key} = new List<SelectedItem>();
");
                }
                return rv.Replace("$formfields$", fieldstr.ToString()).Replace("$fieldinit$", fieldinit.ToString()).Replace("$init$", apiinit.ToString()).Replace("$area$", area);
            }

            return rv.Replace("$area$", area);
        }

        public string GetResource(string fileName, string subdir = "")
        {
            //获取编译在程序中的Controller原始代码文本
            Assembly assembly = Assembly.GetExecutingAssembly();

            var AAS = assembly.GetManifestResourceNames();
            string loc = "";
            if (string.IsNullOrEmpty(subdir))
            {
                loc = $"Caomei.WTM_Mvc.GeneratorFiles.{fileName}";
            }
            else
            {
                loc = $"Caomei.WTM_Mvc.GeneratorFiles.{subdir}.{fileName}";
            }

            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(loc));
            string content = textStreamReader.ReadToEnd();
            textStreamReader.Close();
            return content;
        }

        private string GetRelatedNamespace(List<FieldInfo> pros, string s)
        {
            string otherns = @"";
            Type modelType = Type.GetType(SelectedModel);
            foreach (var pro in pros)
            {
                Type proType = null;

                if (string.IsNullOrEmpty(pro.RelatedField))
                {
                    proType = modelType.GetSingleProperty(pro.FieldName)?.PropertyType;
                }
                else
                {
                    proType = Type.GetType(pro.RelatedField);
                }
                string prons = proType.Namespace;
                if (proType.IsNullable())
                {
                    prons = proType.GetGenericArguments()[0].Namespace;
                }
                if (s.Contains($"using {prons};") == false && otherns.Contains($"using {prons};") == false)
                {
                    otherns += $@"using {prons};
";
                }
            }

            return s.Replace("$othernamespace$", otherns);
        }
    }

    public enum FieldInfoType
    { Normal, One2Many, Many2Many }

    public class FieldInfo
    {
        public string FieldName { get; set; }
        public string RelatedField { get; set; }

        public bool IsSearcherField { get; set; }

        public bool IsListField { get; set; }

        public bool IsFormField { get; set; }

        public bool IsImportField { get; set; }
        public bool IsBatchField { get; set; }

        public FieldInfoType InfoType
        {
            get
            {
                if (string.IsNullOrEmpty(RelatedField))
                {
                    return FieldInfoType.Normal;
                }
                else
                {
                    if (string.IsNullOrEmpty(SubIdField))
                    {
                        return FieldInfoType.One2Many;
                    }
                    else
                    {
                        return FieldInfoType.Many2Many;
                    }
                }
            }
        }

        /// <summary>
        /// 字段关联的类名
        /// </summary>
        public string SubField { get; set; }

        /// <summary>
        /// 多对多关系时，记录中间表关联到主表的字段名称
        /// </summary>
        public string SubIdField { get; set; }

        public string GetField(IDataContext DC, Type modelType)
        {
            if (this.InfoType == FieldInfoType.One2Many)
            {
                var fk = DC.GetFKName2(modelType, this.FieldName);
                return fk;
            }
            else
            {
                return this.FieldName;
            }
        }

        public string GetFKType(IDataContext DC, Type modelType)
        {
            Type fktype = null;
            if (this.InfoType == FieldInfoType.One2Many)
            {
                var fk = this.GetField(DC, modelType);
                fktype = modelType.GetSingleProperty(fk)?.PropertyType;
            }
            if (this.InfoType == FieldInfoType.Many2Many)
            {
                var middletype = modelType.GetSingleProperty(this.FieldName)?.PropertyType;
                fktype = middletype.GetGenericArguments()[0].GetSingleProperty(this.SubIdField)?.PropertyType;
            }
            var typename = "string";

            if (fktype == typeof(short) || fktype == typeof(short?))
            {
                typename = "short";
            }
            if (fktype == typeof(int) || fktype == typeof(int?))
            {
                typename = "int";
            }
            if (fktype == typeof(long) || fktype == typeof(long?))
            {
                typename = "long";
            }
            if (fktype == typeof(Guid) || fktype == typeof(Guid?))
            {
                typename = "Guid";
            }

            return typename;
        }
    }
}