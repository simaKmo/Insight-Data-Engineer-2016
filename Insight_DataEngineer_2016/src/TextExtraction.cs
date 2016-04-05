using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Insight_DataEngineer_2016.src
{
    class TextExtraction
    {
        /// <summary>
        /// \insight_testsuite\tests\test-2-tweets-all-distinct\tweet_input
        /// I have considered three pathes for input and output files as constant
        /// </summary>
        private SlidingWindow _slidingWindows;
        private const string _filePath = @"..\\insight_testsuite\tests\test-2-tweets-all-distinct\\tweet_input\tweets.txt";//@"C:\Users\sima\Documents\Visual Studio 2015\InsightDataScience\InsightDataScience\bin\Debug\tweets.txt";//@"..\\..\\coding-challenge-master\tweet_input\tweets.txt";
        private const string _outPutTwoPath = @"..\\insight_testsuite\tests\test-2-tweets-all-distinct\\tweet_output\output.txt";// @"C:\Users\sima\Documents\Visual Studio 2015\InsightDataScience\InsightDataScience\bin\Debug\output.txt";// @"..\\..\\coding-challenge-master\tweet_output\output.txt";

        /// <summary>
        /// I am using the Newtonsoft.Json library to have easy access to json file attribures.
        /// for having Newtonsoft libarary I have used manage NuGet option of Refrence menue.
        /// </summary>
        public TextExtraction()
        {
            _slidingWindows = new SlidingWindow();

        }
        /// <summary>
        /// in this method I am trying to extarct two main attributes of each json row: text and create_at
        /// then it calculates the degree by the help of SlidingWindow class.
        /// 
        /// the another important option about writing and reading from the files.
        /// actually reagdring the clean and friendly coding rules it would be better to do these two tasks seperately
        /// but I prefere to do both in one function becuase I do not like to use extra memory to save the results. 
        /// on the other hands using events and delegates cuase many intra-communications between classes and functions which 
        /// cause more time and rsources consuming. 
        /// Finally:
        /// I am extracting the text and date 
        /// then I am writing them immidiately on output to have better memory management
        /// </summary>
        public void FindCleanTextAndDegree()
        {
            using (FileStream fs = File.Open(_filePath, FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            using (StreamWriter sw2 = File.AppendText(_outPutTwoPath))
            {
                string s;
                double degree = -1;


                while ((s = sr.ReadLine()) != null)
                {

                    JObject json = JObject.Parse(s);
                    string text = string.Empty;
                    string date = string.Empty;
                    foreach (KeyValuePair<String, JToken> app in json)
                    {

                        string appName = app.Key;
                        if (appName == "created_at")
                            date = (String)app.Value;




                        else if (appName == "entities")
                        {

                            JObject json1 = (JObject)app.Value;
                            List<string> tags = GetHashTaggedWords(json1);
                            _slidingWindows.CheckSlidingWindowUpdate(tags, date);
                            //double degree = _slidingWindows.GetDegree();
                            //sw2.WriteLine(degree.ToString());




                        }

                    }
                    degree = _slidingWindows.GetDegree();
                    sw2.WriteLine(degree.ToString());
                }

            }
        }

        /// <summary>
        /// Get Hash Tags words from text tags by the help of Regular Expression
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private List<string> GetHashTaggedWords(JObject json)
        {
            if (json == null)
                return null;
            List<string> tags = new List<string>();
            JToken hashs = json.GetValue("hashtags");

            foreach (JToken child in hashs.Children<JToken>())

                foreach (JProperty nest in child.Children<JProperty>())
                {
                    if (nest.Name == "text")
                        tags.Add(nest.Value.ToString());
                }

            return tags;

        }



    }
}
