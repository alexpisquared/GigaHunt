using DAL;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace OppEntry
{
  public partial class Form1 : Form
  {
    readonly DBHelper dbh = DBHelperFactory.Instance(Properties.Settings.Default.QStatsConnectionString);

    public Form1() => InitializeComponent(); 

    private void Form1_Load(object s, EventArgs e)
    {
      // TODO: This line of code loads data into the 'qStatsDataSet1.OppContactView' table. You can move, or remove it, as needed.
      oppContactViewTableAdapter1.Fill(qStatsDataSet1.OppContactView);
      // TODO: This line of code loads data into the 'qStatsOppDataSet.OppContactFor' table. You can move, or remove it, as needed.
      oppContactForTableAdapter.Fill(qStatsOppDataSet.OppContactFor);
      // TODO: This line of code loads data into the 'qStatsOppDataSet.OppCompany' table. You can move, or remove it, as needed.
      oppCompanyTableAdapter1.Fill(qStatsOppDataSet.OppCompany);
      // TODO: This line of code loads data into the 'qStatsOppDataSet.OppLocation' table. You can move, or remove it, as needed.
      oppLocationTableAdapter.Fill(qStatsOppDataSet.OppLocation);

      dataGridView1.SelectAll();
      Icon = Properties.Resources.Icon1;
    }

    private void btnInsert_Click(object s, EventArgs e)
    {
      insertUpdate();

      cbxCompany.Text = cbxLocation.Text = tbxDescription.Text = tbxStart.Text = tbxTerm.Text = "";
      nudRateAsked.Value = 0;
      cbxContact.SelectedIndex = -1;

      Form1_Load(s, e);
    }

    void insertUpdate()
    {
      var count = (int)dbh.ExecuteScalarText(@"SELECT COUNT(*) FROM Opportunity WHERE Company = @Company AND Location = @Location",
        new SqlParameter("@Company", cbxCompany.Text),
        new SqlParameter("@Location", cbxLocation.Text));

      if (count == 1)
      {
        var dr = MessageBox.Show("Insert New? \r\n\n(No - Updates the description)", "Company & Location already exists", MessageBoxButtons.YesNoCancel);
        if (dr == DialogResult.No)
        {
          dbh.ExecuteNonQueryText(@"
						UPDATE Opportunity SET Description = @Description, Start = @Start, Term = @Term, LastActivityAt=@LastActivityAt  WHERE Company = @Company AND Location = @Location",
              new SqlParameter("@Description", tbxDescription.Text),
              new SqlParameter("@Start", tbxStart.Text),
              new SqlParameter("@Term", tbxTerm.Text),
              new SqlParameter("@LastActivityAt", DateTime.Now),
              new SqlParameter("@Company", cbxCompany.Text),
              new SqlParameter("@Location", cbxLocation.Text));
          return;
        }
        else if (dr == DialogResult.Cancel)
        {
          return;
        }
      }
      else if (count > 1)
      {
        var dr = MessageBox.Show("Insert New? \r\n\n(No - Updates the description)", "Multiple Company & Location already exists", MessageBoxButtons.YesNoCancel);
        if (dr != DialogResult.Yes)
          return;
      }

      dbh.ExecuteNonQueryText(@"
				INSERT INTO Opportunity (Company, Location, RateAsked, ContactId, Description, Start, Term)
				VALUES                 (@Company,@Location,@RateAsked,@ContactId,@Description,@Start,@Term)",
          new SqlParameter("@Company", cbxCompany.Text),
          new SqlParameter("@Location", cbxLocation.Text),
          new SqlParameter("@RateAsked", nudRateAsked.Value),
          new SqlParameter("@ContactId", cbxContact.SelectedValue),
          new SqlParameter("@Description", tbxDescription.Text),
          new SqlParameter("@Start", tbxStart.Text),
          new SqlParameter("@Term", tbxTerm.Text));

    }

    private void btnUpdate_Click(object s, EventArgs e)
    {
      var err = "";
      if (tbxDescription.Text.Trim().Length == 0) { err += "Notes\r\n"; }
      if (tbxStart.Text.Trim().Length == 0) { err += "Start\r\n"; }
      if (tbxTerm.Text.Trim().Length == 0) { err += "Term\r\n"; }
      if (cbxCompany.Text.Trim().Length == 0) { err += "Company\r\n"; }
      if (cbxLocation.Text.Trim().Length == 0) { err += "Location\r\n"; }
      if (dataGridView1.SelectedRows[0].Cells[9].Value.ToString().Trim().Length == 0) { err += "ID\r\n"; }

      if (err.Length > 0)
      {
        MessageBox.Show(err, "Supply missing fields then repeat:");
        return;
      }

      dbh.ExecuteNonQueryText(@"
				UPDATE Opportunity SET Description=@Description, Start=@Start, Term=@Term, Company=@Company, Location=@Location, RateAsked=@RateAsked, ContactId=@ContactId WHERE ID=@ID",
          new SqlParameter("@Description", tbxDescription.Text),
          new SqlParameter("@Start", tbxStart.Text),
          new SqlParameter("@Term", tbxTerm.Text),
          new SqlParameter("@Company", cbxCompany.Text),
          new SqlParameter("@Location", cbxLocation.Text),
          new SqlParameter("@RateAsked", nudRateAsked.Value),
          new SqlParameter("@ContactId", cbxContact.SelectedValue),
          new SqlParameter("@ID", dataGridView1.SelectedRows[0].Cells[9].Value));

      Form1_Load(s, e);
    }
    private void btnUpdateLastActDate_Click(object s, EventArgs e)
    {
      if (dataGridView1.SelectedRows[0].Cells[9].Value.ToString().Trim().Length == 0)
      {
        MessageBox.Show("No row selected.");
        return;
      }

      dbh.ExecuteNonQueryText(@"
				UPDATE Opportunity SET LastActivityAt=@LastActivityAt WHERE ID=@ID",
          new SqlParameter("@LastActivityAt", DateTime.Now),
          new SqlParameter("@ID", dataGridView1.SelectedRows[0].Cells[9].Value));

      Form1_Load(s, e);
    }

    private void dataGridView1_SelectionChanged(object s, EventArgs e)
    {
      try
      {
        cbxCompany.Text = (string)dataGridView1.SelectedRows[0].Cells[2].Value;
        cbxLocation.Text = (string)dataGridView1.SelectedRows[0].Cells[3].Value;
        tbxDescription.Text = (string)dataGridView1.SelectedRows[0].Cells[8].Value;
        nudRateAsked.Value = (int)dataGridView1.SelectedRows[0].Cells[4].Value;
        cbxContact.Text = (string)dataGridView1.SelectedRows[0].Cells[7].Value;
        tbxStart.Text = (string)dataGridView1.SelectedRows[0].Cells[5].Value;
        tbxTerm.Text = (string)dataGridView1.SelectedRows[0].Cells[6].Value;
      }
      catch
      {
        return;
      }

      btnUpdate.Enabled = true;
    }

    private void cbxCompany_Validating(object s, CancelEventArgs e) //an example of overweight validating pattern 
    {
      if (cbxCompany.Text.Trim().Length == 0)
      {
        e.Cancel = true;// Cancel the event and select the text to be corrected by the user.
        cbxCompany.Select(0, cbxCompany.Text.Length);

        // Set the ErrorProvider error with the text to display. 
        errorProvider1.SetError(cbxCompany, "Enter valid company name");
      }
    }
    private void cbxCompany_Validated(object s, EventArgs e) =>
      // If all conditions have been met, clear the ErrorProvider of errors.
      errorProvider1.SetError(cbxCompany, "");

  }
}