﻿using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using AttachmentCollection = ActionMailerNext.Utils.AttachmentCollection;

namespace ActionMailerNext.Interfaces
{
    /// <summary>
    ///     All mailers should implement this interface.
    /// </summary>
    public interface IMailAttributes
    {
        /// <summary>
        ///     A string representation of who this mail should be from.  Could be
        ///     your name and email address or just an email address by itself.
        /// </summary>
        MailAddress From { get; set; }

        /// <summary>
        ///     The subject line of the email.
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        ///     The Priority of the email.
        /// </summary>
        MailPriority Priority { get; set; }

        /// <summary>
        ///     A collection of addresses this email should be sent to.
        /// </summary>
        List<MailAddress> To { get; }

        /// <summary>
        ///     A collection of addresses that should be CC'ed.
        /// </summary>
        IList<MailAddress> Cc { get; }

        /// <summary>
        ///     A collection of addresses that should be BCC'ed.
        /// </summary>
        IList<MailAddress> Bcc { get; }

        /// <summary>
        ///     A collection of addresses that should be listed in Reply-To header.
        /// </summary>
        List<MailAddress> ReplyTo { get; }

        /// <summary>
        ///     Any custom headers (name and value) that should be placed on the message.
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        ///     Gets or sets the default message encoding when delivering mail.
        /// </summary>
        Encoding MessageEncoding { get; set; }

        /// <summary>
        ///     Any attachments you wish to add.  The key of this collection is what
        ///     the file should be named.  The value is should represent the actual content
        ///     of the file.
        /// </summary>
        /// <example>
        ///     Attachments["picture.jpg"] = File.ReadAllBytes(@"C:\picture.jpg");
        /// </example>
        AttachmentCollection Attachments { get; }

        /// <summary>
        ///     Any view you wish to add.  The key of this collection is what
        ///     the view should be named.
        /// </summary>
        IList<AlternateView> AlternateViews { get; }
    }
}