using System;
using System.Collections.Generic;

namespace Farmarcas.Radar.Import.Models.Store
{
    public class StoreModel
    {
        public long Id { get; set; }
        public string CompanyName { get; set; }
        public string CNPJ { get; set; }

        public virtual StoreCommercialStrategy CommercialStrategy { get; set; } = new StoreCommercialStrategy();
        
    }
}
