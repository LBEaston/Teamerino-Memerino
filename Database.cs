﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Teamerino_Memerino
{
    class Database
    {
        static private Database instance = new Database();
        static public Database Instance { get { return instance; } }

        private List<InventoryStruct> itemlist = new List<InventoryStruct>();
        private List<SalesStruct> recordlist = new List<SalesStruct>();

        private BindingSource itemlistBinding = new BindingSource();
        private BindingSource recordlistBinding = new BindingSource();     

        //Load item and record
        private Database()
        {
            itemlistBinding.DataSource = typeof(InventoryStruct);
            recordlistBinding.DataSource = typeof(SalesStruct);
        }

        public List<InventoryStruct> ShowItem()
        {
            return itemlist;
        }

        public List<SalesStruct> ShowRecord()
        {
            return recordlist;
        }

        public void AddItem(InventoryStruct item)
        {
            itemlist.Add(item);
            itemlistBinding.Add(item);
        }

        public void RemoveItem(InventoryStruct item)
        {
            itemlist.Remove(item);
            itemlistBinding.Remove(item);
        }

        public void BindInventoryToDVG(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = true;
            dgv.DataSource = itemlistBinding;
        }

        public void BindInventoryToListBox(ListBox list)
        {
            list.DataSource = itemlistBinding;
        }

        public void BindSalesToDVG(DataGridView dgv)
        {
            dgv.DataSource = recordlistBinding;
        }

        public void EditItem()
        {

        }

        public void AddRecord(DataGridView dgv)
        {
            SalesStruct Record = new SalesStruct();

            //Sets the sales record information
            Record.RecordNum = recordlist.Count + 1;
            Record.Time = DateTime.Now.ToString("h:mm:ss");
            Record.Date = DateTime.Today.ToString("d");
            Record.Price = 0.00;

            //Sets the item quantity information
            foreach (DataGridViewRow row in dgv.Rows)
            {
                InventoryStruct theItem = FindItemByBarcorde(int.Parse(row.Cells[0].Value.ToString()));
                Record.Price += theItem.Price * int.Parse(row.Cells[2].Value.ToString());

                //Creates the item stock and adds it to the sales record
                SalesStockStruct tempStock = new SalesStockStruct();
                tempStock.Barcode = (int)row.Cells[0].Value;
                tempStock.Quantity = int.Parse(row.Cells[2].Value.ToString());
                Record.ItemQuantity.Add(tempStock);

                //Sets the quantity of the item accordingly
                theItem.Stock -= tempStock.Quantity;
                if (theItem.Stock < 0)
                {
                    MessageBox.Show("Warning: " + theItem.ItemName + " stock level has gone negative!");
                }
                else if (theItem.Stock < theItem.LowStockLevel)
                {
                    MessageBox.Show("Warning: " + theItem.ItemName + " stock level is running low!");
                }
            }

            recordlist.Add(Record);
            recordlistBinding.Add(Record);
        }

        public void RemoveRecord()
        {

        }

        public void EditRecord(DataGridView dgv, SalesStruct recordToEdit)
        {
            //Gets the record that needs to be edited
            int index = recordlist.FindIndex(x => x == recordToEdit);

            //Increases the stock of each item that was in the sales record
            foreach (SalesStockStruct stockToIncrease in recordlist[index].ItemQuantity)
            {
                InventoryStruct theItem = FindItemByBarcorde(stockToIncrease.Barcode);
                theItem.Stock += stockToIncrease.Quantity;
            }

            recordlist[index].ItemQuantity = new List<SalesStockStruct>();
            recordlist[index].Price = 0;

            //Adds in the new item stocks to the record
            foreach (DataGridViewRow row in dgv.Rows)
            {
                InventoryStruct theItem = FindItemByBarcorde(int.Parse(row.Cells[0].Value.ToString()));
                recordlist[index].Price += theItem.Price * int.Parse(row.Cells[2].Value.ToString());

                SalesStockStruct tempStock = new SalesStockStruct();
                tempStock.Barcode = (int)row.Cells[0].Value;
                tempStock.Quantity = int.Parse(row.Cells[2].Value.ToString());
                recordlist[index].ItemQuantity.Add(tempStock);

                //Sets the quantity of the item accordingly
                theItem.Stock -= tempStock.Quantity;
                if (theItem.Stock < 0)
                {
                    MessageBox.Show("Warning: " + theItem.ItemName + " stock level has gone negative!");
                }
                else if (theItem.Stock < theItem.LowStockLevel)
                {
                    MessageBox.Show("Warning: " + theItem.ItemName + " stock level is running low!");
                }
            }

            recordlistBinding[index] = recordlist[index];
        }

        //This method finds a item using a given barcode
        private InventoryStruct FindItemByBarcorde(int barcode)
        {
            InventoryStruct theItem = new InventoryStruct();

            foreach (InventoryStruct i in itemlist)
            {
                if (i.Barcode == barcode)
                {
                    theItem = i;
                    break;
                }
            }

            return theItem;
        }
        public void LoadItems()
        {
            List<string[]> rows = System.IO.File.ReadAllLines("...//...//Resources//Items//inventory.txt").Select(x => x.Split(',')).ToList();
            rows.ForEach(x => {
                InventoryStruct loadItem = new InventoryStruct();
                int c = 0;
                double y = 0;
                Int32.TryParse(x[0], out c);
                loadItem.Barcode = c;
                loadItem.ItemName = x[1];
                Int32.TryParse(x[2], out c);
                loadItem.Stock = c;
                Int32.TryParse(x[3], out c);
                loadItem.LowStockLevel = c;
                double.TryParse(x[4], out y);
                loadItem.Price = y;
                Database.Instance.AddItem(loadItem);
            });
        }
        public void LoadRecords()
        {
            List<string[]> rows = System.IO.File.ReadAllLines("...//...//Resources//Records//sales.txt").Select(x => x.Split(',')).ToList();
            rows.ForEach(x => {
                SalesStruct loadRecord = new SalesStruct();
                SalesStockStruct loadQuantity = new SalesStockStruct();
                int c = 0;
                double y = 0;
                int z = 4;
                List<int> v = new List<int> { };
                Int32.TryParse(x[0], out c);
                loadRecord.RecordNum = c;
                Double.TryParse(x[1], out y);
                loadRecord.Price = y;
                loadRecord.Time = x[2];
                loadRecord.Date = x[3];
                //NeedQuantity
                foreach (SalesStockStruct item in loadRecord.ItemQuantity)
                {
                    Int32.TryParse(x[z], out c);
                    item.Barcode = c;
                    Int32.TryParse(x[z + 1], out c);
                    item.Quantity = c;
                }
                recordlist.Add(loadRecord);
                recordlistBinding.Add(loadRecord);
            });
        }
        public void WriteSales()
        {
            string line = "";
            string[] saveText = new string[recordlist.Count];
            int c = 0;
           // System.IO.File.WriteAllText("...//...//Resources//Records//sales.txt", "");
            foreach (SalesStruct sale in recordlist)
            {
                int i = 0;
                line += sale.RecordNum.ToString();
                line += ",";
                line += sale.Price.ToString();
                line += ",";
                line += sale.Time.ToString();
                line += ",";
                line += sale.Date.ToString();
                line += ",";
                while (i < sale.ItemQuantity.Count)
                {
                    line += sale.ItemQuantity[i].Barcode.ToString();
                    line += ",";
                    line += sale.ItemQuantity[i].Quantity.ToString();
                    line += ",";
                    i++;
                }
                line.TrimEnd(',');
                saveText[c] = line;
                line = "";
                c++;
            }
            System.IO.File.WriteAllLines("...//...//Resources//Records//sales.txt", saveText);
        }
        public void WriteItems(string path, DataGridView dgv)
        {
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dgv.SelectAll();
            DataObject toSave = dgv.GetClipboardContent();
            System.IO.File.WriteAllText(path, toSave.GetText(TextDataFormat.CommaSeparatedValue));
        }
    }
}
