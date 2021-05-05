using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using Colorful;
using HtmlAgilityPack;

namespace EduPage
{
    class EduMain
    {

        //Variables we need to perform the login
        public static String schoolURL;
        public static String username;
        public static String password;
        public static String csrfToken;
        public static String basicToken;
        public static Boolean isLoggedIn;

        /*
         * Project: EduPage Auth
         * Date: 05.05.2021
         * License: GPL-3.0 License
         * @author vizee
         */
        static void Main(string[] args)
        {
            //Console Setup
            System.Console.Title = "EduPage - vizee";
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine(" ");
            System.Console.WriteLine(" ");
            System.Console.WriteLine(" ");
        
            Colorful.Console.WriteAscii("EduPage Auth", Color.FromArgb(0, 255, 255));

            //School edupage sub-domain grabber
            System.Console.WriteLine(" ");
            System.Console.Write(new string(' ', (System.Console.WindowWidth - 59) / 2));
            System.Console.Write("School: ");
            schoolURL = @"https://" + System.Console.ReadLine() + ".edupage.org/login/edubarLogin.php";

            //Simple csrf Token grabber
            var web = new HtmlWeb();
            var doc = web.Load(schoolURL);
            //Searches for the token column
            var inputEmails = doc.DocumentNode.SelectNodes("//input[@type='hidden'][@name='csrfauth']");
            var inputNext = doc.DocumentNode.SelectNodes("//input[@type='hidden'][@name='csrfauth']");

            foreach (HtmlNode input in inputEmails)
            {       
                basicToken = input.OuterHtml;
            }

            //Parsing html tags
            var realToken = basicToken.Split(new[] { '=' });
            var value = realToken[3];
            //Replacing unwanted symbols
            csrfToken = value.Replace(">", "").Replace("\"", "");
          
            //Username grabber
            System.Console.WriteLine(" ");
            System.Console.Write(new string(' ', (System.Console.WindowWidth - 59) / 2));
            System.Console.Write("Username: ");
            username = System.Console.ReadLine();

            //Password grabber
            System.Console.WriteLine(" ");
            System.Console.Write(new string(' ', (System.Console.WindowWidth - 59) / 2));
            System.Console.Write("Password: ");
            password = System.Console.ReadLine();

            //Performs the login
            login();

          
        }


        //Auth system
        static void login()
        {
            //Checks if arguments are not null
            if (username == null || password == null || schoolURL == null)
            {
                return;
            }
            string appURL = schoolURL;
            string strPostData = String.Format("username={0}&password={1}&csrfauth={2}",
            username, password, csrfToken);

            //Setup for HTTP request
            HttpWebRequest webreq1 = WebRequest.Create(appURL) as HttpWebRequest;
            webreq1.Method = "post";
            webreq1.ContentLength = strPostData.Length;
            webreq1.ContentType = "application/x-www-form-urlencoded";
            webreq1.AllowAutoRedirect = true;

            //Sets previous cookie session
            CookieContainer cookieContainer = new CookieContainer();
            webreq1.CookieContainer = cookieContainer;

            //Posts credentials into url
            StreamWriter web1_p = new StreamWriter(webreq1.GetRequestStream());
            web1_p.Write(strPostData);
            web1_p.Close();

            //Web Response grabber
            HttpWebResponse Hr_resp = (HttpWebResponse)webreq1.GetResponse();

            //Gets the URL to check response code
            String url = Hr_resp.ResponseUri.AbsoluteUri;

            //Bad Login
            if (url.Contains("bad=1"))
            {
                isLoggedIn = false;
                System.Console.Clear();
                System.Console.WriteLine("Wrong username/password!");

            //Successfull login
            } else if (url.Contains("user"))
            {
                isLoggedIn = true;
                System.Console.Clear();
                System.Console.WriteLine("Successfully logged in!");
            }
        }
    }
}
