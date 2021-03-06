﻿using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ActionMailerNext.Interfaces;
using ActionMailerNext.Utils;

namespace ActionMailerNext.Mvc5_1
{
    /// <summary>
    ///     A special result that should be returned from each eaction in your
    ///     mail controller.  Your controller must inherit from MailerBase.
    /// </summary>
    public class EmailResult : ViewResult, IEmailResult
    {
        private readonly DeliveryHelper _deliveryHelper;
        private readonly IMailInterceptor _interceptor;
        private readonly IMailAttributes _mail;
        private readonly IMailSender _mailSender;
        private readonly Encoding _messageEncoding;
        private readonly bool _trimBody;

        private IView _htmlView;
        private string _htmlViewName;

        private IView _textView;
        private string _textViewName;

        /// <summary>
        ///     Creates a new EmailResult.  You must call ExecuteCore() before this result
        ///     can be successfully delivered.
        /// </summary>
        /// <param name="interceptor">The IMailInterceptor that we will call when delivering mail.</param>
        /// <param name="sender">The IMailSender that we will use to send mail.</param>
        /// <param name="mail">The mail messageBase who's body needs populating.</param>
        /// <param name="viewName">The view to use when rendering the messageBase body (can be null)</param>
        /// <param name="masterName">The maste rpage to use when rendering the messageBase body (can be null)</param>
        /// <param name="messageEncoding">The encoding to use when rendering a messageBase.</param>
        /// <param name="trimBody">Whether or not we should trim whitespace from the beginning and end of the messageBase body.</param>
        public EmailResult(IMailInterceptor interceptor, IMailSender sender, IMailAttributes mail, string viewName,
            string masterName, Encoding messageEncoding, bool trimBody)
        {
            if (interceptor == null)
                throw new ArgumentNullException("interceptor");

            if (sender == null)
                throw new ArgumentNullException("sender");

            if (mail == null)
                throw new ArgumentNullException("mail");

            ViewName = viewName ?? ViewName;
            MasterName = masterName ?? MasterName;
            _messageEncoding = messageEncoding;
            _mail = mail;
            _mailSender = sender;
            _interceptor = interceptor;
            _deliveryHelper = new DeliveryHelper(_mailSender, _interceptor);
            _trimBody = trimBody;
        }

        /// <summary>
        ///     The underlying MailMessage object that was passed to this object's constructor.
        /// </summary>
        public IMailAttributes Mail
        {
            get { return _mail; }
        }

        /// <summary>
        ///     The IMailSender instance that is used to deliver mail.
        /// </summary>
        public IMailSender MailSender
        {
            get { return _mailSender; }
        }

        /// <summary>
        ///     The default encoding used to send a messageBase.
        /// </summary>
        public Encoding MessageEncoding
        {
            get { return _messageEncoding; }
        }

        /// <summary>
        ///     Sends your messageBase.  This call will block while the messageBase is being sent. (not recommended)
        /// </summary>
        public void Deliver()
        {
            _deliveryHelper.Deliver(Mail);
        }

        /// <summary>
        ///     Sends your messageBase asynchronously.  This method does not block.  If you need to know
        ///     when the messageBase has been sent, then override the OnMailSent method in MailerBase which
        ///     will not fire until the asyonchronous send operation is complete.
        /// </summary>
        public async Task<IMailAttributes> DeliverAsync()
        {
            Task<IMailAttributes> deliverTask = _deliveryHelper.DeliverAsync(Mail);
            return await deliverTask;
        }

        /// <summary>
        ///     Causes the body of the mail messageBase to be generated.
        /// </summary>
        /// <param name="context">The controller context to use while rendering the body.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            AddMessageViews(context);
        }

        private void LocateViews(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (string.IsNullOrEmpty(ViewName))
                ViewName = context.RouteData.GetRequiredString("action");

            _htmlViewName = String.Format("{0}.html", ViewName);
            _textViewName = String.Format("{0}.txt", ViewName);

            ViewEngineResult htmlViewResult = ViewEngines.Engines.FindView(context, _htmlViewName, MasterName);
            if (htmlViewResult.View != null)
            {
                _htmlView = htmlViewResult.View;
            }

            ViewEngineResult textViewResult = ViewEngines.Engines.FindView(context, _textViewName, MasterName);
            if (textViewResult.View != null)
            {
                _textView = textViewResult.View;
            }
        }

        private string RenderViewAsString(ControllerContext context, IView view)
        {
            using (var writer = new StringWriter())
            {
                var viewContext = new ViewContext(context, view, ViewData, TempData, writer);
                view.Render(viewContext, writer);

                string output = writer.GetStringBuilder().ToString();
                if (_trimBody)
                    output = output.Trim();

                return output;
            }
        }

        private void AddMessageViews(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            LocateViews(context);

            if (_textView == null && _htmlView == null)
            {
                string message =
                    String.Format(
                        "You must provide a view for this email.  Views should be named {0}.txt.cshtml or {1}.html.cshtml (or aspx for WebFormsViewEngine) depending on the format you wish to render.",
                        ViewName, ViewName);
                throw new NoViewsFoundException(message);
            }

            if (_textView != null)
            {
                string body = RenderViewAsString(context, _textView);
                AlternateView altView = AlternateView.CreateAlternateViewFromString(body,
                    MessageEncoding ?? Encoding.Default, MediaTypeNames.Text.Plain);
                Mail.AlternateViews.Add(altView);
            }

            if (_htmlView != null)
            {
                string body = RenderViewAsString(context, _htmlView);
                AlternateView altView = AlternateView.CreateAlternateViewFromString(body,
                    MessageEncoding ?? Encoding.Default, MediaTypeNames.Text.Html);
                Mail.AlternateViews.Add(altView);
            }
        }
    }
}