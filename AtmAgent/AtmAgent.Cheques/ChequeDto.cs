using System;
using System.Globalization;
using CafeLib.Core.Data;
using RepoDb.Attributes;

namespace AtmAgent.Cheques
{
    [Map("Cheque")]
    public class ChequeDto : IEntity
    {
        [Identity]
        public int Id { get; set; }

        public string ChequeId { get; set; }

        public string ChequeDate { get; set; }

        public string Atm { get; set; }

        public string Status { get; set; }

        public string MetadataFile { get; set; }

        public string FrontImageFile { get; set; }

        public string RearImageFile { get; set; }

        public string CreationDate { get; set; } = DateTime.Now.ToString(CultureInfo.InvariantCulture);

        public string LastUpdateDate { get; set; } = DateTime.Now.ToString(CultureInfo.InvariantCulture);

        public int IsDeleted { get; set; } = 0;
    }
}