using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Common
{
    public enum OrderStatuses
    {
        InProcessing = 0,
        OnWay = 1,
        Delivered = 2,
        Canceled = 3,
        Paid = 4,
    }
}
