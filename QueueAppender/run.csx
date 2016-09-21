#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

public static void Run(string myQueueItem, TraceWriter log)
{
    
    log.Verbose($"C# Queue trigger function processed: {myQueueItem}");
    
    string accName = ConfigurationManager.AppSettings["queueStorageName"];
    string accKey = ConfigurationManager.AppSettings["queueStorageKey"];

    StorageCredentials creds = new StorageCredentials(accName, accKey);
    CloudStorageAccount strAcc = new CloudStorageAccount(creds, true);
    CloudBlobClient blobClient = strAcc.CreateCloudBlobClient();

    CloudBlobContainer container = blobClient.GetContainerReference("logs");
    
    var appendBlob = container.GetAppendBlobReference("merged.log");
    if (!appendBlob.Exists())
    {
       appendBlob.CreateOrReplace();
    }
    
    using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(myQueueItem + "\n\n")))
    {
       appendBlob.AppendBlock(stream);
    }
}