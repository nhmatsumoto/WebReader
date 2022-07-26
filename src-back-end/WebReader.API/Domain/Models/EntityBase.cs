using Microsoft.AspNetCore.Mvc;

namespace WebReader.API.Domain.Models
{
    public abstract class EntityBase
    {
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime InsertTimestamp { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime UpdateTimestamp { get; set; }

        public bool Active { get; set; }

        public EntityBase() => Id = Guid.NewGuid();
    }
}
