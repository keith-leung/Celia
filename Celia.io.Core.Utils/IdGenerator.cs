using System;
using SequentialGuid;
using System.Collections.Generic;
using System.Text;

namespace Celia.io.Core.Utils
{
    /// <summary>
    /// 用于生成适用于分布式微服务的ID，使用MongoDB规则，基本有序且保证不冲突。
    /// 新项目建议使用GenerateObjectId，旧项目可以使用GenerateGuid进行与Guid无缝替换。
    /// </summary>
    public class IdGenerator
    {
        /// <summary>
        /// 新项目建议使用这个ObjectId方法，字符串是16进制
        /// </summary>
        /// <returns>MongoDB ObjectId</returns>
        public static ObjectId GenerateObjectId()
        {
            return ObjectId.GenerateNewId();
        }

        /// <summary>
        /// 旧项目用于无缝替换Guid的基本有序字符串ID。
        /// </summary>
        /// <returns>基本有序的Guid</returns>
        public static Guid GenerateGuid()
        {
            return SequentialGuidGenerator.Instance.NewGuid();
        }
    }
}
