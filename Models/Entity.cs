namespace WebReader.Models
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTime InsertTimestamp { get; set; }
        public DateTime UpdateTimestamp { get; set; }
        public bool Active { get; set; }

        public Entity() => Id = Guid.NewGuid();
    }
}