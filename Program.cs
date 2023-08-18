using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.Rest;
using System.Linq;
using System;
using Microsoft.PowerBI.Api.Models;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.IO;


namespace DemoPowerBI
{
    class RealtimeLog
    {
        public RealtimeLog() { }

        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Client { get; set; }
        public double Duration { get; set; }
        public double Amount { get; set; }
    }

    class Program
    {
        static PowerBIClient _powerBIClient = null;

        #region Authentication settings
        // Authentication settings
        private const string clientId = "e0883e94-46cb-4492-bde0-bb982cfeb5c9";   // Created in AD (Realtime Demo)
        private const string clientSecret = "20x7j9yLKo/cOX5FgF9fUk4FZZjBmk6+X5weqeoik9Q="; // Application client secret (generated in AAD for RealtimeDemo app)
        private const string tenantId = "e4ac844a-b483-48f2-a621-4e50119811d7";   // AAD Tenant ID (contoso-bi)
        private static readonly string authority = $"https://login.microsoftonline.com/{tenantId}";
        #endregion

        // Power BI API settings
        private const string resource = "https://analysis.windows.net/powerbi/api";
        private const string ApiUrl = "https://api.powerbi.com";

        // Dataset and Group settings
        // private const string RealtimeDatasetName = "Realtime Dataset";  // You can change this name pointing to the Dataset you want to create/update

        // Retrieve this value from the Workspace group ID
        // This can be easily retrieved from the URL when you navigate on powerbi.com
        // and it is the string just after "groups" in the URL, for example:
        // https://app.powerbi.com/groups/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
        // static readonly Guid GroupRealtimeDemoId = new Guid("23cdd283-ecf1-4502-8929-43a56207a35a"); // This is the "Realtime Demos" workspace name in Power BI
                                                                                                     

        // static string _powerbi_datasetId_RT = null;

        /// <summary>
        /// This initialization works with the service provider
        /// </summary>
        static string InitPowerBI()
        {
            #region CreateAccessToken
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(clientId)
               .WithClientSecret(clientSecret)
               .WithAuthority(new Uri(authority))
               .Build();

            // Use .default to get all the permissions available (those configured in AAD for this app)
            string[] scopes = new string[] { $"{resource}/.default" };

            AuthenticationResult result = null;
            try
            {
                result = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;
            }
            catch (MsalUiRequiredException ex)
            {
                // The application doesn't have sufficient permissions.
                // - Did you declare enough app permissions during app creation?
                // - Did the tenant admin grant permissions to the application?
                Console.WriteLine(ex.Message);
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be in the form "https://resourceurl/.default"
                // Mitigation: Change the scope to be as expected.
                Console.WriteLine(ex.Message);
            }

            #endregion

            Console.WriteLine($"App-Only Access Token:\n{result.AccessToken}\n");
            var tokenCredentials = new TokenCredentials(result.AccessToken, "Bearer");
            _powerBIClient = new PowerBIClient(new Uri(ApiUrl), tokenCredentials);

            return result.AccessToken;
        }

      

        static void WriteTextToFile(string filePath, string result)
        {
            try
            {
                // Open the file for writing
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the text to the file
                    writer.Write(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }



        static void Main(string[] args)
        {
            // Initialize API and authenticate
            string result = InitPowerBI();

            // Provide the path to the file where you want to write the text
            string filePath = "/home/ec2-user/Documents/PowerBITokenAuthC/file.txt";

           WriteTextToFile(filePath, result);

            Console.WriteLine("Text has been written to the file.");

            // Push data
            // LogPowerBI_RT(_powerBIClient, RealtimeDatasetName); COMENTEI

            Console.WriteLine("Done!");
        }
    }
}
