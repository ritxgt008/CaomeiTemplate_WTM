// // DistributedCacheExtensions.cs // Author: Michael,Vito // Copyright (c) 2019 WTM // Permission
// is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions: // The above copyright notice and this permission notice
// shall be included in all copies or substantial portions of the Software. // THE SOFTWARE IS
// PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO
// EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Caomei.Core.Extensions
{
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// CachingKey 分割符
        /// </summary>
        private const string SPLIT_CHAR = ":";

        private static string _instanceName;

        private static string InstanceName
        {
            get
            {
                if (_instanceName == null)
                {
                    _instanceName = Assembly.GetEntryAssembly().GetName().Name + SPLIT_CHAR;
                }
                return _instanceName;
            }
        }

        public static void SetInstanceName(
            this IDistributedCache cache,
            string instanceName)
        {
            _instanceName = instanceName + SPLIT_CHAR;
        }

        #region Get

        public static T Get<T>(
            this IDistributedCache cache,
            string key)
        {
            var value = cache.GetString(InstanceName + key.ToLower());
            if (value == null)
                return default;
            else
                return JsonSerializer.Deserialize<T>(value, Core.CoreProgram.DefaultJsonOption);
        }

        public static async Task<T> GetAsync<T>(
            this IDistributedCache cache,
            string key,
            CancellationToken token = default)
        {
            var value = await cache.GetStringAsync(InstanceName + key.ToLower(), token);
            if (value == null)
                return default;
            else
                return JsonSerializer.Deserialize<T>(value, Core.CoreProgram.DefaultJsonOption);
        }

        public static bool TryGetValue<T>(
            this IDistributedCache cache,
            string key,
            out T outValue)
        {
            var value = cache.GetString(InstanceName + key.ToLower());
            if (value == null)
            {
                outValue = default;
                return false;
            }
            else
            {
                outValue = JsonSerializer.Deserialize<T>(value, Core.CoreProgram.DefaultJsonOption);
                return true;
            }
        }

        #endregion Get

        #region Set

        public static void Add<T>(
            this IDistributedCache cache,
            string key,
            T value,
            DistributedCacheEntryOptions options = null)
        {
            if (options == null)
                cache.Set(InstanceName + key.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, CoreProgram.DefaultJsonOption)));
            else
                cache.Set(InstanceName + key.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, CoreProgram.DefaultJsonOption)), options);
        }

        public static async Task AddAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            DistributedCacheEntryOptions options = null,
            CancellationToken token = default)
        {
            if (options == null)
                await cache.SetAsync(InstanceName + key.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, CoreProgram.DefaultJsonOption)), token);
            else
                await cache.SetAsync(InstanceName + key.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, CoreProgram.DefaultJsonOption)), options, token);
        }

        #endregion Set

        #region Delete

        public static void Delete(
            this IDistributedCache cache,
            string key)
        {
            cache.Remove(InstanceName + key.ToLower());
        }

        public static async Task DeleteAsync(
            this IDistributedCache cache,
            string key,
            CancellationToken token = default)
        {
            await cache.RemoveAsync(InstanceName + key.ToLower(), token);
        }

        #endregion Delete
    }
}