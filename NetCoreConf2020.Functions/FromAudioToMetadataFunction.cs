using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using NetCoreConf2020.Functions.Models;
using NetCoreConf2020.Functions.Services;
using Newtonsoft.Json;

namespace NetCoreConf2020.Functions
{
    public static class FromAudioToMetadataFunction
    {
        [FunctionName("FromAudioToMetadataFunction")]
        public static async Task Run([BlobTrigger("audios/{name}", Connection = "BlobStorageConnectionString")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            var searchServiceKey = Environment.GetEnvironmentVariable("SpeechToTextSubscriptionKey");
            var searchServiceRegion = Environment.GetEnvironmentVariable("SpeechToTextServiceRegion");
            var speechRecognizer = new SpeechToTextService(searchServiceKey, searchServiceRegion);


            DirectoryInfo info = new DirectoryInfo("audios");
            if (!info.Exists)
            {
                info.Create();
            }

            var filePath = Path.Combine("audios", name);
            using (FileStream outputFileStream = new FileStream(filePath, FileMode.Create))
            {
                myBlob.CopyTo(outputFileStream);
            }

            var speechText = await speechRecognizer.RecognizeContinuousAudio(filePath);
            if (!string.IsNullOrWhiteSpace(speechText))
            {
                var fileNameWithNoExtension = Path.GetFileNameWithoutExtension(filePath);
                var metadataFromFilename = name.Split('-');
                var author = metadataFromFilename.Length > 1 ? metadataFromFilename[0] : name;
                var title = metadataFromFilename.Length > 1 ? metadataFromFilename[1] : name;
                var urlFile = Environment.GetEnvironmentVariable("AudioBlobUrlBase") + name;
                var audioInfo = new AudioInfo() { Author=author,Title=title,SpeechText=speechText,FileName=name,UriFile=urlFile};
                
                //we create a json file from new audioInfo created
                var jsonAudioInfo = JsonConvert.SerializeObject(audioInfo);
                var outputFilepath= Path.Combine("audios", fileNameWithNoExtension + ".json");
                File.WriteAllText(outputFilepath, jsonAudioInfo);

                //finally we must upload new metadata  file to aour blob. This will be data source for our  azure search service
                BlobStorageService blobService = new BlobStorageService(Environment.GetEnvironmentVariable("BlobStorageConnectionString"));
                await blobService.UploadFile(Environment.GetEnvironmentVariable("UploadBlob"), outputFilepath, "application/json");
            }

        }
    }
}
