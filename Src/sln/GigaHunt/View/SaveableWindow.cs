namespace GigaHunt.View;

public abstract class SaveableWindow : WpfUserControlLib.Base.WindowBase//, ISaveable
{
  protected readonly QstatsRlsContext _db = QstatsRlsContext.Create();

  public SaveableWindow() : base()  {  }
  public async void Save() => _ = await saveAsync();
  public abstract void setProperValuesToNewRecords();
  public async Task<string> saveAsync()
  {
    App.Speak("DB Saving..."); await Task.Delay(333);

    setProperValuesToNewRecords();
    var (_, _, report) = await _db.TrySaveReportAsync("LeadManager.cs");
    Title = report;
    App.Speak(report); await Task.Delay(333);
    return Title;
  }
}
