using Celia.io.Core.Utils;
using Celia.io.Core.StaticObjects.Abstractions;
using Celia.io.Core.StaticObjects.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Celia.io.Core.StaticObjects.Storages.Default
{
    public class AzureStorageProvider : IStorageProvider
    {
        public static readonly string[] IMAGE_FORMATS = new string[] { "jpg", "jpeg", "gif", "png", "bmp", "ico", "tif", "webp" };

        private DisconfService _disconfService = null;

        public AzureStorageProvider(DisconfService disconfService)
        {
            _disconfService = disconfService ?? throw new ArgumentNullException(nameof(disconfService));
        }

        public const string STORAGE_PROVIDER_TYPE_KEY = "STORAGE_PROVIDER_TYPE_AZURE";

        public const string INTERNAL_CONTAINER = "internal";
        public const string EXTERNAL_CONTAINER = "external";

        public const int BUFFER_SIZE = 1000000;

        public static readonly Type STORAGE_PROVIDER_TYPE_VALUE = typeof(AzureStorageProvider);

        public Stream GetStream(string storageId, string storageAccessKey, StorageMode mode,
            string downloadHost, string filePath, string fileName)
        {
            CloudBlobContainer container = GetContainer(storageId, storageAccessKey, mode, filePath);

            //if (string.IsNullOrEmpty(filePath))
            //{
            //    filePath = DEFAULT_CONTAINER;
            //}

            // write a blob to the container
            CloudBlockBlob blob = container.GetBlockBlobReference(
                Path.Combine(filePath, fileName));
            var temp = blob.ExistsAsync();
            temp.Wait();
            if (temp.Result == false)
                return null;

            var task = blob.OpenReadAsync();
            task.Wait();
            return task.Result;
        }

        public string GetUrlByFormatSizeQuality(IStorageInfo storageInfo, MediaElementUrlType urlType,
            string filePath, string fileName, string format, int maxWidthHeight, int percentage)
        {
            string sizeQualityString = BuildFormatSizeAndPercentage(format, maxWidthHeight, percentage);

            return GetUrlCustom(storageInfo, urlType, filePath, fileName, sizeQualityString);

            //if (mode == StorageMode.External)
            //{
            //    return $"{downloadHost}?{sizeQualityString}";
            //}
            //else
            //{
            //    CloudBlobContainer container = GetContainer(storageId, storageAccessKey, mode, filePath);

            //    if (string.IsNullOrEmpty(filePath))
            //    {
            //        filePath = DEFAULT_CONTAINER;
            //    }

            //    // write a blob to the container
            //    CloudBlockBlob blob = container.GetBlockBlobReference(
            //        Path.Combine(filePath, fileName));
            //    var temp = blob.ExistsAsync();
            //    temp.Wait();
            //    if (temp.Result == false)
            //        return string.Empty;

            //    Uri uri = blob.StorageUri.PrimaryUri;
            //    Uri result = new Uri(uri.AbsoluteUri + "?" + sizeQualityString);

            //    return result.AbsoluteUri;
            //}
        }

        private string BuildFormatSizeAndPercentage(string format, int maxWidthHeight, int percentage)
        {
            string hw = string.Empty;
            if (maxWidthHeight >= 1 && maxWidthHeight <= 4096)
            {
                hw = $"{maxWidthHeight}h_{maxWidthHeight}w";
                //basic = 1080h_1080w_60q.webp
            }
            string q = string.Empty;
            if (percentage > 0 && percentage < 100)
            {
                q = $"{percentage}q";
            }

            string result = string.Empty;
            if (!string.IsNullOrEmpty(hw))
            {
                result = hw;
            }
            if (!string.IsNullOrEmpty(q))
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result = result + "_" + q;
                }
                else result = q;
            }
            if (!string.IsNullOrEmpty(format))
            {
                result = result + $".{format}";
            }

            if (!string.IsNullOrEmpty(result))
            {
                return $"basic={result}";
            }
            return string.Empty;
        }

        public string GetUrlByStyle(IStorageInfo storageInfo, MediaElementUrlType urlType,
            string filePath, string fileName, string styleName)
        {//Azure不支持Style
            return GetUrlCustom(storageInfo, urlType, filePath, fileName, styleName);

            //if (mode == StorageMode.External)
            //{
            //    return $"{downloadHost}?{styleName}";
            //}
            //else
            //{
            //    CloudBlobContainer container = GetContainer(storageId, storageAccessKey, mode, filePath);

            //    if (string.IsNullOrEmpty(filePath))
            //    {
            //        filePath = DEFAULT_CONTAINER;
            //    }

            //    // write a blob to the container
            //    CloudBlockBlob blob = container.GetBlockBlobReference(
            //        Path.Combine(filePath, fileName));
            //    var temp = blob.ExistsAsync();
            //    temp.Wait();
            //    if (temp.Result == false)
            //        return string.Empty;

            //    Uri uri = blob.StorageUri.PrimaryUri;
            //    Uri result = new Uri(uri.AbsoluteUri + "?" + styleName);

            //    return result.AbsoluteUri;
            //}
        }

        public string GetUrlCustom(IStorageInfo storageInfo, MediaElementUrlType urlType,
            string filePath, string fileName, string customStyleProcessStr)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                filePath = filePath.Trim('/');
            }
            fileName = fileName.Trim('/');
            if (!string.IsNullOrEmpty(customStyleProcessStr))
            {
                customStyleProcessStr = "?" + customStyleProcessStr;
            }
            if (urlType == MediaElementUrlType.PublishDownloadUrl
                || urlType == MediaElementUrlType.PublishOutputUrl)
            {
                filePath = (!string.IsNullOrEmpty(filePath) ? filePath + "/" : "")
                    + fileName + customStyleProcessStr;

                var downloadHost = storageInfo.StorageHost.Trim('/');

                return $"{downloadHost}/{EXTERNAL_CONTAINER}/{filePath}";
            }
            else
            {
                CloudBlobContainer container = GetContainer(storageInfo.StorageId,
                    storageInfo.StorageAccessKey, StorageMode.Internal, filePath);
                filePath = (!string.IsNullOrEmpty(filePath) ? filePath + "/" : "")
                    + fileName;// + customStyleProcessStr; 
                // write a blob to the container
                CloudBlockBlob blob = container.GetBlockBlobReference(filePath);
                var temp = blob.ExistsAsync();
                temp.Wait();
                if (temp.Result == false)
                    return string.Empty;

                Uri uri = blob.StorageUri.PrimaryUri;
                if (!string.IsNullOrEmpty(customStyleProcessStr))
                {
                    uri = new Uri(blob.StorageUri.PrimaryUri.AbsoluteUri + customStyleProcessStr);
                }

                var sasConstraints = new SharedAccessBlobPolicy();
                sasConstraints.SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5);
                sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(60);
                sasConstraints.Permissions = SharedAccessBlobPermissions.Read;
                SharedAccessBlobHeaders sasHeaders = new SharedAccessBlobHeaders();
                sasHeaders.ContentType = this.GetContentType(fileName,
                    customStyleProcessStr, sasHeaders.ContentType);

                var sasBlobToken = blob.GetSharedAccessSignature(sasConstraints, sasHeaders);

                if (uri.AbsoluteUri.LastIndexOf('?') > 0)
                {
                    return $"{uri.AbsoluteUri}&{sasBlobToken.Trim('?')}";
                }

                return uri.AbsoluteUri + sasBlobToken;
            }
        }

        private string GetContentType(string fileName, string customStyleProcessStr, string defFormat)
        {
            string extension = Path.GetExtension(fileName).Trim('.');
            List<string> formatList = new List<string>(IMAGE_FORMATS);
            if (formatList.Contains(extension.ToLower()))
            {
                return "image/" + extension;
            }

            return defFormat;
        }

        public Task RemoveFileAsync(string storageId, string storageAccessKey,
            StorageMode mode, string host, string filePath, string fileName)
        {
            return Task.Run(() =>
            {
                CloudBlobContainer container = GetContainer(storageId, storageAccessKey, mode, filePath);

                //if (string.IsNullOrEmpty(filePath))
                //{
                //    filePath = DEFAULT_CONTAINER;
                //}

                // write a blob to the container
                CloudBlockBlob blob = container.GetBlockBlobReference(
                    Path.Combine(filePath, fileName));

                var task = blob.DeleteIfExistsAsync();
                task.Wait();
            });
        }

        public Task UploadFileAsync(Stream stream, string storageId, string storageAccessKey,
            StorageMode mode, string downloadHost, string filePath, string fileName)
        {

            return Task.Run(() =>
            {
                if (stream.Length < 1)
                    return;

                CloudBlobContainer container = GetContainer(storageId, storageAccessKey, mode, filePath);

                //if (string.IsNullOrEmpty(filePath))
                //{
                //    filePath = DEFAULT_CONTAINER;
                //}

                // write a blob to the container
                CloudBlockBlob blob = container.GetBlockBlobReference(
                    Path.Combine(filePath, fileName));
                //var temp = blob.ExistsAsync();
                //temp.Wait();
                //if (temp.Result == false)
                //    return;

                var task = blob.OpenWriteAsync();
                task.Wait();

                var writeStream = task.Result;

                byte[] buffer = null;

                if (stream.Length <= BUFFER_SIZE)
                {
                    buffer = new byte[(int)stream.Length];
                    stream.Read(buffer, 0, (int)stream.Length);

                    writeStream.Write(buffer, 0, (int)stream.Length);

                    var temp2 = writeStream.CommitAsync();
                    temp2.Wait();
                }
                else
                {
                    long current = 0;

                    while (current <= stream.Length)
                    {
                        long length = Math.Min((long)BUFFER_SIZE, stream.Length - current);
                        buffer = new byte[(int)length];
                        System.Diagnostics.Trace.WriteLine($"LargeStream position: {stream.Position}");
                        stream.Read(buffer, 0, (int)length);
                        current += BUFFER_SIZE;
                        writeStream.Write(buffer, 0, (int)length);
                    }

                    var temp2 = writeStream.CommitAsync();
                    temp2.Wait();
                }
            });
        }

        private static CloudBlobContainer GetContainer(string storageId, string storageAccessKey,
            StorageMode mode, string filePath)
        {
            string endPoint = ExtractEndpoint(storageAccessKey);
            string accountKey = ExtractAccountKey(storageAccessKey);

            string storageConnectionString = storageAccessKey;
            //"DefaultEndpointsProtocol=https;"
            //            + $"AccountName={storageId}"
            //            + $";AccountKey={accountKey}"
            //            + $";EndpointSuffix={endPoint}";

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            string containerName = INTERNAL_CONTAINER;
            if (mode == StorageMode.External)
                containerName = EXTERNAL_CONTAINER;
            //if (string.IsNullOrEmpty(filePath))
            //{
            //    containerName = DEFAULT_CONTAINER;
            //}
            //else
            //{
            //    string[] splited = filePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            //    containerName = splited[0];
            //}

            // Create container. Name must be lower case.
            //Console.WriteLine("Creating container...");
            var container = serviceClient.GetContainerReference(containerName);
            var exists = container.ExistsAsync();
            exists.Wait();
            if (false == exists.Result)
            {
                container.CreateIfNotExistsAsync();
                if (mode == StorageMode.External)
                {
                    var temp = container.SetPermissionsAsync(new BlobContainerPermissions()
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob,
                    });
                    temp.Wait();
                }
                else
                {
                    var temp = container.SetPermissionsAsync(new BlobContainerPermissions()
                    {
                        PublicAccess = BlobContainerPublicAccessType.Off,
                    });
                    temp.Wait();
                }
            }

            return container;
        }

        private static string ExtractAccountKey(string storageAccessKey)
        {
            var splited = storageAccessKey.Split(new string[] { "AccountKey=", ";" },
                StringSplitOptions.RemoveEmptyEntries);
            return splited[2];
        }

        private static string ExtractEndpoint(string storageAccessKey)
        {
            var splited = storageAccessKey.Split(new string[] { "EndpointSuffix=" },
                StringSplitOptions.RemoveEmptyEntries);
            return splited[1];
        }
    }
}
