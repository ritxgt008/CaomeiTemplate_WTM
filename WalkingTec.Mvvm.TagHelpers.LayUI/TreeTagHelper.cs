using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:tree", TagStructure = TagStructure.WithoutEndTag)]
    public class TreeTagHelper : BaseFieldTag
    {
        public ModelExpression Items { get; set; }
        public bool ShowLine { get; set; } = true;

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <summary>
        /// 点击时触发的js函数名，func(data)格式;
        /// <para>
        /// data.elem得到当前节点元素;
        /// </para>
        /// <para>
        /// data.data得到当前点击的节点数据
        /// </para>
        /// <para>
        /// data.state得到当前节点的展开状态：open、close、normal
        /// </para>
        /// </summary>
        public string ClickFunc { get; set; }

        /// <summary>
        /// 勾选事件
        /// </summary>
        /// <summary>
        /// 勾选时触发的js函数名，func(data)格式;
        /// <para>
        /// data.elem得到当前节点元素;
        /// </para>
        /// <para>
        /// data.data得到当前点击的节点数据
        /// </para>
        /// <para>
        /// data.checked是否被选中
        /// </para>
        /// </summary>
        public string CheckFunc { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            bool MultiSelect = false;
            var type = Field.Metadata.ModelType;
            if (type.IsArray || (type.IsGenericType && typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition())))// Array or List
            {
                MultiSelect = true;
            }
            string oncheck = "";
            string onclick = "";
            if (MultiSelect != true)
            {
                var formid = "";
                if (context.Items.ContainsKey("formid"))
                {
                    formid = $",'{context.Items["formid"]}form'";
                }

                onclick = $@"
                ,click: function(data){{
                    var ele = data.elem.find('.layui-tree-main:first');
                    if(last{Id} != null){{
                        last{Id}.css('background-color','');
                        last{Id}.find('.layui-tree-txt').css('color','');
                    }}
                    $('#tree{Id}hidden').html('');
                    if(last{Id} != null && last{Id}.is(ele)){{
                        last{Id} = null;
                    }}
                    else{{
                        ele.css('background-color','#5fb878');
                        ele.find('.layui-tree-txt').css('color','#fff');
                        $('#tree{Id}hidden').append(""<input type='hidden' name='{Field?.Name}' value='""+data.data.id+""'/>"");
                        last{Id} = ele;
                    }}
                    {FormatFuncName(CheckFunc)};
                  }}";
            }
            else
            {
                onclick = $@"
,click: function(data){{
    {FormatFuncName(ClickFunc)};
  }}";
            }
            oncheck = $@"
                ,oncheck: function(data){{
                    if(loaded{Id} == false) return;
                    var checkData = layui.tree.getChecked('{Id}');
                    var ids = ff.getTreeChecked(checkData);
                    $('#tree{Id}hidden').html('');
                    for(var i=0;i<ids.length;i++){{
                        $('#tree{Id}hidden').append(""<input type='hidden' name='{Field?.Name}' value='""+ids[i]+""'/>"");
                    }}
                    {FormatFuncName(CheckFunc)};
                  }}";

            output.TagName = "div";
            output.Attributes.Add("id", "div" + Id);
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("wtm-ctype", "tree");
            output.Attributes.Add("wtm-name", Field.Name);
            Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToNoSplitString() : Id;

            List<object> vals = new List<object>();
            if (Field?.Model != null)
            {
                if (MultiSelect == true)
                {
                    foreach (var item in Field.Model as dynamic)
                    {
                        vals.Add(item.ToString());
                    }
                }
                else
                {
                    vals.Add(Field.Model.ToString());
                }
            }
            List<LayuiTreeItem> treeitems = new List<LayuiTreeItem>();

            if (string.IsNullOrEmpty(ItemUrl) == true && Items?.Model is List<TreeSelectListItem> mm)
            {
                treeitems = GetLayuiTree(mm, vals);
            }
            var script = $@"
<script>
layui.use(['tree'],function(){{
  var last{Id} = null;
  var loaded{Id} = false;
  layui.tree.render({{
    id:'{Id}',elem: '#div{Id}',onlyIconControl:{(!MultiSelect).ToString().ToLower()}, showCheckbox:{MultiSelect.ToString().ToLower()},showLine:{ShowLine.ToString().ToLower()}
    ,data: {JsonSerializer.Serialize(treeitems)} {oncheck} {onclick}
  }});
  loaded{Id} = true;";

            var defaultselect = "";
            if (Field.Model != null)
            {
                defaultselect = $@"
    var selected = $(""div[data-id='{Field.Model.ToString()}']"");
    var selected2 = selected.find('.layui-tree-main:first');
    selected2.css('background-color','#5fb878');
    selected2.find('.layui-tree-txt').css('color','#fff');
    last{Id} = selected2;
";
            }

            if (MultiSelect == false && Field.Model != null)
            {
                script += defaultselect;
            }
            script += $@"
}})
</script>
";
            output.PostElement.AppendHtml(script);
            if (string.IsNullOrEmpty(ItemUrl) == false)
            {
                output.PostElement.AppendHtml($@"<script>
ff.LoadComboItems('tree','{ItemUrl}','{Id}','{Field.Name}',{JsonSerializer.Serialize(vals)},function(){{
    {defaultselect}
}})

</script>");
            }
            string hidden = $"<p id='tree{Id}hidden'>";
            if (Field?.Model != null)
            {
                if (MultiSelect == true)
                {
                    foreach (var item in Field.Model as dynamic)
                    {
                        hidden += $@"
<input type='hidden' name='{Field?.Name}' value='{item.ToString()}'/>";
                    }
                }
                else
                {
                    hidden += $"<input type='hidden' name='{Field?.Name}' value='{Field.Model}'/>";
                }
                hidden += " </p>";
            }

            output.PostElement.AppendHtml(hidden);

            base.Process(context, output);
        }

        private List<LayuiTreeItem> GetLayuiTree(IEnumerable<TreeSelectListItem> tree, List<object> values)
        {
            List<LayuiTreeItem> rv = new List<LayuiTreeItem>();
            foreach (var s in tree)
            {
                var news = new LayuiTreeItem
                {
                    Id = s.Value.ToString(),
                    Title = s.Text,
                    Url = s.Url,
                    Expand = s.Expended,
                    //Children = new List<LayuiTreeItem>()
                };
                if (values.Contains(s.Value.ToString()))
                {
                    news.Checked = true;
                }
                if (s.Children != null && s.Children.Count() > 0)
                {
                    news.Children = GetLayuiTree(s.Children, values);
                    if (news.Children.Where(x => x.Checked == true || x.Expand == true).Count() > 0)
                    {
                        news.Expand = true;
                    }
                }
                rv.Add(news);
            }
            return rv;
        }
    }
}