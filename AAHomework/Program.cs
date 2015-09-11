using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.DesignerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace AAHomework
{
    class Program
    {
        static void Main(string[] args)
        {
            //Getting the values into a deserialized object
            var client = new RestClient("http://internal-comfybuffalo-1-dev.apphb.com/");
            var guid = Guid.NewGuid();
            var response = client.Execute(new RestRequest("/values/" + guid, Method.GET));
            var deserializedResponse = JsonConvert.DeserializeObject<AcklenResponse>(response.Content);
            
            //Sorting the words in alphabetical order
            deserializedResponse.Words.Sort();

            //Starting altering the words
            var nwList = new List<string>();
            foreach (var word in deserializedResponse.Words)
            {
                int cont = 0;
                var initialFibIndex = Math.Round(deserializedResponse.StartingFibonacciNumber);
                var builder = new StringBuilder(word);
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i] != 'a' && word[i] != 'e' && word[i] != 'i' && word[i] != 'o' && word[i] != 'u')
                    {
                        //Alternating between upper and lower case if the character is a consonant
                        if (cont%2 == 0)
                        {
                            builder[i] = Char.ToUpper(word[i]);
                        }
                        cont++;
                    }
                    else
                    {
                        //getting the fibonacci index from the response round so it cant give the N-th fibonacci value and can be assigned to the char 
                        var fibonacciValue = Fibonacci((int)initialFibIndex);
                       // builder[i] = Convert.ToChar(fibonacciValue);
                        initialFibIndex++;
                    }
                }
                //adding the fully modified word to a new list
                nwList.Add(builder.ToString());
            }
            //swapping the lists
            deserializedResponse.Words = nwList;

            //Reversing list order
            deserializedResponse.Words.Reverse();

            //concatenating strings with previous word's first letter's ascii value as delimiter
            var lastWord = deserializedResponse.Words.Last();
            string nwString = (int)lastWord[0] + deserializedResponse.Words.First();
            for (int i = 1; i < deserializedResponse.Words.Count; i++)
            {
                var currentWord = deserializedResponse.Words[i];
                var previousWord = deserializedResponse.Words[i - 1];
                nwString += (int) previousWord[0] + currentWord;
            }

            var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(nwString));

            var postRestRequest = new RestRequest("/values/" + guid, Method.POST);
            postRestRequest.AddBody(new
                                    {
                                        EncodedValue = base64String,
                                	    EmailAddress = "viktor@acklenavenue.com",
                                	    WebhookUrl=  "https://github.com/ZSnake"
                                    });
            var postResponse = client.Execute(postRestRequest);
            Console.Read();
        }

        public static int Fibonacci(int n)
        {
            int i = 0, j = 1, k, t;
            for (k = 1; k <= n; ++k)
            {
                t = i + j;
                i = j;
                j = t;
            }
            return j;
        }

        public static double Phi()
        {
            return (1 + Math.Sqrt(5)) / 2;
        }

        public static double InverseFibonacci(double F)
        {
            return (Math.Log(F, Phi())*Math.Sqrt(5)) + 0.5;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
