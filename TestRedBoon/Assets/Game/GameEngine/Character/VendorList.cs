using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.Character
{
    [Serializable]
    public class VendorList
    {
        [SerializeField] private List<Vendor> _listVendors = new List<Vendor>();

        public void AddVendor(Vendor vendor)
        {
            _listVendors.Add(vendor);
        }

        public List<Vendor> GetListVendors() => _listVendors;

        public override string ToString() => _listVendors.Count.ToString();
    } 
}
