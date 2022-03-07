using System;

namespace Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        protected AuditableEntity()
        {
        }

        protected AuditableEntity(string createdBy, DateTime created)
        {
            CreatedBy = createdBy;
            Created = created;
        }

        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
