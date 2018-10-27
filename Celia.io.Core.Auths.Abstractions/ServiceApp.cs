using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Celia.io.Core.Auths.Abstractions
{
    public class ServiceApp
    {
        [Key]
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        //
        // Summary:
        //     Gets or sets a flag indicating if the user could be locked out.
        public virtual bool LockoutEnabled { get; set; }
        //
        // Summary:
        //     Gets or sets the number of failed login attempts for the current user.
        public virtual int AccessFailedCount { get; set; }
        //
        // Summary:
        //     Gets or sets the date and time, in UTC, when any user lockout ends.
        //
        // Remarks:
        //     A value in the past means the user is not locked out.
        public virtual DateTimeOffset? LockoutEnd { get; set; }
    }
}
