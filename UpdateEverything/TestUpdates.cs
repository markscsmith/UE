using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WUApiLib;
using System.Web.Script.Serialization;


namespace UpdateEverything
{
    class TestUpdates
    {
        static void Main(string[] args)
        {
            UpdateSession session = new UpdateSession();
            ISearchResult uResult;

            Boolean moreUpdates = true;
            while (moreUpdates)
            {

                uResult = CheckForUpdates(session);

                getAutoUpdates(uResult);
            }
        }

        private static ISearchResult CheckForUpdates(UpdateSession session)
        {
            ISearchResult uResult;
            Console.WriteLine("Checking for updates!");
            session = new UpdateSession();
            IUpdateSearcher uSearcher = session.CreateUpdateSearcher();
            uResult = uSearcher.Search("IsInstalled = 0 and Type='Software'");
            return uResult;
        }

//        class Foo {
//    public int A {get;set;}
//    public string B {get;set;}
//}
//...
//Foo foo = new Foo {A = 1, B = "abc"};
//foreach(var prop in foo.GetType().GetProperties()) {
//    Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
//}


        static void getAutoUpdates(ISearchResult found)
        {
            Console.WriteLine("Accepting EULAs");
            UpdateCollection toInstallAutomatically = new UpdateCollection();
            foreach (IUpdate update in found.Updates)
            {
                Console.WriteLine(update.Title);
                /*
                 * Critical, Important, Moderate and Low. This field may also be blank.
                 * Blank does not mean "optional update", it just means "not a security-critical update. Probably."
                 */
               Console.WriteLine("MsrcSeverity:         " + update.MsrcSeverity);
               //  IUpdate::AutoSelectOnWebSites property (Windows)
               //  shows whether or not an update is selected by default?
                Console.WriteLine("AutoSelectOnWebSites: " + update.AutoSelectOnWebSites);
                Console.WriteLine("DownloadPriority:     " + update.DownloadPriority);
                Console.WriteLine("IsMandatory:          " + update.IsMandatory);
                Console.WriteLine("Eula Accepted:        " + update.EulaAccepted);

                Console.WriteLine();

            }
        }
    }
}
