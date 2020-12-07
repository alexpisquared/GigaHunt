using AsLink;
using Db.QStats.DbModel;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace xPoc
{
  public partial class MainWindow : WindowBase
  {
    readonly A0DbContext _db = new A0DbContext();
    public MainWindow() => InitializeComponent();

    async void Wnd_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        await _db.EMails
          .Where(r => r.ID.Contains('@') && r.Company != null
          && !new[] {' Richmond Hill Toyota',
          '	\''Andre',
          '01CF0D63',
          '01D04485',
          '2marketsearch',
          '2x',
          '407ETR',
          'adriatic',
          'aei',
          'airport',
          'alerts',
          'alt',
          'amd',
          'amdocs',
          'amigainformatics',
          'avivacanada',
          'bell',
          'bellnet',
          'bulletproof',
          'ca',
          'ca/documents/promotionalcontent/hays_1734352',
          'ca/documents/webassets/hays_1734352',
          'canadapost',
          'canadarunningseries',
          'carbonite',
          'chatrwireless',
          'cloud',
          'cmail1',
          'cmail19',
          'cmail2',
          'cmail20',
          'cmail5',
          'commmunity neighbors',
          'cooler',
          'dock',
          'docusign',
          'e',
          'email',
          'e-mail',
          'endecaindex&source=19&FREE_TEXT=Robert+Half&rating=99',
          'eteaminc',
          'example',
          'facebookappmail',
          'garmin',
          'github',
          'goodtimesrunning',
          'imax',
          'intel',
          'invalidemail',
          'iRun',
          'jazz',
          'kijiji',
          'lexisnexis',
          'mail',
          'mail102',
          'mail108',
          'mail115',
          'mail135',
          'mail139',
          'mail147',
          'mail17',
          'mail171',
          'mail181',
          'mail2',
          'mail200',
          'mail201',
          'mail21',
          'mail210',
          'mail221',
          'mail222',
          'mail24',
          'mail252',
          'mail37',
          'mail39',
          'mail43',
          'mail63',
          'mail66',
          'mail68',
          'mail69',
          'mail99',
          'news',
          'nokia',
          'nymi',
          'ramac',
          'reply',
          'richmondhilltoyota',
          'runningroom',
          'shatny',
          'sleepcountry',
          'stackoverflow',
          'torontopolice',
          'twitter',
          'uk/',
          'ukr',
          'umca',
          'Undisclosed recipients:',
          'Undisclosed-Recipient:;',
          '        undisclosed - recipients:',
          'vbuzzer',
          'wietzestoyota',
          }.Contains(r.Company)
          )
          .LoadAsync();

        var src = _db.EMails.Local
          .AsEnumerable()                 // ouch
          .Where(r => ValidEmail(r.ID))
          .OrderBy(r => r.Company);

        ((CollectionViewSource)(FindResource('eMailViewSource'))).Source = src;
        Title = $'{src.Count()}';
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex);
        tbkErr.Text = ex.Message;
      }
    }
    public static bool ValidEmail(string email)
    {
      if (email == '')
        return false;
      else
        return new System.Text.RegularExpressions.Regex(@'^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$').IsMatch(email);
    }
  }
}
