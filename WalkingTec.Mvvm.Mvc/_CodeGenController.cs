using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc
{
    [DebugOnly]
    public class _CodeGenController : BaseController
    {
        [ActionDescription("Sys.CodeGen")]
        public IActionResult Index(UIEnum ui)
        {
            var vm = Wtm.CreateVM<CodeGenVM>();
            vm.UI = ui;
            vm.EntryDir = AppDomain.CurrentDomain.BaseDirectory;
            vm.AllModels = GetAllModels().ToListItems(x => x.Name, x => x.AssemblyQualifiedName);
            return View(vm);
        }

        [HttpPost]
        [ActionDescription("Codegen.ConfigurationField")]
        public IActionResult SetField(CodeGenVM vm)
        {
            if (vm.SelectedModel != null)
            {
                Type modeltype = Type.GetType(vm.SelectedModel);
                if (modeltype.IsSubclassOf(typeof(TopBasePoco)) == false)
                {
                    ModelState.AddModelError("SelectedModel", MvcProgram._localizer["Codegen.SelectedModelMustBeBasePoco"]);
                }
            }
            if (!ModelState.IsValid)
            {
                vm.AllModels = GetAllModels().ToListItems(x => x.Name, x => x.AssemblyQualifiedName);
                return View("Index", vm);
            }
            else
            {
                vm.FieldList.ModelFullName = vm.SelectedModel;
                return View(vm);
            }
        }

        [HttpPost]
        [ActionDescription("Codegen.GenerateConfirmation")]
        public IActionResult Gen(CodeGenVM vm)
        {
            return View(vm);
        }

        [HttpPost]
        public IActionResult DoGen(CodeGenVM vm)
        {
            vm.DoGen();
            return FFResult().Alert(MvcProgram._localizer["Codegen.Success"]);
        }

        [ActionDescription("Codegen.Preview")]
        [HttpPost]
        public IActionResult Preview(CodeGenVM vm)
        {
            if (vm.PreviewFile == "Controller")
            {
                ViewData["filename"] = $"{vm.ModelName}{(vm.IsApi == true ? "Api" : "")}Controller.cs";
                ViewData["code"] = vm.GenerateController();
            }
            else if (vm.PreviewFile == "Searcher" || vm.PreviewFile.EndsWith("VM"))
            {
                ViewData["filename"] = vm.ModelName + $"{(vm.IsApi == true ? "Api" : "")}" + vm.PreviewFile.Replace("CrudVM", "VM") + ".cs";
                ViewData["code"] = vm.GenerateVM(vm.PreviewFile);
            }
            else if (vm.UI == UIEnum.React)
            {
                if (vm.PreviewFile == "storeindex")
                {
                    ViewData["code"] = vm.GetResource("index.txt", "Spa.React.store").Replace("$modelname$", vm.ModelName.ToLower());
                }
                else if (vm.PreviewFile == "index")
                {
                    ViewData["code"] = vm.GetResource("index.txt", "Spa.React").Replace("$modelname$", vm.ModelName.ToLower());
                }
                else if (vm.PreviewFile == "style")
                {
                    ViewData["code"] = vm.GetResource("style.txt", "Spa.React").Replace("$modelname$", vm.ModelName.ToLower());
                }
                else
                {
                    ViewData["code"] = vm.GenerateReactView(vm.PreviewFile);
                }
            }
            else if (vm.UI == UIEnum.VUE)
            {
                List<string> apineeded = new List<string>();
                ViewData["code"] = vm.GenerateVUEView(vm.PreviewFile, apineeded);
            }
            else if (vm.UI == UIEnum.Blazor)
            {
                ViewData["code"] = vm.GenerateBlazorView(vm.PreviewFile);
            }
            else if (vm.PreviewFile.EndsWith("View"))
            {
                ViewData["filename"] = vm.PreviewFile.Replace("ListView", "Index").Replace("View", "") + "cshtml";
                ViewData["code"] = vm.GenerateView(vm.PreviewFile);
            }
            return PartialView(vm);
        }

        private List<Type> GetAllModels()
        {
            var models = new List<Type>();

            //获取所有模型
            var pros = Wtm.ConfigInfo.Connections.SelectMany(x => x.DcConstructor.DeclaringType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance));
            if (pros != null)
            {
                foreach (var pro in pros)
                {
                    if (pro.PropertyType.IsGeneric(typeof(DbSet<>)))
                    {
                        models.Add(pro.PropertyType.GetGenericArguments()[0]);
                    }
                }
            }
            return models;
        }

    }
}