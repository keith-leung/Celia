using Aliyun.OSS;
using Celia.io.Core.Utils;
using Celia.io.Core.StaticObjects.Abstractions;
using Celia.io.Core.StaticObjects.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Celia.io.Core.StaticObjects.Storages.Default
{
    public class AliyunStorageProvider : IStorageProvider
    {
        private DisconfService _disconfService = null;

        public AliyunStorageProvider(DisconfService disconfService)
        {
            _disconfService = disconfService ?? throw new ArgumentNullException(nameof(disconfService));
        }

        internal bool IsDebug
        {
            get
            {
                bool isDebug = false;
                object isDebugObj = null;
                if (_disconfService.CustomConfigs.TryGetValue("IsDebugMode", out isDebugObj))
                {
                    bool.TryParse(isDebugObj.ToString(), out isDebug);
                }

                return isDebug;
            }
        }

        public const string STORAGE_PROVIDER_TYPE_KEY = "STORAGE_PROVIDER_TYPE_ALIYUN";

        public const string DEFAULT_CONTAINER = "defaultcontainer";

        public static readonly Type STORAGE_PROVIDER_TYPE_VALUE = typeof(AliyunStorageProvider);

        /*
        public string GetUrlByFormatSizeQuality(string storageId, string storageAccessKey,
            MediaElementUrlType urlType, string downloadHost, string filePath, string fileName,
            string format, int maxWidthHeight, int percentage)
        {
            string sizeQualityString = BuildFormatSizeAndPercentage(format, maxWidthHeight, percentage);

            return GetUrlCustom(storageId, storageAccessKey, urlType,
                downloadHost, filePath, fileName, sizeQualityString);
        }

        public string GetUrlByStyle(string storageId, string storageAccessKey, MediaElementUrlType urlType,
            string downloadHost, string filePath, string fileName, string styleName)
        {
            return GetUrlCustom(storageId, storageAccessKey, urlType,
                downloadHost, filePath, fileName,
                string.IsNullOrEmpty(styleName) ? "" : $"x-oss-process=style/{styleName}");
        }

        public string GetUrlCustom(string storageId, string storageAccessKey, MediaElementUrlType urlType,
            string downloadHost, string filePath, string fileName, string customStyleProcessStr)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = DEFAULT_CONTAINER;
            }
            filePath = filePath.Trim('/');
            fileName = fileName.Trim('/');

            if (urlType == MediaElementUrlType.PublishOutputUrl
                || urlType == MediaElementUrlType.PublishDownloadUrl)
            {
                downloadHost = downloadHost.Trim('/');
                if (!string.IsNullOrEmpty(customStyleProcessStr))
                {
                    customStyleProcessStr = "?" + customStyleProcessStr;
                }
                return $"{downloadHost}/{filePath}/{fileName}{customStyleProcessStr}";
            }
            else
            {
                StorageMode mode = StorageMode.Internal;
                if (urlType == MediaElementUrlType.OutputUrl)
                    mode = StorageMode.External;
                string bucketName, objectName;
                OssClient client;
                GetClient(mode, storageId, storageAccessKey, filePath, fileName,
                    out bucketName, out objectName, out client);

                string objectKey = //($"{filePath}/{fileName}"); 
                    ($"{filePath}/{fileName}?{customStyleProcessStr}");
                    //.Trim(new char[] { '/' });

                var presignedUri = client.GeneratePresignedUri(bucketName, objectKey,
                    DateTime.Now.AddHours(1), SignHttpMethod.Get);

                return presignedUri.AbsoluteUri;
            }
        }*/



        public string GetUrlByFormatSizeQuality(IStorageInfo storageInfo, MediaElementUrlType urlType,
            string filePath, string fileName, string format, int maxWidthHeight, int percentage)
        {
            string sizeQualityString = BuildFormatSizeAndPercentage(format, maxWidthHeight, percentage);

            return GetUrlCustom(storageInfo, urlType, filePath, fileName, sizeQualityString);
        }

        public string GetUrlByStyle(IStorageInfo storageInfo, MediaElementUrlType urlType,
            string filePath, string fileName, string styleName)
        {
            return GetUrlCustom(storageInfo, urlType, filePath, fileName,
                string.IsNullOrEmpty(styleName) ? "" : $"style/{styleName}");
        }

        public string GetUrlCustom(IStorageInfo storageInfo, MediaElementUrlType urlType, string filePath,
            string fileName, string customStyleProcessStr)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = DEFAULT_CONTAINER;
            }
            filePath = filePath.Trim('/');
            fileName = fileName.Trim('/');

            if (urlType == MediaElementUrlType.PublishOutputUrl
                || urlType == MediaElementUrlType.PublishDownloadUrl)
            {
                var downloadHost = storageInfo.StorageHost.Trim('/');
                if (!string.IsNullOrEmpty(customStyleProcessStr))
                {
                    customStyleProcessStr = "?x-oss-process=" + customStyleProcessStr;
                }
                return $"{downloadHost}/{filePath}/{fileName}{customStyleProcessStr}";
            }
            else
            {
                string bucketName, objectName;
                OssClient client;
                GetClient(storageInfo.StorageId, storageInfo.StorageAccessKey,
                    storageInfo.NetType, filePath, fileName,
                    out bucketName, out objectName, out client);

                //string objectKey = //($"{filePath}/{fileName}"); 
                //    ($"{filePath}/{fileName}?{customStyleProcessStr}");
                //.Trim(new char[] { '/' });

                GeneratePresignedUriRequest request = new GeneratePresignedUriRequest(bucketName, objectName);
                request.Process = customStyleProcessStr;
                request.Expiration = DateTime.Now.AddHours(1);
                request.Method = SignHttpMethod.Get;

                var presignedUri = client.GeneratePresignedUri(request);

                return presignedUri.AbsoluteUri;
            }
        }

        private void GetClient(string storageId, string storageAccessKey, NetType netType,
            string filePath, string fileName, out string bucketName, out string objectName, out OssClient client)
        {
            JObject jobject = JObject.Parse(storageAccessKey);

            string endpoint = ExtractEndPoint(netType, jobject);// "<yourEndpoint>";
            string accessKeyId = ExtractAccessKeyId(jobject); //"<yourAccessKeyId>";
            string accessKeySecret = ExtractAccessKeySecret(jobject); //"<yourAccessKeySecret>";
            bucketName = storageId;
            objectName = (!string.IsNullOrEmpty(filePath) ? filePath.Trim('/') + "/" : string.Empty)
                + fileName;
            //$"{filePath.Trim(new char[] { '/', '\\' })}/{fileName}";
            //Path.Combine(filePath, fileName);
            //var localFilename = "<yourLocalFilename>";
            // 创建OssClient实例。
            client = new OssClient(endpoint, accessKeyId, accessKeySecret);
        }

        public Stream GetStream(string storageId, string storageAccessKey, StorageMode mode,
            string downloadHost, string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = DEFAULT_CONTAINER;
            }

            string bucketName, objectName;
            OssClient client;
            GetClient(mode, storageId, storageAccessKey, filePath, fileName,
                out bucketName, out objectName, out client);

            var obj = client.GetObject(bucketName, objectName);
            if (obj != null)
            {
                return obj.ResponseStream;
            }

            return null;
        }

        public async Task RemoveFileAsync(string storageId, string storageAccessKey,
             StorageMode mode, string downloadHost, string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = DEFAULT_CONTAINER;
            }

            string bucketName, objectName;
            OssClient client;
            GetClient(mode, storageId, storageAccessKey, filePath, fileName,
                out bucketName, out objectName, out client);

            client.DeleteObject(bucketName, objectName);
            Console.WriteLine("Delete object succeeded");
        }

        private void GetClient(StorageMode mode, string storageId, string storageAccessKey,
            string filePath, string fileName, out string bucketName, out string objectName, out OssClient client)
        {
            JObject jobject = JObject.Parse(storageAccessKey);

            string endpoint = ExtractEndPoint(mode, jobject);// "<yourEndpoint>";
            string accessKeyId = ExtractAccessKeyId(jobject); //"<yourAccessKeyId>";
            string accessKeySecret = ExtractAccessKeySecret(jobject); //"<yourAccessKeySecret>";
            bucketName = storageId;
            objectName = $"{filePath.Trim(new char[] { '/', '\\' })}/{fileName}";
            //Path.Combine(filePath, fileName);
            //var localFilename = "<yourLocalFilename>";
            // 创建OssClient实例。
            client = new OssClient(endpoint, accessKeyId, accessKeySecret);
        }

        public async Task UploadFileAsync(Stream stream, string storageId, string storageAccessKey,
            StorageMode mode, string downloadHost, string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = DEFAULT_CONTAINER;
            }

            string bucketName, objectName;
            OssClient client;
            GetClient(mode, storageId, storageAccessKey, filePath, fileName,
                out bucketName, out objectName, out client);

            // 上传文件。
            PutObjectResult result = client.PutObject(bucketName, objectName, stream);
            if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Put object succeeded");
            }
        }

        private string BuildFormatSizeAndPercentage(string format, int maxWidthHeight, int percentage)
        {
            string formatStr = "";
            //$"x-oss-process=image/resize,m_mfit,h_{maxWidthHeight},w_{maxWidthHeight}";
            if (maxWidthHeight <= 4000 && maxWidthHeight > 1)
            {
                formatStr = formatStr + $"/resize,m_mfit,h_{maxWidthHeight},w_{maxWidthHeight}";
            }
            if (!string.IsNullOrEmpty(format))
            {
                formatStr = formatStr + $"/format,{format}";
            }
            if (percentage > 1 && percentage < 100)
            {
                formatStr = formatStr + $"/quality,q_{percentage}";
            }

            if (!string.IsNullOrEmpty(formatStr))
            {
                formatStr = "image" + formatStr;
                return formatStr;
            }

            return string.Empty;
        }

        private string ExtractAccessKeySecret(JObject keyObject)
        {
            return keyObject.Value<string>("SecretKey");
        }

        private string ExtractAccessKeyId(JObject keyObject)
        {
            return keyObject.Value<string>("AccessKey");
        }

        private string ExtractEndPoint(NetType netType, JObject keyObject)
        {
            if (IsDebug)
            {
                return keyObject.Value<string>("OutputEndPoint");
            }

            if (netType == NetType.Internet)
            {
                return keyObject.Value<string>("OutputEndPoint");
            }
            return keyObject.Value<string>("DownloadEndPoint");
        }

        private string ExtractEndPoint(StorageMode mode, JObject keyObject)
        {
            if (IsDebug)
            {
                return keyObject.Value<string>("OutputEndPoint");
            }

            if (mode == StorageMode.External)
            {
                return keyObject.Value<string>("OutputEndPoint");
            }
            return keyObject.Value<string>("DownloadEndPoint");
        }
    }
}
