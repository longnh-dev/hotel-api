using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain   
{
    [Table("Right")]
    public class Right
    { 
        public Right()
        {
            RightsRole = new HashSet<RightMapRole>();
            RightsOfUser = new HashSet<RightMapUser>();
        }
    
        [Key]
        public Guid Id { get; set; }
    
        [Required]
        [StringLength(64)]
        public string Code { get; set; }
    
        [Required]
        [StringLength(128)]
        public string Name { get; set; }
    
        [StringLength(1024)]
        public string Description { get; set; }
    
        [StringLength(64)]
        public string GroupCode { get; set; }
    
        public bool Status { get; set; }
        public int Order { get; set; }
        public bool IsSystem { get; set; }
        public ICollection<RightMapRole> RightsRole { get; set; }
        public ICollection<RightMapUser> RightsOfUser { get; set; }
        
    }
}
