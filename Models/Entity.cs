using Microsoft.AspNetCore.Mvc;
namespace WebReader.Models
{
    public abstract class Entity
    {
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime InsertTimestamp { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime UpdateTimestamp { get; set; }

        public bool Active { get; set; }

        public Entity() => Id = Guid.NewGuid();
    }
}