using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MNISTDataLibrary
{
    public class DataLoader
    {
      
        //This does the main legwork of pulling in files
        //need to pull in data from json files at a specified path
        public Dictionary<int, List<float[]>> loadPictures(string jsonPath = @"")
        {
            Dictionary<int, List<float[]>> picturesFromJSON = new Dictionary<int, List<float[]>>();

            //now let's load it uppppp
            if (jsonPath == "")
            {
                //no path supplied, we shall build our own to the first chunk
                string currentAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                jsonPath = currentAssemblyDirectoryName + "../../../../../build/chunks/full.json";
                
            }

            //now lets load file to full string, then parse that string

            // Read the file as one string.
            System.IO.StreamReader jsonFile =
               new System.IO.StreamReader(jsonPath);

            //read the whole file into a string
            string jsonString = jsonFile.ReadToEnd();

            //json file closed -- no need of it now
            jsonFile.Close();

            //parse from the chunk string into an actuall jobject, which we will filter through to find the digits
            JObject j = JObject.Parse(jsonString);

            //cycle through the digits
            for (var i = 0; i < 10; i++)
            {
                var ix = i.ToString();
                //skip digit if it doesn't exist!
                if (j[ix] == null)
                    continue;

                //grab the collection of pictures for this digit
                JArray digitArray = (JArray)j[ix];

                //for each digit that exists, create a list 
                picturesFromJSON[i] = new List<float[]>();

                //loop through all the pictures
                for (var a = 0; a < digitArray.Count; a++)
                {
                    //grab a picture from the list
                    JArray pictureArray = (JArray)digitArray[a];

                    //prepare to turn into a byte array
                    float[] floatPixels = new float[3 * pictureArray.Count];

                    //convert the single bytes to multiple bytes for display purposes
                    for (var p = 0; p < pictureArray.Count; p++)
                    {
                        //grab the pixel info -- divide by 255 to get [0,1] float
                        float pixel = (byte)pictureArray[p]/255.0f;

                        //add to the pixel byte array (3 at a time RGB all equal)
                        floatPixels[3 * p] = pixel;
                        floatPixels[3 * p + 1] = pixel;
                        floatPixels[3 * p + 2] = pixel;
                    }

                    //add picture for this digit
                    picturesFromJSON[i].Add(floatPixels);

                }
            }

            //all built up! Just need to send back the pixel info
            return picturesFromJSON;
        }

        //if we want the returned images as bytes (for bitmap display)
        //this is our helper -- just calls loadImages that returns a float collection and converts to bytes
        //more expensive -- but just for testing purposes
        public Dictionary<int, List<byte[]>> loadPicturesAsBytes(string jsonPath = @"")
        {
            Dictionary<int, List<float[]>> picturesFromJSON = loadPictures(jsonPath);
            Dictionary<int, List<byte[]>> byteConversions = new Dictionary<int, List<byte[]>>();

            foreach (var kvp in picturesFromJSON)
            {
                byteConversions[kvp.Key] = new List<byte[]>();
                foreach (var floatArray in kvp.Value)
                {
                    byte[] bpixels = new byte[floatArray.Length];
                    for (var i = 0; i < floatArray.Length; i++)
                    {
                        //return to 0, 255
                        bpixels[i] = (byte)Math.Round(floatArray[i] * 255.0f);
                    }
                    //now add the new byte converted array to our list
                    byteConversions[kvp.Key].Add(bpixels);
                }
            }
            return byteConversions;
        }
    }
}
