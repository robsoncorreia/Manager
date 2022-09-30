using System;
using System.Collections.Generic;

namespace QRCodeGenerator.Model
{
    public class QRCodeModel
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<string> To { get; set; }
        public string From { get; set; }
    }
}