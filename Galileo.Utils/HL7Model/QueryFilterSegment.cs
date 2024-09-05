using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils.HL7Model
{
    public class QueryFilterSegment
    {
        public string WhereSubjectFilter { get; set; } // Where Subject Filter
        public string WhenDataStartDateTime { get; set; } // When Data Start Date/Time
        public string WhenDataEndDateTime { get; set; } // When Data End Date/Time
        public string WhatUserQualifier { get; set; } // What User Qualifier
        public string OtherQRYSubjectFilter { get; set; } // Other QRY Subject Filter
        public string WhichDateTimeQualifier { get; set; } // Which Date/Time Qualifier
        public string WhichDateTimeStatusQualifier { get; set; } // Which Date/Time Status Qualifier
        public string DateTimeSelectionQualifier { get; set; } // Date/Time Selection Qualifier
        public string WhenQuantityTimingQualifier { get; set; } // When Quantity/Timing Qualifier


        public string Serializar()
        {
            return $"QRF|{WhereSubjectFilter}|{WhenDataStartDateTime}|{WhenDataEndDateTime}|{WhatUserQualifier}|" +
                   $"{OtherQRYSubjectFilter}|{WhichDateTimeQualifier}|{WhichDateTimeStatusQualifier}|" +
                   $"{DateTimeSelectionQualifier}|{WhenQuantityTimingQualifier}|" + char.ConvertFromUtf32(13);
        }

    }
}
