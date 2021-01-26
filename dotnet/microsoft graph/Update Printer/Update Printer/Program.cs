using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Update_Printer
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            string clientId = AskForInfo("Enter Client Id");
            string clientSecret = AskForInfo("Enter Client Secret");
            string returnUrl = AskForInfo("Enter Return Url");
            string printerId = AskForInfo("Enter Printer Id");

            Console.WriteLine();

            var app = ConfidentialClientApplicationBuilder
                        .Create(clientId)
                        .WithClientSecret(clientSecret)
                        .WithAuthority(AzureCloudInstance.AzurePublic, "organizations")
                        .WithRedirectUri(returnUrl).Build();

            string[] scopesToTest = {
                                        "https://graph.microsoft.com/.default",
                                        "Printer.ReadWrite.All",
                                        "https://graph.microsoft.com/Printer.ReadWrite.All",
                                        "Printer.FullControl.All",
                                        "https://graph.microsoft.com/Printer.FullControl.All",
                                    };

            foreach (var scope in scopesToTest)
            {
                TestScopeToUpdatePrinter(app, printerId, scope).Wait();
            }

            Console.WriteLine("Hit enter to exit.");
            Console.ReadLine();
        }

        async static Task TestScopeToUpdatePrinter(IConfidentialClientApplication app, string printerId, string scope)
        {
            string msg = $"Testing [{scope}]...";

            Console.WriteLine(msg);
            Console.WriteLine(new string('=', msg.Length));
            Console.WriteLine("Test Scopes returned by API:");
            Console.WriteLine("----------------------------");

            try
            {
                var auth = await app.AcquireTokenForClient(new[] { scope }).ExecuteAsync();

                foreach (var item in auth.Scopes)
                {
                    Console.WriteLine(item);
                }
            }
            catch (Exception ex)
            {
                OutError(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Update Printer:");
            Console.WriteLine("----------------------------");

            try
            {
                await new GraphServiceClient(new ClientCredentialProvider(app, scope))
                // for readability
                .Print.Printers[printerId].Request().UpdateAsync(new Printer()
                {
                    ODataType = null,
                    Capabilities = new PrinterCapabilities()
                    {
                        ODataType = null,
                        Orientations = new[] { PrintOrientation.Portrait, PrintOrientation.Landscape },
                        SupportedOrientations = new[] { PrintOrientation.Portrait, PrintOrientation.Landscape }
                    },
                    Defaults = new PrinterDefaults()
                    {
                        ODataType = null,
                        Orientation = PrintOrientation.Portrait
                    }
                });

                OutSuccess("Update Successful");
            }
            catch (Exception ex)
            {
                OutError(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        static string AskForInfo(string title)
        {
            Console.Write($"{title}: ");
            return Console.ReadLine();
        }


        static void OutColored(string msg, ConsoleColor color)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = temp;
        }

        static void OutError(string msg)
        {
            OutColored(msg, ConsoleColor.Red);
        }

        static void OutSuccess(string msg)
        {
            OutColored(msg, ConsoleColor.Green);
        }
    }
}
