using InventorySystem.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sapiox.API
{
    public class Item
    {
        public GameObject GameObject { get; internal set; }
        public ItemType Type { get; set; }
        public Item()
        {
        }
    }
}
