﻿using System;
using System.Web.Mvc;

namespace ActionMailer.Net.Mvc5_1.Tests.Mvc5_1
{
    public class TextViewEngine : IViewEngine
    {
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName,
            bool useCache)
        {
            throw new NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName,
            bool useCache)
        {
            if (viewName.Contains("txt"))
                return new ViewEngineResult(new TextView(), this);

            return new ViewEngineResult(new[] {""});
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new NotImplementedException();
        }
    }
}