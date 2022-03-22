using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI.Form
{
    [HtmlTargetElement("wt:richtextbox", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class RichTextBoxTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }
        public string UploadUrl { get; set; }
        public string UploadGroupName { get; set; }
        public string UploadSubdir { get; set; }
        public new int? Height { get; set; }
        public string ConnectionString { get; set; }
        public string ExtraQuery { get; set; }
        public string UploadMode { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string placeHolder = EmptyText ?? "";
            output.TagName = "textarea";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("placeholder", placeHolder);
            output.Attributes.Add("style", "display:none");
            output.Attributes.Add("isrich", "1");

            if (string.IsNullOrEmpty(Field?.Model?.ToString()) == false)
            {
                DefaultValue = null;
            }

            if (DefaultValue != null)
            {
                output.Content.SetContent(DefaultValue.ToString());
            }
            else
            {
                output.Content.SetContent(Field?.Model?.ToString());
            }
            string url = UploadUrl;
            if (string.IsNullOrEmpty(url))
            {
                url = "/_framework/UploadForLayUIRichTextBox";
            }
            if (string.IsNullOrEmpty(UploadGroupName) == false)
            {
                url = url.AppendQuery($"groupName={UploadGroupName}");
            }
            if (string.IsNullOrEmpty(UploadSubdir) == false)
            {
                url = url.AppendQuery($"subdir={UploadSubdir}");
            }
            if (string.IsNullOrEmpty(UploadMode) == false)
            {
                url = url.AppendQuery($"sm={UploadMode}");
            }
            if (string.IsNullOrEmpty(ConnectionString) == true)
            {
                if (context.Items.ContainsKey("model") == true)
                {
                    var bvm = context.Items["model"] as BaseVM;
                    if (bvm?.CurrentCS != null)
                    {
                        url = url.AppendQuery($"_DONOT_USE_CS={bvm.CurrentCS}");
                    }
                }
            }
            else
            {
                url = url.AppendQuery($"_DONOT_USE_CS={ConnectionString}");
            }
            url = url.AppendQuery(ExtraQuery);

            output.PostElement.AppendHtml($@"
<script>
layui.use('layedit', function(){{
  var layedit = layui.layedit;
  layedit.set({{
    uploadImage: {{
      url: '{url}'
    }}
  }});
  var index = layedit.build('{Id}'{(Height.HasValue ? $",{{height:{Height.Value}}}" : "")});
  $('#{Id}').attr('layeditindex',index);
}});
</script>
");
            base.Process(context, output);
        }
    }
}