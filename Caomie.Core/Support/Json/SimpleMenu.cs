using System;

namespace Caomei.Core.Support.Json
{
    [Serializable]
    public class SimpleMenu
    {
        public Guid ID { get; set; }
        public bool? IsInherit { get; set; }

        public Guid? ActionId { get; set; }

        public bool? IsPublic { get; set; }

        public string Url { get; set; }

        public Guid? ParentId { get; set; }

        public string PageName { get; set; }

        public int? DisplayOrder { get; set; }

        public string Icon { get; set; }

        public bool ShowOnMenu { get; set; }
    }

    public class SimpleMenuApi
    {
        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public string Icon { get; set; }
    }
}