using System;
using CafeLib.Core.Extensions;
using CafeLib.Data.Mapping;
// ReSharper disable UnusedMember.Global

namespace AtmAgent.Cheques
{
    public class Cheque : MappedEntity<Cheque, ChequeDto>
    {
        public Cheque()
        {
            Map(p => p.Status).Convert<ChequeStatus>(o => o.Humanize());
            Map(p => p.Status).Convert<string, ChequeStatus>(Enum.Parse<ChequeStatus>);
        }

        public int Id { get; set; }

        public string ChequeId { get; set; }

        public string ChequeDate { get; set; }

        public string Atm { get; set; }

        public ChequeStatus Status { get; set; } = ChequeStatus.None;

        public string MetadataFile { get; set; }

        public string FrontImageFile { get; set; }

        public string RearImageFile { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public DateTime LastUpdateDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;
    }
}