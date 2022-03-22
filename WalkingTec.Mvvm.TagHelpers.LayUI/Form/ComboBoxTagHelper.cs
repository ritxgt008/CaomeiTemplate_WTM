using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:combobox", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class ComboBoxTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }

        public bool AutoComplete { get; set; }

        public string YesText { get; set; }

        public string NoText { get; set; }

        /// <summary>
        /// 启用搜索
        /// 注意：多选与搜索不能同时启用
        /// </summary>
        public bool EnableSearch { get; set; }

        public ModelExpression Items { get; set; }

        public ModelExpression LinkField { get; set; }

        public string LinkId { get; set; }
        public string TriggerUrl { get; set; }

        /// <summary>
        /// 是否多选
        /// 默认根据Field 绑定的值类型进行判断。Array or List 即多选，否则单选
        /// 注意：多选与搜索不能同时启用
        /// </summary>
        public bool? MultiSelect { get; set; }

        /// <summary>
        /// 改变选择时触发的js函数，func(data)格式;
        /// <para>
        /// data.elem得到select原始DOM对象;
        /// </para>
        /// <para>
        /// data.value得到被选中的值;
        /// </para>
        /// <para>
        /// data.othis得到美化后的DOM对象;
        /// </para>
        /// </summary>
        public string ChangeFunc { get; set; }

        private WTMContext _wtm;

        public ComboBoxTagHelper(IOptionsMonitor<Configs> configs, WTMContext wtm)
        {
            if (EmptyText == null)
            {
                EmptyText = THProgram._localizer["Sys.PleaseSelect"];
            }
            EnableSearch = configs.CurrentValue.UIOptions.ComboBox.DefaultEnableSearch;
            _wtm = wtm;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("name", Field.Name);
            output.Attributes.Add("lay-filter", $"_WTMMultiCombo_{Guid.NewGuid()}_" + Field.Name);
            output.Attributes.Add("wtm-name", Field.Name);
            output.Attributes.Add("wtm-ctype", "combo");
            if (Disabled == true)
            {
                output.Attributes.Add("disabled", "disabled");
            }

            var modeltype = Field.Metadata.ModelType;
            if (MultiSelect == null)
            {
                MultiSelect = false;
                if (Field.Name.Contains("[") || modeltype.IsArray || modeltype.IsList())// Array or List
                {
                    MultiSelect = true;
                }
            }
            if (MultiSelect.Value)
            {
                output.Attributes.Add("wtm-combo", "MULTI_COMBO");
            }
            if (!MultiSelect.Value && EnableSearch)
            {
                output.Attributes.Add("lay-search", string.Empty);
            }
            if (string.IsNullOrEmpty(ChangeFunc) == false)
            {
                output.Attributes.Add("wtm-cf", FormatFuncName(ChangeFunc, false));
            }
            if (LinkField != null || string.IsNullOrEmpty(LinkId) == false)
            {
                var linkto = "";
                if (string.IsNullOrEmpty(LinkId))
                {
                    linkto = Core.Utils.GetIdByName(LinkField.ModelExplorer.Container.ModelType.Name + "." + LinkField.Name);
                }
                else
                {
                    linkto = LinkId;
                }
                output.Attributes.Add("wtm-linkto", $"{linkto}");
            }
            if (TriggerUrl != null)
            {
                output.Attributes.Add("wtm-turl", TriggerUrl);
            }
            var contentBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(EmptyText) == false)
            {
                contentBuilder.Append($"<option value=''>{EmptyText}</option>");
            }

            output.PreElement.AppendHtml($@"<input type=""hidden"" name=""_DONOTUSE_{Field.Name}"" value=""1"" />");

            #region 添加下拉数据 并 设置默认选中

            var listItems = new List<ComboSelectListItem>();
            var selectVal = new List<string>();
            if (Field.Name.Contains("[") && modeltype.IsList() == false && modeltype.IsArray == false)
            {
                //默认多对多不必填
                if (Required == null)
                {
                    Required = false;
                }
                selectVal.AddRange(Field.ModelExplorer.Container.Model.GetPropertySiblingValues(Field.Name));
            }
            else if (Field.Model != null)
            {
                if (modeltype.IsArray || (modeltype.IsGenericType && typeof(List<>).IsAssignableFrom(modeltype.GetGenericTypeDefinition())))
                {
                    foreach (var item in Field.Model as dynamic)
                    {
                        selectVal.Add(item.ToString().ToLower());
                    }
                }
                else
                {
                    selectVal.Add(Field.Model.ToString().ToLower());
                }
            }

            if (selectVal.Count == 0)
            {
                if (string.IsNullOrEmpty(DefaultValue) == false)
                {
                    selectVal.AddRange(DefaultValue.Split(',').Select(x => x.ToLower()));
                }
            }

            if (string.IsNullOrEmpty(ItemUrl) == false)
            {
                if (_wtm.HttpContext?.Request?.Host != null)
                {
                    ItemUrl = _wtm.HttpContext.Request.IsHttps ? "https://" : "http://" + _wtm.HttpContext?.Request?.Host.ToString() + ItemUrl;
                }
                output.PostElement.AppendHtml($"<script>ff.LoadComboItems('combo','{ItemUrl}','{Id}','{Field.Name}',{JsonSerializer.Serialize(selectVal)})</script>");
            }
            else
            {
                if (Items?.Model == null) // 添加默认下拉数据源
                {
                    var checktype = modeltype;
                    if ((modeltype.IsGenericType && typeof(List<>).IsAssignableFrom(modeltype.GetGenericTypeDefinition())))
                    {
                        checktype = modeltype.GetGenericArguments()[0];
                    }

                    if (checktype.IsEnumOrNullableEnum())
                    {
                        listItems = checktype.ToListItems(DefaultValue ?? Field.Model);
                    }
                    else if (checktype == typeof(bool) || checktype == typeof(bool?))
                    {
                        bool? df = null;
                        if (bool.TryParse(DefaultValue ?? "", out bool test) == true)
                        {
                            df = test;
                        }
                        listItems = Utils.GetBoolCombo(BoolComboTypes.Custom, df ?? (bool?)Field.Model, YesText, NoText);
                    }
                }
                else // 添加用户设置的设置源
                {
                    if (typeof(IEnumerable<ComboSelectListItem>).IsAssignableFrom(Items.Metadata.ModelType))
                    {
                        if (typeof(IEnumerable<TreeSelectListItem>).IsAssignableFrom(Items.Metadata.ModelType))
                        {
                            listItems = (Items.Model as IEnumerable<TreeSelectListItem>).FlatTreeSelectList().Cast<ComboSelectListItem>().ToList();
                        }
                        else
                        {
                            listItems = (Items.Model as IEnumerable<ComboSelectListItem>).ToList();
                        }
                        foreach (var item in listItems)
                        {
                            if (selectVal.Contains(item.Value?.ToString().ToLower()))
                            {
                                item.Selected = true;
                            }
                            else
                            {
                                item.Selected = false;
                            }
                        }
                    }
                    else if (Items.Metadata.ModelType.IsList())
                    {
                        var exports = (Items.Model as IList);
                        foreach (var item in exports)
                        {
                            listItems.Add(new ComboSelectListItem
                            {
                                Text = item?.ToString(),
                                Value = item?.ToString(),
                                Selected = selectVal.Contains(item?.ToString().ToLower())
                            });
                        }
                    }
                }
            }
            if (MultiSelect == true)
            {
                foreach (var item in listItems)
                {
                    contentBuilder.Append($"<option value='{item.Value}'{(string.IsNullOrEmpty(item.Icon) ? string.Empty : $" icon='{item.Icon}'")}>{item.Text}</option>");
                }

                // 添加默认选中项
                var selected = listItems.Where(x => x.Selected).ToList();
                var mulvalues = selected.ToSepratedString(x => x.Value, seperator: "`");
                var mulnamess = selected.ToSepratedString(x => x.Text, seperator: "`");
                output.Attributes.Add("wtm-combovalue", $"{mulvalues}");
                output.Attributes.Add("wtm-comboname", $"{mulnamess}");
            }
            else // 添加用户设置的设置源
            {
                foreach (var item in listItems)
                {
                    if (item.Selected == true)
                    {
                        if (Disabled == true)
                        {
                            output.PostElement.AppendHtml($"<input name='{Field.Name}' value='{item.Value}' text='{item.Text}' type='hidden' />");
                        }
                        contentBuilder.Append($"<option value='{item.Value}'{(string.IsNullOrEmpty(item.Icon) ? string.Empty : $" icon='{item.Icon}'")} selected>{item.Text}</option>");
                    }
                    else
                    {
                        contentBuilder.Append($"<option value='{item.Value}'{(string.IsNullOrEmpty(item.Icon) ? string.Empty : $" icon='{item.Icon}'")} {(item.Disabled == true ? "disabled=\"\"" : string.Empty)}>{item.Text}</option>");
                    }
                }
            }
            output.Content.SetHtmlContent(contentBuilder.ToString());

            #endregion 添加下拉数据 并 设置默认选中

            base.Process(context, output);
        }
    }
}