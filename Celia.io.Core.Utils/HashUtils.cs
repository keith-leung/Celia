using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Celia.io.Core.Utils
{
    public class HashUtils
    {
        private static MD5 _md5;

        static HashUtils()
        {
            _md5 = new MD5CryptoServiceProvider();
        }

        public static string GetMd5String(string sourceStr)
        {
            if (string.IsNullOrEmpty(sourceStr))
                throw new ArgumentNullException(nameof(sourceStr));
            byte[] result = Encoding.UTF8.GetBytes(sourceStr);

            byte[] output = _md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }
    }
}
