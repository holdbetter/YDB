using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YDB.Models;
using Xamarin.Forms;

namespace YDB.Views
{
    public class DataObjectCreate : ContentView
    {
        StackLayout main;
        public int Index { get; set; }

        public DataObjectCreate(DbMenuListModel model, int index)
        {
            this.BindingContext = model;
            Index = index;

            main = new StackLayout();

            foreach (var item in model.DatabaseData.Data)
            {
                TableItemOnAdd tableItem = new TableItemOnAdd(item, index);
                main.Children.Add(tableItem);
            }

            ScrollView scr = new ScrollView()
            {
                Content = main
            };

            Content = scr;
        }
    }
}