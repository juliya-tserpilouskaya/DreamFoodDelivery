using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Common
{
    public class NumberСonstants
    {
        public const double FREE_DELIVERY_BORDER = 100; //$
        public const double DELIVERY_PRICE  = 25; //$
        public const double TIME_TO_CHANGE_ORDER_IN_MINUTES = 15;
        public const int IMAGE_SIZE = 204800; //in byte == 200 kB
        public const int TOKEN_TIME_HOUR = 1;
        public const int LOGGER_FILE_SIZE = 30_720;
        public const int REFRESH = 50;
        //public const int REFRESH = 3540;
        public const int DAYS_TO_EXPIRE = 1;
    }
}
