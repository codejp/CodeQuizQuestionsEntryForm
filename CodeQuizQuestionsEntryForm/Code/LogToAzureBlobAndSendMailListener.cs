using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json.Linq;

namespace CodeQuizQuestionsEntryForm.Code
{
    public class LogToAzureBlobAndSendMailListener : TraceListener
    {
        public override void WriteLine(string message)
        {
            this.Write(message + "\r\n");
        }

        public override void Write(string message)
        {
            var blobUri = default(Uri);
            try { blobUri = LogToAzureBlob(message); }
            catch { }

            try { SendNotifyMail(blobUri); }
            catch { }
        }

        private Uri LogToAzureBlob(string message)
        {
            var configStr = ConfigurationManager.AppSettings["LogToAzureBlobAndSendMail.Blob." + this.Name];
            if (string.IsNullOrWhiteSpace(configStr)) return null;

            dynamic config = JObject.Parse(configStr);
            var credencial = new StorageCredentials((string)config.accountName, (string)config.keyValue);
            var storageAcount = new CloudStorageAccount(credencial, useHttps: true);

            var blobClient = storageAcount.CreateCloudBlobClient();

            var blobContainer = blobClient.GetContainerReference((string)config.container);
            blobContainer.CreateIfNotExists();
            var blobRef = blobContainer.GetBlockBlobReference(Guid.NewGuid().ToString("N"));
            using (var sw = new StreamWriter(blobRef.OpenWrite()))
            {
                sw.Write(message);
            }
            blobRef.Properties.ContentType = "text/plain";
            blobRef.SetProperties();

            return blobRef.Uri;
        }

        private void SendNotifyMail(Uri blobUri)
        {
            var configStr = ConfigurationManager.AppSettings["LogToAzureBlobAndSendMail.Mail." + this.Name];
            if (string.IsNullOrWhiteSpace(configStr)) return;

            dynamic config = JObject.Parse(configStr);

            var smtpClient = new SmtpClient
            {
                Host = (string)config.host,
                Port = (int)config.port,
                Credentials = new NetworkCredential((string)config.user, (string)config.password)
            };
            var mailMsg = new MailMessage(
                (string)config.from,
                (string)config.to,
                (string)config.subject,
                body: blobUri != null ? blobUri.ToString() : ""
                );
            smtpClient.Send(mailMsg);
        }
    }
}