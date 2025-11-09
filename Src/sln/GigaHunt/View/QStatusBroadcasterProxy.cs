//using Microsoft.Office.Interop.Outlook;


using Emailing.NET6;
using Microsoft.Office.Interop.Outlook;

namespace OutlookToDbWpfApp;

internal class QStatusBroadcasterProxy
{
  internal static async Task<bool> SendLetter_UpdateDb(bool v, string senderEmailAddress, string fnm)
  {
    if (!DevOps.IsDbg)
      return await QStatusBroadcaster.SendLetter_UpdateDb(v, senderEmailAddress, fnm);

    //await new SpeechSynth().SpeakFreeAsync("No sending no db updates in Debug mode.");

    return true;
  }
}