using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Tabs.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Tabs.DataModels;
using System.Collections.Generic;

namespace Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void loadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });


            await MakePredictionRequest(file);
        }


        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "dfc5b98ebe3f44c79cec1d603dae6a45");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/054fb04e-8ef3-407f-94b1-95dfb8d138ae/image";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);


                if (response.IsSuccessStatusCode)
                {
                    //var responseString = await response.Content.ReadAsStringAsync();

                    //EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    //double max = responseModel.Predictions.Max(m => m.Probability);

                    //TagLabel.Text = (max >= 0.5) ? "That is the central business district" : "That is not the central business district";
                    //PredictionLabel.Text = Math.Round(max, 2).ToString();

                    //Test
                    var responseString = await response.Content.ReadAsStringAsync();

                    JObject rss = JObject.Parse(responseString);

                    //Querying with LINQ
                    //Get all Prediction Values
                    var Probability = from p in rss["Predictions"] select (int)p["Probability"];
                    var Tag = from p in rss["Predictions"] select (string)p["Tag"];
                    List<string> list = new List<string>();
                    //Truncate values to labels in XAML
                    TagLabel.Text = "";
                    PredictionLabel.Text = "";
                    foreach (var item in Tag)
                    {
                        TagLabel.Text += item + ": \n";
                        list.Add(item);
                    }

                    int index = 0;
                    foreach (var item in Probability)
                    {
                        PredictionLabel.Text += item + "\n";
                        if (item == 1)
                        {
                            //Post Information to easytable
                            easytable model = new easytable()
                            {
                                Tag = list[index]

                            };

                            await AzureManager.AzureManagerInstance.PostInformation(model);
                        }
                        index += 1;
                    }
                    //--------------
                } else
                {
                    TagLabel.Text = "Something went wrong";
                }


                

                //Get rid of file once we have finished using it
                file.Dispose();
            }
        }


    }
}