using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Insight_DataEngineer_2016.src
{
    /*
    I have created a project with the name of InsightDataScience and I have put the coding-challenge-master folder exactly same inside it.
    the output and input address are same. they are located constantly on :
        - InsightDataScience\coding-challenge-master\tweet_input\tweets.txt
        - InsightDataScience\coding-challenge-master\tweet_output\ft1.txt and ft2.txt
    you can run project easily by going to coding-challenge-master\run folder of project and run the InsightDataScience.exe.
    I have run project many times by putting tweets.txt from data_gen folder. 
    the output is available on ft1 and ft2. you can change the files path on TextExtraction Class. 
    you can build the project and by coing to bin/debug folder run the project too.
    this project is on .Net, C# and I provide comments about each methods and algorithms on the code.
    I have add one free library with name of Newtonsoft.json by help of Nuget management for working  on json file.
    my general idea was about using Regular expression for the first task and using dictionary as main data structure for the second part.
    for each two tags that are in the same tweets I have considered an edge which the name is unique on SilidingWindow. 
    for example if in one tweet there are two tags:Spark and Appache then the edge is SparkAppache which should be unique in SlidingWindow class. 
    Sliding window is a class to keep the relation between tags on last 60 seconds. I am updating slidingWindow by reading each tweet from input.

*/
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Please be sure that inputfile is located at this address:\"..\\tweet_output\\tweets.txt\"");
                TextExtraction _textEctraction = new TextExtraction();
                _textEctraction.FindCleanTextAndDegree();
                Console.WriteLine("Congradulation! you can Check output results on this path:\"..\\tweet_output\\output.txt\"");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;

            }

        }
    }
}



