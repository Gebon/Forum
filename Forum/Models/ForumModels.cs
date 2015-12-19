using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Board
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Board name must be less than 100 characters", MinimumLength = 6)]
        [Display(Name = "Название раздела")]
        public string Name { get; set; }
        [StringLength(2000, ErrorMessage = "Short description of Board theme must be less than 2000 characters")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        public virtual ICollection<ForumThread> Threads { get; set; } 
    }

    public class ForumThread
    {
        public int Id { get; set; }
        [Required]
        [StringLength(80, ErrorMessage = "Thread name must be less than 80 characters", MinimumLength = 6)]
        [Display(Name = "Название темы")]
        public string Name { get; set; }
        [StringLength(2000, ErrorMessage = "Short description of thread must be less than 2000 characters")]
        [Display(Name = "Описание темы")]
        public string Description { get; set; }
        public virtual Board Board { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<Message> Messages { get; set; } 
    }

    public class Message
    {
        public int Id { get; set; }
        [Required]
        [StringLength(4000, ErrorMessage = "Message must be less than 4000 characters")]
        public string Content { get; set; }
        public DateTime PublicationTime { get; set; }
        public virtual ForumThread Thread { get; set; }
        public virtual ApplicationUser Owner { get; set; }
    }
}