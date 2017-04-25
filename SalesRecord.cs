﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teamerino_Memerino
{
    public class SalesRecord
    {
            
        private int _recnum;
        private double _price;
        private string _date;
        private string _time;
        //index int refers to barcode, value int refers to quantity
        private List<SalesRecordItem> _items = new List<SalesRecordItem>();

        public int RecordNum
        {
            get
            {
                return _recnum;
            }
            set
            {
                _recnum = value;
            }
        }

        public double Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
            }
        }

        public string Time
        {
            get
            {
                return _time;
            }

            set
            {
                _time = value;
            }
        }
        public string Date
        {
            get
            {
                return _date;
            }

            set
            {
                _date = value;
            }
        }

        public List<SalesRecordItem> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }

    }

   
}