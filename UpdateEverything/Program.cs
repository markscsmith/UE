using System;
/*
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management; */
using WUApiLib;

namespace UpdateEverything
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    UpdateSession session = new UpdateSession();
        //    ISearchResult uResult;

        //    Boolean moreUpdates = true;
        //    while (moreUpdates)
        //    {

        //        uResult = CheckForUpdates(session);

        //        UpdateCollection toInstallAutomatically = getAutoUpdates(uResult);

        //        if (toInstallAutomatically.Count > 0)
        //        {
        //            IInstallationResult installationRes = installUpdates(session, toInstallAutomatically);

        //            if (installationRes.RebootRequired)
        //            {
        //                Console.WriteLine("Rebooting to finish installation!");
        //                moreUpdates = false;
        //                System.Diagnostics.Process.Start("ShutDown", "/r");
        //            }

        //        }
        //        else
        //        {
        //            moreUpdates = false;
        //            System.Console.WriteLine("Updates Complete! Press a key to continue.");
        //            System.Console.ReadKey();
        //        }
        //    }
        //}

        //private static ISearchResult CheckForUpdates(UpdateSession session)
        //{
        //    ISearchResult uResult;
        //    Console.WriteLine("Checking for updates!");
        //    session = new UpdateSession();
        //    IUpdateSearcher uSearcher = session.CreateUpdateSearcher();
        //    uResult = uSearcher.Search("IsInstalled = 0 and Type='Software'");
        //    return uResult;
        //}

        private static IInstallationResult installUpdates(UpdateSession session, UpdateCollection toInstallAutomatically)
        {
            Console.WriteLine("Downloading {0} updates", toInstallAutomatically.Count);
            UpdateDownloader downloader = session.CreateUpdateDownloader();

            downloader.Updates = toInstallAutomatically;
            downloader.Download();
            UpdateCollection updatesToInstall = new UpdateCollection();
            foreach (IUpdate update in toInstallAutomatically)
            {

                if (update.IsDownloaded)
                {
                    updatesToInstall.Add(update);
                }

            }
            Console.WriteLine("Installing {0} updates", updatesToInstall.Count);
            IUpdateInstaller installer = session.CreateUpdateInstaller();
            installer.Updates = updatesToInstall;
            IInstallationResult installtionRes = installer.Install();
            Console.WriteLine("Updates complete!");
            return installtionRes;
        }

        static UpdateCollection getAutoUpdates(ISearchResult found)
        {
            Console.WriteLine("Accepting EULAs");
            UpdateCollection toInstallAutomatically = new UpdateCollection();
            foreach (IUpdate update in found.Updates)
            {
                if (update.MsrcSeverity.Equals("Critical"))
                {


                    if (!update.EulaAccepted)
                    {
                        try
                        {
                            update.AcceptEula();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Unable to accept EULA on update " + update.Title);
                        }
                    }

                    if (update.EulaAccepted && !update.InstallationBehavior.CanRequestUserInput)
                    {
                        toInstallAutomatically.Add(update);
                    }

                    // works for service packs, even though CanRequestUserInput is true. however, IE9 blocks progress.
                    // This currently will try to install anything wtih "service pack" in the name. No bueno.
                    if (update.EulaAccepted && update.Title.Contains("Service Pack"))
                    {
                        toInstallAutomatically.Add(update);
                        Console.WriteLine("Service pack found!");
                        Console.WriteLine(update.Title);
                    }
                }

            }
            if (toInstallAutomatically.Count == 1)
            {
                Console.WriteLine(String.Format("{0,5} update out of {0} can be installed automatically.", toInstallAutomatically.Count, found.Updates.Count));
            }
            else
            {
                Console.WriteLine(String.Format("{0,5} updates out of {0} can be installed automatically.", toInstallAutomatically.Count, found.Updates.Count));
            }
            foreach (IUpdate update in toInstallAutomatically)
            {
                Console.WriteLine(update.Title);
            }
            return toInstallAutomatically;
        }
    }
}
