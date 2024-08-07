﻿using Parcel.Types;

namespace Parcel.Data
{
    public static class RandomImage
    {
        public static Image GetRandomImage(uint? width = null, uint? height = null)
        {
            string uri = GetPicsumPhotosUrl(width, height);

            string outputPath = Image.GetTempImagePath();
            new HttpClient().DownloadFileTaskAsync(new Uri(uri), outputPath).GetAwaiter().GetResult();
            return new Image(outputPath);

            static string GetPicsumPhotosUrl(uint? width = null, uint? height = null)
            {
                if (width == null)
                    return "https://picsum.photos/200/300";
                else if (height == null) 
                    return $"https://picsum.photos/{width}";
                else return $"https://picsum.photos/{width}/{height}";
            }
        }
    }

    public static class HTTPClientHelper
    {
        public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string outputPath)
        {
            // TODO: Not working, program is stuck here.
            HttpResponseMessage response = await client.GetAsync(uri);
            using FileStream fileStream = new(outputPath, FileMode.CreateNew);
            await response.Content.CopyToAsync(fileStream);
        }
    }
}
