using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace XCentium_Code_Challenge_Amit.Models
{
    public class HtmlUtility
    {
        public static string ConvertToDisplayText(string htmlSource)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlSource);
            StringWriter sw = new StringWriter();
            ConvertTo(doc.DocumentNode, sw);
            sw.Flush();

            return sw.ToString();
        }

        public static void ConvertTo(HtmlNode node, TextWriter outputText)
        {
            string html;
            switch (node.NodeType)
            {
                //if Comment - do not process
                case HtmlNodeType.Comment:
                    break;

                //nested loop to get to all child of parent nodes.
                case HtmlNodeType.Document:
                    ConvertContentTo(node, outputText);
                    break;

                case HtmlNodeType.Text:
                    string parentName = node.ParentNode.Name;
                    //do not process scripts and style nodes
                    if((parentName == "script") || (parentName == "style")){
                        break;
                    }

                    //if not script or style, get text
                    html = ((HtmlTextNode)node).Text;

                    //if node is nested closing, do not need to process
                    if (HtmlNode.IsOverlappedClosingElement(html)) break;

                    //check text is not just spaces
                    if(html.Trim().Length > 0)
                    {
                        outputText.Write(HtmlEntity.DeEntitize(html));
                    }
                    break;

                case HtmlNodeType.Element:
                    switch (node.Name)
                    {
                        //paragraph and line-break
                        case "p":
                        case "br":
                            outputText.Write(" ");
                            break;
                    }
                    if (node.HasChildNodes)
                    {
                        ConvertContentTo(node, outputText);
                    }
                    break;
            }
        }

        private static void ConvertContentTo(HtmlNode node, TextWriter outputText)
        {
            foreach(HtmlNode subnode in node.ChildNodes)
            {
                ConvertTo(subnode, outputText);
            }
        }

        public static List<string> GetAllImages(string htmlSource)
        {
            List<string> images = new List<string>();
            foreach (Match m in Regex.Matches(htmlSource, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                string src = m.Groups[1].Value;
                // add src to some array
                images.Add(src);
            }
            return images;
        }

        public static Dictionary<string, int> GetWordsFrequency(string plaintext)
        {
            //remove CRLF put in by HtmlAgility
            plaintext = plaintext.Replace("\r", "").Replace("\n", "");
            //no need of commas and periods
            plaintext = plaintext.Replace(",", "").Replace(".", "");
            string[] arr = plaintext.Split(' ');

            //ignore case for better counting
            Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

            //A word is something with at least 3 letters (avoid counting 'is', 'a' etc...)
            const int minimumLength = 3;

            foreach(string word in arr)
            {
                if (word.Length >= minimumLength)
                {
                    if (dictionary.ContainsKey(word)) dictionary[word] = dictionary[word] + 1; //increase count if word exists
                    else dictionary[word] = 1; //add word and count to dictionary.
                }
            }

            dictionary.OrderByDescending(key => key.Value); //highest word frequency first

            return dictionary;
        }
    }
}