namespace OppEntry
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			this.label1 = new System.Windows.Forms.Label();
			this.btnInsert = new System.Windows.Forms.Button();
			this.tbxDescription = new System.Windows.Forms.TextBox();
			this.cbxCompany = new System.Windows.Forms.ComboBox();
			this.oppCompanyBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
			this.qStatsOppDataSet = new OppEntry.QStatsOppDataSet();
			this.oppCompanyBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cbxLocation = new System.Windows.Forms.ComboBox();
			this.oppLocationBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.oppLocationTableAdapter = new OppEntry.QStatsOppDataSetTableAdapters.OppLocationTableAdapter();
			this.oppCompanyTableAdapter1 = new OppEntry.QStatsOppDataSetTableAdapters.OppCompanyTableAdapter();
			this.label4 = new System.Windows.Forms.Label();
			this.cbxContact = new System.Windows.Forms.ComboBox();
			this.oppContactForBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.oppContactForTableAdapter = new OppEntry.QStatsOppDataSetTableAdapters.OppContactForTableAdapter();
			this.nudRateAsked = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.tbxStart = new System.Windows.Forms.TextBox();
			this.tbxTerm = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.addedAtDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lastActivityAtDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.companyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.locationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.rateAskedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Start = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Term = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.agentCompanyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.notesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.OppId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.oppContactViewBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
			this.qStatsDataSet1 = new OppEntry.QStatsDataSet1();
			this.oppContactViewTableAdapter1 = new OppEntry.QStatsDataSet1TableAdapters.OppContactViewTableAdapter();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.btnUpdateLastActDate = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.oppCompanyBindingSource1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.qStatsOppDataSet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.oppCompanyBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.oppLocationBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.oppContactForBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudRateAsked)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.oppContactViewBindingSource1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.qStatsDataSet1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 113);
			this.label1.TabIndex = 0;
			this.label1.Text = "Notes (technologies, web app, WS, C#/VB)";
			// 
			// btnInsert
			// 
			this.btnInsert.Location = new System.Drawing.Point(857, 2);
			this.btnInsert.Name = "btnInsert";
			this.btnInsert.Size = new System.Drawing.Size(76, 25);
			this.btnInsert.TabIndex = 3;
			this.btnInsert.Text = "&Insert";
			this.btnInsert.UseVisualStyleBackColor = true;
			this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
			// 
			// tbxDescription
			// 
			this.tbxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.tbxDescription.Location = new System.Drawing.Point(76, 64);
			this.tbxDescription.MaxLength = 4000;
			this.tbxDescription.Multiline = true;
			this.tbxDescription.Name = "tbxDescription";
			this.tbxDescription.Size = new System.Drawing.Size(1688, 140);
			this.tbxDescription.TabIndex = 4;
			// 
			// cbxCompany
			// 
			this.cbxCompany.DataSource = this.oppCompanyBindingSource1;
			this.cbxCompany.DisplayMember = "Company";
			this.cbxCompany.FormattingEnabled = true;
			this.cbxCompany.Location = new System.Drawing.Point(76, 3);
			this.cbxCompany.Name = "cbxCompany";
			this.cbxCompany.Size = new System.Drawing.Size(286, 24);
			this.cbxCompany.TabIndex = 0;
			this.cbxCompany.ValueMember = "Company";
			this.cbxCompany.Validating += new System.ComponentModel.CancelEventHandler(this.cbxCompany_Validating);
			this.cbxCompany.Validated += new System.EventHandler(this.cbxCompany_Validated);
			// 
			// oppCompanyBindingSource1
			// 
			this.oppCompanyBindingSource1.DataMember = "OppCompany";
			this.oppCompanyBindingSource1.DataSource = this.qStatsOppDataSet;
			// 
			// qStatsOppDataSet
			// 
			this.qStatsOppDataSet.DataSetName = "QStatsOppDataSet";
			this.qStatsOppDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Company";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(424, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(62, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "Location";
			// 
			// cbxLocation
			// 
			this.cbxLocation.DataSource = this.oppLocationBindingSource;
			this.cbxLocation.DisplayMember = "Location";
			this.cbxLocation.FormattingEnabled = true;
			this.cbxLocation.Location = new System.Drawing.Point(492, 3);
			this.cbxLocation.Name = "cbxLocation";
			this.cbxLocation.Size = new System.Drawing.Size(277, 24);
			this.cbxLocation.TabIndex = 1;
			this.cbxLocation.ValueMember = "Location";
			// 
			// oppLocationBindingSource
			// 
			this.oppLocationBindingSource.DataMember = "OppLocation";
			this.oppLocationBindingSource.DataSource = this.qStatsOppDataSet;
			// 
			// oppLocationTableAdapter
			// 
			this.oppLocationTableAdapter.ClearBeforeFill = true;
			// 
			// oppCompanyTableAdapter1
			// 
			this.oppCompanyTableAdapter1.ClearBeforeFill = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(430, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 17);
			this.label4.TabIndex = 8;
			this.label4.Text = "Contact";
			// 
			// cbxContact
			// 
			this.cbxContact.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.cbxContact.DataSource = this.oppContactForBindingSource;
			this.cbxContact.DisplayMember = "Expr1";
			this.cbxContact.FormattingEnabled = true;
			this.cbxContact.Location = new System.Drawing.Point(492, 6);
			this.cbxContact.MaxDropDownItems = 20;
			this.cbxContact.Name = "cbxContact";
			this.cbxContact.Size = new System.Drawing.Size(277, 24);
			this.cbxContact.TabIndex = 1;
			this.cbxContact.ValueMember = "ID";
			// 
			// oppContactForBindingSource
			// 
			this.oppContactForBindingSource.DataMember = "OppContactFor";
			this.oppContactForBindingSource.DataSource = this.qStatsOppDataSet;
			// 
			// oppContactForTableAdapter
			// 
			this.oppContactForTableAdapter.ClearBeforeFill = true;
			// 
			// nudRateAsked
			// 
			this.nudRateAsked.Location = new System.Drawing.Point(76, 6);
			this.nudRateAsked.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudRateAsked.Name = "nudRateAsked";
			this.nudRateAsked.Size = new System.Drawing.Size(80, 22);
			this.nudRateAsked.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(32, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 17);
			this.label5.TabIndex = 10;
			this.label5.Text = "Rate";
			// 
			// btnUpdate
			// 
			this.btnUpdate.Enabled = false;
			this.btnUpdate.Location = new System.Drawing.Point(939, 2);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(76, 25);
			this.btnUpdate.TabIndex = 2;
			this.btnUpdate.Text = "&Update";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(32, 39);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(38, 17);
			this.label6.TabIndex = 12;
			this.label6.Text = "Start";
			// 
			// tbxStart
			// 
			this.tbxStart.Location = new System.Drawing.Point(76, 36);
			this.tbxStart.Name = "tbxStart";
			this.tbxStart.Size = new System.Drawing.Size(286, 22);
			this.tbxStart.TabIndex = 2;
			// 
			// tbxTerm
			// 
			this.tbxTerm.Location = new System.Drawing.Point(492, 36);
			this.tbxTerm.Name = "tbxTerm";
			this.tbxTerm.Size = new System.Drawing.Size(277, 22);
			this.tbxTerm.TabIndex = 3;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(445, 39);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(41, 17);
			this.label7.TabIndex = 14;
			this.label7.Text = "Term";
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.btnUpdateLastActDate);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.cbxCompany);
			this.panel1.Controls.Add(this.cbxLocation);
			this.panel1.Controls.Add(this.btnInsert);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.btnUpdate);
			this.panel1.Location = new System.Drawing.Point(12, 7);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1771, 34);
			this.panel1.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel2.Controls.Add(this.nudRateAsked);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.tbxTerm);
			this.panel2.Controls.Add(this.label7);
			this.panel2.Controls.Add(this.tbxDescription);
			this.panel2.Controls.Add(this.tbxStart);
			this.panel2.Controls.Add(this.cbxContact);
			this.panel2.Controls.Add(this.label6);
			this.panel2.Controls.Add(this.label4);
			this.panel2.Controls.Add(this.label5);
			this.panel2.Location = new System.Drawing.Point(12, 47);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(1771, 211);
			this.panel2.TabIndex = 1;
			// 
			// panel3
			// 
			this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
							| System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel3.Controls.Add(this.dataGridView1);
			this.panel3.Location = new System.Drawing.Point(12, 264);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(1771, 567);
			this.panel3.TabIndex = 2;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToOrderColumns = true;
			this.dataGridView1.AutoGenerateColumns = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.addedAtDataGridViewTextBoxColumn,
            this.lastActivityAtDataGridViewTextBoxColumn,
            this.companyDataGridViewTextBoxColumn,
            this.locationDataGridViewTextBoxColumn,
            this.rateAskedDataGridViewTextBoxColumn,
            this.Start,
            this.Term,
            this.agentCompanyDataGridViewTextBoxColumn,
            this.notesDataGridViewTextBoxColumn,
            this.OppId});
			this.dataGridView1.DataSource = this.oppContactViewBindingSource1;
			this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView1.Location = new System.Drawing.Point(0, 0);
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowTemplate.Height = 24;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(1767, 563);
			this.dataGridView1.TabIndex = 0;
			this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
			// 
			// addedAtDataGridViewTextBoxColumn
			// 
			this.addedAtDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.addedAtDataGridViewTextBoxColumn.DataPropertyName = "AddedAt";
			dataGridViewCellStyle4.Format = "dd-MMM-yy";
			dataGridViewCellStyle4.NullValue = null;
			this.addedAtDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
			this.addedAtDataGridViewTextBoxColumn.HeaderText = "AddedAt";
			this.addedAtDataGridViewTextBoxColumn.Name = "addedAtDataGridViewTextBoxColumn";
			this.addedAtDataGridViewTextBoxColumn.ReadOnly = true;
			this.addedAtDataGridViewTextBoxColumn.Width = 87;
			// 
			// lastActivityAtDataGridViewTextBoxColumn
			// 
			this.lastActivityAtDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.lastActivityAtDataGridViewTextBoxColumn.DataPropertyName = "LastActivityAt";
			dataGridViewCellStyle5.Format = "dd-MMM-yy HH:mm";
			this.lastActivityAtDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
			this.lastActivityAtDataGridViewTextBoxColumn.HeaderText = "LastActivityAt";
			this.lastActivityAtDataGridViewTextBoxColumn.Name = "lastActivityAtDataGridViewTextBoxColumn";
			this.lastActivityAtDataGridViewTextBoxColumn.ReadOnly = true;
			this.lastActivityAtDataGridViewTextBoxColumn.Width = 117;
			// 
			// companyDataGridViewTextBoxColumn
			// 
			this.companyDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.companyDataGridViewTextBoxColumn.DataPropertyName = "Company";
			this.companyDataGridViewTextBoxColumn.HeaderText = "Company";
			this.companyDataGridViewTextBoxColumn.Name = "companyDataGridViewTextBoxColumn";
			this.companyDataGridViewTextBoxColumn.ReadOnly = true;
			this.companyDataGridViewTextBoxColumn.Width = 92;
			// 
			// locationDataGridViewTextBoxColumn
			// 
			this.locationDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.locationDataGridViewTextBoxColumn.DataPropertyName = "Location";
			this.locationDataGridViewTextBoxColumn.HeaderText = "Location";
			this.locationDataGridViewTextBoxColumn.Name = "locationDataGridViewTextBoxColumn";
			this.locationDataGridViewTextBoxColumn.ReadOnly = true;
			this.locationDataGridViewTextBoxColumn.Width = 87;
			// 
			// rateAskedDataGridViewTextBoxColumn
			// 
			this.rateAskedDataGridViewTextBoxColumn.DataPropertyName = "RateAsked";
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.rateAskedDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
			this.rateAskedDataGridViewTextBoxColumn.FillWeight = 33F;
			this.rateAskedDataGridViewTextBoxColumn.HeaderText = "Rate";
			this.rateAskedDataGridViewTextBoxColumn.Name = "rateAskedDataGridViewTextBoxColumn";
			this.rateAskedDataGridViewTextBoxColumn.ReadOnly = true;
			this.rateAskedDataGridViewTextBoxColumn.Width = 44;
			// 
			// Start
			// 
			this.Start.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.Start.DataPropertyName = "Start";
			this.Start.FillWeight = 33F;
			this.Start.HeaderText = "Start";
			this.Start.Name = "Start";
			this.Start.ReadOnly = true;
			this.Start.Width = 63;
			// 
			// Term
			// 
			this.Term.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.Term.DataPropertyName = "Term";
			this.Term.FillWeight = 33F;
			this.Term.HeaderText = "Term";
			this.Term.Name = "Term";
			this.Term.ReadOnly = true;
			this.Term.Width = 66;
			// 
			// agentCompanyDataGridViewTextBoxColumn
			// 
			this.agentCompanyDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.agentCompanyDataGridViewTextBoxColumn.DataPropertyName = "AgentCompany";
			this.agentCompanyDataGridViewTextBoxColumn.HeaderText = "Agent[cy]";
			this.agentCompanyDataGridViewTextBoxColumn.Name = "agentCompanyDataGridViewTextBoxColumn";
			this.agentCompanyDataGridViewTextBoxColumn.ReadOnly = true;
			this.agentCompanyDataGridViewTextBoxColumn.Width = 92;
			// 
			// notesDataGridViewTextBoxColumn
			// 
			this.notesDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.notesDataGridViewTextBoxColumn.DataPropertyName = "Notes";
			this.notesDataGridViewTextBoxColumn.HeaderText = "Notes";
			this.notesDataGridViewTextBoxColumn.Name = "notesDataGridViewTextBoxColumn";
			this.notesDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// OppId
			// 
			this.OppId.DataPropertyName = "OppId";
			this.OppId.HeaderText = "OppId";
			this.OppId.Name = "OppId";
			this.OppId.ReadOnly = true;
			this.OppId.Visible = false;
			// 
			// oppContactViewBindingSource1
			// 
			this.oppContactViewBindingSource1.DataMember = "OppContactView";
			this.oppContactViewBindingSource1.DataSource = this.qStatsDataSet1;
			this.oppContactViewBindingSource1.Sort = "LastActivityAt Desc";
			// 
			// qStatsDataSet1
			// 
			this.qStatsDataSet1.DataSetName = "QStatsDataSet1";
			this.qStatsDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// oppContactViewTableAdapter1
			// 
			this.oppContactViewTableAdapter1.ClearBeforeFill = true;
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// btnUpdateLastActDate
			// 
			this.btnUpdateLastActDate.Location = new System.Drawing.Point(1021, 2);
			this.btnUpdateLastActDate.Name = "btnUpdateLastActDate";
			this.btnUpdateLastActDate.Size = new System.Drawing.Size(76, 25);
			this.btnUpdateLastActDate.TabIndex = 7;
			this.btnUpdateLastActDate.Text = "Up&Date";
			this.btnUpdateLastActDate.UseVisualStyleBackColor = true;
			this.btnUpdateLastActDate.Click += new System.EventHandler(this.btnUpdateLastActDate_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1795, 843);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "New Opportunity Entry Form";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.oppCompanyBindingSource1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.qStatsOppDataSet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.oppCompanyBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.oppLocationBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.oppContactForBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudRateAsked)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.oppContactViewBindingSource1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.qStatsDataSet1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnInsert;
		private System.Windows.Forms.TextBox tbxDescription;
		private System.Windows.Forms.ComboBox cbxCompany;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.BindingSource oppCompanyBindingSource;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cbxLocation;
		private QStatsOppDataSet qStatsOppDataSet;
		private System.Windows.Forms.BindingSource oppLocationBindingSource;
		private OppEntry.QStatsOppDataSetTableAdapters.OppLocationTableAdapter oppLocationTableAdapter;
		private System.Windows.Forms.BindingSource oppCompanyBindingSource1;
		private OppEntry.QStatsOppDataSetTableAdapters.OppCompanyTableAdapter oppCompanyTableAdapter1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cbxContact;
		private System.Windows.Forms.BindingSource oppContactForBindingSource;
		private OppEntry.QStatsOppDataSetTableAdapters.OppContactForTableAdapter oppContactForTableAdapter;
		private System.Windows.Forms.NumericUpDown nudRateAsked;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbxStart;
		private System.Windows.Forms.TextBox tbxTerm;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.DataGridView dataGridView1;
		private QStatsDataSet1 qStatsDataSet1;
		private System.Windows.Forms.BindingSource oppContactViewBindingSource1;
		private OppEntry.QStatsDataSet1TableAdapters.OppContactViewTableAdapter oppContactViewTableAdapter1;
		//private System.Windows.Forms.DataGridViewTextBoxColumn fNameDataGridViewTextBoxColumn;
		//private System.Windows.Forms.DataGridViewTextBoxColumn lNameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn addedAtDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn lastActivityAtDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn companyDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn locationDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn rateAskedDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn Start;
		private System.Windows.Forms.DataGridViewTextBoxColumn Term;
		private System.Windows.Forms.DataGridViewTextBoxColumn agentCompanyDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn notesDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn OppId;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.Button btnUpdateLastActDate;
	}
}

