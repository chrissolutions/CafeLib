using System;
using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Dto
{
    public class EntityBase<T> : IEntity
    {
        [Key]
        public T Id { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
