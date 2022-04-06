using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Caomei.Core
{
    public class CS
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DBTypeEnum? DbType { get; set; }
        public string Version { get; set; }
        public string DbContext { get; set; }

        public ConstructorInfo DcConstructor;

        public IDataContext CreateDC()
        {
            if (DcConstructor == null)
            {
                var AllAssembly = Utils.GetAllAssembly();
                List<ConstructorInfo> cis = new List<ConstructorInfo>();
                if (AllAssembly != null)
                {
                    foreach (var ass in AllAssembly)
                    {
                        try
                        {
                            var t = ass.GetExportedTypes().Where(x => typeof(DbContext).IsAssignableFrom(x) && x.Name != "DbContext" && x.Name != "FrameworkContext" && x.Name != "EmptyContext").ToList();
                            foreach (var st in t)
                            {
                                var ci = st.GetConstructor(new Type[] { typeof(CS) });
                                if (ci != null)
                                {
                                    cis.Add(ci);
                                }
                            }
                        }
                        catch { }
                    }
                    string dcname = DbContext;
                    if (string.IsNullOrEmpty(dcname))
                    {
                        dcname = "DataContext";
                    }
                    DcConstructor = cis.Where(x => x.DeclaringType.Name.ToLower() == dcname.ToLower()).FirstOrDefault();
                    if (DcConstructor == null)
                    {
                        DcConstructor = cis.FirstOrDefault();
                    }
                }
            }
            return (IDataContext)DcConstructor?.Invoke(new object[] { this });
        }
    }
}