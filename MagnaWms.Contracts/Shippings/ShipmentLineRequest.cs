using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagnaWms.Contracts.Shippings;
public sealed record ShipmentLineRequest(
    long ItemId,
    decimal Quantity
);
