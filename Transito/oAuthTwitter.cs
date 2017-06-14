using System;
using System.Data;
using System.Configuration;
using System.Web;
//using System.Web.Security;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace Transito
{
    public class oAuthTwitter : OAuthBase
    {
        public enum Method { GET, POST };
        public const string REQUEST_TOKEN = "http://twitter.com/oauth/request_token";
        public const string AUTHORIZE = "http://twitter.com/oauth/authorize";
        public const string ACCESS_TOKEN = "http://twitter.com/oauth/access_token";

        //ILSRJ
        //private string _consumerKey = "xHopg1ho7s7NNUaBOBUuA";
        //private string _consumerSecret = "DFpqvuqdepRealZvbZgZVqkLB5jhKheFSFh1o3Ywrs";
        //private string _token = "72913745-FrsvV0gtRoiXu2v3xITEnGhlpMyLxlaOho9reMl1P";
        //private string _tokenSecret = "kPZiMqgCKtlstj6YKsxwEHejVLHtnGAsyyJli1UqTI";
        //private string _pin = "7759541"; // JDevlin

        //RadarBlitz
        private string _consumerKey = "GJmXm38LJpJYKZ3ad3tfw";
        private string _consumerSecret = "FlMNFlytvNzUOMBHGSpyELO6xvNRYpGQbtdASXWoVY";
        private string _token = "85891484-uhHhFSfcYarAqG5cQ1ARUdzo6NwOJ68TEbZpJrAXp";
        private string _tokenSecret = "yT35FXXwqH64mrGTC4vHqCvvHMIDjExES9CWg4em6LI";
        private string _pin = "8569196"; // JDevlin

        // JMD: this property should not have a dependency on the Settings file.
        public string ConsumerKey 
        {
            get
            {
                //if (_consumerKey == null || _consumerKey.Length == 0)
                //{
                //    _consumerKey = Settings1.Default.consumerKey;
                //}
                return _consumerKey; 
            } 
            set { _consumerKey = value; } 
        }
        
        // JMD: this property should not have a dependency on the Settings file.
        public string ConsumerSecret { 
            get {
                //if (_consumerSecret.Length == 0)
                //{
                //    _consumerSecret = Settings1.Default.consumerSecret;
                //}
                return _consumerSecret; 
            } 
            set { _consumerSecret = value; } 
        }

        public string OAuthToken { get; set; }
        public string Token { get { return _token; } set { _token = value; } }
        public string PIN { get { return _pin; } set { _pin = value; } }
        public string TokenSecret { get { return _tokenSecret; } set { _tokenSecret = value; } }

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet()
        {
            string ret = null;

            // First let's get a REQUEST token.
            string response = oAuthWebRequest(Method.GET, REQUEST_TOKEN, String.Empty);
            if (response.Length > 0)
            {
                //response contains token and token secret.  We only need the token.
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    OAuthToken = qs["oauth_token"]; // tuck this away for later
                    ret = AUTHORIZE + "?oauth_token=" + qs["oauth_token"];// +"&oauth_callback=oob";
                }
            }
            return ret;
        }

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token is supplied by Twitter's authorization page following the callback.</param>
        public string AccessTokenGet(string authToken, string PIN)
        {
            this.Token = authToken;
            this._pin = PIN; // JDevlin

            string response = oAuthWebRequest(Method.GET, ACCESS_TOKEN, String.Empty);

            if (response.Length > 0)
            {
                //Store the Token and Token Secret
                NameValueCollection qs = HttpUtility.ParseQueryString(response);
                if (qs["oauth_token"] != null)
                {
                    this.Token = qs["oauth_token"];
                }
                if (qs["oauth_token_secret"] != null)
                {
                    this.TokenSecret = qs["oauth_token_secret"];
                }
            }

            return response;
        }
        
        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="url">The full url, including the querystring.</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        public string oAuthWebRequest(Method method, string url, string postData)
        {
            string outUrl = "";
            string querystring = "";
            string ret = "";


            //Setup postData for signing.
            //Add the postData to the querystring.
            if (method == Method.POST)
            {
                if (postData.Length > 0)
                {
                    //Decode the parameters and re-encode using the oAuth UrlEncode method.
                    NameValueCollection qs = HttpUtility.ParseQueryString(postData);
                    postData = "";
                    foreach (string key in qs.AllKeys)
                    {
                        if (postData.Length > 0)
                        {
                            postData += "&";
                        }
                        qs[key] = HttpUtility.UrlDecode(qs[key]);
                        qs[key] = this.UrlEncode(qs[key]);
                        postData += key + "=" + qs[key];

                    }
                    if (url.IndexOf("?") > 0)
                    {
                        url += "&";
                    }
                    else
                    {
                        url += "?";
                    }
                    url += postData;
                }
            }
            else if (method == Method.GET && !String.IsNullOrEmpty(postData))
            {
                url += "?" + postData;
            }

            Uri uri = new Uri(url);
            
            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();

            //Generate Signature
            string sig = this.GenerateSignature(uri,
                this.ConsumerKey,
                this.ConsumerSecret,
                this.Token,
                this.TokenSecret,
                method.ToString(),
                timeStamp,
                nonce,
                this.PIN,
                out outUrl,
                out querystring);

            querystring += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

            //Convert the querystring to postData
            if (method == Method.POST)
            {
                postData = querystring;
                querystring = "";
            }

            if (querystring.Length > 0)
            {
                outUrl += "?";
            }

            ret = WebRequest(method, outUrl +  querystring, postData);

            return ret;
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(Method method, string url, string postData)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            //webRequest.UserAgent  = "Identify your application please.";
            //webRequest.Timeout = 20000;

            if (method == Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;

        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }

        // JMD: added for convenience. Reset the state of the oAuthTwitter object.
        public void Reset()
        {
            ConsumerKey = ConsumerSecret = OAuthToken = Token = TokenSecret = PIN = String.Empty;
        }
    }
}
