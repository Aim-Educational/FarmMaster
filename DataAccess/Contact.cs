using System.ComponentModel.DataAnnotations;

namespace DataAccess
{
    /// <summary>
    /// Describes what kind of contact they are.
    /// </summary>
    public enum ContactType
    {
        /// <summary>
        /// Fail-safe
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// An individual person. Please ensure consent is aquired before storing any information about them.
        /// </summary>
        Individual = 1,

        /// <summary>
        /// Some form of business entity, such as a shop or a company.
        /// </summary>
        BusinessEntity = 2
    }

    /// <summary>
    /// Represents a contact, whether it be a person or a company.
    /// </summary>
    public class Contact
    {
        public const int MAX_NAME_LENGTH = 75;
        public const int MAX_GENERIC_LENGTH = 75;

        [Key]
        public int ContactId { get; set; }

        [Required]
        [StringLength(MAX_NAME_LENGTH)]
        public string Name { get; set; }

        [Required]
        public ContactType Type { get; set; }

        [StringLength(MAX_GENERIC_LENGTH)]
        public string Email { get; set; }

        [StringLength(MAX_GENERIC_LENGTH)]
        public string Phone { get; set; }

        public int? NoteOwnerId { get; set; }
        public NoteOwner NoteOwner { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
