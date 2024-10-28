using DokoTelegramService.Logic;
using System.Collections.Generic;
using System.Windows.Forms;
using ToolKit.Core.UI.Tools;

namespace DokoTelegramService
{
    public partial class Form1 : Form
    {
        private Service Service { get; set; }

        public Form1()
        {
            InitializeComponent();

            Service = new Service();

            Session.SessionListChanged += RefreshSessionsList;
            Session.SessionChanged += (s) => refresh(null, null);

            Service.Start();
        }

        private void RefreshSessionsList()
        {
            //var sessionsBindingSource = new BindingSource();
            //sessionsBindingSource.DataSource = Session.Sessions;

            sessionsGridView.ThreadSafe(() =>
            {
                sessionsGridView.DataSource = Session.Sessions;
                sessionsGridView.Refresh();
            });

            //var bindingList = new SortableBindingList<ItemHistoryEntry>(data);

            //var bindingSource = new BindingSource(bindingList, null);

            ////_itemsBindingSource.ListChanged += itemsBindingSource_ListChanged;
            //this.ThreadSafe(() =>
            //{
            //    this.DataSource = bindingSource;

            //    Sort(this.Columns[nameof(ItemHistoryEntry.Day)], ListSortDirection.Ascending);
            //});
        }

        private void refresh(object sender, System.EventArgs e)
        {
            sessionsGridView.ThreadSafe(() =>
            {
                //sessionsGridView.DataSource = Session.Sessions;
                sessionsGridView.Refresh();
            });
        }
    }
}