using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreConf2020.Functions.Services
{
    public class SpeechToTextService
    {
        private SpeechConfig speechConfig;
        private SpeechRecognizer speechRecognizer;
        public SpeechToTextService(string key, string region)
        {
            speechConfig = SpeechConfig.FromSubscription(key, region);
            speechConfig.SpeechRecognitionLanguage = "es-ES";
        }

        /// <summary>
        /// Recognizes wav files but only 15 secs at max
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string> RecognizeShortAudio(Stream stream, string fileName)
        {

            DirectoryInfo info = new DirectoryInfo("audios");
            if (!info.Exists)
            {
                info.Create();
            }

            string path = Path.Combine("audios", fileName);
            using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
            {
                stream.CopyTo(outputFileStream);
            }
            var audioConfig = AudioConfig.FromWavFileInput(path);
            var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
            var result = await recognizer.RecognizeOnceAsync();

            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    return result.Text;
                case ResultReason.NoMatch:
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    }
                    break;
            }
            return null;
        }
        public async Task<string> RecognizeContinuousAudio(string filePath)
        {
            try
            {
                string resultText = string.Empty;

                var audioConfig = AudioConfig.FromWavFileInput(filePath);

                var stopRecognition = new TaskCompletionSource<int>();

                var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
                recognizer.Recognizing += (s, e) =>
                {
                    Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                        resultText += e.Result.Text;
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    }
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"CANCELED: Reason={e.Reason}");

                    if (e.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }

                    stopRecognition.TrySetResult(0);
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\n    Session stopped event.");
                    stopRecognition.TrySetResult(0);
                };

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                await recognizer.StartContinuousRecognitionAsync();

                // Waits for completion. Use Task.WaitAny to keep the task rooted.
                Task.WaitAny(new[] { stopRecognition.Task });

                // Stops recognition.
                await recognizer.StopContinuousRecognitionAsync();

                return resultText;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception Thrown: ErrorCode={ex.Message}");
                return null;
            }
        }

    }


}
