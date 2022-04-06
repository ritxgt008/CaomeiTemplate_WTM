using System;

namespace Caomei.Core.Support.Json
{
    [Serializable]
    public class SimpleRole
    {
        public Guid ID { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
    }
}