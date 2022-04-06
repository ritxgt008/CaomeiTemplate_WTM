using System.Text.Json.Serialization;

namespace Caomei.Core.ConfigOptions
{
    public class UEditorOptions
    {
        #region 上传图片配置项

        /// <summary>
        /// 执行上传图片的action名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageActionName")]
        public string ImageActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 提交的图片表单名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageFieldName")]
        public string ImageFieldName { get; set; } = "FileID";

        /// <summary>
        /// 上传大小限制，单位B
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageMaxSize")]
        public int ImageMaxSize { get; set; } = 2048000;

        /// <summary>
        /// 上传图片格式显示
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageAllowFiles")]
        public string[] ImageAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

        /// <summary>
        /// 是否压缩图片,默认是true
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageCompressEnable")]
        public bool ImageCompressEnable { get; set; } = true;

        /// <summary>
        /// 图片压缩最长边限制
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageCompressBorder")]
        public int ImageCompressBorder { get; set; } = 1600;

        /// <summary>
        /// 插入的图片浮动方式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageInsertAlign")]
        public string ImageInsertAlign { get; set; } = "none";

        /// <summary>
        /// 图片访问路径前缀 默认返回全路径
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageUrlPrefix")]
        public string ImageUrlPrefix { get; set; } = string.Empty;

        /* {filename} 会替换成原文件名,配置这项需要注意中文乱码问题 */
        /* {rand:6} 会替换成随机数,后面的数字是随机数的位数 */
        /* {time} 会替换成时间戳 */
        /* {yyyy} 会替换成四位年份 */
        /* {yy} 会替换成两位年份 */
        /* {mm} 会替换成两位月份 */
        /* {dd} 会替换成两位日期 */
        /* {hh} 会替换成两位小时 */
        /* {ii} 会替换成两位分钟 */
        /* {ss} 会替换成两位秒 */
        /* 非法字符 \ : * ? " < > | */
        /* 具请体看线上文档: fex.baidu.com/ueditor/#use-format_upload_filename */

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imagePathFormat")]
        public string ImagePathFormat { get; set; } = "upload/image/{yyyy}{mm}{dd}/{time}{rand:6}";

        #endregion 上传图片配置项

        #region 涂鸦图片上传配置项

        /// <summary>
        /// 执行上传涂鸦的action名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("scrawlActionName")]
        public string ScrawlActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 提交的图片表单名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("scrawlFieldName")]
        public string ScrawlFieldName { get; set; } = "FileID";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("scrawlPathFormat")]
        public string ScrawlPathFormat { get; set; } = "upload/image/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 上传大小限制，单位B
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("scrawlMaxSize")]
        public int ScrawlMaxSize { get; set; } = 2048000;

        /// <summary>
        /// 图片访问路径前缀
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("scrawlUrlPrefix")]
        public string ScrawlUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 插入的图片浮动方式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("scrawlInsertAlign")]
        public string ScrawlInsertAlign { get; set; } = "none";

        #endregion 涂鸦图片上传配置项

        #region 截图工具上传

        /// <summary>
        /// 执行上传截图的action名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("snapscreenActionName")]
        public string SnapscreenActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("snapscreenPathFormat")]
        public string SnapscreenPathFormat { get; set; } = "upload/image/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 图片访问路径前缀
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("snapscreenUrlPrefix")]
        public string SnapscreenUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 插入的图片浮动方式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("snapscreenInsertAlign")]
        public string SnapscreenInsertAlign { get; set; } = "none";

        #endregion 截图工具上传

        #region 抓取远程图片配置

        [JsonPropertyName("catcherLocalDomain")]
        public string[] CatcherLocalDomain { get; set; } = new string[] { "127.0.0.1", "localhost", "img.baidu.com" };

        /// <summary>
        /// 执行抓取远程图片的action名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("catcherActionName")]
        public string CatcherActionName { get; set; } = "catchimage";

        /// <summary>
        /// 提交的图片列表表单名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("catcherFieldName")]
        public string CatcherFieldName { get; set; } = "source";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("catcherPathFormat")]
        public string CatcherPathFormat { get; set; } = "upload/image/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 图片访问路径前缀
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("catcherUrlPrefix")]
        public string CatcherUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 上传大小限制，单位B
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("catcherMaxSize")]
        public int CatcherMaxSize { get; set; } = 2048000;

        /// <summary>
        /// 抓取图片格式显示
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("catcherAllowFiles")]
        public string[] CatcherAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

        #endregion 抓取远程图片配置

        #region 上传视频配置

        /// <summary>
        /// 执行上传视频的action名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("videoActionName")]
        public string VideoActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 提交的视频表单名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("videoFieldName")]
        public string VideoFieldName { get; set; } = "FileID";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("videoPathFormat")]
        public string VideoPathFormat { get; set; } = "upload/video/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 视频访问路径前缀
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("videoUrlPrefix")]
        public string VideoUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 上传大小限制，单位B，默认100MB
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("videoMaxSize")]
        public int VideoMaxSize { get; set; } = 102400000;

        /// <summary>
        /// 上传视频格式显示
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("videoAllowFiles")]
        public string[] VideoAllowFiles { get; set; } = new string[] { ".flv", ".swf", ".mkv", ".avi", ".rm", ".rmvb", ".mpeg", ".mpg", ".ogg", ".ogv", ".mov", ".wmv", ".mp4", ".webm", ".mp3", ".wav", ".mid" };

        #endregion 上传视频配置

        #region 上传文件配置

        /// <summary>
        /// controller里,执行上传视频的action名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileActionName")]
        public string FileActionName { get; set; } = "UploadForLayUIUEditor";

        /// <summary>
        /// 提交的文件表单名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileFieldName")]
        public string FileFieldName { get; set; } = "FileID";

        /// <summary>
        /// 上传保存路径,可以自定义保存路径和文件名格式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("filePathFormat")]
        public string FilePathFormat { get; set; } = "upload/file/{yyyy}{mm}{dd}/{time}{rand:6}";

        /// <summary>
        /// 文件访问路径前缀
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileUrlPrefix")]
        public string FileUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 上传大小限制，单位B，默认50MB
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileMaxSize")]
        public int FileMaxSize { get; set; } = 51200000;

        /// <summary>
        /// 上传文件格式显示
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileAllowFiles")]
        public string[] FileAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".flv", ".swf", ".mkv", ".avi", ".rm", ".rmvb", ".mpeg", ".mpg", ".ogg", ".ogv", ".mov", ".wmv", ".mp4", ".webm", ".mp3", ".wav", ".mid", ".rar", ".zip", ".tar", ".gz", ".7z", ".bz2", ".cab", ".iso", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".txt", ".md", ".xml" };

        #endregion 上传文件配置

        #region 列出指定目录下的图片

        /// <summary>
        /// 执行图片管理的action名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageManagerActionName")]
        public string ImageManagerActionName { get; set; } = "listimage";

        /// <summary>
        /// 指定要列出图片的目录
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageManagerListPath")]
        public string ImageManagerListPath { get; set; } = string.Empty;

        /// <summary>
        /// 每次列出文件数量
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageManagerListSize")]
        public int ImageManagerListSize { get; set; } = 20;

        /// <summary>
        /// 图片访问路径前缀
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageManagerUrlPrefix")]
        public string ImageManagerUrlPrefix { get; set; } = string.Empty;

        /// <summary>
        /// 插入的图片浮动方式
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageManagerInsertAlign")]
        public string ImageManagerInsertAlign { get; set; } = "none";

        /// <summary>
        /// 列出的文件类型
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("imageManagerAllowFiles")]
        public string[] ImageManagerAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };

        #endregion 列出指定目录下的图片

        #region 列出指定目录下的文件

        /// <summary>
        /// 执行文件管理的action名称
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileManagerActionName")]
        public string FileManagerActionName { get; set; } = "listfile";

        /// <summary>
        /// 指定要列出文件的目录
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileManagerListPath")]
        public string FileManagerListPath { get; set; } = "upload/file";

        /// <summary>
        /// 文件访问路径前缀
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileManagerUrlPrefix")]
        public string FileManagerUrlPrefix { get; set; } = "/ueditor/net/";

        /// <summary>
        /// 每次列出文件数量
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileManagerListSize")]
        public int FileManagerListSize { get; set; } = 20;

        /// <summary>
        /// 列出的文件类型
        /// </summary>
        /// <value> </value>
        [JsonPropertyName("fileManagerAllowFiles")]
        public string[] FileManagerAllowFiles { get; set; } = new string[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".flv", ".swf", ".mkv", ".avi", ".rm", ".rmvb", ".mpeg", ".mpg", ".ogg", ".ogv", ".mov", ".wmv", ".mp4", ".webm", ".mp3", ".wav", ".mid", ".rar", ".zip", ".tar", ".gz", ".7z", ".bz2", ".cab", ".iso", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".txt", ".md", ".xml" };

        #endregion 列出指定目录下的文件
    }
}